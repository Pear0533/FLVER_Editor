namespace SoulsFormats
{
    public partial class SMD4
    {
        /// <summary>
        /// Unknown.
        /// </summary>
        public class Unk10
        {
            /// <summary>
            /// Unknown.
            /// </summary>
            public byte Unk00 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte Unk01 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte Unk02 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte Unk03 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Create a new <see cref="Unk10"/>.
            /// </summary>
            public Unk10()
            {
                Unk00 = 0;
                Unk01 = 0;
                Unk02 = 0;
                Unk03 = 0;
                Name = string.Empty;
            }

            /// <summary>
            /// Clone an existing <see cref="Unk10"/>.
            /// </summary>
            public Unk10(Unk10 unk10)
            {
                Unk00 = unk10.Unk00;
                Unk01 = unk10.Unk01;
                Unk02 = unk10.Unk02;
                Unk03 = unk10.Unk03;
                Name = unk10.Name;
            }

            /// <summary>
            /// Read a <see cref="Unk10"/> from a stream.
            /// </summary>
            /// <param name="br">The stream reader.</param>
            internal Unk10(BinaryReaderEx br)
            {
                Unk00 = br.ReadByte();
                Unk01 = br.ReadByte();
                Unk02 = br.ReadByte();
                Unk03 = br.ReadByte();
                Name = br.ReadFixStr(32);
            }

            /// <summary>
            /// Write this <see cref="Unk10"/> to a stream.
            /// </summary>
            /// <param name="bw">The stream writer.</param>
            internal void Write(BinaryWriterEx bw)
            {
                bw.WriteByte(Unk00);
                bw.WriteByte(Unk01);
                bw.WriteByte(Unk02);
                bw.WriteByte(Unk03);
                bw.WriteFixStr(Name, 32, 0);
            }
        }
    }
}
