using System.Collections.Generic;

namespace SoulsFormats.Other.MWC
{
    /// <summary>
    /// Container for model-related files used in Metal Wolf Chaos. Extension: _m.dat
    /// </summary>
    public class MDAT : SoulsFile<MDAT>
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public int Unk1C;
        public byte[] mdlData;
        public byte[] Data2;
        public byte[] Data3;
        public byte[] Data5;
        public byte[] Data6;

        protected override void Read(BinaryReaderEx br)
        {
            br.BigEndian = false;
            int fileSize = br.ReadInt32();
            int mdlDataOffset = br.ReadInt32();
            int offset2 = br.ReadInt32();
            int offset3 = br.ReadInt32();
            br.AssertInt32(0);
            int offset5 = br.ReadInt32();
            int offset6 = br.ReadInt32();
            Unk1C = br.ReadInt32();

            var offsets = new List<int> { fileSize, mdlDataOffset, offset2, offset3, offset5, offset6 };
            offsets.Sort();

            if (mdlDataOffset != 0)
                this.mdlData = br.GetBytes(mdlDataOffset, offsets[offsets.IndexOf(mdlDataOffset) + 1] - mdlDataOffset);
            if (offset2 != 0)
                Data2 = br.GetBytes(offset2, offsets[offsets.IndexOf(offset2) + 1] - offset2);
            if (offset3 != 0)
                Data3 = br.GetBytes(offset3, offsets[offsets.IndexOf(offset3) + 1] - offset3);
            if (offset5 != 0)
                Data5 = br.GetBytes(offset5, offsets[offsets.IndexOf(offset5) + 1] - offset5);
            if (offset6 != 0)
                Data6 = br.GetBytes(offset6, offsets[offsets.IndexOf(offset6) + 1] - offset6);
        }

        protected override void Write(BinaryWriterEx bw)
        {
            int currentOffset = 0x20;
            bw.WriteInt32(mdlData.Length + Data2.Length + Data3.Length + Data5.Length + Data6.Length + 0x20);

            //Write model data
            WriteDataOffset(bw, ref currentOffset, mdlData.Length);

            //Write Data2
            WriteDataOffset(bw, ref currentOffset, Data2.Length);

            //Write Data3
            WriteDataOffset(bw, ref currentOffset, Data3.Length);

            bw.WriteInt32(0);

            //Write Data5
            WriteDataOffset(bw, ref currentOffset, Data5.Length);

            //Write Data6
            WriteDataOffset(bw, ref currentOffset, Data6.Length);

            bw.WriteInt32(Unk1C);

            WriteData(bw, mdlData);
            WriteData(bw, Data2);
            WriteData(bw, Data3);
            WriteData(bw, Data5);
            WriteData(bw, Data6);
        }

        public static void WriteData(BinaryWriterEx bw, byte[] data)
        {
            if(data.Length > 0)
            {
                bw.WriteBytes(data);
                bw.Pad(0x10);
            }
        }

        public static void WriteDataOffset(BinaryWriterEx bw, ref int currentOffset, int dataLength)
        {
            if(dataLength > 0)
            {
                bw.WriteInt32(currentOffset);
                currentOffset += dataLength;
            } else
            {
                bw.WriteInt32(0);
            }
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
