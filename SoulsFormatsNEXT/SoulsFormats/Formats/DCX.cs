using SoulsFormats.Compression;
using System;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;
using Org.BouncyCastle.Security;

namespace SoulsFormats
{
    /// <summary>
    /// A general-purpose single compressed file wrapper used in DeS, DS1, ACV, ACVD, DS2, DSR, DS3, BB, Sekiro, ER, and AC6.
    /// </summary>
    public static class DCX
    {
        #region Is

        internal static bool Is(BinaryReaderEx br)
        {
            if (br.Stream.Length < 4)
                return false;

            string magic = br.GetASCII(0, 4);
            return magic == "DCP\0" || magic == "DCX\0";
        }

        /// <summary>
        /// Returns true if the stream appears to be a DCX file.
        /// </summary>
        public static bool Is(Stream stream)
        {
            if (stream.Position != 0)
            {
                // Cannot ensure offset jumping for every format will work otherwise
                throw new InvalidOperationException($"Cannot safely read if stream is not at position {0}.");
            }

            using (var br = new BinaryReaderEx(true, stream, true))
            {
                // We know the "Is" call won't advance the stream for this format anyways
                // So no need to reset position
                return Is(br);
            }
        }

        /// <summary>
        /// Returns true if the bytes appear to be a DCX file.
        /// </summary>
        public static bool Is(byte[] bytes)
        {
            using (var br = new BinaryReaderEx(true, bytes))
            {
                return Is(br);
            }
        }

        /// <summary>
        /// Returns true if the file appears to be a DCX file.
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

        #region Decompress

        /// <summary>
        /// Decompress a <see cref="DCX"/> file from a stream and return the detected <see cref="DCX"/> type.
        /// </summary>
        public static byte[] Decompress(Stream stream, out CompressionInfo compression)
        {
            if (stream.Position != 0)
            {
                // Cannot ensure offset jumping for every format will work otherwise
                throw new InvalidOperationException($"Cannot safely read if stream is not at position {0}.");
            }

            using (BinaryReaderEx br = new BinaryReaderEx(true, stream, true))
            {
                return Decompress(br, out compression);
            }
        }

        /// <summary>
        /// Decompress a <see cref="DCX"/> file from a stream.
        /// </summary>
        public static byte[] Decompress(Stream stream)
        {
            return Decompress(stream, out _);
        }

        /// <summary>
        /// Decompress a <see cref="DCX"/> file from an array of bytes and return the detected <see cref="DCX"/> type.
        /// </summary>
        public static byte[] Decompress(byte[] data, out CompressionInfo compression)
        {
            using (BinaryReaderEx br = new BinaryReaderEx(true, data))
            {
                return Decompress(br, out compression);
            }
        }

        /// <summary>
        /// Decompress a <see cref="DCX"/> file from an array of bytes.
        /// </summary>
        public static byte[] Decompress(byte[] data)
        {
            return Decompress(data, out _);
        }

        /// <summary>
        /// Decompress a <see cref="DCX"/> file from the specified path and return the detected <see cref="DCX"/> type.
        /// </summary>
        public static byte[] Decompress(string path, out CompressionInfo compression)
        {
            using (FileStream stream = File.OpenRead(path))
            using (BinaryReaderEx br = new BinaryReaderEx(true, stream, true))
            {
                return Decompress(br, out compression);
            }
        }

        /// <summary>
        /// Decompress a <see cref="DCX"/> file from the specified path.
        /// </summary>
        public static byte[] Decompress(string path)
        {
            return Decompress(path, out _);
        }

        #endregion

        #region Decompress Internal

