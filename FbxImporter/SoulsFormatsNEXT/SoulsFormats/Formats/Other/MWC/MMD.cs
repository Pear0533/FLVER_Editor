using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace SoulsFormats.Other.MWC
{
    /// <summary>
    /// Metal Wolf Chaos Map Model Data
    /// </summary>
    public class MMD : SoulsFile<MMD>
    {
        /// <summary>
        /// MMD Header data
        /// </summary>
        public MMDHeader header;
        /// <summary>
        /// Mesh headers List
        /// </summary>
        public List<MeshHeader> meshHeaders = new List<MeshHeader>();
        /// <summary>
        /// Vertices List
        /// </summary>
        public List<Vertex> vertices = new List<Vertex>();
        /// <summary>
        /// Face Indices List
        /// </summary>
        public List<ushort> faceIndices = new List<ushort>();
        /// <summary>
        /// Bones list
        /// </summary>
        public List<Bone> bones = new List<Bone>();
        /// <summary>
        /// Texture names list
        /// </summary>
        public List<string> texNames = new List<string>();
        /// <summary>
        /// MMD Base Constructor
        /// </summary>
        public MMD() { }

        /// <summary>
        /// MMD BinaryReaderEx Constructor
        /// </summary>
        public MMD(BinaryReaderEx br)
        {
            Read(br);
        }

        /// <summary>
        /// Method to determine if this is indeed an MMD
        /// </summary>
        protected override bool Is(BinaryReaderEx br)
        {
            if (br.Length < 8)
                return false;

            br.ReadInt32();
            string magic = br.GetASCII(4);
            return magic == "MMD ";
        }

        /// <summary>
        /// Method to read MMD
        /// </summary>
        protected override void Read(BinaryReaderEx br)
        {
            header = new MMDHeader();
            br.ReadInt32(); //Filesize
            br.AssertASCII("MMD ");
            header.usht_08 = br.ReadUInt16();
            header.usht_0A = br.ReadUInt16();
            header.int_0C = br.ReadInt32();

            header.meshCount = br.ReadInt32();
            header.faceIndexCount = br.ReadInt32();
            header.vertexCount = br.ReadInt32();
            header.nodeCount = br.ReadInt32();

            header.textureCount = br.ReadInt32();
            var meshHeaderOffset = br.ReadInt32();
            var faceIndicesOffset = br.ReadInt32();
            var vertexDataOffset = br.ReadInt32();

            var boneOffset = br.ReadInt32();
            var textureNameOffset = br.ReadInt32();
            header.unkCount3 = br.ReadInt32();
            header.unkCount4 = br.ReadInt32();

            br.Position = meshHeaderOffset;
            for (int i = 0; i < header.meshCount; i++)
            {
                MeshHeader msh = new MeshHeader();
                msh.materialId = br.ReadInt32();
                msh.int_04 = br.ReadInt32();
                msh.faceIndexCount = br.ReadInt32();
                msh.faceIndexStart = br.ReadInt32();
                meshHeaders.Add(msh);
            }

            br.Position = faceIndicesOffset;
            for (int i = 0; i < header.faceIndexCount; i++)
            {
                faceIndices.Add(br.ReadUInt16());
            }

            br.Position = vertexDataOffset;
            for (int i = 0; i < header.vertexCount; i++)
            {
                vertices.Add(new Vertex(br));
            }

            br.Position = boneOffset;
            for (int i = 0; i < header.nodeCount; i++)
            {
                var bone = new Bone();
                bone.int00 = br.ReadInt32();
                bone.int04 = br.ReadInt32();
                bone.int08 = br.ReadInt32();
                bone.int0C = br.ReadInt32();

                bone.int10 = br.ReadInt32();
                bone.int14 = br.ReadInt32();
                bone.int18 = br.ReadInt32();
                bone.int1C = br.ReadInt32();

                bone.translation = br.ReadVector3();
                bone.extraFloat0 = br.ReadSingle();
                bone.rotation = br.ReadQuaternion();
                bone.scale = br.ReadVector3();
                bone.extraFloat1 = br.ReadSingle();

                bone.int50 = br.ReadInt32();
                bone.int54 = br.ReadInt32();
                bone.int58 = br.ReadInt32();
                bone.int5C = br.ReadInt32();

                bone.flt60 = br.ReadInt32();
                bone.int64 = br.ReadInt32();
                bone.int68 = br.ReadInt32();
                bone.int6C = br.ReadInt32();

                bone.int70 = br.ReadInt32();
                bone.int74 = br.ReadInt32();
                bone.int78 = br.ReadInt32();
                bone.int7C = br.ReadInt32();

                bones.Add(bone);
            }

            br.Position = textureNameOffset;
            for (int i = 0; i < header.textureCount; i++)
            {
                texNames.Add(br.ReadASCII());
            }
        }

        /// <summary>
        /// Structure of the MMDHeader
        /// </summary>
        public struct MMDHeader
        {
            /// <summary>
            /// Unknown
            /// </summary>
            public ushort usht_08;
            /// <summary>
            /// Unknown
            /// </summary>
            public ushort usht_0A;
            /// <summary>
            /// Unknown
            /// </summary>
            public int int_0C;

            /// <summary>
            ///  Amount of meshes in this model
            /// </summary>
            public int meshCount;
            /// <summary>
            /// Amount of face indices in this model
            /// </summary>
            public int faceIndexCount;
            /// <summary>
            /// Amount of vertices in this model
            /// </summary>
            public int vertexCount;
            /// <summary>
            /// Nodes used in this model
            /// </summary>
            public int nodeCount;

            /// <summary>
            /// Textures used in this model
            /// </summary>
            public int textureCount;

            /// <summary>
            /// Unknown
            /// </summary>
            public int unkCount3;
            /// <summary>
            /// Unknown
            /// </summary>
            public int unkCount4;
        }

        /// <summary>
        /// Structure of a mesh header in an MMD
        /// </summary>
        public struct MeshHeader
        {
            /// <summary>
            /// Mesh's Material Id
            /// </summary>
            public int materialId;
            /// <summary>
            /// Unknown
            /// </summary>
            public int int_04;
            /// <summary>
            /// Face indices count
            /// </summary>
            public int faceIndexCount;
            /// <summary>
            /// Starting face index in the larger index list
            /// </summary>
            public int faceIndexStart;
        }

        /// <summary>
        /// Vertex in an MMD
        /// </summary>
        public class Vertex
        {
            /// <summary>
            /// These positions are all VERY large for some reason. 
            /// Based on the white house model in m000, the first mmd model, we can estimate that these positions are about 2000x the scale they should be.
            /// </summary>
            public virtual Vector3 Position { get; set; }
            /// <summary>
            /// Vertex normal, probably
            /// </summary>
            public virtual Vector3 Normal { get; set; }
            /// <summary>
            /// Vertex weight indices
            /// </summary>
            public byte[] WeightIndices { get; set; }
            /// <summary>
            /// Vertex weights
            /// </summary>
            public byte[] Weights { get; set; }      

            /// <summary>
            /// UVs, seemiingly 4 of these per vertex.
            /// </summary>
            public Vector2[] UVs;
            /// <summary>
            /// Vertex color
            /// </summary>
            public Color Color;

            public Vertex()
            {
                UVs = new Vector2[4];
            }

            internal Vertex(BinaryReaderEx br)
            {
                Position = br.ReadVector3();
                Normal = br.Read11_11_10Vector3();
                WeightIndices = br.ReadBytes(4);
                Weights = br.ReadBytes(4);
                UVs = new Vector2[4];
                for (int i = 0; i < 4; i++)
                    UVs[i] = br.ReadVector2();
                Color = br.ReadRGBA();

            }
        }

        /// <summary>
        /// Structure of a node in an MMD
        /// </summary>
        public struct Bone
        {
            /// <summary>
            /// Unknown
            /// </summary>
            public int int00;
            /// <summary>
            /// Unknown
            /// </summary>
            public int int04;
            /// <summary>
            /// Unknown
            /// </summary>
            public int int08;
            /// <summary>
            /// Unknown
            /// </summary>
            public int int0C;

            /// <summary>
            /// Unknown
            /// </summary>
            public int int10;
            /// <summary>
            /// Unknown
            /// </summary>
            public int int14;
            /// <summary>
            /// Unknown
            /// </summary>
            public int int18;
            /// <summary>
            /// Unknown
            /// </summary>
            public int int1C; //Always 0xFFFFFFFF?
            
            /// <summary>
            /// Translation of bone
            /// </summary>
            public Vector3 translation;
            /// <summary>
            /// Padding
            /// </summary>
            public float extraFloat0;
            /// <summary>
            /// Rotation of bone as a quaternion
            /// </summary>
            public Quaternion rotation;
            /// <summary>
            /// Scale of bone
            /// </summary>
            public Vector3 scale;
            /// <summary>
            /// Padding
            /// </summary>
            public float extraFloat1;

            /// <summary>
            /// Unknown
            /// </summary>
            public int int50;
            /// <summary>
            /// Unknown
            /// </summary>
            public int int54;
            /// <summary>
            /// Unknown
            /// </summary>
            public int int58;
            /// <summary>
            /// Unknown
            /// </summary>
            public int int5C;

            /// <summary>
            /// Unknown
            /// </summary>
            public float flt60;
            /// <summary>
            /// Unknown
            /// </summary>
            public int int64;
            /// <summary>
            /// Unknown
            /// </summary>
            public int int68;
            /// <summary>
            /// Unknown
            /// </summary>
            public int int6C;

            /// <summary>
            /// Unknown
            /// </summary>
            public int int70;
            /// <summary>
            /// Unknown
            /// </summary>
            public int int74;
            /// <summary>
            /// Unknown
            /// </summary>
            public int int78;
            /// <summary>
            /// Unknown
            /// </summary>
            public int int7C;
        }
    }
}
