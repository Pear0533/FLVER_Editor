using SoulsFormats.Compression;
using SoulsFormats.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace SoulsFormats
{
    /// <summary>
    /// A simplistic data archive format with a set number of entries for a header.
    /// </summary>
    public class FSDATA
    {
        /// <summary>
        /// How large sectors are.
        /// </summary>
        private const int SECTOR_SIZE = 0x800;

        /// <summary>
        /// What files align to.
        /// </summary>
        private const int ALIGNMENT_SIZE = 0x8000;

        /// <summary>
        /// The files in this <see cref="FSDATA"/>.
        /// </summary>
        public List<File> Files { get; set; }

        /// <summary>
        /// Whether or not this <see cref="FSDATA"/> should be compressed.
        /// </summary>
        public bool Compressed { get; set; }

        /// <summary>
        /// The number of entries in this <see cref="FSDATA"/>.
        /// </summary>
        // Placed in a field for value validation.
        private int _entryCount;

        /// <summary>
        /// The number of entries in this <see cref="FSDATA"/>.
        /// </summary>
        public int EntryCount
        {
            get => _entryCount;
            set
            {
                if (value % SECTOR_SIZE != 0)
                {
                    throw new ArgumentException($"{nameof(value)} must be divisible by {SECTOR_SIZE}", nameof(value));
                }

                _entryCount = value;
            }
        }

        /// <summary>
        /// Creates a new <see cref="FSDATA"/> with a default entry count and no compression.
        /// </summary>
        public FSDATA()
        {
            _entryCount = 8192;
            Compressed = false;
            Files = new List<File>();
        }

        /// <summary>
        /// Creates a new <see cref="FSDATA"/> with a default entry count and the specified state of compression.
        /// </summary>
        /// <param name="compressed">Whether or not files should be compressed when writing.</param>
        public FSDATA(bool compressed)
        {
            _entryCount = 8192;
            Compressed = compressed;
            Files = new List<File>();
        }

        /// <summary>
        /// Creates a new <see cref="FSDATA"/> with the specified entry count and state of compression.
        /// </summary>
        /// <param name="entryCount">The number of entries in the <see cref="FSDATA"/>.</param>
        /// <param name="compressed">Whether or not files should be compressed when writing.</param>
        public FSDATA(int entryCount, bool compressed)
        {
            EntryCount = entryCount;
            Compressed = compressed;
            Files = new List<File>(entryCount);
        }

        /// <summary>
        /// Reads a <see cref="FSDATA"/> with the specified entry count and state of compression.
        /// </summary>
        /// <param name="br">A <see cref="BinaryReaderEx"/> to read the <see cref="FSDATA"/>.</param>
        /// <param name="entryCount">The number of entries in the <see cref="FSDATA"/>.</param>
        /// <param name="compressed">Whether or not files are compressed.</param>
        private FSDATA(BinaryReaderEx br, int entryCount, bool compressed)
        {
            EntryCount = entryCount;
            Compressed = compressed;
            Files = new List<File>(entryCount);
            int dataOffset = (entryCount * 4) * (Compressed ? 3 : 2);
            for (int i = 0; i < entryCount; i++)
            {
                var file = new File(br, i, dataOffset, Compressed, out bool emptyEntry);
                if (!emptyEntry)
                {
                    Files.Add(file);
                }
            }
        }

        #region Read

        /// <summary>
        /// Reads a <see cref="FSDATA"/> from the specified path.
        /// </summary>
        /// <param name="path">The file path to a <see cref="FSDATA"/>.</param>
        /// <param name="entryCount">The number of entries in the <see cref="FSDATA"/>.</param>
        /// <param name="compressed">Whether or not files are compressed.</param>
        /// <returns>The read <see cref="FSDATA"/>.</returns>
        public static FSDATA Read(string path, int entryCount, bool compressed)
        {
            using (var br = new BinaryReaderEx(false, path))
            {
                return new FSDATA(br, entryCount, compressed);
            }
        }

        /// <summary>
        /// Reads a <see cref="FSDATA"/> from the specified bytes.
        /// </summary>
        /// <param name="bytes">The bytes of a <see cref="FSDATA"/>.</param>
        /// <param name="entryCount">The number of entries in the <see cref="FSDATA"/>.</param>
        /// <param name="compressed">Whether or not files are compressed.</param>
        /// <returns>The read <see cref="FSDATA"/>.</returns>
        public static FSDATA Read(byte[] bytes, int entryCount, bool compressed)
        {
            using (var br = new BinaryReaderEx(false, bytes))
            {
                return new FSDATA(br, entryCount, compressed);
            }
        }

        /// <summary>
        /// Reads a <see cref="FSDATA"/> from the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> with the current position starting with a <see cref="FSDATA"/>.</param>
        /// <param name="entryCount">The number of entries in the <see cref="FSDATA"/>.</param>
        /// <param name="compressed">Whether or not files are compressed.</param>
        /// <param name="leaveStreamOpen">Whether or not to leave the provided stream open after reading is finished.</param>
        /// <returns>The read <see cref="FSDATA"/>.</returns>
        public static FSDATA Read(Stream stream, int entryCount, bool compressed, bool leaveStreamOpen = false)
        {
            using (var br = new BinaryReaderEx(false, stream, leaveStreamOpen))
            {
                return new FSDATA(br, entryCount, compressed);
            }
        }

        #endregion

        #region Write

        /// <summary>
        /// Writes this <see cref="FSDATA"/> to a file on the specified path.
        /// </summary>
        /// <param name="path">The path to write the <see cref="FSDATA"/> to.</param>
        public void Write(string path)
        {
            using (var bw = new BinaryWriterEx(false, path))
            {
                Write(bw);
            }
        }

        /// <summary>
        /// Writes this <see cref="FSDATA"/> to a new byte array.
        /// </summary>
        /// <returns>A byte array.</returns>
        public byte[] Write()
        {
            var bw = new BinaryWriterEx(false);
            Write(bw);
            return bw.FinishBytes();
        }

        /// <summary>
        /// Writes this <see cref="FSDATA"/> to a <see cref="Stream"/> at the current position.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/>.</param>
        /// <param name="leaveOpen">Whether or not to leave the <see cref="Stream"/> open after writing has finished.</param>
        public void Write(Stream stream, bool leaveOpen = false)
        {
            using (var bw = new BinaryWriterEx(false, stream, leaveOpen))
            {
                Write(bw);
            }
        }
        
        /// <summary>
        /// Writes this <see cref="FSDATA"/>.
        /// </summary>
        /// <param name="bw">A <see cref="BinaryWriterEx"/> for writing.</param>
        private void Write(BinaryWriterEx bw)
        {
            int entrySize = 4 * (Compressed ? 3 : 2);
            int entriesSize = _entryCount * entrySize;

            var sortedFiles = Files;
            sortedFiles.Sort((x, y) => x.ID.CompareTo(y.ID));
            var usedIDs = new List<int>(_entryCount);

            int previousIndex = 0;
            int count = sortedFiles.Count;
            for (int i = 0; i < count; i++)
            {
                var file = sortedFiles[i];
                if (usedIDs.Contains(file.ID))
                {
                    throw new InvalidOperationException($"ID already taken: {file.ID}");
                }

                if (file.ID < 0)
                {
                    throw new InvalidOperationException($"ID must be greater than or equal to 0: {file.ID}");
                }

                if (file.ID >= _entryCount)
                {
                    throw new IndexOutOfRangeException($"ID must be less than or equal to {nameof(EntryCount)} - 1: {file.ID}");
                }

                while (file.ID > previousIndex)
                {
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    if (Compressed)
                    {
                        bw.WriteInt32(0);
                    }

                    previousIndex++;
                }

                file.Write(bw, Compressed, i);
                usedIDs.Add(file.ID);
                previousIndex++;
            }

            while (_entryCount > previousIndex)
            {
                bw.WriteInt32(0);
                bw.WriteInt32(0);
                if (Compressed)
                {
                    bw.WriteInt32(0);
                }

                previousIndex++;
            }

            for (int i = 0; i < count; i++)
            {
                var file = sortedFiles[i];
                bw.FillInt32($"SectorOffset_{i}", (int)((bw.Position - entriesSize) / SECTOR_SIZE));
                if (Compressed)
                {
                    int bytesWritten = ZlibHelper.WriteZlib(bw, 0x9C, file.Bytes);
                    bw.FillInt32($"SectorLength_{i}", MathHelper.Align(bytesWritten, SECTOR_SIZE) / SECTOR_SIZE);
                }
                else
                {
                    bw.WriteBytes(file.Bytes);
                }
                bw.Pad(ALIGNMENT_SIZE);
            }
        }

        #endregion

        #region File

        /// <summary>
        /// A file in a <see cref="FSDATA"/>.
        /// </summary>
        public class File
        {
            /// <summary>
            /// The ID of the file and also the index it appears at in file entries when written to a <see cref="FSDATA"/>.
            /// </summary>
            public int ID { get; set; }

            /// <summary>
            /// The data of the file.
            /// </summary>
            public byte[] Bytes { get; set; }

            /// <summary>
            /// Create an empty file with a placeholder ID.
            /// </summary>
            public File()
            {
                ID = -1;
                Bytes = Array.Empty<byte>();
            }

            /// <summary>
            /// Create an empty file with the provided ID.
            /// </summary>
            /// <param name="id">The ID of the file.</param>
            public File(int id)
            {
                ID = id;
                Bytes = Array.Empty<byte>();
            }

            /// <summary>
            /// Create a file with the provided bytes and a placeholder ID.
            /// </summary>
            /// <param name="bytes">The data of the file.</param>
            public File(byte[] bytes)
            {
                ID = -1;
                Bytes = bytes;
            }

            /// <summary>
            /// Create a file with the provided bytes and ID.
            /// </summary>
            /// <param name="id">The ID of the file.</param>
            /// <param name="bytes">The data of the file.</param>
            public File(int id, byte[] bytes)
            {
                ID = id;
                Bytes = bytes;
            }

            /// <summary>
            /// Read a file.
            /// </summary>
            /// <param name="br">A <see cref="BinaryReaderEx"/> for reading.</param>
            /// <param name="index">The index of the file.</param>
            /// <param name="dataOffset">The offset data begins at.</param>
            /// <param name="compressed">Whether or not the data is compressed.</param>
            /// <param name="emptyEntry">Whether or not the entry was entirely empty.</param>
            internal File(BinaryReaderEx br, int index, int dataOffset, bool compressed, out bool emptyEntry)
            {
                ID = index;
                int sectorOffset = br.ReadInt32();

                if (compressed)
                {
                    br.ReadInt32(); // Decompressed Sector Length
                    int sectorLength = br.ReadInt32();

                    if (sectorOffset == 0 && sectorLength == 0)
                    {
                        emptyEntry = true;
                    }
                    else
                    {
                        emptyEntry = false;
                    }

                    if (sectorLength > 0)
                    {
                        br.StepIn(dataOffset + (sectorOffset * SECTOR_SIZE));
                        Bytes = ZlibHelper.ReadZlib(br, sectorLength * SECTOR_SIZE);
                        br.StepOut();
                    }
                    else
                    {
                        Bytes = Array.Empty<byte>();
                    }
                }
                else
                {
                    int sectorLength = br.ReadInt32();

                    if (sectorOffset == 0 && sectorLength == 0)
                    {
                        emptyEntry = true;
                    }
                    else
                    {
                        emptyEntry = false;
                    }

                    if (sectorLength > 0)
                    {
                        br.StepIn(dataOffset + (sectorOffset * SECTOR_SIZE));
                        Bytes = br.ReadBytes(sectorLength * SECTOR_SIZE);
                        br.StepOut();
                    }
                    else
                    {
                        Bytes = Array.Empty<byte>();
                    }
                }
            }

            /// <summary>
            /// Writes this <see cref="File"/>.
            /// </summary>
            /// <param name="bw">A <see cref="BinaryWriterEx"/> for writing.</param>
            /// <param name="compressed">Whether or not data is to be compressed.</param>
            /// <param name="index">The current index of the file so entry fields can be reserved.</param>
            internal void Write(BinaryWriterEx bw, bool compressed, int index)
            {
                bw.ReserveInt32($"SectorOffset_{index}");
                bw.WriteInt32(MathHelper.Align(Bytes.Length, SECTOR_SIZE) / SECTOR_SIZE);
                if (compressed)
                {
                    bw.ReserveInt32($"SectorLength_{index}");
                }
            }
        }

        #endregion
    }
}
