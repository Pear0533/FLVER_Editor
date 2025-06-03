using System;
using System.Collections.Generic;
using System.IO;

namespace SoulsFormats
{
    /// <summary>
    /// A Binder2 archive.<para/>
    /// Has been seen in the following games:<br/>
    /// Metal Wolf Chaos<br/>
    /// Another Century's Episode 2<br/>
    /// Armored Core: Formula Front (PSP and PS2)<br/>
    /// Armored Core: Nine Breaker<br/>
    /// Armored Core: Last Raven (PS2 and PSP)
    /// <para>Build settings for a file of this format were found in Armored Core Formula Front PSP which mentioned an app named Binder2.</para>
    /// </summary>
    public class BND2 : SoulsFile<BND2>, IBND2
    {
        /// <summary>
        /// An enum for the different supported file path modes.
        /// </summary>
        public enum FilePathModeEnum : byte
        {
            /// <summary>
            /// Files in this BND have no name.
            /// </summary>
            Nameless = 0,

            /// <summary>
            /// Files in this BND only have file names.
            /// </summary>
            FileName = 1,

            /// <summary>
            /// All files use a full file path.
            /// </summary>
            FullPath = 2,

            /// <summary>
            /// Add a base directory all paths start from, then write the rest of the path as each file name.
            /// </summary>
            BaseDirectory = 3
        }

        /// <summary>
        /// Header Info flags describing what features are enabled.<br/>
        /// Makes dubious assumptions on what the flags are.
        /// </summary>
        [Flags]
        public enum HeaderInfoFlagsEnum : byte
        {
            /// <summary>
            /// Unknown.
            /// </summary>
            HeaderItem = 0b00000001,

            /// <summary>
            /// Unknown.
            /// </summary>
            Endian = 0b00000010,

            /// <summary>
            /// Unknown.
            /// </summary>
            FileVersion = 0b00000100,

            /// <summary>
            /// Unknown.
            /// </summary>
            FileSize = 0b00001000,

            /// <summary>
            /// Unknown.
            /// </summary>
            FileNum = 0b00010000,

            /// <summary>
            /// Unknown.
            /// </summary>
            BaseDirOffset = 0b00100000,

            /// <summary>
            /// Unknown.
            /// </summary>
            AlignmentSize = 0b01000000,

            /// <summary>
            /// Unknown.
            /// </summary>
            Option = 0b10000000
        }

        /// <summary>
        /// File Info flags describing what features are enabled.<br/>
        /// Makes dubious assumptions on what the flags are.
        /// </summary>
        [Flags]
        public enum FileInfoFlagsEnum : byte
        {
            /// <summary>
            /// Unknown, likely whether or not ID is included.
            /// </summary>
            ID = 0b00000001,

            /// <summary>
            /// Unknown, likely whether or not Offset is included.
            /// </summary>
            Offset = 0b00000010,

            /// <summary>
            /// Unknown, likely whether or not Size is included.
            /// </summary>
            Size = 0b00000100,

            /// <summary>
            /// Whether or not NameOffset is included.
            /// </summary>
            NameOffset = 0b00001000,

            /// <summary>
            /// Unknown.
            /// </summary>
            Flag5 = 0b00010000,

            /// <summary>
            /// Unknown.
            /// </summary>
            Flag6 = 0b00100000,

            /// <summary>
            /// Unknown.
            /// </summary>
            Flag7 = 0b01000000,

            /// <summary>
            /// Unknown.
            /// </summary>
            Flag8 = 0b10000000
        }

        /// <summary>
        /// Header info flags?
        /// </summary>
        public HeaderInfoFlagsEnum HeaderInfoFlags { get; set; }

        /// <summary>
        /// File info flags.
        /// </summary>
        public FileInfoFlagsEnum FileInfoFlags { get; set; }

        /// <summary>
        /// Unknown.
        /// </summary>
        public byte Unk06 { get; set; }

        /// <summary>
        /// Endian?
        /// </summary>
        public byte Unk07 { get; set; }

