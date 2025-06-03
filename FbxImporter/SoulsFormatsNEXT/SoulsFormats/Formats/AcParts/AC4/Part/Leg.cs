namespace SoulsFormats
{
    public partial class AcParts4
    {
        /// <summary>
        /// A Leg part in an ACPARTS file.
        /// </summary>
        public class Leg : IPart, IFrame
        {
            /// <summary>
            /// The leg type affecting legs in different ways such as part category blacklisting.
            /// </summary>
            public enum LegType : byte
            {
                /// <summary>
                /// Bipedal leg type.
                /// </summary>
                Bipedal = 0,

                /// <summary>
                /// Reverse Joint leg type.
                /// </summary>
                ReverseJoint = 1,

                /// <summary>
                /// Quad leg type meaning four legs.
                /// </summary>
                Quad = 2,

                /// <summary>
                /// Tank leg type meaning tank legs.
                /// </summary>
                Tank = 3
            }

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
            /// The chosen leg type affecting legs in unknown ways such as part category blacklisting.
            /// </summary>
            public LegType Type { get; set; }

            /// <summary>
            /// The selected motion/animation for these legs;
            /// 0 = Biped Middle;
            /// 1 = Reverse Joint;
            /// 2 = Quad;
            /// 3 = Tank;
            /// 4 = Biped Light;
            /// 5 = Biped Heavy;
            /// 6 = Biped Light Alternate (Only used on Leg Lahire)
            /// </summary>
            public byte Motion { get; set; }

            /// <summary>
            /// Unknown; Is always 0; May be Core Turn Range.
            /// </summary>
            public ushort Unk1E { get; set; }

            /// <summary>
            /// The max amount of weight this Leg can hold;
            /// Going past this value turns the weight gauge red and will not allow exit from the Assemble menu.
            /// </summary>
            public ushort MaxLoad { get; set; }

            /// <summary>
            /// The amount of weight the leg units can bear. 
            /// The larger the value, the greater the carrying capacity.
            /// </summary>
            public ushort Load { get; set; }

            /// <summary>
            /// The value the tune slider will set the mentioned stat to at the end of the slider, 50 FRS points in.
            /// </summary>
            public ushort TuneMaxLoad { get; set; }

            /// <summary>
            /// Whether or not putting FRS points into tuning the stat will change it or not.
            /// </summary>
            public ushort TuneEfficiencyLoad { get; set; }

            /// <summary>
            /// Raises back unit angle.
            /// Used on quad legs and tank legs to make sure back units do not clip.
            /// Causes camera to break at extremely high values.
            /// </summary>
            public int BackUnitAngle { get; set; }

            /// <summary>
            /// How fast these legs walk.
            /// </summary>
            public int MovementAbility { get; set; }

            /// <summary>
            /// Indicates turning ability of the AC.
            /// Larger values allow the AC to turn more quickly.
            /// Negative values reverse turning controls.
            /// </summary>
            public int TurningAbility { get; set; }

            /// <summary>
            /// How fast these legs are able to halt movement after landing and stopping.
            /// </summary>
            public int BrakingAbility { get; set; }

            /// <summary>
            /// How high these legs' initial jump speed is.
            /// </summary>
            public int JumpingAbility { get; set; }

            /// <summary>
            /// Indicates the landing stability of the AC.
            /// The larger the value, the more stable the AC.
            /// </summary>
            public int LandingStability { get; set; }

            /// <summary>
            /// Indicates the stability of the AC when hit.
            /// </summary>
            public int HitStability { get; set; }

            /// <summary>
            /// Indicates the stability of the AC when shooting.
            /// </summary>
            public int ShootStability { get; set; }

            /// <summary>
            /// The value the tune slider will set the mentioned stat to at the end of the slider, 50 FRS points in.
            /// </summary>
            public int TuneMaxMovementAbility { get; set; }

            /// <summary>
            /// The value the tune slider will set the mentioned stat to at the end of the slider, 50 FRS points in.
            /// </summary>
            public int TuneMaxTurningAbility { get; set; }

            /// <summary>
            /// The value the tune slider will set the mentioned stat to at the end of the slider, 50 FRS points in.
            /// </summary>
            public int TuneMaxBrakingAbility { get; set; }

            /// <summary>
            /// The value the tune slider will set the mentioned stat to at the end of the slider, 50 FRS points in.
            /// </summary>
            public int TuneMaxJumpingAbility { get; set; }

            /// <summary>
            /// The value the tune slider will set the mentioned stat to at the end of the slider, 50 FRS points in.
            /// </summary>
            public int TuneMaxLandingStability { get; set; }

            /// <summary>
            /// Assumed to be Unk40 Tune Target from position.
            /// The value the tune slider will set the mentioned stat to at the end of the slider, 50 FRS points in.
            /// </summary>
            public int TuneMaxHitStability { get; set; }

            /// <summary>
            /// The value the tune slider will set the mentioned stat to at the end of the slider, 50 FRS points in.
            /// </summary>
            public int TuneMaxShootStability { get; set; }

            /// <summary>
            /// Whether or not putting FRS points into tuning the stat will change it or not.
            /// </summary>
            public ushort TuneEfficiencyMovementAbility { get; set; }

            /// <summary>
            /// Whether or not putting FRS points into tuning the stat will change it or not.
            /// </summary>
            public ushort TuneEfficiencyTurningAbility { get; set; }

            /// <summary>
            /// Whether or not putting FRS points into tuning the stat will change it or not.
            /// </summary>
            public ushort TuneEfficiencyBrakingAbility { get; set; }

            /// <summary>
            /// Whether or not putting FRS points into tuning the stat will change it or not.
            /// </summary>
            public ushort TuneEfficiencyJumpingAbility { get; set; }

            /// <summary>
            /// Whether or not putting FRS points into tuning the stat will change it or not.
            /// </summary>
            public ushort TuneEfficiencyLandingStability { get; set; }

            /// <summary>
            /// Whether or not putting FRS points into tuning the stat will change it or not.
            /// </summary>
            public ushort TuneEfficiencyHitStability { get; set; }

            /// <summary>
            /// Whether or not putting FRS points into tuning the stat will change it or not.
            /// </summary>
            public ushort TuneEfficiencyShootStability { get; set; }

            /// <summary>
            /// Unknown; Is always 0.
            /// </summary>
            public ushort Unk70 { get; set; }

            /// <summary>
            /// Horizontal Booster stats.
            /// </summary>
            public BoosterComponent HorizontalBoost { get; set; }

            /// <summary>
            /// Vertical Booster stats.
            /// </summary>
            public BoosterComponent VerticalBoost { get; set; }

            /// <summary>
            /// Quick Booster stats.
            /// </summary>
            public BoosterComponent QuickBoost { get; set; }

            /// <summary>
            /// Unknown; ACFA only; May be a short instead; Is always 0 on normal legs, 36 on energy tank legs, and 40 on tank legs.
            /// </summary>
            public byte UnkA4 { get; set; }

            /// <summary>
            /// Unknown; ACFA only; May be a short instead; Is always 0.
            /// </summary>
            public byte UnkA5 { get; set; }

            /// <summary>
            /// Unknown; ACFA only; Is always 0.
            /// </summary>
            public short UnkA6 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public short StabilizerBackX { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public short StabilizerBackY { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public short StabilizerUpX { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public short StabilizerUpY { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public short StabilizerMidX { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public short StabilizerMidY { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public short StabilizerLowX { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public short StabilizerLowY { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public short StabilizerUpRightX { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public short StabilizerUpRightY { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public short StabilizerMidRightX { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public short StabilizerMidRightY { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public short StabilizerLowRightX { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public short StabilizerLowRightY { get; set; }

            /// <summary>
            /// Makes a new <see cref="Leg"/>.
            /// </summary>
            public Leg()
            {
                PartComponent = new PartComponent();
                PartComponent.Category = PartComponent.PartCategory.Legs;
                DefenseComponent = new DefenseComponent();
                PAComponent = new PAComponent();
                FrameComponent = new FrameComponent();
                HorizontalBoost = new BoosterComponent();
                VerticalBoost = new BoosterComponent();
                QuickBoost = new BoosterComponent();
            }

            /// <summary>
            /// Reads a Leg part from a stream.
            /// </summary>
            /// <param name="br">A binary reader.</param>
            /// <param name="version">The version indicating which 4thgen game's AcParts is being read.</param>
            internal Leg(BinaryReaderEx br, AcParts4Version version)
            {
                PartComponent = new PartComponent(br, version);
                DefenseComponent = new DefenseComponent(br);
                PAComponent = new PAComponent(br);
                FrameComponent = new FrameComponent(br);
                Type = br.ReadEnum8<LegType>();
                Motion = br.ReadByte();
                Unk1E = br.ReadUInt16();
                MaxLoad = br.ReadUInt16();
                Load = br.ReadUInt16();
                TuneMaxLoad = br.ReadUInt16();
                TuneEfficiencyLoad = br.ReadUInt16();
                BackUnitAngle = br.ReadInt32();
                MovementAbility = br.ReadInt32();
                TurningAbility = br.ReadInt32();
                BrakingAbility = br.ReadInt32();
                JumpingAbility = br.ReadInt32();
                LandingStability = br.ReadInt32();
                HitStability = br.ReadInt32();
                ShootStability = br.ReadInt32();
                TuneMaxMovementAbility = br.ReadInt32();
                TuneMaxTurningAbility = br.ReadInt32();
                TuneMaxBrakingAbility = br.ReadInt32();
                TuneMaxJumpingAbility = br.ReadInt32();
                TuneMaxLandingStability = br.ReadInt32();
                TuneMaxHitStability = br.ReadInt32();
                TuneMaxShootStability = br.ReadInt32();
                TuneEfficiencyMovementAbility = br.ReadUInt16();
                TuneEfficiencyTurningAbility = br.ReadUInt16();
                TuneEfficiencyBrakingAbility = br.ReadUInt16();
                TuneEfficiencyJumpingAbility = br.ReadUInt16();
                TuneEfficiencyLandingStability = br.ReadUInt16();
                TuneEfficiencyHitStability = br.ReadUInt16();
                TuneEfficiencyShootStability = br.ReadUInt16();
                Unk70 = br.ReadUInt16();
                HorizontalBoost = new BoosterComponent(br);
                VerticalBoost = new BoosterComponent(br);
                QuickBoost = new BoosterComponent(br);

                if (version == AcParts4Version.ACFA)
                {
                    UnkA4 = br.ReadByte();
                    UnkA5 = br.ReadByte();
                    UnkA6 = br.ReadInt16();
                }

                StabilizerBackX = br.ReadInt16();
                StabilizerBackY = br.ReadInt16();
                StabilizerUpX = br.ReadInt16();
                StabilizerUpY = br.ReadInt16();
                StabilizerMidX = br.ReadInt16();
                StabilizerMidY = br.ReadInt16();
                StabilizerLowX = br.ReadInt16();
                StabilizerLowY = br.ReadInt16();
                StabilizerUpRightX = br.ReadInt16();
                StabilizerUpRightY = br.ReadInt16();
                StabilizerMidRightX = br.ReadInt16();
                StabilizerMidRightY = br.ReadInt16();
                StabilizerLowRightX = br.ReadInt16();
                StabilizerLowRightY = br.ReadInt16();
            }

            /// <summary>
            /// Writes a Leg part to a stream.
            /// </summary>
            /// <param name="bw">A binary writer.</param>
            /// <param name="version">The version indicating which 4thgen game's AcParts is being written.</param>
            public void Write(BinaryWriterEx bw, AcParts4Version version)
            {
                PartComponent.Write(bw, version);
                DefenseComponent.Write(bw);
                PAComponent.Write(bw);
                FrameComponent.Write(bw);
                bw.WriteByte((byte)Type);
                bw.WriteByte(Motion);
                bw.WriteUInt16(Unk1E);
                bw.WriteUInt16(MaxLoad);
                bw.WriteUInt16(Load);
                bw.WriteUInt16(TuneMaxLoad);
                bw.WriteUInt16(TuneEfficiencyLoad);
                bw.WriteInt32(BackUnitAngle);
                bw.WriteInt32(MovementAbility);
                bw.WriteInt32(TurningAbility);
                bw.WriteInt32(BrakingAbility);
                bw.WriteInt32(JumpingAbility);
                bw.WriteInt32(LandingStability);
                bw.WriteInt32(HitStability);
                bw.WriteInt32(ShootStability);
                bw.WriteInt32(TuneMaxMovementAbility);
                bw.WriteInt32(TuneMaxTurningAbility);
                bw.WriteInt32(TuneMaxBrakingAbility);
                bw.WriteInt32(TuneMaxJumpingAbility);
                bw.WriteInt32(TuneMaxLandingStability);
                bw.WriteInt32(TuneMaxHitStability);
                bw.WriteInt32(TuneMaxShootStability);
                bw.WriteUInt16(TuneEfficiencyMovementAbility);
                bw.WriteUInt16(TuneEfficiencyTurningAbility);
                bw.WriteUInt16(TuneEfficiencyBrakingAbility);
                bw.WriteUInt16(TuneEfficiencyJumpingAbility);
                bw.WriteUInt16(TuneEfficiencyLandingStability);
                bw.WriteUInt16(TuneEfficiencyHitStability);
                bw.WriteUInt16(TuneEfficiencyShootStability);
                bw.WriteUInt16(Unk70);
                HorizontalBoost.Write(bw);
                VerticalBoost.Write(bw);
                QuickBoost.Write(bw);

                if (version == AcParts4Version.ACFA)
                {
                    bw.WriteByte(UnkA4);
                    bw.WriteByte(UnkA5);
                    bw.WriteInt16(UnkA6);
                }

                bw.WriteInt16(StabilizerBackX);
                bw.WriteInt16(StabilizerBackY);
                bw.WriteInt16(StabilizerUpX);
                bw.WriteInt16(StabilizerUpY);
                bw.WriteInt16(StabilizerMidX);
                bw.WriteInt16(StabilizerMidY);
                bw.WriteInt16(StabilizerLowX);
                bw.WriteInt16(StabilizerLowY);
                bw.WriteInt16(StabilizerUpRightX);
                bw.WriteInt16(StabilizerUpRightY);
                bw.WriteInt16(StabilizerMidRightX);
                bw.WriteInt16(StabilizerMidRightY);
                bw.WriteInt16(StabilizerLowRightX);
                bw.WriteInt16(StabilizerLowRightY);
            }

            /// <inheritdoc/>
            public override string ToString()
            {
                return PartComponent.ToString();
            }
        }
    }
}

