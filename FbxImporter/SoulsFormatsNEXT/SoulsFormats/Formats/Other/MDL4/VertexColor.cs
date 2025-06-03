namespace SoulsFormats.Other
{
    /// <summary>
    /// A 3D model format used in early PS3/X360 games. Extension: .mdl
    /// </summary>
    public partial class MDL4
    {
        /// <summary>
        /// A vertex color with ARGB components.
        /// </summary>
        public struct VertexColor
        {
            /// <summary>
            /// Alpha component of the color.
            /// </summary>
            public byte A;

            /// <summary>
            /// Red component of the color.
            /// </summary>
            public byte R;

            /// <summary>
            /// Green component of the color.
            /// </summary>
            public byte G;

            /// <summary>
            /// Blue component of the color.
            /// </summary>
            public byte B;

            /// <summary>
            /// Creates a VertexColor with the given ARGB values.
            /// </summary>
            public VertexColor(byte a, byte r, byte g, byte b)
            {
                A = a;
                R = r;
                G = g;
                B = b;
            }

            /// <summary>
            /// Read a VertexColor from a stream in ARGB order.
            /// </summary>
            internal static VertexColor ReadByteARGB(BinaryReaderEx br)
            {
                byte a = br.ReadByte();
                byte r = br.ReadByte();
                byte g = br.ReadByte();
                byte b = br.ReadByte();
                return new VertexColor(a, r, g, b);
            }

            /// <summary>
            /// Write a VertexColor to a stream in ARGA order.
            /// </summary>
            internal void WriteByteARGB(BinaryWriterEx bw)
            {
                bw.WriteByte(A);
                bw.WriteByte(R);
                bw.WriteByte(G);
                bw.WriteByte(B);
            }
        }
    }
}
