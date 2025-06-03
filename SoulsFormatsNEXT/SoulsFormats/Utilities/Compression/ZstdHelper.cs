using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZstdNet;

namespace SoulsFormats.Compression
{
    /// <summary>
    /// Helper methods for zstd.
    /// </summary>
    public static class ZstdHelper
    {
        // Written by ClayAmore
        public static byte[] ReadZstd(BinaryReaderEx br, int compressedSize)
        {
            byte[] compressed = br.ReadBytes(compressedSize);

            using (var decompressedStream = new MemoryStream())
            {
                using (var compressedStream = new MemoryStream(compressed))
                using (var deflateStream = new DecompressionStream(compressedStream))
                {
                    deflateStream.CopyTo(decompressedStream);
                }
                return decompressedStream.ToArray();
            }
        }

        public static byte[] WriteZstd(byte[] data, int compressionLevel)
        {
            var options = new CompressionOptions(null, new Dictionary<ZSTD_cParameter, int> { { ZSTD_cParameter.ZSTD_c_contentSizeFlag, 0 }, { ZSTD_cParameter.ZSTD_c_windowLog, 16 } }, compressionLevel);
            using (var compressor = new Compressor(options))
            {
                return compressor.Wrap(data).ToArray();
            }
        }
    }
}
