using System;
using System.Diagnostics.Contracts;
using System.IO;
using Hashing;

namespace FMCS
{
    class Program 
    {
        
        private const string TargetDir = "./target_test_folder";
        static void Main(string[] args)
        {
            // Example usage of the FileHandler class
            HashingAlgorithm hasher = new MD5();
            try
            {
                string folderPath = TargetDir;
                List<string> filePaths = FileHandler.GetFilePaths(folderPath);
                Console.WriteLine("Files in folder:");
                FileHandler.HashFilesFromList(filePaths, hasher);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
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

        public static void HashFilesFromList(List<string> filePaths, HashingAlgorithm hasher)
        {
            foreach (string filePath in filePaths)
            {
                byte[] fileContents = ReadFileAsByteArray(filePath);
                Console.WriteLine("File " + filePath + " read successfully. Byte array length: " + fileContents.Length);
                Console.WriteLine(hasher.Hash(fileContents));
            }
        }

        
    }

    class RuntimeDirectoryManagement
    {
        private const string DirName = ".runtimedir";
        private const string HashListFileName = "hashlist";
        public static void CreateDirectory(string directoryPath)
        {
            // Check if the directory already exists
            if (Directory.Exists(directoryPath))
            {
                throw new IOException("The specified directory already exists.");
            }

            // Create the directory
            Directory.CreateDirectory(directoryPath);
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

        public static void InitializeDirectory(string targetdir, string directoryPath)
        {
            CreateDirectory(targetdir + "/" + directoryPath);
            CreateHashListFile(targetdir + "/" + directoryPath, targetdir + "/" + directoryPath + "/" + HashListFileName);

        }
    }
}