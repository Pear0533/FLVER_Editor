using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulsFormats
{
    public partial class MSBDR
    {
        private class MapstudioTree : Param<Tree>
        {
            internal override string Name => "MAPSTUDIO_TREE_ST";

            public List<Tree> Trees { get; set; }

            public MapstudioTree()
            {
                Trees = new List<Tree>();
            }

            internal override Tree ReadEntry(BinaryReaderEx br)
            {
                return Trees.EchoAdd(new Tree(br));
            }

            public override List<Tree> GetEntries()
            {
                return Trees;
            }
        }

        /// <summary>
        /// Unknown.
        /// </summary>
        public class Tree : Entry
        {
            /// <summary>
            /// Unknown.
            /// </summary>
            public float Unk00 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public float Unk04 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public float Unk08 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public long Unk0C { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public float Unk10 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public float Unk14 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public float Unk18 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public long Unk1C { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public float Unk20 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public float Unk24 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public float Unk28 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public long Unk2C { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public float Unk30 { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public List<short> UnkShorts { get; set; }

            /// <summary>
            /// Creates a Tree with default values.
            /// </summary>
            public Tree()
            {
                UnkShorts = new List<short>();
            }

            /// <summary>
            /// Creates a deep copy of the tree.
            /// </summary>
            public Tree DeepCopy()
            {
                var tree = (Tree)MemberwiseClone();
                tree.UnkShorts = new List<short>(UnkShorts);
                return tree;
            }

            internal Tree(BinaryReaderEx br)
            {
                Unk00 = br.ReadSingle();
                Unk04 = br.ReadSingle();
                Unk08 = br.ReadSingle();
                Unk10 = br.ReadSingle();
                Unk14 = br.ReadSingle();
                Unk18 = br.ReadSingle();
                Unk20 = br.ReadSingle();
                Unk24 = br.ReadSingle();
                Unk28 = br.ReadSingle();
                br.Pad(0x8);
                Unk0C = br.ReadInt64();
                Unk1C = br.ReadInt64();
                Unk2C = br.ReadInt64();
                Unk30 = br.ReadSingle();
                int shortCount = br.ReadInt32();
                UnkShorts = new List<short>(br.ReadInt16s(shortCount));
            }

            internal override void Write(BinaryWriterEx bw, int id)
            {
                bw.WriteSingle(Unk00);
                bw.WriteSingle(Unk04);
                bw.WriteSingle(Unk08);
                bw.WriteSingle(Unk10);
                bw.WriteSingle(Unk14);
                bw.WriteSingle(Unk18);
                bw.WriteSingle(Unk20);
                bw.WriteSingle(Unk24);
                bw.WriteSingle(Unk28);
                bw.PadFF(0x8);
                bw.WriteInt64(Unk0C);
                bw.WriteInt64(Unk1C);
                bw.WriteInt64(Unk2C);
                bw.WriteSingle(Unk30);
                bw.WriteInt32(UnkShorts.Count);
                bw.WriteInt16s(UnkShorts);
                bw.PadFF(0x8);
            }
        }
    }
}