        /// <summary>
        /// The version of this <see cref="BND2"/>.
        /// <para>Only 202 and 211 have been seen.</para>
        /// </summary>
        public int FileVersion { get; set; }

        /// <summary>
        /// The alignment of each <see cref="File"/>.
        /// <para>The bigger the aligment, the more empty bytes are added as padding. This increases the size of the archive.</para>
        /// </summary>
        public ushort AlignmentSize { get; set; }

        /// <summary>
        /// The file path mode determining how paths are handled.
        /// </summary>
        public FilePathModeEnum FilePathMode { get; set; }

        /// <summary>
        /// Unknown; Was found set to 1 on files extracted from memory.
        /// Usually set to 0.
        /// </summary>
        public byte Unk1B { get; set; }

        /// <summary>
        /// The base directory of all files.
        /// <para>Only used when <see cref="FilePathModeEnum.BaseDirectory"/> is set on <see cref="FilePathMode"/>.</para>
        /// </summary>
        public string BaseDirectory { get; set; }

        /// <summary>
        /// The files in this <see cref="BND2"/>.
        /// </summary>
        public List<File> Files { get; set; }

        /// <summary>
        /// Creates a <see cref="BND2"/>.
        /// </summary>
        public BND2()
        {
            HeaderInfoFlags = (HeaderInfoFlagsEnum)0xFF;
            FileInfoFlags = (FileInfoFlagsEnum)0xFF;
            Unk06 = 0x00;
            Unk07 = 0x00;
            FileVersion = 211;
            AlignmentSize = 2048;
            FilePathMode = FilePathModeEnum.FileName;
            Unk1B = 0;
            BaseDirectory = string.Empty;
            Files = new List<File>();
        }

        /// <summary>
        /// Creates a <see cref="BND2"/> with the specified version.
        /// </summary>
        public BND2(int version)
        {
            FileVersion = version;
            AlignmentSize = 2048;
            FilePathMode = FilePathModeEnum.FileName;
            Unk1B = 0;
            BaseDirectory = string.Empty;
            Files = new List<File>();
        }

        /// <summary>
        /// Creates a <see cref="BND2"/> with the specified <see cref="FilePathMode"/>.
        /// </summary>
        public BND2(FilePathModeEnum filePathMode)
        {
            FileVersion = 211;
            AlignmentSize = 2048;
            FilePathMode = filePathMode;
            Unk1B = 0;
            BaseDirectory = string.Empty;
            Files = new List<File>();
        }

        /// <summary>
        /// Creates a <see cref="BND2"/> with the specified version and <see cref="FilePathMode"/>.
        /// </summary>
        public BND2(int version, FilePathModeEnum filePathMode)
        {
            FileVersion = version;
            AlignmentSize = 2048;
            FilePathMode = filePathMode;
            Unk1B = 0;
            BaseDirectory = string.Empty;
            Files = new List<File>();
        }

        /// <summary>
        /// Returns true if the data appears to be a <see cref="BND2"/>.
        /// </summary>
        protected override bool Is(BinaryReaderEx br)
        {
            if (br.Length < 32)
                return false;

            string magic = br.ReadASCII(4);
            br.Position += 4; // Flags1, Flags2, Unk06, Unk07
            int fileVersion = br.ReadInt32();
            br.Position += 8; // File Size, File Num
            int baseDirOffset = br.ReadInt32();
            br.Position += 2; // Alignment Size
            byte filePathMode = br.ReadByte();
            byte unk1B = br.ReadByte();
            uint unk1C = br.ReadUInt32();

            // All file path modes except for one have namesOffset set to 0.
            bool validNamesOffset;
            switch (filePathMode)
            {
                case 0:
                case 1:
                case 2:
                    validNamesOffset = baseDirOffset <= br.Length && baseDirOffset == 0;
                    break;
                case 3:
                    validNamesOffset = baseDirOffset <= br.Length;
                    break;
                default:
                    // File path mode was invalid
                    return false;
            }

            bool validMagic = magic == "BND\0";
            bool expectedFileVersion = fileVersion >= 202 && fileVersion <= 211;
            bool expectedUnk1B = unk1B == 0 || unk1B == 1;
            bool expectedUnk1C = unk1C == 0;
            return validMagic && expectedFileVersion && validNamesOffset && expectedUnk1B && expectedUnk1C;
        }