        internal static byte[] Decompress(BinaryReaderEx br, out CompressionInfo compression)
        {
            compression = new UnkCompressionInfo();
            br.BigEndian = true;

            string magic = br.ReadASCII(4);
            if (magic == "DCP\0")
            {
                string format = br.GetASCII(4, 4);
                switch (format)
                {
                    case "DFLT":
                        compression = new DcpDfltCompressionInfo();
                        break;
                    case "EDGE":
                        compression = new DcpEdgeCompressionInfo();
                        break;
                }
            }
            else if (magic == "DCX\0")
            {
                string format = br.GetASCII(0x28, 4);
                switch (format)
                {
                    case "EDGE":
                        compression = new DcxEdgeCompressionInfo();
                        break;
                    case "DFLT":
                        int unk04 = br.GetInt32(0x4);
                        int unk10 = br.GetInt32(0x10);
                        int unk14 = br.GetInt32(0x14);
                        byte unk30 = br.GetByte(0x30);
                        byte unk38 = br.GetByte(0x38);

                        compression = new DcxDfltCompressionInfo(unk04, unk10, unk14, unk30, unk38);
                        break;
                    case "KRAK":
                        byte compressionLevel = br.GetByte(0x30);
                        compression = new DcxKrakCompressionInfo(compressionLevel);
                        break;
                    case "ZSTD":
                        compression = new DcxZstdCompressionInfo();
                        break;
                }
            }
            else
            {
                byte b0 = br.GetByte(0);
                byte b1 = br.GetByte(1);
                if (b0 == 0x78 && (b1 == 0x01 || b1 == 0x5E || b1 == 0x9C || b1 == 0xDA))
                {
                    compression = new ZlibCompressionInfo();
                }
            }

            br.Position = 0;
            switch (compression.Type)
            {
                case Type.Zlib:
                    return ZlibHelper.ReadZlib(br, (int)br.Length);
                case Type.DCP_EDGE:
                    return DecompressDCPEDGE(br);
                case Type.DCP_DFLT:
                    return DecompressDCPDFLT(br);
                case Type.DCX_EDGE:
                    return DecompressDCXEDGE(br);
                case Type.DCX_DFLT:
                    return DecompressDCXDFLT(br, (DcxDfltCompressionInfo) compression);
                case Type.DCX_KRAK:
                    return DecompressDCXKRAK(br, (DcxKrakCompressionInfo) compression);
                case Type.DCX_ZSTD:
                    return DecompressDCXZSTD(br);
                default:
                    throw new FormatException($"Unknown DCX format {compression.Type}.");
            }
        }

        #endregion

        #region Decompress Algorithms

        private static byte[] DecompressDCPDFLT(BinaryReaderEx br)
        {
            br.AssertASCII("DCP\0");
            br.AssertASCII("DFLT");
            br.AssertInt32(0x20);
            br.AssertInt32(0x9000000);
            br.AssertInt32(0);
            br.AssertInt32(0);
            br.AssertInt32(0);
            br.AssertInt32(0x00010100);

            br.AssertASCII("DCS\0");
            int uncompressedSize = br.ReadInt32();
            int compressedSize = br.ReadInt32();

            byte[] decompressed = ZlibHelper.ReadZlib(br, compressedSize);

            br.AssertASCII("DCA\0");
            br.AssertInt32(8);

            return decompressed;
        }

        private static byte[] DecompressDCPEDGE(BinaryReaderEx br)
        {
            br.AssertASCII("DCP\0");
            br.AssertASCII("EDGE");
            br.AssertInt32(0x20);
            br.AssertInt32(0x9000000);
            br.AssertInt32(0x10000);
            br.AssertInt32(0x0);
            br.AssertInt32(0x0);
            br.AssertInt32(0x00100100);

            br.AssertASCII("DCS\0");
            int uncompressedSize = br.ReadInt32();
            int compressedSize = br.ReadInt32();
            br.AssertInt32(0);
            long dataStart = br.Position;
            br.Skip(compressedSize);

            br.AssertASCII("DCA\0");
            int dcaSize = br.ReadInt32();
            // ???
            br.AssertASCII("EgdT");
            br.AssertInt32(0x00010000);
            br.AssertInt32(0x20);
            br.AssertInt32(0x10);
            br.AssertInt32(0x10000);
            int egdtSize = br.ReadInt32();
            int chunkCount = br.ReadInt32();
            br.AssertInt32(0x100000);

            if (egdtSize != 0x20 + chunkCount * 0x10)
                throw new InvalidDataException("Unexpected EgdT size in EDGE DCX.");

            byte[] decompressed = new byte[uncompressedSize];
            using (MemoryStream dcmpStream = new MemoryStream(decompressed))
            {
                for (int i = 0; i < chunkCount; i++)
                {
                    br.AssertInt32(0);
                    int offset = br.ReadInt32();
                    int size = br.ReadInt32();
                    bool compressed = br.AssertInt32(0, 1) == 1;

                    byte[] chunk = br.GetBytes(dataStart + offset, size);

                    if (compressed)
                    {
                        using (MemoryStream cmpStream = new MemoryStream(chunk))
                        using (DeflateStream dfltStream = new DeflateStream(cmpStream, CompressionMode.Decompress))
                            dfltStream.CopyTo(dcmpStream);
                    }
                    else
                    {
                        dcmpStream.Write(chunk, 0, chunk.Length);
                    }
                }
            }

            return decompressed;
        }

