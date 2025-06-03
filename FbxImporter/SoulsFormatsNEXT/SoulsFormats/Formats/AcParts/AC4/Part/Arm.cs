namespace SoulsFormats
{
    public partial class AcParts4
    {
        /// <summary>
        /// An Arm part in an ACPARTS file.
        /// </summary>
        public class Arm : IPart, IFrame, IWeapon
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
            /// Whether or not this Arm is a Weapon Arm and to use Weapon Arm Stats;
            /// Equipping Arm Units or having Hangar Units will be disabled upon enabling this.
            /// </summary>
            public bool IsWeaponArm { get; set; }

            /// <summary>
            /// Unknown; Is always 0; May be motion, or animation ID for arms.
            /// </summary>
            public byte Unk1D { get; set; }

            /// <summary>
            /// Reduces loss of maneuverability due to weapon weight 
            /// and suppresses weapon recoil when firing.
            /// </summary>
            public ushort FiringStability { get; set; }

            /// <summary>
            /// Boosts attack power of energy weapons.
            /// A larger value indicates stronger energy weapon attack power.
            /// </summary>
            public ushort EnergyWeaponSkill { get; set; }

            /// <summary>
            /// Enables rapid weapon movement and targeting.
            /// Larger values indicate the weapon acquires enemies quickly.
            /// </summary>
            public ushort WeaponManeuverability { get; set; }

            /// <summary>
            /// Indicates weapon accuracy.
            /// The larger the value, the more accurate the attack.
            /// </summary>
            public ushort AimPrecision { get; set; }

            /// <summary>
            /// Unknown; Is always 0 in AC4; Is always 100 or 75 in ACFA.
            /// May be a short instead.
            /// </summary>
            public byte Unk26 { get; set; }

            /// <summary>
            /// Unknown; Is always 0 in AC4; Is always 70 or 98 in ACFA.
            /// May be a short instead.
            /// </summary>
            public byte Unk27 { get; set; }

            /// <summary>
            /// Unknown; Is always 0.
            /// </summary>
            public ushort Unk28 { get; set; }

            /// <summary>
            /// Unknown; Is always 0.
            /// </summary>
            public ushort Unk2A { get; set; }

            /// <summary>
            /// The value the tune slider will set the mentioned stat to at the end of the slider, 50 FRS points in.
            /// </summary>
            public ushort TuneMaxFiringStability { get; set; }

            /// <summary>
            /// Whether or not putting FRS points into tuning the stat will change it or not.
            /// </summary>
            public ushort TuneEfficiencyFiringStability { get; set; }

            /// <summary>
            /// The value the tune slider will set the mentioned stat to at the end of the slider, 50 FRS points in.
            /// </summary>
            public ushort TuneMaxEnergyWeaponSkill { get; set; }

            /// <summary>
            /// Whether or not putting FRS points into tuning the stat will change it or not.
            /// </summary>
            public ushort TuneEfficiencyEnergyWeaponSkill { get; set; }

            /// <summary>
            /// The value the tune slider will set the mentioned stat to at the end of the slider, 50 FRS points in.
            /// </summary>
            public ushort TuneMaxWeaponManeuverability { get; set; }

            /// <summary>
            /// Whether or not putting FRS points into tuning the stat will change it or not.
            /// </summary>
            public ushort TuneEfficiencyWeaponManeuverability { get; set; }

            /// <summary>
            /// The value the tune slider will set the mentioned stat to at the end of the slider, 50 FRS points in.
            /// </summary>
            public ushort TuneMaxAimPrecision { get; set; }

            /// <summary>
            /// Whether or not putting FRS points into tuning the stat will change it or not.
            /// </summary>
            public ushort TuneEfficiencyAimPrecision { get; set; }

            /// <summary>
            /// Changes what stat descriptions are used for weapon arms.<br/>
            /// Believed to be used as an index for stat descriptions in the AssemMenu file.<br/>
            /// The <see cref="DispType"/> enum can be used to get the name of each index in the vanilla game.
            /// </summary>
            public byte DisplayType { get; set; }

            /// <summary>
            /// Unknown; Is always 0.
            /// </summary>
            public byte Unk3D { get; set; }

            /// <summary>
            /// Unknown; Is always 0.
            /// </summary>
            public byte Unk3E { get; set; }

            /// <summary>
            /// Unknown; Is always 0.
            /// </summary>
            public byte Unk3F { get; set; }

            /// <summary>
            /// The aim type of the weapon arm.
            /// </summary>
            public string AimType { get; set; }

            /// <summary>
            /// The Weapon Arm Stats for this Arm.
            /// </summary>
            public WeaponComponent WeaponComponent { get; set; }

            /// <summary>
            /// Unknown; Something to do with stabilizers on the x axis.
            /// </summary>
            public short StabilizerX { get; set; }

            /// <summary>
            /// Unknown; Something to do with stabilizers on the y axis.
            /// </summary>
            public short StabilizerY { get; set; }

            /// <summary>
            /// Makes a new <see cref="Arm"/>.
            /// </summary>
            public Arm()
            {
                PartComponent = new PartComponent();
                PartComponent.Category = PartComponent.PartCategory.Arms;
                DefenseComponent = new DefenseComponent();
                PAComponent = new PAComponent();
                FrameComponent = new FrameComponent();
                AimType = string.Empty;
                WeaponComponent = new WeaponComponent();
            }

            /// <summary>
            /// Reads an Arm part from a stream.
            /// </summary>
            /// <param name="br">A binary reader.</param>
            /// <param name="version">The version indicating which 4thgen game's AcParts is being read.</param>
            internal Arm(BinaryReaderEx br, AcParts4Version version)
            {
                PartComponent = new PartComponent(br, version);
                DefenseComponent = new DefenseComponent(br);
                PAComponent = new PAComponent(br);
                FrameComponent = new FrameComponent(br);
                IsWeaponArm = br.ReadBoolean();
                Unk1D = br.ReadByte();
                FiringStability = br.ReadUInt16();
                EnergyWeaponSkill = br.ReadUInt16();
                WeaponManeuverability = br.ReadUInt16();
                AimPrecision = br.ReadUInt16();
                Unk26 = br.ReadByte();
                Unk27 = br.ReadByte();
                Unk28 = br.ReadUInt16();
                Unk2A = br.ReadUInt16();
                TuneMaxFiringStability = br.ReadUInt16();
                TuneEfficiencyFiringStability = br.ReadUInt16();
                TuneMaxEnergyWeaponSkill = br.ReadUInt16();
                TuneEfficiencyEnergyWeaponSkill = br.ReadUInt16();
                TuneMaxWeaponManeuverability = br.ReadUInt16();
                TuneEfficiencyWeaponManeuverability = br.ReadUInt16();
                TuneMaxAimPrecision = br.ReadUInt16();
                TuneEfficiencyAimPrecision = br.ReadUInt16();
                DisplayType = br.ReadByte();
                Unk3D = br.ReadByte();
                Unk3E = br.ReadByte();
                Unk3F = br.ReadByte();
                AimType = br.ReadFixStr(0x10);
                WeaponComponent = new WeaponComponent(br);
                StabilizerX = br.ReadInt16();
                StabilizerY = br.ReadInt16();
            }

            /// <summary>
            /// Writes an Arm part to a stream.
            /// </summary>
            /// <param name="bw">A binary writer.</param>
            /// <param name="version">The version indicating which 4thgen game's AcParts is being written.</param>
            public void Write(BinaryWriterEx bw, AcParts4Version version)
            {
                PartComponent.Write(bw, version);
                DefenseComponent.Write(bw);
                PAComponent.Write(bw);
                FrameComponent.Write(bw);
                bw.WriteBoolean(IsWeaponArm);
                bw.WriteByte(Unk1D);
                bw.WriteUInt16(FiringStability);
                bw.WriteUInt16(EnergyWeaponSkill);
                bw.WriteUInt16(WeaponManeuverability);
                bw.WriteUInt16(AimPrecision);
                bw.WriteByte(Unk26);
                bw.WriteByte(Unk27);
                bw.WriteUInt16(Unk28);
                bw.WriteUInt16(Unk2A);
                bw.WriteUInt16(TuneMaxFiringStability);
                bw.WriteUInt16(TuneEfficiencyFiringStability);
                bw.WriteUInt16(TuneMaxEnergyWeaponSkill);
                bw.WriteUInt16(TuneEfficiencyEnergyWeaponSkill);
                bw.WriteUInt16(TuneMaxWeaponManeuverability);
                bw.WriteUInt16(TuneEfficiencyWeaponManeuverability);
                bw.WriteUInt16(TuneMaxAimPrecision);
                bw.WriteUInt16(TuneEfficiencyAimPrecision);
                bw.WriteByte(DisplayType);
                bw.WriteByte(Unk3D);
                bw.WriteByte(Unk3E);
                bw.WriteByte(Unk3F);
                bw.WriteFixStr(AimType, 0x10, 0x20);
                WeaponComponent.Write(bw);
                bw.WriteInt16(StabilizerX);
                bw.WriteInt16(StabilizerY);
            }

            /// <inheritdoc/>
            public override string ToString()
            {
                return PartComponent.ToString();
            }
        }
    }
}
