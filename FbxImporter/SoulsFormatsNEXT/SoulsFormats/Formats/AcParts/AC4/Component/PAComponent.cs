namespace SoulsFormats
{
    public partial class AcParts4
    {
        /// <summary>
        /// A Component which contains Primal Armor stats.
        /// </summary>
        public class PAComponent
        {
            /// <summary>
            /// The Primal Armor Rectification added for equipping this part.
            /// </summary>
            public ushort PARectification { get; set; }

            /// <summary>
            /// The Primal Armor Durability added for equipping this part.
            /// </summary>
            public ushort PADurability { get; set; }

            /// <summary>
            /// Makes a new <see cref="PAComponent"/>.
            /// </summary>
            public PAComponent()
            {

            }

            /// <summary>
            /// Reads a PA component from a stream.
            /// </summary>
            /// <param name="br">A binary reader.</param>
            internal PAComponent(BinaryReaderEx br)
            {
                PARectification = br.ReadUInt16();
                PADurability = br.ReadUInt16();
            }

            /// <summary>
            /// Writes a PA component to a stream.
            /// </summary>
            /// <param name="bw">A binary writer.</param>
            public void Write(BinaryWriterEx bw)
            {
                bw.WriteUInt16(PARectification);
                bw.WriteUInt16(PADurability);
            }
        }
    }
}
