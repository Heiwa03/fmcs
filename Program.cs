using System;
using Hashing;

namespace FMCS
{
    class Program
    {
        static void Main(string[] args)
        {
            HashingAlgorithm hash = new MD5();
            string message = "Hello, World!";
            string altmessage = "This is a test of the hashing algorithm used in my code";
            Console.WriteLine(hash.Hash(message));
            Console.WriteLine();
            Console.WriteLine(hash.Hash(message));
            Console.WriteLine();
            Console.WriteLine(hash.Hash(altmessage));
        }
    }
}