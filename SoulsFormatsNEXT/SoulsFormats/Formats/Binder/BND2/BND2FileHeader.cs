using System;

namespace SoulsFormats
{
    /// <summary>
    /// Metadata for a file in a <see cref="BND2"/>.
    /// </summary>
    public class BND2FileHeader
    {
        /// <summary>
        /// The ID of this file.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The name of this file.
        /// <para>Will be set to ID if name does not exist.</para>
        /// <para>Will be a path with a drive letter if <see cref="BND2.FilePathModeEnum.FullPath"/> is set.</para>
        /// <para>Will need <see cref="BND2.BaseDirectory"/> added as the base directory if <see cref="BND2.FilePathModeEnum.BaseDirectory"/> is set.</para>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Location of file data in the <see cref="BND2"/>. Do not modify unless you know what you're doing.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Size of the file. Do not modify unless you know what you're doing.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Create a new <see cref="BND2FileHeader"/>.
        /// </summary>
        /// <param name="offset">The location of file data.</param>
        /// <param name="size">The size of the file.</param>
        /// <param name="id">The ID of the file.</param>
        /// <param name="name">The name of the file.</param>
        public BND2FileHeader(int offset, int size, int id = -1, string name = "")
        {
            ID = id;
            Name = name;
            Name = name;
            Offset = offset;
            Size = size;
        }

        internal BND2FileHeader(BND2.File file) : this(-1, -1, file.ID, file.Name)
        {
        
        }

        internal static BND2FileHeader ReadBinder2FileHeader(BinaryReaderEx br, BND2.FilePathModeEnum filePathMode, BND2.FileInfoFlagsEnum fileInfoFlags)
        {
            int id = br.ReadInt32();
            int offset = br.ReadInt32();
            int size = br.ReadInt32();

            string name;
            if ((fileInfoFlags & BND2.FileInfoFlagsEnum.NameOffset) != 0)
            {
                int nameOffset = br.ReadInt32();

                switch (filePathMode)
                {
                    case BND2.FilePathModeEnum.Nameless:
                        name = id.ToString();
                        break;
                    case BND2.FilePathModeEnum.FileName:
                    case BND2.FilePathModeEnum.FullPath:
                    case BND2.FilePathModeEnum.BaseDirectory:
                        name = br.GetShiftJIS(nameOffset);
                        break;
                    default:
                        throw new NotSupportedException($"{nameof(filePathMode)} {filePathMode} is not supported.");
                }
            }
            else
            {
                name = string.Empty;
            }

            return new BND2FileHeader(offset, size, id, name);
        }

        internal void WriteBinder2FileHeader(BinaryWriterEx bw, BND2.FilePathModeEnum filePathMode, BND2.FileInfoFlagsEnum fileInfoFlags, int index)
        {
            bw.WriteInt32(ID);
            bw.ReserveInt32($"fileOffset_{index}");
            bw.ReserveInt32($"fileSize_{index}");

            if ((fileInfoFlags & BND2.FileInfoFlagsEnum.NameOffset) != 0)
            {
                if (filePathMode == BND2.FilePathModeEnum.Nameless)
                {
                    bw.WriteInt32(0);
                }
                else
                {
                    bw.ReserveInt32($"nameOffset_{index}");
                }
            }
        }

        internal BND2.File ReadFileData(BinaryReaderEx br)
        {
            byte[] bytes = br.GetBytes(Offset, Size);
            return new BND2.File(ID, Name, bytes);
        }

        private void WriteFileData(BinaryWriterEx bw, byte[] bytes)
        {
            Offset = (int)bw.Position;
            Size = bytes.Length;
            bw.WriteBytes(bytes);
        }

        internal void WriteBinder2FileData(BinaryWriterEx bwHeader, BinaryWriterEx bwData, int index, ushort alignmentSize, byte[] bytes)
        {
            bwData.Pad(alignmentSize);
            WriteFileData(bwData, bytes);
            bwHeader.FillInt32($"fileOffset_{index}", Offset);
            bwHeader.FillInt32($"fileSize_{index}", Size);
        }

        /// <summary>
        /// Returns a string representation of the object.
        /// </summary>
        public override string ToString()
        {
            return $"{ID} {Name}";
        }
    }
}
