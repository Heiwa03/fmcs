using Hashing;

namespace FMCS
{
    public abstract class Command
    {
        protected HashingAlgorithm Hasher;

        protected Command(HashingAlgorithm hasher)
        {
            Hasher = hasher;
        }

        public abstract void Execute(string? argument);
    }

    public class CommitCommand : Command
    {
        public CommitCommand(HashingAlgorithm hasher) : base(hasher) { }

        public override void Execute(string? argument)
        {
            try
            {
                // Initialize the directory and hash list file
                RuntimeDirectoryManagement.InitializeDirectory(Program.TargetDir);

                // Generate initial hashes and store them
                List<string> filePaths = FileHandler.GetFilePaths(Program.TargetDir);
                FileHandler.GenerateInitialHashes(filePaths, Hasher, Program.TargetDir);

                Console.WriteLine("Initial keys updated successfully.");

                // Run detection manually to print the status
                Program.RunDetection(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred during commit: " + ex.Message);
            }
        }
    }

    public class StatusCommand : Command
    {
        public StatusCommand(HashingAlgorithm hasher) : base(hasher) { }

        public override void Execute(string? argument)
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
    }

    public class ExitCommand : Command 
    {
        public ExitCommand(HashingAlgorithm hasher) : base(hasher) { }

        public override void Execute(string? argument)
        {
            Environment.Exit(0);
        }
    }

    public class InfoCommand : Command
    {
        public InfoCommand(HashingAlgorithm hasher) : base(hasher) { }

        public override void Execute(string? argument)
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
                Console.WriteLine("Hash: " + Hasher.Hash(FileHandler.ReadFileAsByteArray(argument)));

                if (fileInfo.Extension.Equals(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    string fileContent = FileHandler.ReadFileAsString(argument);
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
                    using (var image = Aspose.Drawing.Image.FromFile(argument))
                    {
                        Console.WriteLine("Resolution: " + image.Width + "x" + image.Height);
                    }
                }
                else if (fileInfo.Extension.Equals(".cs", StringComparison.OrdinalIgnoreCase))
                {
                    string[] lines = FileHandler.ReadFileAsString(argument).Split('\n');
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
                { "exit", new ExitCommand(hasher) }
            };
        }

        public void HandleCommand(string command)
        {
            string[] commandParts = command.Split(' ', 2);
            string mainCommand = commandParts[0].ToLower();
            string? argument = commandParts.Length > 1 ? commandParts[1] : null;

            if (Commands.ContainsKey(mainCommand))
            {
                Commands[mainCommand].Execute(argument);
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