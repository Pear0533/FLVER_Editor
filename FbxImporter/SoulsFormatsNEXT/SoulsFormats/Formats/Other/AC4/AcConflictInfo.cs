using System.Collections.Generic;

namespace SoulsFormats.Other.AC4
{
    /// <summary>
    /// Details about what parts are not allowed to be equipped at the same time in Armored Core For Answer.
    /// </summary>
    public class AcConflictInfo : SoulsFile<AcConflictInfo>
    {
        /// <summary>
        /// The conflict entries.
        /// </summary>
        public List<ConflictEntry> Entries { get; set; }

        /// <summary>
        /// Create a new <see cref="AcConflictInfo"/>.
        /// </summary>
        public AcConflictInfo()
        {
            Entries = new List<ConflictEntry>();
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

            Entries = new List<ConflictEntry>(entryCount);
            for (int i = 0; i < entryCount; i++)
            {
                Entries.Add(new ConflictEntry(br));
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
                Entries[i].Write(bw);
            }
        }

        /// <summary>
        /// A conflict entry.
        /// </summary>
        public class ConflictEntry
        {
            /// <summary>
            /// The part category.
            /// </summary>
            public AcPartCategory Category { get; set; }

            /// <summary>
            /// The blocked part category.
            /// </summary>
            public AcPartCategory BlockedCategory { get; set; }

            /// <summary>
            /// The part ID.
            /// </summary>
            public ushort PartID { get; set; }

            /// <summary>
            /// The blocked part ID.
            /// </summary>
            public ushort BlockedPartID { get; set; }

            /// <summary>
            /// Create a new <see cref="ConflictEntry"/>.
            /// </summary>
            public ConflictEntry()
            {
                Category = AcPartCategory.Head;
                BlockedCategory = AcPartCategory.HeadTop;
                PartID = 10;
                BlockedPartID = 5;
            }

            /// <summary>
            /// Reads from a stream.
            /// </summary>
            /// <param name="br">The stream reader.</param>
            internal ConflictEntry(BinaryReaderEx br)
            {
                Category = br.ReadEnum8<AcPartCategory>();
                BlockedCategory = br.ReadEnum8<AcPartCategory>();
                PartID = br.ReadUInt16();
                BlockedPartID = br.ReadUInt16();
                br.AssertUInt16(0);
            }

            /// <summary>
            /// Writes to a stream.
            /// </summary>
            /// <param name="bw">The stream writer.</param>
            internal void Write(BinaryWriterEx bw)
            {
                bw.WriteByte((byte)Category);
                bw.WriteByte((byte)BlockedCategory);
                bw.WriteUInt16(PartID);
                bw.WriteUInt16(BlockedPartID);
                bw.WriteUInt16(0);
            }
        }
    }
}
