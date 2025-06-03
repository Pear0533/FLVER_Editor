using System;
using System.IO;

namespace SoulsFormats
{
    /// <summary>
    /// An on-demand reader for BND3 containers.
    /// </summary>
    public class BND3Reader : BinderReader, IBND3
    {
        /// <summary>
        /// Unknown; always 0 except in DeS where it's occasionally 0x80000000 (probably a byte).
        /// </summary>
        public int Unk18 { get; set; }

        /// <summary>
        /// Whether or not to write the file headers end value or 0.<br/>
        /// Some Binders have this as 0 and require it to be as such for some reason.
        /// </summary>
        public bool WriteFileHeadersEnd { get; set; }

        /// <summary>
        /// Type of compression used, if any.
        /// </summary>
        public DCX.CompressionInfo Compression { get; set; }

        /// <summary>
        /// Reads a BND3 from the given path, decompressing if necessary.
        /// </summary>
        public BND3Reader(string path)
        {
            FileStream fs = File.OpenRead(path);
            var br = new BinaryReaderEx(false, fs);
            Read(br);
        }

        /// <summary>
        /// Reads a BND3 from the given bytes, decompressing if necessary.
        /// </summary>
        public BND3Reader(byte[] bytes)
        {
            var br = new BinaryReaderEx(false, bytes);
            Read(br);
        }

        /// <summary>
        /// Reads a BND3 from the given <see cref="Stream"/>, decompressing if necessary.
        /// </summary>
        public BND3Reader(Stream stream)
        {
            if (stream.Position != 0)
            {
                // Cannot ensure offset jumping for every format will work otherwise
                throw new InvalidOperationException($"Cannot safely read if stream is not at position {0}.");
            }

            var br = new BinaryReaderEx(false, stream, true);
            Read(br);
        }

        private void Read(BinaryReaderEx br)
        {
            br = SFUtil.GetDecompressedBinaryReader(br, out DCX.CompressionInfo compression);
            Compression = compression;
            Files = BND3.ReadHeader(this, br);
            DataBR = br;
        }
    }
}
