using System;
using System.Text;

namespace FMCS
{
    class Hashing
    {
        private static byte[] PadMessage(byte[] message)
        {
            int originalLength = message.Length;
            // Calculate the padding length to make the total length 56 bytes mod 64
            int paddingLength = (56 - (originalLength + 1) % 64) % 64;
            // Create a new byte array with the original message, padding, and length
            byte[] paddedMessage = new byte[originalLength + paddingLength + 9];
            // Copy the original message to the new array
            Array.Copy(message, paddedMessage, originalLength);
            // Append the '1' bit (0x80 in hex) to the message
            paddedMessage[originalLength] = 0x80;
            // Append the length of the original message in bits as a 64-bit number
            ulong messageLengthBits = (ulong)originalLength * 8;
            Array.Copy(BitConverter.GetBytes(messageLengthBits), 0, paddedMessage, paddedMessage.Length - 8, 8);
            return paddedMessage;
        }

        private static uint[] BreakMessageIntoBlocks(byte[] message)
        {
            uint[] blocks = new uint[message.Length / 4];
            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i] = BitConverter.ToUInt32(message, i * 4);
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
            byte[] paddedMessage = PadMessage(Encoding.UTF8.GetBytes(input));

            // Step 2: Break the message into 512-bit blocks
            uint[] blocks = BreakMessageIntoBlocks(paddedMessage);

            // Step 3: Initialize variables
            uint a = 0x67452301;
            uint b = 0xefcdab89;
            uint c = 0x98badcfe;
            uint d = 0x10325476;

            // Constants for MD5
            uint[] T = new uint[64];
            for (int i = 0; i < 64; i++)
            {
                T[i] = (uint)(Math.Abs(Math.Sin(i + 1)) * Math.Pow(2, 32));
            }

            // Step 4: Process each block
            for (int i = 0; i < blocks.Length / 16; i++)
            {
                uint[] X = new uint[16];
                Array.Copy(blocks, i * 16, X, 0, 16);

                uint AA = a;
                uint BB = b;
                uint CC = c;
                uint DD = d;

                // Round 1
                for (int j = 0; j < 16; j += 4)
                {
                    a = b + LeftRotate(a + F(b, c, d) + X[j] + T[j], 7);
                    d = a + LeftRotate(d + F(a, b, c) + X[j + 1] + T[j + 1], 12);
                    c = d + LeftRotate(c + F(d, a, b) + X[j + 2] + T[j + 2], 17);
                    b = c + LeftRotate(b + F(c, d, a) + X[j + 3] + T[j + 3], 22);
                }

                // Round 2
                for (int j = 0; j < 16; j += 4)
                {
                    a = b + LeftRotate(a + G(b, c, d) + X[(5 * j + 1) % 16] + T[j + 16], 5);
                    d = a + LeftRotate(d + G(a, b, c) + X[(5 * j + 6) % 16] + T[j + 17], 9);
                    c = d + LeftRotate(c + G(d, a, b) + X[(5 * j + 11) % 16] + T[j + 18], 14);
                    b = c + LeftRotate(b + G(c, d, a) + X[(5 * j) % 16] + T[j + 19], 20);
                }

                // Round 3
                for (int j = 0; j < 16; j += 4)
                {
                    a = b + LeftRotate(a + H(b, c, d) + X[(3 * j + 5) % 16] + T[j + 32], 4);
                    d = a + LeftRotate(d + H(a, b, c) + X[(3 * j + 8) % 16] + T[j + 33], 11);
                    c = d + LeftRotate(c + H(d, a, b) + X[(3 * j + 11) % 16] + T[j + 34], 16);
                    b = c + LeftRotate(b + H(c, d, a) + X[(3 * j + 14) % 16] + T[j + 35], 23);
                }

                // Round 4
                for (int j = 0; j < 16; j += 4)
                {
                    a = b + LeftRotate(a + I(b, c, d) + X[(7 * j) % 16] + T[j + 48], 6);
                    d = a + LeftRotate(d + I(a, b, c) + X[(7 * j + 7) % 16] + T[j + 49], 10);
                    c = d + LeftRotate(c + I(d, a, b) + X[(7 * j + 14) % 16] + T[j + 50], 15);
                    b = c + LeftRotate(b + I(c, d, a) + X[(7 * j + 21) % 16] + T[j + 51], 21);
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
            foreach (byte octet in hash)
            {
                sb.Append(octet.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}