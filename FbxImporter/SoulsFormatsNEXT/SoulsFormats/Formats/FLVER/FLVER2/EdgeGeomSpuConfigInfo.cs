﻿using SoulsFormats.Utilities;
using System.IO;

namespace SoulsFormats
{
    public partial class FLVER2
    {
        /// <summary>
        /// Configuration information for SPU edge geometry.
        /// </summary>
        public struct EdgeGeomSpuConfigInfo
        {
            /// <summary>
            /// Configuration flags for the geometry pipeline.
            /// </summary>
            public enum GeomConfigFlags : byte
            {
                /// <summary>
                /// The SPU job will not modify input geometry.<br/>
                /// This allows some functions in Edge to skip performing unnecessary work.
                /// </summary>
                StaticGeomFastPath = 1, // 0x10 in the combine flags and uniform table count byte.

                /// <summary>
                /// If a segment is using either culling or custom blend shape flavors, this flag is required.<br/>
                /// Both of culling and custom blend shape flavors require another uniform table to store temporary data.<br/>
                /// The size of the temporary uniform table has to be accounted for in the Edge partitioner when a segment is made.
                /// </summary>
                ExtraUniformTable = 8 // 0x80 in the combine flags and uniform table count byte.
            }

            /// <summary>
            /// The different supported index flavors.
            /// </summary>
            public enum EdgeGeomIndexes : byte
            {
                /// <summary>
                /// Indexes are in a triangle list with triangles wound clockwise, each index being an unsigned 16-bit integer.
                /// </summary>
                TriangleListUShortCW = 0,

                /// <summary>
                /// Indexes are in a triangle list with triangles wound counter-clockwise, each index being an unsigned 16-bit integer.
                /// </summary>
                TriangleListUShortCCW = 1,

                /// <summary>
                /// Indexes are in a compressed triangle list with triangles wound clockwise.
                /// </summary>
                TriangleListCompressedCW = 2,

                /// <summary>
                /// Indexes are in a compressed triangle list with triangles wound counter-clockwise.
                /// </summary>
                TriangleListCompressedCCW = 3
            }

            /// <summary>
            /// Matrix palette skinning types.
            /// </summary>
            public enum EdgeGeomSkin : byte
            {
                /// <summary>
                /// No skinning.
                /// </summary>
                None = 0,

                /// <summary>
                /// Do skinning by unit matrix.
                /// </summary>
                NoScaling = 1,

                /// <summary>
                /// Do skinning.
                /// </summary>
                UniformScaling = 2,

                /// <summary>
                /// Do skinning and compute cofactor matrices for transforming normals.
                /// </summary>
                NonUniformScaling = 3,

                /// <summary>
                /// Do skinning by a single bone unit matrix.
                /// </summary>
                SingleBoneNoScaling = 4,

                /// <summary>
                /// Do skinning by a single bone.
                /// </summary>
                SingleBoneUniformScaling = 5,

                /// <summary>
                /// Do skinning by a single bone and compute cofactor matrices for transforming normals.
                /// </summary>
                SingleBoneNonUniformScaling = 6
            }

            /// <summary>
            /// The different supported skinning matrix formats.
            /// </summary>
            public enum EdgeGeomSkinningMatrixFormat : byte
            {
                /// <summary>
                /// This is the native matrix type of edge and uses less memory than 4x4 matrices.<br/>
                /// It has the top three rows of a 4x4 matrix that is “DirectX-style”.<br/>
                /// The fourth row is not needed explicitly as it is always [0,0,0,1].
                /// </summary>
                RowMajor3x4 = 0,

                /// <summary>
                /// This matrix type is a “DirectX-style” row-major 4x4 matrix. 
                /// </summary>
                RowMajor4x4 = 1,

                /// <summary>
                /// This matrix type is a "OpenGL-style" column-major 4x4 matrix.
                /// </summary>
                ColumnMajor4x4 = 2
            }

            /// <summary>
            /// The input format for vertices coming from the SPU.
            /// </summary>
            public enum SpuVertexFormat : byte
            {
                /// <summary>
                /// Position in 3 floats.
                /// </summary>
                Float3 = 0,

