using System;
using System.IO;

namespace SoulsFormats
{
    /// <summary>
    /// An on-demand reader for BXF3 containers.
    /// </summary>
    public class BXF3Reader : BinderReader, IBXF3
    {
        /// <summary>
        /// Reads a BXF3 from the given BHD and BDT paths.
        /// </summary>
        public BXF3Reader(string bhdPath, string bdtPath)
        {
            using (FileStream fsHeader = File.OpenRead(bhdPath))
            {
                var brHeader = new BinaryReaderEx(false, fsHeader);
                var brData = new BinaryReaderEx(false, bdtPath);
                Read(brHeader, brData);
            }
        }

        /// <summary>
        /// Reads a BXF3 from the given BHD path and BDT bytes.
        /// </summary>
        public BXF3Reader(string bhdPath, byte[] bdtBytes)
        {
            using (FileStream fsHeader = File.OpenRead(bhdPath))
            {
                var brHeader = new BinaryReaderEx(false, fsHeader);
                var brData = new BinaryReaderEx(false, bdtBytes);
                Read(brHeader, brData);
            }
        }

        /// <summary>
        /// Reads a BXF3 from the given BHD path and BDT stream.
        /// </summary>
        public BXF3Reader(string bhdPath, Stream bdtStream)
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
        /// Reads a BXF3 from the given BHD and BDT bytes.
        /// </summary>
        public BXF3Reader(byte[] bhdBytes, byte[] bdtBytes)
        {
            using (var msHeader = new MemoryStream(bhdBytes))
            {
                var brHeader = new BinaryReaderEx(false, msHeader);
                var brData = new BinaryReaderEx(false, bdtBytes);
                Read(brHeader, brData);
            }
        }

        /// <summary>
        /// Reads a BXF3 from the given BHD bytes and BDT stream.
        /// </summary>
        public BXF3Reader(byte[] bhdBytes, string bdtPath)
        {
            using (var msHeader = new MemoryStream(bhdBytes))
            {
                var brHeader = new BinaryReaderEx(false, msHeader);
                var brData = new BinaryReaderEx(false, bdtPath);
                Read(brHeader, brData);
            }
        }

        /// <summary>
        /// Reads a BXF3 from the given BHD bytes and BDT path.
        /// </summary>
        public BXF3Reader(byte[] bhdBytes, Stream bdtStream)
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
        /// Reads a BXF3 from the given BHD and BDT streams.
        /// </summary>
        public BXF3Reader(Stream bhdStream, Stream bdtStream)
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
        /// Reads a BXF3 from the given BHD stream and BDT path.
        /// </summary>
        public BXF3Reader(Stream bhdStream, string bdtPath)
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
        /// Reads a BXF3 from the given BHD stream and BDT bytes.
        /// </summary>
        public BXF3Reader(Stream bhdStream, byte[] bdtBytes)
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
            BXF3.ReadBDFHeader(brData);
            Files = BXF3.ReadBHFHeader(this, brHeader);
            DataBR = brData;
        }
    }
}
