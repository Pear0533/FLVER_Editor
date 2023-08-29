using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SoulsAssetPipeline.XmlStructs
{
    public static class XmlStructExtensionMethods
    {
        public static void WriteStructField(this XmlWriter writer, string fieldType, string fieldName, Action doWrite)
        {
            writer.WriteStartElement(fieldType);
            writer.WriteAttributeString("name", fieldName);
            doWrite();
            writer.WriteEndElement();
        }

        public static T ReadStructField<T>(this XmlNode xmlNode, string fieldType, string fieldName, Action<XmlNode, T> doReadXml, bool optional = false)
            where T : class, new()
        {
            var newThing = new T();

            foreach (XmlNode childNode in xmlNode.ChildNodes)
            {
                if (childNode.Name == fieldType && childNode.SafeGetAttribute("name", optional: true) == fieldName)
                {
                    doReadXml(childNode, newThing);
                    return newThing;
                }
            }
            if (!optional)
                throw new XmlStructException($"Struct field <{fieldType} name=\"{fieldName}'\" was not present " +
                    $"in this struct, but it is not optional.");

            return null;
        }

        public static XmlNode SelectStructField(this XmlNode xmlNode, string fieldName, bool optional, params string[] fieldTypes)
        {
            foreach (XmlNode childNode in xmlNode.ChildNodes)
            {
                if (fieldTypes.Contains(childNode.Name) && childNode.SafeGetAttribute("name", optional: true) == fieldName)
                    return childNode;
            }
            if (!optional)
                throw new XmlStructException($"Struct field <{(string.Join("|", fieldTypes))} name=\"{fieldName}'\" was not present " +
                    $"in this struct, but it is not optional.");

            return null;
        }

        public static string SafeGetAttribute(this XmlNode xmlNode, string attributeName, bool optional = false)
        {
            var nodeText = xmlNode.Attributes[attributeName]?.InnerText;

            if (nodeText == null && !optional)
                throw new XmlStructException($"XML attribute '{attributeName}' was not defined " +
                    $"in this <{xmlNode.Name}> node, but that attribute is not optional in this node type.");

            return nodeText;
        }

        public static bool SafeGetBoolAttribute(this XmlNode xmlNode, string attributeName)
        {
            string txt = xmlNode.SafeGetAttribute(attributeName);
            string check = txt.ToUpper().Trim();

            if (check == "TRUE")
                return true;
            else if (check == "FALSE")
                return false;
            else
                throw new XmlStructException($"XML attribute '{attributeName}' is a boolean " +
                    $"integer attribute and '{txt}' is not a valid boolean value. Expected 'true' or 'false'.");
        }

        public static bool? SafeGetOptionalBoolAttribute(this XmlNode xmlNode, string attributeName)
        {
            string txt = xmlNode.SafeGetAttribute(attributeName, optional: true);

            if (txt == null)
                return null;

            string check = txt.ToUpper().Trim();

            if (check == "TRUE")
                return true;
            else if (check == "FALSE")
                return false;
            else
                throw new XmlStructException($"XML attribute '{attributeName}' is a boolean " +
                    $"integer attribute and '{txt}' is not a valid boolean value. Expected 'true' or 'false'.");
        }

        public static byte SafeGetUInt8Attribute(this XmlNode xmlNode, string attributeName)
        {
            string txt = xmlNode.SafeGetAttribute(attributeName);
            if (byte.TryParse(txt, out byte intVal))
                return intVal;
            else
                throw new XmlStructException($"XML attribute '{attributeName}' is an unsigned 8-bit " +
                    $"integer attribute and '{txt}' is not a valid unsigned 8-bit integer value.");
        }

        public static byte? SafeGetOptionalUInt8Attribute(this XmlNode xmlNode, string attributeName)
        {
            string txt = xmlNode.SafeGetAttribute(attributeName, optional: true);

            if (txt == null)
                return null;

            if (byte.TryParse(txt, out byte intVal))
                return intVal;
            else
                throw new XmlStructException($"XML attribute '{attributeName}' is an unsigned 8-bit " +
                    $"integer attribute and '{txt}' is not a valid unsigned 8-bit integer value.");
        }

        public static short SafeGetInt16Attribute(this XmlNode xmlNode, string attributeName)
        {
            string txt = xmlNode.SafeGetAttribute(attributeName);
            if (short.TryParse(txt, out short intVal))
                return intVal;
            else
                throw new XmlStructException($"XML attribute '{attributeName}' is a signed 16-bit " +
                    $"integer attribute and '{txt}' is not a valid signed 16-bit integer value.");
        }

        public static short? SafeGetOptionalInt16Attribute(this XmlNode xmlNode, string attributeName)
        {
            string txt = xmlNode.SafeGetAttribute(attributeName, optional: true);

            if (txt == null)
                return null;

            if (short.TryParse(txt, out short intVal))
                return intVal;
            else
                throw new XmlStructException($"XML attribute '{attributeName}' is a signed 16-bit " +
                    $"integer attribute and '{txt}' is not a valid signed 16-bit integer value.");
        }

        public static int SafeGetInt32Attribute(this XmlNode xmlNode, string attributeName)
        {
            string txt = xmlNode.SafeGetAttribute(attributeName);
            if (int.TryParse(txt, out int intVal))
                return intVal;
            else
                throw new XmlStructException($"XML attribute '{attributeName}' is a signed 32-bit " +
                    $"integer attribute and '{txt}' is not a valid 32-bit integer value.");
        }

        public static int? SafeGetOptionalInt32Attribute(this XmlNode xmlNode, string attributeName)
        {
            string txt = xmlNode.SafeGetAttribute(attributeName, optional: true);

            if (txt == null)
                return null;

            if (int.TryParse(txt, out int intVal))
                return intVal;
            else
                throw new XmlStructException($"XML attribute '{attributeName}' is a signed 32-bit " +
                    $"integer attribute and '{txt}' is not a valid 32-bit integer value.");
        }

        public static float SafeGetFloatAttribute(this XmlNode xmlNode, string attributeName)
        {
            string txt = xmlNode.SafeGetAttribute(attributeName);
            if (float.TryParse(txt, out float floatVal))
            {
                if (floatVal == 0 && txt.StartsWith("-"))
                    return -0f;
                else
                    return floatVal;
            }
            else
                throw new XmlStructException($"XML attribute '{attributeName}' is a 32-bit floating point " +
                    $"number attribute and '{txt}' is not a valid 32-bit floating point number value.");
        }

        public static float? SafeGetOptionalFloatAttribute(this XmlNode xmlNode, string attributeName)
        {
            string txt = xmlNode.SafeGetAttribute(attributeName, optional: true);

            if (txt == null)
                return null;

            if (float.TryParse(txt, out float intVal))
                return intVal;
            else
                throw new XmlStructException($"XML attribute '{attributeName}' is a 32-bit floating point " +
                    $"number attribute and '{txt}' is not a valid 32-bit floating point number value.");
        }


        public static int ReadFXR1Varint(this BinaryReaderEx br)
        {
            int result = br.ReadInt32();
            br.AssertFXR1Garbage();
            return result;
        }

        public static void AssertFXR1Garbage(this BinaryReaderEx br)
        {
            if (br.VarintLong)
                br.AssertUInt32(0, 0xCDCDCDCD);
        }

        public static int GetFXR1Varint(this BinaryReaderEx br, long offset)
        {
            int result = -1;
            br.StepIn(offset);
            result = br.ReadInt32();
            br.AssertFXR1Garbage();
            br.StepOut();
            return result;
        }

        public static int AssertFXR1Varint(this BinaryReaderEx br, params int[] v)
        {
            int result = br.AssertInt32(v);
            br.AssertFXR1Garbage();
            return result;
        }

        public static float ReadFXR1Single(this BinaryReaderEx br)
        {
            float result = br.ReadSingle();
            br.AssertFXR1Garbage();
            return result;
        }

        public static void WriteFXR1Garbage(this BinaryWriterEx bw)
        {
            //if (bw.VarintLong)
            //    bw.WriteUInt32(0xCDCDCDCD);
            if (bw.VarintLong)
                bw.WriteUInt32(0);
        }

        public static void WriteFXR1Varint(this BinaryWriterEx bw, int v)
        {
            bw.WriteInt32(v);
            bw.WriteFXR1Garbage();
        }

        public static void ReserveFXR1Varint(this BinaryWriterEx bw, string name)
        {
            bw.ReserveInt32(name);
            bw.WriteFXR1Garbage();
        }

        public static void FillFXR1Varint(this BinaryWriterEx bw, string name, int v)
        {
            bw.FillInt32(name, v);
            //bw.WriteFXR1Garbage();
        }

        public static void WriteFXR1Single(this BinaryWriterEx bw, float v)
        {
            bw.WriteSingle(v);
            bw.WriteFXR1Garbage();
        }

        private struct FloatCompareStruct
        {
            public float Val;
        }

        static FloatCompareStruct IsNegativeZero_NegativeZeroConst = new FloatCompareStruct() { Val = -0f };

        public static bool IsNegativeZero(this float f)
        {
            var asStruct = new FloatCompareStruct()
            {
                Val = f
            };
            return asStruct.Equals(IsNegativeZero_NegativeZeroConst);
        }

        public static string ToStringEX(this float f)
        {
            if (f.IsNegativeZero())
                return "-0";
            else
                return f.ToString("G9");
        }

        public static float ParseFloatEX(string s)
        {
            float f = float.Parse(s);
            if (f == 0 && s.StartsWith("-"))
                return -0f;
            else
                return f;
        }
    }
}
