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
            switch (command.ToLower())
            {
                case "commit":
                    Commit();
                    break;
                case "status":
                    Status();
                    break;
                default:
                    Console.WriteLine("Unknown command: " + command);
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
                List<string> filePaths = FileHandler.GetFilePaths(TargetDir);
                FileHandler.DetectChanges(filePaths, hasher, TargetDir);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred during status check: " + ex.Message);
            }
        }
    }
}