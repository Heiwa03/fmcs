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

    }
}