using Hashing;

namespace FMCS
{
    class Program 
    {
        public const string TargetDir = "./target_test_folder";
        private const double DetectionInterval = 5000; // 5000 milliseconds = 5 seconds
        private static System.Timers.Timer? detectionTimer;
        public static HashingAlgorithm Hasher = new MD5();

        static void Main(string[] args)
        {
            try
            {
                Initialize();

                // Set up a timer to run the detection every 5 seconds
                detectionTimer = new System.Timers.Timer(DetectionInterval);
                detectionTimer.Elapsed += (sender, e) => RunDetection(false);
                detectionTimer.AutoReset = true;
                detectionTimer.Enabled = true;

                // Keep the application running and handle terminal input
                CommandHandler commandHandler = new CommandHandler(Hasher);
                commandHandler.PrintInitialPrompt();
                string? command;
                while ((command = Console.ReadLine()) != null)
                {
                    commandHandler.HandleCommand(command);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        private static void Initialize()
        {
            // Initialize the directory and hash list file
            RuntimeDirectoryManagement.InitializeDirectory(TargetDir);

            // Generate initial hashes and store them if the hash list file does not exist
            if (!File.Exists(Path.Combine(TargetDir, RuntimeDirectoryManagement.DirName, RuntimeDirectoryManagement.HashListFileName)))
            {
                List<string> filePaths = FileHandler.GetFilePaths(TargetDir);
                FileHandler.GenerateInitialHashes(filePaths, Hasher, TargetDir);
            }
        }

        public static void RunDetection(bool isManual)
        {
            try
            {
                if (isManual)
                {
                    Console.WriteLine("Running detection...");
                }

                List<string> filePaths = FileHandler.GetFilePaths(TargetDir);
                List<string> changes = FileHandler.DetectChanges(filePaths, Hasher, TargetDir, isManual);

                if (!isManual && changes.Count > 0)
                {
                    Console.WriteLine("Changes detected:");
                    foreach (var change in changes)
                    {
                        Console.WriteLine(change);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred during detection: " + ex.Message);
            }
        }
    }
}