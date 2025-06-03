using System.Collections.Generic;
using System.IO;

namespace SoulsFormats.Other.Dreamcast
{
    /// <summary>
    /// An archive format used in games such as Frame Gride. Extension: *.mgf, File magic: MGFL
    /// </summary>
    public class MGF : SoulsFile<MGF>
    {
        /// <summary>
        /// Unknown; Seen as 1 or 2.
        /// </summary>
        public int Unk04;

        /// <summary>
        /// The files within the MGF archive.
        /// </summary>
        public List<File> Files;

        /// <summary>
        /// Returns true if a file appears to be an MGF archive.
        /// </summary>
        protected override bool Is(BinaryReaderEx br)
        {
            br.BigEndian = false;
            if (br.Length < 4)
                return false;
            return br.ReadASCII(4) == "MGFL";
        }

        /// <summary>
        /// Deserializes file data from a stream.
        /// </summary>
        protected override void Read(BinaryReaderEx br)
        {
            br.BigEndian = false;
            br.AssertASCII("MGFL");
            Unk04 = br.ReadInt32();
            int fileCount = br.ReadInt32();

            Files = new List<File>();
            for (int i = 0; i < fileCount; i++)
                Files.Add(new File(br));
        }

        /// <summary>
        /// Serializes file data to a stream.
        /// </summary>
        protected override void Write(BinaryWriterEx bw)
        {
            bw.BigEndian = false;
            bw.WriteASCII("MGFL", false);
            bw.WriteInt32(Unk04);
            bw.WriteInt32(Files.Count);

            int next_start_block = 1;
            foreach (File file in Files)
            {
                int blockCount = File.GetBlockCount(file.Bytes.Length);
                file.Write(bw, blockCount, next_start_block);
                next_start_block += blockCount;
            }

            bw.Pad(0x800);
            foreach (File file in Files)
            {
                bw.WriteBytes(file.Bytes);
                bw.Pad(0x800);
            }
        }

        /// <summary>
        /// A file in an MGF archive.
        /// </summary>
        public class File
        {
            /// <summary>
            /// The data of the file.
            /// </summary>
            public byte[] Bytes;

            /// <summary>
            /// Create a new file with the provided bytes.
            /// </summary>
            /// <param name="bytes">The bytes to create the file with.</param>
            public File(byte[] bytes)
            {
                Bytes = bytes;
            }

            internal File(BinaryReaderEx br)
            {
                int length = br.ReadInt32();
                int blockCount = br.ReadInt32();
                int startBlock = br.ReadInt32();

                int calculated = GetBlockCount(length);
                if (calculated != blockCount)
                    throw new InvalidDataException($"Block count {calculated} calcuated from length did not match block count {blockCount} given in entry.");

                br.StepIn(startBlock * 0x800);
                Bytes = br.ReadBytes(length);
                br.StepOut();
            }

            internal void Write(BinaryWriterEx bw, int blockCount, int startBlock)
            {
                bw.WriteInt32(Bytes.Length);
                bw.WriteInt32(blockCount);
                bw.WriteInt32(startBlock);
            }

            /// <summary>
            /// Get the number of aligned 0x800 blocks a certain length of data occupies.
            /// </summary>
            /// <param name="length">The number of bytes.</param>
            /// <returns>The number of occupied 0x800 blocks.</returns>
            internal static int GetBlockCount(int length)
            {
                int padding = (0x800 - (length % 0x800) % 0x800);
                int paddedLength = length + padding;
                return paddedLength / 0x800;
            }
        }
    }
}