                /// <summary>
                /// Position in 3 floats.<br/>
                /// Normal with X in 11 bits, Y in 11 bits, and Z in 10 bits.<br/>
                /// Tangent with X in 11 bits, Y in 11 bits, and Z in 10 bits.
                /// </summary>
                Float3PackedNorm2 = 1,

                /// <summary>
                /// Position in 3 floats.<br/>
                /// Normal with X in 11 bits, Y in 11 bits, and Z in 10 bits.<br/>
                /// Tangent with XYZW in a normalized to range [-1.0 - 1.0], signed 16-bit integer.
                /// </summary>
                Float3PackedNormShortNorm4 = 2,

                /// <summary>
                /// Position in 3 floats.<br/>
                /// Normal with X in 11 bits, Y in 11 bits, and Z in 10 bits.<br/>
                /// Tangent with X in 11 bits, Y in 11 bits, and Z in 10 bits.<br/>
                /// BiNormal with X in 11 bits, Y in 11 bits, and Z in 10 bits.
                /// </summary>
                Float3PackedNorm3 = 3,

                /// <summary>
                /// Position as an edge fixed point.<br/>
                /// Normal as an edge unit vector.<br/>
                /// Tangent as an edge unit vector.
                /// </summary>
                EdgeFixedUnit2 = 4,

                /// <summary>
                /// Position as an edge fixed point.<br/>
                /// Normal as an edge unit vector.<br/>
                /// Tangent as an edge unit vector.<br/>
                /// BiNormal as an edge unit vector.
                /// </summary>
                EdgeFixedUnit3 = 5,

                /// <summary>
                /// Position as an edge fixed point.<br/>
                /// Found in FromSoftware models in Armored Core Verdict Day.
                /// </summary>
                EdgeFixed = 254,

                /// <summary>
                /// A user defined format.
                /// </summary>
                Custom = 255
            }

            /// <summary>
            /// The output format for vertices going to the RSX.
            /// </summary>
            public enum RsxVertexFormat : byte
            {
                /// <summary>
                /// Position in 3 floats.
                /// </summary>
                Float3 = 0,

                /// <summary>
                /// Position in 3 floats.<br/>
                /// Normal with X in 11 bits, Y in 11 bits, and Z in 10 bits.<br/>
                /// Tangent with X in 11 bits, Y in 11 bits, and Z in 10 bits.
                /// </summary>
                Float3PackedNorm2 = 1,

                /// <summary>
                /// Position in 3 floats.<br/>
                /// Normal with X in 11 bits, Y in 11 bits, and Z in 10 bits.<br/>
                /// Tangent with XYZW in a normalized to range [-1.0 - 1.0], signed 16-bit integer.
                /// </summary>
                Float3PackedNormShortNorm4 = 2,

                /// <summary>
                /// Position in 3 floats.<br/>
                /// Normal with X in 11 bits, Y in 11 bits, and Z in 10 bits.<br/>
                /// Tangent with X in 11 bits, Y in 11 bits, and Z in 10 bits.<br/>
                /// BiNormal with X in 11 bits, Y in 11 bits, and Z in 10 bits.
                /// </summary>
                Float3PackedNorm3 = 3,

                /// <summary>
                /// A user defined format.
                /// </summary>
                Custom = 255
            }

            /// <summary>
            /// Processing flags for the geometry pipeline.
            /// </summary>
            public GeomConfigFlags Flags;

            /// <summary>
            /// The number of uniform tables.
            /// </summary>
            public byte UniformTableCount; // Subtract 1 when packing into byte to allow for values 1-16 in 4 bits.

            /// <summary>
            /// The max size of the command hole buffer needed for outputting graphics commands expressed in qwords.
            /// </summary>
            public byte CommandBufferHoleSize;

            /// <summary>
            /// The input SPU vertex stream's format ID. Custom formats are specified as 0xFF.
            /// </summary>
            public SpuVertexFormat InputVertexFormatId;

            /// <summary>
            /// The secondary SPU vertex stream's format ID. Can be any value if the stream is empty. Custom formats are specified as 0xFF.
            /// </summary>
            public SpuVertexFormat SecondaryInputVertexFormatId;

