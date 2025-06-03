using System;
using System.IO;

namespace SoulsFormats
{
    /// <summary>
    /// A compression format used in ACLR and ACNB.<br/>
    /// Support currently very incomplete.
    /// </summary>
    public class FSLIBLZS
    {
        #region Is

        internal static bool Is(BinaryReaderEx br)
        {
            if (br.Stream.Length < 4)
                return false;

            string magic = br.GetASCII(0, 8);
            return magic == "fsliblzs" || magic == "fsliblzs";
        }

        /// <summary>
        /// Returns true if the bytes appear to be an fsliblzs file.
        /// </summary>
        public static bool Is(byte[] bytes)
        {
            var br = new BinaryReaderEx(true, bytes);
            return Is(br);
        }

        /// <summary>
        /// Returns true if the file appears to be an fsliblzs file.
        /// </summary>
        public static bool Is(string path)
        {
            using (FileStream stream = File.OpenRead(path))
            {
                var br = new BinaryReaderEx(true, stream);
                return Is(br);
            }
        }

        #endregion

        /// <summary>
        /// Decompress an fsliblzs.
        /// </summary>
        public static byte[] Decompress(BinaryReaderEx br)
        {
            br.AssertASCII("fsliblzs");
            br.AssertInt32(0);
            br.AssertInt32(0);
            int compressedSize = br.ReadInt32();
            br.AssertInt32(1);
            br.AssertInt32(0);
            br.AssertInt32(0);

            br.BigEndian = true;
            br.AssertInt16(1);
            br.AssertInt16(0);
            int decompressedSize = br.ReadInt32();
            br.AssertInt32(0);
            br.BigEndian = false;

            

            throw new NotImplementedException();
        }

        /// <summary>
        /// Compress a file to fsliblzs.
        /// </summary>
        public static byte[] Compress(byte[] data, BinaryWriterEx bw)
        {
            bw.WriteASCII("fsliblzs");
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.ReserveInt32("CompressedSize");
            bw.WriteInt32(1);
            bw.WriteInt32(0);
            bw.WriteInt32(0);

            bw.BigEndian = true;
            bw.WriteInt16(1);
            bw.WriteInt16(0);
            bw.WriteInt32(data.Length); // decompressed size
            bw.WriteInt32(0);
            bw.BigEndian = false;

            throw new NotImplementedException();
        }
    }
}
