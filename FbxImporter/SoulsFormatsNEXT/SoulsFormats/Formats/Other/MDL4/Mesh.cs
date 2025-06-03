using System.Collections.Generic;

namespace SoulsFormats.Other
{
    /// <summary>
    /// A 3D model format used in early PS3/X360 games. Extension: .mdl
    /// </summary>
    public partial class MDL4
    {
        /// <summary>
        /// An individual chunk of a model.
        /// </summary>
        public class Mesh
        {
            /// <summary>
            /// The format of vertices in this mesh.
            /// </summary>
            public byte VertexFormat;

            /// <summary>
            /// The index of the material used by all triangles in this mesh.
            /// </summary>
            public byte MaterialIndex;

            /// <summary>
            /// Unknown.<br/>
            /// Cullbackfaces?
            /// </summary>
            public bool Unk02;

            /// <summary>
            /// Unknown.<br/>
            /// TriangleStrip?
            /// </summary>
            public bool Unk03;

            /// <summary>
            /// Unknown.
            /// </summary>
            public short Unk08;

            /// <summary>
            /// The indexes of bones used by this mesh.
            /// </summary>
            /// <remarks>
            /// Always has 28 indices; Unused indices are set to -1.
            /// </remarks>
            public short[] BoneIndices;

            /// <summary>
            /// Indexes of the vertices of this mesh.
            /// </summary>
            public List<ushort> Indices;

            /// <summary>
            /// Vertices in this mesh.
            /// </summary>
            public List<Vertex> Vertices;

            /// <summary>
            /// Unknown; Only present if VertexFormat is 2.
            /// </summary>
            public byte[][] UnkBlocks;

            /// <summary>
            /// Reads a mesh from a BinaryReaderEx.
            /// </summary>
            internal Mesh(BinaryReaderEx br, int dataOffset, int version)
            {
                VertexFormat = br.AssertByte(0, 1, 2);
                MaterialIndex = br.ReadByte();
                Unk02 = br.ReadBoolean();
                Unk03 = br.ReadBoolean();
                ushort vertexIndexCount = br.ReadUInt16();
                Unk08 = br.ReadInt16();
                BoneIndices = br.ReadInt16s(28);
                int vertexIndicesLength = br.ReadInt32();
                int vertexIndicesOffset = br.ReadInt32();
                int bufferLength = br.ReadInt32();
                int bufferOffset = br.ReadInt32();

                if (VertexFormat == 2)
                {
                    UnkBlocks = new byte[16][];
                    for (int i = 0; i < 16; i++)
                    {
                        int length = br.ReadInt32();
                        int offset = br.ReadInt32();
                        UnkBlocks[i] = br.GetBytes(dataOffset + offset, length);
                    }
                }

                Indices = new List<ushort>(br.GetUInt16s(dataOffset + vertexIndicesOffset, vertexIndexCount));

                br.StepIn(dataOffset + bufferOffset);
                {
                    int vertexSize = 0;
                    if (version == 0x40001)
                    {
                        if (VertexFormat == 0)
                            vertexSize = 0x40;
                        else if (VertexFormat == 1)
                            vertexSize = 0x54;
                        else if (VertexFormat == 2)
                            vertexSize = 0x3C;
                    }
                    else if (version == 0x40002)
                    {
                        if (VertexFormat == 0)
                            vertexSize = 0x28;
                    }
                    int vertexCount = bufferLength / vertexSize;
                    Vertices = new List<Vertex>(vertexCount);
                    for (int i = 0; i < vertexCount; i++)
                        Vertices.Add(new Vertex(br, version, VertexFormat));
                }
                br.StepOut();
            }