        private static byte[] DecompressDCXEDGE(BinaryReaderEx br)
        {
            br.AssertASCII("DCX\0");
            br.AssertInt32(0x10000);
            br.AssertInt32(0x18);
            br.AssertInt32(0x24);
            br.AssertInt32(0x24);
            int unk1 = br.ReadInt32();

            br.AssertASCII("DCS\0");
            int uncompressedSize = br.ReadInt32();
            int compressedSize = br.ReadInt32();

            br.AssertASCII("DCP\0");
            br.AssertASCII("EDGE");
            br.AssertInt32(0x20);
            br.AssertInt32(0x9000000);
            br.AssertInt32(0x10000);
            br.AssertInt32(0x0);
            br.AssertInt32(0x0);
            br.AssertInt32(0x00100100);

            long dcaStart = br.Position;
            br.AssertASCII("DCA\0");
            int dcaSize = br.ReadInt32();
            // ???
            br.AssertASCII("EgdT");
            br.AssertInt32(0x00010100);
            br.AssertInt32(0x24);
            br.AssertInt32(0x10);
            br.AssertInt32(0x10000);
            // Uncompressed size of last block
            int trailingUncompressedSize = br.AssertInt32(uncompressedSize % 0x10000, 0x10000);
            int egdtSize = br.ReadInt32();
            int chunkCount = br.ReadInt32();
            br.AssertInt32(0x100000);

            if (unk1 != 0x50 + chunkCount * 0x10)
                throw new InvalidDataException("Unexpected unk1 value in EDGE DCX.");

            if (egdtSize != 0x24 + chunkCount * 0x10)
                throw new InvalidDataException("Unexpected EgdT size in EDGE DCX.");

            byte[] decompressed = new byte[uncompressedSize];
            using (MemoryStream dcmpStream = new MemoryStream(decompressed))
            {
                for (int i = 0; i < chunkCount; i++)
                {
                    br.AssertInt32(0);
                    int offset = br.ReadInt32();
                    int size = br.ReadInt32();
                    bool compressed = br.AssertInt32(0, 1) == 1;

                    byte[] chunk = br.GetBytes(dcaStart + dcaSize + offset, size);

                    if (compressed)
                    {
                        using (MemoryStream cmpStream = new MemoryStream(chunk))
                        using (DeflateStream dfltStream = new DeflateStream(cmpStream, CompressionMode.Decompress))
                            dfltStream.CopyTo(dcmpStream);
                    }
                    else
                    {
                        dcmpStream.Write(chunk, 0, chunk.Length);
                    }
                }
            }

            return decompressed;
        }

