namespace SoulsFormats
{
    public partial class AcParts4
    {
        /// <summary>
        /// A Component which contains Defense stats.
        /// </summary>
        public class DefenseComponent
        {
            /// <summary>
            /// Ability to withstand damage from projectiles and shells.
            /// </summary>
            public ushort BallisticDefense { get; set; }

            /// <summary>
            /// Ability to withstand damage from energy weapons.
            /// </summary>
            public ushort EnergyDefense { get; set; }

            /// <summary>
            /// Makes a new <see cref="DefenseComponent"/>.
            /// </summary>
            public DefenseComponent()
            {

            }

            /// <summary>
            /// Reads a Defense component from a stream.
            /// </summary>
            /// <param name="br">A binary reader.</param>
            internal DefenseComponent(BinaryReaderEx br)
            {
                BallisticDefense = br.ReadUInt16();
                EnergyDefense = br.ReadUInt16();
            }

            /// <summary>
            /// Writes a Defense component to a stream.
            /// </summary>
            /// <param name="bw">A binary writer.</param>
            public void Write(BinaryWriterEx bw)
            {
                bw.WriteUInt16(BallisticDefense);
                bw.WriteUInt16(EnergyDefense);
            }
        }
    }
}
