using System;
using System.Collections.Generic;

namespace SoulsFormats.Other.AC3SL
{
    /// <summary>
    /// The format of the Binder file found in Armored Core 3 Silent Line.
    /// </summary>
    public class BND : SoulsFile<BND>
    {
        /// <summary>
        /// Presumed to be an 8 char file version.
        /// <para>Only seen as "LTL"</para>
        /// </summary>
        public string FileVersion { get; set; }

        /// <summary>
        /// The alignment of each <see cref="File"/>.
        /// <para>The bigger the aligment, the more empty bytes are added as padding. This increases the size of the archive.</para>
        /// </summary>
        public short AlignmentSize { get; set; }

        /// <summary>
        /// Unknown; Seen set to 4.
        /// </summary>
        public short Unk1E { get; set; }

        /// <summary>
        /// Files in this <see cref="BND"/>.
        /// </summary>
        public List<File> Files { get; set; }

        /// <summary>
        /// Creates a <see cref="BND"/>.
        /// </summary>
        public BND()
        {
            FileVersion = "LTL";
            AlignmentSize = 2048;
            Unk1E = 4;
            Files = new List<File>();
        }

        /// <summary>
        /// Returns true if the data appears to be a <see cref="BND"/> of this type.
        /// </summary>
        protected override bool Is(BinaryReaderEx br)
        {
            // Ensure there is enough length to even read the header
            int headerLength = 32;
            if (br.Length < headerLength)
            {
                return false;
            }

            // Ensure the magic is valid
            bool validMagic = br.ReadASCII(4) == "BND\0";
            if (!validMagic)
            {
                return false;
            }

            br.Position += 8; // File Version
            br.Position += 4; // File Size
            int fileNum = br.ReadInt32();
            int totalFileSize = br.ReadInt32();
            bool expectedUnk18 = br.ReadInt32() == 0;
            br.Position += 4; // Alignment Size, Unk1E

            int fileEntrySize = 16;
            int entryLength = fileNum * fileEntrySize;
            int headerEntryLength = headerLength + entryLength;

            // Ensure file is long enough for the possible header + entry length
            if (br.Length < headerEntryLength)
            {
                return false;
            }

            int detectedTotalFileSize = 0;
            int previousOffset = -1;
            for (int i = 0; i < fileNum; i++)
            {
                br.Position += 4; // ID
                int size = br.ReadInt32();
                int offset = br.ReadInt32();

                // Ensure offset does not leave file
                if (offset > br.Length)
                {
                    return false;
                }

                // Ensure offset is not less than the previous offset
                if (size > 0 && offset < previousOffset)
                {
                    return false;
                }

                // Ensure that if size is greater than 0, offset is not 0 and does not land before the data even starts
                if (size > 0 && offset < headerEntryLength)
                {
                    return false;
                }

                if (br.ReadInt32() != 0) // Unk04
                {
                    return false;
                }

                detectedTotalFileSize += size;
                previousOffset = offset;
            }

            bool validTotalFileSize = detectedTotalFileSize == totalFileSize;
            return expectedUnk18 && validTotalFileSize;
        }

        /// <summary>
        /// Reads a <see cref="BND"/> from a stream.
        /// </summary>
        protected override void Read(BinaryReaderEx br)
        {
            br.BigEndian = false;
            br.AssertASCII("BND\0");
            br.ReadASCII(8);
            br.Position += 4; // File Size
            int fileNum = br.ReadInt32();
            br.Position += 4; // Total File Size (Not including padding)
            br.AssertInt32(0);
            AlignmentSize = br.ReadInt16();
            Unk1E = br.ReadInt16();

            Files = new List<File>(fileNum);
            for (int i = 0; i < fileNum; i++)
            {
                Files.Add(new File(br));
            }
        }

        /// <summary>
        /// Writes this <see cref="BND"/> to a stream.
        /// </summary>
        protected override void Write(BinaryWriterEx bw)
        {
            bw.BigEndian = false;
            bw.WriteASCII("BND\0");
            bw.WriteFixStr(FileVersion, 8);
            bw.ReserveInt32("FileSize");
            bw.WriteInt32(Files.Count);
            bw.ReserveInt32("TotalFileSize");
            bw.WriteInt32(0);
            bw.WriteInt16(AlignmentSize);
            bw.WriteInt16(Unk1E);

            for (int i = 0; i < Files.Count; i++)
            {
                Files[i].Write(bw, i);
            }
            bw.Pad(AlignmentSize);

            int totalFileSize = 0;
            for (int i = 0; i < Files.Count; i++)
            {
                if (Files[i].Bytes.Length > 0)
                {
                    bw.FillInt32($"FileOffset_{i}", (int)bw.Position);
                }

                bw.WriteBytes(Files[i].Bytes);
                bw.Pad(AlignmentSize);
                totalFileSize += Files[i].Bytes.Length;
            }
            bw.FillInt32("TotalFileSize", totalFileSize);
            bw.FillInt32("FileSize", (int)bw.Position);
        }

        /// <summary>
        /// A file in a <see cref="BND"/>.
        /// </summary>
        public class File
        {
            /// <summary>
            /// The ID of this <see cref="File"/>.
            /// </summary>
            public int ID { get; set; }

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
                Bytes = Array.Empty<byte>();
            }

            /// <summary>
            /// Creates a <see cref="File"/> with an ID.
            /// </summary>
            public File(int id)
            {
                ID = id;
                Bytes = Array.Empty<byte>();
            }

            /// <summary>
            /// Creates a <see cref="File"/> with bytes.
            /// </summary>
            public File(byte[] bytes)
            {
                ID = -1;
                Bytes = bytes;
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
            /// Reads a <see cref="File"/> from a stream.
            /// </summary>
            internal File(BinaryReaderEx br)
            {
                ID = br.ReadInt32();
                int size = br.ReadInt32();
                int offset = br.ReadInt32();
                br.AssertInt32(0); // Potential name offset?

                if (offset > 0)
                {
                    if (size > 0)
                    {
                        Bytes = br.GetBytes(offset, size);
                    }
                    else
                    {
                        Bytes = Array.Empty<byte>();
                    }
                }
                else
                {
                    Bytes = Array.Empty<byte>();
                }
            }

            /// <summary>
            /// Writes this <see cref="File"/> entry to a stream.
            /// </summary>
            internal void Write(BinaryWriterEx bw, int index)
            {
                bw.WriteInt32(ID);
                bw.WriteInt32(Bytes.Length);

                if (Bytes.Length > 0)
                {
                    bw.ReserveInt32($"FileOffset_{index}");
                }
                else
                {
                    bw.WriteInt32(0);
                }

                bw.WriteInt32(0);
            }
        }
    }
}
