using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Timers;
using Timer = System.Timers.Timer;


namespace FolderSyncronizer
{
    public class FolderSynchronizer
    {
        private readonly string sourceFolder;
        private readonly string replicaFolder;
        private readonly int intervalInSeconds;
        private readonly string logFilePath;
        private readonly Timer timer;

        public FolderSynchronizer(string sourceFolder, string replicaFolder, int intervalInSeconds, string logFilePath)
        {
            this.sourceFolder = sourceFolder;
            this.replicaFolder = replicaFolder;
            this.intervalInSeconds = intervalInSeconds;
            this.logFilePath = logFilePath;

            timer = new Timer(intervalInSeconds * 1000);
            timer.Elapsed += (sender, e) => SynchronizeFolders();
        }

        public void Start()
        {
            SynchronizeFolders();
            timer.Start();
            Console.WriteLine($"Started folder synchronization every {intervalInSeconds} seconds. ");
            // I tried using Console.Writeline() to keep the program running, but apparently it didnt work
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }

        private void SynchronizeFolders()
        {
            try
            {
                var sourceFiles = Directory.GetFiles(sourceFolder, "*", SearchOption.AllDirectories)
                                                    .Select(func => func.Substring(sourceFolder.Length + 1)).ToList();
                var replicaFiles = Directory.GetFiles(replicaFolder, "*", SearchOption.AllDirectories)
                                                    .Select(func => func.Substring(replicaFolder.Length + 1)).ToList();


                // THIS IS WHERE WE COPY OR UPDATE THE FILES
                foreach (var file in sourceFiles)
                {
                    var sourceFilePath = Path.Combine(sourceFolder, file);
                    var replicaFilePath = Path.Combine(replicaFolder, file);

                    if (!File.Exists(replicaFilePath) || !FilesAreEqual(sourceFilePath, replicaFilePath))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(replicaFilePath));
                        File.Copy(sourceFilePath, replicaFilePath, true);
                        Log($"Copied / Updated File : {file}");
                    }

                }

                // THIS IS WHERE WE DELETE THE FILES IF NEED BE
                foreach (var file in replicaFiles.Except(sourceFiles))
                {
                    var replicaFilePath = Path.Combine(replicaFolder, file);
                    File.Delete(replicaFilePath);
                    Log($"Deleted File : {file}");
                }

                Console.WriteLine("The Synchronization is complete! ");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during synchronization: {ex.Message}");
            }
        }

        private bool FilesAreEqual(string firstFile, string secondFile)
        {
            using (var hashAlgorithm = MD5.Create())
            {
                byte[] firstFile_Hash = hashAlgorithm.ComputeHash(File.ReadAllBytes(firstFile));
                byte[] secondFile_Hash = hashAlgorithm.ComputeHash(File.ReadAllBytes(secondFile));
                return firstFile_Hash.SequenceEqual(secondFile_Hash);
            }
        }

        private void Log(string logMessage)
        {
            using (var writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine(logMessage);
            }
            Console.WriteLine(logMessage);
        }
    }
}
