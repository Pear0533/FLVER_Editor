using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using ObjLoader.Loader.Data.Elements;
using SoulsAssetPipeline.FLVERImporting;
using SoulsFormats;

namespace FLVER_Editor;

/// <summary>
///     Part of the program class
///     Contains some important tools
///     Might need to be sorted a bit more
/// </summary>
internal static partial class Program
{
    public enum TextureFormats
    {
        DXT1 = 0,
        BC7_UNORM = 107,
        ATI1 = 103,
        ATI2 = 104,
        ATI3 = 103
    }
    public static List<string> MTDs;
    public static int DefaultMTDIndex = 0;
    public static FLVER2MaterialInfoBank MaterialInfoBank;

    /*************** Basic Tools section *****************/

    public static string RemoveIndexSuffix(string filePath)
    {
        return Regex.Replace(Regex.Replace(filePath, @"_\d\.", "."), @"_\d_", "_");
    }

    public static string GetFlverVersion(FLVER2 flver)
    {
        return flver.Header.Version switch
        {
            131084 => "DS1",
            131088 => "DS2",
            131092 => "DS3",
            131098 when flver.Materials.Any(x => x.MTD.Contains(".matxml")) => "ER",
            131098 => MainWindow.ShowSelectorDialog("Choose target game:", "Target Game",
                    new object[] { "Elden Ring", "Sekiro" }) switch
            {
                "Elden Ring" => "ER",
                "Sekiro" => "SDT",
                null => null,
                _ => throw new ArgumentOutOfRangeException()
            },
            131099 => "AC6",
            _ => throw new InvalidDataException("Invalid Flver Version")
        };
    }

    public static bool PopulateMaterials(FLVER2 flver)
    {
        string basePath = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "SapResources", "FLVER2MaterialInfoBank");
        string version = GetFlverVersion(flver);
        string bankFileName = $"Bank{version}.xml";
        string xmlPath = Path.Join(basePath, bankFileName);
        try
        {
            MaterialInfoBank = FLVER2MaterialInfoBank.ReadFromXML(xmlPath);
        }
        catch
        {
            MainWindow.ShowErrorDialog($"Warning: {version} material bank could not be found.");
            return false;
        }
        MTDs = new List<string>(MaterialInfoBank.MaterialDefs.Keys.Where(x => !string.IsNullOrEmpty(x)).OrderBy(x => x));

        var defaultMtdIndex = MTDs.FindIndex(x => x.Contains("c[amsn]_e.mtd", StringComparison.OrdinalIgnoreCase));

        if (defaultMtdIndex >= 0)
        {
            DefaultMTDIndex = defaultMtdIndex;
        }

