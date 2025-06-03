using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SoulsFormats
{
    public partial class PARAM
    {
        /// <summary>
        /// One row in a param file.
        /// </summary>
        public class Row
        {
            /// <summary>
            /// The ID number of this row.
            /// </summary>
            public int ID { get; set; }

            /// <summary>
            /// A name given to this row; no functional significance, may be null.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Cells contained in this row. Must be loaded with PARAM.ApplyParamdef() before use.
            /// </summary>
            public IReadOnlyList<Cell> Cells { get; private set; }

            internal long DataOffset;

            /// <summary>
            /// Creates a new row based on the given paramdef with default values.
            /// </summary>
            public Row(int id, string name, PARAMDEF paramdef)
            {
                ID = id;
                Name = name;

                var cells = new Cell[paramdef.Fields.Count];
                for (int i = 0; i < paramdef.Fields.Count; i++)
                {
                    PARAMDEF.Field field = paramdef.Fields[i];
                    object value = ParamUtil.ConvertDefaultValue(field);
                    cells[i] = new Cell(field, value);
                }
                Cells = cells;
            }

            /// <summary>
            /// Copy constructor for a row. Does not add to the param.
            /// </summary>
            /// <param name="clone">The row that is being copied</param>
            public Row(Row clone)
            {
                ID = clone.ID;
                Name = clone.Name;
                var cells = new List<Cell>(clone.Cells.Count);

                foreach (var cell in clone.Cells)
                {
                    cells.Add(new Cell(cell));
                }
                Cells = cells;
            }

            internal Row(BinaryReaderEx br, PARAM parent, ref long actualStringsOffset)
            {
                long nameOffset;
                if (parent.Format2D.HasFlag(FormatFlags1.LongDataOffset))
                {
                    ID = br.ReadInt32();
                    br.ReadInt32(); // I would like to assert 0, but some of the generatordbglocation params in DS2S have garbage here
                    DataOffset = br.ReadInt64();

                    if (!parent.UnnamedRows)
                        nameOffset = br.ReadInt64();
                    else
                        nameOffset = -1;
                }
                else
                {
                    ID = br.ReadInt32();
                    DataOffset = br.ReadUInt32();

                    if (!parent.UnnamedRows)
                        nameOffset = br.ReadUInt32();
                    else
                        nameOffset = -1;
                }

                if (!parent.UnnamedRows && nameOffset != 0 && nameOffset != br.Length)
                {
                    if (actualStringsOffset == 0 || nameOffset < actualStringsOffset)
                        actualStringsOffset = nameOffset;

                    if (parent.Format2E.HasFlag(FormatFlags2.UnicodeRowNames))
                        Name = br.GetUTF16(nameOffset);
                    else
                        Name = br.GetShiftJIS(nameOffset);
                }
            }

            // Only for headerless rows
            internal Row(int id, long dataOffset)
            {
                ID = id;
                DataOffset = dataOffset;
            }

            internal void ReadCells(BinaryReaderEx br, PARAMDEF paramdef)
            {
                // In case someone decides to add new rows before applying the paramdef (please don't do that)
                if (DataOffset == 0)
                    return;

                br.Position = DataOffset;
                var cells = new Cell[paramdef.Fields.Count];

                int bitOffset = -1;
                int bitLimit = -1;
                ulong bitValue = 0; // This is ulong so checkOrphanedBits doesn't fail on offsets of 32
                const int BIT_VALUE_SIZE = 64;

                void checkOrphanedBits()
                {
                    if (bitOffset != -1 && (bitValue >> bitOffset) != 0)
                    {
                        throw new InvalidDataException($"Invalid paramdef {paramdef.ParamType}; bits would be lost before +0x{br.Position - DataOffset:X} in row {ID}.");
                    }
                }

                for (int i = 0; i < paramdef.Fields.Count; i++)
                {
                    PARAMDEF.Field field = paramdef.Fields[i];
                    object value = null;
                    PARAMDEF.DefType type = field.DisplayType;

                    if (type == PARAMDEF.DefType.b32)
                        value = br.ReadInt32();
                    else if (type == PARAMDEF.DefType.f32 || type == PARAMDEF.DefType.angle32)
                        value = br.ReadSingle();
                    else if (type == PARAMDEF.DefType.f64)
                        value = br.ReadDouble();
                    else if (type == PARAMDEF.DefType.fixstr)
                        value = br.ReadFixStr(field.ArrayLength);
                    else if (type == PARAMDEF.DefType.fixstrW)
                        value = br.ReadFixStrW(field.ArrayLength * 2);
                    else if (ParamUtil.IsBitType(type))
                    {
                        if (field.BitSize == -1)
                        {
                            switch (type)
                            {
                                case PARAMDEF.DefType.s8:
                                    value = br.ReadSByte();
                                    break;
                                case PARAMDEF.DefType.u8:
                                    if (field.ArrayLength > 1)
                                        value = br.ReadBytes(field.ArrayLength);
                                    else
                                        value = br.ReadByte();
                                    break;
                                case PARAMDEF.DefType.s16:
                                    value = br.ReadInt16();
                                    break;
                                case PARAMDEF.DefType.u16:
                                    value = br.ReadUInt16();
                                    break;
                                case PARAMDEF.DefType.s32:
                                    value = br.ReadInt32();
                                    break;
                                case PARAMDEF.DefType.u32:
                                    value = br.ReadUInt32();
                                    break;
                                case PARAMDEF.DefType.dummy8:
                                    value = br.ReadBytes(field.ArrayLength);
                                    break;
                                default:
                                    throw new InvalidOperationException($"Unexpected type in bitfield handling: {type}");
                            }
                        }
                    }
                    else
                        throw new NotImplementedException($"Unsupported field type: {type}");

                    if (value != null)
                    {
                        checkOrphanedBits();
                        bitOffset = -1;
                    }
                    else
                    {
                        if (bitOffset == -1 || ParamUtil.GetBitLimit(type) != bitLimit || bitOffset + field.BitSize > bitLimit)
                        {
                            checkOrphanedBits();
                            bitOffset = 0;
                            bitLimit = ParamUtil.GetBitLimit(type);

                            // Always read unsigned to retain the exact bits; it will be sign-extended later if applicable
                            switch (bitLimit)
                            {
                                case 8:
                                    bitValue = br.ReadByte();
                                    break;
                                case 16:
                                    bitValue = br.ReadUInt16();
                                    break;
                                case 32:
                                    bitValue = br.ReadUInt32();
                                    break;
                                default:
                                    throw new InvalidOperationException($"Unexpected bit limit in bitfield handling: {bitLimit}");
                            }
                        }

                        if (field.BitSize == 0)
                            throw new NotImplementedException($"Bit size 0 is not supported.");
                        if (field.BitSize > bitLimit)
                            throw new InvalidDataException($"Bit size {field.BitSize} is too large to fit in type {type}.");

                        long shifted;
                        int leftShift = BIT_VALUE_SIZE - field.BitSize - bitOffset;
                        int rightShift = BIT_VALUE_SIZE - field.BitSize;

                        // Cast before shifting for sign extension
                        if (ParamUtil.IsSignedBitType(type))
                            shifted = (long)bitValue << leftShift >> rightShift;
                        // Cast after shifting to avoid sign extension
                        else
                            shifted = (long)(bitValue << leftShift >> rightShift);

                        bitOffset += field.BitSize;
                        switch (type)
                        {
                            case PARAMDEF.DefType.s8:
                                value = (sbyte)shifted;
                                break;
                            case PARAMDEF.DefType.u8:
                                value = (byte)shifted;
                                break;
                            case PARAMDEF.DefType.s16:
                                value = (short)shifted;
                                break;
                            case PARAMDEF.DefType.u16:
                                value = (ushort)shifted;
                                break;
                            case PARAMDEF.DefType.s32:
                                value = (int)shifted;
                                break;
                            case PARAMDEF.DefType.u32:
                                value = (uint)shifted;
                                break;
                            case PARAMDEF.DefType.dummy8:
                                value = (byte)shifted;
                                break;
                            default:
                                throw new InvalidOperationException($"Unexpected type in bitfield handling: {type}");
                        }
                    }

                    cells[i] = new Cell(field, value);
                }

                checkOrphanedBits();
                Cells = cells;
            }

            internal void WriteHeader(BinaryWriterEx bw, PARAM parent, int i)
            {
                if (parent.Format2D.HasFlag(FormatFlags1.LongDataOffset))
                {
                    bw.WriteInt32(ID);
                    bw.WriteInt32(0);
                    bw.ReserveInt64($"RowOffset{i}");

                    // Likely won't be nameless at this point anyways
                    if (!parent.UnnamedRows)
                        bw.ReserveInt64($"NameOffset{i}");
                }
                else
                {
                    bw.WriteInt32(ID);
                    bw.ReserveUInt32($"RowOffset{i}");

                    if (!parent.UnnamedRows)
                        bw.ReserveUInt32($"NameOffset{i}");
                }
            }

            internal void WriteCells(BinaryWriterEx bw, PARAM parent, int index)
            {
                if (!parent.HeaderlessRows)
                {
                    if (parent.Format2D.HasFlag(FormatFlags1.LongDataOffset))
                        bw.FillInt64($"RowOffset{index}", bw.Position);
                    else
                        bw.FillUInt32($"RowOffset{index}", (uint)bw.Position);
                }

                int bitOffset = -1;
                int bitLimit = -1;
                ulong bitValue = 0;
                const int BIT_VALUE_SIZE = 64;

                for (int i = 0; i < Cells.Count; i++)
                {
                    Cell cell = Cells[i];
                    object value = cell.Value;
                    PARAMDEF.Field field = cell.Def;
                    PARAMDEF.DefType type = field.DisplayType;

                    if (type == PARAMDEF.DefType.b32)
                        bw.WriteInt32((int)value);
                    else if (type == PARAMDEF.DefType.f32 || type == PARAMDEF.DefType.angle32)
                        bw.WriteSingle((float)value);
                    else if (type == PARAMDEF.DefType.f64)
                        bw.WriteDouble((double)value);
                    else if (type == PARAMDEF.DefType.fixstr)
                        bw.WriteFixStr((string)value, field.ArrayLength);
                    else if (type == PARAMDEF.DefType.fixstrW)
                        bw.WriteFixStrW((string)value, field.ArrayLength * 2);
                    else if (ParamUtil.IsBitType(type))
                    {
                        if (field.BitSize == -1)
                        {
                            switch (type)
                            {
                                case PARAMDEF.DefType.s8: bw.WriteSByte((sbyte)value); break;
                                case PARAMDEF.DefType.u8:
                                    if (field.ArrayLength > 1)
                                        bw.WriteBytes((byte[])value);
                                    else
                                        bw.WriteByte((byte)value);
                                    break;
                                case PARAMDEF.DefType.s16: bw.WriteInt16((short)value); break;
                                case PARAMDEF.DefType.u16: bw.WriteUInt16((ushort)value); break;
                                case PARAMDEF.DefType.s32: bw.WriteInt32((int)value); break;
                                case PARAMDEF.DefType.u32: bw.WriteUInt32((uint)value); break;
                                case PARAMDEF.DefType.dummy8: bw.WriteBytes((byte[])value); break;
                                default: throw new InvalidOperationException($"Unexpected type in bitfield handling: {type}");
                            }
                        }
                        else
                        {
                            if (bitOffset == -1)
                            {
                                bitOffset = 0;
                                bitLimit = ParamUtil.GetBitLimit(type);
                                bitValue = 0;
                            }

                            ulong shifted;
                            switch (type)
                            {
                                case PARAMDEF.DefType.s8:
                                    shifted = (byte)(sbyte)value;
                                    break;
                                case PARAMDEF.DefType.u8:
                                    shifted = (byte)value;
                                    break;
                                case PARAMDEF.DefType.s16:
                                    shifted = (ushort)(short)value;
                                    break;
                                case PARAMDEF.DefType.u16:
                                    shifted = (ushort)value;
                                    break;
                                case PARAMDEF.DefType.s32:
                                    shifted = (uint)(int)value;
                                    break;
                                case PARAMDEF.DefType.u32:
                                    shifted = (uint)value;
                                    break;
                                case PARAMDEF.DefType.dummy8:
                                    shifted = (byte)value;
                                    break;
                                default:
                                    throw new InvalidOperationException($"Unexpected type in bitfield handling: {type}");
                            }

                            // Shift left first to clear any out-of-range bits
                            shifted = shifted << (BIT_VALUE_SIZE - field.BitSize) >> (BIT_VALUE_SIZE - field.BitSize - bitOffset);
                            bitValue |= shifted;
                            bitOffset += field.BitSize;

                            bool write = false;
                            if (i == Cells.Count - 1)
                            {
                                write = true;
                            }
                            else
                            {
                                PARAMDEF.Field nextField = Cells[i + 1].Def;
                                PARAMDEF.DefType nextType = nextField.DisplayType;
                                if (!ParamUtil.IsBitType(nextType) || nextField.BitSize == -1 || ParamUtil.GetBitLimit(nextType) != bitLimit
                                     || bitOffset + nextField.BitSize > bitLimit)
                                {
                                    write = true;
                                }
                            }

                            if (write)
                            {
                                bitOffset = -1;
                                switch (bitLimit)
                                {
                                    case 8: bw.WriteByte((byte)bitValue); break;
                                    case 16: bw.WriteUInt16((ushort)bitValue); break;
                                    case 32: bw.WriteUInt32((uint)bitValue); break;
                                    default: throw new InvalidOperationException($"Unexpected bit limit in bitfield handling: {bitLimit}");
                                }
                            }
                        }
                    }
                    else
                        throw new NotImplementedException($"Unsupported field type: {type}");
                }
            }

            internal void WriteName(BinaryWriterEx bw, PARAM parent, int i)
            {
                long nameOffset = 0;
                if (Name != null)
                {
                    nameOffset = bw.Position;
                    if (parent.Format2E.HasFlag(FormatFlags2.UnicodeRowNames))
                        bw.WriteUTF16(Name, true);
                    else
                        bw.WriteShiftJIS(Name, true);
                }

                if (parent.Format2D.HasFlag(FormatFlags1.LongDataOffset))
                    bw.FillInt64($"NameOffset{i}", nameOffset);
                else
                    bw.FillUInt32($"NameOffset{i}", (uint)nameOffset);
            }

            /// <summary>
            /// Returns a string representation of the row.
            /// </summary>
            public override string ToString()
            {
                return $"{ID} {Name}";
            }

            /// <summary>
            /// Returns the first cell in the row with the given internal name.
            /// </summary>
            public Cell this[string name] => Cells.First(cell => cell.Def.InternalName == name);
        }
    }
}
