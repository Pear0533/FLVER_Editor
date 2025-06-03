namespace SoulsFormats
{
    public partial class AcParts4
    {
        /// <summary>
        /// A Component which contains common stats across all parts.
        /// </summary>
        public class PartComponent
        {
            /// <summary>
            /// The category of part this part is.
            /// </summary>
            public enum PartCategory : byte
            {
                /// <summary>
                /// A head part.
                /// </summary>
                Head = 0,

                /// <summary>
                /// A core, or torso part.
                /// </summary>
                Core = 1,

                /// <summary>
                /// An arms part.
                /// </summary>
                Arms = 2,

                /// <summary>
                /// A legs part.
                /// </summary>
                Legs = 3,

                /// <summary>
                /// A Fire Control System part for locking on.
                /// </summary>
                FCS = 4,

                /// <summary>
                /// A Generator part for generating energy.
                /// </summary>
                Generator = 5,

                /// <summary>
                /// A Main Booster part for forward movement.
                /// </summary>
                MainBooster = 6,

                /// <summary>
                /// A Back Booster part for backwards movement.
                /// </summary>
                BackBooster = 7,

                /// <summary>
                /// A Side Booster part for side-to-side movement.
                /// </summary>
                SideBooster = 8,

                /// <summary>
                /// A Overed Booster part for over boosting in forward movement.
                /// </summary>
                OveredBooster = 9,

                /// <summary>
                /// An Arm Unit part, for hand-held weapons.
                /// </summary>
                ArmUnit = 10,

                /// <summary>
                /// A Back Unit part for over-the-shoulder weapons and other misc parts.
                /// </summary>
                BackUnit = 11,

                /// <summary>
                /// A Shoulder Unit part for weapons or other misc parts on the sides of Shoulders.
                /// </summary>
                ShoulderUnit = 12,

                /// <summary>
                /// A Stabilizer on the top of Heads.
                /// </summary>
                HeadTopStabilizer = 13,

                /// <summary>
                /// Stabilizers on the sides of Heads.
                /// </summary>
                HeadSideStabilizer = 14,

                /// <summary>
                /// Stabilizers on the upper sides of Cores.
                /// </summary>
                CoreUpperSideStabilizer = 15,

                /// <summary>
                /// Stabilizers on the lower sides of Cores.
                /// </summary>
                CoreLowerSideStabilizer = 16,

                /// <summary>
                /// Stabilizers on the sides of Arms.
                /// </summary>
                ArmStabilizer = 17,

                /// <summary>
                /// Stabilizers on the back of a set of Legs.
                /// </summary>
                LegBackStabilizer = 18,

                /// <summary>
                /// Stabilizers on the upper sides of Legs.
                /// </summary>
                LegUpperStabilizer = 19,

                /// <summary>
                /// Stabilizers on the middle sides of Legs.
                /// </summary>
                LegMiddleStabilizer = 20,

                /// <summary>
                /// Stabilizers on the lower sides of Legs.
                /// </summary>
                LegLowerStabilizer = 21
            }

            /// <summary>
            /// The ID for this part, used to identify it in many places.
            /// FMG description is also decided using this.
            /// </summary>
            public ushort PartID { get; set; }

            /// <summary>
            /// The ID of the model used by this part, formatted with four digits.
            /// </summary>
            public ushort ModelID { get; set; }

            /// <summary>
            /// The price of this part in the shop.
            /// </summary>
            public int Price { get; set; }

            /// <summary>
            /// The part's weight.
            /// Heavier parts will cause your AC to move more slowly.
            /// </summary>
            public ushort Weight { get; set; }

            /// <summary>
            /// The energy cost of this part
            /// </summary>
            public ushort ENCost { get; set; }

            /// <summary>
            /// The category/type of part this part is.
            /// </summary>
            public PartCategory Category { get; set; }

            /// <summary>
            /// Unknown; Found in AC4 files.
            /// </summary>
            public byte InitStatus { get; set; }

            /// <summary>
            /// Unknown; An ID indentifying which cap file should be used on weapons.
            /// </summary>
            public ushort CapID { get; set; }

            /// <summary>
            /// The Name of this part.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// The name of the company who makes of this part.
            /// This also determines which company icon gets shown.
            /// </summary>
            public string MakerName { get; set; }

            /// <summary>
            /// A description describing weight group, part type, or other misc things.
            /// </summary>
            public string SubCategory { get; set; }

            /// <summary>
            /// Some kind of subcategory ID; Purpose Unknown; ACFA only.
            /// </summary>
            public ushort SubCategoryID { get; set; }

            /// <summary>
            /// Some kind of ID identifying sets of parts; Purpose Unknown; ACFA only.
            /// </summary>
            public ushort SetID { get; set; }

            /// <summary>
            /// An internal description describing this part, might be for development tools or might be a scraped idea that was supposed to load in descriptions.
            /// 252 bytes long in ACFA, 256 bytes long in AC4.
            /// </summary>
            public string Explain { get; set; }

            /// <summary>
            /// Makes a new <see cref="PartComponent"/>.
            /// </summary>
            public PartComponent()
            {
                Name = string.Empty;
                MakerName = string.Empty;
                SubCategory = string.Empty;
                Explain = string.Empty;
            }

            /// <summary>
            /// Reads a Part component from a stream.
            /// </summary>
            /// <param name="br">A binary reader.</param>
            /// <param name="version">The version indicating which 4thgen game's AcParts is being read.</param>
            internal PartComponent(BinaryReaderEx br, AcParts4Version version)
            {
                PartID = br.ReadUInt16();
                ModelID = br.ReadUInt16();
                Price = br.ReadInt32();
                Weight = br.ReadUInt16();
                ENCost = br.ReadUInt16();
                Category = br.ReadEnum8<PartCategory>();
                InitStatus = br.ReadByte();
                CapID = br.ReadUInt16();
                Name = br.ReadFixStr(0x20);
                MakerName = br.ReadFixStr(0x20);
                SubCategory = br.ReadFixStr(0x20);

                if (version == AcParts4Version.AC4)
                {
                    Explain = br.ReadFixStr(0x100);
                }
                else if (version == AcParts4Version.ACFA)
                {
                    SubCategoryID = br.ReadUInt16();
                    SetID = br.ReadUInt16();
                    Explain = br.ReadFixStr(0xFC);
                }
            }

            /// <summary>
            /// Writes a Part component to a stream.
            /// </summary>
            /// <param name="bw">A binary writer.</param>
            /// <param name="version">The version indicating which 4thgen game's AcParts is being written.</param>
            internal void Write(BinaryWriterEx bw, AcParts4Version version)
            {
                bw.WriteUInt16(PartID);
                bw.WriteUInt16(ModelID);
                bw.WriteInt32(Price);
                bw.WriteUInt16(Weight);
                bw.WriteUInt16(ENCost);
                bw.WriteByte((byte)Category);
                bw.WriteByte(InitStatus);
                bw.WriteUInt16(CapID);
                bw.WriteFixStr(Name, 0x20, 0x20);
                bw.WriteFixStr(MakerName, 0x20, 0x20);
                bw.WriteFixStr(SubCategory, 0x20, 0x20);

                if (version == AcParts4Version.AC4)
                {
                    bw.WriteFixStr(Explain, 0x100, 0x20);
                }
                else if (version == AcParts4Version.ACFA)
                {
                    bw.WriteUInt16(SubCategoryID);
                    bw.WriteUInt16(SetID);
                    bw.WriteFixStr(Explain, 0xFC, 0x20);
                }
            }

            /// <inheritdoc/>
            public override string ToString()
            {
                return Name;
            }
        }
    }
}
