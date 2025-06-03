namespace SoulsFormats
{
    public partial class AcParts4
    {
        /// <summary>
        /// An FCS part in an ACPARTS file.
        /// </summary>
        public class FCS : IPart
        {
            /// <summary>
            /// Deflect Type; Purpose unknown.
            /// </summary>
            public enum DeflectType : byte
            {
                /// <summary>
                /// Unknown; None in ACFA, for some reason in fcs.txt mentioned as Detailed in AC4.
                /// </summary>
                None = 0,

                /// <summary>
                /// Unknown.
                /// </summary>
                Rough = 1,

                /// <summary>
                /// Unknown; Detailed in ACFA.
                /// </summary>
                Detailed = 2
            }

            /// <summary>
            /// A Component which contains common stats across all parts.
            /// </summary>
            public PartComponent PartComponent { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public DeflectType Deflect { get; set; }

            /// <summary>
            /// The maximum number of targets this FCS can lock onto at once.
            /// </summary>
            public byte LockTargetMax { get; set; }

            /// <summary>
            /// The minimum distance laser blades start to home in on targets from.
            /// </summary>
            public ushort BladeLockDistance { get; set; }

            /// <summary>
            /// Ability to use two weapons in tandem.
            /// Reduces lock-on degradation when firing two weapons simultaneously.
            /// </summary>
            public ushort ParallelProcessing { get; set; }

            /// <summary>
            /// Unknown; Found in fcs.txt for EnemyParts.bin.
            /// </summary>
            public ushort Visibility { get; set; }

            /// <summary>
            /// The maximum distance at which this FCS can still lock onto a target in view.
            /// </summary>
            public ushort LockDistance { get; set; }

            /// <summary>
            /// Appears to be the height of a hidden FCS lockbox.
            /// </summary>
            public ushort LockBoxHeight { get; set; }

            /// <summary>
            /// Appears to be the width of a hidden FCS lockbox.
            /// </summary>
            public ushort LockBoxWidth { get; set; }

            /// <summary>
            /// Unknown; Part of the Lock Range/Lock Box stat found in Fcs.txt for EnemyParts.bin.
            /// Has same value as LockBoxWidth on player FCS but can be different as seen in EnemyParts.bin.
            /// </summary>
            public ushort UnkLockRange4 { get; set; }

            /// <summary>
            /// 6000 divided by this value rounded is the lock speed of this FCS.
            /// </summary>
            public ushort SecondLockTime { get; set; }

            /// <summary>
            /// How fast Weapons with Missiles can lock on using this FCS.
            /// </summary>
            public ushort MissileLockSpeed { get; set; }

            /// <summary>
            /// Unknown; Assumed from positioning and value; Might be whether or not the FCS has multi-lock.
            /// </summary>
            public bool MultiLock { get; set; }

            /// <summary>
            /// Unknown; Is always 0.
            /// </summary>
            public byte Unk15 { get; set; }

            /// <summary>
            /// Unknown; Is always 0; Might be a part of Zoom Range as an int.
            /// </summary>
            public ushort Unk16 { get; set; }

            /// <summary>
            /// Zoom range; Purpose unknown.
            /// </summary>
            public ushort ZoomRange { get; set; }

            /// <summary>
            /// A Component which contains Radar stats.
            /// </summary>
            public RadarComponent RadarComponent { get; set; }

            /// <summary>
            /// Tuning relating to lock speed, see SecondLockTime.
            /// The value the tune slider will set the mentioned stat to at the end of the slider, 50 FRS points in.
            /// </summary>
            public ushort TuneMaxSecondLockTime { get; set; }

            /// <summary>
            /// Tuning relating to lock speed, see SecondLockTime.
            /// Whether or not putting FRS points into tuning the stat will change it or not.
            /// </summary>
            public ushort TuneEfficiencySecondLockTime { get; set; }

            /// <summary>
            /// The value the tune slider will set the mentioned stat to at the end of the slider, 50 FRS points in.
            /// </summary>
            public ushort TuneMaxMissileLockSpeed { get; set; }

            /// <summary>
            /// Whether or not putting FRS points into tuning the stat will change it or not.
            /// </summary>
            public ushort TuneEfficiencyMissileLockSpeed { get; set; }

            /// <summary>
            /// Makes a new <see cref="FCS"/>.
            /// </summary>
            public FCS()
            {
                PartComponent = new PartComponent();
                PartComponent.Category = PartComponent.PartCategory.FCS;
                RadarComponent = new RadarComponent();
            }

            /// <summary>
            /// Reads an FCS part from a stream.
            /// </summary>
            /// <param name="br">A binary reader.</param>
            /// <param name="version">The version indicating which 4thgen game's AcParts is being read.</param>
            internal FCS(BinaryReaderEx br, AcParts4Version version)
            {
                PartComponent = new PartComponent(br, version);
                Deflect = br.ReadEnum8<DeflectType>();
                LockTargetMax = br.ReadByte();
                BladeLockDistance = br.ReadUInt16();
                ParallelProcessing = br.ReadUInt16();
                Visibility = br.ReadUInt16();
                LockDistance = br.ReadUInt16();
                LockBoxHeight = br.ReadUInt16();
                LockBoxWidth = br.ReadUInt16();
                UnkLockRange4 = br.ReadUInt16();
                SecondLockTime = br.ReadUInt16();
                MissileLockSpeed = br.ReadUInt16();
                MultiLock = br.ReadBoolean();
                Unk15 = br.ReadByte();
                Unk16 = br.ReadUInt16();
                ZoomRange = br.ReadUInt16();
                RadarComponent = new RadarComponent(br);
                TuneMaxSecondLockTime = br.ReadUInt16();
                TuneEfficiencySecondLockTime = br.ReadUInt16();
                TuneMaxMissileLockSpeed = br.ReadUInt16();
                TuneEfficiencyMissileLockSpeed = br.ReadUInt16();
            }

            /// <summary>
            /// Writes an FCS part to a stream.
            /// </summary>
            /// <param name="bw">A binary writer.</param>
            /// <param name="version">The version indicating which 4thgen game's AcParts is being written.</param>
            public void Write(BinaryWriterEx bw, AcParts4Version version)
            {
                PartComponent.Write(bw, version);
                bw.WriteByte((byte)Deflect);
                bw.WriteByte(LockTargetMax);
                bw.WriteUInt16(BladeLockDistance);
                bw.WriteUInt16(ParallelProcessing);
                bw.WriteUInt16(Visibility);
                bw.WriteUInt16(LockDistance);
                bw.WriteUInt16(LockBoxHeight);
                bw.WriteUInt16(LockBoxWidth);
                bw.WriteUInt16(UnkLockRange4);
                bw.WriteUInt16(SecondLockTime);
                bw.WriteUInt16(MissileLockSpeed);
                bw.WriteBoolean(MultiLock);
                bw.WriteByte(Unk15);
                bw.WriteUInt16(Unk16);
                bw.WriteUInt16(ZoomRange);
                RadarComponent.Write(bw);
                bw.WriteUInt16(TuneMaxSecondLockTime);
                bw.WriteUInt16(TuneEfficiencySecondLockTime);
                bw.WriteUInt16(TuneMaxMissileLockSpeed);
                bw.WriteUInt16(TuneEfficiencyMissileLockSpeed);
            }

            /// <inheritdoc/>
            public override string ToString()
            {
                return PartComponent.ToString();
            }
        }
    }
}
