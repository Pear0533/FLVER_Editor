using SoulsFormats.Compression;
using System;
using System.Collections.Generic;
using System.IO;

namespace SoulsFormats.Other
{
    /// <summary>
    /// A split header and data archive of some kind found in:<br/>
    /// Monster Hunter Diary: Poka Poka Airou Village<br/>
    /// Another Century's Episode Portable.<br/>
    /// <br/>
    /// Extensions:<br/>
    /// Header: .bhd<br/>
    /// Data: .bnd
    /// </summary>
    public class LDMU
    {
        /// <summary>
        /// How large sectors are.
        /// </summary>
        private const int SECTOR_SIZE = 0x400;

        /// <summary>
        /// What files align to.
        /// </summary>
        private const int ALIGNMENT_SIZE = 0x800;

        /// <summary>
        /// Unknown; Seen as 528.
        /// </summary>
        public int Unk04 { get; set; }

        /// <summary>
        /// The files in this <see cref="LDMU"/>.
        /// </summary>
        public List<File> Files { get; set; }

        /// <summary>
        /// Creates an empty <see cref="LDMU"/>.
        /// </summary>
        public LDMU()
        {
            Files = new List<File>();
        }

        /// <summary>
        /// Creates an empty <see cref="LDMU"/> initialized with the specified file capacity.
        /// </summary>
        public LDMU(int capacity)
        {
            Files = new List<File>(capacity);
        }

        /// <summary>
        /// Reads an <see cref="LDMU"/>.
        /// </summary>
        /// <param name="br">A <see cref="BinaryReaderEx"/> for reading the <see cref="LDMU"/> header.</param>
        /// <param name="dataStream">A <see cref="Stream"/> containing the data of the <see cref="LDMU"/>.</param>
        private LDMU(BinaryReaderEx br, Stream dataStream)
        {
            br.AssertASCII("LDMU");
            Unk04 = br.ReadInt32(); // Version?
            int fileCount = br.ReadInt32();
            br.ReadInt32(); // Data Size
            br.AssertInt32(0);
            br.AssertInt32(0);
            br.AssertInt32(0);

            Files = new List<File>(fileCount);
            for (int i = 0; i < fileCount; i++)
            {
                Files.Add(new File(br, dataStream));
            }
        }

        #region Read

        /// <summary>
        /// Reads a <see cref="LDMU"/> from the specified paths.
        /// </summary>
        /// <param name="headerPath">The file path to an <see cref="LDMU"/> header.</param>
        /// <param name="dataPath">The file path to <see cref="LDMU"/> data.</param>
        /// <returns>The read <see cref="LDMU"/>.</returns>
        public static LDMU Read(string headerPath, string dataPath)
        {
            using (var br = new BinaryReaderEx(false, headerPath))
            using (var fs = System.IO.File.OpenRead(dataPath))
            {
                return new LDMU(br, fs);
            }
        }

        /// <summary>
        /// Reads a <see cref="LDMU"/> from the specified paths.
        /// </summary>
        /// <param name="headerPath">The file path to an <see cref="LDMU"/> header.</param>
        /// <param name="dataBytes">The bytes of <see cref="LDMU"/> data.</param>
        /// <returns>The read <see cref="LDMU"/>.</returns>
        public static LDMU Read(string headerPath, byte[] dataBytes)
        {
            using (var br = new BinaryReaderEx(false, headerPath))
            using (var ms = new MemoryStream(dataBytes, false))
            {
                return new LDMU(br, ms);
            }
        }

        /// <summary>
        /// Reads a <see cref="LDMU"/> from the specified header path and data stream.
        /// </summary>
        /// <param name="headerPath">The file path to an<see cref="LDMU"/> header.</param>
        /// <param name="dataStream">A <see cref="Stream"/> with the current position starting with a <see cref="LDMU"/> data file.</param>
        /// <returns>The read <see cref="LDMU"/>.</returns>
        public static LDMU Read(string headerPath, Stream dataStream)
        {
            using (var br = new BinaryReaderEx(false, headerPath))
            {
                return new LDMU(br, dataStream);
            }
        }

