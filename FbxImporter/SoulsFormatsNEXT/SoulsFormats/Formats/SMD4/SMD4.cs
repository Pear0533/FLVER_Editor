using System.Collections.Generic;
using System.Numerics;

namespace SoulsFormats
{
    /// <summary>
    /// A shadow mesh model format in Armored Core 4thgen and 5thgen games.
    /// </summary>
    public partial class SMD4 : SoulsFile<SMD4>
    {
        /// <summary>
        /// General values for this model.
        /// </summary>
        public SMDHeader Header { get; set; }

        /// <summary>
        /// Unknown.
        /// </summary>
        public List<Unk10> Unk10s { get; set; }

        /// <summary>
        /// Joints available for vertices to be attached to.
        /// </summary>
        public List<Node> Nodes { get; set; }

        /// <summary>
        /// Individual chunks of the model.
        /// </summary>
        public List<Mesh> Meshes { get; set; }

        /// <summary>
        /// Create a new <see cref="SMD4"/>.
        /// </summary>
        public SMD4()
        {
            Header = new SMDHeader();
            Unk10s = new List<Unk10>();
            Nodes = new List<Node>();
            Meshes = new List<Mesh>();
        }

        /// <summary>
        /// Clone an existing <see cref="SMD4"/>.
        /// </summary>
        public SMD4(SMD4 smd)
        {
            Header = new SMDHeader();
            Unk10s = new List<Unk10>();
            Nodes = new List<Node>();
            Meshes = new List<Mesh>();

            Header.Version = smd.Header.Version;
            Header.BoundingBoxMin = smd.Header.BoundingBoxMin;
            Header.BoundingBoxMax = smd.Header.BoundingBoxMax;

            for (int i = 0; i < smd.Unk10s.Count; i++)
                Unk10s.Add(new Unk10(smd.Unk10s[i]));
            foreach (Node bone in smd.Nodes)
                Nodes.Add(new Node(bone));
            foreach (Mesh mesh in smd.Meshes)
                Meshes.Add(new Mesh(mesh));
        }

        /// <summary>
        /// Returns true if the data appears to be an <see cref="SMD4"/> model.
        /// </summary>
        protected override bool Is(BinaryReaderEx br)
        {
            if (br.Length < 128)
                return false;
            return br.ReadASCII(4) == "SMD4";
        }

        /// <summary>
        /// Read an <see cref="SMD4"/> from a stream.
        /// </summary>
        /// <param name="br">The stream reader.</param>
        protected override void Read(BinaryReaderEx br)
        {
            br.BigEndian = true;
            br.AssertASCII("SMD4");
            Header = new SMDHeader();

            Header.Version = br.ReadInt32();
            int dataOffset = br.ReadInt32();
            int dataSize = br.ReadInt32();
            int countUnk10 = br.ReadInt32();
            int boneCount = br.ReadInt32();
            int meshCount = br.ReadInt32();
            br.AssertInt32(meshCount); // Vertex Buffer Count?

            Header.BoundingBoxMin = br.ReadVector3();
            Header.BoundingBoxMax = br.ReadVector3();
            int trueFaceCount = br.ReadInt32();
            int totalFaceCount = br.ReadInt32();
            br.AssertPattern(32, 0);

            Unk10s = new List<Unk10>();
            Nodes = new List<Node>();
            Meshes = new List<Mesh>();

            for (int i = 0; i < countUnk10; i++)
            {
                Unk10s.Add(new Unk10(br));
            }

            for (int i = 0; i < boneCount; i++)
                Nodes.Add(new Node(br));
            for (int i = 0; i < meshCount; i++)
                Meshes.Add(new Mesh(br, dataOffset, Header.Version));
        }

