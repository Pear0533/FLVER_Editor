using System;

namespace SoulsFormats
{
    public partial class PARAM
    {
        /// <summary>
        /// One cell in one row in a param.
        /// </summary>
        public class Cell
        {
            /// <summary>
            /// The paramdef field that describes this cell.
            /// </summary>
            public PARAMDEF.Field Def { get; }

            /// <summary>
            /// The value of this cell.
            /// </summary>
            public object Value
            {
                get => value;
                set
                {
                    if (value == null)
                        throw new NullReferenceException($"Cell value may not be null.");

                    switch (Def.DisplayType)
                    {
                        case PARAMDEF.DefType.s8: this.value = Convert.ToSByte(value); break;
                        case PARAMDEF.DefType.u8:
                            if (Def.ArrayLength > 1)
                                this.value = (byte[])value;
                            else
                                this.value = Convert.ToByte(value);
                            break;
                        case PARAMDEF.DefType.s16: this.value = Convert.ToInt16(value); break;
                        case PARAMDEF.DefType.u16: this.value = Convert.ToUInt16(value); break;
                        case PARAMDEF.DefType.s32: this.value = Convert.ToInt32(value); break;
                        case PARAMDEF.DefType.u32: this.value = Convert.ToUInt32(value); break;
                        case PARAMDEF.DefType.b32: this.value = Convert.ToInt32(value); break;
                        case PARAMDEF.DefType.f32: this.value = Convert.ToSingle(value); break;
                        case PARAMDEF.DefType.angle32: this.value = Convert.ToSingle(value); break;
                        case PARAMDEF.DefType.f64: this.value = Convert.ToDouble(value); break;
                        case PARAMDEF.DefType.fixstr: this.value = Convert.ToString(value); break;
                        case PARAMDEF.DefType.fixstrW: this.value = Convert.ToString(value); break;
                        case PARAMDEF.DefType.dummy8:
                            if (Def.BitSize == -1)
                                this.value = (byte[])value;
                            else
                                this.value = Convert.ToByte(value);
                            break;

                        default:
                            throw new NotImplementedException($"Conversion not specified for type {Def.DisplayType}");
                    }
                }
            }
            private object value;

            /// <summary>
            /// The display type of the cell.
            /// </summary>
            public PARAMDEF.DefType DisplayType
                => Def.DisplayType;

            /// <summary>
            /// The internal type of the cell.
            /// </summary>
            public string InternalType
                => Def.InternalType;

            /// <summary>
            /// The display name of the cell.
            /// </summary>
            public string DisplayName
                => Def.DisplayName;

            /// <summary>
            /// The internal name of the cell.
            /// </summary>
            public string InternalName
                => Def.InternalName;

            /// <summary>
            /// An optional description of the cell.
            /// </summary>
            public string Description
                => Def.Description;

            /// <summary>
            /// A string for formatting the value of the cell.
            /// </summary>
            public string DisplayFormat
                => Def.DisplayFormat;

            /// <summary>
            /// The default value of the cell.
            /// </summary>
            public object Default
                => Def.Default;

            /// <summary>
            /// How much the cell's value increments by.
            /// </summary>
            public object Increment
                => Def.Increment;

            /// <summary>
            /// The minimum value of the cell.
            /// </summary>
            public object Minimum
                => Def.Minimum;

            /// <summary>
            /// The maximum value of the cell.
            /// </summary>
            public object Maximum
                => Def.Maximum;

            /// <summary>
            /// Cells are sorted by this ID.
            /// </summary>
            public int SortID
                => Def.SortID;

            /// <summary>
            /// Number of elements for arrays.
            /// </summary>
            public int ArrayLength
                => Def.ArrayLength;

            /// <summary>
            /// The number of bits used by a bitfield.
            /// </summary>
            public int BitSize
                => Def.BitSize;

            internal Cell(PARAMDEF.Field def, object value)
            {
                Def = def;
                Value = value;
            }

            /// <summary>
            /// Make a new cell from a cell.
            /// </summary>
            /// <param name="clone">The cell to copy.</param>
            public Cell(Cell clone)
            {
                Def = clone.Def;
                Value = clone.Value;
            }

            /// <summary>
            /// Returns a string representation of the cell.
            /// </summary>
            public override string ToString()
            {
                return $"{Def.DisplayType} {Def.InternalName} = {Value}";
            }
        }
    }
}
