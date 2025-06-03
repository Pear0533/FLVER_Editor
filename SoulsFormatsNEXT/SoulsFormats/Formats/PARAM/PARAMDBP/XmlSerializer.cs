using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace SoulsFormats
{
    public partial class PARAMDBP
    {
        /// <summary>
        /// Serializes and deserializes params and dbps to xml.
        /// </summary>
        public class XmlSerializer
        {
            #region Serialize

            /// <summary>
            /// Serialize a dbp to an xml file.
            /// </summary>
            /// <param name="dbp">A dbp to serialize.</param>
            /// <param name="path">The path to serialize an XML to.</param>
            public static void SerializeDbp(PARAMDBP dbp, string path)
            {
                var xws = new XmlWriterSettings
                {
                    Indent = true,
                    Encoding = SFEncoding.ShiftJIS
                };

                using (var xw = XmlWriter.Create(path, xws))
                {
                    SerializeDbp(dbp, xw, Path.GetFileName(path));
                }
            }

            /// <summary>
            /// Serialize a dbp to an xml.
            /// </summary>
            /// <param name="dbp">A dbp to serialize.</param>
            /// <param name="xw">An xml writer.</param>
            /// <param name="name">The name of the dbp.</param>
            public static void SerializeDbp(PARAMDBP dbp, XmlWriter xw, string name)
            {
                xw.WriteStartElement("dbp");
                xw.WriteElementString("BigEndian", dbp.BigEndian.ToString());
                xw.WriteElementString("Name", name);
                xw.WriteStartElement("Fields");
                foreach (var field in dbp.Fields)
                {
                    xw.WriteStartElement("Field");
                    xw.WriteElementString("DisplayName", field.DisplayName);
                    xw.WriteElementString("DisplayFormat", field.DisplayFormat);
                    xw.WriteElementString("DisplayType", field.DisplayType.ToString());
                    xw.WriteElementString("Default", field.Default.ToString());
                    xw.WriteElementString("Minimum", field.Minimum.ToString());
                    xw.WriteElementString("Maximum", field.Maximum.ToString());
                    xw.WriteElementString("Increment", field.Increment.ToString());
                    xw.WriteEndElement();
                }
                xw.WriteEndElement();
                xw.WriteEndElement();
                xw.Dispose();
            }

            /// <summary>
            /// Serialize a param to an xml file.
            /// </summary>
            /// <param name="param">A param to serialize.</param>
            /// <param name="path">The path to serialize an XML to.</param>
            public static void SerializeParam(DBPPARAM param, string path)
            {
                var xws = new XmlWriterSettings
                {
                    Indent = true,
                    Encoding = SFEncoding.ShiftJIS
                };

                using (var xw = XmlWriter.Create(path, xws))
                {
                    SerializeParam(param, xw, Path.GetFileName(path));
                }
            }

            /// <summary>
            /// Serialize a param that has an applied dbp to an xml.
            /// </summary>
            /// <param name="param">A param to serialize.</param>
            /// <param name="xw">An xml writer.</param>
            /// <param name="name">The name of the param.</param>
            public static void SerializeParam(DBPPARAM param, XmlWriter xw, string name)
            {
                if (!param.DbpApplied)
                    throw new InvalidDataException("Dbp has not been applied.");

                xw.WriteStartElement("dbpparam");
                xw.WriteElementString("Name", name);
                xw.WriteStartElement("Cells");
                foreach (var cell in param.Cells)
                {
                    xw.WriteStartElement("Cell");
                    xw.WriteElementString("DisplayName", cell.DisplayName);
                    xw.WriteElementString("DisplayFormat", cell.DisplayFormat);
                    xw.WriteElementString("DisplayType", cell.DisplayType.ToString());
                    xw.WriteElementString("Default", cell.Default.ToString());
                    xw.WriteElementString("Minimum", cell.Minimum.ToString());
                    xw.WriteElementString("Maximum", cell.Maximum.ToString());
                    xw.WriteElementString("Increment", cell.Increment.ToString());
                    xw.WriteElementString("Value", cell.Value.ToString());
                    xw.WriteEndElement();
                }
                xw.WriteEndElement();
                xw.WriteEndElement();
                xw.Dispose();
            }

            #endregion

            #region Deserialize

            /// <summary>
            /// Deserialize an xml on the specified path to a dbp.
            /// </summary>
            /// <param name="path">The file path to an xml.</param>
            /// <returns>A new dbp.</returns>
            public static PARAMDBP DeserializeDbp(string path)
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(path);
                return DeserializeDbp(xml);
            }

            /// <summary>
            /// Deserialize an xml to a dbp.
            /// </summary>
            /// <param name="xml">An xml document.</param>
            /// <returns>A new dbp.</returns>
            public static PARAMDBP DeserializeDbp(XmlDocument xml)
            {
                var dbp = new PARAMDBP();
                bool bigendian = bool.Parse(xml.SelectSingleNode("dbp/BigEndian").InnerText);
                var fieldsNode = xml.SelectSingleNode("dbp/Fields");
                foreach (XmlNode fieldNode in fieldsNode.SelectNodes("Field"))
                {
                    var field = new Field();
                    string name = fieldNode.SelectSingleNode("DisplayName").InnerText;
                    string format = fieldNode.SelectSingleNode("DisplayFormat").InnerText;
                    var type = (PARAMDBP.DbpType)Enum.Parse(typeof(PARAMDBP.DbpType), fieldNode.SelectSingleNode("DisplayType").InnerText);
                    object defaultValue = Field.ConvertToDbpType(fieldNode.SelectSingleNode("Default").InnerText, type);
                    object minimum = Field.ConvertToDbpType(fieldNode.SelectSingleNode("Minimum").InnerText, type);
                    object maximum = Field.ConvertToDbpType(fieldNode.SelectSingleNode("Maximum").InnerText, type);

                    object increment = Field.ConvertToDbpType(fieldNode.SelectSingleNode("Increment").InnerText, type);

                    field.DisplayName = name;
                    field.DisplayFormat = format;
                    field.DisplayType = type;
                    field.Default = defaultValue;
                    field.Minimum = minimum;
                    field.Maximum = maximum;
                    field.Increment = increment;
                    dbp.Fields.Add(field);
                }

                dbp.BigEndian = bigendian;

                return dbp;
            }

            /// <summary>
            /// Deserialize an xml on the specified path to a param.
            /// </summary>
            /// <param name="path">The file path to an xml.</param>
            /// <returns>A new param.</returns>
            public static DBPPARAM DeserializeParam(string path)
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(path);
                return DeserializeParam(xml);
            }

            /// <summary>
            /// Deserialize an xml to a param.
            /// </summary>
            /// <param name="xml">An xml document.</param>
            /// <returns>A new param.</returns>
            public static DBPPARAM DeserializeParam(XmlDocument xml)
            {
                var dbp = new PARAMDBP();
                var cellsNode = xml.SelectSingleNode("dbpparam/Cells");
                var paramValues = new List<object>();
                foreach (XmlNode cellNode in cellsNode.SelectNodes("Cell"))
                {
                    var field = new Field();
                    string name = cellNode.SelectSingleNode("DisplayName").InnerText;
                    string format = cellNode.SelectSingleNode("DisplayFormat").InnerText;
                    var type = (PARAMDBP.DbpType)Enum.Parse(typeof(PARAMDBP.DbpType), cellNode.SelectSingleNode("DisplayType").InnerText);
                    object defaultValue = Field.ConvertToDbpType(cellNode.SelectSingleNode("Default").InnerText, type);
                    object minimum = Field.ConvertToDbpType(cellNode.SelectSingleNode("Minimum").InnerText, type);
                    object maximum = Field.ConvertToDbpType(cellNode.SelectSingleNode("Maximum").InnerText, type);
                    object increment = Field.ConvertToDbpType(cellNode.SelectSingleNode("Increment").InnerText, type);
                    object value = Field.ConvertToDbpType(cellNode.SelectSingleNode("Value").InnerText, type);

                    field.DisplayName = name;
                    field.DisplayFormat = format;
                    field.DisplayType = type;
                    field.Default = defaultValue;
                    field.Minimum = minimum;
                    field.Maximum = maximum;
                    field.Increment = increment;
                    paramValues.Add(value);

                    dbp.Fields.Add(field);
                }

                var param = new DBPPARAM(dbp);
                for (int i = 0; i < paramValues.Count; i++)
                    param.Cells[i].Value = paramValues[i];

                return param;
            }

            #endregion
        }
    }
}