        return true;
    }

    public static FLVER2.Material GetMaterialFromMTD(FLVER2 flver, string name, string mtd)
    {
        string[] nameParts = name.Split('|').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        string trimmedName = nameParts.Length > 1 ? nameParts[0] : name;
        List<FLVER2.Texture> textures = new List<FLVER2.Texture>();
        if (MTDs.Count > 0)
        {
            textures = new List<FLVER2.Texture>(MaterialInfoBank.MaterialDefs[mtd].TextureChannels.Values.Select(x => new FLVER2.Texture { Type = x }));
        }
        FLVER2.Material newMaterial = new()
        {
            Name = trimmedName,
            MTD = mtd,
            Unk18 = flver.Materials.Count,
            Textures = textures
        };
        return newMaterial;
    }

    public static FLVER2.GXList GetGXListFromMTD(string mtd)
    {
        FLVER2.GXList gxList = new();
        if (!string.IsNullOrEmpty(mtd))
        {
            gxList.AddRange(MaterialInfoBank.GetDefaultGXItemsForMTD(mtd));
        }
        return gxList;
    }

    /// <summary>
    ///     Rotate a numerics Vector3 point
    /// </summary>
    /// <param name="point">The numerics Vector3 point to rotate</param>
    /// <param name="pitch">The pitch to rotate by as a float</param>
    /// <param name="roll">The roll to rotate by as a float</param>
    /// <param name="yaw">The yaw to rotate by as a float</param>
    /// <returns>A rotated numerics Vector3</returns>
    public static Vector3 RotatePoint(Vector3 point, float pitch, float roll, float yaw)
    {
        Vector3 ans = new(0, 0, 0);
        double cosa = Math.Cos(yaw);
        double sina = Math.Sin(yaw);
        double cosb = Math.Cos(pitch);
        double sinb = Math.Sin(pitch);
        double cosc = Math.Cos(roll);
        double sinc = Math.Sin(roll);
        double Axx = cosa * cosb;
        double Axy = cosa * sinb * sinc - sina * cosc;
        double Axz = cosa * sinb * cosc + sina * sinc;
        double Ayx = sina * cosb;
        double Ayy = sina * sinb * sinc + cosa * cosc;
        double Ayz = sina * sinb * cosc - cosa * sinc;
        double Azx = -sinb;
        double Azy = cosb * sinc;
        double Azz = cosb * cosc;
        float px = point.X;
        float py = point.Y;
        float pz = point.Z;
        ans.X = (float)(Axx * px + Axy * py + Axz * pz);
        ans.Y = (float)(Ayx * px + Ayy * py + Ayz * pz);
        ans.Z = (float)(Azx * px + Azy * py + Azz * pz);
        return ans;
    }

    /// <summary>
    ///     Rotate a numerics Vector4 point
    /// </summary>
    /// <param name="point">The numerics Vector4 point to rotate</param>
    /// <param name="pitch">The pitch to rotate by as a float</param>
    /// <param name="roll">The roll to rotate by as a float</param>
    /// <param name="yaw">The yaw to rotate by as a float</param>
    /// <returns>A rotated numerics Vector4</returns>
    public static Vector4 RotatePoint(Vector4 point, float pitch, float roll, float yaw)
    {
        Vector4 ans = new(0, 0, 0, point.W);
        double cosa = Math.Cos(yaw);
        double sina = Math.Sin(yaw);
        double cosb = Math.Cos(pitch);
        double sinb = Math.Sin(pitch);
        double cosc = Math.Cos(roll);
        double sinc = Math.Sin(roll);
        double Axx = cosa * cosb;
        double Axy = cosa * sinb * sinc - sina * cosc;
        double Axz = cosa * sinb * cosc + sina * sinc;
        double Ayx = sina * cosb;
        double Ayy = sina * sinb * sinc + cosa * cosc;
        double Ayz = sina * sinb * cosc - cosa * sinc;
        double Azx = -sinb;
        double Azy = cosb * sinc;
        double Azz = cosb * cosc;
        float px = point.X;
        float py = point.Y;
        float pz = point.Z;
        ans.X = (float)(Axx * px + Axy * py + Axz * pz);
        ans.Y = (float)(Ayx * px + Ayy * py + Ayz * pz);
        ans.Z = (float)(Azx * px + Azy * py + Azz * pz);
        return ans;
    }

    /// <summary>
    ///     Rotate a line on a shape with numerics Vector3 points
    /// </summary>
    /// <param name="point"></param>
    /// <param name="org"></param>
    /// <param name="direction"></param>
    /// <param name="theta"></param>
    /// <returns></returns>
    public static Vector3 RotateLine(Vector3 point, Vector3 org, Vector3 direction, double theta)
    {
        double x = point.X;
        double y = point.Y;
        double z = point.Z;
        double a = org.X;
        double b = org.Y;
        double c = org.Z;
        double nu = direction.X / direction.Length();
        double nv = direction.Y / direction.Length();
        double nw = direction.Z / direction.Length();
        double[] rP = new double[3];
        rP[0] = (a * (nv * nv + nw * nw) - nu * (b * nv + c * nw - nu * x - nv * y - nw * z)) * (1 - Math.Cos(theta))
            + x * Math.Cos(theta)
            + (-c * nv + b * nw - nw * y + nv * z) * Math.Sin(theta);
        rP[1] = (b * (nu * nu + nw * nw) - nv * (a * nu + c * nw - nu * x - nv * y - nw * z)) * (1 - Math.Cos(theta))
            + y * Math.Cos(theta)
            + (c * nu - a * nw + nw * x - nu * z) * Math.Sin(theta);
        rP[2] = (c * (nu * nu + nv * nv) - nw * (a * nu + b * nv - nu * x - nv * y - nw * z)) * (1 - Math.Cos(theta))
            + z * Math.Cos(theta)
            + (-b * nu + a * nv - nv * x + nu * y) * Math.Sin(theta);
        Vector3 ans = new((float)rP[0], (float)rP[1], (float)rP[2]);
        return ans;
    }

    /// <summary>
    ///     Computes the Dot Product of two Xna Vector3s
    /// </summary>
    /// <param name="vector1">The first Xna Vector3</param>
    /// <param name="vector2">The second Xna Vector3</param>
    /// <returns>The Dot Product of the Xna Vector3s as an Xna Vector3</returns>
    public static float XnaDotProduct(Microsoft.Xna.Framework.Vector3 vector1, Microsoft.Xna.Framework.Vector3 vector2)
    {
        float x1 = vector1.X;
        float y1 = vector1.Y;
        float z1 = vector1.Z;
        float x2 = vector2.X;
        float y2 = vector2.Y;
        float z2 = vector2.Z;
        return x1 * x2 + y1 * y2 + z1 * z2;
    }

    /// <summary>
    ///     Computes the Cross Product of two Xna Vector3s
    /// </summary>
    /// <param name="vector1">The first Xna Vector3</param>
    /// <param name="vector2">The second Xna Vector3</param>
    /// <returns>The Cross Product of the Xna Vector3s as an Xna Vector3</returns>
    public static Microsoft.Xna.Framework.Vector3 XnaCrossProduct(Microsoft.Xna.Framework.Vector3 vector1, Microsoft.Xna.Framework.Vector3 vector2)
    {
        float x1 = vector1.X;
        float y1 = vector1.Y;
        float z1 = vector1.Z;
        float x2 = vector2.X;
        float y2 = vector2.Y;
        float z2 = vector2.Z;
        return new Microsoft.Xna.Framework.Vector3(y1 * z2 - z1 * y2, z1 * x2 - x1 * z2, x1 * y2 - y1 * x2);
    }

    /// <summary>
    ///     Swaps a sekiro model with a Dark Souls or BloodBorne model
    /// </summary>
    public static void ModelSwapModule()
    {
        OpenFileDialog openSeikiroModelDonorDialog;
        openSeikiroModelDonorDialog = new OpenFileDialog
        {
            InitialDirectory = Directory.GetCurrentDirectory(),
            Title = "Choose template seikiro model file."
        };
        if (openSeikiroModelDonorDialog.ShowDialog() == DialogResult.OK) Console.WriteLine(openSeikiroModelDonorDialog.FileName);
        else return;
        FLVER2 seikiroDonorModel = FLVER2.Read(openSeikiroModelDonorDialog.FileName);
        OpenFileDialog openReplacementModelDialog = new()
        {
            InitialDirectory = Directory.GetCurrentDirectory(),
            Title = "Choose source DS/BB model file."
        };
        if (openReplacementModelDialog.ShowDialog() == DialogResult.OK) Console.WriteLine(openSeikiroModelDonorDialog.FileName);
        else return;
        FLVER2 replacementModel = FLVER2.Read(openReplacementModelDialog.FileName);
        Console.WriteLine(seikiroDonorModel.Header);
        Console.WriteLine("Seikiro unk is:" + seikiroDonorModel.SekiroUnk);
        Console.WriteLine("Material:");
        foreach (FLVER2.Material material in seikiroDonorModel.Materials) Console.WriteLine(material.Name);
        foreach (FLVER2.Mesh material in seikiroDonorModel.Meshes) Console.WriteLine("Mesh#" + material.MaterialIndex);

        //* new
        //b.Header.BigEndian = src.Header.BigEndian;

        //

        //X: is not the sword axis!!!
        //Y: ++ means closer to the hand!
        //Unit: in meter(?)

        //For Moonlight sword -> threaded cane, Y+0.5f
        Form form = new();
        Label label = Forms.MakeLabel(new Size(150, 15), new Point(10, 20), "x,y,z offset? Y= weapon length axis,Y+=Closer to hand");
        form.Controls.Add(label);
        TextBox textbox1 = Forms.MakeTextBox(new Size(70, 15), new Point(10, 60), "0");
        TextBox textbox2 = Forms.MakeTextBox(new Size(70, 15), new Point(10, 100), "0");
        TextBox textbox3 = Forms.MakeTextBox(new Size(70, 15), new Point(10, 140), "0");
        form.Controls.Add(textbox1);
        form.Controls.Add(textbox2);
        form.Controls.Add(textbox3);
        CheckBox copyMaterial = Forms.MakeCheckBox(new Size(70, 15), new Point(10, 160), "Copy Material");
        CheckBox copyBones = Forms.MakeCheckBox(new Size(150, 15), new Point(10, 180), "Copy Bones");
        CheckBox copyDummy = Forms.MakeCheckBox(new Size(150, 15), new Point(10, 200), "Copy Dummy");
        CheckBox weightToFirstBone = Forms.MakeCheckBox(new Size(), new Point(10, 220), "All vertex weight to first bone");
        form.Controls.Add(copyMaterial);
        form.Controls.Add(copyBones);
        form.Controls.Add(copyDummy);
        form.Controls.Add(weightToFirstBone);
        form.ShowDialog();
        float x = float.Parse(textbox1.Text);
        float y = float.Parse(textbox2.Text);
        float z = float.Parse(textbox3.Text);
        seikiroDonorModel.Meshes = replacementModel.Meshes;
        if (copyMaterial.Checked) seikiroDonorModel.Materials = replacementModel.Materials;
        if (copyBones.Checked) seikiroDonorModel.Bones = replacementModel.Bones;
        if (copyDummy.Checked) seikiroDonorModel.Dummies = replacementModel.Dummies;
        if (weightToFirstBone.Checked)
        {
            for (int i = 0; i < seikiroDonorModel.Meshes.Count; i++)
            {
                seikiroDonorModel.Meshes[i].BoneIndices = new List<int> { 0, 1 };
                seikiroDonorModel.Meshes[i].DefaultBoneIndex = 1;
                foreach (FLVER.Vertex vertex in seikiroDonorModel.Meshes[i].Vertices)
                {
                    if (Util3D.BoneWeightsToFloatArray(vertex.BoneWeights) == null) continue;
                    vertex.Position = new Vector3(0, 0, 0);
                    for (int k = 0; k < vertex.BoneWeights.Length; k++)
                    {
                        vertex.BoneWeights[k] = 0;
                        vertex.BoneIndices[k] = 0;
                    }
                    vertex.BoneIndices[0] = 1;
                    vertex.BoneWeights[0] = 1;
                }
                //flver2.Meshes[i].Vertices = new List<FLVER.Vertex>();
            }
        }
        foreach (FLVER2.Mesh mesh in seikiroDonorModel.Meshes)
        {
            foreach (FLVER.Vertex vertex in mesh.Vertices)
            {
                vertex.Position = new Vector3(vertex.Position.X + x, vertex.Position.Y + y, vertex.Position.Z + z);
            }
        }
        seikiroDonorModel.Write(openSeikiroModelDonorDialog.FileName + "n");
        MessageBox.Show("Swap completed!", "Info");
        //Console.WriteLine("End reading");
        //Application.Exit();
    }

    /// <summary>
    ///     Gets face vertices from an ObjLoader face
    /// </summary>
    /// <param name="face">An ObjLoader face</param>
    /// <returns>An ObjLoader face vertex array</returns>
    private static FaceVertex[] GetVertices(Face face)
    {
        FaceVertex[] ans = new FaceVertex[face.Count];
        for (int i = 0; i < face.Count; i++)
        {
            ans[i] = face[i];
        }
        return ans;
    }

    /// <summary>
    ///     Export data from a single string to JSON
    ///     I honestly would have used a string array for this :fatcat:
    /// </summary>
    /// <param name="content">The string to export to JSON</param>
    /// <param name="fileName">The name of the resulting JSON file, "export.json" by default</param>
    /// <param name="endMessage">
    ///     A string containing a message to show in a MessageBox to the user after the export has
    ///     finished
    /// </param>
    public static void ExportJson(string content, string fileName = "export.json", string endMessage = "")
    {
        SaveFileDialog saveDialog = new()
        {
            FileName = fileName
        };
        if (saveDialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                StreamWriter sw = new(saveDialog.FileName);
                sw.Write(content);
                sw.Close();
                MessageBox.Show(endMessage, "Info");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" + $"Details:\n\n{ex.StackTrace}");
            }
        }
    }

    /// <summary>
    ///     Formats a JSON string into a more readable string
    /// </summary>
    /// <param name="jsonString">A</param>
    /// <returns>A string</returns>
    public static string FormatOutput(string jsonString)
    {
        StringBuilder stringBuilder = new();
        bool escaping = false;
        bool inQuotes = false;
        int indentation = 0;
        foreach (char character in jsonString)
        {
            if (escaping)
            {
                escaping = false;
                stringBuilder.Append(character);
            }
            else
            {
                if (character == '\\')
                {
                    escaping = true;
                    stringBuilder.Append(character);
                }
                else if (character == '\"')
                {
                    inQuotes = !inQuotes;
                    stringBuilder.Append(character);
                }
                else if (!inQuotes)
                {
                    if (character == ',')
                    {
                        stringBuilder.Append(character);
                        stringBuilder.Append("\r\n");
                        stringBuilder.Append('\t', indentation);
                    }
                    else if (character == '[' || character == '{')
                    {
                        stringBuilder.Append(character);
                        stringBuilder.Append("\r\n");
                        stringBuilder.Append('\t', ++indentation);
                    }
                    else if (character == ']' || character == '}')
                    {
                        stringBuilder.Append("\r\n");
                        stringBuilder.Append('\t', --indentation);
                        stringBuilder.Append(character);
                    }
                    else if (character == ':')
                    {
                        stringBuilder.Append(character);
                        stringBuilder.Append('\t');
                    }
                    else
                    {
                        stringBuilder.Append(character);
                    }
                }
                else
                {
                    stringBuilder.Append(character);
                }
            }
        }
        return stringBuilder.ToString();
    }

    /// <summary>
    ///     A class for containing a vertex normal list so that an average can be calculated
    /// </summary>
    public class VertexNormalList
    {
        public readonly List<Vector3D> normals = new();

        /// <summary>
        ///     Calculate the average of all the normals added to the normals list
        /// </summary>
        /// <returns>An FE Vector3D normal that is the average of all the normals in the normals list</returns>
        public Vector3D CalculateAvgNormal()
        {
            Vector3D normalAverage = new();
            foreach (Vector3D normal in normals)
            {
                normalAverage += normal;
            }
            return normalAverage.Normalize();
        }

        /// <summary>
        ///     Adds to the normals list of the VertexNormalList class
        /// </summary>
        /// <param name="normal">A FE Vector3D normal to add to the list</param>
        public void Add(Vector3D normal)
        {
            normals.Add(normal);
        }
    }
}