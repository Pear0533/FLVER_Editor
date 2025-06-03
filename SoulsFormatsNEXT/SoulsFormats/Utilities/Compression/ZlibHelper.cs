using System;
using System.IO.Compression;
using System.IO;
namespace SoulsFormats.Compression
{
    /// <summary>
    /// Helper methods for zlib.
    /// </summary>
    public static class ZlibHelper
    {
        /// <summary>
        /// Compresses data and writes it to a BinaryWriterEx with Zlib wrapper.
        /// </summary>
        public static int WriteZlib(BinaryWriterEx bw, byte formatByte, byte[] input)
        {
            long start = bw.Position;
            bw.WriteByte(0x78);
            bw.WriteByte(formatByte);

            using (var deflateStream = new DeflateStream(bw.Stream, CompressionMode.Compress, true))
            {
                deflateStream.Write(input, 0, input.Length);
            }

            bw.WriteUInt32(Adler32(input));
            return (int)(bw.Position - start);
        }

        /// <summary>
        /// Reads a Zlib block from a BinaryReaderEx and returns the uncompressed data.
        /// </summary>
        public static byte[] ReadZlib(BinaryReaderEx br, int compressedSize)
        {
            br.AssertByte(0x78);
            br.AssertByte(0x01, 0x5E, 0x9C, 0xDA);
            return DecompressZlibBytes(br.ReadBytes(compressedSize - 2));
        }

        /// <summary>
        /// Decompresses zlib starting at the current position in a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/>.</param>
        /// <param name="compressedSize">The size of the compressed data including the 2 byte zlib header.</param>
        /// <returns>Decompressed zlib data.</returns>
        /// <exception cref="EndOfStreamException">Cannot read beyond the end of the stream.</exception>
        /// <exception cref="InvalidDataException">A valid zlib header could not be detected.</exception>
        /// <exception cref="Exception">Did not read the expected number of compressed bytes from the <see cref="Stream"/>.</exception>
        public static byte[] DecompressZlib(Stream stream, int compressedSize)
        {
            var cmf = stream.ReadByte();
            var flg = stream.ReadByte();

            if (cmf == -1 || flg == -1)
            {
                throw new EndOfStreamException("Cannot read beyond the end of the stream.");
            }

            if (cmf != 0x78)
            {
                throw new InvalidDataException("Zlib header could not be detected.");
            }

            if (flg != 0x01 && flg != 0x5E && flg != 0x9C && flg != 0xDA)
            {
                throw new InvalidDataException("Valid zlib compression level could not be detected.");
            }

            byte[] bytes = new byte[compressedSize - 2];
            if (stream.Read(bytes, 0, bytes.Length) < bytes.Length)
            {
                throw new Exception("Could not read the expected number of bytes.");
            }
            return DecompressZlibBytes(bytes);
        }

        /// <summary>
        /// Decompresses zlib bytes coming after a zlib header.
        /// </summary>
        /// <param name="compressedBytes">Compressed bytes not including the zlib header.</param>
        /// <returns>Decompressed data.</returns>
        public static byte[] DecompressZlibBytes(byte[] compressedBytes)
        {
            using (var decompressedStream = new MemoryStream())
            using (var compressedStream = new MemoryStream(compressedBytes))
            using (var deflateStream = new DeflateStream(compressedStream, CompressionMode.Decompress, true))
            {
                deflateStream.CopyTo(decompressedStream);
                return decompressedStream.ToArray();
            }
        }

        /// <summary>
        /// Computes an Adler32 checksum used by Zlib.
        /// </summary>
        public static uint Adler32(byte[] data)
        {
            uint adlerA = 1;
            uint adlerB = 0;

            foreach (byte b in data)
            {
                adlerA = (adlerA + b) % 65521;
                adlerB = (adlerB + adlerA) % 65521;
            }

            return (adlerB << 16) | adlerA;
        }
    }
}
