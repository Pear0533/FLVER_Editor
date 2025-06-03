using System;

namespace SoulsFormats
{
    public static partial class FLVER
    {
        /// <summary>
        /// Represents one property of a vertex.
        /// </summary>
        public class LayoutMember
        {
            /// <summary>
            /// The index of this member into the current layout.<br/>
            /// Used to combine members into a single layout.<br/>
            /// Primarily used in edge-compressed PS3 models due to positions needing to be separate.
            /// </summary>
            public int Unk00 { get; set; }

            /// <summary>
            /// Value of -32768 denotes this member isn't stored with the vertex buffer due to Speedtree.
            /// </summary>
            public short SpecialModifier { get; set; }

            /// <summary>
            /// Format used to store this member.
            /// </summary>
            public LayoutType Type { get; set; }

            /// <summary>
            /// Vertex property being stored.
            /// </summary>
            public LayoutSemantic Semantic { get; set; }

            /// <summary>
            /// For semantics that may appear more than once such as UVs, which one this member is.
            /// </summary>
            public int Index { get; set; }

            /// <summary>
            /// The size of this member's ValueType, in bytes.
            /// </summary>
            public int Size
            {
                get
                {
                    if (Type == LayoutType.EdgeCompressed)
                    {
                        return 1;
                    }
                    if (Type == LayoutType.Float1
                        || Type == LayoutType.Color
                        || Type == LayoutType.UByte4
                        || Type == LayoutType.Byte4
                        || Type == LayoutType.UByte4Norm
                        || Type == LayoutType.Byte4Norm
                        || Type == LayoutType.Short2
                        || Type == LayoutType.UShort2
                        || Type == LayoutType.Byte4E
                        || Type == LayoutType.Half2
                        || Type == LayoutType.Byte4A
                        || Type == LayoutType.Byte4B
                        || Type == LayoutType.Short2toFloat2
                        || Type == LayoutType.Byte4C
                        || Type == LayoutType.UV
                        || Type == LayoutType.Byte4E)
                    {
                        return 4;
                    }
                    if (Type == LayoutType.Float2 || Type == LayoutType.Short4 || Type == LayoutType.UShort4 || Type == LayoutType.Short4Norm || Type == LayoutType.Half4
                        || Type == LayoutType.Float2
                        || Type == LayoutType.UVPair
                        || Type == LayoutType.ShortBoneIndices
                        || Type == LayoutType.Short4toFloat4A
                        || Type == LayoutType.Short4toFloat4B)
                    {
                        return 8;
                    }
                    if (Type == LayoutType.Float3)
                    {
                        return 12;
                    }
                    if (Type == LayoutType.Float4)
                    {
                        return 16;
                    }
                    throw new NotImplementedException($"No size defined for buffer layout type: {Type}");
                }
            }

            /// <summary>
            /// Creates a <see cref="LayoutMember"/> with the specified values.
            /// </summary>
            public LayoutMember(LayoutType type, LayoutSemantic semantic, int index = 0, int unk00 = 0, short specialModifier = 0)
            {
                Unk00 = unk00;
                SpecialModifier = specialModifier;
                Type = type;
                Semantic = semantic;
                Index = index;
            }

            /// <summary>
            /// Clone an existing <see cref="LayoutMember"/>.
            /// </summary>
            public LayoutMember(LayoutMember layoutMember)
            {
                Unk00 = layoutMember.Unk00;
                Type = layoutMember.Type;
                Semantic = layoutMember.Semantic;
                Index = layoutMember.Index;
            }

            internal LayoutMember(BinaryReaderEx br, int structOffset, bool isSpeedTree)
            {
                if (isSpeedTree)
                {
                    Unk00 = br.ReadInt16();
                    SpecialModifier = br.ReadInt16();
                }
                else
                {
                    Unk00 = br.ReadInt32();
                }

                br.AssertInt32(structOffset);
                Type = br.ReadEnum32<LayoutType>();
                Semantic = br.ReadEnum32<LayoutSemantic>();
                Index = br.ReadInt32();
            }

            internal void Write(BinaryWriterEx bw, int structOffset, bool isSpeedTree)
            {
                if (isSpeedTree)
                {
                    bw.WriteInt16((short)Unk00);
                    bw.WriteInt16(SpecialModifier);
                }
                else
                {
                    bw.WriteInt32(Unk00);
                }

                bw.WriteInt32(structOffset);
                bw.WriteUInt32((uint)Type);
                bw.WriteUInt32((uint)Semantic);
                bw.WriteInt32(Index);
            }

