namespace SoulsFormats.Utilities
{
    /// <summary>
    /// Endianness helper methods.
    /// </summary>
    public static class EndianHelper
    {
        /// <summary>
        /// Reverses the order of bits in a byte, probably very inefficiently.
        /// </summary>
        public static byte ReverseBits(byte value)
        {
            return (byte)(
                ((value & 0b00000001) << 7) |
                ((value & 0b00000010) << 5) |
                ((value & 0b00000100) << 3) |
                ((value & 0b00001000) << 1) |
                ((value & 0b00010000) >> 1) |
                ((value & 0b00100000) >> 3) |
                ((value & 0b01000000) >> 5) |
                ((value & 0b10000000) >> 7)
            );
        }
    }
}
