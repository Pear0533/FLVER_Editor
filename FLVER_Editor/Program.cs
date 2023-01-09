using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using SoulsFormats;

namespace FLVER_Editor
{
    public class VertexInfo
    {
        public int meshIndex;
        public uint vertexIndex;
    }

    internal static partial class Program
    {
        public static FLVER flver;
        public static TPF tpf = null;
        public static List<VertexInfo> verticesInfo = new List<VertexInfo>();
        public static Vector3D[] bonePosList = new Vector3D[1000];
        public static Dictionary<string, string> boneParentList;
        public static List<FLVER.Vertex> vertices = new List<FLVER.Vertex>();
        public static string filePath = "";
        public static Vector3 checkingPoint;
        public static Vector3 checkingPointNormal;
        public static bool useCheckingPoint;
        public static bool loadTexture = true;
        public static int boneFindParentTimes = 15;
        public static bool boneDisplay = true;
        public static bool dummyDisplay = true;
        public static bool setVertexPos = false;
        public static float setVertexX = 0;
        public static float setVertexY = 1.75f;
        public static float setVertexZ = 0;
        public static MainWindow window;
        public static string windowTitle = "FLVER Editor";

        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            MainWindow.arguments = args.ToList();
            window = new MainWindow();
            window.Text = windowTitle;
            window.ShowDialog();
        }

        private static int FindBoneIndexByName(FLVER flverFile, string name)
        {
            for (var i = 0; i < flverFile.Bones.Count; ++i)
                if (flverFile.Bones[i].Name == name)
                    return i;
            return -1;
        }

        public static void SetMaterialPath(FLVER.Material material, string typeName, string newPath)
        {
            foreach (FLVER.Texture t in material.Textures.Where(t => t.Type == typeName))
            {
                t.Path = newPath;
                return;
            }
            var newTexture = new FLVER.Texture { Type = typeName, Path = newPath, ScaleX = 1, ScaleY = 1, Unk10 = 1, Unk11 = true };
            material.Textures.Add(newTexture);
        }
    }
}