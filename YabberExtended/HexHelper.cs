using System;
using System.Collections.Generic;

namespace YabberExtended
{
    internal static class HexHelper
    {
        internal static byte[] HexToBytes(this string str)
        {
            string cleanInput = CleanInput(str);
            if (cleanInput.Length % 2 != 0)
            {
                throw new Exception("Hex length was not even.");
            }

            int length = cleanInput.Length / 2;
            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                bytes[i] = Convert.ToByte(cleanInput.Substring(i * 2, 2), 16);
            }

            return bytes;
        }

        internal static string ToHex(this byte[] bytes)
        {
            string str = string.Empty;
            for (int i = 0; i < bytes.Length; i++)
            {
                str += bytes[i].ToString("X2");
            }
            return str;
        }

        internal static bool IsHex(this char c)
            => (c >= '0' && c <= '9')
                || (c >= 'a' && c <= 'f')
                || (c >= 'A' && c <= 'F');

        internal static bool IsHex(this string str)
        {
            if (str.Length % 2 != 0)
            {
                return false;
            }

            foreach (char c in str)
            {
                if (!IsHex(c))
                {
                    return false;
                }
            }

            return true;
        }

        static string CleanInput(string input)
        {
            List<char> chars = new List<char>();
            foreach (char c in input)
            {
                if (c.IsHex())
                {
                    chars.Add(char.ToUpper(c));
                }
            }

            return new string(chars.ToArray());
        }
    }
}
