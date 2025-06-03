using System.Numerics;

namespace SoulsFormats.Other.MWC
{
    /// <summary>
    /// Metal Wolf Chaos SMD file format, not to be confused with the Valve SMD file format
    /// </summary>
    public class SMD : SoulsFile<SMD>
    {
        /// <summary>
        /// SMD Header structure
        /// </summary>
        public struct SMDHeader
        {
            /// <summary>
            /// SMD Filesize
            /// </summary>
            public int fileSize;
            /// <summary>
            /// Header entry count
            /// </summary>
            public int SMDHeaderEntryCount;
            /// <summary>
            /// Unknown
            /// </summary>
            public int unkCount0;
            /// <summary>
            /// Unknown
            /// </summary>
            public int unkCount1;

            /// <summary>
            /// Header entry offset
            /// </summary>
            public int headerEntryOffset;
            /// <summary>
            /// Indices offset. Triangle indices? Might not be tristrips
            /// </summary>
            public int indicesOffset;
            /// <summary>
            /// Data offset.
            /// </summary>
            public int dataOffset;
            /// <summary>
            /// Unknown count
            /// </summary>
            public int unkCount2;

            /// <summary>
            /// Unknown count
            /// </summary>
            public int unkCount3;
            /// <summary>
            /// Unknown count
            /// </summary>
            public int unkCount4;
        }

        /// <summary>
        /// Unknown
        /// </summary>
        public struct SMDHeaderEntry
        {
            /// <summary>
            /// Unknown
            /// </summary>
            public int int00;
            /// <summary>
            /// Unknown
            /// </summary>
            public int int04;
            /// <summary>
            /// Unknown
            /// </summary>
            public int int08;
            /// <summary>
            /// Unknown
            /// </summary>
            public int int0C;

            /// <summary>
            /// Unknown
            /// </summary>
            public int int10;
        }

        /// <summary>
        /// Unknown, maybe a vertex?
        /// </summary>
        public struct SMDDataEntry
        {
            /// <summary>
            /// Unknown
            /// </summary>
            public Vector3 vec3Data;
            /// <summary>
            /// Unknown
            /// </summary>
            public short usht0C;
            /// <summary>
            /// Unknown
            /// </summary>
            public short usht0A;
        }
    }
}