        private static byte[] DecompressDCXDFLT(BinaryReaderEx br, DcxDfltCompressionInfo compression)
        {
            br.AssertASCII("DCX\0");
            br.AssertInt32(compression.Unk04);
            br.AssertInt32(0x18);
            br.AssertInt32(0x24);
            br.AssertInt32(compression.Unk10);
            br.AssertInt32(compression.Unk14);

            br.AssertASCII("DCS\0");
            int uncompressedSize = br.ReadInt32();
            int compressedSize = br.ReadInt32();

            br.AssertASCII("DCP\0");
            br.AssertASCII("DFLT");
            br.AssertInt32(0x20);
            br.AssertByte(compression.Unk30);
            br.AssertByte(0);
            br.AssertByte(0);
            br.AssertByte(0);
            br.AssertInt32(0x0);
            br.AssertByte(compression.Unk38);
            br.AssertByte(0);
            br.AssertByte(0);
            br.AssertByte(0);
            br.AssertInt32(0x0);
            // These look suspiciously like flags
            br.AssertInt32(0x00010100);

            br.AssertASCII("DCA\0");
            int compressedHeaderLength = br.ReadInt32();

            return ZlibHelper.ReadZlib(br, Convert.ToInt32(br.Length - br.Position));
        }

        private static byte[] DecompressDCXKRAK(BinaryReaderEx br, DcxKrakCompressionInfo compression)
        {
            br.AssertASCII("DCX\0");
            br.AssertInt32(0x11000);
            br.AssertInt32(0x18);
            br.AssertInt32(0x24);
            br.AssertInt32(0x44);
            br.AssertInt32(0x4C);
            br.AssertASCII("DCS\0");
            uint uncompressedSize = br.ReadUInt32();
            uint compressedSize = br.ReadUInt32();
            br.AssertASCII("DCP\0");
            br.AssertASCII("KRAK");
            br.AssertInt32(0x20);
            br.AssertByte(compression.CompressionLevel);
            br.AssertByte(0);
            br.AssertByte(0);
            br.AssertByte(0);
            br.AssertInt32(0);
            br.AssertInt32(0);
            br.AssertInt32(0);
            br.AssertInt32(0x10100);
            br.AssertASCII("DCA\0");
            br.AssertInt32(8);

            byte[] compressed = br.ReadBytes((int)compressedSize);
            return Oodle.GetOodleCompressor().Decompress(compressed, uncompressedSize);
        }

        // Written by ClayAmore
        private static byte[] DecompressDCXZSTD(BinaryReaderEx br)
        {
            br.AssertASCII("DCX\0");
            br.AssertInt32(0x11000);
            br.AssertInt32(0x18);
            br.AssertInt32(0x24);
            br.AssertInt32(0x44);
            br.AssertInt32(0x4C);

            br.AssertASCII("DCS\0");
            br.ReadInt32(); // uncompressed size
            int compressedSize = br.ReadInt32();

            br.AssertASCII("DCP\0");
            br.AssertASCII("ZSTD");
            br.AssertInt32(0x20);
            br.ReadByte(); // compression level
            br.AssertByte(0);
            br.AssertByte(0);
            br.AssertByte(0);
            br.AssertInt32(0x0);
            br.AssertByte(0);
            br.AssertByte(0);
            br.AssertByte(0);
            br.AssertByte(0);
            br.AssertInt32(0x0);
            br.AssertInt32(0x010100);

            br.AssertASCII("DCA\0");
            br.AssertInt32(8);

            return ZstdHelper.ReadZstd(br, compressedSize);
        }

        #endregion

        #region Compress

        /// <summary>
        /// Compress a DCX file to an array of bytes using the specified DCX type.
        /// </summary>
        public static byte[] Compress(byte[] data, CompressionInfo compression)
        {
            BinaryWriterEx bw = new BinaryWriterEx(true);
            Compress(data, bw, compression);
            return bw.FinishBytes();
        }

        /// <summary>
        /// Compress a DCX file to the specified path using the specified DCX type.
        /// </summary>
        public static void Compress(byte[] data, CompressionInfo compression, string path)
        {
            using (FileStream stream = File.Create(path))
            {
                BinaryWriterEx bw = new BinaryWriterEx(true, stream);
                Compress(data, bw, compression);
                bw.Finish();
            }
        }

        #endregion

        #region Compress Internal