        /// <summary>
        /// Reads a <see cref="BND2"/> from a stream.
        /// </summary>
        protected override void Read(BinaryReaderEx br)
        {
            List<BND2FileHeader> fileHeaders = ReadHeader(this, br);
            Files = new List<File>(fileHeaders.Count);
            foreach (BND2FileHeader fileHeader in fileHeaders)
                Files.Add(fileHeader.ReadFileData(br));
        }

        internal static List<BND2FileHeader> ReadHeader(IBND2 bnd, BinaryReaderEx br)
        {
            br.BigEndian = false;

            br.AssertASCII("BND\0");
            bnd.HeaderInfoFlags = (HeaderInfoFlagsEnum)br.ReadByte();
            bnd.FileInfoFlags = (FileInfoFlagsEnum)br.ReadByte();
            bnd.Unk06 = br.ReadByte();
            bnd.Unk07 = br.ReadByte();
            bnd.FileVersion = br.ReadInt32(); // Versions between 202 and 211 not seen.
            br.Position += 4; // File Size
            int fileCount = br.ReadInt32();
            int baseDirOffset = br.ReadInt32();
            bnd.AlignmentSize = br.ReadUInt16();
            bnd.FilePathMode = br.ReadEnum8<FilePathModeEnum>();
            bnd.Unk1B = br.AssertByte(0, 1);
            br.AssertUInt32(0);

            // Odd
            if ((bnd.FileInfoFlags & FileInfoFlagsEnum.NameOffset) == 0)
            {
                br.AssertUInt32(0);
            }

            if (bnd.FilePathMode == FilePathModeEnum.BaseDirectory && (bnd.FileInfoFlags & FileInfoFlagsEnum.NameOffset) != 0)
            {
                bnd.BaseDirectory = br.GetShiftJIS(baseDirOffset);
            }
            else
            {
                bnd.BaseDirectory = string.Empty;
            }

            var fileHeaders = new List<BND2FileHeader>(fileCount);
            for (int i = 0; i < fileCount; i++)
            {
                fileHeaders.Add(BND2FileHeader.ReadBinder2FileHeader(br, bnd.FilePathMode, bnd.FileInfoFlags));
            }

            return fileHeaders;
        }

        /// <summary>
        /// Writes this <see cref="BND2"/> to a stream.
        /// </summary>
        protected override void Write(BinaryWriterEx bw)
        {
            var fileHeaders = new List<BND2FileHeader>(Files.Count);
            foreach (File file in Files)
                fileHeaders.Add(new BND2FileHeader(file));

            WriteHeader(this, bw, fileHeaders);
            for (int i = 0; i < Files.Count; i++)
                fileHeaders[i].WriteBinder2FileData(bw, bw, i, AlignmentSize, Files[i].Bytes);

            bw.FillInt32("fileSize", (int)bw.Position);
        }

        internal static void WriteHeader(IBND2 bnd, BinaryWriterEx bw, List<BND2FileHeader> fileHeaders)
        {
            bw.BigEndian = false;

            bw.WriteASCII("BND\0");
            bw.WriteByte((byte)bnd.HeaderInfoFlags);
            bw.WriteByte((byte)bnd.FileInfoFlags);
            bw.WriteByte(bnd.Unk06);
            bw.WriteByte(bnd.Unk07);
            bw.WriteInt32(bnd.FileVersion);
            bw.ReserveInt32("fileSize");
            bw.WriteInt32(fileHeaders.Count);
            bw.ReserveInt32("baseDirOffset");
            bw.WriteUInt16(bnd.AlignmentSize);
            bw.WriteByte((byte)bnd.FilePathMode);
            bw.WriteByte(bnd.Unk1B);
            bw.WriteUInt32(0);

            // Odd
            if ((bnd.FileInfoFlags & FileInfoFlagsEnum.NameOffset) == 0)
            {
                bw.WriteUInt32(0);
            }

            for (int i = 0; i < fileHeaders.Count; i++)
            {
                fileHeaders[i].WriteBinder2FileHeader(bw, bnd.FilePathMode, bnd.FileInfoFlags, i);
            }

            if ((bnd.FileInfoFlags & FileInfoFlagsEnum.NameOffset) != 0)
            {
                WriteFileNames(bw, bnd.BaseDirectory, bnd.FilePathMode, fileHeaders);
            }
            else
            {
                bw.FillInt32("baseDirOffset", 0);
            }
        }