            /// <summary>
            /// Writes a mesh to a BinaryWriterEx.
            /// </summary>
            internal void Write(BinaryWriterEx bw, int index)
            {
                bw.WriteByte(VertexFormat);
                bw.WriteByte(MaterialIndex);
                bw.WriteBoolean(Unk02);
                bw.WriteBoolean(Unk03);
                bw.WriteUInt16((ushort)Indices.Count); // Vertex Index Count
                bw.WriteInt16(Unk08);
                bw.WriteInt16s(BoneIndices);
                bw.ReserveInt32($"VertexIndicesLength_{index}");
                bw.ReserveInt32($"VertexIndicesOffset_{index}");
                bw.ReserveInt32($"BufferLength_{index}");
                bw.ReserveInt32($"BufferOffset_{index}");
            }

            /// <summary>
            /// Get a list of faces as index arrays.
            /// </summary>
            /// <param name="allowPrimitiveRestarts">Whether or not to allow primitive restarts.</param>
            /// <param name="includeDegenerateFaces">Whether or not to include degenerate faces.</param>
            /// <returns>A list of triangle arrays.</returns>
            public List<ushort[]> GetFaceIndices(bool allowPrimitiveRestarts, bool includeDegenerateFaces)
            {
                List<ushort> indices = Triangulate(allowPrimitiveRestarts, includeDegenerateFaces);
                var faces = new List<ushort[]>();
                for (int i = 0; i < indices.Count; i += 3)
                {
                    faces.Add(new ushort[]
                    {
                        indices[i + 0],
                        indices[i + 1],
                        indices[i + 2]
                    });
                }
                return faces;
            }

            /// <summary>
            /// Get an approximate triangle count for the mesh indices.
            /// </summary>
            /// <param name="allowPrimitiveRestarts">Whether or not to allow primitive restarts.</param>
            /// <param name="includeDegenerateFaces">Whether or not to include degenerate faces.</param>
            /// <returns>An approximate triangle count.</returns>
            public int GetFaceCount(bool allowPrimitiveRestarts, bool includeDegenerateFaces)
            {
                // Triangle strip
                int counter = 0;
                for (int i = 0; i < Indices.Count - 2; i++)
                {
                    int vi1 = Indices[i];
                    int vi2 = Indices[i + 1];
                    int vi3 = Indices[i + 2];

                    bool notRestart = allowPrimitiveRestarts || (vi1 != 0xFFFF && vi2 != 0xFFFF && vi3 != 0xFFFF);
                    bool included = includeDegenerateFaces || (vi1 != vi2 && vi1 != vi3 && vi2 != vi3);
                    if (notRestart && included)
                    {
                        counter++;
                    }
                }

                return counter;
            }

            /// <summary>
            /// Attempt to triangulate the mesh face indices.
            /// </summary>
            /// <param name="allowPrimitiveRestarts">Whether or not to allow primitive restarts.</param>
            /// <param name="includeDegenerateFaces">Whether or not to include degenerate faces.</param>
            /// <returns>A triangulated list of face indices.</returns>
            public List<ushort> Triangulate(bool allowPrimitiveRestarts, bool includeDegenerateFaces)
            {
                var triangles = new List<ushort>();
                bool flip = false;
                for (int i = 0; i < Indices.Count - 2; i++)
                {
                    ushort vi1 = Indices[i];
                    ushort vi2 = Indices[i + 1];
                    ushort vi3 = Indices[i + 2];

                    if (allowPrimitiveRestarts && (vi1 == 0xFFFF || vi2 == 0xFFFF || vi3 == 0xFFFF))
                    {
                        flip = true;
                    }
                    else
                    {
                        if (includeDegenerateFaces || (vi1 != vi2 && vi2 != vi3 && vi3 != vi1))
                        {
                            if (flip)
                            {
                                triangles.Add(vi3);
                                triangles.Add(vi2);
                                triangles.Add(vi1);
                            }
                            else
                            {
                                triangles.Add(vi1);
                                triangles.Add(vi2);
                                triangles.Add(vi3);
                            }
                        }
                        flip = !flip;
                    }
                }

                return triangles;
            }
        }
    }
}
