using System;
using System.Numerics;

namespace SoulsFormats
{
    public partial class SMD4
    {
        /// <summary>
        /// A single point in a mesh.
        /// </summary>
        public class Vertex
        {
            /// <summary>
            /// Where the vertex is.
            /// </summary>
            public Vector3 Position { get; set; }

            /// <summary>
            /// A texture coordinate of the vertex.
            /// </summary>
            public Vector2 UV { get; set; }

            /// <summary>
            /// Bones the vertex is weighted to, indexing the parent mesh's bone indices.
            /// </summary>
            public VertexBoneIndices BoneIndices { get; set; }

            /// <summary>
            /// Weight of the vertex's attachment to bones.
            /// </summary>
            public VertexBoneWeights BoneWeights { get; set; }

            /// <summary>
            /// Create a new Vertex with default values.
            /// </summary>
            public Vertex(){}

            /// <summary>
            /// Clone an existing Vertex.
            /// </summary>
            public Vertex(Vertex vertex)
            {
                Position = vertex.Position;
                BoneIndices = vertex.BoneIndices;
                BoneWeights = vertex.BoneWeights;
            }

            /// <summary>
            /// Read a Vertex from a stream.
            /// </summary>
            internal Vertex(BinaryReaderEx br, int version, int vertexFormat)
            {
                if (version == 0x40001)
                {
                    if (vertexFormat == 0)
                    {
                        Position = br.ReadVector3();
                        BoneIndices = new VertexBoneIndices(br.ReadInt16(), -1, -1, -1);
                        BoneWeights = new VertexBoneWeights(1f, 0f, 0f, 0f);
                        br.AssertInt16(0);
                    }
                    else if (vertexFormat == 1)
                    {
                        Position = br.ReadVector3();
                        UV = br.ReadVector2();
                        BoneIndices = new VertexBoneIndices(br.ReadInt16(), -1, -1, -1);
                        BoneWeights = new VertexBoneWeights(1f, 0f, 0f, 0f);
                        br.AssertInt16(0);
                    }
                    else if (vertexFormat == 2)
                    {
                        Position = br.ReadVector3();
                        BoneIndices = new VertexBoneIndices(br.ReadInt16(), br.ReadInt16(), br.ReadInt16(), br.ReadInt16());
                        BoneWeights = new VertexBoneWeights(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                    }
                    else
                    {
                        throw new NotSupportedException($"VertexFormat {vertexFormat} is not currently supported.");
                    }
                }
                else
                {
                    throw new NotSupportedException($"Version {version} is not currently supported.");
                }
            }

            /// <summary>
            /// Write a Vertex to a stream.
            /// </summary>
            internal void Write(BinaryWriterEx bw, int version, int vertexFormat)
            {
                if (version == 0x40001)
                {
                    if (vertexFormat == 0)
                    {
                        bw.WriteVector3(Position);
                        bw.WriteInt16(BoneIndices[0]);
                        bw.WriteInt16(0);
                    }
                    else if (vertexFormat == 1)
                    {
                        bw.WriteVector3(Position);
                        bw.WriteVector2(UV);
                        bw.WriteInt16(BoneIndices[0]);
                        bw.WriteInt16(0);
                    }
                    else if (vertexFormat == 2)
                    {
                        bw.WriteVector3(Position);
                        for (int i = 0; i < 4; i++)
                            bw.WriteInt16(BoneIndices[i]);
                        for (int i = 0; i < 4; i++)
                            bw.WriteSingle(BoneWeights[i]);
                    }
                    else
                    {
                        throw new NotSupportedException($"VertexFormat {vertexFormat} is not currently supported for Version {version}.");
                    }
                }
                else
                {
                    throw new NotSupportedException($"Version {version} is not currently supported.");
                }
            }
        }
    }
}
