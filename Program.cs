using FolderSyncronizer;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 4)
        {
            Console.WriteLine("Usage: <sourceFolder> <replicaFolder> <intervalInSeconds> <logFilePath>");
            return;
        }

        string sourceFolder = args[0];
        string replicaFolder = args[1];
        int intervalInSeconds = int.Parse(args[2]);
        string logFilePath = args[3];

        var synchronizer = new FolderSynchronizer(sourceFolder, replicaFolder, intervalInSeconds, logFilePath);
        synchronizer.Start();
    }
}