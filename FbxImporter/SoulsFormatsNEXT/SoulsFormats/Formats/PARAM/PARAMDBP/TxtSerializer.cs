using System.Collections.Generic;
using System.IO;

namespace SoulsFormats
{
    public partial class PARAMDBP
    {
        /// <summary>
        /// Serializes and deserializes params and dbps to txt.
        /// </summary>
        public class TxtSerializer
        {
            #region Serialize

            /// <summary>
            /// Serialize a dbp to a txt on the specified path.
            /// </summary>
            /// <param name="dbp">A dbp.</param>
            /// <param name="path">The path to serialize a txt to.</param>
            public static void SerializeDbp(PARAMDBP dbp, string path)
                => File.WriteAllLines(path, SerializeDbp(dbp));

            /// <summary>
            /// Serialize a dbp to a string array.
            /// </summary>
            /// <param name="dbp">A dbp.</param>
            /// <returns>A string array.</returns>
            public static string[] SerializeDbp(PARAMDBP dbp)
            {
                int fieldCount = dbp.Fields.Count;
                const int valueCount = 8;
                string[] lines = new string[fieldCount * valueCount];
                int lineIndex = 0;
                for (int i = 0; i < fieldCount; i++)
                {
                    Field field = dbp.Fields[i];
                    lines[lineIndex++] = $"[{i}]";
                    lines[lineIndex++] = $"DisplayName   : {field.DisplayName}";
                    lines[lineIndex++] = $"DisplayFormat : {field.DisplayFormat}";
                    lines[lineIndex++] = $"DisplayType   : {field.DisplayType}";
                    lines[lineIndex++] = $"Default       : {field.Default}";
                    lines[lineIndex++] = $"Increment     : {field.Increment}";
                    lines[lineIndex++] = $"Minimum       : {field.Minimum}";
                    lines[lineIndex++] = $"Maximum       : {field.Maximum}";
                }

                return lines;
            }

            /// <summary>
            /// Serialize a param that has an applied dbp to a txt on the specified path.
            /// </summary>
            /// <param name="param">A param.</param>
            /// <param name="path">The path to serialize a txt to.</param>
            public static void SerializeParam(DBPPARAM param, string path)
                => File.WriteAllLines(path, SerializeParam(param));

            /// <summary>
            /// Serialize a param that has an applied dbp to a string array.
            /// </summary>
            /// <param name="param">A param to serialize to txt.</param>
            public static string[] SerializeParam(DBPPARAM param)
            {
                if (!param.DbpApplied)
                    throw new InvalidDataException("Dbp has not been applied.");

                int cellCount = param.Cells.Count;
                const int valueCount = 9;
                string[] lines = new string[cellCount * valueCount];
                int lineIndex = 0;
                for (int i = 0; i < cellCount; i++)
                {
                    var cell = param.Cells[i];
                    lines[lineIndex++] = $"[{i}]";
                    lines[lineIndex++] = $"DisplayName   : {cell.DisplayName}";
                    lines[lineIndex++] = $"DisplayFormat : {cell.DisplayFormat}";
                    lines[lineIndex++] = $"DisplayType   : {cell.DisplayType}";
                    lines[lineIndex++] = $"Default       : {cell.Default}";
                    lines[lineIndex++] = $"Increment     : {cell.Increment}";
                    lines[lineIndex++] = $"Minimum       : {cell.Minimum}";
                    lines[lineIndex++] = $"Maximum       : {cell.Maximum}";
                    lines[lineIndex++] = $"Value         : {cell.Value}";
                }

                return lines;
            }

            #endregion

            #region Deserialize

            /// <summary>
            /// Deserialize a dbp from the specified txt path.
            /// </summary>
            /// <param name="path">The path to deserialize a txt from.</param>
            /// <returns>A new dbp.</returns>
            public static PARAMDBP DeserializeDbp(string path)
                => DeserializeDbp(File.ReadAllLines(path));

