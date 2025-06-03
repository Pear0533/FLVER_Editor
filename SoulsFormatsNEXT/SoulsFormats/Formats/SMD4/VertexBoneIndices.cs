using System;

namespace SoulsFormats
{
    public partial class SMD4
    {
        /// <summary>
        /// Four indices of bones to bind a vertex to, accessed like an array. Unused bones should be set to -1.
        /// </summary>
        public struct VertexBoneIndices
        {
            private short A, B, C, D;

            /// <summary>
            /// Length of bone indices is always 4.
            /// </summary>
            public int Length => 4;

            /// <summary>
            /// Accesses bone indices as an int[4].
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
            /// Create a new VertexBoneIndices array using the specified indices.
            /// </summary>
            public VertexBoneIndices(short a, short b, short c, short d)
            {
                A = a;
                B = b;
                C = c;
                D = d;
            }
        }
    }
}
