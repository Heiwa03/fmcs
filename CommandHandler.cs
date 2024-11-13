using System;
using System.Collections.Generic;
using System.IO;
using Hashing;

namespace FMCS
{
    class CommandHandler
    {
        private static HashingAlgorithm hasher = new MD5();
        private const string TargetDir = "./target_test_folder";

        public static void HandleCommand(string command)
        {
            string[] commandParts = command.Split(' ', 2);
            string mainCommand = commandParts[0].ToLower();
            string? argument = commandParts.Length > 1 ? commandParts[1] : null;

            switch (mainCommand)
            {
                case "commit":
                    Commit();
                    break;
                case "status":
                    Status();
                    break;
                case "info":
                    Info(argument);
                    break;
                case "exit":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Unknown command: " + mainCommand);
                    break;
            }
        }

        private static void Commit()
        {
            try
            {
                // Initialize the directory and hash list file
                RuntimeDirectoryManagement.InitializeDirectory(TargetDir);

                // Generate initial hashes and store them
                List<string> filePaths = FileHandler.GetFilePaths(TargetDir);
                FileHandler.GenerateInitialHashes(filePaths, hasher, TargetDir);

                Console.WriteLine("Initial keys updated successfully.");

                // Run detection manually to print the status
                Program.RunDetection(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred during commit: " + ex.Message);
            }
        }

        private static void Status()
        {
            try
            {
                Console.WriteLine("Checking for changes...");
                Program.RunDetection(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred during status check: " + ex.Message);
            }
        }

        private static void Info(string? filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    Console.WriteLine("Usage: info <path_to_file>");
                    return;
                }

                if (!File.Exists(filePath))
                {
                    Console.WriteLine("File not found: " + filePath);
                    return;
                }

                FileInfo fileInfo = new FileInfo(filePath);
                Console.WriteLine("File: " + fileInfo.FullName);
                Console.WriteLine("Size: " + fileInfo.Length + " bytes");
                Console.WriteLine("Created: " + fileInfo.CreationTime);
                Console.WriteLine("Modified: " + fileInfo.LastWriteTime);
                Console.WriteLine("Hash: " + hasher.Hash(FileHandler.ReadFileAsByteArray(filePath)));
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred during info retrieval: " + ex.Message);
            }
        }
    }
}