namespace SoulsFormats
{
    public partial class AcParts4
    {
        /// <summary>
        /// An Overed Booster part in an ACPARTS file.
        /// </summary>
        public class OveredBooster : IPart, IBooster
        {
            /// <summary>
            /// A Component which contains common stats across all parts.
            /// </summary>
            public PartComponent PartComponent { get; set; }

            /// <summary>
            /// Overed Booster stats.
            /// </summary>
            public BoosterComponent HorizontalBoost { get; set; }

            /// <summary>
            /// Kojima Particle (Primal Armor Gauge) consumed when Overed Boost is engaged.
            /// Larger values indicate a greater Kojima Particle cost requirement for use.
            /// </summary>
            public ushort OveredBoostKPCost { get; set; }

            /// <summary>
            /// Unknown; Is always 0.
            /// </summary>
            public ushort Unk12 { get; set; }

            /// <summary>
            /// The energy cost of using overboost during activation of overboost.
            /// </summary>
            public uint PrepareENCost { get; set; }

            /// <summary>
            /// The kojima particle cost of using overboost during activation of overboost.
            /// </summary>
            public uint PrepareKPCost { get; set; }

            /// <summary>
            /// Thrust power during OB activation.
            /// The larger the value, the more powerful the Overed Boost activation thrust.
            /// </summary>
            public uint OBActivationThrust { get; set; }

            /// <summary>
            /// Energy consumed upon activating Overed Boost.
            /// Larger values indicate a greater EN cost requirement for use.
            /// </summary>
            public uint OBActivationENCost { get; set; }

            /// <summary>
            /// Kojima Particle (Primal Armor Gauge) consumed upon activating Overed Boost.
            /// Larger values indicate a greater Kojima Particle cost requirement for use.
            /// </summary>
            public uint OBActivationKPCost { get; set; }

            /// <summary>
            /// Amount of time Overed Boost activation thrust is enabled.
            /// The larger the value, the longer the thrust, but the more EN and KP consumed.
            /// </summary>
            public uint OBActivationLimit { get; set; }

            /// <summary>
            /// Unknown; might be an SFX ID for overboost charging in AC4.
            /// </summary>
            public ushort SFXOverboostCharge { get; set; }

            /// <summary>
            /// Unknown; might be an SFX ID for overboost launching in AC4.
            /// </summary>
            public ushort SFXOverboostLaunch { get; set; }

            /// <summary>
            /// Unknown; Is always 0.
            /// </summary>
            public uint Unk2C { get; set; }

            /// <summary>
            /// Attack power of Assault Armor.
            /// Considered energy weaponry.
            /// </summary>
            public ushort AssaultArmorAttackPower { get; set; }

            /// <summary>
            /// Effective range of Assault Armor.
            /// </summary>
            public ushort AssaultArmorRange { get; set; }

            /// <summary>
            /// Unknown; Is always 0.
            /// </summary>
            public uint Unk34 { get; set; }

            /// <summary>
            /// Unknown; Is always 0.
            /// </summary>
            public uint Unk38 { get; set; }

            /// <summary>
            /// Unknown; Is always 0.
            /// </summary>
            public uint Unk3C { get; set; }

            /// <summary>
            /// Makes a new <see cref="OveredBooster"/>.
            /// </summary>
            public OveredBooster()
            {
                PartComponent = new PartComponent();
                PartComponent.Category = PartComponent.PartCategory.OveredBooster;
                HorizontalBoost = new BoosterComponent();
            }

            /// <summary>
            /// Reads an Overed Booster part from a stream.
            /// </summary>
            /// <param name="br">A binary reader.</param>
            /// <param name="version">The version indicating which 4thgen game's AcParts is being read.</param>
            internal OveredBooster(BinaryReaderEx br, AcParts4Version version)
            {
                PartComponent = new PartComponent(br, version);
                HorizontalBoost = new BoosterComponent(br);
                OveredBoostKPCost = br.ReadUInt16();
                Unk12 = br.ReadUInt16();
                PrepareENCost = br.ReadUInt32();
                PrepareKPCost = br.ReadUInt32();
                OBActivationThrust = br.ReadUInt32();
                OBActivationENCost = br.ReadUInt32();
                OBActivationKPCost = br.ReadUInt32();
                OBActivationLimit = br.ReadUInt32();

                if (version == AcParts4Version.AC4)
                {
                    SFXOverboostCharge = br.ReadUInt16();
                    SFXOverboostLaunch = br.ReadUInt16();
                }
                else if (version == AcParts4Version.ACFA)
                {
                    Unk2C = br.ReadUInt32();
                    AssaultArmorAttackPower = br.ReadUInt16();
                    AssaultArmorRange = br.ReadUInt16();
                    Unk34 = br.ReadUInt32();
                    Unk38 = br.ReadUInt32();
                    Unk3C = br.ReadUInt32();
                }
            }

            /// <summary>
            /// Writes an Overed Booster part to a stream.
            /// </summary>
            /// <param name="bw">A binary writer.</param>
            /// <param name="version">The version indicating which 4thgen game's AcParts is being written.</param>
            public void Write(BinaryWriterEx bw, AcParts4Version version)
            {
                PartComponent.Write(bw, version);
                HorizontalBoost.Write(bw);
                bw.WriteUInt16(OveredBoostKPCost);
                bw.WriteUInt16(Unk12);
                bw.WriteUInt32(PrepareENCost);
                bw.WriteUInt32(PrepareKPCost);
                bw.WriteUInt32(OBActivationThrust);
                bw.WriteUInt32(OBActivationENCost);
                bw.WriteUInt32(OBActivationKPCost);
                bw.WriteUInt32(OBActivationLimit);

                if (version == AcParts4Version.AC4)
                {
                    bw.WriteUInt16(SFXOverboostCharge);
                    bw.WriteUInt16(SFXOverboostLaunch);
                }
                else if (version == AcParts4Version.ACFA)
                {
                    bw.WriteUInt32(Unk2C);
                    bw.WriteUInt16(AssaultArmorAttackPower);
                    bw.WriteUInt16(AssaultArmorRange);
                    bw.WriteUInt32(Unk34);
                    bw.WriteUInt32(Unk38);
                    bw.WriteUInt32(Unk3C);
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
