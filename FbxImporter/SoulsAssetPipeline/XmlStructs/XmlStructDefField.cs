using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SoulsAssetPipeline.XmlStructs
{
    public class XmlStructDefField
    {
        public string ValueType { get; set; }
        public string Name { get; set; } = null;

        private static bool DebugDisableAsserts = false;
        public object AssertValue { get; set; } = null;
        public bool IsAssert => DebugDisableAsserts ? false : AssertValue != null;

        public bool IsXmlAttribute { get; set; } = false;
        //TODO: IMPLEMENT FLATTENED
        public bool IsNodeFlattened { get; set; } = false;
        public bool IsOptional { get; set; } = false;
        public string Tag { get; set; } = null;

        public XmlStructDefField()
        {

        }

        public XmlStructDefField(XmlNode xmlNode)
        {
            ValueType = xmlNode.Name;
            Name = xmlNode.SafeGetAttribute("name", optional: true);

            if (string.IsNullOrWhiteSpace(Name))
                Name = null;

            var assertValueString = xmlNode.SafeGetAttribute("assert", optional: true);

            if (assertValueString != null)
                AssertValue = ReadStandardValueFromString(ValueType, assertValueString, justReturnStringForNonStandardValue: true);

            IsXmlAttribute = xmlNode.SafeGetOptionalBoolAttribute("attribute") ?? false;
            IsNodeFlattened = xmlNode.SafeGetOptionalBoolAttribute("flattened") ?? false;
            IsOptional = xmlNode.SafeGetOptionalBoolAttribute("optional") ?? false;

            Tag = xmlNode.SafeGetAttribute("tag", optional: true);
        }

        public void WriteWithinSerializedFfxXml(XmlWriter writer)
        {
            writer.WriteStartElement(ValueType);
            writer.WriteAttributeString("name", Name);

            if (IsAssert)
                writer.WriteAttributeString("assert", AssertValue.ToString());

            if (IsOptional)
                writer.WriteAttributeString("optional", "true");

            if (IsXmlAttribute)
                writer.WriteAttributeString("attribute", "true");

            if (IsNodeFlattened)
                writer.WriteAttributeString("flattened", "true");

            if (Tag != null)
                writer.WriteAttributeString("tag", Tag);

            

            writer.WriteEndElement();
        }

        public static bool IsStandardValueType(string valueType)
        {
            return (valueType == "b" ||
                valueType == "u8" || valueType == "s8" || valueType == "x8" ||
                valueType == "u16" || valueType == "s16" || valueType == "x16" ||
                valueType == "u32" || valueType == "s32" || valueType == "x32" ||
                valueType == "u64" || valueType == "s64" || valueType == "x64" ||
                valueType == "f32" || valueType == "f64");
        }

        public static object ReadStandardValueFromString(string valueType, string s, bool justReturnStringForNonStandardValue = false)
        {
            switch (valueType)
            {

                case "b":
                    string check = s.ToUpper().Trim();
                    if (check == "TRUE")
                        return true;
                    else if (check == "FALSE")
                        return false;
                    else
                        throw new XmlStructException($"Value '{s}' is not a valid boolean (<b> node) value.");

                case "u8":  
                    if (byte.TryParse(s, out byte u8Value))
                        return u8Value;
                    else
                        throw new XmlStructException($"Value '{s}' is not a valid unsigned 8-bit integer (<u8> node) value.");

                case "x8":
                    if (byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out byte x8Value))
                        return x8Value;
                    else
                        throw new XmlStructException($"Value '{s}' is not a valid hexadecimal 8-bit integer (<x8> node) value.");

                case "s8":
                    if (sbyte.TryParse(s, out sbyte s8Value))
                        return s8Value;
                    else
                        throw new XmlStructException($"Value '{s}' is not a valid signed 8-bit integer (<s8> node) value.");

                case "u16":
                    if (ushort.TryParse(s, out ushort u16Value))
                        return u16Value;
                    else
                        throw new XmlStructException($"Value '{s}' is not a valid unsigned 16-bit integer (<u16> node) value.");

                case "x16":
                    if (ushort.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out ushort x16Value))
                        return x16Value;
                    else
                        throw new XmlStructException($"Value '{s}' is not a valid hexadecimal 16-bit integer (<x16> node) value.");

                case "s16":
                    if (short.TryParse(s, out short s16Value))
                        return s16Value;
                    else
                        throw new XmlStructException($"Value '{s}' is not a valid signed 16-bit integer (<s16> node) value.");

                case "u32":
                    if (uint.TryParse(s, out uint u32Value))
                        return u32Value;
                    else
                        throw new XmlStructException($"Value '{s}' is not a valid unsigned 32-bit integer (<u32> node) value.");

                case "x32":
                    if (uint.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out uint x32Value))
                        return x32Value;
                    else
                        throw new XmlStructException($"Value '{s}' is not a valid hexadecimal 32-bit integer (<x32> node) value.");

                case "s32":
                    if (int.TryParse(s, out int s32Value))
                        return s32Value;
                    else
                        throw new XmlStructException($"Value '{s}' is not a valid signed 32-bit integer (<s32> node) value.");

                case "u64":
                    if (ulong.TryParse(s, out ulong u64Value))
                        return u64Value;
                    else
                        throw new XmlStructException($"Value '{s}' is not a valid unsigned 64-bit integer (<u64> node) value.");

                case "x64":
                    if (ulong.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out ulong x64Value))
                        return x64Value;
                    else
                        throw new XmlStructException($"Value '{s}' is not a valid hexadecimal 64-bit integer (<x64> node) value.");

                case "s64":
                    if (int.TryParse(s, out int s64Value))
                        return s64Value;
                    else
                        throw new XmlStructException($"Value '{s}' is not a valid signed 64-bit integer (<s64> node) value.");

                case "f32":
                    if (float.TryParse(s, out float f32Value))
                    {
                        if (f32Value == 0 && s.StartsWith("-"))
                            return -0f;
                        else
                            return f32Value;
                    }
                    else
                        throw new XmlStructException($"Value '{s}' is not a valid 32-bit floating point (<f32> node) value.");

                case "f64":
                    if (double.TryParse(s, out double f64Value))
                        return f64Value;
                    else
                        throw new XmlStructException($"Value '{s}' is not a valid 64-bit floating point (<f64> node) value.");

                default:
                    if (justReturnStringForNonStandardValue)
                    {
                        return s;
                    }
                    else
                    {
                        throw new Exception($"Value type '{valueType}' is not a standard type " +
                           $"and must be handled manually instead of calling {nameof(XmlStructDefField)}." +
                           $"{nameof(ReadStandardValueFromString)}.");
                    }
                    
            }
        }

        public object ReadStandardValueFromRaw(BinaryReaderEx br)
        {
            object valueThatWasRead = null;

            switch (ValueType)
            {
                
                case "b": valueThatWasRead = br.ReadBoolean(); break;
                case "u8": case "x8": valueThatWasRead = br.ReadByte(); break;
                case "s8": valueThatWasRead = br.ReadSByte(); break;
                case "u16": case "x16": valueThatWasRead = br.ReadUInt16(); break;
                case "s16": valueThatWasRead = br.ReadInt16(); break;
                case "u32": case "x32": valueThatWasRead = br.ReadUInt32(); break;
                case "s32": valueThatWasRead = br.ReadInt32(); break;
                case "u64": case "x64": valueThatWasRead = br.ReadUInt64(); break;
                case "s64": valueThatWasRead = br.ReadInt64(); break;
                case "f32": valueThatWasRead = br.ReadSingle(); break;
                case "f64": valueThatWasRead = br.ReadDouble(); break;
                default: throw new Exception($"Value type '{ValueType}' is not a standard type " +
                    $"and must be handled manually instead of calling {nameof(XmlStructDefField)}." +
                    $"{nameof(ReadStandardValueFromRaw)}.");
            }

            if (IsAssert && !AssertValue.Equals(valueThatWasRead))
            {
                throw new XmlStructException($"Assert failed for field '{Name}'. " +
                    $"Read '{valueThatWasRead}', expected to read '{AssertValue}'.");
            }

            return valueThatWasRead;
        }


        public void WriteStandardValueTypeToRaw(BinaryWriterEx bw, object value)
        {
            switch (ValueType)
            {
                case "aob": bw.WriteBytes((byte[])value); break;
                case "b": bw.WriteBoolean((bool)value); break;
                case "u8": case "x8": bw.WriteByte((byte)value); break;
                case "s8": bw.WriteSByte((sbyte)value); break;
                case "u16": case "x16": bw.WriteUInt16((ushort)value); break;
                case "s16": bw.WriteInt16((short)value); break;
                case "u32": case "x32": bw.WriteUInt32((uint)value); break;
                case "s32": bw.WriteInt32((int)value); break;
                case "u64": case "x64": bw.WriteUInt64((ulong)value); break;
                case "s64": bw.WriteInt64((long)value); break;
                case "f32": bw.WriteSingle((float)value); break;
                case "f64": bw.WriteDouble((double)value); break;
                default: throw new Exception($"Value type '{ValueType}' is not a standard type " +
                    $"and must be handled manually instead of calling {nameof(XmlStructDefField)}." +
                    $"{nameof(WriteStandardValueTypeToRaw)}.");
            }
        }
    }
}
