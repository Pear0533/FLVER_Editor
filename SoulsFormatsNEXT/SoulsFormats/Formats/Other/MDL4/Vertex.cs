using System;
using System.Collections.Generic;
using System.Numerics;

namespace SoulsFormats.Other
{
    /// <summary>
    /// A 3D model format used in early PS3/X360 games. Extension: .mdl
    /// </summary>
    public partial class MDL4
    {
        /// <summary>
        /// A single point in a mesh.
        /// </summary>
        public class Vertex
        {
            /// <summary>
            /// Where the vertex is.
            /// </summary>
            public Vector3 Position;

            /// <summary>
            /// Vector pointing away from the surface.
            /// </summary>
            public Vector4 Normal;

            /// <summary>
            /// Vector pointing perpendicular to the vector.
            /// </summary>
            public Vector4 Tangent;

            /// <summary>
            /// Vector pointing perpendicular to the vector and tangent.
            /// </summary>
            public Vector4 Bitangent;

            /// <summary>
            /// This Vertex's Color.
            /// </summary>
            public VertexColor Color;

            /// <summary>
            /// Texture coordinates of the vertex.
            /// </summary>
            public List<Vector2> UVs;

            /// <summary>
            /// Bones the vertex is weighted to, indexing the parent mesh's bone indices; must be 4 length.
            /// </summary>
            public VertexBoneIndices BoneIndices;

            /// <summary>
            /// Weight of the vertex's attachment to bones; must be 4 length.
            /// </summary>
            public VertexBoneWeights BoneWeights;

            /// <summary>
            /// Unknown.
            /// </summary>
            public short Unk3C;

            /// <summary>
            /// Unknown.
            /// </summary>
            public short Unk3E;

            /// <summary>
            /// Create a new and empty Vertex.
            /// </summary>
            public Vertex()
            {
                UVs = new List<Vector2>();
            }

            /// <summary>
            /// Clone an existing Vertex.
            /// </summary>
            public Vertex(Vertex vertex)
            {
                Position = vertex.Position;
                Normal = vertex.Normal;
                Tangent = vertex.Tangent;
                Bitangent = vertex.Bitangent;
                UVs = new List<Vector2>(4);
                foreach (Vector2 uv in vertex.UVs)
                    UVs.Add(uv);
                BoneIndices = new VertexBoneIndices();
                BoneWeights = new VertexBoneWeights();
            }

            /// <summary>
            /// Read a Vertex from a stream.
            /// </summary>
            internal Vertex(BinaryReaderEx br, int version, byte format)
            {
                UVs = new List<Vector2>();
                if (version == 0x40001)
                {
                    if (format == 0)
                    {
                        Position = br.ReadVector3();
                        Normal = Read10BitVector4(br);
                        Tangent = Read10BitVector4(br);
                        Bitangent = Read10BitVector4(br);
                        Color = VertexColor.ReadByteARGB(br);
                        UVs.Add(br.ReadVector2());
                        UVs.Add(br.ReadVector2());
                        UVs.Add(br.ReadVector2());
                        UVs.Add(br.ReadVector2());
                        Unk3C = br.ReadInt16();
                        Unk3E = br.AssertInt16(0);
                    }
                    else if (format == 1)
                    {
                        Position = br.ReadVector3();
                        Normal = Read10BitVector4(br);
                        Tangent = Read10BitVector4(br);
                        Bitangent = Read10BitVector4(br);
                        Color = VertexColor.ReadByteARGB(br);
                        UVs.Add(br.ReadVector2());
                        UVs.Add(br.ReadVector2());
                        UVs.Add(br.ReadVector2());
                        UVs.Add(br.ReadVector2());
                        BoneIndices = VertexBoneIndices.ReadBoneIndices(br);
                        BoneWeights = VertexBoneWeights.ReadBoneWeights(br);
                    }
                    else if (format == 2)
                    {
                        Color = VertexColor.ReadByteARGB(br);
                        UVs.Add(br.ReadVector2());
                        UVs.Add(br.ReadVector2());
                        UVs.Add(br.ReadVector2());
                        UVs.Add(br.ReadVector2());
                        BoneIndices = VertexBoneIndices.ReadBoneIndices(br);
                        BoneWeights = VertexBoneWeights.ReadBoneWeights(br);
                    }
                }
                else if (version == 0x40002)
                {
                    if (format == 0)
                    {
                        Position = br.ReadVector3();
                        Normal = ReadSByteVector4Normal(br);
                        Tangent = ReadSByteVector4(br);
                        Color = VertexColor.ReadByteARGB(br);
                        UVs.Add(ReadShortUV(br));
                        UVs.Add(ReadShortUV(br));
                        UVs.Add(ReadShortUV(br));
                        UVs.Add(ReadShortUV(br));
                    }
                }
            }

