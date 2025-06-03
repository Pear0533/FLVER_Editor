using System;
using System.Collections.Generic;

namespace SoulsFormats
{
    /// <summary>
    /// BND file, used in older FromSoft titles such as Metal Wolf Chaos
    /// </summary>
    public class BND : SoulsFile<BND>, IBinder
    {
        /// <summary>
        /// BND version, seemingly.
        /// </summary>
        public int internalVersion;

        /// <summary>
        /// The first BND format definition
        /// </summary>
        public ushort format0;

        /// <summary>
        /// The second BND format definition.
        /// Bit 0 determines if filenames exist
        /// Bit 1 determines if the rootFilePath exists
        /// </summary>
        public ushort format1;

        /// <summary>
        /// Format for BND3+. Format is entirely different in BND.
        /// </summary>
        public Binder.Format Format { get => 0; set => Format = 0; }

        /// <summary>
        /// Interally, the version is an int for BND
        /// </summary>
        public string Version { get => internalVersion.ToString(); set => internalVersion = Int32.Parse(value); }

        /// <summary>
        /// The files in the BND
        /// </summary>
        public List<BinderFile> Files { get; set; }

        /// <summary>
        /// The root file path. Not all BNDs have this.
        /// </summary>
        public string rootFilePath = "";

        /// <summary>
        /// Checks whether the data appears to be a file of this format.
        /// </summary>
        protected override bool Is(BinaryReaderEx br)
        {
            if (br.Length < 4)
                return false;

            string magic = br.GetASCII(0, 4);
            return magic == "BND\0";
        }

        /// <summary>
        /// Deserializes file data from a stream.
        /// </summary>
        protected override void Read(BinaryReaderEx br)
        {
            List<BinderFileHeader> fileHeaders = ReadHeader(this, br);
            Files = new List<BinderFile>(fileHeaders.Count);
            foreach (BinderFileHeader fileHeader in fileHeaders)
                Files.Add(fileHeader.ReadFileData(br));
        }

        /// <summary>
        /// Reads the BND header
        /// </summary>
        /// <param name="bnd"></param>
        /// <param name="br"></param>
        /// <returns>BinderFileHeaders which contain everything needed to grab a file from the BinaryReaderEx</returns>
        internal static List<BinderFileHeader> ReadHeader(BND bnd, BinaryReaderEx br)
        {
            br.AssertASCII("BND\0");
            br.AssertUInt16(0xFFFF);
            br.AssertUInt16(0);
            bnd.internalVersion = br.ReadInt32();
            br.ReadUInt32(); //Filesize
            var fileCount = br.ReadInt32();
            var rootFilePathOffset = br.ReadInt32(); //This will be 0 if there is no rootFilePath
            if (rootFilePathOffset != 0)
            {
                br.StepIn(rootFilePathOffset);
                bnd.rootFilePath = br.ReadASCII();
                br.StepOut();
            }

            bnd.format0 = br.ReadUInt16(); //Unknown
            bnd.format1 = br.ReadUInt16();
            br.AssertInt32(0);

            var fileHeaders = new List<BinderFileHeader>(fileCount);
            for (int i = 0; i < fileCount; i++)
            {
                fileHeaders.Add(ReadBinderFileHeader(br));
            }

            return fileHeaders;
        }

        /// <summary>
        /// Reads a BND file header
        /// </summary>
        /// <param name="br"></param>
        /// <returns>BinderFileHeader which contains everything needed to grab a file from the BinaryReaderEx</returns>
        internal static BinderFileHeader ReadBinderFileHeader(BinaryReaderEx br)
        {
            var id = br.ReadInt32();
            var dataOffset = br.ReadUInt32();
            var fileSize = br.ReadUInt32();
            var nameOffset = br.ReadUInt32();

            string name = $"File_{id}";
            if (nameOffset != 0)
            {
                br.StepIn(nameOffset);
                name = br.ReadShiftJIS();
                br.StepOut();
            }

            return new BinderFileHeader(id, name) { DataOffset = dataOffset, UncompressedSize = fileSize, CompressedSize = fileSize };
        }

        /// <summary>
        /// Write BND with rootFilePath
        /// </summary>
        /// <param name="bw"></param>
        /// <param name="rootFilePath"></param>
        protected void Write(BinaryWriterEx bw, string rootFilePath)
        {
            this.rootFilePath = rootFilePath;
            Write(bw);
        }

        /// <summary>
        /// Write BND
        /// </summary>
        /// <param name="bw"></param>
        protected override void Write(BinaryWriterEx bw)
        {
            bw.WriteASCII("BND\0");
            bw.WriteUInt16(0xFFFF);
            bw.WriteUInt16(0);
            bw.WriteInt32(internalVersion);
            bw.ReserveInt32("FileSize");

            bw.WriteInt32(Files.Count);
            bw.ReserveInt32("RootFilePath");
            bw.WriteUInt16(format0);
            bw.WriteUInt16(format1);
            bw.WriteUInt32(0);

            //Write file headers
            for (int i = 0; i < Files.Count; i++)
            {
                bw.WriteInt32(i);
                bw.ReserveInt32($"FileOffset{i}");
                bw.WriteInt32(Files[i].Bytes.Length);
                bw.ReserveInt32($"FileName{i}");
            }

            //Write RootFilePath
            if (rootFilePath != "")
            {
                bw.FillInt32("RootFilePath", (int)bw.Position);
                bw.WriteShiftJIS(rootFilePath, true);
            }
            else
            {
                bw.FillInt32("RootFilePath", 0);
            }

            //Names
            for (int i = 0; i < Files.Count; i++)
            {
                bw.FillInt32($"FileName{i}", (int)bw.Position);
                bw.WriteShiftJIS(Files[i].Name, true);
            }
            bw.Pad(0x10);

            //Files
            for (int i = 0; i < Files.Count; i++)
            {
                bw.FillInt32($"FileOffset{i}", (int)bw.Position);
                bw.WriteBytes(Files[i].Bytes);

                if (i != Files.Count - 1)
                {
                    bw.Pad(0x10);
                }
            }

            bw.FillInt32("FileSize", (int)bw.Position);
        }
    }
}