            /// <summary>
            /// Deserialize a dbp from a string array.
            /// </summary>
            /// <param name="lines">The lines to deserialize.</param>
            /// <returns>A new dbp.</returns>
            /// <exception cref="InvalidDataException">Something is wrong with the provided data.</exception>
            public static PARAMDBP DeserializeDbp(string[] lines)
            {
                const int valueCount = 8;
                if (lines.Length % valueCount != 0)
                    throw new InvalidDataException($"Invalid line count: {lines.Length} % {valueCount} != {0}");

                var dbp = new PARAMDBP();
                for (int i = 0; i < lines.Length; i++)
                {
                    // Skip the "[i]" line.
                    i++;

                    // Create a new field.
                    Field field = new Field();

                    // Skip to the colon delimiter and past its whitespace, then get the value of and advance after each line in the entry.
                    var displayName = lines[i].Substring(lines[i].IndexOf(":") + 2); i++;
                    var displayFormat = lines[i].Substring(lines[i].IndexOf(":") + 2); i++;
                    var displayType = Field.GetDbpType(lines[i].Substring(lines[i].IndexOf(":") + 2)); i++;
                    var defaultValue = Field.ConvertToDbpType(lines[i].Substring(lines[i].IndexOf(":") + 2), displayType); i++;
                    var increment = Field.ConvertToDbpType(lines[i].Substring(lines[i].IndexOf(":") + 2), displayType); i++;
                    var minimum = Field.ConvertToDbpType(lines[i].Substring(lines[i].IndexOf(":") + 2), displayType); i++;
                    var maximum = Field.ConvertToDbpType(lines[i].Substring(lines[i].IndexOf(":") + 2), displayType);

                    // Set the field values.
                    field.DisplayName = displayName;
                    field.DisplayFormat = displayFormat;
                    field.DisplayType = displayType;
                    field.Default = defaultValue;
                    field.Increment = increment;
                    field.Minimum = minimum;
                    field.Maximum = maximum;

                    // Add the field.
                    dbp.Fields.Add(field);
                }

                return dbp;
            }

            /// <summary>
            /// Deserialize a param from the specified txt path.
            /// </summary>
            /// <param name="path">The path to deserialize a txt from.</param>
            /// <returns>A new param.</returns>
            public static DBPPARAM DeserializeParam(string path)
                => DeserializeParam(File.ReadAllLines(path));

            /// <summary>
            /// Deserialize a param from a string array.
            /// </summary>
            /// <param name="lines">The lines to deserialize.</param>
            /// <returns>A new param.</returns>
            /// <exception cref="InvalidDataException">Something is wrong with the provided data.</exception>
            public static DBPPARAM DeserializeParam(string[] lines)
            {
                const int valueCount = 9;
                if (lines.Length % valueCount != 0)
                    throw new InvalidDataException($"Invalid line count: {lines.Length} % {valueCount} != {0}");

                var dbp = new PARAMDBP();
                var paramValues = new List<object>();
                for (int i = 0; i < lines.Length; i++)
                {
                    // Skip the "[i]" line.
                    i++;

                    // Create a new field.
                    Field field = new Field();

                    // Skip to the colon delimiter and past its whitespace, then get the value of and advance after each line in the entry.
                    var displayName = lines[i].Substring(lines[i].IndexOf(":") + 2); i++;
                    var displayFormat = lines[i].Substring(lines[i].IndexOf(":") + 2); i++;
                    var displayType = Field.GetDbpType(lines[i].Substring(lines[i].IndexOf(":") + 2)); i++;
                    var defaultValue = Field.ConvertToDbpType(lines[i].Substring(lines[i].IndexOf(":") + 2), displayType); i++;
                    var increment = Field.ConvertToDbpType(lines[i].Substring(lines[i].IndexOf(":") + 2), displayType); i++;
                    var minimum = Field.ConvertToDbpType(lines[i].Substring(lines[i].IndexOf(":") + 2), displayType); i++;
                    var maximum = Field.ConvertToDbpType(lines[i].Substring(lines[i].IndexOf(":") + 2), displayType); i++;
                    var value = Field.ConvertToDbpType(lines[i].Substring(lines[i].IndexOf(":") + 2), displayType);

                    // Set the field values.
                    field.DisplayName = displayName;
                    field.DisplayFormat = displayFormat;
                    field.DisplayType = displayType;
                    field.Default = defaultValue;
                    field.Increment = increment;
                    field.Minimum = minimum;
                    field.Maximum = maximum;
                    paramValues.Add(value);

                    // Add the field.
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