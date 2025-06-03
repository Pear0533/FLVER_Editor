using System.Runtime.CompilerServices;

namespace SoulsFormats.Utilities
{
    internal static class MathHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BinaryAlign(int num, int alignment)
            => (num + (--alignment)) & ~alignment;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long BinaryAlign(long num, long alignment)
            => (num + (--alignment)) & ~alignment;

        public static int Align(int value, int alignment)
        {
            var remainder = value % alignment;
            if (remainder > 0)
            {
                return value + (alignment - remainder);
            }
            return value;
        }

        public static long Align(long value, long alignment)
        {
            var remainder = value % alignment;
            if (remainder > 0)
            {
                return value + (alignment - remainder);
            }
            return value;
        }
    }
}
