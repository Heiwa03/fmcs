using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using Hashing;

namespace FMCS
{
    class Program 
    {
        private const string TargetDir = "./target_test_folder";
        private static System.Timers.Timer detectionTimer;
        private static HashingAlgorithm hasher = new MD5();

        static void Main(string[] args)
        {
            try
            {
                // Initialize the directory and hash list file
                RuntimeDirectoryManagement.InitializeDirectory(TargetDir);

                // Generate initial hashes and store them
                List<string> filePaths = FileHandler.GetFilePaths(TargetDir);
                FileHandler.GenerateInitialHashes(filePaths, hasher, TargetDir);

                // Set up a timer to run the detection every 5 seconds
                detectionTimer = new System.Timers.Timer(5000); // 5000 milliseconds = 5 seconds
                detectionTimer.Elapsed += (sender, e) => RunDetection(filePaths);
                detectionTimer.AutoReset = true;
                detectionTimer.Enabled = true;

                // Keep the application running
                Console.WriteLine("Press [Enter] to exit the program.");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        private static void RunDetection(List<string> filePaths)
        {
            try
            {
                Console.WriteLine("Running detection...");
                FileHandler.DetectChanges(filePaths, hasher, TargetDir);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred during detection: " + ex.Message);
            }
        }
    }

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
            string hashListFilePath = targetDir + "/" + RuntimeDirectoryManagement.DirName + "/" + RuntimeDirectoryManagement.HashListFileName;
            using (StreamWriter writer = new StreamWriter(hashListFilePath))
            {
                foreach (string filePath in filePaths)
                {
                    byte[] fileContents = ReadFileAsByteArray(filePath);
                    string hash = hasher.Hash(fileContents);
                    writer.WriteLine(filePath + ":" + hash);
                }
            }
        }

        // Method to detect changes in the directory
        public static void DetectChanges(List<string> filePaths, HashingAlgorithm hasher, string targetDir)
        {
            string hashListFilePath = targetDir + "/" + RuntimeDirectoryManagement.DirName + "/" + RuntimeDirectoryManagement.HashListFileName;
            Dictionary<string, string> storedHashes = new Dictionary<string, string>();

            // Read the stored hashes from the hash list file
            using (StreamReader reader = new StreamReader(hashListFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(':');
                    if (parts.Length == 2)
                    {
                        storedHashes[parts[0]] = parts[1];
                    }
                }
            }

            // Compute the current hashes and compare with the stored hashes
            foreach (string filePath in filePaths)
            {
                byte[] fileContents = ReadFileAsByteArray(filePath);
                string currentHash = hasher.Hash(fileContents);

                if (storedHashes.ContainsKey(filePath))
                {
                    if (storedHashes[filePath] != currentHash)
                    {
                        Console.WriteLine("File changed: " + filePath);
                    }
                    else
                    {
                        Console.WriteLine("DEBUG: File unchanged: " + filePath);
                    }
                }
                else
                {
                    Console.WriteLine("New file detected: " + filePath);
                }
            }

            // Check for deleted files
            foreach (string storedFilePath in storedHashes.Keys)
            {
                if (!filePaths.Contains(storedFilePath))
                {
                    Console.WriteLine("File deleted: " + storedFilePath);
                }
            }
        }
    }

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

            // Create the hash list file
            File.Create(hashListFilePath).Close();
        }

        public static void InitializeDirectory(string targetDir)
        {
            string runtimeDirPath = targetDir + "/" + DirName;
            CreateDirectory(runtimeDirPath);
            CreateHashListFile(runtimeDirPath, runtimeDirPath + "/" + HashListFileName);
        }
    }
}