        private static void WriteFileNames(BinaryWriterEx bw, string baseDirectory, FilePathModeEnum filePathMode, List<BND2FileHeader> fileHeaders)
        {
            if (filePathMode == FilePathModeEnum.BaseDirectory)
            {
                bw.FillInt32("baseDirOffset", (int)bw.Position);
                bw.WriteShiftJIS(baseDirectory, true);
            }
            else
            {
                bw.FillInt32("baseDirOffset", 0);
            }

            if (filePathMode != FilePathModeEnum.Nameless)
            {
                for (int i = 0; i < fileHeaders.Count; i++)
                {
                    bw.FillInt32($"nameOffset_{i}", (int)bw.Position);
                    string name = fileHeaders[i].Name;
                    if (filePathMode == FilePathModeEnum.FullPath)
                    {
                        if (!Path.IsPathRooted(name))
                        {
                            name = Path.Combine("K:\\", name);
                        }
                    }

                    bw.WriteShiftJIS(name, true);
                }
            }
        }

        /// <summary>
        /// A file in a <see cref="BND2"/>.
        /// </summary>
        public class File
        {
            /// <summary>
            /// The ID of this <see cref="File"/>.
            /// </summary>
            public int ID { get; set; }

            /// <summary>
            /// The name of this <see cref="File"/>.
            /// <para>Will be set to ID if name does not exist.</para>
            /// <para>Will be a path with a drive letter if <see cref="FilePathModeEnum.FullPath"/> is set.</para>
            /// <para>Will need <see cref="BaseDirectory"/> added as the base directory if <see cref="FilePathModeEnum.BaseDirectory"/> is set.</para>
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
                Name = string.Empty;
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
            internal File(BinaryReaderEx br, FilePathModeEnum filePathMode, FileInfoFlagsEnum fileInfoFlags)
            {
                ID = br.ReadInt32();
                int offset = br.ReadInt32();
                int size = br.ReadInt32();

                if ((fileInfoFlags & FileInfoFlagsEnum.NameOffset) != 0)
                {
                    int nameOffset = br.ReadInt32();

                    switch (filePathMode)
                    {
                        case FilePathModeEnum.Nameless:
                            Name = ID.ToString();
                            break;
                        case FilePathModeEnum.FileName:
                        case FilePathModeEnum.FullPath:
                        case FilePathModeEnum.BaseDirectory:
                            Name = br.GetShiftJIS(nameOffset);
                            break;
                        default:
                            throw new NotSupportedException($"{nameof(filePathMode)} {filePathMode} is not supported.");
                    }
                }
                else
                {
                    Name = ID.ToString();
                }

                Bytes = br.GetBytes(offset, size);
            }

            /// <summary>
            /// Writes this <see cref="File"/> entry to a stream.
            /// </summary>
            internal void Write(BinaryWriterEx bw, FilePathModeEnum filePathMode, FileInfoFlagsEnum fileInfoFlags, int index)
            {
                bw.WriteInt32(ID);
                bw.ReserveInt32($"fileOffset_{index}");
                bw.WriteInt32(Bytes.Length);

                if ((fileInfoFlags & FileInfoFlagsEnum.NameOffset) != 0)
                {
                    if (filePathMode == FilePathModeEnum.Nameless)
                    {
                        bw.WriteInt32(0);
                    }
                    else
                    {
                        bw.ReserveInt32($"nameOffset_{index}");
                    }
                }
            }
        }
    }
}
