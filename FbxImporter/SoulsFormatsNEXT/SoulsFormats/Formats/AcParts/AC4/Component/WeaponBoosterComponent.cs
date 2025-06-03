namespace SoulsFormats
{
    public partial class AcParts4
    {
        /// <summary>
        /// ACFA only. A Component which contains Booster stats for Weapons.
        /// </summary>
        public class WeaponBoosterComponent
        {
            /// <summary>
            /// Capacity for horizontal thrust using standard boost.
            /// A larger value allows for faster lateral movement.
            /// </summary>
            public uint HorizontalThrust { get; set; }

            /// <summary>
            /// Capacity for vertical thrust using standard boost.
            /// A larger value allows for faster vertical movement.
            /// </summary>
            public uint VerticalThrust { get; set; }

            /// <summary>
            /// Amount of thrust produced during quick boost.
            /// A larger value indicates faster quick boost movement.
            /// </summary>
            public uint QuickBoost { get; set; }

            /// <summary>
            /// Unknown; Likely is another type of Boost.
            /// </summary>
            public uint Unk0CThrust { get; set; }

            /// <summary>
            /// Energy consumed when engaging horizontal thrust.
            /// Larger values indicate a greater EN cost requirement for use.
            /// </summary>
            public uint HorizontalENCost { get; set; }

            /// <summary>
            /// Energy consumed when engaging vertical thrust.
            /// Larger values indicate a greater EN cost requirement for use.
            /// </summary>
            public uint VerticalENCost { get; set; }

            /// <summary>
            /// Energy consumed when engaging quick boost.
            /// Larger values indicate a greater EN cost requirement for use.
            /// </summary>
            public uint QuickBoostENCost { get; set; }

            /// <summary>
            /// Unknown; Likely is another type of Boost's EN cost.
            /// </summary>
            public uint Unk0CENCost { get; set; }

            /// <summary>
            /// Makes a new <see cref="WeaponBoosterComponent"/>.
            /// </summary>
            public WeaponBoosterComponent()
            {

            }

            /// <summary>
            /// Reads a Weapon Booster component from a stream.
            /// </summary>
            /// <param name="br">A binary reader.</param>
            internal WeaponBoosterComponent(BinaryReaderEx br)
            {
                HorizontalThrust = br.ReadUInt32();
                VerticalThrust = br.ReadUInt32();
                QuickBoost = br.ReadUInt32();
                Unk0CThrust = br.ReadUInt32();
                HorizontalENCost = br.ReadUInt32();
                VerticalENCost = br.ReadUInt32();
                QuickBoostENCost = br.ReadUInt32();
                Unk0CENCost = br.ReadUInt32();
            }

            /// <summary>
            /// Writes a Weapon Booster component to a stream.
            /// </summary>
            /// <param name="bw">A binary writer.</param>
            public void Write(BinaryWriterEx bw)
            {
                bw.WriteUInt32(HorizontalThrust);
                bw.WriteUInt32(VerticalThrust);
                bw.WriteUInt32(QuickBoost);
                bw.WriteUInt32(Unk0CThrust);
                bw.WriteUInt32(HorizontalENCost);
                bw.WriteUInt32(VerticalENCost);
                bw.WriteUInt32(QuickBoostENCost);
                bw.WriteUInt32(Unk0CENCost);
            }
        }
    }
}
