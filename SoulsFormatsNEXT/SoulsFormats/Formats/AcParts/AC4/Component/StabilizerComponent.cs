namespace SoulsFormats
{
    public partial class AcParts4
    {
        /// <summary>
        /// A Component which contains Stabilizer stats.
        /// </summary>
        public class StabilizerComponent
        {
            /// <summary>
            /// The stabilizer category of this stabilizer, should be the same as part category in stabilizer part component.
            /// </summary>
            public byte Category { get; set; }

            /// <summary>
            /// Corrects the AC's center of gravity.
            /// The larger the value, the more the center of gravity will be corrected.
            /// </summary>
            public float ControlCalibration { get; set; }

            /// <summary>
            /// Makes a new <see cref="StabilizerComponent"/>.
            /// </summary>
            public StabilizerComponent()
            {

            }

            /// <summary>
            /// Reads a Stabilizer component from a stream.
            /// </summary>
            /// <param name="br">A binary reader.</param>
            internal StabilizerComponent(BinaryReaderEx br)
            {
                Category = br.ReadByte();
                br.AssertByte(0);
                br.AssertByte(0);
                br.AssertByte(0);
                ControlCalibration = br.ReadSingle();
            }

            /// <summary>
            /// Writes a Stabilizer component to a stream.
            /// </summary>
            /// <param name="bw">A binary writer.</param>
            public void Write(BinaryWriterEx bw)
            {
                bw.WriteByte(Category);
                bw.WriteByte(0);
                bw.WriteByte(0);
                bw.WriteByte(0);
                bw.WriteSingle(ControlCalibration);
            }
        }
    }
}
