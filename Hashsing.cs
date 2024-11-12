using System;
using System.Text;

namespace FMCS
{
    class Hashing
    {   
        private const uint A = 0x67452301;
        private const uint B = 0xefcdab89;
        private const uint C = 0x98badcfe;
        private const uint D = 0x10325476;

        private static string PadMessage(string message)
        {
            // Append a 1 to the message
            message += "1";

            // Append 0s to the message until the length is 448 mod 512
            while (message.Length % 512 != 448)
            {
                message += "0";
            }

            // Append the length of the message in bits as a 64-bit number
            message += Convert.ToString(message.Length * 8, 2).PadLeft(64, '0');

            return message;
        }
            private static uint[] BreakMessageIntoBlocks(string message)
        {
            uint[] blocks = new uint[message.Length / 512];

            for (int i = 0; i < message.Length; i += 512)
            {
                blocks[i / 512] = Convert.ToUInt32(message.Substring(i, 512), 2);
            }

            return blocks;
        }

        private static uint F(uint x, uint y, uint z)
        {
            return (x & y) | (~x & z);
        }

        private static uint G(uint x, uint y, uint z)
        {
            return (x & z) | (y & ~z);
        }

        private static uint H(uint x, uint y, uint z)
        {
            return x ^ y ^ z;
        }

        private static uint I(uint x, uint y, uint z)
        {
            return y ^ (x | ~z);
        }

        private static uint LeftRotate(uint x, int n)
        {
            return (x << n) | (x >> (32 - n));
        }
        
        public static string Hash(string input)
        {
            // Step 1: Pad the message
            string paddedMessage = PadMessage(input);

            // Step 2: Break the message into 512-bit blocks
            uint[] blocks = BreakMessageIntoBlocks(paddedMessage);

            // Step 3: Initialize variables
            uint a = A;
            uint b = B;
            uint c = C;
            uint d = D;

            // Constants for MD5
            uint[] T = new uint[64];
            for (int i = 0; i < 64; i++)
            {
                T[i] = (uint)(Math.Abs(Math.Sin(i + 1)) * Math.Pow(2, 32));
            }

            // Step 4: Process each block
            foreach (uint block in blocks)
            {
                uint[] X = new uint[16];
                for (int i = 0; i < 16; i++)
                {
                    X[i] = (block >> (i * 32)) & 0xFFFFFFFF;
                }

                uint AA = a;
                uint BB = b;
                uint CC = c;
                uint DD = d;

                // Round 1
                for (int i = 0; i < 16; i++)
                {
                    a = b + LeftRotate(a + F(b, c, d) + X[i] + T[i], 7);
                    d = a + LeftRotate(d + F(a, b, c) + X[(i + 1) % 16] + T[i + 1], 12);
                    c = d + LeftRotate(c + F(d, a, b) + X[(i + 2) % 16] + T[i + 2], 17);
                    b = c + LeftRotate(b + F(c, d, a) + X[(i + 3) % 16] + T[i + 3], 22);
                }

                // Round 2
                for (int i = 16; i < 32; i++)
                {
                    a = b + LeftRotate(a + G(b, c, d) + X[(5 * i + 1) % 16] + T[i], 5);
                    d = a + LeftRotate(d + G(a, b, c) + X[(5 * i + 6) % 16] + T[i + 1], 9);
                    c = d + LeftRotate(c + G(d, a, b) + X[(5 * i + 11) % 16] + T[i + 2], 14);
                    b = c + LeftRotate(b + G(c, d, a) + X[(5 * i + 0) % 16] + T[i + 3], 20);
                }

                // Round 3
                for (int i = 32; i < 48; i++)
                {
                    a = b + LeftRotate(a + H(b, c, d) + X[(3 * i + 5) % 16] + T[i], 4);
                    d = a + LeftRotate(d + H(a, b, c) + X[(3 * i + 8) % 16] + T[i + 1], 11);
                    c = d + LeftRotate(c + H(d, a, b) + X[(3 * i + 11) % 16] + T[i + 2], 16);
                    b = c + LeftRotate(b + H(c, d, a) + X[(3 * i + 14) % 16] + T[i + 3], 23);
                }

                // Round 4
                for (int i = 48; i < 64; i++)
                {
                    a = b + LeftRotate(a + I(b, c, d) + X[(7 * i) % 16] + T[i], 6);
                    d = a + LeftRotate(d + I(a, b, c) + X[(7 * i + 7) % 16] + T[i + 1], 10);
                    c = d + LeftRotate(c + I(d, a, b) + X[(7 * i + 14) % 16] + T[i + 2], 15);
                    b = c + LeftRotate(b + I(c, d, a) + X[(7 * i + 21) % 16] + T[i + 3], 21);
                }

                a += AA;
                b += BB;
                c += CC;
                d += DD;
            }

            // Step 5: Produce the final hash
            byte[] hash = new byte[16];
            Buffer.BlockCopy(BitConverter.GetBytes(a), 0, hash, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(b), 0, hash, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(c), 0, hash, 8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(d), 0, hash, 12, 4);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}