using System.Collections.Generic;
using System.IO;

namespace SoulsFormats.Other.MWC
{
    /// <summary>
    /// Also accounts for the DIV variation. The DEV variation has the header detached while the DIV variation places the header at the beginning of the first data file.
    /// </summary>
    public class DEV : SoulsFile<DEV>
    {
        /// <summary>
        /// Archive headers
        /// </summary>
        public List<ArchiveHeader> archiveHeaders = new List<ArchiveHeader>();
        /// <summary>
        /// Sets of file headers
        /// </summary>
        public List<List<FileHeader>> fileHeaderSets = new List<List<FileHeader>>();
        /// <summary>
        /// Sets of files
        /// </summary>
        public List<List<byte[]>> fileSets = new List<List<byte[]>>();
        /// <summary>
        /// File Magic
        /// </summary>
        public string magic;
        /// <summary>
        /// Method to check if this is a DEV/DIV
        /// </summary>
        protected override bool Is(BinaryReaderEx br)
        {
            if (br.Length < 4)
                return false;

            string magic = br.GetASCII(0, 4);
            return magic == "DEV\0" || magic == "DIV\0";
        }
        /// <summary>
        /// Method to read DEV/DIV
        /// </summary>
        protected override void Read(BinaryReaderEx br)
        {
            magic = br.AssertASCII("DEV\0", "DIV\0");
            br.AssertUInt16(0xFFFF);
            br.AssertUInt16(0);
            br.AssertUInt32(0x3F947AE1, 0xCA);
            br.ReadInt32(); //File size

            br.ReadInt32(); //Data size, file size above -0x20
            var archiveFileCount = br.ReadInt32(); //Archive file count (Should only be 1 or 2 for Metal Wolf Chaos)
            br.AssertUInt32(0);
            br.AssertInt16(0x10);
            br.AssertInt16(0x2, 0x5);

            for (int i = 0; i < archiveFileCount; i++)
            {
                ArchiveHeader archiveHeader = new ArchiveHeader();
                archiveHeader.id = br.ReadUInt16();
                archiveHeader.unk0 = br.ReadUInt16();
                archiveHeader.fileCount = br.ReadUInt32();
                archiveHeader.archiveSize = br.ReadUInt32();
                archiveHeader.fileHeadersOffset = br.ReadUInt32();

                archiveHeaders.Add(archiveHeader);
            }

            foreach (var archiveHeader in archiveHeaders)
            {
                var fileHeaderSet = new List<FileHeader>();
                for (int i = 0; i < archiveHeader.fileCount; i++)
                {
                    FileHeader fileHeader = new FileHeader();
                    fileHeader.id = br.ReadUInt16();
                    fileHeader.archiveId = br.ReadUInt16();
                    fileHeader.dataOffset = br.ReadUInt32();
                    fileHeader.dataSize = br.ReadUInt32();
                    fileHeader.fileNameOffset = br.ReadUInt32();

                    br.StepIn(fileHeader.fileNameOffset);
                    fileHeader.fileName = br.ReadShiftJIS();
                    br.StepOut();

                    fileHeaderSet.Add(fileHeader);
                }
                fileHeaderSets.Add(fileHeaderSet);
            }
        }

        /// <summary>
        /// Method to read file data
        /// </summary>
        public void ReadData(string filePath)
        {
            if(magic == "DEV\0")
            {
                for (int i = 0; i < fileHeaderSets.Count; i++)
                {
                    var fileHeaderSet = fileHeaderSets[i];
                    var dataPath = filePath.Replace("_header.dev", $"_data.00{i}");
                    BinaryReaderEx br = new BinaryReaderEx(false, File.ReadAllBytes(dataPath));
                    var fileSet = new List<byte[]>();

                    for (int j = 0; j < fileHeaderSet.Count; j++)
                    {
                        var fileHeader = fileHeaderSet[j];
                        br.Position = fileHeader.dataOffset;
                        fileSet.Add(br.ReadBytes((int)fileHeader.dataSize));
                    }

                    fileSets.Add(fileSet);
                }
            } else if(magic == "DIV\0")
            {
                for (int i = 0; i < fileHeaderSets.Count; i++)
                {
                    var fileHeaderSet = fileHeaderSets[i];
                    var dataPath = Path.ChangeExtension(filePath, $".00{i}");
                    BinaryReaderEx br = new BinaryReaderEx(false, File.ReadAllBytes(dataPath));
                    var fileSet = new List<byte[]>();

                    for (int j = 0; j < fileHeaderSet.Count; j++)
                    {
                        var fileHeader = fileHeaderSet[j];
                        br.Position = fileHeader.dataOffset;
                        fileSet.Add(br.ReadBytes((int)fileHeader.dataSize));
                    }

                    fileSets.Add(fileSet);
                }
            }
        }

        /// <summary>
        /// Archive Header
        /// </summary>
        public class ArchiveHeader
        {
            /// <summary>
            /// Archive id
            /// </summary>
            public ushort id;
            /// <summary>
            /// Unknown int16
            /// </summary>
            public ushort unk0;
            /// <summary>
            /// Offset in this DEV file where file headers start
            /// </summary>
            public uint fileCount;
            /// <summary>
            /// Size of actual archive file
            /// </summary>
            public uint archiveSize;
            /// <summary>
            /// Offset of file headers list
            /// </summary>
            public uint fileHeadersOffset;
        }

        /// <summary>
        /// File header
        /// </summary>
        public class FileHeader
        {
            /// <summary>
            /// Global file id
            /// </summary>
            public ushort id;
            /// <summary>
            /// Some DEVs reference multiple archives. This denotes the archive id
            /// </summary>
            public ushort archiveId;
            /// <summary>
            /// Offset of this file's data
            /// </summary>
            public uint dataOffset;
            /// <summary>
            /// Filesize of this file's data
            /// </summary>
            public uint dataSize;
            /// <summary>
            /// Fileheader's filename offset
            /// </summary>
            public uint fileNameOffset;

            /// <summary>
            /// FileHeader's filename string
            /// </summary>
            public string fileName;
        }

    }
}
