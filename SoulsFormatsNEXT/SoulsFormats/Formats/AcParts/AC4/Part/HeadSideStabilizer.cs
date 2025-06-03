namespace SoulsFormats
{
    public partial class AcParts4
    {
        /// <summary>
        /// A Stabilizer on the sides of Head parts in an ACPARTS file.
        /// </summary>
        public class HeadSideStabilizer : IPart, IStabilizer
        {
            /// <summary>
            /// A Component which contains common stats across all parts.
            /// </summary>
            public PartComponent PartComponent { get; set; }

            /// <summary>
            /// A Component which contains Stabilizer stats.
            /// </summary>
            public StabilizerComponent StabilizerComponent { get; set; }

            /// <summary>
            /// Makes a new <see cref="HeadSideStabilizer"/>.
            /// </summary>
            public HeadSideStabilizer()
            {
                PartComponent = new PartComponent();
                PartComponent.Category = PartComponent.PartCategory.HeadSideStabilizer;
                StabilizerComponent = new StabilizerComponent();
                StabilizerComponent.Category = (byte)PartComponent.Category;
            }

            /// <summary>
            /// Reads a Head Side Stabilizer part from a stream.
            /// </summary>
            /// <param name="br">A binary reader.</param>
            /// <param name="version">The version indicating which 4thgen game's AcParts is being read.</param>
            internal HeadSideStabilizer(BinaryReaderEx br, AcParts4Version version)
            {
                PartComponent = new PartComponent(br, version);
                StabilizerComponent = new StabilizerComponent(br);
            }

            /// <summary>
            /// Writes a Head Side Stabilizer part to a stream.
            /// </summary>
            /// <param name="bw">A binary writer.</param>
            /// <param name="version">The version indicating which 4thgen game's AcParts is being written.</param>
            public void Write(BinaryWriterEx bw, AcParts4Version version)
            {
                PartComponent.Write(bw, version);
                StabilizerComponent.Write(bw);
            }

            /// <inheritdoc/>
            public override string ToString()
            {
                return PartComponent.ToString();
            }
        }
    }
}