            /// <summary>
            /// The output RSX vertex stream's format ID. Custom formats are specified as 0xFF.
            /// </summary>
            public RsxVertexFormat OutputVertexFormatId;

            /// <summary>
            /// The blend shape vertex format. Custom formats are specified as 0xFF. Unused as of edge 0.4.2 (all blend shapes use custom formats).
            /// </summary>
            public byte VertexDeltaFormatId;

            /// <summary>
            /// How indexes for vertices are stored.
            /// </summary>
            public EdgeGeomIndexes IndexMode;

            /// <summary>
            /// The type of calculation for matrix palette skinning.
            /// </summary>
            public EdgeGeomSkin SkinningMode;

            /// <summary>
            /// The format ID for skinning matrices.<br/>
            /// This is ignored if no skinning is done.
            /// </summary>
            public EdgeGeomSkinningMatrixFormat SkinningMatrixFormat;

            /// <summary>
            /// The number of vertexes in the edge vertex buffer.
            /// </summary>
            public ushort NumVertexes;

            /// <summary>
            /// The number of indexes in the edge index buffer.
            /// </summary>
            public ushort NumIndexes;

            /// <summary>
            /// The offset to index data in RSX I/O.<br/>
            /// This can be used when the index buffer can be used without modification by things such as compression or culling.<br/>
            /// To use edge generated index buffers, set this to -1.
            /// </summary>
            public int IndexesOffset;

            /// <summary>
            /// Create a <see cref="EdgeGeomSpuConfigInfo"/> from standard read values.
            /// </summary>
            /// <param name="flagsAndUniformTableCount">The byte containing the flags and uniform table count, each in 4 bits.</param>
            /// <param name="commandBufferHoleSize">The command buffer hole size.</param>
            /// <param name="inputVertexFormatId">The input vertex format ID.</param>
            /// <param name="secondaryInputVertexFormatId">The secondary vertex format ID.</param>
            /// <param name="outputVertexFormatId">The output vertex format ID.</param>
            /// <param name="vertexDeltaFormatId">The vertex delta format ID. Unused as of edge 0.4.2.</param>
            /// <param name="indexesFlavorAndSkinningFlavor">The byte containing the indexes and skinning flavors, each in 4 bits.</param>
            /// <param name="skinningMatrixFormat">The skinning matrix format.</param>
            /// <param name="numVertexes">The number of vertices.</param>
            /// <param name="numIndexes">The number of indexes.</param>
            /// <param name="indexesOffset">The offset to index data in RSX I/O.</param>
            public EdgeGeomSpuConfigInfo(byte flagsAndUniformTableCount, byte commandBufferHoleSize, byte inputVertexFormatId,
                                         byte secondaryInputVertexFormatId, byte outputVertexFormatId, byte vertexDeltaFormatId,
                                         byte indexesFlavorAndSkinningFlavor, byte skinningMatrixFormat, ushort numVertexes, ushort numIndexes, int indexesOffset)
            {
                byte flagsAndUniformTableCountValues = flagsAndUniformTableCount;
                Flags = (GeomConfigFlags)(flagsAndUniformTableCountValues >> 4);
                UniformTableCount = (byte)((flagsAndUniformTableCountValues & 0b0000_1111) + 1);
                CommandBufferHoleSize = commandBufferHoleSize;
                InputVertexFormatId = (SpuVertexFormat)inputVertexFormatId;
                SecondaryInputVertexFormatId = (SpuVertexFormat)secondaryInputVertexFormatId;
                OutputVertexFormatId = (RsxVertexFormat)outputVertexFormatId;
                VertexDeltaFormatId = vertexDeltaFormatId;
                byte indexesFlavorAndSkinningFlavorValues = indexesFlavorAndSkinningFlavor;
                IndexMode = (EdgeGeomIndexes)(indexesFlavorAndSkinningFlavorValues >> 4);
                SkinningMode = (EdgeGeomSkin)(indexesFlavorAndSkinningFlavorValues & 0b0000_1111);
                SkinningMatrixFormat = (EdgeGeomSkinningMatrixFormat)skinningMatrixFormat;
                NumVertexes = numVertexes;
                NumIndexes = numIndexes;
                IndexesOffset = indexesOffset;
            }

