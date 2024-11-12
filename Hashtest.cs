using System;
using Hashing;

namespace FMCS
{
    class Hashtest
    {
        static void Tester(string[] args)
        {
            HashingAlgorithm hasher = new MD5();
            string message = "Hello, World!";
            string altmessage = "This is a test of the hashing algorithm used in my code";
            Console.WriteLine(hasher.Hash(message));
            Console.WriteLine();
            Console.WriteLine(hasher.Hash(message));
            Console.WriteLine();
            Console.WriteLine(hasher.Hash(altmessage));
        }
    }
}