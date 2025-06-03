using System;
using System.Collections.Generic;
using DbpType = SoulsFormats.PARAMDBP.DbpType;
using DefType = SoulsFormats.PARAMDEF.DefType;

namespace SoulsFormats
{
    internal static class ParamDbpUtil
    {
        private static readonly Dictionary<DbpType, object> Defaults = new Dictionary<DbpType, object>
        {
            [DbpType.s8] = 0,
            [DbpType.u8] = 0,
            [DbpType.s16] = 0,
            [DbpType.u16] = 0,
            [DbpType.s32] = 0,
            [DbpType.u32] = 0,
            [DbpType.f32] = 0f,
        };

        private static readonly Dictionary<DbpType, object> Minimums = new Dictionary<DbpType, object>
        {
            [DbpType.s8] = (int)sbyte.MinValue,
            [DbpType.u8] = (int)byte.MinValue,
            [DbpType.s16] = (int)short.MinValue,
            [DbpType.u16] = (int)ushort.MinValue,
            [DbpType.s32] = int.MinValue,
            [DbpType.u32] = (int)uint.MinValue,
            [DbpType.f32] = float.MinValue,
        };

        private static readonly Dictionary<DbpType, object> Maximums = new Dictionary<DbpType, object>
        {
            [DbpType.s8] = (int)sbyte.MaxValue,
            [DbpType.u8] = (int)byte.MaxValue,
            [DbpType.s16] = (int)short.MaxValue,
            [DbpType.u16] = (int)ushort.MaxValue,
            [DbpType.s32] = int.MaxValue,
            [DbpType.u32] = int.MaxValue, // Yes, u32 uses signed int too (usually)
            [DbpType.f32] = float.MaxValue
        };

        private static readonly Dictionary<DbpType, object> Increments = new Dictionary<DbpType, object>
        {
            [DbpType.s8] = 1,
            [DbpType.u8] = 1,
            [DbpType.s16] = 1,
            [DbpType.u16] = 1,
            [DbpType.s32] = 1,
            [DbpType.u32] = 1,
            [DbpType.f32] = 0.01f
        };

        public static object GetDefaultDefault(DbpType type)
        {
            return Defaults[type];
        }

        public static object GetDefaultMinimum(DbpType type)
        {
            return Minimums[type];
        }

        public static object GetDefaultMaximum(DbpType type)
        {
            return Maximums[type];
        }

        public static object GetDefaultIncrement(DbpType type)
        {
            return Increments[type];
        }

        public static string GetDefaultFormat(DbpType type)
        {
            switch (type)
            {
                case DbpType.s8: return "%d";
                case DbpType.u8: return "%d";
                case DbpType.s16: return "%d";
                case DbpType.u16: return "%d";
                case DbpType.s32: return "%d";
                case DbpType.u32: return "%d";
                case DbpType.f32: return "%f";

                default:
                    throw new NotImplementedException($"No default format specified for {nameof(DbpType)}.{type}");
            }
        }
    }
}
