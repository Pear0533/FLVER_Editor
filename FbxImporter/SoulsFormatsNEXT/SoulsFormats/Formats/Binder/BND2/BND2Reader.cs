using System;
using System.Collections.Generic;
using System.IO;

namespace SoulsFormats
{
    /// <summary>
    /// An on-demand reader for <see cref="BND2"/> containers.
    /// </summary>
    public class BND2Reader : IBND2, IDisposable
    {
        /// <summary>
        /// Header info flags?
        /// </summary>
        public BND2.HeaderInfoFlagsEnum HeaderInfoFlags { get; set; }

        /// <summary>
        /// File info flags.
        /// </summary>
        public BND2.FileInfoFlagsEnum FileInfoFlags { get; set; }

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
        public BND2.FilePathModeEnum FilePathMode { get; set; }

        /// <summary>
        /// Unknown; Was found set to 1 on files extracted from memory.
        /// Usually set to 0.
        /// </summary>
        public byte Unk1B { get; set; }

        /// <summary>
        /// The base directory of all files.
        /// <para>Only used when <see cref="BND2.FilePathModeEnum.BaseDirectory"/> is set on <see cref="FilePathMode"/>.</para>
        /// </summary>
        public string BaseDirectory { get; set; }

        /// <summary>
        /// Metadata for files present in the <see cref="BND2"/>.
        /// </summary>
        public List<BND2FileHeader> Files { get; set; }

        /// <summary>
        /// Reader to read file data from.
        /// </summary>
        protected BinaryReaderEx DataBR;

        /// <summary>
        /// Reads a <see cref="BND2"/> from the given path, decompressing if necessary.
        /// </summary>
        public BND2Reader(string path)
        {
            var br = new BinaryReaderEx(false, path);
            Read(br);
        }

        /// <summary>
        /// Reads a <see cref="BND2"/> from the given bytes, decompressing if necessary.
        /// </summary>
        public BND2Reader(byte[] bytes)
        {
            var br = new BinaryReaderEx(false, bytes);
            Read(br);
        }

        /// <summary>
        /// Reads a <see cref="BND2"/> from the given <see cref="Stream"/>, decompressing if necessary.
        /// </summary>
        public BND2Reader(Stream stream)
        {
            if (stream.Position != 0)
            {
                // Cannot ensure offset jumping for every format will work otherwise
                throw new InvalidOperationException($"Cannot safely read if stream is not at position {0}.");
            }

            var br = new BinaryReaderEx(false, stream, true);
            Read(br);
        }

        /// <summary>
        /// Get file headers for a <see cref="BND2"/> from the given path.
        /// </summary>
        public static List<BND2FileHeader> GetFileHeaders(string path)
        {
            var reader = new BND2Reader(path);
            reader.DataBR.Dispose();
            return reader.Files;
        }

        /// <summary>
        /// Get file headers for a <see cref="BND2"/> from the given bytes.
        /// </summary>
        public static List<BND2FileHeader> GetFileHeaders(byte[] bytes)
        {
            var reader = new BND2Reader(bytes);
            reader.DataBR.Dispose();
            return reader.Files;
        }

        /// <summary>
        /// Get file headers for a <see cref="BND2"/> from the given <see cref="Stream"/>.
        /// </summary>
        public static List<BND2FileHeader> GetFileHeaders(Stream stream)
        {
            var reader = new BND2Reader(stream);
            reader.DataBR.Dispose();
            return reader.Files;
        }

        private void Read(BinaryReaderEx br)
        {
            Files = BND2.ReadHeader(this, br);
            DataBR = br;
        }

        /// <summary>
        /// Reads file data according to the header at the given index in Files.
        /// </summary>
        public byte[] ReadFile(int index)
        {
            return ReadFile(Files[index]);
        }

        /// <summary>
        /// Reads file data according to the given header.
        /// </summary>
        public byte[] ReadFile(BND2FileHeader fileHeader)
        {
            BND2.File file = fileHeader.ReadFileData(DataBR);
            return file.Bytes;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Releases the unmanaged resources used by the BinderReader and optionally releases the managed resources.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DataBR?.Dispose();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Releases all resources used by the BinderReader.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
