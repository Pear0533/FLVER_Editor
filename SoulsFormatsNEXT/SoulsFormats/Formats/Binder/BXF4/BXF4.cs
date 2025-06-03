using System;
using System.Collections.Generic;
using System.IO;

namespace SoulsFormats
{
    /// <summary>
    /// A general-purpose split header and data binder.
    /// <para>Header Extensions: .bhd, .*bhd</para>
    /// <para>Data Extensions: .bdt, .*bdt</para>
    /// </summary>
    public class BXF4 : IBinder, IBXF4
    {
        /// <summary>
        /// The files contained within this <see cref="BXF4"/>.
        /// </summary>
        public List<BinderFile> Files { get; set; }

        /// <summary>
        /// A timestamp or version number, 8 characters maximum.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Indicates the format of the <see cref="BXF4"/>.
        /// </summary>
        public Binder.Format Format { get; set; }

        /// <summary>
        /// Unknown.
        /// </summary>
        public bool Unk04 { get; set; }

        /// <summary>
        /// Unknown.
        /// </summary>
        public bool Unk05 { get; set; }

        /// <summary>
        /// Whether to use big-endian byte ordering.
        /// </summary>
        public bool BigEndian { get; set; }

        /// <summary>
        /// Controls ordering of flag bits.
        /// </summary>
        public bool BitBigEndian { get; set; }

        /// <summary>
        /// Whether to write strings in UTF-16.
        /// </summary>
        public bool Unicode { get; set; }

        /// <summary>
        /// Indicates the presence of a filename hash table.
        /// </summary>
        public byte Extended { get; set; }

        /// <summary>
        /// Creates an empty <see cref="BXF4"/> formatted for DS3.
        /// </summary>
        public BXF4()
        {
            Files = new List<BinderFile>();
            Version = Binder.DateToBinderTimestamp(DateTime.Now);
            Unicode = true;
            Format = Binder.Format.IDs | Binder.Format.Names1 | Binder.Format.Names2 | Binder.Format.Compression;
            Extended = 4;
        }

        private BXF4(BinaryReaderEx bhdReader, BinaryReaderEx bdtReader)
        {
            ReadBDFHeader(bdtReader);
            List<BinderFileHeader> fileHeaders = ReadBHFHeader(this, bhdReader);
            Files = new List<BinderFile>(fileHeaders.Count);
            foreach (BinderFileHeader fileHeader in fileHeaders)
            {
                Files.Add(fileHeader.ReadFileData(bdtReader));
            }
        }

        #region Read

