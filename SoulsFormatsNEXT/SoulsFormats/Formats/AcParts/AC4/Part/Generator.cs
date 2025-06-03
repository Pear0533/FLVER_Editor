namespace SoulsFormats
{
    public partial class AcParts4
    {
        /// <summary>
        /// A Generator part in an ACPARTS file.
        /// </summary>
        public class Generator : IPart
        {
            /// <summary>
            /// A Component which contains common stats across all parts.
            /// </summary>
            public PartComponent PartComponent { get; set; }

            /// <summary>
            /// The max amount of energy this Generator can hold.
            /// </summary>
            public int EnergyCapacity { get; set; }

            /// <summary>
            /// The value the tune slider will set the mentioned stat to at the end of the slider, 50 FRS points in.
            /// The slider will have the increments inbetween calculated automatically.
            /// The value can be lower or higher than the current stat value if needed, lower means the slider will lower the stat.
            /// </summary>
            public int TuneMaxEnergyCapacity { get; set; }

            /// <summary>
            /// Whether or not putting FRS points into tuning the stat will change it or not.
            /// </summary>
            public ushort TuneEfficiencyEnergyCapacity { get; set; }

            /// <summary>
            /// Unknown; Is always 0.
            /// </summary>
            public ushort Unk0A { get; set; }

            /// <summary>
            /// The overuse limit for energy output.
            /// If total Energy Cost exceeds this number the Energy Cost gauge will turn orange,
            /// and energy output performance will be affected.
            /// </summary>
            public int EnergyOutputSoftLimit { get; set; }

            /// <summary>
            /// How fast this Generator outputs energy.
            /// </summary>
            public int EnergyOutput { get; set; }

            /// <summary>
            /// The value the tune slider will set the mentioned stat to at the end of the slider, 50 FRS points in.
            /// The slider will have the increments inbetween calculated automatically.
            /// The value can be lower or higher than the current stat value if needed, lower means the slider will lower the stat.
            /// </summary>
            public int TuneMaxEnergyOutput { get; set; }

            /// <summary>
            /// Whether or not putting FRS points into tuning the stat will change it or not.
            /// </summary>
            public ushort TuneEfficiencyEnergyOutput { get; set; }

            /// <summary>
            /// How fast this Generator outputs Kojima Particles to recharge Primal Armor.
            /// </summary>
            public ushort KPOutput { get; set; }

            /// <summary>
            /// The value the tune slider will set the mentioned stat to at the end of the slider, 50 FRS points in.
            /// The slider will have the increments inbetween calculated automatically.
            /// The value can be lower or higher than the current stat value if needed, lower means the slider will lower the stat.
            /// </summary>
            public ushort TuneMaxKPOutput { get; set; }

            /// <summary>
            /// Whether or not putting FRS points into tuning the stat will change it or not.
            /// </summary>
            public ushort TuneEfficiencyKPOutput { get; set; }

            /// <summary>
            /// Unknown; Active Sound Effect Possibly?
            /// </summary>
            public ushort ActiveSE { get; set; }

            /// <summary>
            /// Unknown; Is always 0.
            /// </summary>
            public ushort Unk22 { get; set; }

            /// <summary>
            /// Makes a new <see cref="Generator"/>.
            /// </summary>
            public Generator()
            {
                PartComponent = new PartComponent();
                PartComponent.Category = PartComponent.PartCategory.Generator;
            }

            /// <summary>
            /// Reads a Generator part from a stream.
            /// </summary>
            /// <param name="br">A binary reader.</param>
            /// <param name="version">The version indicating which 4thgen game's AcParts is being read.</param>
            internal Generator(BinaryReaderEx br, AcParts4Version version)
            {
                PartComponent = new PartComponent(br, version);
                EnergyCapacity = br.ReadInt32();
                TuneMaxEnergyCapacity = br.ReadInt32();
                TuneEfficiencyEnergyCapacity = br.ReadUInt16();
                Unk0A = br.ReadUInt16();
                EnergyOutputSoftLimit = br.ReadInt32();
                EnergyOutput = br.ReadInt32();
                TuneMaxEnergyOutput = br.ReadInt32();
                TuneEfficiencyEnergyOutput = br.ReadUInt16();
                KPOutput = br.ReadUInt16();
                TuneMaxKPOutput = br.ReadUInt16();
                TuneEfficiencyKPOutput = br.ReadUInt16();
                ActiveSE = br.ReadUInt16();
                Unk22 = br.ReadUInt16();
            }

            /// <summary>
            /// Writes a Generator part to a stream.
            /// </summary>
            /// <param name="bw">A binary writer.</param>
            /// <param name="version">The version indicating which 4thgen game's AcParts is being written.</param>
            public void Write(BinaryWriterEx bw, AcParts4Version version)
            {
                PartComponent.Write(bw, version);
                bw.WriteInt32(EnergyCapacity);
                bw.WriteInt32(TuneMaxEnergyCapacity);
                bw.WriteUInt16(TuneEfficiencyEnergyCapacity);
                bw.WriteUInt16(Unk0A);
                bw.WriteInt32(EnergyOutputSoftLimit);
                bw.WriteInt32(EnergyOutput);
                bw.WriteInt32(TuneMaxEnergyOutput);
                bw.WriteUInt16(TuneEfficiencyEnergyOutput);
                bw.WriteUInt16(KPOutput);
                bw.WriteUInt16(TuneMaxKPOutput);
                bw.WriteUInt16(TuneEfficiencyKPOutput);
                bw.WriteUInt16(ActiveSE);
                bw.WriteUInt16(Unk22);
            }

            /// <inheritdoc/>
            public override string ToString()
            {
                return PartComponent.ToString();
            }
        }
    }
}
