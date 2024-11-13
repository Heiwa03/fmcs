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
                case "help":
                    PrintPossibleCommands();
                    break;
                case "exit":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Unknown command: " + mainCommand);
                    break;
            }
        }

        public static void PrintPossibleCommands()
        {
            Console.WriteLine("Possible commands:");
            Console.WriteLine("commit - Commit the current state of the target directory");
            Console.WriteLine("status - Check for changes in the target directory");
            Console.WriteLine("info <path_to_file> - Get information about a file");
            Console.WriteLine("exit - Exit the program");
        }

        public static void PrintInitialPrompt()
        {
            Console.WriteLine("Enter commands (e.g., 'commit' to update initial keys):");
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

                if (fileInfo.Extension.Equals(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    string fileContent = FileHandler.ReadFileAsString(filePath);
                    int lineCount = fileContent.Split('\n').Length;
                    int wordCount = fileContent.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
                    int charCount = fileContent.Length;

                    Console.WriteLine("Line Count: " + lineCount);
                    Console.WriteLine("Word Count: " + wordCount);
                    Console.WriteLine("Character Count: " + charCount);
                }
                else if (fileInfo.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                        fileInfo.Extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                        fileInfo.Extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase))
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (var image = Aspose.Drawing.Image.FromStream(fileStream))
                        {       
                            var height = image.Height;
                            var width = image.Width;
                            Console.WriteLine("Resolution: " + width + "x" + height);
                        }
                    }
                }
                else if (fileInfo.Extension.Equals(".cs", StringComparison.OrdinalIgnoreCase))
                {
                    string[] lines = FileHandler.ReadFileAsString(filePath).Split('\n');
                    int lineCount = lines.Length;
                    int classCount = 0;
                    int methodCount = 0;

                    foreach (string line in lines)
                    {
                        string trimmedLine = line.Trim();
                        if (trimmedLine.StartsWith("class "))
                        {
                            classCount++;
                        }
                        else if (trimmedLine.Contains("(") && trimmedLine.Contains(")") && 
                                (trimmedLine.Contains("public ") || trimmedLine.Contains("private ") || 
                                trimmedLine.Contains("protected ") || trimmedLine.Contains("internal ")))
                        {
                            methodCount++;
                        }
                    }

                    Console.WriteLine("Line Count: " + lineCount);
                    Console.WriteLine("Class Count: " + classCount);
                    Console.WriteLine("Method Count: " + methodCount);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred during info retrieval: " + ex.Message);
            }
        }
    }
}