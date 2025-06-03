namespace SoulsFormats
{
    public partial class AcParts4
    {
        /// <summary>
        /// A Component which contains Booster stats.
        /// </summary>
        public class BoosterComponent
        {
            /// <summary>
            /// Capacity for thrust using standard boost.
            /// A larger value allows for faster movement in the specified direction.
            /// </summary>
            public uint Thrust { get; set; }

            /// <summary>
            /// The value the tune slider will set the mentioned stat to at the end of the slider, 50 FRS points in.
            /// </summary>
            public uint TuneMaxThrust { get; set; }

            /// <summary>
            /// Whether or not putting FRS points into tuning the stat will change it or not.
            /// </summary>
            public ushort TuneEfficiencyThrust { get; set; }

            /// <summary>
            /// Only usable on Quick Booster stats.
            /// Length of time quick boost can be engaged.
            /// The larger this value, the further the AC can move during
            /// quick boost, but overall energy consumption will increase.
            /// </summary>
            public ushort QuickBoostDuration { get; set; }

            /// <summary>
            /// Energy consumed when engaging thrust.
            /// Larger values indicate a greater EN cost requirement for use.
            /// </summary>
            public uint ThrustENCost { get; set; }

            /// <summary>
            /// Makes a new <see cref="BoosterComponent"/>.
            /// </summary>
            public BoosterComponent()
            {
                
            }

            /// <summary>
            /// Reads a Quick Booster component from a stream.
            /// </summary>
            /// <param name="br">A binary reader.</param>
            internal BoosterComponent(BinaryReaderEx br)
            {
                Thrust = br.ReadUInt32();
                TuneMaxThrust = br.ReadUInt32();
                TuneEfficiencyThrust = br.ReadUInt16();
                QuickBoostDuration = br.ReadUInt16();
                ThrustENCost = br.ReadUInt32();
            }

            /// <summary>
            /// Writes a Quick Booster component to a stream.
            /// </summary>
            /// <param name="bw">A binary writer.</param>
            public void Write(BinaryWriterEx bw)
            {
                bw.WriteUInt32(Thrust);
                bw.WriteUInt32(TuneMaxThrust);
                bw.WriteUInt16(TuneEfficiencyThrust);
                bw.WriteUInt16(QuickBoostDuration);
                bw.WriteUInt32(ThrustENCost);
            }
        }
    }
}
