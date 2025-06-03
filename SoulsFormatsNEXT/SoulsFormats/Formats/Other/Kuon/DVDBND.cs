using System;
using System.Collections.Generic;

namespace SoulsFormats.Other.Kuon
{
    /// <summary>
    /// The format of the Binder files acting as Kuon's main archive, ALL/ELL. Extension: .bnd
    /// <para>The difference is that this one does include a size field in file entries. This one includes padding before file entries.</para>
    /// </summary>
    public class DVDBND : SoulsFile<DVDBND>
    {
        /// <summary>
        /// The files in this <see cref="DVDBND"/>.
        /// </summary>
        public List<File> Files { get; set; }

        /// <summary>
        /// The version of this <see cref="DVDBND"/>.
        /// <para>Only 202 has been seen.</para>
        /// </summary>
        public int FileVersion { get; set; }

        /// <summary>
        /// Creates a <see cref="DVDBND"/>.
        /// </summary>
        public DVDBND()
        {
            Files = new List<File>();
            FileVersion = 202;
        }

        /// <summary>
        /// Creates a <see cref="DVDBND"/> with the specified version.
        /// </summary>
        public DVDBND(int version)
        {
            Files = new List<File>();
            FileVersion = version;
        }

        /// <summary>
        /// Converts a <see cref="BND"/> to a <see cref="DVDBND"/>.
        /// </summary>
        public DVDBND(BND bnd)
        {
            Files = new List<File>(bnd.Files.Count);
            for (int i = 0; i < bnd.Files.Count; i++)
            {
                Files.Add(new File(bnd.Files[i].ID, bnd.Files[i].Name, bnd.Files[i].Bytes));
            }

            FileVersion = bnd.FileVersion;
        }

        /// <summary>
        /// Checks whether the data appears to be a <see cref="DVDBND"/>.
        /// </summary>
        protected override bool Is(BinaryReaderEx br)
        {
            if (br.Length < 16)
                return false;

            return br.GetASCII(0, 4) == "BND\0";
        }

        /// <summary>
        /// Reads a <see cref="DVDBND"/> from a stream.
        /// </summary>
        protected override void Read(BinaryReaderEx br)
        {
            br.BigEndian = false;

            br.AssertASCII("BND\0");
            FileVersion = br.ReadInt32();
            br.Position += 4; // File Size
            int fileCount = br.ReadInt32();

            Files = new List<File>(fileCount);
            for (int i = 0; i < fileCount; i++)
                Files.Add(new File(br));
        }

        /// <summary>
        /// Writes this <see cref="DVDBND"/> to a stream.
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

            for (int i = 0; i < Files.Count; i++)
            {
                bw.Pad(0x800);
                bw.FillInt32($"DataOffset_{i}", (int)bw.Position);
                bw.WriteBytes(Files[i].Bytes);
            }

            bw.FillInt32("FileSize", (int)bw.Position);
        }

        /// <summary>
        /// A <see cref="File"/> in a <see cref="DVDBND"/>.
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
            internal File(BinaryReaderEx br)
            {
                ID = br.ReadInt32();
                int dataOffset = br.ReadInt32();
                int dataSize = br.ReadInt32();
                int nameOffset = br.ReadInt32();

                Name = br.GetShiftJIS(nameOffset);
                Bytes = br.GetBytes(dataOffset, dataSize);
            }

            /// <summary>
            /// Writes this <see cref="File"/> entry to a stream.
            /// </summary>
            internal void Write(BinaryWriterEx bw, int index)
            {
                bw.WriteInt32(ID);
                bw.ReserveInt32($"DataOffset_{index}");
                bw.WriteInt32(Bytes.Length);
                bw.ReserveInt32($"NameOffset_{index}");
            }
        }
    }
}
