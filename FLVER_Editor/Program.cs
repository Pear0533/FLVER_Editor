using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using SoulsFormats;

namespace FLVER_Editor
{
    public class VertexInfo
    {
        public int MeshIndex;
        public int VertexIndex;
    }

    internal static partial class Program
    {
        public static FLVER2 Flver;
        public static TPF Tpf = null;
        public static List<VertexInfo> VerticesInfo = new List<VertexInfo>();
        public static Dictionary<string, string> BoneParentList = new Dictionary<string, string>();
        public static List<FLVER.Vertex> Vertices = new List<FLVER.Vertex>();
        public static string FilePath = "";
        public static Vector3 CheckingPoint;
        public static Vector3 CheckingPointNormal;
        public static bool UseCheckingPoint;
        public static bool LoadTexture = true;
        public static int BoneFindParentTimes = 15;
        public static bool BoneDisplay = true;
        public static bool DummyDisplay = true;
        public static bool SetVertexPos = false;
        public static float SetVertexX = 0;
        public static float SetVertexY = 1.75f;
        public static float SetVertexZ = 0;
        public static MainWindow Window;
        public static string WindowTitle = "FLVER Editor";

        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.DpiUnaware);
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            MainWindow.Arguments = args.ToList();
            Window = new MainWindow();
            Window.Text = WindowTitle;
            Window.ShowDialog();
        }
    }
}