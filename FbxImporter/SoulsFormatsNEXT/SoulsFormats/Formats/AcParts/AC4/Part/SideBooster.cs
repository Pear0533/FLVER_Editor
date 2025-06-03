namespace SoulsFormats
{
    public partial class AcParts4
    {
        /// <summary>
        /// A Side Booster part in an ACPARTS file.
        /// </summary>
        public class SideBooster : IPart, IBooster
        {
            /// <summary>
            /// A Component which contains common stats across all parts.
            /// </summary>
            public PartComponent PartComponent { get; set; }

            /// <summary>
            /// Horizontal Booster stats.
            /// </summary>
            public BoosterComponent HorizontalBoost { get; set; }

            /// <summary>
            /// Quick Booster Stats.
            /// </summary>
            public BoosterComponent QuickBoost { get; set; }

            /// <summary>
            /// ACFA only. After using quick boost, indicates the amount of time before it becomes available again.
            /// </summary>
            public byte QuickReloadTime { get; set; }

            /// <summary>
            /// Unknown; Is always 0; ACFA only.
            /// </summary>
            public byte Unk31 { get; set; }

            /// <summary>
            /// Unknown; Is always 0; ACFA only.
            /// </summary>
            public ushort Unk32 { get; set; }

            /// <summary>
            /// Makes a new <see cref="SideBooster"/>.
            /// </summary>
            public SideBooster()
            {
                PartComponent = new PartComponent();
                PartComponent.Category = PartComponent.PartCategory.SideBooster;
                HorizontalBoost = new BoosterComponent();
                QuickBoost = new BoosterComponent();
            }

            /// <summary>
            /// Reads a Side Booster part from a stream.
            /// </summary>
            /// <param name="br">A binary reader.</param>
            /// <param name="version">The version indicating which 4thgen game's AcParts is being read.</param>
            internal SideBooster(BinaryReaderEx br, AcParts4Version version)
            {
                PartComponent = new PartComponent(br, version);
                HorizontalBoost = new BoosterComponent(br);
                QuickBoost = new BoosterComponent(br);

                if (version == AcParts4Version.ACFA)
                {
                    QuickReloadTime = br.ReadByte();
                    Unk31 = br.ReadByte();
                    Unk32 = br.ReadUInt16();
                }
            }

            /// <summary>
            /// Writes a Side Booster part to a stream.
            /// </summary>
            /// <param name="bw">A binary writer.</param>
            /// <param name="version">The version indicating which 4thgen game's AcParts is being written.</param>
            public void Write(BinaryWriterEx bw, AcParts4Version version)
            {
                PartComponent.Write(bw, version);
                HorizontalBoost.Write(bw);
                QuickBoost.Write(bw);

                if (version == AcParts4Version.ACFA)
                {
                    bw.WriteByte(QuickReloadTime);
                    bw.WriteByte(Unk31);
                    bw.WriteUInt16(Unk32);
                }
            }

            /// <inheritdoc/>
            public override string ToString()
            {
                return PartComponent.ToString();
            }
        }
    }
}
