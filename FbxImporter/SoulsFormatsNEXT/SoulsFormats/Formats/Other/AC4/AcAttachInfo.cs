using System.Collections.Generic;

namespace SoulsFormats.Other.AC4
{
    /// <summary>
    /// Details what should attach to what in Armored Core For Answer parts.<br/>
    /// Does not contain the main things to attach, normally only has special cases.
    /// </summary>
    public class AcAttachInfo : SoulsFile<AcAttachInfo>
    {
        /// <summary>
        /// The attach entries.
        /// </summary>
        public List<AttachEntry> Entries { get; set; }

        /// <summary>
        /// Creates a new <see cref="AcAttachInfo"/>.
        /// </summary>
        public AcAttachInfo()
        {
            Entries = new List<AttachEntry>();
        }

        /// <summary>
        /// Reads from a stream.
        /// </summary>
        /// <param name="br">The stream reader.</param>
        protected override void Read(BinaryReaderEx br)
        {
            br.BigEndian = true;
            int entryCount = br.ReadInt32();
            br.AssertInt32(0);
            br.AssertInt32(0);
            br.AssertInt32(0);

            int namesOffset = 16 + entryCount * 16;
            Entries = new List<AttachEntry>(entryCount);
            for (int i = 0; i < entryCount; i++)
            {
                Entries.Add(new AttachEntry(br, namesOffset));
            }
        }

        /// <summary>
        /// Writes to a stream.
        /// </summary>
        /// <param name="bw">The stream writer.</param>
        protected override void Write(BinaryWriterEx bw)
        {
            bw.BigEndian = true;
            bw.WriteInt32(Entries.Count);
            bw.WriteInt32(0);
            bw.WriteInt32(0);
            bw.WriteInt32(0);

            for (int i = 0; i < Entries.Count; i++)
            {
                Entries[i].Write(bw, i);
            }

            for (int i = 0; i < Entries.Count; i++)
            {
                bw.FillInt32($"BoneNameOffset_{i}", (int)bw.Position);
                bw.WriteASCII(Entries[i].BoneName, true);
            }
        }

        /// <summary>
        /// An attach entry.
        /// </summary>
        public class AttachEntry
        {
            /// <summary>
            /// The category of the part the <see cref="AttachID"/> is present on.
            /// </summary>
            public AcPartCategory SourceCategory { get; set; }

            /// <summary>
            /// The attach ID.
            /// </summary>
            public byte AttachID { get; set; }

            /// <summary>
            /// The category of the part to attach.
            /// </summary>
            public AcPartCategory DestinationCategory { get; set; }

            /// <summary>
            /// Unknown; Seen as 0, 1, 2, 3, 4 and 5.
            /// </summary>
            public byte Unk03 { get; set; }

            /// <summary>
            /// Unknown; Seen as 0 or 1.
            /// </summary>
            public byte Unk0C { get; set; }

            /// <summary>
            /// The name of the bone to attach.
            /// </summary>
            public string BoneName { get; set; }

            /// <summary>
            /// Creates a new <see cref="AttachEntry"/>.
            /// </summary>
            public AttachEntry()
            {
                SourceCategory = AcPartCategory.Core;
                AttachID = 4;
                DestinationCategory = AcPartCategory.Head;
                Unk03 = 0;
                Unk0C = 0;
                BoneName = "core";
            }

            /// <summary>
            /// Reads from a stream.
            /// </summary>
            /// <param name="br">The stream reader.</param>
            /// <param name="namesOffset">The offset where names begin.</param>
            internal AttachEntry(BinaryReaderEx br, int namesOffset)
            {
                SourceCategory = br.ReadEnum8<AcPartCategory>();
                AttachID = br.ReadByte();
                DestinationCategory = br.ReadEnum8<AcPartCategory>();
                Unk03 = br.ReadByte();
                int nameOffset = br.ReadInt32();
                br.AssertInt32(0);
                Unk0C = br.ReadByte();
                br.AssertByte(0);
                br.AssertByte(0);
                br.AssertByte(0);

                BoneName = br.GetASCII(namesOffset + nameOffset);
            }

            /// <summary>
            /// Writes to a stream.
            /// </summary>
            /// <param name="bw">The stream writer.</param>
            /// <param name="index">The index of the entry.</param>
            internal void Write(BinaryWriterEx bw, int index)
            {
                bw.WriteByte((byte)SourceCategory);
                bw.WriteByte(AttachID);
                bw.WriteByte((byte)DestinationCategory);
                bw.WriteByte(Unk03);
                bw.ReserveInt32($"BoneNameOffset_{index}");
                bw.WriteInt32(0);
                bw.WriteByte(Unk0C);
                bw.WriteByte(0);
                bw.WriteByte(0);
                bw.WriteByte(0);
            }
        }
    }
}