            /// <summary>
            /// Create a <see cref="EdgeGeomSpuConfigInfo"/> from standard read values in a byte array.
            /// </summary>
            /// <param name="edgeGeomSpuConfigInfoBytes">The bytes of the <see cref="EdgeGeomSpuConfigInfo"/>.</param>
            /// <exception cref="InvalidDataException">The byte array must be 16 bytes in total.</exception>
            public EdgeGeomSpuConfigInfo(byte[] edgeGeomSpuConfigInfoBytes)
            {
                if (edgeGeomSpuConfigInfoBytes.Length != 16)
                {
                    throw new InvalidDataException($"{nameof(EdgeGeomSpuConfigInfo)} must be 16 bytes in total.");
                }

                byte flagsAndUniformTableCountValues = edgeGeomSpuConfigInfoBytes[0];
                Flags = (GeomConfigFlags)(flagsAndUniformTableCountValues >> 4);
                UniformTableCount = (byte)((flagsAndUniformTableCountValues & 0b0000_1111) + 1);
                CommandBufferHoleSize = edgeGeomSpuConfigInfoBytes[1];
                InputVertexFormatId = (SpuVertexFormat)edgeGeomSpuConfigInfoBytes[2];
                SecondaryInputVertexFormatId = (SpuVertexFormat)edgeGeomSpuConfigInfoBytes[3];
                OutputVertexFormatId = (RsxVertexFormat)edgeGeomSpuConfigInfoBytes[4];
                VertexDeltaFormatId = edgeGeomSpuConfigInfoBytes[5];
                byte indexesFlavorAndSkinningFlavorValues = edgeGeomSpuConfigInfoBytes[6];
                IndexMode = (EdgeGeomIndexes)(indexesFlavorAndSkinningFlavorValues >> 4);
                SkinningMode = (EdgeGeomSkin)(indexesFlavorAndSkinningFlavorValues & 0b0000_1111);
                SkinningMatrixFormat = (EdgeGeomSkinningMatrixFormat)edgeGeomSpuConfigInfoBytes[7];
                NumVertexes = BitConverterHelper.ToUInt16BigEndian(edgeGeomSpuConfigInfoBytes, 8);
                NumIndexes = BitConverterHelper.ToUInt16BigEndian(edgeGeomSpuConfigInfoBytes, 10);
                IndexesOffset = BitConverterHelper.ToInt32BigEndian(edgeGeomSpuConfigInfoBytes, 12);
            }

            /// <summary>
            /// Create a <see cref="EdgeGeomSpuConfigInfo"/> from standard read values.
            /// </summary>
            /// <param name="br">A <see cref="BinaryReaderEx"/>.</param>
            internal EdgeGeomSpuConfigInfo(BinaryReaderEx br)
            {
                byte flagsAndUniformTableCountValues = br.ReadByte();
                Flags = (GeomConfigFlags)(flagsAndUniformTableCountValues >> 4);
                UniformTableCount = (byte)((flagsAndUniformTableCountValues & 0b0000_1111) + 1);
                CommandBufferHoleSize = br.ReadByte();
                InputVertexFormatId = (SpuVertexFormat)br.ReadByte();
                SecondaryInputVertexFormatId = (SpuVertexFormat)br.ReadByte();
                OutputVertexFormatId = (RsxVertexFormat)br.ReadByte();
                VertexDeltaFormatId = br.ReadByte();
                byte indexesFlavorAndSkinningFlavorValues = br.ReadByte();
                IndexMode = (EdgeGeomIndexes)(indexesFlavorAndSkinningFlavorValues >> 4);
                SkinningMode = (EdgeGeomSkin)(indexesFlavorAndSkinningFlavorValues & 0b0000_1111);
                SkinningMatrixFormat = (EdgeGeomSkinningMatrixFormat)br.ReadByte();
                NumVertexes = br.ReadUInt16();
                NumIndexes = br.ReadUInt16();
                IndexesOffset = br.ReadInt32();
            }
        }
    }
}