        /// <summary>
        /// Reads a <see cref="BXF4"/> using (paths.
        /// </summary>
        /// <param name="bhdPath">The header path.</param>
        /// <param name="bdtBytes">The data path.</param>
        /// <returns>A new <see cref="BXF4"/>.</returns>
        public static BXF4 Read(string bhdPath, string bdtBytes)
        {
            using (BinaryReaderEx bhdReader = new BinaryReaderEx(false, bhdPath))
            using (BinaryReaderEx bdtReader = new BinaryReaderEx(false, bdtBytes))
            using (BinaryReaderEx bhdReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bhdReader, out DCX.CompressionInfo _))
            using (BinaryReaderEx bdtReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bdtReader, out DCX.CompressionInfo _))
            {
                return new BXF4(bhdReaderDecompressed, bdtReaderDecompressed);
            }
        }

        /// <summary>
        /// Reads a <see cref="BXF4"/> using (a path and bytes.
        /// </summary>
        /// <param name="bhdPath">The header path.</param>
        /// <param name="bdtBytes">The data bytes.</param>
        /// <returns>A new <see cref="BXF4"/>.</returns>
        public static BXF4 Read(string bhdPath, byte[] bdtBytes)
        {
            using (BinaryReaderEx bhdReader = new BinaryReaderEx(false, bhdPath))
            using (BinaryReaderEx bdtReader = new BinaryReaderEx(false, bdtBytes))
            using (BinaryReaderEx bhdReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bhdReader, out DCX.CompressionInfo _))
            using (BinaryReaderEx bdtReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bdtReader, out DCX.CompressionInfo _))
            {
                return new BXF4(bhdReaderDecompressed, bdtReaderDecompressed);
            }
        }

        /// <summary>
        /// Reads a <see cref="BXF4"/> using (a path and a stream.
        /// </summary>
        /// <param name="bhdPath">The header path.</param>
        /// <param name="bdtStream">The data stream.</param>
        /// <returns>A new <see cref="BXF4"/>.</returns>
        public static BXF4 Read(string bhdPath, Stream bdtStream)
        {
            using (BinaryReaderEx bhdReader = new BinaryReaderEx(false, bhdPath))
            using (BinaryReaderEx bdtReader = new BinaryReaderEx(false, bdtStream, true))
            using (BinaryReaderEx bhdReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bhdReader, out DCX.CompressionInfo _))
            using (BinaryReaderEx bdtReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bdtReader, out DCX.CompressionInfo _))
            {
                return new BXF4(bhdReaderDecompressed, bdtReaderDecompressed);
            }
        }

        /// <summary>
        /// Reads a <see cref="BXF4"/> using (bytes.
        /// </summary>
        /// <param name="bhdBytes">The header bytes.</param>
        /// <param name="bdtBytes">The data bytes.</param>
        /// <returns>A new <see cref="BXF4"/>.</returns>
        public static BXF4 Read(byte[] bhdBytes, byte[] bdtBytes)
        {
            using (BinaryReaderEx bhdReader = new BinaryReaderEx(false, bhdBytes))
            using (BinaryReaderEx bdtReader = new BinaryReaderEx(false, bdtBytes))
            using (BinaryReaderEx bhdReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bhdReader, out DCX.CompressionInfo _))
            using (BinaryReaderEx bdtReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bdtReader, out DCX.CompressionInfo _))
            {
                return new BXF4(bhdReaderDecompressed, bdtReaderDecompressed);
            }
        }

        /// <summary>
        /// Reads a <see cref="BXF4"/> using (bytes and a path.
        /// </summary>
        /// <param name="bhdBytes">The header bytes.</param>
        /// <param name="bdtPath">The data path.</param>
        /// <returns>A new <see cref="BXF4"/>.</returns>
        public static BXF4 Read(byte[] bhdBytes, string bdtPath)
        {
            using (BinaryReaderEx bhdReader = new BinaryReaderEx(false, bhdBytes))
            using (BinaryReaderEx bdtReader = new BinaryReaderEx(false, bdtPath))
            using (BinaryReaderEx bhdReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bhdReader, out DCX.CompressionInfo _))
            using (BinaryReaderEx bdtReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bdtReader, out DCX.CompressionInfo _))
            {
                return new BXF4(bhdReaderDecompressed, bdtReaderDecompressed);
            }
        }

        /// <summary>
        /// Reads a <see cref="BXF4"/> using (bytes and a stream.
        /// </summary>
        /// <param name="bhdBytes">The header bytes.</param>
        /// <param name="bdtStream">The data stream.</param>
        /// <returns>A new <see cref="BXF4"/>.</returns>
        public static BXF4 Read(byte[] bhdBytes, Stream bdtStream)
        {
            using (BinaryReaderEx bhdReader = new BinaryReaderEx(false, bhdBytes))
            using (BinaryReaderEx bdtReader = new BinaryReaderEx(false, bdtStream, true))
            using (BinaryReaderEx bhdReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bhdReader, out DCX.CompressionInfo _))
            using (BinaryReaderEx bdtReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bdtReader, out DCX.CompressionInfo _))
            {
                return new BXF4(bhdReaderDecompressed, bdtReaderDecompressed);
            }
        }

        /// <summary>
        /// Reads a <see cref="BXF4"/> using (streams.
        /// </summary>
        /// <param name="bhdStream">The header stream.</param>
        /// <param name="bdtStream">The data stream.</param>
        /// <returns>A new <see cref="BXF4"/>.</returns>
        public static BXF4 Read(Stream bhdStream, Stream bdtStream)
        {
            using (BinaryReaderEx bhdReader = new BinaryReaderEx(false, bhdStream, true))
            using (BinaryReaderEx bdtReader = new BinaryReaderEx(false, bdtStream, true))
            using (BinaryReaderEx bhdReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bhdReader, out DCX.CompressionInfo _))
            using (BinaryReaderEx bdtReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bdtReader, out DCX.CompressionInfo _))
            {
                return new BXF4(bhdReaderDecompressed, bdtReaderDecompressed);
            }
        }

        /// <summary>
        /// Reads a <see cref="BXF4"/> using (a stream and a path.
        /// </summary>
        /// <param name="bhdStream">The header stream.</param>
        /// <param name="bdtPath">The data path.</param>
        /// <returns>A new <see cref="BXF4"/>.</returns>
        public static BXF4 Read(Stream bhdStream, string bdtPath)
        {
            using (BinaryReaderEx bhdReader = new BinaryReaderEx(false, bhdStream, true))
            using (BinaryReaderEx bdtReader = new BinaryReaderEx(false, bdtPath))
            using (BinaryReaderEx bhdReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bhdReader, out DCX.CompressionInfo _))
            using (BinaryReaderEx bdtReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bdtReader, out DCX.CompressionInfo _))
            {
                return new BXF4(bhdReaderDecompressed, bdtReaderDecompressed);
            }
        }

        /// <summary>
        /// Reads a <see cref="BXF4"/> using (a stream and bytes.
        /// </summary>
        /// <param name="bhdStream">The header stream.</param>
        /// <param name="bdtBytes">The data bytes.</param>
        /// <returns>A new <see cref="BXF4"/>.</returns>
        public static BXF4 Read(Stream bhdStream, byte[] bdtBytes)
        {
            using (BinaryReaderEx bhdReader = new BinaryReaderEx(false, bhdStream, true))
            using (BinaryReaderEx bdtReader = new BinaryReaderEx(false, bdtBytes))
            using (BinaryReaderEx bhdReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bhdReader, out DCX.CompressionInfo _))
            using (BinaryReaderEx bdtReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bdtReader, out DCX.CompressionInfo _))
            {
                return new BXF4(bhdReaderDecompressed, bdtReaderDecompressed);
            }
        }

        // I am very tempted to preserve these since they don't always match the BHF,
        // but it makes the API messy and they don't actually do anything.
        internal static void ReadBDFHeader(BinaryReaderEx br)
        {
            br.AssertASCII("BDF4");
            br.ReadBoolean(); // Unk04
            br.ReadBoolean(); // Unk05
            br.AssertByte(0);
            br.AssertByte(0);
            br.AssertByte(0);
            br.BigEndian = br.ReadBoolean();
            br.ReadBoolean(); // BitBigEndian
            br.AssertByte(0);
            br.AssertInt32(0);
            br.AssertInt64(0x30, 0x40); // Header size, pretty sure 0x40 is just a mistake
            br.ReadFixStr(8); // Version
            br.AssertInt64(0);
            br.AssertInt64(0);
        }

        internal static List<BinderFileHeader> ReadBHFHeader(IBXF4 bxf, BinaryReaderEx br)
        {
            br.AssertASCII("BHF4");

            bxf.Unk04 = br.ReadBoolean();
            bxf.Unk05 = br.ReadBoolean();
            br.AssertByte(0);
            br.AssertByte(0);

            br.AssertByte(0);
            bxf.BigEndian = br.ReadBoolean();
            bxf.BitBigEndian = !br.ReadBoolean();
            br.AssertByte(0);

            br.BigEndian = bxf.BigEndian;

            int fileCount = br.ReadInt32();
            br.AssertInt64(0x40); // Header size
            bxf.Version = br.ReadFixStr(8);
            long fileHeaderSize = br.ReadInt64();
            br.AssertInt64(0);

            bxf.Unicode = br.ReadBoolean();
            bxf.Format = Binder.ReadFormat(br, bxf.BitBigEndian);
            bxf.Extended = br.AssertByte(0, 4);
            br.AssertByte(0);

            if (fileHeaderSize != Binder.GetBND4FileHeaderSize(bxf.Format))
                throw new FormatException($"File header size for format {bxf.Format} is expected to be 0x{Binder.GetBND4FileHeaderSize(bxf.Format):X}, but was 0x{fileHeaderSize:X}");

            br.AssertInt32(0);

            if (bxf.Extended == 4)
            {
                long hashGroupsOffset = br.ReadInt64();
                br.StepIn(hashGroupsOffset);
                BinderHashTable.Assert(br);
                br.StepOut();
            }
            else
            {
                br.AssertInt64(0);
            }

            var fileHeaders = new List<BinderFileHeader>(fileCount);
            for (int i = 0; i < fileCount; i++)
                fileHeaders.Add(BinderFileHeader.ReadBinder4FileHeader(br, bxf.Format, bxf.BitBigEndian, bxf.Unicode));

            return fileHeaders;
        }

        #endregion

        #region Write

        /// <summary>
        /// Writes the BHD and BDT as two arrays of bytes.
        /// </summary>
        public void Write(out byte[] bhdBytes, out byte[] bdtBytes)
        {
            BinaryWriterEx bhdWriter = new BinaryWriterEx(false);
            BinaryWriterEx bdtWriter = new BinaryWriterEx(false);
            Write(bhdWriter, bdtWriter);
            bhdBytes = bhdWriter.FinishBytes();
            bdtBytes = bdtWriter.FinishBytes();
        }

        /// <summary>
        /// Writes the BHD as an array of bytes and the BDT as a file.
        /// </summary>
        public void Write(out byte[] bhdBytes, string bdtPath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(bdtPath));
            using (FileStream bdtStream = File.Create(bdtPath))
            {
                BinaryWriterEx bhdWriter = new BinaryWriterEx(false);
                BinaryWriterEx bdtWriter = new BinaryWriterEx(false, bdtStream);
                Write(bhdWriter, bdtWriter);
                bdtWriter.Finish();
                bhdBytes = bhdWriter.FinishBytes();
            }
        }

        /// <summary>
        /// Writes the BHD as a file and the BDT as an array of bytes.
        /// </summary>
        public void Write(string bhdPath, out byte[] bdtBytes)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(bhdPath));
            using (FileStream bhdStream = File.Create(bhdPath))
            {
                BinaryWriterEx bhdWriter = new BinaryWriterEx(false, bhdStream);
                BinaryWriterEx bdtWriter = new BinaryWriterEx(false);
                Write(bhdWriter, bdtWriter);
                bhdWriter.Finish();
                bdtBytes = bdtWriter.FinishBytes();
            }
        }

        /// <summary>
        /// Writes the BHD and BDT as two files.
        /// </summary>
        public void Write(string bhdPath, string bdtPath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(bhdPath));
            Directory.CreateDirectory(Path.GetDirectoryName(bdtPath));
            using (FileStream bhdStream = File.Create(bhdPath))
            using (FileStream bdtStream = File.Create(bdtPath))
            {
                BinaryWriterEx bhdWriter = new BinaryWriterEx(false, bhdStream);
                BinaryWriterEx bdtWriter = new BinaryWriterEx(false, bdtStream);
                Write(bhdWriter, bdtWriter);
                bhdWriter.Finish();
                bdtWriter.Finish();
            }
        }

        private void Write(BinaryWriterEx bhdWriter, BinaryWriterEx bdtWriter)
        {
            var fileHeaders = new List<BinderFileHeader>(Files.Count);
            foreach (BinderFile file in Files)
                fileHeaders.Add(new BinderFileHeader(file));

            WriteBDFHeader(this, bdtWriter);
            WriteBHFHeader(this, bhdWriter, fileHeaders);
            for (int i = 0; i < Files.Count; i++)
                fileHeaders[i].WriteBinder4FileData(bhdWriter, bdtWriter, Format, i, Files[i].Bytes);
        }

        internal static void WriteBDFHeader(IBXF4 bxf, BinaryWriterEx bw)
        {
            bw.BigEndian = bxf.BigEndian;
            bw.WriteASCII("BDF4");
            bw.WriteBoolean(bxf.Unk04);
            bw.WriteBoolean(bxf.Unk05);
            bw.WriteByte(0);
            bw.WriteByte(0);
            bw.WriteByte(0);
            bw.WriteBoolean(bxf.BigEndian);
            bw.WriteBoolean(!bxf.BitBigEndian);
            bw.WriteByte(0);
            bw.WriteInt32(0);
            bw.WriteInt64(0x30);
            bw.WriteFixStr(bxf.Version, 8);
            bw.WriteInt64(0);
            bw.WriteInt64(0);
        }

        internal static void WriteBHFHeader(IBXF4 bxf, BinaryWriterEx bw, List<BinderFileHeader> fileHeaders)
        {
            bw.BigEndian = bxf.BigEndian;

            bw.WriteASCII("BHF4");

            bw.WriteBoolean(bxf.Unk04);
            bw.WriteBoolean(bxf.Unk05);
            bw.WriteByte(0);
            bw.WriteByte(0);

            bw.WriteByte(0);
            bw.WriteBoolean(bxf.BigEndian);
            bw.WriteBoolean(!bxf.BitBigEndian);
            bw.WriteByte(0);

            bw.WriteInt32(fileHeaders.Count);
            bw.WriteInt64(0x40);
            bw.WriteFixStr(bxf.Version, 8);
            bw.WriteInt64(Binder.GetBND4FileHeaderSize(bxf.Format));
            bw.WriteInt64(0);

            bw.WriteBoolean(bxf.Unicode);
            Binder.WriteFormat(bw, bxf.BitBigEndian, bxf.Format);
            bw.WriteByte(bxf.Extended);
            bw.WriteByte(0);

            bw.WriteInt32(0);
            bw.ReserveInt64("HashTableOffset");

            for (int i = 0; i < fileHeaders.Count; i++)
                fileHeaders[i].WriteBinder4FileHeader(bw, bxf.Format, bxf.BitBigEndian, i);

            for (int i = 0; i < fileHeaders.Count; i++)
                fileHeaders[i].WriteFileName(bw, bxf.Format, bxf.Unicode, i);

            if (bxf.Extended == 4)
            {
                bw.Pad(0x8);
                bw.FillInt64("HashTableOffset", bw.Position);
                BinderHashTable.Write(bw, fileHeaders);
            }
            else
            {
                bw.FillInt64("HashTableOffset", 0);
            }
        }

        #endregion

        #region Is

        /// <summary>
        /// Whether or not the data appears to be a header file.
        /// </summary>
        public static bool IsHeader(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (fs.Length < 4)
                {
                    return false;
                }

                using (BinaryReaderEx br = new BinaryReaderEx(false, fs))
                using (BinaryReaderEx brd = SFUtil.GetDecompressedBinaryReader(br, out DCX.CompressionInfo compression))
                {
                    return IsHeader(brd);
                }
            }
        }

        /// <summary>
        /// Whether or not the data appears to be a header file.
        /// </summary>
        public static bool IsHeader(byte[] bytes)
        {
            if (bytes.Length < 4)
            {
                return false;
            }

            using (BinaryReaderEx br = new BinaryReaderEx(false, bytes))
            using (BinaryReaderEx brd = SFUtil.GetDecompressedBinaryReader(br, out DCX.CompressionInfo compression))
            {
                return IsHeader(brd);
            }
        }

        /// <summary>
        /// Whether or not the data appears to be a header file.
        /// </summary>
        public static bool IsHeader(Stream stream)
        {
            if ((stream.Length - stream.Position) < 4)
            {
                return false;
            }

            using (BinaryReaderEx br = new BinaryReaderEx(false, stream, true))
            using (BinaryReaderEx brd = SFUtil.GetDecompressedBinaryReader(br, out DCX.CompressionInfo compression))
            {
                return IsHeader(brd);
            }
        }

        /// <summary>
        /// Whether or not the data appears to be a header file.
        /// </summary>
        private static bool IsHeader(BinaryReaderEx br) => br.Remaining >= 4 && br.GetASCII(br.Position, 4) == "BHF4";

        /// <summary>
        /// Whether or not the data appears to be a data file.
        /// </summary>
        public static bool IsData(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (fs.Length < 4)
                {
                    return false;
                }

                using (BinaryReaderEx br = new BinaryReaderEx(false, fs))
                using (BinaryReaderEx brd = SFUtil.GetDecompressedBinaryReader(br, out DCX.CompressionInfo compression))
                {
                    return IsData(brd);
                }
            }
        }

        /// <summary>
        /// Whether or not the data appears to be a data file.
        /// </summary>
        public static bool IsData(byte[] bytes)
        {
            if (bytes.Length < 4)
            {
                return false;
            }

            using (BinaryReaderEx br = new BinaryReaderEx(false, bytes))
            using (BinaryReaderEx brd = SFUtil.GetDecompressedBinaryReader(br, out DCX.CompressionInfo compression))
            {
                return IsData(brd);
            }
        }

        /// <summary>
        /// Whether or not the data appears to be a data file.
        /// </summary>
        public static bool IsData(Stream stream)
        {
            if ((stream.Length - stream.Position) < 4)
            {
                return false;
            }

            using (BinaryReaderEx br = new BinaryReaderEx(false, stream, true))
            using (BinaryReaderEx brd = SFUtil.GetDecompressedBinaryReader(br, out DCX.CompressionInfo compression))
            {
                return IsData(brd);
            }
        }

        /// <summary>
        /// Whether or not the data appears to be a data file.
        /// </summary>
        private static bool IsData(BinaryReaderEx br)
            => br.Remaining >= 4 && br.GetASCII(br.Position, 4) == "BDF4";

        #endregion
    }
}
