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
    public class BXF3 : IBinder, IBXF3
    {
        /// <summary>
        /// The files contained within this <see cref="BXF3"/>.
        /// </summary>
        public List<BinderFile> Files { get; set; }

        /// <summary>
        ///A timestamp or version number, 8 characters maximum.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Indicates the format of this <see cref="BXF3"/>.
        /// </summary>
        public Binder.Format Format { get; set; }

        /// <summary>
        /// Whether to use big-endian byte ordering.
        /// </summary>
        public bool BigEndian { get; set; }

        /// <summary>
        /// Controls ordering of flag bits.
        /// </summary>
        public bool BitBigEndian { get; set; }

        /// <summary>
        /// Creates an empty <see cref="BXF3"/> formatted for DS1.
        /// </summary>
        public BXF3()
        {
            Files = new List<BinderFile>();
            Version = Binder.DateToBinderTimestamp(DateTime.Now);
            Format = Binder.Format.IDs | Binder.Format.Names1 | Binder.Format.Names2 | Binder.Format.Compression;
        }

        private BXF3(BinaryReaderEx bhdReader, BinaryReaderEx bdtReader)
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
        /// Reads a <see cref="BXF3"/> using paths.
        /// </summary>
        /// <param name="bhdPath">The header path.</param>
        /// <param name="bdtBytes">The data path.</param>
        /// <returns>A new <see cref="BXF3"/>.</returns>
        public static BXF3 Read(string bhdPath, string bdtBytes)
        {
            using (BinaryReaderEx bhdReader = new BinaryReaderEx(false, bhdPath))
            using (BinaryReaderEx bdtReader = new BinaryReaderEx(false, bdtBytes))
            using (BinaryReaderEx bhdReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bhdReader, out DCX.CompressionInfo _))
            using (BinaryReaderEx bdtReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bdtReader, out DCX.CompressionInfo _))
            {
                return new BXF3(bhdReaderDecompressed, bdtReaderDecompressed);
            }
        }

        /// <summary>
        /// Reads a <see cref="BXF3"/> using a path and bytes.
        /// </summary>
        /// <param name="bhdPath">The header path.</param>
        /// <param name="bdtBytes">The data bytes.</param>
        /// <returns>A new <see cref="BXF3"/>.</returns>
        public static BXF3 Read(string bhdPath, byte[] bdtBytes)
        {
            using (BinaryReaderEx bhdReader = new BinaryReaderEx(false, bhdPath))
            using (BinaryReaderEx bdtReader = new BinaryReaderEx(false, bdtBytes))
            using (BinaryReaderEx bhdReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bhdReader, out DCX.CompressionInfo _))
            using (BinaryReaderEx bdtReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bdtReader, out DCX.CompressionInfo _))
            {
                return new BXF3(bhdReaderDecompressed, bdtReaderDecompressed);
            }
        }

        /// <summary>
        /// Reads a <see cref="BXF3"/> using a path and a stream.
        /// </summary>
        /// <param name="bhdPath">The header path.</param>
        /// <param name="bdtStream">The data stream.</param>
        /// <returns>A new <see cref="BXF3"/>.</returns>
        public static BXF3 Read(string bhdPath, Stream bdtStream)
        {
            using (BinaryReaderEx bhdReader = new BinaryReaderEx(false, bhdPath))
            using (BinaryReaderEx bdtReader = new BinaryReaderEx(false, bdtStream, true))
            using (BinaryReaderEx bhdReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bhdReader, out DCX.CompressionInfo _))
            using (BinaryReaderEx bdtReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bdtReader, out DCX.CompressionInfo _))
            {
                return new BXF3(bhdReaderDecompressed, bdtReaderDecompressed);
            }
        }

        /// <summary>
        /// Reads a <see cref="BXF3"/> using bytes.
        /// </summary>
        /// <param name="bhdBytes">The header bytes.</param>
        /// <param name="bdtBytes">The data bytes.</param>
        /// <returns>A new <see cref="BXF3"/>.</returns>
        public static BXF3 Read(byte[] bhdBytes, byte[] bdtBytes)
        {
            using (BinaryReaderEx bhdReader = new BinaryReaderEx(false, bhdBytes))
            using (BinaryReaderEx bdtReader = new BinaryReaderEx(false, bdtBytes))
            using (BinaryReaderEx bhdReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bhdReader, out DCX.CompressionInfo _))
            using (BinaryReaderEx bdtReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bdtReader, out DCX.CompressionInfo _))
            {
                return new BXF3(bhdReaderDecompressed, bdtReaderDecompressed);
            }
        }

        /// <summary>
        /// Reads a <see cref="BXF3"/> using bytes and a path.
        /// </summary>
        /// <param name="bhdBytes">The header bytes.</param>
        /// <param name="bdtPath">The data path.</param>
        /// <returns>A new <see cref="BXF3"/>.</returns>
        public static BXF3 Read(byte[] bhdBytes, string bdtPath)
        {
            using (BinaryReaderEx bhdReader = new BinaryReaderEx(false, bhdBytes))
            using (BinaryReaderEx bdtReader = new BinaryReaderEx(false, bdtPath))
            using (BinaryReaderEx bhdReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bhdReader, out DCX.CompressionInfo _))
            using (BinaryReaderEx bdtReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bdtReader, out DCX.CompressionInfo _))
            {
                return new BXF3(bhdReaderDecompressed, bdtReaderDecompressed);
            }
        }

        /// <summary>
        /// Reads a <see cref="BXF3"/> using bytes and a stream.
        /// </summary>
        /// <param name="bhdBytes">The header bytes.</param>
        /// <param name="bdtStream">The data stream.</param>
        /// <returns>A new <see cref="BXF3"/>.</returns>
        public static BXF3 Read(byte[] bhdBytes, Stream bdtStream)
        {
            using (BinaryReaderEx bhdReader = new BinaryReaderEx(false, bhdBytes))
            using (BinaryReaderEx bdtReader = new BinaryReaderEx(false, bdtStream, true))
            using (BinaryReaderEx bhdReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bhdReader, out DCX.CompressionInfo _))
            using (BinaryReaderEx bdtReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bdtReader, out DCX.CompressionInfo _))
            {
                return new BXF3(bhdReaderDecompressed, bdtReaderDecompressed);
            }
        }

        /// <summary>
        /// Reads a <see cref="BXF3"/> using streams.
        /// </summary>
        /// <param name="bhdStream">The header stream.</param>
        /// <param name="bdtStream">The data stream.</param>
        /// <returns>A new <see cref="BXF3"/>.</returns>
        public static BXF3 Read(Stream bhdStream, Stream bdtStream)
        {
            using (BinaryReaderEx bhdReader = new BinaryReaderEx(false, bhdStream, true))
            using (BinaryReaderEx bdtReader = new BinaryReaderEx(false, bdtStream, true))
            using (BinaryReaderEx bhdReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bhdReader, out DCX.CompressionInfo _))
            using (BinaryReaderEx bdtReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bdtReader, out DCX.CompressionInfo _))
            {
                return new BXF3(bhdReaderDecompressed, bdtReaderDecompressed);
            }
        }

        /// <summary>
        /// Reads a <see cref="BXF3"/> using a stream and a path.
        /// </summary>
        /// <param name="bhdStream">The header stream.</param>
        /// <param name="bdtPath">The data path.</param>
        /// <returns>A new <see cref="BXF3"/>.</returns>
        public static BXF3 Read(Stream bhdStream, string bdtPath)
        {
            using (BinaryReaderEx bhdReader = new BinaryReaderEx(false, bhdStream, true))
            using (BinaryReaderEx bdtReader = new BinaryReaderEx(false, bdtPath))
            using (BinaryReaderEx bhdReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bhdReader, out DCX.CompressionInfo _))
            using (BinaryReaderEx bdtReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bdtReader, out DCX.CompressionInfo _))
            {
                return new BXF3(bhdReaderDecompressed, bdtReaderDecompressed);
            }
        }

        /// <summary>
        /// Reads a <see cref="BXF3"/> using a stream and bytes.
        /// </summary>
        /// <param name="bhdStream">The header stream.</param>
        /// <param name="bdtBytes">The data bytes.</param>
        /// <returns>A new <see cref="BXF3"/>.</returns>
        public static BXF3 Read(Stream bhdStream, byte[] bdtBytes)
        {
            using (BinaryReaderEx bhdReader = new BinaryReaderEx(false, bhdStream, true))
            using (BinaryReaderEx bdtReader = new BinaryReaderEx(false, bdtBytes))
            using (BinaryReaderEx bhdReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bhdReader, out DCX.CompressionInfo _))
            using (BinaryReaderEx bdtReaderDecompressed = SFUtil.GetDecompressedBinaryReader(bdtReader, out DCX.CompressionInfo _))
            {
                return new BXF3(bhdReaderDecompressed, bdtReaderDecompressed);
            }
        }

        internal static void ReadBDFHeader(BinaryReaderEx br)
        {
            br.AssertASCII("BDF3");
            br.ReadFixStr(8); // Version
            br.AssertInt32(0);
        }

        internal static List<BinderFileHeader> ReadBHFHeader(IBXF3 bxf, BinaryReaderEx br)
        {
            br.AssertASCII("BHF3");
            bxf.Version = br.ReadFixStr(8);

            bxf.BitBigEndian = br.GetBoolean(0xE);

            bxf.Format = Binder.ReadFormat(br, bxf.BitBigEndian);
            bxf.BigEndian = br.ReadBoolean();
            br.AssertBoolean(bxf.BitBigEndian);
            br.AssertByte(0);

            br.BigEndian = bxf.BigEndian || Binder.ForceBigEndian(bxf.Format);

            int fileCount = br.ReadInt32();
            br.AssertInt32(0);
            br.AssertInt32(0);
            br.AssertInt32(0);

            var fileHeaders = new List<BinderFileHeader>(fileCount);
            for (int i = 0; i < fileCount; i++)
                fileHeaders.Add(BinderFileHeader.ReadBinder3FileHeader(br, bxf.Format, bxf.BitBigEndian));

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
                fileHeaders[i].WriteBinder3FileData(bhdWriter, bdtWriter, Format, i, Files[i].Bytes);
        }

        internal static void WriteBDFHeader(IBXF3 bxf, BinaryWriterEx bw)
        {
            bw.WriteASCII("BDF3");
            bw.WriteFixStr(bxf.Version, 8);
            bw.WriteInt32(0);
        }

        internal static void WriteBHFHeader(IBXF3 bxf, BinaryWriterEx bw, List<BinderFileHeader> fileHeaders)
        {
            bw.BigEndian = bxf.BigEndian || Binder.ForceBigEndian(bxf.Format);

            bw.WriteASCII("BHF3");
            bw.WriteFixStr(bxf.Version, 8);

            Binder.WriteFormat(bw, bxf.BitBigEndian, bxf.Format);
            bw.WriteByte(0);
            bw.WriteByte(0);
            bw.WriteByte(0);

            bw.WriteInt32(fileHeaders.Count);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0);

            for (int i = 0; i < fileHeaders.Count; i++)
                fileHeaders[i].WriteBinder3FileHeader(bw, bxf.Format, bxf.BitBigEndian, i);

            for (int i = 0; i < fileHeaders.Count; i++)
                fileHeaders[i].WriteFileName(bw, bxf.Format, false, i);
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
        private static bool IsHeader(BinaryReaderEx br)
            => br.Remaining >= 4 && br.GetASCII(br.Position, 4) == "BHF3";

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
            => br.Remaining >= 4 && br.GetASCII(br.Position, 4) == "BDF3";

        #endregion
    }
}
