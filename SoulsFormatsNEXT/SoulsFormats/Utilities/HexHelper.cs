using System;

namespace SoulsFormats.Utilities
{
    /// <summary>
    /// Helper methods for parsing hex strings.
    /// </summary>
    public static class HexHelper
    {
        /// <summary>
        /// Converts a hex string in format "AA BB CC DD" to a byte array.
        /// </summary>
        public static byte[] ParseHexString(string str)
        {
            string[] strings = str.Split(' ');
            byte[] bytes = new byte[strings.Length];
            for (int i = 0; i < strings.Length; i++)
                bytes[i] = Convert.ToByte(strings[i], 16);
            return bytes;
        }
    }
}
