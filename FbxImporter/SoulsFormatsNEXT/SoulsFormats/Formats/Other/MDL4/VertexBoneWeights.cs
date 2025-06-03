using System;

namespace SoulsFormats.Other
{
    public partial class MDL4
    {
        /// <summary>
        /// Four weights for binding a vertex to bones, accessed like an array. Unused bones should be set to 0.
        /// </summary>
        public struct VertexBoneWeights
        {
            private float A, B, C, D;

            /// <summary>
            /// Length of bone weights is always 4.
            /// </summary>
            public int Length => 4;

            /// <summary>
            /// Accesses bone weights as a float[4].
            /// </summary>
            public float this[int i]
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
            /// Creates a VertexBoneWeights with the given ABCD values in float form.
            /// </summary>
            public VertexBoneWeights(float a, float b, float c, float d)
            {
                A = a;
                B = b;
                C = c;
                D = d;
            }

            /// <summary>
            /// Read a VertexBoneWeight from a stream.
            /// </summary>
            internal static VertexBoneWeights ReadBoneWeights(BinaryReaderEx br)
            {
                float a = br.ReadSingle();
                float b = br.ReadSingle();
                float c = br.ReadSingle();
                float d = br.ReadSingle();
                return new VertexBoneWeights(a, b, c, d);
            }

            /// <summary>
            /// Write this VertexBoneWeight to a stream.
            /// </summary>
            internal void WriteBoneWeights(BinaryWriterEx bw)
            {
                bw.WriteSingle(A);
                bw.WriteSingle(B);
                bw.WriteSingle(C);
                bw.WriteSingle(D);
            }
        }
    }
}