        internal static void Compress(byte[] data, BinaryWriterEx bw, CompressionInfo compression)
        {
            bw.BigEndian = true;
            switch (compression.Type)
            {
                case Type.Zlib:
                    ZlibHelper.WriteZlib(bw, 0xDA, data);
                    return;
                case Type.DCP_EDGE:
                    return;
                case Type.DCP_DFLT:
                    CompressDCPDFLT(data, bw);
                    return;
                case Type.DCX_EDGE:
                    CompressDCXEDGE(data, bw);
                    return;
                case Type.DCX_DFLT:
                    CompressDCXDFLT(data, bw, (DcxDfltCompressionInfo)compression);
                    return;
                case Type.DCX_KRAK:
                    CompressDCXKRAK(data, bw, (DcxKrakCompressionInfo)compression);
                    return;
                case Type.DCX_ZSTD:
                    CompressDCXZSTD(data, bw);
                    return;
                case Type.Unknown:
                    throw new ArgumentException("You cannot compress a DCX with an unknown type.");
                default:
                    throw new NotImplementedException("Compression for the given type is not implemented.");
            }
        }

        #endregion

        #region Compress Algorithms

        private static void CompressDCPDFLT(byte[] data, BinaryWriterEx bw)
        {
            bw.WriteASCII("DCP\0");
            bw.WriteASCII("DFLT");
            bw.WriteInt32(0x20);
            bw.WriteInt32(0x9000000);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0x00010100);

            bw.WriteASCII("DCS\0");
            bw.WriteInt32(data.Length);
            bw.ReserveInt32("CompressedSize");

            int compressedSize = ZlibHelper.WriteZlib(bw, 0xDA, data);
            bw.FillInt32("CompressedSize", compressedSize);

            bw.WriteASCII("DCA\0");
            bw.WriteInt32(8);
        }

        private static void CompressDCXEDGE(byte[] data, BinaryWriterEx bw)
        {
            int chunkCount = data.Length / 0x10000;
            if (data.Length % 0x10000 > 0)
                chunkCount++;

            bw.WriteASCII("DCX\0");
            bw.WriteInt32(0x10000);
            bw.WriteInt32(0x18);
            bw.WriteInt32(0x24);
            bw.WriteInt32(0x24);
            bw.WriteInt32(0x50 + chunkCount * 0x10);

            bw.WriteASCII("DCS\0");
            bw.WriteInt32(data.Length);
            bw.ReserveInt32("CompressedSize");

            bw.WriteASCII("DCP\0");
            bw.WriteASCII("EDGE");
            bw.WriteInt32(0x20);
            bw.WriteInt32(0x9000000);
            bw.WriteInt32(0x10000);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0x00100100);

            long dcaStart = bw.Position;
            bw.WriteASCII("DCA\0");
            bw.ReserveInt32("DCASize");
            long egdtStart = bw.Position;
            bw.WriteASCII("EgdT");
            bw.WriteInt32(0x00010100);
            bw.WriteInt32(0x24);
            bw.WriteInt32(0x10);
            bw.WriteInt32(0x10000);
            bw.WriteInt32(data.Length % 0x10000);
            bw.ReserveInt32("EGDTSize");
            bw.WriteInt32(chunkCount);
            bw.WriteInt32(0x100000);

            for (int i = 0; i < chunkCount; i++)
            {
                bw.WriteInt32(0);
                bw.ReserveInt32($"ChunkOffset{i}");
                bw.ReserveInt32($"ChunkSize{i}");
                bw.ReserveInt32($"ChunkCompressed{i}");
            }

            bw.FillInt32("DCASize", (int)(bw.Position - dcaStart));
            bw.FillInt32("EGDTSize", (int)(bw.Position - egdtStart));
            long dataStart = bw.Position;

