using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace SoulsFormats
{
    /// <summary>
    /// Armored Core For Answer animations.
    /// </summary>
	/// <remarks>
	/// Extremely poor and incomplete support at this time.
	/// </remarks>
    public class ANI : SoulsFile<ANI>
    {
        /// <summary>
        /// Bones in the animation.
        /// </summary>
        public List<Bone> Bones { get; set; }

        /// <summary>
        /// The positions of each bone for every frame.
        /// </summary>
        public List<Vector3> Positions { get; set; }

        /// <summary>
        /// The rotations of each bone for every frame.
        /// </summary>
        public List<Vector3> Rotations { get; set; }

        /// <summary>
        /// Deserializes file data from a stream.
        /// </summary>
        protected override void Read(BinaryReaderEx br)
        {
            br.BigEndian = true;
            br.AssertInt32(0x20051014);
            br.AssertInt32(0);
            br.ReadInt32(); // Frame Count
            int bonesOffset = br.ReadInt32();
            int boneCount = br.ReadInt32();
            int positionsOffset = br.ReadInt32();
            int rotationsOffset = br.ReadInt32();
            int positionCount = br.ReadInt32();
            int rotationCount = br.ReadInt32();
            int dataSize = br.ReadInt32();

            if (!(dataSize == br.Length || dataSize < br.Length))
                throw new InvalidDataException("Data size value was greater than stream size.");

            // Assert any extra bytes are null
            if (dataSize < br.Length)
            {
                br.StepIn(dataSize);
                br.AssertPattern((int)br.Length - dataSize, 0);
                br.StepOut();
            }

            br.AssertInt32(0);
            br.AssertInt32(1);
            br.AssertByte(1);
            br.AssertByte(1);
            br.AssertPattern(70, 0);

            Positions = new List<Vector3>(positionCount);
            Rotations = new List<Vector3>(rotationCount);
            Bones = new List<Bone>(boneCount);

            br.StepIn(positionsOffset);
            for (int i = 0; i < positionCount; i++)
                Positions.Add(br.ReadVector3());
            br.StepOut();

            br.StepIn(rotationsOffset);
            for (int i = 0; i < rotationCount; i++)
                Rotations.Add(ReadVector3Short(br));
            br.StepOut();

            br.StepIn(bonesOffset);
            for (int i = 0; i < boneCount; i++)
                Bones.Add(new Bone(br));
            br.StepOut();
        }

        /// <summary>
        /// Serializes file data to a stream.
        /// </summary>
        protected override void Write(BinaryWriterEx bw)
        {
            bw.BigEndian = true;
            bw.WriteInt32(0x20051014);
            bw.WriteInt32(0);
            bw.WriteInt32(GetKeyFrameCount());
            bw.WriteInt32(120); // AnimEntryOffset
            bw.WriteInt32(Bones.Count);
            bw.ReserveInt32("PositionsOffset");
            bw.ReserveInt32("RotationsOffset");
            bw.WriteInt32(Positions.Count);
            bw.WriteInt32(Rotations.Count);
            bw.ReserveInt32("DataSize");
            bw.WriteInt32(0);
            bw.WriteInt32(1);
            bw.WriteByte(1);
            bw.WriteByte(1);
            bw.WritePattern(70, 0);

            for (int i = 0; i < Bones.Count; i++)
                Bones[i].Write(bw, i);

            for (int i = 0; i < Bones.Count; i++)
            {
                bw.FillInt32($"BoneNameOffset_{i}", (int)bw.Position);
                bw.WriteShiftJIS(Bones[i].Name, true);
                if (Bones[i].Frames != null)
                {
                    bw.FillInt32($"AnimationOffset_{i}", (int)bw.Position);
                    Bones[i].Frames.Write(bw);
                }
            }

            bw.FillInt32("PositionsOffset", (int)bw.Position);
            foreach (var position in Positions)
                bw.WriteVector3(position);
            bw.FillInt32("RotationsOffset", (int)bw.Position);
            foreach (var rotation in Rotations)
                WriteVector3Short(bw, rotation);
            bw.FillInt32("DataSize", (int)bw.Position);
        }

        /// <summary>
        /// Checks whether the data appears to be a file of this format.
        /// </summary>
        protected override bool Is(BinaryReaderEx br)
        {
            br.BigEndian = true;
            if (br.Length < 64)
                return false;

            return br.ReadInt32() == 0x20051014;
        }

        /// <summary>
        /// Reads three shorts which are divided by 1000.0f into floats to get a Vector3.
        /// </summary>
        /// <param name="br">A BinaryReaderEx.</param>
        /// <returns>A Vector3.</returns>
        private Vector3 ReadVector3Short(BinaryReaderEx br)
        {
            return new Vector3(br.ReadInt16() / 1000.0f, br.ReadInt16() / 1000.0f, br.ReadInt16() / 1000.0f);
        }

        /// <summary>
        /// Write a Vector3 into 3 shorts by multipying its coordinates by 1000.
        /// </summary>
        /// <param name="bw">A BinaryWriterEx.</param>
        /// <param name="vector">The Vector3 to write.</param>
        private void WriteVector3Short(BinaryWriterEx bw, Vector3 vector)
        {
            bw.WriteInt16((short)(vector.X * 1000));
            bw.WriteInt16((short)(vector.Y * 1000));
            bw.WriteInt16((short)(vector.Z * 1000));
        }

        /// <summary>
        /// Gets the number of key frames.
        /// </summary>
        /// <returns>The number of frames.</returns>
        public int GetKeyFrameCount()
        {
            int value = 0;
            foreach (var entry in Bones)
            {
                if (entry.Frames != null)
                {
                    foreach (var frame in entry.Frames.Frames)
                    {
                        if (frame.KeyFrame > value)
                        {
                            value = frame.KeyFrame;
                        }
                    }
                }
            }
            return value;
        }

        /// <summary>
        /// A bone and information regarding where it is each frame.
        /// </summary>
        public class Bone
        {
            /// <summary>
            /// The different object types.
            /// </summary>
            public enum ObjectType : int
            {
                /// <summary>
                /// The object is defined in a model.
                /// </summary>
                Geom = 1,

                /// <summary>
                /// The object is only defined in an animation.
                /// </summary>
                Dummy = 2
            }

            /// <summary>
            /// The object type.
            /// </summary>
            public ObjectType Type { get; set; }

            /// <summary>
            /// The index of this entry.
            /// </summary>
            public short BoneIndex { get; set; }

            /// <summary>
            /// The geometry index of this entry.
            /// </summary>
            public short GeomIndex { get; set; }

            /// <summary>
            /// The index of the parent entry.
            /// </summary>
            public short ParentIndex { get; set; }

            /// <summary>
            /// The index of the child entry.
            /// </summary>
            public short ChildIndex { get; set; }

            /// <summary>
            /// The index of the next sibling entry.
            /// </summary>
            public short NextSiblingIndex { get; set; }

            /// <summary>
            /// Unknown, always seems to be -1, but not all files have been tested yet.
            /// </summary>
            public short UnkIndex12 { get; set; }

            /// <summary>
            /// Where the bone will be moved when animating.
            /// </summary>
            public Vector3 Translation { get; set; }

            /// <summary>
            /// How the bone will rotate when animating.
            /// </summary>
            public Vector3 Rotation { get; set; }

            /// <summary>
            /// The size the bone will be when animating.
            /// </summary>
            public Vector3 Scale { get; set; }

            /// <summary>
            /// The name of the bone.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Information about frames for this bone.<br/>
            /// Null when there is none.
            /// </summary>
            public Animation Frames { get; set; }

            /// <summary>
            /// Create a new <see cref="Bone"/>.
            /// </summary>
            public Bone()
            {
                Type = ObjectType.Geom;
                BoneIndex = -1;
                GeomIndex = -1;
                ParentIndex = -1;
                ChildIndex = -1;
                NextSiblingIndex = -1;
                UnkIndex12 = -1;
                Translation = Vector3.Zero;
                Rotation = Vector3.Zero;
                Scale = Vector3.One;
                Name = string.Empty;
            }

            /// <summary>
            /// Deserializes a <see cref="Bone"/> from a stream.
            /// </summary>
            internal Bone(BinaryReaderEx br)
            {
                int boneNameOffset = br.ReadInt32();

                if (boneNameOffset < 1)
                    throw new InvalidDataException("Entry must have a bone name.");

                Name = br.GetShiftJIS(boneNameOffset);
                Type = br.ReadEnum32<ObjectType>();
                BoneIndex = br.ReadInt16();
                GeomIndex = br.ReadInt16();
                ParentIndex = br.ReadInt16();
                ChildIndex = br.ReadInt16();
                NextSiblingIndex = br.ReadInt16();
                UnkIndex12 = br.ReadInt16();
                Translation = br.ReadVector3();
                Rotation = br.ReadVector3();
                Scale = br.ReadVector3();
                int animDataOffset = br.ReadInt32();
                br.AssertPattern(184, 0);

                if (animDataOffset > 0)
                {
                    long pos = br.Position;
                    br.Position = animDataOffset;
                    Frames = new Animation(br);
                    br.Position = pos;
                }
            }

            /// <summary>
            /// Serializes the <see cref="Bone"/> to a stream.
            /// </summary>
            internal void Write(BinaryWriterEx bw, int index)
            {
                bw.ReserveInt32($"BoneNameOffset_{index}");
                bw.WriteInt32((int)Type);
                bw.WriteInt16(BoneIndex);
                bw.WriteInt16(GeomIndex);
                bw.WriteInt16(ParentIndex);
                bw.WriteInt16(ChildIndex);
                bw.WriteInt16(NextSiblingIndex);
                bw.WriteInt16(UnkIndex12);
                bw.WriteVector3(Translation);
                bw.WriteVector3(Rotation);
                bw.WriteVector3(Scale);
                if (Frames != null)
                    bw.ReserveInt32($"AnimationOffset_{index}");

                bw.WritePattern(184, 0);
            }

            /// <summary>
            /// A group of frame information for a bone.
            /// </summary>
            public class Animation
            {
                /// <summary>
                /// Unknown, seems to determine whether position and rotation indices are present, and in how many bytes.
                /// </summary>
                public enum AnimationType : int
                {
                    /// <summary>
                    /// Position and rotation indices stored as bytes.
                    /// </summary>
                    PosRotBytes = 1,

                    /// <summary>
                    /// Position and rotation indices stored as shorts.
                    /// </summary>
                    PosRotShorts = 2,

                    /// <summary>
                    /// Rotation indices stored as shorts.
                    /// </summary>
                    RotShorts = 4
                }

                /// <summary>
                /// Unknown, seems to determine whether position and rotation indices are present, and in how many bytes.
                /// </summary>
                public AnimationType Type { get; set; }

                /// <summary>
                /// Unknown; A rotation of some kind, usually the same as the rotation of the bone that owns it.
                /// </summary>
                public Vector3 Unk10 { get; set; }

                /// <summary>
                /// Unknown; A rotation of some kind, usually the same as the rotation of the bone that owns it.
                /// </summary>
                public Vector3 Unk20 { get; set; }

                /// <summary>
                /// The key frames in this <see cref="Animation"/>.
                /// </summary>
                public List<Frame> Frames { get; set; }

                /// <summary>
                /// Create a new <see cref="Animation"/>.
                /// </summary>
                public Animation()
                {
                    Type = AnimationType.PosRotShorts;
                    Frames = new List<Frame>();
                }

                /// <summary>
                /// Create a new <see cref="Animation"/>.
                /// </summary>
                public Animation(int frameCount)
                {
                    Type = AnimationType.PosRotShorts;
                    Frames = new List<Frame>(frameCount);
                }

                /// <summary>
                /// Create a new <see cref="Animation"/>.
                /// </summary>
                public Animation(AnimationType animationType, int frameCount)
                {
                    Type = animationType;
                    Frames = new List<Frame>(frameCount);
                }

                /// <summary>
                /// Deserializes a <see cref="Animation"/> from a stream.
                /// </summary>
                internal Animation(BinaryReaderEx br)
                {
                    int framesOffset = br.ReadInt32();
                    int frameCount = br.ReadInt32();
                    Type = br.ReadEnum32<AnimationType>();
                    Unk10 = br.ReadVector3();
                    Unk20 = br.ReadVector3();
                    br.AssertInt32(0);

                    br.Position = framesOffset;
                    Frames = new List<Frame>(frameCount);
                    for (int i = 0; i < frameCount; i++)
                        Frames.Add(new Frame(br, Type));
                }

                /// <summary>
                /// Serializes the <see cref="Animation"/> to a stream.
                /// </summary>
                internal void Write(BinaryWriterEx bw)
                {
                    bw.WriteInt32((int)bw.Position + 36);
                    bw.WriteInt32(Frames.Count);
                    bw.WriteInt32((int)Type);
                    bw.WriteVector3(Unk10);
                    bw.WriteVector3(Unk20);
                    bw.WriteInt32(0);

                    foreach (var frame in Frames)
                        frame.Write(bw, Type);
                }

                /// <summary>
                /// Information on a frame.
                /// </summary>
                public class Frame
                {
                    /// <summary>
                    /// The key frame this information is for.
                    /// </summary>
                    public short KeyFrame { get; set; }

                    /// <summary>
                    /// The index of the position in the positions array.
                    /// </summary>
                    public short PositionIndex { get; set; }

                    /// <summary>
                    /// An unknown index.
                    /// </summary>
                    public short UnkIndex2 { get; set; }

                    /// <summary>
                    /// An unknown index.
                    /// </summary>
                    public short UnkIndex3 { get; set; }

                    /// <summary>
                    /// The index of the rotation in the rotations array.
                    /// </summary>
                    public short RotationIndex { get; set; }

                    /// <summary>
                    /// An unknown index.
                    /// </summary>
                    public short UnkIndex5 { get; set; }

                    /// <summary>
                    /// An unknown index.
                    /// </summary>
                    public short UnkIndex6 { get; set; }

                    /// <summary>
                    /// An unknown index.
                    /// </summary>
                    public short UnkIndex7 { get; set; }

                    /// <summary>
                    /// Create a new <see cref="Frame"/>.
                    /// </summary>
                    public Frame()
                    {
                        KeyFrame = 0;
                    }

                    /// <summary>
                    /// Create a new <see cref="Frame"/>.
                    /// </summary>
                    public Frame(short keyFrame)
                    {
                        KeyFrame = keyFrame;
                    }

                    /// <summary>
                    /// Serializes the <see cref="Frame"/> to a stream.
                    /// </summary>
                    internal Frame(BinaryReaderEx br, AnimationType animationType)
                    {
                        KeyFrame = br.ReadInt16();
                        switch (animationType)
                        {
                            case AnimationType.PosRotBytes:
                                PositionIndex = br.ReadByte();
                                UnkIndex2 = br.ReadByte();
                                UnkIndex3 = br.ReadByte();
                                RotationIndex = br.ReadByte();
                                UnkIndex5 = br.ReadByte();
                                UnkIndex6 = br.ReadByte();
                                UnkIndex7 = -1;
                                break;
                            case AnimationType.PosRotShorts:
                                PositionIndex = br.ReadInt16();
                                UnkIndex2 = br.ReadInt16();
                                UnkIndex3 = br.ReadInt16();
                                RotationIndex = br.ReadInt16();
                                UnkIndex5 = br.ReadInt16();
                                UnkIndex6 = br.ReadInt16();
                                UnkIndex7 = br.ReadInt16();
                                break;
                            case AnimationType.RotShorts:
                                PositionIndex = -1;
                                UnkIndex2 = -1;
                                UnkIndex3 = -1;
                                RotationIndex = br.ReadInt16();
                                UnkIndex5 = br.ReadInt16();
                                UnkIndex6 = br.ReadInt16();
                                UnkIndex7 = -1;
                                break;
                            default:
                                throw new NotImplementedException($"AnimType \"{animationType}\" has not been implemented in index reading.");
                        }
                    }

                    /// <summary>
                    /// Write FrameData to a stream.
                    /// </summary>
                    /// <param name="bw">A BinaryWriterEx.</param>
                    /// <param name="animationType">The animation type.</param>
                    internal void Write(BinaryWriterEx bw, AnimationType animationType)
                    {
                        bw.WriteInt32(KeyFrame);
                        switch (animationType)
                        {
                            case AnimationType.PosRotBytes:
                                bw.WriteByte((byte)PositionIndex);
                                bw.WriteByte((byte)UnkIndex2);
                                bw.WriteByte((byte)UnkIndex3);
                                bw.WriteByte((byte)RotationIndex);
                                bw.WriteByte((byte)UnkIndex5);
                                bw.WriteByte((byte)UnkIndex6);
                                break;
                            case AnimationType.PosRotShorts:
                                bw.WriteInt16(PositionIndex);
                                bw.WriteInt16(UnkIndex2);
                                bw.WriteInt16(UnkIndex3);
                                bw.WriteInt16(RotationIndex);
                                bw.WriteInt16(UnkIndex5);
                                bw.WriteInt16(UnkIndex6);
                                bw.WriteInt16(UnkIndex7);
                                break;
                            case AnimationType.RotShorts:
                                bw.WriteInt16(RotationIndex);
                                bw.WriteInt16(UnkIndex5);
                                bw.WriteInt16(UnkIndex6);
                                break;
                            default:
                                throw new NotImplementedException($"AnimType \"{animationType}\" has not been implemented in index writing.");
                        }
                    }

                    /// <summary>
                    /// Get the position of this frame.
                    /// </summary>
                    /// <param name="positions">The positions array from the ANI itself.</param>
                    /// <returns>The position of this frame.</returns>
                    public Vector3 GetPosition(Vector3[] positions)
                    {
                        return positions[PositionIndex];
                    }

                    /// <summary>
                    /// Get the rotation of this frame.
                    /// </summary>
                    /// <param name="rotations">The rotations array from the ANI itself.</param>
                    /// <returns>The rotation of this frame.</returns>
                    public Vector3 GetRotation(Vector3[] rotations)
                    {
                        return rotations[RotationIndex];
                    }
                }
            }
        }
    }
}
