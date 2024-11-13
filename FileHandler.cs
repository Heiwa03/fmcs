
using Hashing;

namespace FMCS
{
    class FileHandler
    {
        // Method to read a file and return its contents as a byte array
        public static byte[] ReadFileAsByteArray(string filePath)
        {
            // Check if the file exists
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified file was not found.", filePath);
            }

            // Read the file contents into a byte array
            return File.ReadAllBytes(filePath);
        }

        // Method to read a file and return its contents as a string
        public static string ReadFileAsString(string filePath)
        {
            // Check if the file exists
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified file was not found.", filePath);
            }

            // Read the file contents into a string
            return File.ReadAllText(filePath);
        }

        // Method to get all file paths in a specified folder
        public static List<string> GetFilePaths(string folderPath)
        {
            // Check if the folder exists
            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException("The specified folder was not found.");
            }

            // Get all file paths in the folder
            string[] filePaths = Directory.GetFiles(folderPath);
            return new List<string>(filePaths);
        }

        // Method to generate initial hashes and store them in the hash list file
        public static void GenerateInitialHashes(List<string> filePaths, HashingAlgorithm hasher, string targetDir)
        {
            string hashListFilePath = Path.Combine(targetDir, RuntimeDirectoryManagement.DirName, RuntimeDirectoryManagement.HashListFileName);
            using (StreamWriter writer = new StreamWriter(hashListFilePath))
            {
                foreach (string filePath in filePaths)
                {
                    byte[] fileContents = ReadFileAsByteArray(filePath);
                    string hash = hasher.Hash(fileContents);
                    writer.WriteLine($"{filePath}:{hash}");
                }
            }
        }

        // Method to detect changes in the directory
        public static List<string> DetectChanges(List<string> filePaths, HashingAlgorithm hasher, string targetDir, bool isManual)
        {
            string hashListFilePath = Path.Combine(targetDir, RuntimeDirectoryManagement.DirName, RuntimeDirectoryManagement.HashListFileName);
            Dictionary<string, string> storedHashes = new Dictionary<string, string>();
            List<string> changes = new List<string>();

            // Read the stored hashes from the hash list file
            using (StreamReader reader = new StreamReader(hashListFilePath))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(':');
                    if (parts.Length == 2)
                    {
                        storedHashes[parts[0]] = parts[1];
                    }
                }
            }

            // Refresh the list of file paths
            List<string> currentFilePaths = GetFilePaths(targetDir);

            // Compute the current hashes and compare with the stored hashes
            foreach (string filePath in currentFilePaths)
            {
                byte[] fileContents = ReadFileAsByteArray(filePath);
                string currentHash = hasher.Hash(fileContents);

                if (storedHashes.ContainsKey(filePath))
                {
                    if (storedHashes[filePath] != currentHash)
                    {
                        AddChange(changes, isManual, "File changed: " + filePath, ConsoleColor.Green);
                    }
                    else if (isManual)
                    {
                        Console.WriteLine("File unchanged: " + filePath);
                    }
                }
                else
                {
                    AddChange(changes, isManual, "New file detected: " + filePath, ConsoleColor.Blue);
                }
            }

            // Check for deleted files
            foreach (string storedFilePath in storedHashes.Keys)
            {
                if (!currentFilePaths.Contains(storedFilePath))
                {
                    AddChange(changes, isManual, "File deleted: " + storedFilePath, ConsoleColor.Red);
                }
            }

            return changes;
        }

        private static void AddChange(List<string> changes, bool isManual, string message, ConsoleColor color)
        {
            if (isManual)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message);
                Console.ResetColor();
            }
            else
            {
                changes.Add(message);
            }
        }
    }
}