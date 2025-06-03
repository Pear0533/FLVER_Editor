using System.Numerics;

namespace SoulsFormats
{
    public partial class SMD4
    {
        /// <summary>
        /// A joint available for vertices to be attached to.
        /// </summary>
        public class Node
        {
            /// <summary>
            /// The name of this <see cref="Node"/>.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// The translation of this <see cref="Node"/>.
            /// </summary>
            public Vector3 Translation { get; set; }

            /// <summary>
            /// The rotation of this <see cref="Node"/>; euler radians in XZY order.
            /// </summary>
            public Vector3 Rotation { get; set; }

            /// <summary>
            /// The scale of this <see cref="Node"/>.
            /// </summary>
            public Vector3 Scale { get; set; }

            /// <summary>
            /// The minimum extent of the vertices weighted to this <see cref="Node"/>.
            /// </summary>
            public Vector3 BoundingBoxMin { get; set; }

            /// <summary>
            /// The maximum extent of the vertices weighted to this <see cref="Node"/>.
            /// </summary>
            public Vector3 BoundingBoxMax { get; set; }

            /// <summary>
            /// The index of the parent of this <see cref="Node"/>, or -1 for none.
            /// </summary>
            public short ParentIndex { get; set; }

            /// <summary>
            /// The index of the first child of this <see cref="Node"/>, or -1 for none.
            /// </summary>
            public short FirstChildIndex { get; set; }

            /// <summary>
            /// The index of the next child of the parent of this <see cref="Node"/>, or -1 for none.
            /// </summary>
            public short NextSiblingIndex { get; set; }

            /// <summary>
            /// The index of the previous child of the parent of this <see cref="Node"/>, or -1 for none.
            /// </summary>
            public short PreviousSiblingIndex { get; set; }

            /// <summary>
            /// Unknown; Only seen as zero.
            /// </summary>
            public int Unk64 { get; set; }

            /// <summary>
            /// Unknown; Only seen as zero.
            /// </summary>
            public int Unk68 { get; set; }

            /// <summary>
            /// Unknown; Only seen as zero.
            /// </summary>
            public int Unk6C { get; set; }

            /// <summary>
            /// Unknown array of 8 indices; Only seen as -1 for each.
            /// </summary>
            public int[] Unk70 { get; private set; }

            /// <summary>
            /// Create a new <see cref="Node"/> with default values.
            /// </summary>
            public Node()
            {
                Name = string.Empty;
                Scale = Vector3.One;
                ParentIndex = -1;
                FirstChildIndex = -1;
                NextSiblingIndex = -1;
                PreviousSiblingIndex = -1;
                Unk64 = 0;
                Unk68 = 0;
                Unk6C = 0;
                Unk70 = new int[] { -1, -1, -1, -1, -1, -1, -1, -1 };
            }

            /// <summary>
            /// Clone an existing <see cref="Node"/>.
            /// </summary>
            public Node(Node bone)
            {
                Name = bone.Name;
                Translation = bone.Translation;
                Rotation = bone.Rotation;
                Scale = bone.Scale;
                BoundingBoxMin = bone.BoundingBoxMin;
                BoundingBoxMax = bone.BoundingBoxMax;
                ParentIndex = bone.ParentIndex;
                FirstChildIndex = bone.FirstChildIndex;
                NextSiblingIndex = bone.NextSiblingIndex;
                PreviousSiblingIndex = bone.PreviousSiblingIndex;
                Unk64 = bone.Unk64;
                Unk68 = bone.Unk68;
                Unk6C = bone.Unk6C;
                Unk70 = new int[8];
                for (int i = 0; i < 8; i++)
                    Unk70[i] = bone.Unk70[i];
            }

            /// <summary>
            /// Read a <see cref="Node"/> from a stream.
            /// </summary>
            /// <param name="br">The stream reader.</param>
            internal Node(BinaryReaderEx br)
            {
                Name = br.ReadASCII(32);
                Translation = br.ReadVector3();
                Rotation = br.ReadVector3();
                Scale = br.ReadVector3();
                BoundingBoxMin = br.ReadVector3();
                BoundingBoxMax = br.ReadVector3();
                ParentIndex = br.ReadInt16();
                FirstChildIndex = br.ReadInt16();
                NextSiblingIndex = br.ReadInt16();
                PreviousSiblingIndex = br.ReadInt16();
                Unk64 = br.ReadInt32();
                Unk68 = br.ReadInt32();
                Unk6C = br.ReadInt32();
                Unk70 = br.ReadInt32s(8);
            }

            /// <summary>
            /// Write a <see cref="Node"/> to a stream.
            /// </summary>
            /// <param name="bw">The stream writer.</param>
            internal void Write(BinaryWriterEx bw)
            {
                bw.WriteFixStr(Name, 32);
                bw.WriteVector3(Translation);
                bw.WriteVector3(Rotation);
                bw.WriteVector3(Scale);
                bw.WriteVector3(BoundingBoxMin);
                bw.WriteVector3(BoundingBoxMax);
                bw.WriteInt16(ParentIndex);
                bw.WriteInt16(FirstChildIndex);
                bw.WriteInt16(NextSiblingIndex);
                bw.WriteInt16(PreviousSiblingIndex);
                bw.WriteInt32(Unk64);
                bw.WriteInt32(Unk68);
                bw.WriteInt32(Unk6C);
                bw.WriteInt32s(Unk70);
            }

            /// <summary>
            /// Creates a transformation matrix from the scale, rotation, and translation of the bone.
            /// </summary>
            public Matrix4x4 ComputeLocalTransform()
            {
                return Matrix4x4.CreateScale(Scale)
                    * Matrix4x4.CreateRotationX(Rotation.X)
                    * Matrix4x4.CreateRotationZ(Rotation.Z)
                    * Matrix4x4.CreateRotationY(Rotation.Y)
                    * Matrix4x4.CreateTranslation(Translation);
            }

            /// <inheritdoc/>
            public override string ToString()
            {
                return Name;
            }
        }
    }
}
