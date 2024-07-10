using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace FileHandling
{
    class SynchFolder
    {
        private readonly string sourcePath;
        private readonly string replicaPath;
        private readonly int interval;
        private readonly string logFilePath;
        private Timer syncTimer;

        public SynchFolder(string sourcePath, string replicaPath, int interval, string logFilePath)
        {
            this.sourcePath = sourcePath;
            this.replicaPath = replicaPath;
            this.interval = interval;
            this.logFilePath = logFilePath;
        }

        public void Start()
        {
            syncTimer = new Timer(Synchronize, null, 0, interval * 1000);
        }

        private void Synchronize(object state)
        {
            try
            {
                Log("Synchronization started.");
                SynchronizeDirectories(sourcePath, replicaPath);
                Log("Synchronization completed.");
            }
            catch (Exception ex)
            {
                Log($"Error during synchronization: {ex.Message}");
            }
        }

        private void SynchronizeDirectories(string source, string target)
        {
            var sourceDir = new DirectoryInfo(source);
            var targetDir = new DirectoryInfo(target);

            if (!targetDir.Exists)
            {
                targetDir.Create();
                Log($"Created directory: {targetDir.FullName}");
            }

            // Sync files
            foreach (var sourceFile in sourceDir.GetFiles())
            {
                var targetFile = new FileInfo(Path.Combine(targetDir.FullName, sourceFile.Name));

                if (!targetFile.Exists || !FilesAreEqual(sourceFile, targetFile))
                {
                    sourceFile.CopyTo(targetFile.FullName, true);
                    Log($"Copied file: {sourceFile.FullName} to {targetFile.FullName}");
                }
            }

            // Delete files not in source
            foreach (var targetFile in targetDir.GetFiles())
            {
                var sourceFile = new FileInfo(Path.Combine(sourceDir.FullName, targetFile.Name));

                if (!sourceFile.Exists)
                {
                    targetFile.Delete();
                    Log($"Deleted file: {targetFile.FullName}");
                }
            }

            // Recursively sync directories
            foreach (var sourceSubDir in sourceDir.GetDirectories())
            {
                var targetSubDir = new DirectoryInfo(Path.Combine(targetDir.FullName, sourceSubDir.Name));
                SynchronizeDirectories(sourceSubDir.FullName, targetSubDir.FullName);
            }

            // Delete directories not in source
            foreach (var targetSubDir in targetDir.GetDirectories())
            {
                var sourceSubDir = new DirectoryInfo(Path.Combine(sourceDir.FullName, targetSubDir.Name));

                if (!sourceSubDir.Exists)
                {
                    targetSubDir.Delete(true);
                    Log($"Deleted directory: {targetSubDir.FullName}");
                }
            }
        }

        private bool FilesAreEqual(FileInfo file1, FileInfo file2)
        {
            using (var hashAlg = MD5.Create())
            {
                var file1Hash = BitConverter.ToString(hashAlg.ComputeHash(file1.OpenRead()));
                var file2Hash = BitConverter.ToString(hashAlg.ComputeHash(file2.OpenRead()));

                return file1Hash == file2Hash;
            }
        }

        private void Log(string message)
        {
            var logMessage = $"{DateTime.Now}: {message}";
            Console.WriteLine(logMessage);
            File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
        }

        public void Stop()
        {
            syncTimer?.Change(Timeout.Infinite, 0);
        }
    }
}

