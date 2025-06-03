namespace SoulsFormats
{
    public partial class AcParts4
    {
        /// <summary>
        /// An Arm Unit part in an ACPARTS file.
        /// </summary>
        public class ArmUnit : IPart, IWeapon
        {
            /// <summary>
            /// The hanger requirements of an Arm Unit.
            /// </summary>
            public enum HangerType : byte
            {
                /// <summary>
                /// The Arm Unit cannot be placed in a hanger.
                /// </summary>
                NotHangerable = 0,

                /// <summary>
                /// The Arm Unit requires a tank hanger to be hangered.
                /// </summary>
                TankOnly = 1,

                /// <summary>
                /// The Arm Unit can be placed in any hanger size.
                /// </summary>
                Enable = 2,
            }

            /// <summary>
            /// A Component which contains common stats across all parts.
            /// </summary>
            public PartComponent PartComponent { get; set; }

            /// <summary>
            /// A Component which contains stats for weapons.
            /// </summary>
            public WeaponComponent WeaponComponent { get; set; }

            /// <summary>
            /// The hanger requirements of this Arm Unit.
            /// </summary>
            public HangerType HangerRequirement { get; set; }

            /// <summary>
            /// Changes what stat descriptions are used.<br/>
            /// Believed to be used as an index for stat descriptions in the AssemMenu file.<br/>
            /// The <see cref="DispType"/> enum can be used to get the name of each index in the vanilla game.
            /// </summary>
            public byte DisplayType { get; set; }

            /// <summary>
            /// Unknown; Is always 0.
            /// </summary>
            public ushort Unk56 { get; set; }

            /// <summary>
            /// Makes a new <see cref="ArmUnit"/>.
            /// </summary>
            public ArmUnit()
            {
                PartComponent = new PartComponent();
                PartComponent.Category = PartComponent.PartCategory.ArmUnit;
                WeaponComponent = new WeaponComponent();
            }

            /// <summary>
            /// Reads an Arm Unit part from a stream.
            /// </summary>
            /// <param name="br">A binary reader.</param>
            /// <param name="version">The version indicating which 4thgen game's AcParts is being read.</param>
            internal ArmUnit(BinaryReaderEx br, AcParts4Version version)
            {
                PartComponent = new PartComponent(br, version);
                WeaponComponent = new WeaponComponent(br);

                HangerRequirement = br.ReadEnum8<HangerType>();
                DisplayType = br.ReadByte();
                Unk56 = br.ReadUInt16();
            }

            /// <summary>
            /// Writes an Arm Unit part to a stream.
            /// </summary>
            /// <param name="bw">A binary writer.</param>
            /// <param name="version">The version indicating which 4thgen game's AcParts is being written.</param>
            public void Write(BinaryWriterEx bw, AcParts4Version version)
            {
                PartComponent.Write(bw, version);
                WeaponComponent.Write(bw);

                bw.WriteByte((byte)HangerRequirement);
                bw.WriteByte(DisplayType);
                bw.WriteUInt16(Unk56);
            }

            /// <inheritdoc/>
            public override string ToString()
            {
                return PartComponent.ToString();
            }
        }
    }
}
