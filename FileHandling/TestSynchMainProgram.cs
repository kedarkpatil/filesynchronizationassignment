using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileHandling
{
    class TestSynchMainProgram
    {
        static void Main(string[] args)
        {

            if (args.Length < 4)
            {
                Console.WriteLine("Usage: FolderSynchronizer.exe <sourcePath> <replicaPath> <intervalInSeconds> <logFilePath>");
                return;
            }
            var sourcePath = args[0];
            var replicaPath = args[1];
            var interval = int.Parse(args[2]);
            var logFilePath = args[3];

            /*    var sourcePath = "C:\\Users\\kedar\\Desktop\\src";
                var replicaPath = "C:\\Users\\kedar\\Desktop\\replica";
                var interval = int.Parse("60");
                var logFilePath = "C:\\Users\\kedar\\Desktop\\log.txt";*/

            var synchronizer = new SynchFolder(sourcePath, replicaPath, interval, logFilePath);
            synchronizer.Start();

            Console.WriteLine("Press Enter Key from Keyboard to exit.");
            Console.ReadLine();

            synchronizer.Stop();

        }
    }
}