        /// <summary>
        /// Reads a <see cref="LDMU"/> from the specified byte arrays.
        /// </summary>
        /// <param name="headerBytes">The bytes of an <see cref="LDMU"/> header.</param>
        /// <param name="dataBytes">The bytes of <see cref="LDMU"/> data.</param>
        /// <returns>The read <see cref="LDMU"/>.</returns>
        public static LDMU Read(byte[] headerBytes, byte[] dataBytes)
        {
            using (var br = new BinaryReaderEx(false, headerBytes))
            using (var ms = new MemoryStream(dataBytes, false))
            {
                return new LDMU(br, ms);
            }
        }

        /// <summary>
        /// Reads a <see cref="LDMU"/> from the specified byte arrays.
        /// </summary>
        /// <param name="headerBytes">The bytes of an <see cref="LDMU"/> header.</param>
        /// <param name="dataPath">The file path to <see cref="LDMU"/> data.</param>
        /// <returns>The read <see cref="LDMU"/>.</returns>
        public static LDMU Read(byte[] headerBytes, string dataPath)
        {
            using (var br = new BinaryReaderEx(false, headerBytes))
            using (var fs = System.IO.File.OpenRead(dataPath))
            {
                return new LDMU(br, fs);
            }
        }

        /// <summary>
        /// Reads a <see cref="LDMU"/> from the specified header path and data stream.
        /// </summary>
        /// <param name="headerBytes">The bytes of an <see cref="LDMU"/> header.</param>
        /// <param name="dataStream">A <see cref="Stream"/> with the current position starting with a <see cref="LDMU"/> data file.</param>
        /// <returns>The read <see cref="LDMU"/>.</returns>
        public static LDMU Read(byte[] headerBytes, Stream dataStream)
        {
            using (var br = new BinaryReaderEx(false, headerBytes))
            {
                return new LDMU(br, dataStream);
            }
        }

        /// <summary>
        /// Reads a <see cref="LDMU"/> from the specified byte arrays.
        /// </summary>
        /// <param name="headerStream">The stream of a <see cref="LDMU"/> header.</param>
        /// <param name="dataStream">The stream of <see cref="LDMU"/> data.</param>
        /// <returns>The read <see cref="LDMU"/>.</returns>
        public static LDMU Read(Stream headerStream, Stream dataStream)
        {
            if (headerStream.Position != 0)
            {
                // Cannot ensure offset jumping for every format will work otherwise
                throw new InvalidOperationException($"Cannot safely read if stream is not at position {0}.");
            }

            using (var br = new BinaryReaderEx(false, headerStream, true))
            {
                return new LDMU(br, dataStream);
            }
        }

        /// <summary>
        /// Reads a <see cref="LDMU"/> from the specified byte arrays.
        /// </summary>
        /// <param name="headerStream">The stream of a <see cref="LDMU"/> header.</param>
        /// <param name="dataPath">The file path to <see cref="LDMU"/> data.</param>
        /// <returns>The read <see cref="LDMU"/>.</returns>
        public static LDMU Read(Stream headerStream, string dataPath)
        {
            if (headerStream.Position != 0)
            {
                // Cannot ensure offset jumping for every format will work otherwise
                throw new InvalidOperationException($"Cannot safely read if stream is not at position {0}.");
            }

            using (var br = new BinaryReaderEx(false, headerStream, true))
            using (var fs = System.IO.File.OpenRead(dataPath))
            {
                return new LDMU(br, fs);
            }
        }

        /// <summary>
        /// Reads a <see cref="LDMU"/> from the specified header path and data stream.
        /// </summary>
        /// <param name="headerStream">The stream of a <see cref="LDMU"/> header.</param>
        /// <param name="dataBytes">The bytes of <see cref="LDMU"/> data.</param>
        /// <returns>The read <see cref="LDMU"/>.</returns>
        public static LDMU Read(Stream headerStream, byte[] dataBytes)
        {
            if (headerStream.Position != 0)
            {
                // Cannot ensure offset jumping for every format will work otherwise
                throw new InvalidOperationException($"Cannot safely read if stream is not at position {0}.");
            }

            using (var br = new BinaryReaderEx(false, headerStream, true))
            using (var ms = new MemoryStream(dataBytes, false))
            {
                return new LDMU(br, ms);
            }
        }

        #endregion

        #region Write