            int compressedSize = 0;
            for (int i = 0; i < chunkCount; i++)
            {
                int chunkSize = 0x10000;
                if (i == chunkCount - 1)
                    chunkSize = data.Length % 0x10000;

                byte[] chunk;
                int chunkOffset = i * 0x10000;
                using (MemoryStream cmpStream = new MemoryStream())
                using (MemoryStream dcmpStream = new MemoryStream(data, chunkOffset, chunkSize))
                {
                    DeflateStream dfltStream = new DeflateStream(cmpStream, CompressionMode.Compress);
                    dcmpStream.CopyTo(dfltStream);
                    dfltStream.Close();
                    chunk = cmpStream.ToArray();
                }

                if (chunk.Length < chunkSize)
                    bw.FillInt32($"ChunkCompressed{i}", 1);
                else
                {
                    // If the compressed chunk is not any smaller than the original, just go with the uncompressed data
                    bw.FillInt32($"ChunkCompressed{i}", 0);
                    chunk = new byte[chunkSize];
                    Array.Copy(data, chunkOffset, chunk, 0, chunkSize);
                }

                compressedSize += chunk.Length;
                bw.FillInt32($"ChunkOffset{i}", (int)(bw.Position - dataStart));
                bw.FillInt32($"ChunkSize{i}", chunk.Length);
                bw.WriteBytes(chunk);
                bw.Pad(0x10);
            }