            /// <summary>
            /// Returns the value type and semantic of this <see cref="LayoutMember"/>.
            /// </summary>
            public override string ToString()
            {
                return $"{Type}: {Semantic}";
            }
        }

        /// <summary>
        /// Format of a vertex property.
        /// </summary>
        public enum LayoutType : uint
        {
            /// <summary>
            /// One single-precision float.
            /// </summary>
            Float1 = 0,

            /// <summary>
            /// Two single-precision floats.
            /// </summary>
            Float2 = 1,

            /// <summary>
            /// Three single-precision floats.
            /// </summary>
            Float3 = 2,

            /// <summary>
            /// Four single-precision floats.
            /// </summary>
            Float4 = 3,

            /// <summary>
            /// Four bytes.
            /// </summary>
            Color = 16,

            /// <summary>
            /// Four unsigned bytes.
            /// </summary>
            UByte4 = 17,

            /// <summary>
            /// Four signed bytes.
            /// </summary>
            Byte4 = 18,

            /// <summary>
            /// Four unsigned and normalized bytes.
            /// </summary>
            UByte4Norm = 19,

            /// <summary>
            /// Four signed and normalized bytes.
            /// </summary>
            Byte4Norm = 20,

            /// <summary>
            /// Two signed shorts.
            /// </summary>
            Short2 = 21,

            /// <summary>
            /// Four signed shorts.
            /// </summary>
            Short4 = 22,

            /// <summary>
            /// Two unsigned shorts.
            /// </summary>
            UShort2 = 23,

            /// <summary>
            /// Four unsigned shorts.
            /// </summary>
            UShort4 = 24,

            /// <summary>
            /// Four signed and normalized shorts.
            /// </summary>
            Short4Norm = 26,

            /// <summary>
            /// Two half-precision values.
            /// </summary>
            Half2 = 45,

            /// <summary>
            /// Four half-precision values.
            /// </summary>
            Half4 = 46,

            /// <summary>
            /// Unknown.
            /// </summary>
            Byte4E = 47,

            /// <summary>
            /// Edge compression specified by edge members in face sets.
            /// </summary>
            EdgeCompressed = 240,

            /// <summary>
            /// Unknown.
            /// </summary>
            Byte4A = 0x10,

            /// <summary>
            /// Four bytes.
            /// </summary>
            Byte4B = 0x11,

            /// <summary>
            /// Two shorts?
            /// </summary>
            Short2toFloat2 = 0x12,

            /// <summary>
            /// Four bytes.
            /// </summary>
            Byte4C = 0x13,

            /// <summary>
            /// Two shorts.
            /// </summary>
            UV = 0x15,

            /// <summary>
            /// Two shorts and two shorts.
            /// </summary>
            UVPair = 0x16,

            /// <summary>
            /// Four shorts, maybe unsigned?
            /// </summary>
            ShortBoneIndices = 0x18,

            /// <summary>
            /// Four shorts.
            /// </summary>
            Short4toFloat4A = 0x1A,

            /// <summary>
            /// Unknown.
            /// </summary>
            Short4toFloat4B = 0x2E,
        }

        /// <summary>
        /// Property of a vertex.
        /// </summary>
        public enum LayoutSemantic : uint
        {
            /// <summary>
            /// Location of the vertex.
            /// </summary>
            Position = 0,

            /// <summary>
            /// Weight of the vertex's attachment to bones.
            /// </summary>
            BoneWeights = 1,

            /// <summary>
            /// Bones the vertex is weighted to, indexing the parent mesh's bone indices.
            /// </summary>
            BoneIndices = 2,

            /// <summary>
            /// Orientation of the vertex.
            /// </summary>
            Normal = 3,

            /// <summary>
            /// Texture coordinates of the vertex.
            /// </summary>
            UV = 5,

            /// <summary>
            /// Vector pointing perpendicular to the normal.
            /// </summary>
            Tangent = 6,

            /// <summary>
            /// Vector pointing perpendicular to the normal and tangent.
            /// </summary>
            Bitangent = 7,

            /// <summary>
            /// Data used for blending, alpha, etc.
            /// </summary>
            VertexColor = 10,
        }
    }
}
