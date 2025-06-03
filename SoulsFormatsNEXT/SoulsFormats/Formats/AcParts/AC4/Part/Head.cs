namespace SoulsFormats
{
    public partial class AcParts4
    {
        /// <summary>
        /// A Head part in an ACPARTS file.
        /// </summary>
        public class Head : IPart, IFrame
        {
            /// <summary>
            /// A Component which contains common stats across all parts.
            /// </summary>
            public PartComponent PartComponent { get; set; }

            /// <summary>
            /// A Component which contains Defense Stats.
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
            /// Indicates the stability of the AC.
            /// The larger the value, the more stable the AC.
            /// </summary>
            public ushort Stability { get; set; }

            /// <summary>
            /// Unknown; Something to do with SFX.
            /// </summary>
            public ushort SFXMonoeye { get; set; }

            /// <summary>
            /// The value the tune slider will set the mentioned stat to at the end of the slider, 50 FRS points in.
            /// </summary>
            public ushort TuneMaxStability { get; set; }

            /// <summary>
            /// Whether or not putting FRS points into tuning the stat will change it or not.
            /// </summary>
            public ushort TuneEfficiencyStability { get; set; }

            /// <summary>
            /// ACFA only.
            /// Functional effectiveness of the head-mounted camera eye. 
            /// Improves base FCS lock-on capability.
            /// </summary>
            public ushort CameraFunctionality { get; set; }

            /// <summary>
            /// ACFA only.
            /// Ability to recover from flash related interference.
            /// The larger this value, the faster the recovery.
            /// </summary>
            public ushort SystemRecovery { get; set; }

            /// <summary>
            /// Unknown; ACFA Only; Is always 0.
            /// </summary>
            public ushort Unk28 { get; set; }

            /// <summary>
            /// Unknown; ACFA Only; Is always 0.
            /// </summary>
            public ushort Unk2A { get; set; }

            /// <summary>
            /// Unknown; Something to do with stabilizers on the top x axis.
            /// </summary>
            public short StabilizerTopX { get; set; }

            /// <summary>
            /// Unknown; Something to do with stabilizers on the top y axis.
            /// </summary>
            public short StabilizerTopY { get; set; }

            /// <summary>
            /// Unknown; Something to do with stabilizers on the side x axis.
            /// </summary>
            public short StabilizerSideX { get; set; }

            /// <summary>
            /// Unknown; Something to do with stabilizers on the side y axis.
            /// </summary>
            public short StabilizerSideY { get; set; }

            /// <summary>
            /// Makes a new <see cref="Head"/>.
            /// </summary>
            public Head()
            {
                PartComponent = new PartComponent();
                PartComponent.Category = PartComponent.PartCategory.Head;
                DefenseComponent = new DefenseComponent();
                PAComponent = new PAComponent();
                FrameComponent = new FrameComponent();
            }

            /// <summary>
            /// Reads a Head part from a stream.
            /// </summary>
            /// <param name="br">A binary reader.</param>
            /// <param name="version">The version indicating which 4thgen game's AcParts is being read.</param>
            internal Head(BinaryReaderEx br, AcParts4Version version)
            {
                PartComponent = new PartComponent(br, version);
                DefenseComponent = new DefenseComponent(br);
                PAComponent = new PAComponent(br);
                FrameComponent = new FrameComponent(br);
                Stability = br.ReadUInt16();
                SFXMonoeye = br.ReadUInt16();
                TuneMaxStability = br.ReadUInt16();
                TuneEfficiencyStability = br.ReadUInt16();

                if (version == AcParts4Version.ACFA)
                {
                    CameraFunctionality = br.ReadUInt16();
                    SystemRecovery = br.ReadUInt16();
                    Unk28 = br.ReadUInt16();
                    Unk2A = br.ReadUInt16();
                }

                StabilizerTopX = br.ReadInt16();
                StabilizerTopY = br.ReadInt16();
                StabilizerSideX = br.ReadInt16();
                StabilizerSideY = br.ReadInt16();
            }

            /// <summary>
            /// Writes a Head part to a stream.
            /// </summary>
            /// <param name="bw">A binary writer.</param>
            /// <param name="version">The version indicating which 4thgen game's AcParts is being written.</param>
            public void Write(BinaryWriterEx bw, AcParts4Version version)
            {
                PartComponent.Write(bw, version);
                DefenseComponent.Write(bw);
                PAComponent.Write(bw);
                FrameComponent.Write(bw);
                bw.WriteUInt16(Stability);
                bw.WriteUInt16(SFXMonoeye);
                bw.WriteUInt16(TuneMaxStability);
                bw.WriteUInt16(TuneEfficiencyStability);

                if (version == AcParts4Version.ACFA)
                {
                    bw.WriteUInt16(CameraFunctionality);
                    bw.WriteUInt16(SystemRecovery);
                    bw.WriteUInt16(Unk28);
                    bw.WriteUInt16(Unk2A);
                }

                bw.WriteInt16(StabilizerTopX);
                bw.WriteInt16(StabilizerTopY);
                bw.WriteInt16(StabilizerSideX);
                bw.WriteInt16(StabilizerSideY);
            }

            /// <inheritdoc/>
            public override string ToString()
            {
                return PartComponent.ToString();
            }
        }
    }
}