            bw.FillInt32("CompressedSize", compressedSize);
        }

        private static void CompressDCXDFLT(byte[] data, BinaryWriterEx bw, DcxDfltCompressionInfo compression)
        {
            bw.WriteASCII("DCX\0");
            
            bw.WriteInt32(compression.Unk04);

            bw.WriteInt32(0x18);
            bw.WriteInt32(0x24);

            bw.WriteInt32(compression.Unk10);
            bw.WriteInt32(compression.Unk14);

            bw.WriteASCII("DCS\0");
            bw.WriteInt32(data.Length);
            bw.ReserveInt32("CompressedSize");
            bw.WriteASCII("DCP\0");
            bw.WriteASCII("DFLT");
            bw.WriteInt32(0x20);

            bw.WriteByte(compression.Unk30);
            bw.WriteByte(0);
            bw.WriteByte(0);
            bw.WriteByte(0);

            bw.WriteInt32(0);
            
            bw.WriteByte(compression.Unk38);
            bw.WriteByte(0);
            bw.WriteByte(0);
            bw.WriteByte(0);

            bw.WriteInt32(0);
            bw.WriteInt32(0x00010100);
            bw.WriteASCII("DCA\0");
            bw.WriteInt32(8);

            long compressedStart = bw.Position;
            ZlibHelper.WriteZlib(bw, 0xDA, data);
            bw.FillInt32("CompressedSize", (int)(bw.Position - compressedStart));
        }

        private static void CompressDCXKRAK(byte[] data, BinaryWriterEx bw, DcxKrakCompressionInfo compression)
        {
            
            byte[] compressed = Oodle.GetOodleCompressor().Compress(data, Oodle.OodleLZ_Compressor.OodleLZ_Compressor_Kraken,
                (Oodle.OodleLZ_CompressionLevel) compression.CompressionLevel);

            bw.WriteASCII("DCX\0");
            bw.WriteInt32(0x11000);
            bw.WriteInt32(0x18);
            bw.WriteInt32(0x24);
            bw.WriteInt32(0x44);
            bw.WriteInt32(0x4C);
            bw.WriteASCII("DCS\0");
            bw.WriteUInt32((uint)data.Length);
            bw.WriteUInt32((uint)compressed.Length);
            bw.WriteASCII("DCP\0");
            bw.WriteASCII("KRAK");
            bw.WriteInt32(0x20);
            bw.WriteByte(compression.CompressionLevel);
            bw.WriteByte(0);
            bw.WriteByte(0);
            bw.WriteByte(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0x10100);
            bw.WriteASCII("DCA\0");
            bw.WriteInt32(8);
            bw.WriteBytes(compressed);
            bw.Pad(0x10);
        }

        private static void CompressDCXZSTD(byte[] data, BinaryWriterEx bw, int compressionLevel = 15)
        {
            byte[] compressed = ZstdHelper.WriteZstd(data, compressionLevel);

            bw.WriteASCII("DCX\0");
            bw.WriteInt32(0x11000);
            bw.WriteInt32(0x18);
            bw.WriteInt32(0x24);
            bw.WriteInt32(0x44);
            bw.WriteInt32(0x4C);
            bw.WriteASCII("DCS\0");
            bw.WriteUInt32((uint)data.Length);
            bw.WriteUInt32((uint)compressed.Length);
            bw.WriteASCII("DCP\0");
            bw.WriteASCII("ZSTD");
            bw.WriteInt32(0x20);
            bw.WriteByte((byte)compressionLevel);
            bw.WriteByte(0);
            bw.WriteByte(0);
            bw.WriteByte(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0x10100);
            bw.WriteASCII("DCA\0");
            bw.WriteInt32(8);
            bw.WriteBytes(compressed);
            bw.Pad(0x10);
        }

        #endregion

        /// <summary>
        /// Specific compression format used for a certain file.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// DCX type could not be detected.
            /// </summary>
            Unknown,

            /// <summary>
            /// The file is not compressed.
            /// </summary>
            None,

            /// <summary>
            /// Plain zlib-wrapped data; not really DCX, but it's convenient to support it here.
            /// </summary>
            Zlib,

            /// <summary>
            /// DCP header, chunked deflate compression. Used in ACE:R TPFs.
            /// </summary>
            DCP_EDGE,

            /// <summary>
            /// DCP header, deflate compression. Used in DeS test maps.
            /// </summary>
            DCP_DFLT,

            /// <summary>
            /// DCX header, chunked deflate compression. Primarily used in DeS.
            /// </summary>
            DCX_EDGE,

            /// <summary>
            /// DCX header, deflate compression. Primarily used in DS1, DS2, BB, DS3, Sekiro and ER.
            /// </summary>
            DCX_DFLT,

            /// <summary>
            /// DCX header, Oodle compression. Used in Sekiro.
            /// </summary>
            DCX_KRAK,

            /// <summary>
            /// DCX header, ZSTD compression. Used in Elden Ring: Shadow of the Erdtree.
            /// </summary>
            DCX_ZSTD,
        }

        /// <summary>
        /// Standard compression types used by various games; may be cast directly to DCX.Type.
        /// </summary>
        public enum DefaultType
        {
            /// <summary>
            /// Most common compression format for Demon's Souls.
            /// </summary>
            DemonsSouls = Type.DCX_EDGE,

            /// <summary>
            /// Most common compression format for Dark Souls 1. DCX_DFLT_10000_24_9
            /// </summary>
            DarkSouls1 = Type.DCX_DFLT,

            /// <summary>
            /// Most common compression format for Dark Souls 2. DCX_DFLT_10000_24_9
            /// </summary>
            DarkSouls2 = Type.DCX_DFLT,

            /// <summary>
            /// Most common compression format for Bloodborne. DCX_DFLT_10000_44_9
            /// </summary>
            Bloodborne = Type.DCX_DFLT,

            /// <summary>
            /// Most common compression format for Dark Souls 3. DCX_DFLT_10000_44_9
            /// </summary>
            DarkSouls3 = Type.DCX_DFLT,

            /// <summary>
            /// Most common compression format for Sekiro.
            /// </summary>
            Sekiro = Type.DCX_KRAK,

            /// <summary>
            /// Most common compression format for Elden Ring.
            /// </summary>
            EldenRing = Type.DCX_KRAK,

            /// <summary>
            /// Most common compression format for Armored Core VI. DCX_KRAK_MAX
            /// </summary>
            AC6 = Type.DCX_KRAK,
        }

        public interface CompressionInfo
        {
            [XmlText]
            Type Type { get; }
        }

        [Serializable]
        public struct UnkCompressionInfo : CompressionInfo
        {
            [XmlText]
            public Type Type => Type.Unknown;
        }

        [Serializable]
        public struct NoCompressionInfo : CompressionInfo
        {
            [XmlText]
            public Type Type => Type.None;
        }

        [Serializable]
        public struct DcpDfltCompressionInfo : CompressionInfo
        {
            [XmlText]
            public Type Type => Type.DCP_DFLT;
        }
        
        [Serializable]
        public struct DcpEdgeCompressionInfo : CompressionInfo
        {
            [XmlText]
            public Type Type => Type.DCP_EDGE;
        }
        
        [Serializable]
        public struct ZlibCompressionInfo : CompressionInfo
        {
            [XmlText]
            public Type Type => Type.Zlib;
        }
        
        [Serializable]
        public struct DcxEdgeCompressionInfo : CompressionInfo
        {
            [XmlText]
            public Type Type => Type.DCX_EDGE;
        }

        public enum DfltCompressionPreset
        {
            DCX_DFLT_10000_24_9,
            DCX_DFLT_10000_44_9,
            DCX_DFLT_11000_44_8,
            DCX_DFLT_11000_44_9,
            DCX_DFLT_11000_44_9_15
        }
        [Serializable]
        public struct DcxDfltCompressionInfo : CompressionInfo
        {
            [XmlText]
            public Type Type => Type.DCX_DFLT;
            [XmlAttribute]
            public int Unk04 { get; }
            [XmlAttribute]
            public int Unk10 { get; }
            [XmlAttribute]
            public int Unk14 { get; }
            [XmlAttribute]
            public byte Unk30 { get; }
            [XmlAttribute]
            public byte Unk38 { get; }

            public DcxDfltCompressionInfo(int unk04, int unk10, int unk14, byte unk30, byte unk38)
            {
                Unk04 = unk04;
                Unk10 = unk10;
                Unk14 = unk14;
                Unk30 = unk30;
                Unk38 = unk38;
            }

            public DcxDfltCompressionInfo(DfltCompressionPreset preset)
            {
                switch (preset)
                {
                    case DfltCompressionPreset.DCX_DFLT_10000_24_9:
                        Unk04 = 0x10000;
                        Unk10 = 0x24;
                        Unk14 = 0x2C;
                        Unk30 = 9;
                        Unk38 = 0;
                        break;
                    case DfltCompressionPreset.DCX_DFLT_10000_44_9:
                        Unk04 = 0x10000;
                        Unk10 = 0x44;
                        Unk14 = 0x4C;
                        Unk30 = 9;
                        Unk38 = 0;
                        break;
                    case DfltCompressionPreset.DCX_DFLT_11000_44_8:
                        Unk04 = 0x11000;
                        Unk10 = 0x44;
                        Unk14 = 0x4C;
                        Unk30 = 8;
                        Unk38 = 0;
                        break;
                    case DfltCompressionPreset.DCX_DFLT_11000_44_9:
                        Unk04 = 0x11000;
                        Unk10 = 0x44;
                        Unk14 = 0x4C;
                        Unk30 = 9;
                        Unk38 = 0;
                        break;
                    case DfltCompressionPreset.DCX_DFLT_11000_44_9_15:
                        Unk04 = 0x11000;
                        Unk10 = 0x44;
                        Unk14 = 0x4C;
                        Unk30 = 9;
                        Unk38 = 15;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(preset), preset, null);
                }
            }
        }

        public enum KrakCompressionPreset
        {
            EldenRing,
            ArmoredCore6
        }
        [Serializable]
        public struct DcxKrakCompressionInfo : CompressionInfo
        {
            [XmlText]
            public Type Type => Type.DCX_KRAK;
            
            [XmlAttribute]
            public byte CompressionLevel { get; }

            public DcxKrakCompressionInfo (byte compressionLevel)
            {
                CompressionLevel = compressionLevel;
            }

            public DcxKrakCompressionInfo(KrakCompressionPreset preset)
            {
                switch (preset)
                {
                    case KrakCompressionPreset.EldenRing:
                        CompressionLevel = 6;
                        break;
                    case KrakCompressionPreset.ArmoredCore6:
                        CompressionLevel = 9;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(preset), preset, null);
                }
            }
        }
        
        [Serializable]
        public struct DcxZstdCompressionInfo : CompressionInfo
        {
            [XmlText]
            public Type Type => Type.DCX_ZSTD;
        }
    }
}
