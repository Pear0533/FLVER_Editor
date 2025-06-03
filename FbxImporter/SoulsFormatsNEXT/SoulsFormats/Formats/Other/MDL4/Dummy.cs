using System.Drawing;
using System.Numerics;

namespace SoulsFormats.Other
{
    /// <summary>
    /// A 3D model format used in early PS3/X360 games. Extension: .mdl
    /// </summary>
    public partial class MDL4
    {
        /// <summary>
        /// "Dummy polygons" in this MDL4.
        /// </summary>
        public class Dummy
        {
            /// <summary>
            /// Location of the dummy point.
            /// </summary>
            public Vector3 Position { get; set; }

            /// <summary>
            /// Vector indicating the dummy point's forward direction.
            /// </summary>
            public Vector3 Forward { get; set; }

            /// <summary>
            /// Unknown; ARGB order.
            /// </summary>
            public Color Color { get; set; }

            /// <summary>
            /// Indicates the type of dummy point this is (hitbox, sfx, etc).
            /// </summary>
            public short ReferenceID { get; set; }

            /// <summary>
            /// Index of a bone that the dummy point is initially transformed to before binding to the attach bone.
            /// </summary>
            public short ParentBoneIndex { get; set; }

            /// <summary>
            /// Index of the bone that the dummy point follows physically.
            /// </summary>
            public short AttachBoneIndex { get; set; }

            /// <summary>
            /// Unknown; Could be two bytes, one representing UseUpwardVector.
            /// </summary>
            public short Unk22 { get; set; }

            /// <summary>
            /// Create a new Dummy with default values.
            /// </summary>
            public Dummy()
            {
                ParentBoneIndex = 0;
                AttachBoneIndex = -1;
            }

            /// <summary>
            /// Clone an existing Dummy.
            /// </summary>
            public Dummy(Dummy dummy)
            {
                Position = dummy.Position;
                Forward = dummy.Forward;
                ReferenceID = dummy.ReferenceID;
                ParentBoneIndex = dummy.ParentBoneIndex;
                AttachBoneIndex = dummy.AttachBoneIndex;
                Color = dummy.Color;
                Unk22 = dummy.Unk22;
            }

            /// <summary>
            /// Read a Dummy from a stream.
            /// </summary>
            internal Dummy(BinaryReaderEx br)
            {
                Position = br.ReadVector3();
                Forward = br.ReadVector3();
                Color = br.ReadARGB();
                ReferenceID = br.ReadInt16();
                ParentBoneIndex = br.ReadInt16();
                AttachBoneIndex = br.ReadInt16();
                Unk22 = br.ReadInt16(); // Some flag then UseUpwardVector?

                // Upward Vector?
                br.AssertInt32(0);
                br.AssertInt32(0);
                br.AssertInt32(0);
            }

            /// <summary>
            /// Writes a dummy to an MDL4 file.
            /// </summary>
            internal void Write(BinaryWriterEx bw)
            {
                bw.WriteVector3(Position);
                bw.WriteVector3(Forward);
                bw.WriteARGB(Color);
                bw.WriteInt16(ReferenceID);
                bw.WriteInt16(ParentBoneIndex);
                bw.WriteInt16(AttachBoneIndex);
                bw.WriteInt16(Unk22); // Some flag then UseUpwardVector?

                // Upward Vector?
                bw.WriteInt32(0);
                bw.WriteInt32(0);
                bw.WriteInt32(0);
            }
        }
    }
}
