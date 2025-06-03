using System;
using System.IO;

namespace SoulsFormats
{
    /// <summary>
    /// An on-demand reader for BXF4 containers.
    /// </summary>
    public class BXF4Reader : BinderReader, IBXF4
    {
        /// <summary>
        /// Unknown.
        /// </summary>
        public bool Unk04 { get; set; }

        /// <summary>
        /// Unknown.
        /// </summary>
        public bool Unk05 { get; set; }

        /// <summary>
        /// Whether to write strings in UTF-16.
        /// </summary>
        public bool Unicode { get; set; }

        /// <summary>
        /// Indicates the presence of a filename hash table.
        /// </summary>
        public byte Extended { get; set; }

        /// <summary>
        /// Reads a BXF4 from the given BHD and BDT paths.
        /// </summary>
        public BXF4Reader(string bhdPath, string bdtPath)
        {
            using (FileStream fsHeader = File.OpenRead(bhdPath))
            {
                var brHeader = new BinaryReaderEx(false, fsHeader);
                var brData = new BinaryReaderEx(false, bdtPath);
                Read(brHeader, brData);
            }
        }

        /// <summary>
        /// Reads a BXF4 from the given BHD path and BDT bytes.
        /// </summary>
        public BXF4Reader(string bhdPath, byte[] bdtBytes)
        {
            using (FileStream fsHeader = File.OpenRead(bhdPath))
            {
                var brHeader = new BinaryReaderEx(false, fsHeader);
                var brData = new BinaryReaderEx(false, bdtBytes);
                Read(brHeader, brData);
            }
        }

        /// <summary>
        /// Reads a BXF4 from the given BHD path and BDT stream.
        /// </summary>
        public BXF4Reader(string bhdPath, Stream bdtStream)
        {
            if (bdtStream.Position != 0)
            {
                // Cannot ensure offset jumping for every format will work otherwise
                throw new InvalidOperationException($"Cannot safely read if stream is not at position {0}.");
            }

            using (FileStream fsHeader = File.OpenRead(bhdPath))
            {
                var brHeader = new BinaryReaderEx(false, fsHeader);
                var brData = new BinaryReaderEx(false, bdtStream, true);
                Read(brHeader, brData);
            }
        }

        /// <summary>
        /// Reads a BXF4 from the given BHD and BDT bytes.
        /// </summary>
        public BXF4Reader(byte[] bhdBytes, byte[] bdtBytes)
        {
            using (var msHeader = new MemoryStream(bhdBytes))
            {
                var brHeader = new BinaryReaderEx(false, msHeader);
                var brData = new BinaryReaderEx(false, bdtBytes);
                Read(brHeader, brData);
            }
        }

        /// <summary>
        /// Reads a BXF4 from the given BHD bytes and BDT stream.
        /// </summary>
        public BXF4Reader(byte[] bhdBytes, string bdtPath)
        {
            using (var msHeader = new MemoryStream(bhdBytes))
            {
                var brHeader = new BinaryReaderEx(false, msHeader);
                var brData = new BinaryReaderEx(false, bdtPath);
                Read(brHeader, brData);
            }
        }

        /// <summary>
        /// Reads a BXF4 from the given BHD bytes and BDT path.
        /// </summary>
        public BXF4Reader(byte[] bhdBytes, Stream bdtStream)
        {
            if (bdtStream.Position != 0)
            {
                // Cannot ensure offset jumping for every format will work otherwise
                throw new InvalidOperationException($"Cannot safely read if stream is not at position {0}.");
            }

            using (var msHeader = new MemoryStream(bhdBytes))
            {
                var brHeader = new BinaryReaderEx(false, msHeader);
                var brData = new BinaryReaderEx(false, bdtStream, true);
                Read(brHeader, brData);
            }
        }

        /// <summary>
        /// Reads a BXF4 from the given BHD and BDT streams.
        /// </summary>
        public BXF4Reader(Stream bhdStream, Stream bdtStream)
        {
            if (bhdStream.Position != 0 && bdtStream.Position != 0)
            {
                // Cannot ensure offset jumping for every format will work otherwise
                throw new InvalidOperationException($"Cannot safely read if streams are not at position {0}.");
            }

            var brHeader = new BinaryReaderEx(false, bhdStream, true);
            var brData = new BinaryReaderEx(false, bdtStream, true);
            Read(brHeader, brData);
        }

        /// <summary>
        /// Reads a BXF4 from the given BHD stream and BDT path.
        /// </summary>
        public BXF4Reader(Stream bhdStream, string bdtPath)
        {
            if (bhdStream.Position != 0)
            {
                // Cannot ensure offset jumping for every format will work otherwise
                throw new InvalidOperationException($"Cannot safely read if stream is not at position {0}.");
            }

            var brHeader = new BinaryReaderEx(false, bhdStream, true);
            var brData = new BinaryReaderEx(false, bdtPath);
            Read(brHeader, brData);
        }

        /// <summary>
        /// Reads a BXF4 from the given BHD stream and BDT bytes.
        /// </summary>
        public BXF4Reader(Stream bhdStream, byte[] bdtBytes)
        {
            if (bhdStream.Position != 0)
            {
                // Cannot ensure offset jumping for every format will work otherwise
                throw new InvalidOperationException($"Cannot safely read if stream is not at position {0}.");
            }

            var brHeader = new BinaryReaderEx(false, bhdStream, true);
            var brData = new BinaryReaderEx(false, bdtBytes);
            Read(brHeader, brData);
        }

        private void Read(BinaryReaderEx brHeader, BinaryReaderEx brData)
        {
            BXF4.ReadBDFHeader(brData);
            Files = BXF4.ReadBHFHeader(this, brHeader);
            DataBR = brData;
        }
    }
}
