using System;

namespace SoulsFormats.Other
{
    public partial class MDL4
    {
        /// <summary>
        /// Four indices of bones to bind a vertex to, accessed like an array. Unused bones should be set to 0.
        /// </summary>
        public struct VertexBoneIndices
        {
            private short A, B, C, D;

            /// <summary>
            /// Length of bone indices is always 4.
            /// </summary>
            public int Length => 4;

            /// <summary>
            /// Accesses bone indices as a short[4].
            /// </summary>
            public short this[int i]
            {
                get
                {
                    switch (i)
                    {
                        case 0: return A;
                        case 1: return B;
                        case 2: return C;
                        case 3: return D;
                        default:
                            throw new IndexOutOfRangeException($"Index ({i}) was out of range. Must be non-negative and less than 4.");
                    }
                }

                set
                {
                    switch (i)
                    {
                        case 0: A = value; break;
                        case 1: B = value; break;
                        case 2: C = value; break;
                        case 3: D = value; break;
                        default:
                            throw new IndexOutOfRangeException($"Index ({i}) was out of range. Must be non-negative and less than 4.");
                    }
                }
            }

            /// <summary>
            /// Creates a VertexBoneIndices with the given ABCD values in short form.
            /// </summary>
            public VertexBoneIndices(short a, short b, short c, short d)
            {
                A = a;
                B = b;
                C = c;
                D = d;
            }

            /// <summary>
            /// Reads a vertex bone indices array from a BinaryReaderEx.
            /// </summary>
            internal static VertexBoneIndices ReadBoneIndices(BinaryReaderEx br)
            {
                short a = br.ReadInt16();
                short b = br.ReadInt16();
                short c = br.ReadInt16();
                short d = br.ReadInt16();
                return new VertexBoneIndices(a, b, c, d);
            }

            /// <summary>
            /// Writes vertex bone indice arrays to a BinaryWriterEx.
            /// </summary>
            internal void WriteBoneIndices(BinaryWriterEx bw)
            {
                bw.WriteInt16(A);
                bw.WriteInt16(B);
                bw.WriteInt16(C);
                bw.WriteInt16(D);
            }
        }
    }
}
