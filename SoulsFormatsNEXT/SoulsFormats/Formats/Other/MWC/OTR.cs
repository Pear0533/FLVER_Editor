using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace SoulsFormats.Other.MWC
{
    /// <summary>
    /// Collision model. Might also be used for navmesh type handling
    /// </summary>
    public class OTR : SoulsFile<OTR>
    {
        /// <summary>
        /// OTR Header
        /// </summary>
        public OTRHeader header;
        /// <summary>
        /// OTR Bounding box data
        /// </summary>
        public BoundingBox bounding = null;
        /// <summary>
        /// OTR Faces List
        /// </summary>
        public List<Face> faces = new List<Face>();
        /// <summary>
        /// OTR Vertices List
        /// </summary>
        public List<Vertex> vertices = new List<Vertex>();
        /// <summary>
        /// OTR Default constructor
        /// </summary>
        public OTR() { }
        /// <summary>
        /// OTR BinaryReaderEx constructor
        /// </summary>
        public OTR(BinaryReaderEx br)
        {
            Read(br);
        }

        /// <summary>
        /// OTR BinaryReaderEx Read
        /// </summary>
        protected override void Read(BinaryReaderEx br)
        {
            header = new OTRHeader() { int00 = br.ReadInt32(), boundingBoxCount = br.ReadInt32(), faceCount = br.ReadInt32(), vertCount = br.ReadInt32() };

            bounding = new BoundingBox(br);
            br.Position = header.boundingBoxCount * 0x64 + 0x10;

            for (int i = 0; i < header.faceCount; i++)
            {
                faces.Add(new Face()
                {
                    vertIndex0 = br.ReadInt32(),
                    vertIndex1 = br.ReadInt32(),
                    vertIndex2 = br.ReadInt32(),
                    normal = br.ReadVector3(),
                    min = br.ReadVector3(),
                    max = br.ReadVector3(),
                    unkInt0 = br.ReadInt32(),
                    unkInt1 = br.ReadInt32(),
                    unkInt2 = br.ReadInt32(),
                    unkFlt = br.ReadSingle(),
                });
            }

            for (int i = 0; i < header.vertCount; i++)
            {
                vertices.Add(new Vertex()
                {
                    Position = br.ReadVector3(),
                    unkInt = br.ReadInt32(),
                    Color = br.ReadRGBA()
                });
            }
        }

        /// <summary>
        /// OTR Header Structure
        /// </summary>
        public struct OTRHeader
        {
            /// <summary>
            /// Unknown
            /// </summary>
            public int int00;
            /// <summary>
            /// Bounding box count
            /// </summary>
            public int boundingBoxCount;
            /// <summary>
            /// Face count
            /// </summary>
            public int faceCount;
            /// <summary>
            /// Vertex Count
            /// </summary>
            public int vertCount;
        }

        /// <summary>
        /// OTR Bounding Box
        /// </summary>
        public class BoundingBox
        {
            /// <summary>
            /// Bounding box default constructor
            /// </summary>
            public BoundingBox() { }
            /// <summary>
            /// Bounding box BinaryReaderEx constructor
            /// </summary>
            public BoundingBox(BinaryReaderEx br)
            {
                minBounding = br.ReadVector3();
                maxBounding = br.ReadVector3();
                int18 = br.ReadInt32();
                int1C = br.ReadInt32();

                int20 = br.ReadInt32();
                int24 = br.ReadInt32();
                int28 = br.ReadInt32();
                int2C = br.ReadInt32();

                int30 = br.ReadInt32();
                int34 = br.ReadInt32();
                int38 = br.ReadInt32();
                unkCount1 = br.ReadInt32();

                boxOffset0 = br.ReadInt32();
                boxOffset1 = br.ReadInt32();
                boxOffset2 = br.ReadInt32();
                boxOffset3 = br.ReadInt32();

                unkBt0 = br.ReadByte();
                unkBt1 = br.ReadByte();
                unkBt2 = br.ReadByte();
                unkBt3 = br.ReadByte();
                int54 = br.ReadInt32();
                unk58 = br.ReadInt32();
                unk5C = br.ReadInt32();

                unk60 = br.ReadInt32();

                if (boxOffset0 != 0)
                {
                    box0 = new BoundingBox(br);
                }
                if (boxOffset1 != 0)
                {
                    box1 = new BoundingBox(br);
                }
                if (boxOffset2 != 0)
                {
                    box2 = new BoundingBox(br);
                }
                if (boxOffset3 != 0)
                {
                    box3 = new BoundingBox(br);
                }
            }

            /// <summary>
            /// Bounding box 0
            /// </summary>
            public BoundingBox box0 = null;
            /// <summary>
            /// Bounding box 1
            /// </summary>
            public BoundingBox box1 = null;
            /// <summary>
            /// Bounding box 2
            /// </summary>
            public BoundingBox box2 = null;
            /// <summary>
            /// Bounding box 3
            /// </summary>
            public BoundingBox box3 = null;

            /// <summary>
            /// Minimum bounding of current bounding box
            /// </summary>
            public Vector3 minBounding;
            /// <summary>
            /// Maximum bounding of current bounding box
            /// </summary>
            public Vector3 maxBounding;
            /// <summary>
            /// Unknown
            /// </summary>
            public int int18;
            /// <summary>
            /// Unknown
            /// </summary>
            public int int1C;

            /// <summary>
            /// Unknown
            /// </summary>
            public int int20;
            /// <summary>
            /// Unknown
            /// </summary>
            public int int24;
            /// <summary>
            /// Unknown
            /// </summary>
            public int int28;
            /// <summary>
            /// Unknown
            /// </summary>
            public int int2C;

            /// <summary>
            /// Unknown
            /// </summary>
            public int int30;
            /// <summary>
            /// Unknown
            /// </summary>
            public int int34;
            /// <summary>
            /// Unknown
            /// </summary>
            public int int38;
            /// <summary>
            /// Unknown count
            /// </summary>
            public int unkCount1;

            /// <summary>
            /// Bounding box 0 offset
            /// </summary>
            public int boxOffset0;
            /// <summary>
            /// Bounding box 1 offset
            /// </summary>
            public int boxOffset1;
            /// <summary>
            /// Bounding box 2 offset
            /// </summary>
            public int boxOffset2;
            /// <summary>
            /// Bounding box 3 offset
            /// </summary>
            public int boxOffset3;

            /// <summary>
            /// Unknown
            /// </summary>
            public byte unkBt0;
            /// <summary>
            /// Unknown
            /// </summary>
            public byte unkBt1;
            /// <summary>
            /// Unknown
            /// </summary>
            public byte unkBt2;
            /// <summary>
            /// Unknown
            /// </summary>
            public byte unkBt3;
            /// <summary>
            /// Unknown
            /// </summary>
            public int int54;
            /// <summary>
            /// Unknown
            /// </summary>
            public int unk58;
            /// <summary>
            /// Unknown
            /// </summary>
            public int unk5C;

            /// <summary>
            /// Unknown
            /// </summary>
            public int unk60;
        }

        /// <summary>
        /// OTR Face
        /// </summary>
        public struct Face
        {
            /// <summary>
            /// Vert 0 id
            /// </summary>
            public int vertIndex0;
            /// <summary>
            /// Vert 1 id
            /// </summary>
            public int vertIndex1;
            /// <summary>
            /// Vert 2 id
            /// </summary>
            public int vertIndex2;
            /// <summary>
            /// Face normal
            /// </summary>
            public Vector3 normal;
            /// <summary>
            /// Minimum values of all vert positions in face
            /// </summary>
            public Vector3 min;
            /// <summary>
            /// Maximum values of all vert positions in face
            /// </summary>
            public Vector3 max;

            /// <summary>
            /// Unknown
            /// </summary>
            public int unkInt0;
            /// <summary>
            /// Unknown
            /// </summary>
            public int unkInt1;
            /// <summary>
            /// Unknown
            /// </summary>
            public int unkInt2;
            /// <summary>
            /// Unknown floating point value
            /// </summary>
            public float unkFlt;
        }

        /// <summary>
        /// OTR Vertex
        /// </summary>
        public struct Vertex
        {
            /// <summary>
            /// Vertex Position
            /// </summary>
            public Vector3 Position;
            /// <summary>
            /// Unknown
            /// </summary>
            public int unkInt;
            /// <summary>
            /// Vertex color
            /// </summary>
            public Color Color;
        }
    }
}
