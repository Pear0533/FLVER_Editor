using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace SoulsFormats
{
    public partial class DRB
    {
        /// <summary>
        /// Indicates the behavior of a UI element.
        /// </summary>
        public enum ControlType
        {
            /// <summary>
            /// Unknown.
            /// </summary>
            DmeCtrlScrollText,

            /// <summary>
            /// Unknown.
            /// </summary>
            FrpgMenuDlgObjContentsHelpItem,

            /// <summary>
            /// Unknown.
            /// </summary>
            Static,

            /// <summary>
            /// A button.
            /// </summary>
            Button,

            /// <summary>
            /// Unknown.
            /// </summary>
            Cursor,

            /// <summary>
            /// Unknown.
            /// </summary>
            LineHelpCtrl,

            /// <summary>
            /// Unknown.
            /// </summary>
            ListCtrl,

            /// <summary>
            /// Unknown.
            /// </summary>
            GridCtrl,

            /// <summary>
            /// A slider in options.
            /// </summary>
            SliderCtrl,

            /// <summary>
            /// Unknown.
            /// </summary>
            ChildDialogCtrl,

            /// <summary>
            /// Unknown.
            /// </summary>
            ComboBoxCtrl
        }

        /// <summary>
        /// Determines the behavior of a UI element.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public abstract class Control
        {
            /// <summary>
            /// The type of this control.
            /// </summary>
            public abstract ControlType Type { get; }

            internal static Control Read(BinaryReaderEx br, Dictionary<int, string> strings, long ctprStart)
            {
                int typeOffset = br.ReadInt32();
                int ctprOffset = br.ReadInt32();
                string type = strings[typeOffset];

                Control result;
                br.StepIn(ctprStart + ctprOffset);
                {
                    if (type == "DmeCtrlScrollText")
                        result = new ScrollTextDummy(br);
                    else if (type == "FrpgMenuDlgObjContentsHelpItem")
                        result = new HelpItem(br);
                    else if (type == "Static")
                        result = new Static(br);
                    else if (type == "Button")
                        result = new Button(br);
                    else if (type == "Cursor")
                        result = new Cursor(br);
                    else if (type == "LineHelpCtrl")
                        result = new LineHelpCtrl(br);
                    else if (type == "ListCtrl")
                        result = new ListCtrl(br);
                    else if (type == "GridCtrl")
                        result = new GridCtrl(br);
                    else if (type == "SliderCtrl")
                        result = new SliderCtrl(br);
                    else if (type == "ChildDialogCtrl")
                        result = new ChildDialogCtrl(br);
                    else if (type == "ComboBoxCtrl")
                        result = new ComboBoxCtrl(br);
                    else
                        throw new InvalidDataException($"Unknown control type: {type}");
                }
                br.StepOut();
                return result;
            }

            internal abstract void WriteData(BinaryWriterEx bw);

            internal void WriteHeader(BinaryWriterEx bw, Dictionary<string, int> stringOffsets, Queue<int> ctprOffsets)
            {
                bw.WriteInt32(stringOffsets[Type.ToString()]);
                bw.WriteInt32(ctprOffsets.Dequeue());
            }

            /// <summary>
            /// Returns the type of the control.
            /// </summary>
            public override string ToString()
            {
                return $"{Type}";
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class ScrollTextDummy : Control
            {
                /// <summary>
                /// ControlType.DmeCtrlScrollText
                /// </summary>
                public override ControlType Type => ControlType.DmeCtrlScrollText;

                /// <summary>
                /// Unknown; always 0.
                /// </summary>
                public int Unk00 { get; set; }

                /// <summary>
                /// Creates a ScrollTextDummy with default values.
                /// </summary>
                public ScrollTextDummy() : base() { }

                internal ScrollTextDummy(BinaryReaderEx br)
                {
                    Unk00 = br.ReadInt32();
                }

                internal override void WriteData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(Unk00);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class HelpItem : Control
            {
                /// <summary>
                /// ControlType.FrpgMenuDlgObjContentsHelpItem
                /// </summary>
                public override ControlType Type => ControlType.FrpgMenuDlgObjContentsHelpItem;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk00 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk04 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk08 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk0C { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk10 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk14 { get; set; }

                /// <summary>
                /// An FMG ID.
                /// </summary>
                public int TextID { get; set; }

                /// <summary>
                /// Creates a HelpItem with default values.
                /// </summary>
                public HelpItem() : base()
                {
                    TextID = -1;
                }

                internal HelpItem(BinaryReaderEx br)
                {
                    Unk00 = br.ReadInt32();
                    Unk04 = br.ReadInt32();
                    Unk08 = br.ReadInt32();
                    Unk0C = br.ReadInt32();
                    Unk10 = br.ReadInt32();
                    Unk14 = br.ReadInt32();
                    TextID = br.ReadInt32();
                }

                internal override void WriteData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(Unk00);
                    bw.WriteInt32(Unk04);
                    bw.WriteInt32(Unk08);
                    bw.WriteInt32(Unk0C);
                    bw.WriteInt32(Unk10);
                    bw.WriteInt32(Unk14);
                    bw.WriteInt32(TextID);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class Static : Control
            {
                /// <summary>
                /// ControlType.Static
                /// </summary>
                public override ControlType Type => ControlType.Static;

                /// <summary>
                /// Unknown; always 0.
                /// </summary>
                public int Unk00 { get; set; }

                /// <summary>
                /// Creates a Static with default values.
                /// </summary>
                public Static() : base() { }

                internal Static(BinaryReaderEx br)
                {
                    Unk00 = br.ReadInt32();
                }

                internal override void WriteData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(Unk00);
                }
            }

            /// <summary>
            /// A button.
            /// </summary>
            public class Button : Control
            {
                /// <summary>
                /// ControlType.Button
                /// </summary>
                public override ControlType Type => ControlType.Button;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk00 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short Unk04 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte Unk06 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte Unk07 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk08 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk0C { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk10 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk14 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short Unk16 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte Unk17 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte Unk18 { get; set; }

                /// <summary>
                /// Creates a Button with default values.
                /// </summary>
                public Button() : base() { }

                internal Button(BinaryReaderEx br)
                {
                    Unk00 = br.ReadInt32();
                    Unk04 = br.ReadInt16();
                    Unk06 = br.ReadByte();
                    Unk07 = br.ReadByte();
                    Unk08 = br.ReadInt32();
                    Unk0C = br.ReadInt32();
                    Unk10 = br.ReadInt32();
                    Unk14 = br.ReadInt32();
                    Unk16 = br.ReadInt16();
                    Unk17 = br.ReadByte();
                    Unk18 = br.ReadByte();
                }

                internal override void WriteData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(Unk00);
                    bw.WriteInt16(Unk04);
                    bw.WriteByte(Unk06);
                    bw.WriteByte(Unk07);
                    bw.WriteInt32(Unk08);
                    bw.WriteInt32(Unk0C);
                    bw.WriteInt32(Unk10);
                    bw.WriteInt32(Unk14);
                    bw.WriteInt16(Unk16);
                    bw.WriteByte(Unk17);
                    bw.WriteByte(Unk18);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class Cursor : Control
            {
                /// <summary>
                /// ControlType.Cursor
                /// </summary>
                public override ControlType Type => ControlType.Cursor;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk00 { get; set; }

                /// <summary>
                /// Creates a Cursor with default values.
                /// </summary>
                public Cursor() : base() { }

                internal Cursor(BinaryReaderEx br)
                {
                    Unk00 = br.ReadInt32();
                }

                internal override void WriteData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(Unk00);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class LineHelpCtrl : Control
            {
                /// <summary>
                /// ControlType.LineHelpCtrl
                /// </summary>
                public override ControlType Type => ControlType.LineHelpCtrl;

                /// <summary>
                /// Unknown; always 0.
                /// </summary>
                public int Unk00 { get; set; }

                /// <summary>
                /// Creates a LineHelpCtrl with default values.
                /// </summary>
                public LineHelpCtrl() : base() { }

                internal LineHelpCtrl(BinaryReaderEx br)
                {
                    Unk00 = br.ReadInt32();
                }

                internal override void WriteData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(Unk00);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class ListCtrl : Control
            {
                /// <summary>
                /// ControlType.ListCtrl
                /// </summary>
                public override ControlType Type => ControlType.ListCtrl;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk00 { get; set; }

                /// <summary>
                /// Unknown; always 8.
                /// </summary>
                public int Unk04 { get; set; }

                /// <summary>
                /// Unknown; always 8.
                /// </summary>
                public int Unk08 { get; set; }

                /// <summary>
                /// Creates a ListCtrl with default values.
                /// </summary>
                public ListCtrl() : base() { }

                internal ListCtrl(BinaryReaderEx br)
                {
                    Unk00 = br.ReadInt32();
                    Unk04 = br.ReadInt32();
                    Unk08 = br.ReadInt32();
                }

                internal override void WriteData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(Unk00);
                    bw.WriteInt32(Unk04);
                    bw.WriteInt32(Unk08);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class GridCtrl : Control
            {
                /// <summary>
                /// ControlType.GridCtrl
                /// </summary>
                public override ControlType Type => ControlType.GridCtrl;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk00 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk04 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk08 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk0C { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk10 { get; set; }

                /// <summary>
                /// Creates a GridCtrl with default values.
                /// </summary>
                public GridCtrl() : base() { }

                internal GridCtrl(BinaryReaderEx br)
                {
                    Unk00 = br.ReadInt32();
                    Unk04 = br.ReadInt32();
                    Unk08 = br.ReadInt32();
                    Unk0C = br.ReadInt32();
                    Unk10 = br.ReadInt32();
                }

                internal override void WriteData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(Unk00);
                    bw.WriteInt32(Unk04);
                    bw.WriteInt32(Unk08);
                    bw.WriteInt32(Unk0C);
                    bw.WriteInt32(Unk10);
                }
            }

            /// <summary>
            /// A slider in options.
            /// </summary>
            public class SliderCtrl : Control
            {
                /// <summary>
                /// ControlType.SliderCtrl
                /// </summary>
                public override ControlType Type => ControlType.SliderCtrl;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk00 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short Unk04 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short Unk06 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk08 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk0C { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk10 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk14 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk18 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk22 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk26 { get; set; }

                /// <summary>
                /// Creates a SliderCtrl with default values.
                /// </summary>
                public SliderCtrl() : base() { }

                internal SliderCtrl(BinaryReaderEx br)
                {
                    Unk00 = br.ReadInt32();
                    Unk04 = br.ReadInt16();
                    Unk06 = br.ReadInt16();
                    Unk08 = br.ReadInt32();
                    Unk0C = br.ReadInt32();
                    Unk10 = br.ReadInt32();
                    Unk14 = br.ReadInt32();
                    Unk22 = br.ReadInt32();
                    Unk26 = br.ReadInt32();
                }

                internal override void WriteData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(Unk00);
                    bw.WriteInt16(Unk04);
                    bw.WriteInt16(Unk06);
                    bw.WriteInt32(Unk08);
                    bw.WriteInt32(Unk0C);
                    bw.WriteInt32(Unk10);
                    bw.WriteInt32(Unk14);
                    bw.WriteInt32(Unk22);
                    bw.WriteInt32(Unk26);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class ChildDialogCtrl : Control
            {
                /// <summary>
                /// ControlType.ChildDialogCtrl
                /// </summary>
                public override ControlType Type => ControlType.ChildDialogCtrl;

                /// <summary>
                /// Unknown; Likely an ID of some kind.
                /// </summary>
                public int Unk00 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk04 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk08 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk0C { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk10 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk14 { get; set; }

                /// <summary>
                /// Creates a ChildDialogCtrl with default values.
                /// </summary>
                public ChildDialogCtrl() : base() { }

                internal ChildDialogCtrl(BinaryReaderEx br)
                {
                    Unk00 = br.ReadInt32();
                    Unk04 = br.ReadInt32();
                    Unk08 = br.ReadInt32();
                    Unk0C = br.ReadInt32();
                    Unk10 = br.ReadInt32();
                    Unk14 = br.ReadInt32();
                }

                internal override void WriteData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(Unk00);
                    bw.WriteInt32(Unk04);
                    bw.WriteInt32(Unk08);
                    bw.WriteInt32(Unk0C);
                    bw.WriteInt32(Unk10);
                    bw.WriteInt32(Unk14);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class ComboBoxCtrl : Control
            {
                /// <summary>
                /// ControlType.ComboBoxCtrl
                /// </summary>
                public override ControlType Type => ControlType.ComboBoxCtrl;

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk00 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short Unk02 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short Unk04 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk08 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk0C { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk10 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk14 { get; set; }

                /// <summary>
                /// Creates a ComboBoxCtrl with default values.
                /// </summary>
                public ComboBoxCtrl() : base() { }

                internal ComboBoxCtrl(BinaryReaderEx br)
                {
                    Unk00 = br.ReadInt32();
                    Unk02 = br.ReadInt16();
                    Unk04 = br.ReadInt16();
                    Unk08 = br.ReadInt32();
                    Unk0C = br.ReadInt32();
                    Unk10 = br.ReadInt32();
                    Unk14 = br.ReadInt32();
                }

                internal override void WriteData(BinaryWriterEx bw)
                {
                    bw.WriteInt32(Unk00);
                    bw.WriteInt16(Unk02);
                    bw.WriteInt16(Unk04);
                    bw.WriteInt32(Unk08);
                    bw.WriteInt32(Unk0C);
                    bw.WriteInt32(Unk10);
                    bw.WriteInt32(Unk14);
                }
            }
        }
    }
}
