# File Monitoring and Change Detection System

This project is a file monitoring and change detection system that periodically checks for changes in a specified directory and provides a command-line interface for various operations.

## Features

- **Auto-detection**: Automatically detects changes in the target directory every 5 seconds.
- **Command-line Interface**: Provides commands for committing initial hashes, checking status, retrieving file information, and exiting the program.
- **Asynchronous Operations**: Uses async/await for non-blocking operations.
- **Cross-platform Compatibility**: Uses Aspose.Drawing for image processing, compatible with Linux.

## Commands

- `commit`: Commits the current state of the target directory by generating initial hashes.
- `status`: Checks for changes in the target directory.
- `info <path_to_file>`: Retrieves information about a specified file, including line count, class count, and method count for `.cs` files.
- `exit`: Exits the program.

## Project Structure

- `Program.cs`: The main entry point of the application. Initializes the system, sets up the timer, and handles the command-line interface.
- `CommandHandler.cs`: Handles the command-line interface and executes commands.
- `FileHandler.cs`: Provides utility methods for file operations, including reading files, generating hashes, and detecting changes.
- `RuntimeDirectoryManagement.cs`: Manages the runtime directory and hash list file.
- `Hashing.cs`: Defines the hashing algorithm used for generating file hashes.

## Usage

1. **Clone the repository**:
   ```sh
   git clone <repository-url>
   cd <repository-directory>
   dotnet build
   dotnet run
   ```