using System;
using System.IO;
using Hashing;

namespace FMCS
{
    class Program 
    {
        static void Main(string[] args)
        {
            // Example usage of the FileHandler class
            HashingAlgorithm hasher = new MD5();
            try
            {
                string filePath = "./target_test_folder/testfile.img";
                //byte[] fileContents = FileHandler.ReadFileAsByteArray(filePath);
                byte[] fileContents = FileHandler.ReadFileAsByteArray(filePath);
                Console.WriteLine("File read successfully. Byte array length: " + fileContents.Length);
                Console.WriteLine(hasher.Hash(fileContents));
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
    }
}