        /// <summary>
        /// Writes this <see cref="LDMU"/> to header and data files on the specified paths.
        /// </summary>
        /// <param name="headerPath">The path to write the <see cref="LDMU"/> header to.</param>
        /// <param name="dataPath">The path to write the <see cref="LDMU"/> data to.</param>
        public void Write(string headerPath, string dataPath)
        {
            using (var headerWriter = new BinaryWriterEx(false, headerPath))
            using (var dataWriter = new BinaryWriterEx(false, dataPath))
            {
                Write(headerWriter, dataWriter);
            }
        }

        /// <summary>
        /// Writes this <see cref="LDMU"/> to the specified header and data streams.
        /// </summary>
        /// <param name="headerStream">The stream to write the <see cref="LDMU"/> header to.</param>
        /// <param name="dataStream">The stream to write the <see cref="LDMU"/> data to.</param>
        /// <param name="leaveHeaderStreamOpen">Whether or not to leave the header stream open after writing is finished.</param>
        /// <param name="leaveDataStreamOpen">Whether or not to leave the data stream open after writing is finished.</param>
        public void Write(Stream headerStream, Stream dataStream, bool leaveHeaderStreamOpen = false, bool leaveDataStreamOpen = false)
        {
            using (var headerWriter = new BinaryWriterEx(false, headerStream, leaveHeaderStreamOpen))
            using (var dataWriter = new BinaryWriterEx(false, dataStream, leaveDataStreamOpen))
            {
                Write(headerWriter, dataWriter);
            }
        }

        /// <summary>
        /// Writes this <see cref="LDMU"/> to the specified header path and data stream.
        /// </summary>
        /// <param name="headerPath">The path to write the <see cref="LDMU"/> header to.</param>
        /// <param name="dataStream">The stream to write the <see cref="LDMU"/> data to.</param>
        /// <param name="leaveDataStreamOpen">Whether or not to leave the data stream open after writing is finished.</param>
        public void Write(string headerPath, Stream dataStream, bool leaveDataStreamOpen = false)
        {
            using (var headerWriter = new BinaryWriterEx(false, headerPath))
            using (var dataWriter = new BinaryWriterEx(false, dataStream, leaveDataStreamOpen))
            {
                Write(headerWriter, dataWriter);
            }
        }

        /// <summary>
        /// Writes this <see cref="LDMU"/>.
        /// </summary>
        /// <param name="headerWriter">A <see cref="BinaryWriterEx"/> for writing the header.</param>
        /// <param name="dataWriter">A <see cref="BinaryWriterEx"/> for writing the data.</param>
        private void Write(BinaryWriterEx headerWriter, BinaryWriterEx dataWriter)
        {
            headerWriter.WriteASCII("LDMU", false);
            headerWriter.WriteInt32(Unk04); // Version?
            headerWriter.WriteInt32(Files.Count);
            headerWriter.ReserveInt32("DataSize");
            headerWriter.WriteInt32(0);
            headerWriter.WriteInt32(0);
            headerWriter.WriteInt32(0);

            foreach (var file in Files)
            {
                file.Write(headerWriter, dataWriter);
            }

            headerWriter.FillInt32("DataSize", (int)dataWriter.Length);
        }

        #endregion

        #region Is

        /// <summary>
        /// Whether or not the file at the specified path is an <see cref="LDMU"/> or not.
        /// </summary>
        /// <param name="path">The file path to an <see cref="LDMU"/> header.</param>
        /// <returns>Whether or not the file is an <see cref="LDMU"/>.</returns>
        public static bool Is(string path)
        {
            using (var br = new BinaryReaderEx(false, path))
            {
                return Is(br);
            }
        }

        /// <summary>
        /// Whether or not the specified bytes are an <see cref="LDMU"/> or not.
        /// </summary>
        /// <param name="bytes">The bytes of an <see cref="LDMU"/> header.</param>
        /// <returns>Whether or not the bytes are an <see cref="LDMU"/>.</returns>
        public static bool Is(byte[] bytes)
        {
            using (var br = new BinaryReaderEx(false, bytes))
            {
                return Is(br);
            }
        }

