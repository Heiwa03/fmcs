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
        
        
    }
}