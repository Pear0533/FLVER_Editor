using System;
using System.Collections.Generic;

namespace SoulsFormats.Other.Kuon
{
    /// <summary>
    /// The format of Binder files in Kuon except for the main archive.
    /// <para>The difference is that these do not include a size field in file entries. They have no padding before file entries.</para>
    /// </summary>
    public class BND : SoulsFile<BND>
    {
        /// <summary>
        /// The files in this <see cref="BND"/>.
        /// </summary>
        public List<File> Files { get; set; }

        /// <summary>
        /// The version of this <see cref="BND"/>.
        /// <para>Only 200 and 202 have been seen.</para>
        /// </summary>
        public int FileVersion { get; set; }

        /// <summary>
        /// Creates a <see cref="BND"/>.
        /// </summary>
        public BND()
        {
            Files = new List<File>();
            FileVersion = 200;
        }

        /// <summary>
        /// Creates a <see cref="BND"/> with the specified version.
        /// </summary>
        public BND(int version)
        {
            Files = new List<File>();
            FileVersion = version;
        }

        /// <summary>
        /// Converts a <see cref="DVDBND"/> to a <see cref="BND"/>.
        /// </summary>
        public BND(DVDBND bnd)
        {
            Files = new List<File>(bnd.Files.Count);
            for (int i = 0; i < bnd.Files.Count; i++)
            {
                Files.Add(new File(bnd.Files[i].ID, bnd.Files[i].Name, bnd.Files[i].Bytes));
            }

            FileVersion = bnd.FileVersion;
        }

        /// <summary>
        /// Checks whether the data appears to be a <see cref="BND"/>.
        /// </summary>
        protected override bool Is(BinaryReaderEx br)
        {
            if (br.Length < 16)
                return false;

            return br.GetASCII(0, 4) == "BND\0";
        }

        /// <summary>
        /// Reads a <see cref="BND"/> from a stream.
        /// </summary>
        protected override void Read(BinaryReaderEx br)
        {
            br.BigEndian = false;

            br.AssertASCII("BND\0");
            FileVersion = br.ReadInt32();
            int fileSize = br.ReadInt32();
            int fileCount = br.ReadInt32();

            Files = new List<File>(fileCount);
            for (int i = 0; i < fileCount; i++)
            {
                int nextOffset = fileSize;
                if (i < fileCount - 1)
                {
                    nextOffset = br.GetInt32(br.Position + 0xC + 4);
                }

                Files.Add(new File(br, nextOffset));
            }
        }

        /// <summary>
        /// Writes this <see cref="BND"/> to a stream.
        /// </summary>
        protected override void Write(BinaryWriterEx bw)
        {
            bw.BigEndian = false;

            bw.WriteASCII("BND\0");
            bw.WriteInt32(FileVersion);
            bw.ReserveInt32("FileSize");
            bw.WriteInt32(Files.Count);

            for (int i = 0; i < Files.Count; i++)
            {
                Files[i].Write(bw, i);
            }
            
            for (int i = 0; i < Files.Count; i++)
            {
                bw.FillInt32($"NameOffset_{i}", (int)bw.Position);
                bw.WriteShiftJIS(Files[i].Name, true);
            }

            // Yoshitsune Eiyuuden Shura AllMsg.bnd
            bw.Pad(0x10);

            for (int i = 0; i < Files.Count; i++)
            {
                bw.FillInt32($"DataOffset_{i}", (int)bw.Position);
                bw.WriteBytes(Files[i].Bytes);
            }

            bw.FillInt32("FileSize", (int)bw.Position);
        }

        /// <summary>
        /// A <see cref="File"/> in a <see cref="BND"/>.
        /// </summary>
        public class File
        {
            /// <summary>
            /// The ID of this <see cref="File"/>.
            /// </summary>
            public int ID { get; set; }

            /// <summary>
            /// Name of this <see cref="File"/>.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// The raw data of this <see cref="File"/>.
            /// </summary>
            public byte[] Bytes { get; set; }

            /// <summary>
            /// Creates a <see cref="File"/>.
            /// </summary>
            public File()
            {
                ID = -1;
                Name = string.Empty;
                Bytes = Array.Empty<byte>();
            }

            /// <summary>
            /// Creates a <see cref="File"/> with an ID.
            /// </summary>
            public File(int id)
            {
                ID = id;
                Name = string.Empty;
                Bytes = Array.Empty<byte>();
            }

            /// <summary>
            /// Creates a <see cref="File"/> with a name.
            /// </summary>
            public File(string name)
            {
                ID = -1;
                Name = name;
                Bytes = Array.Empty<byte>();
            }

            /// <summary>
            /// Creates a <see cref="File"/> with bytes.
            /// </summary>
            public File(byte[] bytes)
            {
                ID = -1;
                Name = string.Empty;
                Bytes = bytes;
            }

            /// <summary>
            /// Creates a <see cref="File"/> with an ID and name.
            /// </summary>
            public File(int id, string name)
            {
                ID = id;
                Name = name;
                Bytes = Array.Empty<byte>();
            }

            /// <summary>
            /// Creates a <see cref="File"/> with an ID and bytes.
            /// </summary>
            public File(int id, byte[] bytes)
            {
                ID = id;
                Bytes = bytes;
            }

            /// <summary>
            /// Creates a <see cref="File"/> with a name and bytes.
            /// </summary>
            public File(string name, byte[] bytes)
            {
                ID = -1;
                Name = name;
                Bytes = bytes;
            }

            /// <summary>
            /// Creates a <see cref="File"/> with an id, name, and bytes.
            /// </summary>
            public File(int id, string name, byte[] bytes)
            {
                ID = id;
                Name = name;
                Bytes = bytes;
            }

            /// <summary>
            /// Reads a <see cref="File"/> from a stream.
            /// </summary>
            internal File(BinaryReaderEx br, int nextOffset)
            {
                ID = br.ReadInt32();
                int dataOffset = br.ReadInt32();
                int nameOffset = br.ReadInt32();

                Name = br.GetShiftJIS(nameOffset);
                Bytes = br.GetBytes(dataOffset, nextOffset - dataOffset);
            }

            /// <summary>
            /// Writes this <see cref="File"/> entry to a stream.
            /// </summary>
            internal void Write(BinaryWriterEx bw, int index)
            {
                bw.WriteInt32(ID);
                bw.ReserveInt32($"DataOffset_{index}");
                bw.ReserveInt32($"NameOffset_{index}");
            }
        }
    }
}
