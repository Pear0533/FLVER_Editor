namespace SoulsFormats
{
    public partial class AcParts4
    {
        /// <summary>
        /// A Shoulder Unit part in an ACPARTS file.
        /// </summary>
        public class ShoulderUnit : IPart, IWeapon
        {
            /// <summary>
            /// The shoulder type determining how the shoulder unit is fired in various ways.
            /// </summary>
            public enum ShoulderType : byte
            {
                /// <summary>
                /// The shoulder unit only fires along with certain back units.
                /// </summary>
                LinkWeapon = 0,

                /// <summary>
                /// A normal, firing, gun.
                /// </summary>
                ManualWeapon = 1,

                /// <summary>
                /// Special Device, as in Kojima device, used on PA Rechargers.
                /// </summary>
                SpecialDevice = 2,

                /// <summary>
                /// Used on the PA molder.
                /// </summary>
                PaMolder = 3,

                /// <summary>
                /// The shoulder unit is an add side booster.
                /// </summary>
                AddBooster = 4
            }

            /// <summary>
            /// A Component which contains common stats across all parts.
            /// </summary>
            public PartComponent PartComponent { get; set; }

            /// <summary>
            /// The shoulder type determining how the shoulder unit is fired in various ways.
            /// </summary>
            public ShoulderType Type { get; set; }

            /// <summary>
            /// Whether or not the shoulder unit is a weapon.
            /// </summary>
            public bool IsWeapon { get; set; }

            /// <summary>
            /// Changes what stat descriptions are used.<br/>
            /// Believed to be used as an index for stat descriptions in the AssemMenu file.<br/>
            /// The <see cref="DispType"/> enum can be used to get the name of each index in the vanilla game.
            /// </summary>
            public byte DisplayType { get; set; }

            /// <summary>
            /// Unknown; Is always 0.
            /// </summary>
            public byte Unk03 { get; set; }

            /// <summary>
            /// A Component which contains Primal Armor stats.
            /// </summary>
            public PAComponent PAComponent { get; set; }

            /// <summary>
            /// Identifies whether or not a shoulder unit is a stealth shoulder unit usually; Purpose unknown.
            /// </summary>
            public string DeviceName { get; set; }

            /// <summary>
            /// The amount of times this shoulder unit can be used; Similar to magazine capacity.
            /// </summary>
            public ushort UseCount { get; set; }

            /// <summary>
            /// How long non-projectile generating Shoulder Unit effects last.
            /// Also called EffectiveFrame.
            /// </summary>
            public ushort EffectDuration { get; set; }

            /// <summary>
            /// How fast non-projectile generating Shoulder Units fire.
            /// </summary>
            public ushort ReloadFrame { get; set; }

            /// <summary>
            /// Unknown; Is always 0.
            /// </summary>
            public ushort Unk1E { get; set; }

            /// <summary>
            /// Unknown; In the same position as AAAttackBoost; AC4 only.
            /// </summary>
            public float EffectParam_0 { get; set; }

            /// <summary>
            /// Unknown; In the same position as AARangeBoost; AC4 only.
            /// </summary>
            public float EffectParam_1 { get; set; }

            /// <summary>
            /// How much attack power is added to Assault Armor while this Shoulder Unit is equipped; ACFA only.
            /// </summary>
            public float AAAttackPower { get; set; }

            /// <summary>
            /// How much range is added to Assault Armor while this Shoulder Unit is equipped; ACFA only.
            /// </summary>
            public float AARangeBoost { get; set; }

            /// <summary>
            /// A Component which contains stats for weapons.
            /// </summary>
            public WeaponComponent WeaponComponent { get; set; }

            /// <summary>
            /// A Component which contains Booster stats for Weapons; ACFA only.
            /// </summary>
            public WeaponBoosterComponent WeaponBoosterComponent { get; set; }

            /// <summary>
            /// Makes a new <see cref="ShoulderUnit"/>.
            /// </summary>
            public ShoulderUnit()
            {
                PartComponent = new PartComponent();
                PartComponent.Category = PartComponent.PartCategory.ShoulderUnit;
                PAComponent = new PAComponent();
                DeviceName = string.Empty;
                WeaponComponent = new WeaponComponent();
                WeaponBoosterComponent = new WeaponBoosterComponent();
            }

            /// <summary>
            /// Reads a Shoulder Unit part from a stream.
            /// </summary>
            /// <param name="br">A binary reader.</param>
            /// <param name="version">The version indicating which 4thgen game's AcParts is being read.</param>
            internal ShoulderUnit(BinaryReaderEx br, AcParts4Version version)
            {
                PartComponent = new PartComponent(br, version);
                Type = br.ReadEnum8<ShoulderType>();
                IsWeapon = br.ReadBoolean();
                DisplayType = br.ReadByte();
                Unk03 = br.ReadByte();
                PAComponent = new PAComponent(br);
                DeviceName = br.ReadFixStr(0x10);
                UseCount = br.ReadUInt16();
                EffectDuration = br.ReadUInt16();
                ReloadFrame = br.ReadUInt16();
                Unk1E = br.ReadUInt16();

                if (version == AcParts4Version.AC4)
                {
                    EffectParam_0 = br.ReadSingle();
                    EffectParam_1 = br.ReadSingle();
                }
                else if (version == AcParts4Version.ACFA)
                {
                    AAAttackPower = br.ReadSingle();
                    AARangeBoost = br.ReadSingle();
                }

                WeaponComponent = new WeaponComponent(br);

                if (version == AcParts4Version.ACFA)
                {
                    WeaponBoosterComponent = new WeaponBoosterComponent(br);
                }
            }

            /// <summary>
            /// Writes a Shoulder Unit part to a stream.
            /// </summary>
            /// <param name="bw">A binary writer.</param>
            /// <param name="version">The version indicating which 4thgen game's AcParts is being written.</param>
            public void Write(BinaryWriterEx bw, AcParts4Version version)
            {
                PartComponent.Write(bw, version);
                bw.WriteByte((byte)Type);
                bw.WriteBoolean(IsWeapon);
                bw.WriteByte(DisplayType);
                bw.WriteByte(Unk03);
                PAComponent.Write(bw);
                bw.WriteFixStr(DeviceName, 0x10, 0x20);
                bw.WriteUInt16(UseCount);
                bw.WriteUInt16(EffectDuration);
                bw.WriteUInt16(ReloadFrame);
                bw.WriteUInt16(Unk1E);

                if (version == AcParts4Version.AC4)
                {
                    bw.WriteSingle(EffectParam_0);
                    bw.WriteSingle(EffectParam_1);
                }
                else if (version == AcParts4Version.ACFA)
                {
                    bw.WriteSingle(AAAttackPower);
                    bw.WriteSingle(AARangeBoost);
                }

                WeaponComponent.Write(bw);

                if (version == AcParts4Version.ACFA)
                {
                    WeaponBoosterComponent.Write(bw);
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