        /// <summary>
        /// Whether or not the data is an <see cref="LDMU"/>.
        /// </summary>
        /// <param name="br">A <see cref="BinaryReaderEx"/> for reading.</param>
        /// <returns>Whether or not the data is an <see cref="LDMU"/>.</returns>
        private static bool Is(BinaryReaderEx br)
            => br.Length > 4 && br.GetASCII(br.Position, 4) == "LDMU";

        #endregion

        #region File

        /// <summary>
        /// A file in a <see cref="LDMU"/>.
        /// </summary>
        public class File
        {
            /// <summary>
            /// The ID of this file.
            /// </summary>
            public int ID { get; set; }

            /// <summary>
            /// The data of the file.
            /// </summary>
            public byte[] Bytes { get; set; }

            /// <summary>
            /// Unknown; May be a hash or checksum of some kind.
            /// </summary>
            public byte[] Unk10 { get; set; }

            /// <summary>
            /// Whether or not to compress this file.
            /// </summary>
            public bool Compress { get; set; }

            /// <summary>
            /// Create an empty file with a placeholder ID.
            /// </summary>
            public File()
            {
                ID = -1;
                Bytes = Array.Empty<byte>();
                Unk10 = new byte[24];
                Compress = true;
            }

            /// <summary>
            /// Create an empty file with the specified ID.
            /// </summary>
            /// <param name="id">The ID of the file.</param>
            public File(int id)
            {
                ID = id;
                Bytes = Array.Empty<byte>();
                Unk10 = new byte[24];
                Compress = true;
            }

            /// <summary>
            /// Create a file with the specified bytes and a placeholder ID.
            /// </summary>
            /// <param name="bytes">The data of the file.</param>
            public File(byte[] bytes)
            {
                ID = -1;
                Bytes = bytes;
                Unk10 = new byte[24];
                Compress = true;
            }

            /// <summary>
            /// Create a file with the specified bytes and ID.
            /// </summary>
            /// <param name="id">The ID of the file.</param>
            /// <param name="bytes">The data of the file.</param>
            public File(int id, byte[] bytes)
            {
                ID = id;
                Bytes = bytes;
                Unk10 = new byte[24];
                Compress = true;
            }

            /// <summary>
            /// Read a file.
            /// </summary>
            /// <param name="br">A <see cref="BinaryReaderEx"/> for reading the header.</param>
            /// <param name="dataStream">A <see cref="Stream"/> with the data.</param>
            /// <exception cref="Exception">Could not read the required number of bytes from the data stream.</exception>
            internal File(BinaryReaderEx br, Stream dataStream)
            {
                ID = br.ReadInt32();
                int sectorOffset = br.ReadInt32();
                int uncompressedLength = br.ReadInt32();
                int compressedLength = br.ReadInt32();
                Unk10 = br.ReadBytes(24); // Hash? Checksum? Both?

                Bytes = new byte[uncompressedLength];
                dataStream.Position = sectorOffset * SECTOR_SIZE;

                Compress = compressedLength > 0;
                if (Compress)
                {
                    Bytes = ZlibHelper.DecompressZlib(dataStream, compressedLength);
                }
                else
                {
                    if (dataStream.Read(Bytes, 0, uncompressedLength) < uncompressedLength)
                    {
                        throw new Exception("Could not read the required number of bytes from the stream.");
                    }
                }
            }

            /// <summary>
            /// Write this file.
            /// </summary>
            /// <param name="headerWriter">A <see cref="BinaryWriterEx"/> for writing the header.</param>
            /// <param name="dataWriter">A <see cref="BinaryWriterEx"/> for writing the data.</param>
            internal void Write(BinaryWriterEx headerWriter, BinaryWriterEx dataWriter)
            {
                headerWriter.WriteInt32(ID);
                headerWriter.WriteInt32((int)(dataWriter.Position / SECTOR_SIZE));
                headerWriter.WriteInt32(Bytes.Length);

                if (Compress)
                {
                    int bytesWritten = ZlibHelper.WriteZlib(dataWriter, 0x9C, Bytes);
                    headerWriter.WriteInt32(bytesWritten);
                }
                else
                {
                    headerWriter.WriteInt32(0);
                    dataWriter.WriteBytes(Bytes);
                }

                headerWriter.WriteBytes(Unk10);
                dataWriter.Pad(ALIGNMENT_SIZE);
            }
        }

        #endregion
    }
}
