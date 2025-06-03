namespace SoulsFormats
{
    public partial class AcParts4
    {
        /// <summary>
        /// A Component which contains Radar stats.
        /// </summary>
        public class RadarComponent
        {
            /// <summary>
            /// The effective range of enemy detection radar.
            /// Larger values allow you to detect more distant targets.
            /// </summary>
            public ushort RadarRange { get; set; }

            /// <summary>
            /// Radar's resistance to ECM interference.
            /// The larger this value, the more this radar can withstand ECM.
            /// </summary>
            public ushort ECMResistance { get; set; }

            /// <summary>
            /// The value the tune slider will set the mentioned stat to at the end of the slider, 50 FRS points in.
            /// </summary>
            public ushort TuneMaxECMResistance { get; set; }

            /// <summary>
            /// Whether or not putting FRS points into tuning the stat will change it or not.
            /// </summary>
            public ushort TuneEfficiencyECMResistance { get; set; }

            /// <summary>
            /// Increases refresh rate of the radar HUD.
            /// Larger values indicate enemy positions are refreshed more frequently.
            /// </summary>
            public ushort RadarRefreshRate { get; set; }

            /// <summary>
            /// The value the tune slider will set the mentioned stat to at the end of the slider, 50 FRS points in.
            /// </summary>
            public ushort TuneMaxRadarRefreshRate { get; set; }

            /// <summary>
            /// Whether or not putting FRS points into tuning the stat will change it or not.
            /// </summary>
            public ushort TuneEfficiencyRadarRefreshRate { get; set; }

            /// <summary>
            /// Makes a new <see cref="RadarComponent"/>.
            /// </summary>
            public RadarComponent()
            {

            }

            /// <summary>
            /// Reads a Radar component from a stream.
            /// </summary>
            /// <param name="br">A binary reader.</param>
            internal RadarComponent(BinaryReaderEx br)
            {
                RadarRange = br.ReadUInt16();
                ECMResistance = br.ReadUInt16();
                TuneMaxECMResistance = br.ReadUInt16();
                TuneEfficiencyECMResistance = br.ReadUInt16();
                RadarRefreshRate = br.ReadUInt16();
                TuneMaxRadarRefreshRate = br.ReadUInt16();
                TuneEfficiencyRadarRefreshRate = br.ReadUInt16();
            }

            /// <summary>
            /// Writes a Radar component to a stream.
            /// </summary>
            /// <param name="bw">A binary writer.</param>
            public void Write(BinaryWriterEx bw)
            {
                bw.WriteUInt16(RadarRange);
                bw.WriteUInt16(ECMResistance);
                bw.WriteUInt16(TuneMaxECMResistance);
                bw.WriteUInt16(TuneEfficiencyECMResistance);
                bw.WriteUInt16(RadarRefreshRate);
                bw.WriteUInt16(TuneMaxRadarRefreshRate);
                bw.WriteUInt16(TuneEfficiencyRadarRefreshRate);
            }
        }
    }
}