        /// <summary>
        /// Write an <see cref="SMD4"/> to a stream.
        /// </summary>
        /// <param name="bw">The stream writer.</param>
        protected override void Write(BinaryWriterEx bw)
        {
            bw.BigEndian = true;
            bw.WriteASCII("SMD4", false);
            bw.WriteInt32(Header.Version);
            bw.ReserveInt32("DataOffset");
            bw.ReserveInt32("DataSize");
            bw.WriteInt32(Unk10s.Count);
            bw.WriteInt32(Nodes.Count);
            bw.WriteInt32(Meshes.Count);
            bw.WriteInt32(Meshes.Count); // Vertex Buffer Count?

            bw.WriteVector3(Header.BoundingBoxMin);
            bw.WriteVector3(Header.BoundingBoxMax);

            int faceCount = 0;
            foreach (var mesh in Meshes)
                faceCount += mesh.GetFaceCount(true);

            int indexCount = faceCount * 3;
            bw.WriteInt32(faceCount); // Not entirely accurate but oh well
            bw.WriteInt32(indexCount); // Not entirely accurate but oh well
            bw.WritePattern(32, 0);

            for (int i = 0; i < Unk10s.Count; i++)
                Unk10s[i].Write(bw);
            foreach (Node bone in Nodes)
                bone.Write(bw);
            for (int i = 0; i < Meshes.Count; i++)
                Meshes[i].Write(bw, i, Header.Version);

            // Fill Data
            bw.Pad(0x800);
            int dataStart = (int)bw.Position;
            bw.FillInt32("DataOffset", dataStart);
            for (int i = 0; i < Meshes.Count; i++)
            {
                Mesh mesh = Meshes[i];
                bw.FillInt32($"VertexIndicesOffset_{i}", (int)bw.Position - dataStart);
                bw.WriteUInt16s(mesh.Indices);
                bw.Pad(0x10);

                bw.FillInt32($"VertexBufferOffset_{i}", (int)bw.Position - dataStart);
                foreach (Vertex vertex in mesh.Vertices)
                    vertex.Write(bw, Header.Version, mesh.VertexFormat);
            }
            bw.Pad(0x800);

            int dataEnd = (int)bw.Position;
            bw.FillInt32("DataSize", dataEnd - dataStart);
        }

        /// <summary>
        /// Compute the world transform for a bone.
        /// </summary>
        /// <param name="index">The index of the bone to compute the world transform of.</param>
        /// <returns>A matrix representing the world transform of the bone.</returns>
        public Matrix4x4 ComputeBoneWorldMatrix(int index)
        {
            var bone = Nodes[index];
            Matrix4x4 matrix = bone.ComputeLocalTransform();
            while (bone.ParentIndex != -1)
            {
                bone = Nodes[bone.ParentIndex];
                matrix *= bone.ComputeLocalTransform();
            }

            return matrix;
        }

        /// <summary>
        /// Compute the world transform for a bone.
        /// </summary>
        /// <param name="bone">The bone to compute the world transform of.</param>
        /// <returns>A matrix representing the world transform of the bone.</returns>
        public Matrix4x4 ComputeBoneWorldMatrix(Node bone)
        {
            Matrix4x4 matrix = bone.ComputeLocalTransform();
            while (bone.ParentIndex != -1)
            {
                bone = Nodes[bone.ParentIndex];
                matrix *= bone.ComputeLocalTransform();
            }

            return matrix;
        }

        /// <summary>
        /// An <see cref="SMD4"/> header containing general values for this model.
        /// </summary>
        public class SMDHeader
        {
            /// <summary>
            /// Version of the format indicating presence of various features.
            /// </summary>
            public int Version { get; set; }

            /// <summary>
            /// Minimum extent of the entire model.
            /// </summary>
            public Vector3 BoundingBoxMin { get; set; }

            /// <summary>
            /// Maximum extent of the entire model.
            /// </summary>
            public Vector3 BoundingBoxMax { get; set; }

            /// <summary>
            /// Create a new <see cref="SMDHeader"/>.
            /// </summary>
            public SMDHeader()
            {
                Version = 0x40001;
            }
        }
    }
}
