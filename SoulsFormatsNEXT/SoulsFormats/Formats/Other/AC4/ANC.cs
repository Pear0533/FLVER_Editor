namespace SoulsFormats.Other.AC4
{
    /// <summary>
    /// A container containing ANE files relating to ANI animations somehow.
    /// Some ANE files have the same names, so ANC may be used to reuse them while having different names.
    /// </summary>
    public class ANC : SoulsFile<ANC>
    {
        /// <summary>
        /// Unknown; is always "#ANIEDIT", and is usually stored in a field of 0x10 size.
        /// </summary>
        public string AniEdit { get; set; }

        /// <summary>
        /// The name of the ANE file contained within, including the ".ane" extension, formatted "%s.ane";
        /// Appears to always be in a field of 0x10 size.
        /// </summary>
        public string AneName { get; set; }

        /// <summary>
        /// The ANE data contained within the ANC.
        /// </summary>
        public byte[] AneData { get; set; }

        /// <summary>
        /// Deserializes file data from a stream.
        /// </summary>
        protected override void Read(BinaryReaderEx br)
        {
            br.BigEndian = true;
            int aniEditOffset = br.ReadInt32();
            br.AssertUInt32(0);
            br.AssertUInt32(0);
            int aneNameOffset = br.ReadInt32();
            int aneOffset = br.ReadInt32();
            int aneSize = br.ReadInt32();
            br.AssertUInt32(0);
            br.AssertUInt32(0);

            AniEdit = br.GetShiftJIS(aniEditOffset);
            AneName = br.GetShiftJIS(aneNameOffset);
            AneData = br.GetBytes(aneOffset, aneSize);
        }

        /// <summary>
        /// Serializes file data to a stream.
        /// </summary>
        protected override void Write(BinaryWriterEx bw)
        {
            bw.BigEndian = true;
            bw.WriteInt32(32);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(48);
            bw.WriteInt32(64);
            bw.WriteInt32(AneData.Length);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteFixStr(AniEdit, 0x10);
            bw.WriteFixStr(AneName, 0x10);
            bw.WriteBytes(AneData);
        }

        /// <summary>
        /// Checks whether the data appears to be a file of this format.
        /// </summary>
        protected override bool Is(BinaryReaderEx br)
        {
            br.BigEndian = true;
            if (br.Length < 64)
                return false;

            int aniEditOffset = br.GetInt32(0);
            int aneNameOffset = br.GetInt32(0xC);
            int aneOffset = br.GetInt32(0x10);
            int aneSize = br.GetInt32(0x14);

            if (aniEditOffset > br.Length || aneNameOffset > br.Length || aneOffset > br.Length || aneSize > br.Length)
                return false;

            string aniEdit = br.GetASCII(aniEditOffset, 8);
            return aniEdit == "#ANIEDIT" &&
                   aniEditOffset < aneNameOffset &&
                   aneNameOffset < aneOffset &&
                   br.Length - aneOffset == aneSize;
        }
    }
}
