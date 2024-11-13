namespace FMCS
{
    class RuntimeDirectoryManagement
    {
        public const string DirName = ".runtimedir";
        public const string HashListFileName = "hashlist";

        public static void CreateDirectory(string directoryPath)
        {
            // Check if the directory already exists
            if (Directory.Exists(directoryPath))
            {
                Console.WriteLine("The specified directory already exists: " + directoryPath);
                return;
            }

            // Create the directory
            Directory.CreateDirectory(directoryPath);
            Console.WriteLine("Directory created: " + directoryPath);
        }

        public static void DeleteDirectory(string directoryPath)
        {
            // Check if the directory exists
            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException("The specified directory was not found.");
            }

            // Delete the directory
            Directory.Delete(directoryPath, true);
        }

        public static void CreateHashListFile(string directoryPath, string hashListFilePath)
        {
            // Check if the directory exists
            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException("The specified directory was not found.");
            }

            // Check if the hash list file already exists
            if (File.Exists(hashListFilePath))
            {
                Console.WriteLine("The hash list file already exists: " + hashListFilePath);
                return;
            }

            // Create the hash list file
            File.Create(hashListFilePath).Close();
        }

        public static void InitializeDirectory(string targetDir)
        {
            string runtimeDirPath = Path.Combine(targetDir, DirName);
            CreateDirectory(runtimeDirPath);
            CreateHashListFile(runtimeDirPath, Path.Combine(runtimeDirPath, HashListFileName));
        }
    }
}