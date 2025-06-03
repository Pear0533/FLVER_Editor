using System.Numerics;

namespace SoulsFormats.Other
{
    /// <summary>
    /// A 3D model format used in early PS3/X360 games. Extension: .mdl
    /// </summary>
    public partial class MDL4
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
            /// An unknown set of indices.<br/>
            /// These may be indices to bones for facial expressions.
            /// </summary>
            public short[] UnkIndices { get; set; }

            /// <summary>
            /// Create a new <see cref="Node"/> with default values.
            /// </summary>
            public Node()
            {
                Name = string.Empty;
                ParentIndex = -1;
                FirstChildIndex = -1;
                NextSiblingIndex = -1;
                PreviousSiblingIndex = -1;
                Scale = Vector3.One;
                UnkIndices = new short[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
            }

            /// <summary>
            /// Read a <see cref="Node"/> from a stream.
            /// </summary>
            /// <param name="br">The stream reader.</param>
            internal Node(BinaryReaderEx br)
            {
                Name = br.ReadFixStr(0x20);
                Translation = br.ReadVector3();
                Rotation = br.ReadVector3();
                Scale = br.ReadVector3();
                BoundingBoxMin = br.ReadVector3();
                BoundingBoxMax = br.ReadVector3();
                ParentIndex = br.ReadInt16();
                FirstChildIndex = br.ReadInt16();
                NextSiblingIndex = br.ReadInt16();
                PreviousSiblingIndex = br.ReadInt16();
                br.AssertInt32(0);
                br.AssertInt32(0);
                br.AssertInt32(0);
                UnkIndices = br.ReadInt16s(16);
            }

            /// <summary>
            /// Write this <see cref="Node"/> to a stream.
            /// </summary>
            /// <param name="bw">The stream writer.</param>
            internal void Write(BinaryWriterEx bw)
            {
                bw.WriteFixStr(Name, 0x20);
                bw.WriteVector3(Translation);
                bw.WriteVector3(Rotation);
                bw.WriteVector3(Scale);
                bw.WriteVector3(BoundingBoxMin);
                bw.WriteVector3(BoundingBoxMax);
                bw.WriteInt16(ParentIndex);
                bw.WriteInt16(FirstChildIndex);
                bw.WriteInt16(NextSiblingIndex);
                bw.WriteInt16(PreviousSiblingIndex);
                bw.WriteInt32(0);
                bw.WriteInt32(0);
                bw.WriteInt32(0);
                bw.WriteInt16s(UnkIndices);
            }

            /// <summary>
            /// Creates a transformation matrix from the scale, rotation, and translation of this <see cref="Node"/>.
            /// </summary>
            public Matrix4x4 ComputeLocalTransform()
            {
                return Matrix4x4.CreateScale(Scale)
                    * Matrix4x4.CreateRotationX(Rotation.X)
                    * Matrix4x4.CreateRotationZ(Rotation.Z)
                    * Matrix4x4.CreateRotationY(Rotation.Y)
                    * Matrix4x4.CreateTranslation(Translation);
            }
        }
    }
}
