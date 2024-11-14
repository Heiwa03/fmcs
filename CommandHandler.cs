using Hashing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Aspose.Drawing;

namespace FMCS
{
    public abstract class Command
    {
        protected HashingAlgorithm Hasher;

        protected Command(HashingAlgorithm hasher)
        {
            Hasher = hasher;
        }

        public abstract Task ExecuteAsync(string? argument);
    }

    public class CommitCommand : Command
    {
        public CommitCommand(HashingAlgorithm hasher) : base(hasher) { }

        public override async Task ExecuteAsync(string? argument)
        {
            try
            {
                // Initialize the directory and hash list file
                RuntimeDirectoryManagement.InitializeDirectory(Program.TargetDir);

                // Generate initial hashes and store them
                List<string> filePaths = FileHandler.GetFilePaths(Program.TargetDir);
                FileHandler.GenerateInitialHashes(filePaths, Hasher, Program.TargetDir);

                // Save a snapshot of the current files
                SaveSnapshot(filePaths);

                Console.WriteLine("Initial keys updated and snapshot saved successfully.");

                // Run detection manually to print the status
                await Program.RunDetectionAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred during commit: " + ex.Message);
            }
        }

        private void SaveSnapshot(List<string> filePaths)
        {
            string snapshotDir = Path.Combine(Program.TargetDir, RuntimeDirectoryManagement.DirName, "snapshots", DateTime.Now.ToString("yyyyMMddHHmmss"));
            Directory.CreateDirectory(snapshotDir);

            foreach (string filePath in filePaths)
            {
                string destFilePath = Path.Combine(snapshotDir, Path.GetFileName(filePath));
                File.Copy(filePath, destFilePath);
            }
        }
    }

    public class StatusCommand : Command
    {
        public StatusCommand(HashingAlgorithm hasher) : base(hasher) { }

        public override async Task ExecuteAsync(string? argument)
        {
            try
            {
                Console.WriteLine("Checking for changes...");
                await Program.RunDetectionAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred during status check: " + ex.Message);
            }
        }
    }

    public class InfoCommand : Command
    {
        public InfoCommand(HashingAlgorithm hasher) : base(hasher) { }

        public override async Task ExecuteAsync(string? argument)
        {
            try
            {
                if (string.IsNullOrEmpty(argument))
                {
                    Console.WriteLine("Usage: info <path_to_file>");
                    return;
                }

                if (!File.Exists(argument))
                {
                    Console.WriteLine("File not found: " + argument);
                    return;
                }

                FileInfo fileInfo = new FileInfo(argument);
                Console.WriteLine("File: " + fileInfo.FullName);
                Console.WriteLine("Size: " + fileInfo.Length + " bytes");
                Console.WriteLine("Created: " + fileInfo.CreationTime);
                Console.WriteLine("Modified: " + fileInfo.LastWriteTime);
                Console.WriteLine("Hash: " + Hasher.Hash(await FileHandler.ReadFileAsByteArrayAsync(argument)));

                if (fileInfo.Extension.Equals(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    string fileContent = await FileHandler.ReadFileAsStringAsync(argument);
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
                    using (var image = Image.FromFile(argument))
                    {
                        Console.WriteLine("Resolution: " + image.Width + "x" + image.Height);
                    }
                }
                else if (fileInfo.Extension.Equals(".cs", StringComparison.OrdinalIgnoreCase))
                {
                    string fileContent = await FileHandler.ReadFileAsStringAsync(argument);
                    string[] lines = fileContent.Split('\n');
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

    public class RevertCommand : Command
    {
        public RevertCommand(HashingAlgorithm hasher) : base(hasher) { }

        public override Task ExecuteAsync(string? argument)
        {
            try
            {
                if (string.IsNullOrEmpty(argument))
                
                {
                    string[] snapshotDirs = Directory.GetDirectories(Path.Combine(Program.TargetDir, RuntimeDirectoryManagement.DirName, "snapshots"));
                    foreach (string dirinlist in snapshotDirs)
                    {
                        Console.WriteLine(Path.GetFileName(dirinlist));
                    }
                    Console.WriteLine("Usage: revert <snapshot_timestamp>");
                    return Task.CompletedTask;
                }

                string snapshotDir = Path.Combine(Program.TargetDir, RuntimeDirectoryManagement.DirName, "snapshots", argument);
                if (!Directory.Exists(snapshotDir))
                {
                    Console.WriteLine("Snapshot not found: " + snapshotDir);
                    return Task.CompletedTask;
                }

                RestoreSnapshot(snapshotDir);
                Console.WriteLine("Reverted to snapshot: " + argument);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred during revert: " + ex.Message);
            }

            return Task.CompletedTask;
        }

        private void RestoreSnapshot(string snapshotDir)
        {
            string[] snapshotFiles = Directory.GetFiles(snapshotDir);
            List<string> filePaths = FileHandler.GetFilePaths(Program.TargetDir);
            foreach (string filePath in filePaths)
            {
                File.Delete(filePath);
            }
            foreach (string snapshotFile in snapshotFiles)
            {
                string destFilePath = Path.Combine(Program.TargetDir, Path.GetFileName(snapshotFile));

                File.Copy(snapshotFile, destFilePath, true);
            }
        }
    }

    public class ExitCommand : Command
    {
        public ExitCommand(HashingAlgorithm hasher) : base(hasher) { }

        public override Task ExecuteAsync(string? argument)
        {
            Console.WriteLine("Exiting the program...");
            Environment.Exit(0);
            return Task.CompletedTask;
        }
    }

    public class CommandHandler
    {
        private readonly Dictionary<string, Command> Commands;

        public CommandHandler(HashingAlgorithm hasher)
        {
            Commands = new Dictionary<string, Command>
            {
                { "commit", new CommitCommand(hasher) },
                { "status", new StatusCommand(hasher) },
                { "info", new InfoCommand(hasher) },
                { "revert", new RevertCommand(hasher) },
                { "exit", new ExitCommand(hasher) }
            };
        }

        public async Task HandleCommandAsync(string command)
        {
            string[] commandParts = command.Split(' ', 2);
            string mainCommand = commandParts[0].ToLower();
            string? argument = commandParts.Length > 1 ? commandParts[1] : null;

            if (Commands.ContainsKey(mainCommand))
            {
                await Commands[mainCommand].ExecuteAsync(argument);
            }
            else
            {
                Console.WriteLine("Unknown command: " + mainCommand);
            }
        }

        public void PrintInitialPrompt()
        {
            Console.WriteLine("Press [Enter] to exit the program.");
            Console.WriteLine("Enter commands (e.g., 'commit' to update initial keys):");
        }
    }
}