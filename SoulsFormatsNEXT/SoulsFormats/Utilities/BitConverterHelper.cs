using System;
using System.Buffers.Binary;

namespace SoulsFormats.Utilities
{
    internal static class BitConverterHelper
    {
        public static ushort ToUInt16BigEndian(byte[] bytes, int offset)
        {
            ushort value = BitConverter.ToUInt16(bytes, offset);
            if (BitConverter.IsLittleEndian)
            {
                value = BinaryPrimitives.ReverseEndianness(value);
            }

            return value;
        }

        public static int ToInt32BigEndian(byte[] bytes, int offset)
        {
            int value = BitConverter.ToInt32(bytes, offset);
            if (BitConverter.IsLittleEndian)
            {
                value = BinaryPrimitives.ReverseEndianness(value);
            }

            return value;
        }
    }
}
