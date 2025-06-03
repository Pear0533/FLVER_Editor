namespace SoulsFormats
{
    public partial class AcParts4
    {
        /// <summary>
        /// A Core part in an ACPARTS file.
        /// </summary>
        public class Core : IPart, IFrame
        {
            /// <summary>
            /// A Component which contains common stats across all parts.
            /// </summary>
            public PartComponent PartComponent { get; set; }

            /// <summary>
            /// A Component which contains Defense stats.
            /// </summary>
            public DefenseComponent DefenseComponent { get; set; }

            /// <summary>
            /// A Component which contains Primal Armor stats.
            /// </summary>
            public PAComponent PAComponent { get; set; }

            /// <summary>
            /// A Component which contains frame part stats.
            /// </summary>
            public FrameComponent FrameComponent { get; set; }

            /// <summary>
            /// Unknown; AC4 only so far; Found using AC4 core.txt in the only spot it could be in assuming core.txt is correct.
            /// </summary>
            public uint HungerUnit { get; set; }

            /// <summary>
            /// Unknown; AC4 only so far; Is always 0.
            /// </summary>
            // Motion?
            public uint Unk20 { get; set; }

            /// <summary>
            /// Unknown; ACFA only.
            /// </summary>
            public ushort Unk1C { get; set; }

            /// <summary>
            /// Unknown; ACFA only; Assumed to be Unk1C's TuneMax from positioning and value.
            /// The value the tune slider will set the mentioned stat to at the end of the slider, 50 FRS points in.
            /// </summary>
            public ushort TuneMaxUnk1C { get; set; }

            /// <summary>
            /// Unknown; ACFA only; Assumed to be Unk1C's TuneEfficiency from positioning and value.
            /// Whether or not putting FRS points into tuning the stat will change it or not.
            /// </summary>
            public ushort TuneEfficiencyUnk1C { get; set; }

            /// <summary>
            /// ACFA only. Indicates the stability of the AC.
            /// The larger the value, the more stable the AC.
            /// </summary>
            public ushort Stability { get; set; }

            /// <summary>
            /// ACFA only.
            /// The value the tune slider will set the mentioned stat to at the end of the slider, 50 FRS points in.
            /// </summary>
            public ushort TuneMaxStability { get; set; }

            /// <summary>
            /// ACFA only.
            /// Whether or not putting FRS points into tuning the stat will change it or not.
            /// </summary>
            public ushort TuneEfficiencyStability { get; set; }

            /// <summary>
            /// Unknown; Something to do with stabilizers on the upper x axis.
            /// </summary>
            public short StabilizerUpX { get; set; }

            /// <summary>
            /// Unknown; Something to do with stabilizers on the upper y axis.
            /// </summary>
            public short StabilizerUpY { get; set; }

            /// <summary>
            /// Unknown; Something to do with stabilizers on the lower x axis.
            /// </summary>
            public short StabilizerLowX { get; set; }

            /// <summary>
            /// Unknown; Something to do with stabilizers on the lower y axis.
            /// </summary>
            public short StabilizerLowY { get; set; }

            /// <summary>
            /// Makes a new <see cref="Core"/>.
            /// </summary>
            public Core()
            {
                PartComponent = new PartComponent();
                PartComponent.Category = PartComponent.PartCategory.Core;
                DefenseComponent = new DefenseComponent();
                PAComponent = new PAComponent();
                FrameComponent = new FrameComponent();
            }

            /// <summary>
            /// Reads a Core part from a stream.
            /// </summary>
            /// <param name="br">A binary reader.</param>
            /// <param name="version">The version indicating which 4thgen game's AcParts is being read.</param>
            internal Core(BinaryReaderEx br, AcParts4Version version)
            {
                PartComponent = new PartComponent(br, version);
                DefenseComponent = new DefenseComponent(br);
                PAComponent = new PAComponent(br);
                FrameComponent = new FrameComponent(br);

                if (version == AcParts4Version.AC4)
                {
                    HungerUnit = br.ReadUInt32();
                    Unk20 = br.ReadUInt32();
                }
                else if (version == AcParts4Version.ACFA)
                {
                    Unk1C = br.ReadUInt16();
                    TuneMaxUnk1C = br.ReadUInt16();
                    TuneEfficiencyUnk1C = br.ReadUInt16();
                    Stability = br.ReadUInt16();
                    TuneMaxStability = br.ReadUInt16();
                    TuneEfficiencyStability = br.ReadUInt16();
                }

                StabilizerUpX = br.ReadInt16();
                StabilizerUpY = br.ReadInt16();
                StabilizerLowX = br.ReadInt16();
                StabilizerLowY = br.ReadInt16();
            }

            /// <summary>
            /// Writes a Core part to a stream.
            /// </summary>
            /// <param name="bw">A binary writer.</param>
            /// <param name="version">The version indicating which 4thgen game's AcParts is being written.</param>
            public void Write(BinaryWriterEx bw, AcParts4Version version)
            {
                PartComponent.Write(bw, version);
                DefenseComponent.Write(bw);
                PAComponent.Write(bw);
                FrameComponent.Write(bw);

                if (version == AcParts4Version.AC4)
                {
                    bw.WriteUInt32(HungerUnit);
                    bw.WriteUInt32(Unk20);
                }
                else if (version == AcParts4Version.ACFA)
                {
                    bw.WriteUInt16(Unk1C);
                    bw.WriteUInt16(TuneMaxUnk1C);
                    bw.WriteUInt16(TuneEfficiencyUnk1C);
                    bw.WriteUInt16(Stability);
                    bw.WriteUInt16(TuneMaxStability);
                    bw.WriteUInt16(TuneEfficiencyStability);
                }

                bw.WriteInt16(StabilizerUpX);
                bw.WriteInt16(StabilizerUpY);
                bw.WriteInt16(StabilizerLowX);
                bw.WriteInt16(StabilizerLowY);
            }

            /// <inheritdoc/>
            public override string ToString()
            {
                return PartComponent.ToString();
            }
        }
    }
}
