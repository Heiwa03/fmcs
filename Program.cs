using System;

namespace FMCS
{
    class Program
    {
        static void Main(string[] args)
        {
            string message = "Hello, World!";
            string altmessage = "This is a test of the hashing algorithm used in my code";
            Console.WriteLine(Hashing.Hash(message));
            Console.WriteLine();
            Console.WriteLine(Hashing.Hash(message));
            Console.WriteLine();
            Console.WriteLine(Hashing.Hash(altmessage));
        }
    }
}