            /// <summary>
            /// Write this Vertex to a stream.
            /// </summary>
            internal void Write(BinaryWriterEx bw, int version, byte format)
            {
                if (version == 0x40001)
                {
                    if (format == 0)
                    {
                        bw.WriteVector3(Position);
                        Write10BitVector4(bw, Normal);
                        Write10BitVector4(bw, Tangent);
                        Write10BitVector4(bw, Bitangent);
                        Color.WriteByteARGB(bw);
                        bw.WriteVector2(UVs[0]);
                        bw.WriteVector2(UVs[1]);
                        bw.WriteVector2(UVs[2]);
                        bw.WriteVector2(UVs[3]);
                        bw.WriteInt16(Unk3C);
                        bw.WriteInt16(Unk3E);
                    }
                    else if (format == 1)
                    {
                        bw.WriteVector3(Position);
                        Write10BitVector4(bw, Normal);
                        Write10BitVector4(bw, Tangent);
                        Write10BitVector4(bw, Bitangent);
                        Color.WriteByteARGB(bw);
                        bw.WriteVector2(UVs[0]);
                        bw.WriteVector2(UVs[1]);
                        bw.WriteVector2(UVs[2]);
                        bw.WriteVector2(UVs[3]);
                        BoneIndices.WriteBoneIndices(bw);
                        BoneWeights.WriteBoneWeights(bw);
                    }
                    else if (format == 2)
                    {
                        Color.WriteByteARGB(bw);
                        bw.WriteVector2(UVs[0]);
                        bw.WriteVector2(UVs[1]);
                        bw.WriteVector2(UVs[2]);
                        bw.WriteVector2(UVs[3]);
                        BoneIndices.WriteBoneIndices(bw);
                        BoneWeights.WriteBoneWeights(bw);
                    }
                }
                else if (version == 0x40002)
                {
                    if (format == 0)
                    {
                        bw.WriteVector3(Position);
                        WriteSByteVector4Normal(bw, Normal);
                        WriteSByteVector4(bw, Tangent);
                        Color.WriteByteARGB(bw);
                        WriteShortUV(bw, UVs[0]);
                        WriteShortUV(bw, UVs[1]);
                        WriteShortUV(bw, UVs[2]);
                        WriteShortUV(bw, UVs[3]);
                    }
                    else
                    {
                        throw new NotImplementedException($"Vertex format {format} not implemented for version {version}.");
                    }
                }
            }

            private static Vector4 ReadSByteVector4(BinaryReaderEx br)
            {
                sbyte w = br.ReadSByte();
                sbyte z = br.ReadSByte();
                sbyte y = br.ReadSByte();
                sbyte x = br.ReadSByte();
                return new Vector4(x / 127f, y / 127f, z / 127f, w / 127f);
            }

            private static Vector4 ReadSByteVector4Normal(BinaryReaderEx br)
            {
                sbyte w = br.ReadSByte();
                sbyte z = br.ReadSByte();
                sbyte y = br.ReadSByte();
                sbyte x = br.ReadSByte();
                return new Vector4(x / 127f, y / 127f, z / 127f, w);
            }

            private static Vector2 ReadShortUV(BinaryReaderEx br)
            {
                short u = br.ReadInt16();
                short v = br.ReadInt16();
                return new Vector2(u / 2048f, v / 2048f);
            }

            private static Vector4 Read10BitVector4(BinaryReaderEx br)
            {
                int vector = br.ReadInt32();
                int x = vector << 22 >> 22;
                int y = vector << 12 >> 22;
                int z = vector << 2 >> 22;
                int w = vector << 0 >> 30;
                return new Vector4((float)Math.Round(x / 511f), (float)Math.Round(y / 511f), (float)Math.Round(z / 511f), w);
            }

            private static void WriteSByteVector4(BinaryWriterEx bw, Vector4 vector)
            {
                bw.WriteSByte((sbyte)(vector.W * 127f));
                bw.WriteSByte((sbyte)(vector.Z * 127f));
                bw.WriteSByte((sbyte)(vector.Y * 127f));
                bw.WriteSByte((sbyte)(vector.X * 127f));
            }

            private static void WriteSByteVector4Normal(BinaryWriterEx bw, Vector4 vector)
            {
                bw.WriteSByte((sbyte)vector.W);
                bw.WriteSByte((sbyte)(vector.Z * 127f));
                bw.WriteSByte((sbyte)(vector.Y * 127f));
                bw.WriteSByte((sbyte)(vector.X * 127f));
            }

            private static void WriteShortUV(BinaryWriterEx bw, Vector2 vector)
            {
                bw.WriteInt16((short)(vector.X * 2048f));
                bw.WriteInt16((short)(vector.Y * 2048f));
            }

            private static void Write10BitVector4(BinaryWriterEx bw, Vector4 vector)
            {
                int x = (int)(vector.X * 511f) & 0x3FF;
                int y = (int)(vector.Y * 511f) & 0x3FF;
                int z = (int)(vector.Z * 511f) & 0x3FF;
                int w = ((int)vector.W) & 0x3;

                bw.WriteInt32(w << 30 | z << 20 | y << 10 | x);
            }
        }
    }
}
