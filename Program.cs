using System;

namespace FMCS
{
    class Program
    {
        static void Main(string[] args)
        {
            string message = "Hello, World!";
            string altmessage = "Hello, World";
            Console.WriteLine(Hashing.Hash(message));
            Console.WriteLine();
            Console.WriteLine(Hashing.Hash(message));
            Console.WriteLine();
            Console.WriteLine(Hashing.Hash(altmessage));
        }
    }
}