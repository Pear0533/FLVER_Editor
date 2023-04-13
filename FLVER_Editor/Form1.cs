﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Xml.Serialization;
using Assimp;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SoulsFormats;
using Color = System.Drawing.Color;
using Matrix4x4 = System.Numerics.Matrix4x4;
using PrimitiveType = Assimp.PrimitiveType;
using Vector3 = Microsoft.Xna.Framework.Vector3;

// ReSharper disable UnusedMember.Local
// TODO: Fix dummy serialization for Elden Ring 1.09
// TODO: Investigate performance improvements for the viewer
// TODO: Allow for hierarchal models to be opened/saved

namespace FLVER_Editor
{
    public partial class MainWindow : Form
    {
        private const int mtEditButtonIndex = 6;
        private const int mtViewerHighlightButtonIndex = 7;
        private const int mtAddPresetCbIndex = 8;
        private const int mtApplyPresetCbIndex = 9;
        private const int mtDeleteCbIndex = 10;
        private const string imageFilesFilter = "DDS File (*.dds)|*.dds";
        private const string jsonFileFilter = @"JSON File (*.json)|*.json";
        private const string version = "1.88";
        private const string patreonSupportUri = "https://www.patreon.com/theonlypear";
        private const string paypalSupportUri = "https://paypal.me/realcucumberlettuce3";
        private const string baseMaterialDictKey = "Base Material";
        public static List<string> arguments;
        public static FLVER flver;
        public static List<FLVER> undoFlverList = new List<FLVER>();
        public static List<FLVER> redoFlverList = new List<FLVER>();
        public static int currUndoFlverListIndex = -1;
        public static int currRedoFlverListIndex = -1;
        public static int undoRedoStackLimit = 25;
        public static FLVER maleBodyFlver = new FLVER();
        public static FLVER femaleBodyFlver = new FLVER();
        private static byte[] currFlverBytes;
        private static BND4 flverBnd;
        private static BND4 matBinBnd;
        private static string flverFilePath;
        private static string matBinBndPath;
        private static DCX.Type flverArchiveType;
        private static int currFlverFileBinderIndex = -1;
        private static int dummyThickness = 5;
        private static Mono3D viewer;
        private static Color tabWindowBackColor = DefaultBackColor;
        private static Color tabWindowForeColor = DefaultForeColor;
        private static List<int> selectedMeshIndices = new List<int>();
        private static List<int> selectedDummyIndices = new List<int>();
        private static List<int> hiddenMeshIndices = new List<int>();
        private static readonly List<int> selectedMaterialMeshIndices = new List<int>();
        private static readonly List<Vector3D> bonePositionList = new List<Vector3D>();
        private static Dictionary<object, object> materialPresets;
        private static Dictionary<object, object> dummyPresets;
        public static RotationOrder rotOrder = RotationOrder.YZX;
        public static readonly string rootFolderPath = Path.GetDirectoryName(Application.ExecutablePath);
        private static readonly string materialPresetsFilePath = $"{rootFolderPath}/mpresets.json";
        private static readonly string dummyPresetsFilePath = $"{rootFolderPath}/dpresets.json";
        private static readonly string userConfigJsonPath = $"{rootFolderPath}/userconfig.json";
        public static JObject userConfigJson = new JObject();
        private static int currMaterialsTableSplitDistance;
        private static string currAutoSaveInterval;
        private static bool meshIsSelected;
        private static bool dummyIsSelected;
        private static bool meshIsHidden;
        private static bool isSettingDefaultInfo = true;
        public static bool textureRefreshEnabled = true;
        private static int selectedMaterialIndex = -1;
        private static float prevNumVal;
        public static bool isSnapped;
        public static bool isSnappedRight = false;
        public static bool isSnappedBottom = false;
        public static bool isSnappedLeft = false;
        public static bool isSnappedTop = false;
        public static bool areDummyIdsVisible = true;
        private static bool dispMaleBody;
        private static bool dispFemaleBody;
        private static bool stopAutoInternalIndexOverride;
        private static bool useWorldOrigin;

        public MainWindow()
        {
            InitializeComponent();
            SetVersionString();
            ReadUserConfig();
            SetEditorWindowSize();
            SetDefaultScreenPosition();
            GloballyDisableDataTableColumnSorting();
            SetMaterialsTableView();
            SetTextureRefreshEnabled();
            SetDummyThickness();
            SetAutoSaveInterval();
            SetAutoSaveInterval();
            SetAutoSaveEnabled();
            SetDummyIDsVisibility();
            SetUseWorldOrigin();
            EnableDarkTheme();
            ImportBodyModels();
            tabWindow.SelectedTab = meshTabPage;
            meshTabDataTableSelector.SelectedIndex = 0;
            Mono3D.mainForm = this;
            if (!OpenFLVERFile()) Environment.Exit(Environment.ExitCode);
        }

        public static bool IsMainWindowFocused()
        {
            IntPtr activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero) return false;
            int procId = Process.GetCurrentProcess().Id;
            GetWindowThreadProcessId(activatedHandle, out int activeProcId);
            return activeProcId == procId;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        private static void ApplyBodyModelMaterial(FLVER bodyFlver)
        {
            bodyFlver.Materials.Add(Program.GetBaseMaterial());
            bodyFlver.Meshes[0].MaterialIndex = 0;
        }

        private static void ImportBodyModels()
        {
            Program.ImportFBX($"{rootFolderPath}\\malebody.obj", true);
            Program.ImportFBX($"{rootFolderPath}\\femalebody.obj", false, true);
            ApplyBodyModelMaterial(maleBodyFlver);
            ApplyBodyModelMaterial(femaleBodyFlver);
        }

        private void SetVersionString()
        {
            versionStr.Text += $@" {version}";
        }


        private void SetTextureRefreshEnabled()
        {
            string textureRefreshStr = userConfigJson["TextureRefreshing"]?.ToString();
            if (textureRefreshStr == null) return;
            textureRefreshEnabled = bool.Parse(textureRefreshStr);
        }

        private void SetDefaultScreenPosition()
        {
            CenterToScreen();
            if (userConfigJson["ViewerSnapPosition"]?.ToString() == "Right") Left = 0;
            else Top = 0;
            TopMost = true;
        }

        public static void ReadUserConfig()
        {
            if (File.Exists(userConfigJsonPath)) userConfigJson = JObject.Parse(File.ReadAllText(userConfigJsonPath));
        }

        public static void WriteUserConfig()
        {
            File.WriteAllText(userConfigJsonPath, JsonConvert.SerializeObject(userConfigJson, Formatting.Indented));
        }

        private void SetUseWorldOrigin()
        {
            string useWorldOriginStr = userConfigJson["UseWorldOrigin"]?.ToString();
            if (useWorldOriginStr == null) return;
            useWorldOrigin = bool.Parse(useWorldOriginStr);
        }

        private void SetAutoSaveEnabled()
        {
            string autoSaveEnabledStr = userConfigJson["IsAutoSaveEnabled"]?.ToString();
            if (autoSaveEnabledStr == null) return;
            autoSaveTimer.Enabled = !bool.Parse(autoSaveEnabledStr);
            ToggleAutoSaveState(false, false);
        }

        private static void SetDummyIDsVisibility()
        {
            string dummyIdsVisibilityStr = userConfigJson["AreDummyIDsVisible"]?.ToString();
            if (dummyIdsVisibilityStr == null) return;
            areDummyIdsVisible = bool.Parse(dummyIdsVisibilityStr);
        }

        private void ToggleAutoSaveState(bool showInfo, bool writeToConfig)
        {
            if (autoSaveTimer.Enabled)
            {
                if (showInfo) ShowInformationDialog("AutoSave is now disabled!");
                autoSaveTimer.Enabled = false;
                autoSaveTimer.Stop();
            }
            else
            {
                if (showInfo) ShowInformationDialog("AutoSave is now enabled, which will now save your work automatically!");
                autoSaveTimer.Enabled = true;
                autoSaveTimer.Start();
            }
            userConfigJson["IsAutoSaveEnabled"] = autoSaveTimer.Enabled;
            if (writeToConfig) userConfigJson["IsAutoSaveEnabled"] = autoSaveTimer.Enabled;
            WriteUserConfig();
        }

        private void SetAutoSaveInterval()
        {
            try
            {
                CheckAutoSaveInterval(userConfigJson["AutoSaveInterval"]?.ToString());
            }
            catch { }
            autoSaveIntervalSelector.Text = (autoSaveTimer.Interval / 60000).ToString();
        }

        private void SetDummyThickness()
        {
            string dummyThicknessStr = userConfigJson["DummyThickness"]?.ToString();
            if (dummyThicknessStr != null)
            {
                dummyThickness = int.Parse(dummyThicknessStr);
                if (dummyThickness > dummyThicknessSelector.Items.Count) dummyThickness = 5;
            }
            dummyThicknessSelector.SelectedIndex = dummyThickness - 1;
        }

        private void SetEditorWindowSize()
        {
            string editorWindowWidthStr = userConfigJson["EditorWindowWidth"]?.ToString();
            string editorWindowHeightStr = userConfigJson["EditorWindowHeight"]?.ToString();
            if (editorWindowWidthStr == null || editorWindowHeightStr == null) return;
            Size = new Size(int.Parse(editorWindowWidthStr), int.Parse(editorWindowHeightStr));
        }

        private void SetMaterialsTableView()
        {
            string materialsTableViewSplitDistanceStr = userConfigJson["MaterialsTableViewSplitDistance"]?.ToString();
            if (materialsTableViewSplitDistanceStr == null) return;
            materialsPagePanelsContainer.SplitterDistance = int.Parse(materialsTableViewSplitDistanceStr);
        }

        private void ChangeTheme(Control control, Color backColor, Color foreColor)
        {
            tabWindowBackColor = backColor;
            tabWindowForeColor = foreColor;
            tabWindow.Refresh();
            foreach (Control subControl in control.Controls)
            {
                switch (subControl)
                {
                    case DataGridView table:
                        table.ColumnHeadersDefaultCellStyle.BackColor = backColor;
                        table.ColumnHeadersDefaultCellStyle.ForeColor = foreColor;
                        table.EnableHeadersVisualStyles = false;
                        table.BackgroundColor = backColor;
                        table.DefaultCellStyle.BackColor = backColor;
                        table.DefaultCellStyle.ForeColor = foreColor;
                        break;
                    case ComboBox box:
                        box.FlatStyle = FlatStyle.Popup;
                        break;
                    case Button button:
                        button.FlatStyle = FlatStyle.Flat;
                        button.FlatAppearance.BorderSize = 1;
                        break;
                    case CheckBox box:
                        box.FlatStyle = FlatStyle.Flat;
                        box.FlatAppearance.BorderSize = 1;
                        break;
                }
                subControl.BackColor = backColor;
                subControl.ForeColor = foreColor;
                ChangeTheme(subControl, backColor, foreColor);
            }
        }

        private void EnableDarkTheme()
        {
            ChangeTheme(this, ColorTranslator.FromHtml("#323232"), ColorTranslator.FromHtml("#d9d9d9"));
        }

        private static Vector3 ConvertSysNumToXnaVector3(System.Numerics.Vector3 v) { return new Vector3(v.X, v.Y, v.Z); }

        private static Vector3 ConvertSysNumToXnaVector3Flipped(System.Numerics.Vector3 v) { return new Vector3(v.X, v.Z, v.Y); }

        private static Vector3 VectorCrossProduct(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.Y * v2.Z - v1.Z * v2.Y, v1.Z * v2.X - v1.X * v2.Z, v1.X * v2.Y - v1.Y * v2.X);
        }

        private static float VectorDotProduct(Vector3 v1, Vector3 v2) { return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z; }

        public static void UpdateMesh()
        {
            if (dispMaleBody) flver.Meshes.Add(maleBodyFlver.Meshes[0]);
            else if (dispFemaleBody) flver.Meshes.Add(femaleBodyFlver.Meshes[0]);
            List<VertexPositionColor> vertexPosColorList = new List<VertexPositionColor>();
            List<VertexPositionColor> faceSetPosColorList = new List<VertexPositionColor>();
            List<VertexPositionColorTexture> faceSetPosColorTexList = new List<VertexPositionColorTexture>();
            List<VertexTexMap> vertexTexMapList = new List<VertexTexMap>();
            for (int i = 0; i < flver.Meshes.Count; ++i)
            {
                if (flver.Meshes[i] == null) continue;
                bool renderBackFaces = flver.Meshes[i].FaceSets.Count > 0 && !flver.Meshes[i].FaceSets[0].CullBackfaces;
                foreach (FLVER.Vertex[] vertexArr in flver.Meshes[i].GetFaces())
                {
                    if (hiddenMeshIndices.IndexOf(i) != -1) continue;
                    Microsoft.Xna.Framework.Color colorLine = Microsoft.Xna.Framework.Color.Black;
                    if (meshIsSelected && selectedMeshIndices.IndexOf(i) != -1)
                    {
                        colorLine.R = colorLine.G = 255;
                    }
                    else if (selectedMaterialMeshIndices.IndexOf(i) != -1)
                    {
                        colorLine.G = 0;
                        colorLine.R = colorLine.B = 255;
                    }
                    colorLine.A = 125;
                    vertexPosColorList.AddRange(new[]
                    {
                        new VertexPositionColor(ConvertSysNumToXnaVector3Flipped(vertexArr[0].Positions[0]), colorLine),
                        new VertexPositionColor(ConvertSysNumToXnaVector3Flipped(vertexArr[1].Positions[0]), colorLine),
                        new VertexPositionColor(ConvertSysNumToXnaVector3Flipped(vertexArr[0].Positions[0]), colorLine),
                        new VertexPositionColor(ConvertSysNumToXnaVector3Flipped(vertexArr[2].Positions[0]), colorLine),
                        new VertexPositionColor(ConvertSysNumToXnaVector3Flipped(vertexArr[1].Positions[0]), colorLine),
                        new VertexPositionColor(ConvertSysNumToXnaVector3Flipped(vertexArr[2].Positions[0]), colorLine)
                    });
                    Microsoft.Xna.Framework.Color faceSetColor = new Microsoft.Xna.Framework.Color();
                    Vector3 vectorA
                        = ConvertSysNumToXnaVector3(vertexArr[1].Positions[0]) - ConvertSysNumToXnaVector3(vertexArr[0].Positions[0]);
                    Vector3 vectorB
                        = ConvertSysNumToXnaVector3(vertexArr[2].Positions[0]) - ConvertSysNumToXnaVector3(vertexArr[0].Positions[0]);
                    Vector3 normalVector = VectorCrossProduct(vectorA, vectorB);
                    normalVector.Normalize();
                    Vector3 lightVector = new Vector3(viewer.lightX, viewer.lightY, viewer.lightZ);
                    lightVector.Normalize();
                    int faceSetColorVal = 125 + (int)(125 * VectorDotProduct(normalVector, lightVector));
                    faceSetColorVal = faceSetColorVal > 255 ? 255 : faceSetColorVal < 0 ? 0 : viewer.flatShading ? 255 : faceSetColorVal;
                    faceSetColor.R = faceSetColor.G = faceSetColor.B = (byte)faceSetColorVal;
                    faceSetColor.A = 255;
                    if (meshIsSelected && selectedMeshIndices.IndexOf(i) != -1)
                    {
                        faceSetColor.B = 0;
                    }
                    else if (selectedMaterialMeshIndices.IndexOf(i) != -1)
                    {
                        faceSetColor.B = 255;
                        faceSetColor.G = 0;
                    }
                    faceSetPosColorList.AddRange(
                        new[]
                        {
                            new VertexPositionColor(ConvertSysNumToXnaVector3Flipped(vertexArr[0].Positions[0]), faceSetColor),
                            new VertexPositionColor(ConvertSysNumToXnaVector3Flipped(vertexArr[2].Positions[0]), faceSetColor),
                            new VertexPositionColor(ConvertSysNumToXnaVector3Flipped(vertexArr[1].Positions[0]), faceSetColor),
                            new VertexPositionColor(ConvertSysNumToXnaVector3Flipped(vertexArr[0].Positions[0]), faceSetColor),
                            new VertexPositionColor(ConvertSysNumToXnaVector3Flipped(vertexArr[2].Positions[0]), faceSetColor),
                            new VertexPositionColor(ConvertSysNumToXnaVector3Flipped(vertexArr[1].Positions[0]), faceSetColor)
                        });
                    faceSetPosColorTexList.AddRange(
                        new[]
                        {
                            new VertexPositionColorTexture(ConvertSysNumToXnaVector3Flipped(vertexArr[0].Positions[0]), faceSetColor,
                                new Microsoft.Xna.Framework.Vector2(vertexArr[0].UVs[0].X, vertexArr[0].UVs[0].Y)),
                            new VertexPositionColorTexture(ConvertSysNumToXnaVector3Flipped(vertexArr[2].Positions[0]), faceSetColor,
                                new Microsoft.Xna.Framework.Vector2(vertexArr[2].UVs[0].X, vertexArr[2].UVs[0].Y)),
                            new VertexPositionColorTexture(ConvertSysNumToXnaVector3Flipped(vertexArr[1].Positions[0]), faceSetColor,
                                new Microsoft.Xna.Framework.Vector2(vertexArr[1].UVs[0].X, vertexArr[1].UVs[0].Y))
                        });
                    if (!renderBackFaces) continue;
                    faceSetPosColorList.AddRange(
                        new[]
                        {
                            new VertexPositionColor(ConvertSysNumToXnaVector3Flipped(vertexArr[0].Positions[0]), faceSetColor),
                            new VertexPositionColor(ConvertSysNumToXnaVector3Flipped(vertexArr[1].Positions[0]), faceSetColor),
                            new VertexPositionColor(ConvertSysNumToXnaVector3Flipped(vertexArr[2].Positions[0]), faceSetColor)
                        });
                    faceSetPosColorTexList.AddRange(
                        new[]
                        {
                            new VertexPositionColorTexture(ConvertSysNumToXnaVector3Flipped(vertexArr[0].Positions[0]), faceSetColor,
                                new Microsoft.Xna.Framework.Vector2(vertexArr[0].UVs[0].X, vertexArr[0].UVs[0].Y)),
                            new VertexPositionColorTexture(ConvertSysNumToXnaVector3Flipped(vertexArr[1].Positions[0]), faceSetColor,
                                new Microsoft.Xna.Framework.Vector2(vertexArr[1].UVs[0].X, vertexArr[1].UVs[0].Y)),
                            new VertexPositionColorTexture(ConvertSysNumToXnaVector3Flipped(vertexArr[2].Positions[0]), faceSetColor,
                                new Microsoft.Xna.Framework.Vector2(vertexArr[2].UVs[0].X, vertexArr[2].UVs[0].Y))
                        });
                }
                for (int j = 0; j < flver.Meshes[i].Vertices.Count; ++j)
                {
                    Program.vertices.Add(flver.Meshes[i].Vertices[j]);
                    Program.verticesInfo.Add(new VertexInfo { meshIndex = i, vertexIndex = (uint)j });
                }
                FLVER.Material currMaterial = flver.Materials.ElementAtOrDefault(flver.Meshes[i].MaterialIndex);
                if (currMaterial == null) continue;
                List<FLVER.Texture> texList = currMaterial.Textures;
                if (texList.Count <= 0) continue;
                VertexTexMap vertexTexMap = new VertexTexMap
                {
                    textureName = Path.GetFileNameWithoutExtension(texList[0].Path),
                    faceSetTextures = faceSetPosColorTexList.ToArray()
                };
                faceSetPosColorTexList.Clear();
                vertexTexMapList.Add(vertexTexMap);
            }
            if (vertexPosColorList.Count % 2 != 0) vertexPosColorList.Add(vertexPosColorList[vertexPosColorList.Count - 1]);
            for (int i = 0; i < bonePositionList.Count; ++i)
                bonePositionList[i] = null;
            // TODO: Investigate the maximum threshold on bones
            Transform3D[] bonesTransform = new Transform3D[flver.Bones.Count];
            for (int i = 0; i < flver.Bones.Count; ++i)
            {
                bonesTransform[i] = new Transform3D { rotOrder = rotOrder, position = new Vector3D(flver.Bones[i].Translation) };
                bonesTransform[i].setRotationInRad(new Vector3D(flver.Bones[i].Rotation));
                bonesTransform[i].scale = new Vector3D(flver.Bones[i].Scale);
                if (flver.Bones[i].ParentIndex < 0) continue;
                bonesTransform[i].parent = bonesTransform[flver.Bones[i].ParentIndex];
                Vector3D absolutePos = bonesTransform[i].getGlobalOrigin();
                if (bonesTransform[flver.Bones[i].ParentIndex] == null) continue;
                Vector3D parentPos = bonesTransform[flver.Bones[i].ParentIndex].getGlobalOrigin();
                vertexPosColorList.Add(new VertexPositionColor(new Vector3(parentPos.X - 0.005f, parentPos.Z - 0.005f, parentPos.Y),
                    Microsoft.Xna.Framework.Color.Purple));
                vertexPosColorList.Add(new VertexPositionColor(new Vector3(absolutePos.X, absolutePos.Z, absolutePos.Y),
                    Microsoft.Xna.Framework.Color.Purple));
                vertexPosColorList.Add(new VertexPositionColor(new Vector3(parentPos.X + 0.005f, parentPos.Z + 0.005f, parentPos.Y),
                    Microsoft.Xna.Framework.Color.Purple));
                vertexPosColorList.Add(new VertexPositionColor(new Vector3(absolutePos.X, absolutePos.Z, absolutePos.Y),
                    Microsoft.Xna.Framework.Color.Purple));
            }
            for (int i = 0; i < flver.Dummies.Count; ++i)
            {
                FLVER.Dummy dummy = flver.Dummies[i];
                bool shouldSelectDummy = dummyIsSelected && selectedDummyIndices.IndexOf(i) != -1;
                Microsoft.Xna.Framework.Color dummyColor = shouldSelectDummy ? Microsoft.Xna.Framework.Color.Yellow : Microsoft.Xna.Framework.Color.Purple;
                float baseDummyYPos = dummy.Position.Y;
                const float posStep = 0.0005f;
                for (int j = 0; j < dummyThickness; ++j)
                {
                    vertexPosColorList.AddRange(new[]
                    {
                        new VertexPositionColor(new Vector3(dummy.Position.X - 0.025f, dummy.Position.Z, baseDummyYPos), dummyColor),
                        new VertexPositionColor(new Vector3(dummy.Position.X + 0.025f, dummy.Position.Z, baseDummyYPos), dummyColor),
                        new VertexPositionColor(new Vector3(dummy.Position.X, dummy.Position.Z - 0.025f, baseDummyYPos), dummyColor),
                        new VertexPositionColor(new Vector3(dummy.Position.X, dummy.Position.Z + 0.025f, baseDummyYPos), dummyColor),
                        new VertexPositionColor(new Vector3(dummy.Position.X, dummy.Position.Z, baseDummyYPos), Microsoft.Xna.Framework.Color.Green),
                        new VertexPositionColor(new Vector3(dummy.Position.X + dummy.Forward.X, dummy.Position.Z + dummy.Forward.Z,
                                baseDummyYPos + dummy.Forward.Y),
                            Microsoft.Xna.Framework.Color.Green)
                    });
                    baseDummyYPos -= posStep;
                }
            }
            if (Program.useCheckingPoint)
            {
                System.Numerics.Vector3 checkingPoint = Program.checkingPoint;
                System.Numerics.Vector3 checkingPointNormal = Program.checkingPointNormal;
                vertexPosColorList.Add(new VertexPositionColor(new Vector3(checkingPoint.X - 0.05f, checkingPoint.Z - 0.05f, checkingPoint.Y),
                    Microsoft.Xna.Framework.Color.AntiqueWhite));
                vertexPosColorList.Add(new VertexPositionColor(new Vector3(checkingPoint.X + 0.05f, checkingPoint.Z + 0.05f, checkingPoint.Y),
                    Microsoft.Xna.Framework.Color.AntiqueWhite));
                vertexPosColorList.Add(new VertexPositionColor(new Vector3(checkingPoint.X - 0.05f, checkingPoint.Z + 0.05f, checkingPoint.Y),
                    Microsoft.Xna.Framework.Color.AntiqueWhite));
                vertexPosColorList.Add(new VertexPositionColor(new Vector3(checkingPoint.X + 0.05f, checkingPoint.Z - 0.05f, checkingPoint.Y),
                    Microsoft.Xna.Framework.Color.AntiqueWhite));
                vertexPosColorList.Add(new VertexPositionColor(new Vector3(checkingPoint.X, checkingPoint.Z, checkingPoint.Y), Microsoft.Xna.Framework.Color.Blue));
                vertexPosColorList.Add(new VertexPositionColor(
                    new Vector3(checkingPoint.X + 0.2f * checkingPointNormal.X, checkingPoint.Z + 0.2f * checkingPointNormal.Z, checkingPoint.Y + 0.2f * checkingPointNormal.Y),
                    Microsoft.Xna.Framework.Color.Blue));
            }
            viewer.vertices = vertexPosColorList.ToArray();
            viewer.vertexTexMapList = vertexTexMapList.ToArray();
            viewer.faceSets = faceSetPosColorList.ToArray();
            if (dispMaleBody) flver.Meshes.Remove(maleBodyFlver.Meshes[0]);
            else if (dispFemaleBody) flver.Meshes.Remove(femaleBodyFlver.Meshes[0]);
        }

        private static void ClearViewerMaterialHighlight()
        {
            selectedMaterialMeshIndices.Clear();
        }

        private void DeselectAllSelectedThings()
        {
            isSettingDefaultInfo = true;
            meshIsSelected = false;
            dummyIsSelected = false;
            meshIsHidden = false;
            selectedMeshIndices.Clear();
            selectedDummyIndices.Clear();
            hiddenMeshIndices.Clear();
            meshModifiersContainer.Enabled = false;
            isSettingDefaultInfo = false;
        }

        private static void LoadViewer()
        {
            if (viewer == null)
            {
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    viewer = new Mono3D();
                    UpdateMesh();
                    viewer.RefreshTextures();
                    viewer.Run();
                }).Start();
                return;
            }
            UpdateMesh();
            viewer.RefreshTextures();
        }

        private static bool LoadPresets(ComboBox selector, ref Dictionary<object, object> dict, string filePath)
        {
            selector.Items.Clear();
            bool hasRead = true;
            try
            {
                dict = new JavaScriptSerializer().Deserialize<Dictionary<object, object>>(File.ReadAllText(filePath));
                if (dict == materialPresets && !dict.ContainsKey(baseMaterialDictKey)) dict.Add(baseMaterialDictKey, Program.GetBaseMaterial());
                selector.Items.AddRange(dict.Keys.ToArray());
            }
            catch
            {
                hasRead = false;
            }
            return hasRead;
        }

        private void LoadMaterialPresets()
        {
            bool hasRead = LoadPresets(materialPresetsSelector, ref materialPresets, materialPresetsFilePath);
            materialsTable.Columns[mtAddPresetCbIndex].Visible = hasRead;
        }

        private void LoadDummyPresets()
        {
            dummyPresetsSelector.Enabled = dummiesTableOKButton.Enabled =
                addAllDummiesToPresetsButton.Enabled = LoadPresets(dummyPresetsSelector, ref dummyPresets, dummyPresetsFilePath);
        }

        private static void DisableDataTableColumnSorting(DataGridView table)
        {
            foreach (DataGridViewColumn column in table.Columns)
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private void GloballyDisableDataTableColumnSorting()
        {
            DisableDataTableColumnSorting(bonesTable);
            DisableDataTableColumnSorting(materialsTable);
            DisableDataTableColumnSorting(texturesTable);
            DisableDataTableColumnSorting(meshTable);
            DisableDataTableColumnSorting(dummiesTable);
        }

        private static int GetModelMask(string materialName)
        {
            int modelMask = -1;
            int maskIndex = materialName.IndexOf("#", StringComparison.Ordinal);
            if (maskIndex == -1) return modelMask;
            try
            {
                string modelMaskStr = materialName.Substring(maskIndex + 1, maskIndex + 2);
                modelMask = int.Parse(modelMaskStr);
            }
            catch { }
            return modelMask;
        }

        private void UpdateUI()
        {
            isSettingDefaultInfo = true;
            applyMatBinTexturesButton.Enabled = false;
            bonesTable.Rows.Clear();
            materialsTable.Rows.Clear();
            texturesTable.Rows.Clear();
            meshTable.Rows.Clear();
            dummiesTable.Rows.Clear();
            for (int i = 0; i < flver.Bones.Count; ++i)
            {
                FLVER.Bone bone = flver.Bones[i];
                DataGridViewRow row = new DataGridViewRow();
                row.Cells.AddRange(new DataGridViewTextBoxCell { Value = i }, new DataGridViewTextBoxCell { Value = bone.Name },
                    new DataGridViewTextBoxCell { Value = bone.ParentIndex }, new DataGridViewTextBoxCell { Value = bone.ChildIndex },
                    new DataGridViewTextBoxCell { Value = $"{bone.Translation.X},{bone.Translation.Y},{bone.Translation.Z}" },
                    new DataGridViewTextBoxCell { Value = $"{bone.Scale.X},{bone.Scale.Y},{bone.Scale.Z}" },
                    new DataGridViewTextBoxCell { Value = $"{bone.Rotation.X},{bone.Rotation.Y},{bone.Rotation.Z}" },
                    new DataGridViewTextBoxCell { Value = $"{bone.BoundingBoxMin.X},{bone.BoundingBoxMin.Y},{bone.BoundingBoxMin.Z}" },
                    new DataGridViewTextBoxCell { Value = $"{bone.BoundingBoxMax.X},{bone.BoundingBoxMax.Y},{bone.BoundingBoxMax.Z}" });
                bonesTable.Rows.Add(row);
            }
            for (int i = 0; i < flver.Materials.Count; ++i)
            {
                FLVER.Material material = flver.Materials[i];
                DataGridViewRow row = new DataGridViewRow();
                if (!stopAutoInternalIndexOverride) material.Unk18 = i;
                row.Cells.AddRange(new DataGridViewTextBoxCell { Value = i }, new DataGridViewTextBoxCell { Value = material.Name },
                    new DataGridViewTextBoxCell { Value = GetModelMask(material.Name) }, new DataGridViewTextBoxCell { Value = material.Flags },
                    new DataGridViewTextBoxCell { Value = material.MTD }, new DataGridViewTextBoxCell { Value = material.Unk18 });
                for (int j = 0; j < 3; ++j)
                    row.Cells.Add(new DataGridViewButtonCell { Value = materialsTable.Columns[j + 6].HeaderText });
                for (int j = 0; j < 2; ++j)
                    row.Cells.Add(new DataGridViewCheckBoxCell { Value = false });
                materialsTable.Rows.Add(row);
            }
            for (int i = 0; i < flver.Meshes.Count; ++i)
            {
                FLVER.Mesh mesh = flver.Meshes[i];
                if (mesh.MaterialIndex < 0) mesh.MaterialIndex = 0;
                DataGridViewRow row = new DataGridViewRow();
                row.Cells.AddRange(new DataGridViewTextBoxCell { Value = i },
                    new DataGridViewTextBoxCell { Value = flver.Materials[mesh.MaterialIndex].Name });
                row.Cells.Add(new DataGridViewButtonCell { Value = "Apply" });
                try
                {
                    row.Cells.Add(new DataGridViewCheckBoxCell { Value = selectedMeshIndices[i] >= 0 });
                }
                catch
                {
                    row.Cells.Add(new DataGridViewCheckBoxCell { Value = false });
                }
                row.Cells.Add(new DataGridViewCheckBoxCell { Value = false });
                meshTable.Rows.Add(row);
            }
            for (int i = 0; i < flver.Dummies.Count; ++i)
            {
                FLVER.Dummy dummy = flver.Dummies[i];
                DataGridViewRow row = new DataGridViewRow();
                row.Cells.AddRange(new DataGridViewTextBoxCell { Value = i },
                    new DataGridViewTextBoxCell { Value = dummy.ReferenceID },
                    new DataGridViewTextBoxCell { Value = dummy.AttachBoneIndex },
                    new DataGridViewTextBoxCell { Value = dummy.DummyBoneIndex });
                row.Cells.Add(new DataGridViewCheckBoxCell { Value = false });
                row.Cells.Add(new DataGridViewButtonCell { Value = dummiesTable.Columns[5].HeaderText });
                dummiesTable.Rows.Add(row);
            }
            if (selectedMaterialIndex != -1)
            {
                applyMatBinTexturesButton.Enabled = true;
                UpdateTexturesTable();
            }
            isSettingDefaultInfo = false;
        }

        private static void BackupFLVERFile()
        {
            string backupFilePath = flverFilePath;
            if (backupFilePath.Contains(".flver")) backupFilePath = backupFilePath.Replace(".flver", ".flver.bak");
            else if (backupFilePath.Contains(".flv")) backupFilePath = backupFilePath.Replace(".flv", ".flv.bak");
            else backupFilePath = backupFilePath.Replace(".dcx", ".dcx.bak");
            if (!File.Exists(backupFilePath)) File.Copy(flverFilePath, backupFilePath);
        }

        private static void UpdateWindowTitle(string flverPath)
        {
            Program.window.Text = $@"{Program.windowTitle} - {Path.GetFileName(flverPath)}";
        }

        private static string PromptFLVERModel()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter =
                    @"FLVER File (*.flv, *.flv.bak, *.flver, *.flver.bak)|*.flv;*.flv.bak;*.flver;*.flver.bak|BND File (*.dcx, *.dcx.bak)|*.dcx;*.dcx.bak|Model Container (*.flv, *.flv.bak, *.flver, *.flver.bak, *.dcx, *.dcx.bak)|*.flv;*.flv.bak;*.flver;*.flver.bak;*.dcx;*.dcx.bak",
                FilterIndex = 3, Multiselect = false
            };
            return dialog.ShowDialog() != DialogResult.OK ? "" : dialog.FileName.ToLower();
        }

        private static bool IsFLVERPath(string filePath)
        {
            return filePath.Contains(".flv") || filePath.Contains(".flver");
        }

        private FLVER ReadFLVERFromDCXPath(string filePath, bool setMainFlverArchiveType, bool setBinderIndex, bool wantsTpf)
        {
            List<BinderFile> flverFiles = new List<BinderFile>();
            BND4 newFlverBnd = null;
            try
            {
                newFlverBnd = SoulsFile<BND4>.Read(filePath);
                flverBnd = newFlverBnd;
            }
            catch
            {
                try
                {
                    newFlverBnd = SoulsFile<BND4>.Read(DCX.Decompress(filePath));
                    flverBnd = newFlverBnd;
                }
                catch { }
            }
            if (newFlverBnd != null)
            {
                if (setMainFlverArchiveType) flverArchiveType = flverBnd.Compression;
                int binderIndex = 0;
                foreach (BinderFile file in flverBnd.Files)
                {
                    if (IsFLVERPath(file.Name))
                    {
                        flverFiles.Add(file);
                        if (currFlverFileBinderIndex == -1 && setBinderIndex) currFlverFileBinderIndex = binderIndex;
                    }
                    else if (file.Name.EndsWith(".tpf") && wantsTpf)
                    {
                        try
                        {
                            Program.tpf = TPF.Read(file.Bytes);
                        }
                        catch { }
                    }
                    binderIndex++;
                }
                if (flverFiles.Count == 1) return FLVER.Read(flverFiles[0].Bytes);
                if (flverFiles.Count > 1)
                {
                    int selectedFlverIndex = ShowSelectorDialog("Pick a FLVER", flverFiles);
                    if (!setBinderIndex || selectedFlverIndex == -1) return selectedFlverIndex == -1 ? null : FLVER.Read(flverFiles[selectedFlverIndex].Bytes);
                    int binderWiseSelectedFlverIndex = flverBnd.Files.FindIndex(i =>
                        i.Name.IndexOf(flverFiles[selectedFlverIndex].Name, StringComparison.OrdinalIgnoreCase) != -1);
                    currFlverFileBinderIndex = binderWiseSelectedFlverIndex;
                    return FLVER.Read(flverFiles[selectedFlverIndex].Bytes);
                }
            }
            ShowInformationDialog("No FLVER files were found in the DCX archive.");
            return null;
        }

        private bool OpenFLVERFile()
        {
            isSettingDefaultInfo = true;
            stopAutoInternalIndexOverride = false;
            if (arguments.Count > 0)
            {
                flverFilePath = arguments[0].ToLower();
                arguments = new List<string>();
            }
            else
            {
                flverFilePath = PromptFLVERModel();
                if (flverFilePath == "") return false;
            }
            if (Program.window != null) UpdateWindowTitle(flverFilePath);
            Mono3D.textureMap = new Dictionary<string, Texture2D>();
            Program.tpf = null;
            Program.filePath = flverFilePath;
            if (IsFLVERPath(flverFilePath))
            {
                flver = FLVER.Read(flverFilePath);
                Program.flver = flver;
            }
            else
            {
                FLVER newFlver = ReadFLVERFromDCXPath(flverFilePath, true, true, true);
                if (newFlver == null) return false;
                flver = newFlver;
                Program.flver = flver;
            }
            currFlverBytes = flver.Write();
            saveToolStripMenuItem.Enabled = saveAsToolStripMenuItem.Enabled = true;
            matBinBndPath = null;
            DeselectAllSelectedThings();
            UpdateUI();
            ClearViewerMaterialHighlight();
            ClearUndoRedoStates();
            LoadMaterialPresets();
            LoadDummyPresets();
            LoadViewer();
            isSettingDefaultInfo = false;
            stopAutoInternalIndexOverride = true;
            return true;
        }

        private void OpenButtonClicked(object sender, EventArgs e)
        {
            OpenFLVERFile();
        }

        private void UpdateTexturesTable()
        {
            if (selectedMaterialIndex == -1) return;
            texturesTable.Rows.Clear();
            for (int i = 0; i < flver.Materials[selectedMaterialIndex].Textures.Count; ++i)
            {
                FLVER.Material material = flver.Materials[selectedMaterialIndex];
                DataGridViewRow row = new DataGridViewRow();
                row.Cells.AddRange(new DataGridViewTextBoxCell { Value = material.Textures[i].Type },
                    new DataGridViewTextBoxCell { Value = material.Textures[i].Path });
                row.Cells.Add(new DataGridViewButtonCell { Value = texturesTable.Columns[2].HeaderText });
                texturesTable.Rows.Add(row);
            }
        }

        private void UpdateMaterialPresets()
        {
            File.WriteAllText(materialPresetsFilePath, JToken.Parse(new JavaScriptSerializer().Serialize(materialPresets)).ToString());
            LoadMaterialPresets();
        }

        private void UpdateDummyPresets()
        {
            File.WriteAllText(dummyPresetsFilePath, new JavaScriptSerializer().Serialize(dummyPresets));
            LoadDummyPresets();
        }

        private static string PromptForPresetName()
        {
            string presetName = ShowInputDialog("Enter a preset name:", "Add Preset");
            return presetName == "" ? "" : presetName;
        }

        private void MaterialsTableButtonClicked(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            switch (e.ColumnIndex)
            {
                case mtEditButtonIndex:
                    applyMatBinTexturesButton.Enabled = true;
                    selectedMaterialIndex = e.RowIndex;
                    UpdateTexturesTable();
                    break;
                case mtViewerHighlightButtonIndex:
                    bool unhighlighted = flver.Meshes.Any(mesh => selectedMaterialMeshIndices.IndexOf(flver.Meshes.IndexOf(mesh)) == -1 && mesh.MaterialIndex == e.RowIndex);
                    ClearViewerMaterialHighlight();
                    if (unhighlighted)
                    {
                        foreach (FLVER.Mesh mesh in flver.Meshes.Where(mesh => mesh.MaterialIndex == e.RowIndex))
                            selectedMaterialMeshIndices.Add(flver.Meshes.IndexOf(mesh));
                    }
                    UpdateMesh();
                    break;
                case mtAddPresetCbIndex when !materialPresets.ContainsKey(flver.Materials[e.RowIndex].Name):
                    string presetName = PromptForPresetName();
                    if (presetName == "") break;
                    if (!materialPresets.ContainsKey(presetName))
                    {
                        materialPresets.Add(presetName, flver.Materials[e.RowIndex]);
                        UpdateMaterialPresets();
                    }
                    break;
            }
        }

        private static bool AreCheckboxesInDataTableAllChecked(DataGridView table, int columnIndex)
        {
            bool allChecked = true;
            foreach (DataGridViewRow row in table.Rows)
                if (!(bool)row.Cells[columnIndex].Value)
                    allChecked = false;
            return allChecked;
        }

        private static void ToggleCheckboxesInDataTable(DataGridView table, int columnIndex)
        {
            bool allChecked = AreCheckboxesInDataTableAllChecked(table, columnIndex);
            foreach (DataGridViewRow row in table.Rows)
                row.Cells[columnIndex].Value = !allChecked;
        }

        private void MaterialsTableApplyToAllButtonClicked(object sender, MouseEventArgs e)
        {
            ToggleCheckboxesInDataTable(materialsTable, mtApplyPresetCbIndex);
        }

        private void MaterialsTableDeleteAllButtonClicked(object sender, MouseEventArgs e)
        {
            ToggleCheckboxesInDataTable(materialsTable, mtDeleteCbIndex);
        }

        private static void InjectTextureIntoTPF(string textureFilePath)
        {
            if (Program.tpf == null) Program.tpf = new TPF();
            BinderFile flverBndTpfEntry = flverBnd?.Files.FirstOrDefault(i => i.Name.EndsWith(".tpf"));
            byte[] ddsBytes = File.ReadAllBytes(textureFilePath);
            DDS dds = new DDS(ddsBytes);
            byte formatByte = 107;
            try
            {
                formatByte = (byte)Enum.Parse(typeof(TextureFormats), dds.header10.dxgiFormat.ToString());
            }
            catch { }
            TPF.Texture texture = new TPF.Texture(Path.GetFileNameWithoutExtension(textureFilePath), formatByte, 0x00, 0, File.ReadAllBytes(textureFilePath));
            int existingTextureIndex = Program.tpf.Textures.FindIndex(i => i.Name == texture.Name);
            if (existingTextureIndex != -1)
            {
                Program.tpf.Textures.RemoveAt(existingTextureIndex);
                Program.tpf.Textures.Insert(existingTextureIndex, texture);
            }
            else Program.tpf.Textures.Add(texture);
            if (flverBndTpfEntry != null) flverBnd.Files[flverBnd.Files.IndexOf(flverBndTpfEntry)].Bytes = Program.tpf.Write();
            else
            {
                if (flverFilePath.Contains(".flver")) Program.tpf.Write(flverFilePath.Replace("_1.", ".").Replace(".flver", ".tpf"));
                else if (flverFilePath.Contains(".flv")) Program.tpf.Write(flverFilePath.Replace("_1.", ".").Replace(".flv", ".tpf"));
            }
        }

        private void TexturesTableButtonClicked(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != 2) return;
            OpenFileDialog dialog = new OpenFileDialog { Filter = imageFilesFilter };
            if (dialog.ShowDialog() != DialogResult.OK) return;
            UpdateUndoState();
            flver.Materials[selectedMaterialIndex].Textures[e.RowIndex].Path = $"{Path.GetFileNameWithoutExtension(dialog.FileName)}.tif";
            InjectTextureIntoTPF(dialog.FileName);
            UpdateTexturesTable();
            UpdateMesh();
            viewer.RefreshTextures();
        }

        private void ResetModifierNumBoxValues()
        {
            foreach (NumericUpDown box in meshModifiersNumBoxesContainer.Controls)
                box.Value = 0;
        }

        private void EnableDisableExtraModifierOptions()
        {
            // TODO: Determine which properties are appropriate for dummies to make use of
            reverseFacesetsCheckbox.Enabled = reverseNormalsCheckbox.Enabled = toggleBackfacesCheckbox.Enabled =
                deleteFacesetsCheckbox.Enabled = uniformScaleCheckbox.Enabled = centerXButton.Enabled = centerYButton.Enabled =
                    centerZButton.Enabled = mirrorXCheckbox.Enabled = mirrorYCheckbox.Enabled = mirrorZCheckbox.Enabled =
                        flipYZAxisCheckbox.Enabled = selectedMeshIndices.Count != 0;
        }

        private static List<int> UpdateIndicesList(DataGridView dataTable, List<int> indices, int columnIndex, int rowIndex, ref bool selectedFlag)
        {
            if (rowIndex < 0) return indices;
            if ((bool)dataTable[columnIndex, rowIndex].Value)
            {
                if (indices.IndexOf(rowIndex) == -1) indices.Add(rowIndex);
                else indices.RemoveAt(indices.IndexOf(rowIndex));
            }
            else
            {
                if (indices.Count < 1) selectedFlag = true;
                if (indices.IndexOf(rowIndex) == -1) indices.Add(rowIndex);
                else indices.RemoveAt(indices.IndexOf(rowIndex));
            }
            return indices;
        }

        private void UpdateSelectedDummies()
        {
            if (isSettingDefaultInfo) return;
            if (dummyIsSelected)
            {
                isSettingDefaultInfo = true;
                bool hasIndices = selectedDummyIndices.Count != 0 || selectedMeshIndices.Count > 0;
                ResetModifierNumBoxValues();
                meshModifiersContainer.Enabled = hasIndices;
                if (hasIndices)
                {
                    EnableDisableExtraModifierOptions();
                    scaleXNumBox.Value = scaleYNumBox.Value = scaleZNumBox.Value = 100;
                    rotXNumBox.Value = rotYNumBox.Value = rotZNumBox.Value = 0;
                }
                isSettingDefaultInfo = false;
            }
            UpdateMesh();
        }

        private void UpdateSelectedMeshes()
        {
            if (isSettingDefaultInfo) return;
            if (meshIsSelected)
            {
                isSettingDefaultInfo = true;
                bool hasIndices = selectedMeshIndices.Count != 0 || selectedDummyIndices.Count > 0;
                ResetModifierNumBoxValues();
                meshModifiersContainer.Enabled = hasIndices;
                if (hasIndices)
                {
                    EnableDisableExtraModifierOptions();
                    scaleXNumBox.Value = scaleYNumBox.Value = scaleZNumBox.Value = 100;
                    rotXNumBox.Value = rotYNumBox.Value = rotZNumBox.Value = 0;
                }
                isSettingDefaultInfo = false;
            }
            UpdateMesh();
        }

        private int ShowSelectorDialog<T>(string windowTitle, IEnumerable<T> selectorList)
        {
            Form selectorForm = new Form();
            selectorForm.Text = windowTitle;
            selectorForm.Icon = Icon;
            selectorForm.Width = 500;
            selectorForm.Height = 500;
            selectorForm.MinimumSize = new Size(300, 300);
            selectorForm.StartPosition = FormStartPosition.CenterScreen;
            selectorForm.MaximizeBox = false;
            TreeView boneSelectionBox = new TreeView();
            boneSelectionBox.Width = 450;
            boneSelectionBox.Height = 420;
            boneSelectionBox.Location = new Point(boneSelectionBox.Location.X + 15, boneSelectionBox.Location.Y + 5);
            boneSelectionBox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            Button cancelButton = new Button
            {
                Text = @"Cancel",
                Size = new Size(65, 25),
                Location = new Point(boneSelectionBox.Width - 105,
                    boneSelectionBox.Bottom + 5),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            Button okButton = new Button
            {
                Text = @"OK",
                Size = new Size(50, 25),
                Location = new Point(boneSelectionBox.Width - 35,
                    boneSelectionBox.Bottom + 5),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                DialogResult = DialogResult.OK
            };

            void CloseDialogHandler(object s, EventArgs e)
            {
                selectorForm.Close();
            }

            cancelButton.Click += CloseDialogHandler;
            okButton.Click += CloseDialogHandler;
            selectorForm.AcceptButton = okButton;
            selectorForm.Controls.Add(boneSelectionBox);
            selectorForm.Controls.Add(cancelButton);
            selectorForm.Controls.Add(okButton);
            foreach (T item in selectorList)
                boneSelectionBox.Nodes.Add(new TreeNode(Path.GetFileName(((dynamic)item).Name)));
            return selectorForm.ShowDialog() == DialogResult.OK ? boneSelectionBox.SelectedNode.Index : -1;
        }

        private void ApplyMeshSimpleSkin(int meshIndex)
        {
            int boneIndex = ShowSelectorDialog("Pick a Bone", flver.Bones);
            if (boneIndex == -1) return;
            List<FLVER.Vertex> unweightedVerts = flver.Meshes[meshIndex].Vertices.Where(v =>
                v.BoneIndices != null && !v.BoneIndices.Contains(boneIndex) && v.BoneWeights != null && v.BoneWeights.All(i => i == 0)).ToList();
            if (!unweightedVerts.Any())
            {
                ShowInformationDialog("Found no unweighted vertices to apply default weights to.");
                return;
            }
            UpdateUndoState();
            foreach (FLVER.Vertex v in unweightedVerts)
            {
                v.BoneIndices[0] = boneIndex;
                v.BoneWeights = new float[] { 1, 0, 0, 0 };
            }
            ShowInformationDialog("Successfully applied mesh simple skin!");
        }

        private void MeshTableCheckboxSelected(int rowIndex, int columnIndex)
        {
            if (rowIndex < 0) return;
            switch (columnIndex)
            {
                case 2:
                    ApplyMeshSimpleSkin(rowIndex);
                    break;
                case 3:
                    selectedMeshIndices = UpdateIndicesList(meshTable, selectedMeshIndices, columnIndex, rowIndex, ref meshIsSelected);
                    UpdateSelectedMeshes();
                    break;
                case 4:
                    hiddenMeshIndices = UpdateIndicesList(meshTable, hiddenMeshIndices, columnIndex, rowIndex, ref meshIsHidden);
                    UpdateMesh();
                    break;
            }
        }

        private void MeshTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            MeshTableCheckboxSelected(e.RowIndex, e.ColumnIndex);
        }

        private void DummiesTableCheckboxSelected(int rowIndex, int columnIndex)
        {
            if (rowIndex < 0) return;
            switch (columnIndex)
            {
                case 4:
                    selectedDummyIndices = UpdateIndicesList(dummiesTable, selectedDummyIndices, columnIndex, rowIndex, ref dummyIsSelected);
                    UpdateSelectedDummies();
                    break;
                case 5:
                    UpdateUndoState();
                    FLVER.Dummy duplicatedDummy = new FLVER.Dummy
                    {
                        Position = flver.Dummies[rowIndex].Position,
                        Forward = flver.Dummies[rowIndex].Forward,
                        ReferenceID = flver.Dummies[rowIndex].ReferenceID,
                        AttachBoneIndex = flver.Dummies[rowIndex].AttachBoneIndex,
                        DummyBoneIndex = flver.Dummies[rowIndex].DummyBoneIndex,
                        Upward = flver.Dummies[rowIndex].Upward,
                        Unk0C = flver.Dummies[rowIndex].Unk0C,
                        Unk0D = flver.Dummies[rowIndex].Unk0D,
                        Unk0E = flver.Dummies[rowIndex].Unk0E,
                        Flag1 = flver.Dummies[rowIndex].Flag1,
                        Flag2 = flver.Dummies[rowIndex].Flag2,
                        Unk30 = flver.Dummies[rowIndex].Unk30,
                        Unk34 = flver.Dummies[rowIndex].Unk34
                    };
                    flver.Dummies.Add(duplicatedDummy);
                    DeselectAllSelectedThings();
                    UpdateUI();
                    UpdateMesh();
                    break;
            }
        }

        private void DummiesTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DummiesTableCheckboxSelected(e.RowIndex, e.ColumnIndex);
        }

        private static System.Numerics.Vector3 CreateTranslationVector(float x, float y, float z, float offset, int nbi)
        {
            return new System.Numerics.Vector3(x + (nbi == 0 ? offset : 0), y + (nbi == 1 ? offset : 0), z + (nbi == 2 ? offset : 0));
        }

        private static System.Numerics.Vector3 CreateScaleVector(float x, float y, float z, float offset, IReadOnlyList<float> totals, int nbi, bool uniform, bool invert)
        {
            float scalar = offset < 0 && !invert ? -(offset - 1) : invert ? offset - 1 : offset + 1;
            float newX = nbi == 0 || uniform ? x - totals[0] : x;
            float newY = nbi == 1 || uniform ? y - totals[1] : y;
            float newZ = nbi == 2 || uniform ? z - totals[2] : z;
            newX = nbi == 0 || uniform ? (offset < 0 && !invert ? newX / scalar : newX * scalar) + totals[0] : x;
            newY = nbi == 1 || uniform ? (offset < 0 && !invert ? newY / scalar : newY * scalar) + totals[1] : y;
            newZ = nbi == 2 || uniform ? (offset < 0 && !invert ? newZ / scalar : newZ * scalar) + totals[2] : z;
            return new System.Numerics.Vector3(newX, newY, newZ);
        }

        private static dynamic CreateRotationVector(float x, float y, float z, float w, float offset, IReadOnlyList<float> totals, int nbi)
        {
            float newX = nbi == 1 ? offset : 0;
            float newY = nbi == 0 ? offset : 0;
            float newZ = nbi == 2 ? offset : 0;
            System.Numerics.Vector3 vector = new System.Numerics.Vector3(x - totals[0], y - totals[1], z - totals[2]);
            vector = Program.RotatePoint(vector, newY, newX, newZ);
            return w == 0 ? (dynamic)new System.Numerics.Vector3(vector.X + totals[0], vector.Y + totals[1], vector.Z + totals[2]) :
                new Vector4(vector.X + totals[0], vector.Y + totals[1], vector.Z + totals[2], w);
        }

        private static void TranslateThing(dynamic thing, float offset, int nbi)
        {
            switch (thing)
            {
                case FLVER.Dummy d:
                    d.Position = CreateTranslationVector(d.Position.X, d.Position.Y, d.Position.Z, offset, nbi);
                    break;
                case FLVER.Vertex v:
                    v.Positions[0] = CreateTranslationVector(v.Positions[0].X, v.Positions[0].Y, v.Positions[0].Z, offset, nbi);
                    break;
            }
        }

        private static void ScaleThing(dynamic thing, float offset, IReadOnlyList<float> totals, int nbi, bool uniform, bool invert)
        {
            if (nbi >= 3 && nbi <= 5) nbi -= 3;
            switch (thing)
            {
                case FLVER.Dummy d:
                    if (selectedMeshIndices.Count != 0) d.Position = CreateScaleVector(d.Position.X, d.Position.Y, d.Position.Z, offset, totals, nbi, uniform, invert);
                    else d.Forward = CreateTranslationVector(d.Forward.X, d.Forward.Y, d.Forward.Z, offset, nbi);
                    break;
                case FLVER.Vertex v:
                    v.Positions[0] = CreateScaleVector(v.Positions[0].X, v.Positions[0].Y, v.Positions[0].Z, offset, totals, nbi, uniform, invert);
                    v.Normals[0] = new Vector4(v.Normals[0].X, v.Normals[0].Y, invert && nbi != 2 ? -v.Normals[0].Z : v.Normals[0].Z, v.Normals[0].W);
                    v.Tangents[0] = new Vector4(v.Tangents[0].X, v.Tangents[0].Y, invert && nbi != 2 ? -v.Normals[0].Z : v.Normals[0].Z, v.Tangents[0].W);
                    break;
            }
        }

        private static void RotateThing(dynamic thing, float offset, IReadOnlyList<float> totals, int nbi)
        {
            if (nbi >= 6 && nbi <= 8) nbi -= 6;
            float newX = nbi == 0 ? offset : 0;
            float newY = nbi == 1 ? offset : 0;
            float newZ = nbi == 2 ? offset : 0;
            switch (thing)
            {
                case FLVER.Dummy d:
                    if (selectedMeshIndices.Count != 0) d.Position = CreateRotationVector(d.Position.X, d.Position.Y, d.Position.Z, 0, offset, totals, nbi);
                    else d.Forward = Program.RotatePoint(d.Forward, newX, newZ, newY);
                    break;
                case FLVER.Vertex v:
                    v.Positions[0] = CreateRotationVector(v.Positions[0].X, v.Positions[0].Y, v.Positions[0].Z, 0, offset, totals, nbi);
                    v.Normals[0] = CreateRotationVector(v.Normals[0].X, v.Normals[0].Y, v.Normals[0].Z, v.Normals[0].W, offset, new float[3], nbi);
                    v.Tangents[0] = CreateRotationVector(v.Tangents[0].X, v.Tangents[0].Y, v.Tangents[0].Z, v.Tangents[0].W, offset, new float[3], nbi);
                    break;
            }
        }

        private void TransformThing(dynamic thing, float offset, IReadOnlyList<float> totals, int nbi, decimal newValue)
        {
            switch (nbi)
            {
                case 0:
                case 1:
                case 2:
                    TranslateThing(thing, offset / 55, nbi);
                    break;
                case 3:
                case 4:
                case 5:
                    ScaleThing(thing, offset, totals, nbi, uniformScaleCheckbox.Checked, false);
                    if (uniformScaleCheckbox.Checked && selectedMeshIndices.Count != 0) scaleXNumBox.Value = scaleYNumBox.Value = scaleZNumBox.Value = newValue;
                    break;
                case 6:
                case 7:
                case 8:
                    RotateThing(thing, offset, totals, nbi);
                    break;
            }
        }

        private float[] CalculateMeshTotals()
        {
            if (useWorldOrigin) return new float[3];
            float vertexCount = 0, xSum = 0, ySum = 0, zSum = 0;
            foreach (int i in selectedMeshIndices)
            {
                foreach (FLVER.Vertex v in flver.Meshes[i].Vertices)
                {
                    xSum += v.Positions[0].X;
                    ySum += v.Positions[0].Y;
                    zSum += v.Positions[0].Z;
                }
                vertexCount += flver.Meshes[i].Vertices.Count;
            }
            return new[] { xSum / vertexCount, ySum / vertexCount, zSum / vertexCount };
        }

        // TODO: Reflect the correct values when performing an undo/redo action
        private void ModifierNumBoxValueChanged(object sender, EventArgs e)
        {
            if (isSettingDefaultInfo) return;
            UpdateUndoState();
            NumericUpDown numBox = (NumericUpDown)sender;
            int nbi = meshModifiersNumBoxesContainer.GetRow(numBox) * meshModifiersNumBoxesContainer.ColumnCount
                + meshModifiersNumBoxesContainer.GetColumn(numBox);
            float newNumVal = (float)(numBox == rotXNumBox || numBox == rotYNumBox || numBox == rotZNumBox ? ToRadians(numBox.Value) : numBox.Value);
            if (numBox == rotYNumBox && selectedMeshIndices.Count != 0) newNumVal = -newNumVal;
            if (numBox == scaleXNumBox || numBox == scaleYNumBox || numBox == scaleZNumBox) newNumVal = (float)(numBox.Value / 100);
            float offset = newNumVal < prevNumVal ?
                -Math.Abs(newNumVal - prevNumVal)
                : Math.Abs(newNumVal - prevNumVal);
            float[] totals = CalculateMeshTotals();
            foreach (FLVER.Vertex v in selectedMeshIndices.SelectMany(i => flver.Meshes[i].Vertices))
                TransformThing(v, offset, totals, nbi, numBox.Value);
            foreach (int i in selectedDummyIndices)
                TransformThing(flver.Dummies[i], offset, totals, nbi, numBox.Value);
            UpdateMesh();
            prevNumVal = newNumVal;
        }

        private void ModifierNumBoxFocused(object sender, EventArgs e)
        {
            NumericUpDown numBox = (NumericUpDown)sender;
            prevNumVal = (float)(numBox == rotXNumBox || numBox == rotYNumBox || numBox == rotZNumBox ? ToRadians(numBox.Value) : numBox.Value);
            if (numBox == rotYNumBox && selectedMeshIndices.Count != 0) prevNumVal = -prevNumVal;
            if (numBox == scaleXNumBox || numBox == scaleYNumBox || numBox == scaleZNumBox) prevNumVal = (float)(numBox.Value / 100);
        }

        private void MaterialsTableOkButtonClicked(object sender, MouseEventArgs e)
        {
            try
            {
                UpdateUndoState();
                foreach (DataGridViewRow row in materialsTable.Rows)
                {
                    if (!(bool)row.Cells[mtApplyPresetCbIndex].Value) continue;
                    string prevName = flver.Materials[row.Index].Name;
                    flver.Materials[row.Index] = new JavaScriptSerializer().Deserialize<FLVER.Material>(
                        new JavaScriptSerializer().Serialize(materialPresets.Values.ToArray()[materialPresetsSelector.SelectedIndex]));
                    flver.Materials[row.Index].Name = prevName;
                }
                for (int i = flver.Materials.Count - 1; i >= 0; --i)
                {
                    if (!(bool)materialsTable.Rows[i].Cells[mtDeleteCbIndex].Value || flver.Materials.Count <= 1) continue;
                    flver.Materials.RemoveAt(i);
                    foreach (FLVER.Mesh mesh in flver.Meshes.Where(mesh => mesh.MaterialIndex > 0))
                        mesh.MaterialIndex--;
                }
                ClearViewerMaterialHighlight();
                UpdateUI();
                UpdateMesh();
                viewer.RefreshTextures();
            }
            catch { }
        }

        private void ModifyAllThings(DataGridView table, int columnIndex)
        {
            bool allChecked = AreCheckboxesInDataTableAllChecked(table, columnIndex);
            foreach (DataGridViewRow row in table.Rows)
            {
                if ((bool)row.Cells[columnIndex].Value && !allChecked) continue;
                switch (columnIndex)
                {
                    case 4 when table == meshTable:
                        hiddenMeshIndices = UpdateIndicesList(meshTable, hiddenMeshIndices, columnIndex, row.Index, ref meshIsHidden);
                        break;
                    case 4 when table == dummiesTable:
                        selectedDummyIndices = UpdateIndicesList(dummiesTable, selectedDummyIndices, columnIndex, row.Index, ref dummyIsSelected);
                        break;
                    case 3:
                        selectedMeshIndices = UpdateIndicesList(meshTable, selectedMeshIndices, columnIndex, row.Index, ref meshIsSelected);
                        break;
                }
            }
            switch (columnIndex)
            {
                case 4 when table == meshTable:
                    UpdateMesh();
                    break;
                case 4 when table == dummiesTable:
                    UpdateSelectedDummies();
                    break;
                case 3:
                    UpdateSelectedMeshes();
                    break;
            }
            isSettingDefaultInfo = true;
            ToggleCheckboxesInDataTable(table, columnIndex);
            isSettingDefaultInfo = false;
        }

        private void SelectAllMeshesButtonClicked(object sender, MouseEventArgs e)
        {
            ModifyAllThings(meshTable, 3);
        }

        private void SelectAllDummiesButtonClicked(object sender, MouseEventArgs e)
        {
            ModifyAllThings(dummiesTable, 4);
        }

        private void DeleteSelectedButtonClicked(object sender, MouseEventArgs e)
        {
            UpdateUndoState();
            for (int i = flver.Meshes.Count - 1; i >= 0; --i)
            {
                if (!(bool)meshTable.Rows[i].Cells[3].Value) continue;
                if (deleteFacesetsCheckbox.Checked)
                {
                    foreach (FLVER.FaceSet fs in flver.Meshes[i].FaceSets)
                        for (int j = 0; j < fs.Vertices.Length; ++j)
                            fs.Vertices[j] = 1;
                }
                else
                {
                    selectedMeshIndices.RemoveAt(selectedMeshIndices.IndexOf(i));
                    flver.Meshes.RemoveAt(i);
                }
            }
            for (int i = flver.Dummies.Count - 1; i >= 0; --i)
            {
                if (!(bool)dummiesTable.Rows[i].Cells[4].Value) continue;
                selectedDummyIndices.RemoveAt(selectedDummyIndices.IndexOf(i));
                flver.Dummies.RemoveAt(i);
            }
            meshModifiersContainer.Enabled = meshIsSelected = dummyIsSelected = false;
            DeselectAllSelectedThings();
            UpdateUI();
            UpdateMesh();
        }

        private void ModifierNumBoxEnterPressed(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) e.SuppressKeyPress = true;
        }

        private static void SaveFLVERFile(string filePath)
        {
            if (IsFLVERPath(filePath))
            {
                BackupFLVERFile();
                flver.Write(filePath);
            }
            else if (filePath.EndsWith(".dcx"))
            {
                BackupFLVERFile();
                flver.Write(filePath);
                flverBnd.Files[currFlverFileBinderIndex].Bytes = File.ReadAllBytes(filePath);
                flverBnd.Write(filePath, flverArchiveType);
            }
        }

        private void SaveFLVERAs()
        {
            string bndFilter = flverFilePath.EndsWith(".dcx") ? "|BND File (*.dcx)|*.dcx" : "";
            SaveFileDialog dialog = new SaveFileDialog
                { Filter = $@"FLVER File (*.flver, *.flv)|*.flver;*.flv{bndFilter}", FileName = Path.GetFileNameWithoutExtension(flverFilePath.Replace(".dcx", "")) };
            if (dialog.ShowDialog() != DialogResult.OK) return;
            string modelFilePath = dialog.FileName;
            if (flverFilePath.EndsWith(".dcx"))
            {
                int newPartID = GetModelPartIDFromName(modelFilePath);
                if (newPartID != 0)
                {
                    int ogModelPartID = GetModelPartIDFromName(flverFilePath);
                    if (ogModelPartID != -1)
                    {
                        foreach (BinderFile file in flverBnd.Files)
                        {
                            if (!Path.GetFileName(file.Name).Contains(ogModelPartID.ToString())) continue;
                            if (file.Name == null) continue;
                            string newInternalPath = file.Name.Replace(ogModelPartID.ToString(), newPartID.ToString());
                            file.Name = newInternalPath;
                        }
                    }
                }
            }
            SaveFLVERFile(modelFilePath);
            arguments.Add(modelFilePath);
            OpenFLVERFile();
        }

        private void SaveButtonClicked(object sender, EventArgs e)
        {
            SaveFLVERFile(flverFilePath);
        }

        private void SaveAsButtonClicked(object sender, EventArgs e)
        {
            SaveFLVERAs();
        }

        private static bool IsTextBoxCell(object sender, int columnIndex, int rowIndex)
        {
            return ((DataGridView)sender)[columnIndex, rowIndex] is DataGridViewTextBoxCell;
        }

        private void BonesTableCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (isSettingDefaultInfo || !IsTextBoxCell(sender, e.ColumnIndex, e.RowIndex)) return;
            try
            {
                UpdateUndoState();
                string bonesTableValue = bonesTable[e.ColumnIndex, e.RowIndex].Value?.ToString();
                if (bonesTableValue != null)
                {
                    switch (e.ColumnIndex)
                    {
                        case 1:
                            flver.Bones[e.RowIndex].Name = bonesTableValue;
                            break;
                        case 2:
                            flver.Bones[e.RowIndex].ParentIndex = short.Parse(bonesTableValue);
                            break;
                        case 3:
                            flver.Bones[e.RowIndex].ChildIndex = short.Parse(bonesTableValue);
                            break;
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                            string[] comp = bonesTableValue.Split(',');
                            System.Numerics.Vector3 vector = new System.Numerics.Vector3(float.Parse(comp[0]), float.Parse(comp[1]), float.Parse(comp[2]));
                            flver.Bones[e.RowIndex].Translation = e.ColumnIndex == 4 ? vector : flver.Bones[e.RowIndex].Translation;
                            flver.Bones[e.RowIndex].Scale = e.ColumnIndex == 5 ? vector : flver.Bones[e.RowIndex].Scale;
                            flver.Bones[e.RowIndex].Rotation = e.ColumnIndex == 6 ? vector : flver.Bones[e.RowIndex].Rotation;
                            flver.Bones[e.RowIndex].BoundingBoxMin = e.ColumnIndex == 7 ? vector : flver.Bones[e.RowIndex].BoundingBoxMin;
                            flver.Bones[e.RowIndex].BoundingBoxMax = e.ColumnIndex == 8 ? vector : flver.Bones[e.RowIndex].BoundingBoxMax;
                            break;
                    }
                }
            }
            catch { }
            UpdateUI();
            UpdateMesh();
        }

        private static string ReplaceModelMask(string materialName, string newModelMaskStr)
        {
            string ogModelMask = GetModelMask(materialName).ToString().PadLeft(2, '0');
            string newModelMask = newModelMaskStr.PadLeft(2, '0');
            int.TryParse(newModelMask, out int newModelMaskVal);
            if (newModelMaskVal >= 0 && newModelMask.Length == 2)
            {
                return materialName.IndexOf(ogModelMask, StringComparison.Ordinal) == -1 ?
                    materialName.Insert(0, $"#{newModelMask}#") :
                    materialName.Replace(ogModelMask, newModelMask);
            }
            ShowInformationDialog("The input value must be a positive two-digit integer.");
            return materialName;
        }

        private void MaterialsTableCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (isSettingDefaultInfo || !IsTextBoxCell(sender, e.ColumnIndex, e.RowIndex)) return;
            try
            {
                UpdateUndoState();
                string materialName = flver.Materials[e.RowIndex].Name;
                string materialsTableValue = materialsTable[e.ColumnIndex, e.RowIndex].Value?.ToString();
                if (materialsTableValue != null)
                {
                    switch (e.ColumnIndex)
                    {
                        case 1:
                            flver.Materials[e.RowIndex].Name = materialsTableValue;
                            break;
                        case 2:
                            flver.Materials[e.RowIndex].Name = ReplaceModelMask(materialName, materialsTableValue);
                            break;
                        case 3:
                            flver.Materials[e.RowIndex].Flags = int.Parse(materialsTableValue);
                            break;
                        case 4:
                            flver.Materials[e.RowIndex].MTD = materialsTableValue;
                            break;
                        case 5:
                            flver.Materials[e.RowIndex].Unk18 = int.Parse(materialsTableValue);
                            break;
                    }
                }
            }
            catch { }
            UpdateUI();
            UpdateMesh();
            viewer.RefreshTextures();
        }

        private void TexturesTableCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (isSettingDefaultInfo || !IsTextBoxCell(sender, e.ColumnIndex, e.RowIndex)) return;
            try
            {
                UpdateUndoState();
                string textureTableValue = texturesTable[e.ColumnIndex, e.RowIndex].Value?.ToString() ?? "";
                switch (e.ColumnIndex)
                {
                    case 0:
                        flver.Materials[selectedMaterialIndex].Textures[e.RowIndex].Type = textureTableValue;
                        break;
                    case 1:
                        flver.Materials[selectedMaterialIndex].Textures[e.RowIndex].Path = textureTableValue;
                        break;
                }
            }
            catch { }
            UpdateMesh();
            viewer.RefreshTextures();
        }

        public static void ShowInformationDialog(string str)
        {
            MessageBox.Show(str, @"Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void ShowErrorDialog(string str)
        {
            MessageBox.Show(str, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static DialogResult ShowQuestionDialog(string str)
        {
            return MessageBox.Show(str, @"Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        public static string ShowInputDialog(string text, string caption)
        {
            Form prompt = new Form
            {
                Width = 240,
                Height = 125,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen,
                MaximizeBox = false
            };
            Label textLabel = new Label { Left = 8, Top = 8, Width = 200, Text = text };
            TextBox textBox = new TextBox { Left = 10, Top = 28, Width = 200 };
            Button cancelButton = new Button { Text = @"Cancel", Left = 9, Width = 100, Top = 55, DialogResult = DialogResult.Cancel };
            cancelButton.Click += (sender, e) => { prompt.Close(); };
            Button confirmation = new Button { Text = @"OK", Left = 112, Width = 100, Top = 55, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(cancelButton);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;
            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }

        private void MeshTableCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (isSettingDefaultInfo || !IsTextBoxCell(sender, e.ColumnIndex, e.RowIndex) || e.ColumnIndex != 2) return;
            try
            {
                UpdateUndoState();
                string boneWeightValue = meshTable[2, e.RowIndex].Value?.ToString();
                if (boneWeightValue != null)
                {
                    int newBoneWeight = int.Parse(boneWeightValue);
                    foreach (FLVER.Vertex v in flver.Meshes[e.RowIndex].Vertices)
                    {
                        for (int i = 0; i < v.Positions.Count; ++i)
                        {
                            if (v.BoneWeights == null)
                            {
                                v.BoneWeights = new float[4];
                                v.BoneIndices = new int[4];
                            }
                            for (int j = 0; j < v.BoneWeights.Length; ++j)
                                v.BoneWeights[j] = 0;
                            v.BoneIndices[0] = newBoneWeight;
                            v.BoneWeights[0] = 1;
                        }
                    }
                    if (!flver.Meshes[e.RowIndex].BoneIndices.Contains(newBoneWeight)) flver.Meshes[e.RowIndex].BoneIndices.Add(newBoneWeight);
                    flver.Meshes[e.RowIndex].Dynamic = true;
                }
            }
            catch { }
            UpdateUI();
            UpdateMesh();
        }

        private void ReverseFaceSets()
        {
            UpdateUndoState();
            foreach (FLVER.FaceSet fs in selectedMeshIndices.SelectMany(i => flver.Meshes[i].FaceSets))
            {
                for (int j = 0; j < fs.Vertices.Length; j += 3)
                    (fs.Vertices[j + 1], fs.Vertices[j + 2]) = (fs.Vertices[j + 2], fs.Vertices[j + 1]);
            }
            UpdateMesh();
        }

        private void ReverseFaceSetsCheckboxChanged(object sender, EventArgs e)
        {
            if (isSettingDefaultInfo) return;
            ReverseFaceSets();
        }

        private void ReverseNormalsCheckboxChanged(object sender, EventArgs e)
        {
            if (isSettingDefaultInfo) return;
            UpdateUndoState();
            foreach (FLVER.Vertex v in selectedMeshIndices.SelectMany(i => flver.Meshes[i].Vertices))
            {
                for (int j = 0; j < v.Normals.Count; ++j)
                    v.Normals[j] = new Vector4(-v.Normals[j].X, -v.Normals[j].Y, -v.Normals[j].Z, v.Normals[j].W);
                for (int j = 0; j < v.Tangents.Count; ++j)
                    v.Tangents[j] = new Vector4(-v.Tangents[j].X, -v.Tangents[j].Y, -v.Tangents[j].Z, v.Tangents[j].W);
            }
            ShowInformationDialog("Mesh normals have been reversed!");
            UpdateMesh();
        }

        private void ToggleBackFacesCheckboxChanged(object sender, EventArgs e)
        {
            if (isSettingDefaultInfo) return;
            foreach (FLVER.FaceSet fs in selectedMeshIndices.SelectMany(i => flver.Meshes[i].FaceSets))
                fs.CullBackfaces = !fs.CullBackfaces;
            ShowInformationDialog("Mesh backfaces have been toggled!");
        }

        private static void UpdateHeaderBoundingBox(FLVER.FLVERHeader header, System.Numerics.Vector3 vertexPos)
        {
            float minX = Math.Min(header.BoundingBoxMin.X, vertexPos.X);
            float minY = Math.Min(header.BoundingBoxMin.Y, vertexPos.Y);
            float minZ = Math.Min(header.BoundingBoxMin.Z, vertexPos.Z);
            float maxX = Math.Max(header.BoundingBoxMax.X, vertexPos.X);
            float maxY = Math.Max(header.BoundingBoxMax.Y, vertexPos.Y);
            float maxZ = Math.Max(header.BoundingBoxMax.Z, vertexPos.Z);
            header.BoundingBoxMin = new System.Numerics.Vector3(minX, minY, minZ);
            header.BoundingBoxMax = new System.Numerics.Vector3(maxX, maxY, maxZ);
        }

        private static void UpdateMeshBoundingBox(FLVER.Mesh mesh, System.Numerics.Vector3 vertexPos)
        {
            float minX = Math.Min(mesh.BoundingBoxMin.X, vertexPos.X);
            float minY = Math.Min(mesh.BoundingBoxMin.Y, vertexPos.Y);
            float minZ = Math.Min(mesh.BoundingBoxMin.Z, vertexPos.Z);
            float maxX = Math.Max(mesh.BoundingBoxMax.X, vertexPos.X);
            float maxY = Math.Max(mesh.BoundingBoxMax.Y, vertexPos.Y);
            float maxZ = Math.Max(mesh.BoundingBoxMax.Z, vertexPos.Z);
            mesh.BoundingBoxMin = new System.Numerics.Vector3(minX, minY, minZ);
            mesh.BoundingBoxMax = new System.Numerics.Vector3(maxX, maxY, maxZ);
        }

        private static Matrix4x4 GetNMatrix(FLVER.Bone b)
        {
            return Matrix4x4.CreateScale(b.Scale)
                * Matrix4x4.CreateRotationX(b.Rotation.X)
                * Matrix4x4.CreateRotationZ(b.Rotation.Z)
                * Matrix4x4.CreateRotationY(b.Rotation.Y)
                * Matrix4x4.CreateTranslation(b.Translation);
        }

        private static FLVER.Bone GetParent(FLVER.Bone b, IReadOnlyList<FLVER.Bone> bones)
        {
            if (b.ParentIndex >= 0 && b.ParentIndex < bones.Count) return bones[b.ParentIndex];
            return null;
        }

        private static Matrix4x4 GetAbsoluteNMatrix(FLVER.Bone b, IReadOnlyList<FLVER.Bone> bones)
        {
            Matrix4x4 result = Matrix4x4.Identity;
            FLVER.Bone parentBone = b;
            while (parentBone != null)
            {
                Matrix4x4 m = GetNMatrix(parentBone);
                result *= m;
                parentBone = GetParent(parentBone, bones);
            }
            return result;
        }

        private static void UpdateBonesBoundingBox(FLVER.Bone b, IReadOnlyList<FLVER.Bone> bones, System.Numerics.Vector3 vertexPos)
        {
            Matrix4x4 boneAbsoluteMatrix = GetAbsoluteNMatrix(b, bones);
            if (!Matrix4x4.Invert(boneAbsoluteMatrix, out Matrix4x4 invertedBoneMatrix)) return;
            System.Numerics.Vector3 posForBBox = System.Numerics.Vector3.Transform(vertexPos, invertedBoneMatrix);
            float minX = Math.Min(b.BoundingBoxMin.X, posForBBox.X);
            float minY = Math.Min(b.BoundingBoxMin.Y, posForBBox.Y);
            float minZ = Math.Min(b.BoundingBoxMin.Z, posForBBox.Z);
            float maxX = Math.Max(b.BoundingBoxMax.X, posForBBox.X);
            float maxY = Math.Max(b.BoundingBoxMax.Y, posForBBox.Y);
            float maxZ = Math.Max(b.BoundingBoxMax.Z, posForBBox.Z);
            b.BoundingBoxMin = new System.Numerics.Vector3(minX, minY, minZ);
            b.BoundingBoxMax = new System.Numerics.Vector3(maxX, maxY, maxZ);
        }

        private void SolveAllBBs()
        {
            UpdateUndoState();
            flver.Header.BoundingBoxMin = new System.Numerics.Vector3();
            flver.Header.BoundingBoxMax = new System.Numerics.Vector3();
            foreach (FLVER.Bone bone in flver.Bones)
            {
                bone.BoundingBoxMin = new System.Numerics.Vector3();
                bone.BoundingBoxMax = new System.Numerics.Vector3();
            }
            foreach (FLVER.Mesh mesh in flver.Meshes)
            {
                foreach (FLVER.Vertex vertex in mesh.Vertices)
                {
                    UpdateHeaderBoundingBox(flver.Header, vertex.Positions[0]);
                    UpdateMeshBoundingBox(mesh, vertex.Positions[0]);
                    if (vertex.BoneIndices == null) continue;
                    foreach (int boneIndex in vertex.BoneIndices)
                    {
                        bool boneDoesNotExist = false;
                        if (boneIndex >= 0 && boneIndex < flver.Bones.Count) flver.Bones[boneIndex].Unk3C = 0;
                        else boneDoesNotExist = true;
                        if (!boneDoesNotExist) UpdateBonesBoundingBox(flver.Bones[boneIndex], flver.Bones, vertex.Positions[0]);
                    }
                }
            }
            ShowInformationDialog("Solved all bone and mesh bounding boxes!");
            UpdateMesh();
        }

        private void SolveAllBBsButtonClicked(object sender, MouseEventArgs e)
        {
            SolveAllBBs();
        }

        private void DummiesTableOKButtonClicked(object sender, MouseEventArgs e)
        {
            if (dummyPresetsSelector.SelectedIndex < 0) return;
            UpdateUndoState();
            DeselectAllSelectedThings();
            string dummyJson = new JavaScriptSerializer().Serialize(dummyPresets.Values.ToArray()[dummyPresetsSelector.SelectedIndex]);
            flver.Dummies = new JavaScriptSerializer().Deserialize<List<FLVER.Dummy>>(dummyJson);
            UpdateUI();
            UpdateMesh();
        }

        private void AddAllDummiesToPresetsButtonClicked(object sender, MouseEventArgs e)
        {
            string presetName = PromptForPresetName();
            if (presetName == "" || dummyPresets.ContainsKey(presetName)) return;
            dummyPresets.Add(presetName, new JavaScriptSerializer().Deserialize<object>(new JavaScriptSerializer().Serialize(flver.Dummies)));
            File.WriteAllText(dummyPresetsFilePath, new JavaScriptSerializer().Serialize(dummyPresets));
            LoadDummyPresets();
        }

        private static void ExportFLVERAsDAE()
        {
            SaveFileDialog dialog = new SaveFileDialog { FileName = $"{Path.GetFileNameWithoutExtension(flverFilePath)}.dae", Filter = @"Collada DAE File (*.dae)|*.dae" };
            if (dialog.ShowDialog() != DialogResult.OK) return;
            try
            {
                Scene scene = new Scene { RootNode = new Node() };
                foreach (FLVER.Material m in flver.Materials)
                    scene.Materials.Add(new Material { Name = m.Name });
                for (int i = 0; i < flver.Meshes.Count; ++i)
                {
                    FLVER.Mesh m = flver.Meshes[i];
                    Mesh newMesh = new Mesh("Mesh_M" + i, PrimitiveType.Triangle);
                    foreach (FLVER.Vertex v in m.Vertices)
                    {
                        newMesh.Vertices.Add(new Assimp.Vector3D(v.Positions[0].X, v.Positions[0].Y, v.Positions[0].Z));
                        newMesh.Normals.Add(new Assimp.Vector3D(v.Normals[0].X, v.Normals[0].Y, v.Normals[0].Z));
                        newMesh.Tangents.Add(new Assimp.Vector3D(v.Tangents[0].X, v.Tangents[0].Y, v.Tangents[0].Z));
                        for (int j = 0; j < v.UVs.Count; ++j)
                            newMesh.TextureCoordinateChannels[j].Add(new Assimp.Vector3D(v.UVs[j].X, 1 - v.UVs[j].Y, 0));
                    }
                    foreach (uint[] arr in m.FaceSets.SelectMany(fs => fs.GetFaces()))
                        newMesh.Faces.Add(new Face(new[] { (int)arr[0], (int)arr[1], (int)arr[2] }));
                    newMesh.MaterialIndex = m.MaterialIndex;
                    scene.Meshes.Add(newMesh);
                    Node nodeBase = new Node { Name = "M_" + i + "_" + flver.Materials[m.MaterialIndex].Name };
                    nodeBase.MeshIndices.Add(i);
                    scene.RootNode.Children.Add(nodeBase);
                }
                AssimpContext exporter = new AssimpContext();
                bool hasExported = exporter.ExportFile(scene, dialog.FileName, "collada");
                if (hasExported) ShowInformationDialog("Successfully exported FLVER file to the Collada DAE format!");
            }
            catch
            {
                ShowInformationDialog("An error occurred during the exporting process.");
            }
        }

        private void ExportToolStripMenuItemClicked(object sender, EventArgs e)
        {
            ExportFLVERAsDAE();
        }

        private string PromptImportModel()
        {
            OpenFileDialog dialog = new OpenFileDialog { Filter = @"3D Object|*.dae;*.obj;*.fbx" };
            return dialog.ShowDialog() != DialogResult.OK ? "" : dialog.FileName;
        }

        private void ImportFLVERFile(bool prompt, string filePath)
        {
            UpdateUndoState();
            if (prompt)
            {
                OpenFileDialog dialog = new OpenFileDialog { Filter = @"3D Object|*.dae;*.obj;*.fbx" };
                if (dialog.ShowDialog() != DialogResult.OK) return;
                if (!Program.ImportFBX(dialog.FileName)) return;
            }
            else if (!Program.ImportFBX(filePath)) return;
            UpdateUndoState();
            flver = Program.flver;
            DeselectAllSelectedThings();
            UpdateUI();
            UpdateMesh();
            viewer.RefreshTextures();
        }

        private void ImportToolStripMenuItemClicked(object sender, EventArgs e)
        {
            ImportFLVERFile(true, "");
        }

        private void MergeFLVERFile()
        {
            string newFlverFilePath = PromptFLVERModel();
            if (newFlverFilePath == "") return;
            try
            {
                UpdateUndoState();
                FLVER newFlver = IsFLVERPath(newFlverFilePath) ? FLVER.Read(newFlverFilePath) :
                    ReadFLVERFromDCXPath(newFlverFilePath, false, false, false);
                if (newFlver == null) return;
                int materialOffset = flver.Materials.Count;
                int layoutOffset = flver.BufferLayouts.Count;
                Dictionary<int, int> newFlverToCurrentFlver = new Dictionary<int, int>();
                for (int i = 0; i < newFlver.Bones.Count; ++i)
                {
                    FLVER.Bone attachBone = newFlver.Bones[i];
                    for (int j = 0; j < flver.Bones.Count; ++j)
                    {
                        if (attachBone.Name != flver.Bones[j].Name) continue;
                        newFlverToCurrentFlver.Add(i, j);
                        break;
                    }
                }
                foreach (FLVER.Mesh m in newFlver.Meshes)
                {
                    m.MaterialIndex += materialOffset;
                    foreach (FLVER.VertexBuffer vb in m.VertexBuffers)
                        vb.LayoutIndex += layoutOffset;
                    foreach (FLVER.Vertex v in m.Vertices.Where(v => v.BoneIndices != null))
                    {
                        for (int i = 0; i < v.BoneIndices.Length; ++i)
                        {
                            if (newFlverToCurrentFlver.ContainsKey(v.BoneIndices[i])) v.BoneIndices[i] = newFlverToCurrentFlver[v.BoneIndices[i]];
                        }
                    }
                }
                flver.BufferLayouts = flver.BufferLayouts.Concat(newFlver.BufferLayouts).ToList();
                flver.Meshes = flver.Meshes.Concat(newFlver.Meshes).ToList();
                flver.Materials = flver.Materials.Concat(newFlver.Materials).ToList();
                ShowInformationDialog(@"Successfully attached new FLVER to the current one!");
                DeselectAllSelectedThings();
                UpdateUI();
                UpdateMesh();
                viewer.RefreshTextures();
            }
            catch
            {
                ShowErrorDialog("An error occurred while attempting to merge external FLVER with the current one.");
            }
        }

        private void MergeToolStripMenuItemClicked(object sender, EventArgs e)
        {
            MergeFLVERFile();
        }

        public static bool PromptToSaveFLVERFile(FormClosingEventArgs e)
        {
            if (!IsMainWindowFocused()) return true;
            byte[] newFlverBytes = flver.Write();
            if (newFlverBytes.SequenceEqual(currFlverBytes)) return true;
            DialogResult result = MessageBox.Show(@"Do you want to save changes to the FLVER before quitting?", @"Warning", MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
            switch (result)
            {
                case DialogResult.Yes:
                    SaveFLVERFile(flverFilePath);
                    break;
                case DialogResult.Cancel:
                    e.Cancel = true;
                    return false;
            }
            return true;
        }

        private void MainWindowClosing(object sender, FormClosingEventArgs e)
        {
            PromptToSaveFLVERFile(e);
        }

        private void AddDummyButtonClicked(object sender, MouseEventArgs e)
        {
            UpdateUndoState();
            FLVER.Dummy newDummy = new FLVER.Dummy
            {
                Position = flver.Dummies.Count > 0 ? flver.Dummies[flver.Dummies.Count - 1].Position : new System.Numerics.Vector3(0, 0, 0),
                ReferenceID = -1
            };
            flver.Dummies.Add(newDummy);
            DeselectAllSelectedThings();
            UpdateUI();
            UpdateMesh();
        }

        private void DummiesTableCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (isSettingDefaultInfo || !IsTextBoxCell(sender, e.ColumnIndex, e.RowIndex)) return;
            try
            {
                UpdateUndoState();
                string dummiesTableValue = dummiesTable[e.ColumnIndex, e.RowIndex].Value?.ToString();
                if (dummiesTableValue != null)
                {
                    short parsed = short.Parse(dummiesTableValue);
                    switch (e.ColumnIndex)
                    {
                        case 1:
                            flver.Dummies[e.RowIndex].ReferenceID = parsed;
                            break;
                        case 2:
                            flver.Dummies[e.RowIndex].AttachBoneIndex = parsed;
                            break;
                        case 3:
                            flver.Dummies[e.RowIndex].DummyBoneIndex = parsed;
                            break;
                    }
                }
            }
            catch { }
            UpdateUI();
            UpdateMesh();
        }

        private static decimal ToRadians(decimal degrees) { return degrees * (decimal)(Math.PI / 180); }

        private void LoadJSON(int type)
        {
            OpenFileDialog dialog = new OpenFileDialog { Filter = jsonFileFilter };
            if (dialog.ShowDialog() != DialogResult.OK) return;
            try
            {
                UpdateUndoState();
                string jsonText = File.ReadAllText(dialog.FileName);
                switch (type)
                {
                    case 0:
                        flver.Bones = JsonConvert.DeserializeObject<List<FLVER.Bone>>(jsonText);
                        break;
                    case 1:
                        flver.Materials = JsonConvert.DeserializeObject<List<FLVER.Material>>(jsonText);
                        break;
                }
                DeselectAllSelectedThings();
                ClearViewerMaterialHighlight();
                UpdateUI();
                UpdateMesh();
                ShowInformationDialog("Successfully parsed JSON!");
            }
            catch
            {
                ShowInformationDialog("The JSON could not be found or read.");
            }
        }

        private static void ExportJSON(dynamic list)
        {
            SaveFileDialog dialog = new SaveFileDialog { Filter = jsonFileFilter };
            if (dialog.ShowDialog() != DialogResult.OK) return;
            try
            {
                File.WriteAllText(dialog.FileName, JsonConvert.SerializeObject(list, Formatting.Indented));
                ShowInformationDialog("Successfully exported JSON!");
            }
            catch
            {
                ShowInformationDialog("An error occurred while attempting to export JSON.");
            }
        }

        private void LoadBonesJSONButtonClicked(object sender, EventArgs e)
        {
            LoadJSON(0);
        }

        private void LoadMaterialsJSONButtonClicked(object sender, EventArgs e)
        {
            LoadJSON(1);
        }

        private void ExportBonesJSONButtonClicked(object sender, EventArgs e)
        {
            ExportJSON(flver.Bones);
        }

        private void ExportMaterialsJSONButtonClicked(object sender, EventArgs e)
        {
            ExportJSON(flver.Materials);
        }

        private void BrowsePresetsFile(bool materialPresetsFile)
        {
            OpenFileDialog dialog = new OpenFileDialog { Filter = jsonFileFilter, Multiselect = false };
            if (dialog.ShowDialog() != DialogResult.OK) return;
            File.WriteAllText(materialPresetsFile ? materialPresetsFilePath : dummyPresetsFilePath, File.ReadAllText(dialog.FileName));
            if (materialPresetsFile) LoadMaterialPresets();
            else LoadDummyPresets();
            ShowInformationDialog("Successfully loaded presets file!");
        }

        private void BrowseMaterialPresetsFileButtonClicked(object sender, EventArgs e)
        {
            BrowsePresetsFile(true);
        }

        private void BrowseDummyPresetsFileButtonClicked(object sender, EventArgs e)
        {
            BrowsePresetsFile(false);
        }

        private void ToggleTextureRefreshButtonClicked(object sender, EventArgs e)
        {
            textureRefreshEnabled = !textureRefreshEnabled;
            userConfigJson["TextureRefreshing"] = textureRefreshEnabled;
            WriteUserConfig();
            if (textureRefreshEnabled)
            {
                ShowInformationDialog("Texture refreshing is now enabled!");
                viewer.RefreshTextures();
            }
            else ShowInformationDialog("Texture refreshing is now disabled to help improve performance!");
        }

        private void MergePresets(bool materialPresetsFile)
        {
            OpenFileDialog dialog = new OpenFileDialog { Filter = jsonFileFilter, Multiselect = false };
            if (dialog.ShowDialog() != DialogResult.OK) return;
            Dictionary<object, object> newPresets = new JavaScriptSerializer().Deserialize<Dictionary<object, object>>(File.ReadAllText(dialog.FileName));
            Dictionary<object, object> presets = materialPresetsFile ? materialPresets : dummyPresets;
            foreach (KeyValuePair<object, object> preset in newPresets.Where(preset => !presets.ContainsKey(preset.Key)))
                presets.Add(preset.Key, preset.Value);
            File.WriteAllText(materialPresetsFile ? materialPresetsFilePath : dummyPresetsFilePath, new JavaScriptSerializer().Serialize(presets));
            if (materialPresetsFile) LoadMaterialPresets();
            else LoadDummyPresets();
            ShowInformationDialog("Successfully merged presets with current presets!");
        }

        private void MergeMaterialPresetsButtonClicked(object sender, EventArgs e)
        {
            MergePresets(true);
        }

        private void MergeDummyPresetsButtonClicked(object sender, EventArgs e)
        {
            MergePresets(false);
        }

        private void SetAllBBsMaxSize()
        {
            UpdateUndoState();
            System.Numerics.Vector3 minVector = new System.Numerics.Vector3(0, 0, 0);
            System.Numerics.Vector3 maxVector = new System.Numerics.Vector3(999, 999, 999);
            flver.Header.BoundingBoxMin = maxVector;
            flver.Header.BoundingBoxMax = minVector;
            foreach (FLVER.Mesh mesh in from mesh in flver.Meshes from vertex in mesh.Vertices select mesh)
            {
                mesh.BoundingBoxMin = maxVector;
                mesh.BoundingBoxMax = minVector;
            }
            ShowInformationDialog("Set all mesh bounding boxes to maximum size!");
            UpdateMesh();
        }

        private void SetAllBBsMaxSizeButtonClicked(object sender, EventArgs e)
        {
            SetAllBBsMaxSize();
        }

        private void TabWindowDrawItem(object sender, DrawItemEventArgs e)
        {
            Rectangle rec = tabWindow.ClientRectangle;
            StringFormat StrFormat = new StringFormat();
            StrFormat.LineAlignment = StringAlignment.Center;
            StrFormat.Alignment = StringAlignment.Center;
            SolidBrush backColor = new SolidBrush(tabWindowBackColor);
            e.Graphics.FillRectangle(backColor, rec);
            Font fntTab = e.Font;
            for (int i = 0; i < tabWindow.TabPages.Count; i++)
            {
                RectangleF tabTextArea = tabWindow.GetTabRect(i);
                SolidBrush fontColor = new SolidBrush(tabWindowForeColor);
                e.Graphics.DrawString(tabWindow.TabPages[i].Text, fntTab, fontColor, tabTextArea, StrFormat);
            }
        }

        private void MainWindowLoad(object sender, EventArgs e)
        {
            UpdateWindowTitle(flverFilePath);
            tabWindow.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabWindow.DrawItem += TabWindowDrawItem;
            TopMost = false;
        }

        // TODO: Investigate conflicts with detail overlay textures
        private void ApplyMATBINTexturesButtonClicked(object sender, EventArgs e)
        {
            if (matBinBndPath == null)
            {
                try
                {
                    string matBinBndPathStr = userConfigJson["MatBinBndPath"]?.ToString();
                    matBinBndPath = matBinBndPathStr ?? throw new Exception();
                    File.ReadAllBytes(matBinBndPath);
                }
                catch
                {
                    matBinBndPath = null;
                    OpenFileDialog dialog = new OpenFileDialog { Filter = @"MATBIN BND (*.matbinbnd.dcx)|*.matbinbnd.dcx" };
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        matBinBndPath = dialog.FileName;
                        userConfigJson["MatBinBndPath"] = matBinBndPath;
                        WriteUserConfig();
                    }
                    else
                    {
                        return;
                    }
                }
            }
            if (matBinBnd == null) matBinBnd = BND4.Read(matBinBndPath);
            foreach (BinderFile matBinFile in matBinBnd.Files)
            {
                string rawMaterialFileName = Path.GetFileNameWithoutExtension(flver.Materials[selectedMaterialIndex].MTD);
                string rawMatBinFileName = Path.GetFileNameWithoutExtension(matBinFile.Name);
                if (rawMaterialFileName != rawMatBinFileName) continue;
                MATBIN matBin = new MATBIN();
                matBin.Read(new BinaryReaderEx(false, matBinFile.Bytes));
                if (matBin.Samplers.Any(sampler => sampler.Path != ""))
                {
                    flver.Materials[selectedMaterialIndex].Textures.Clear();
                    foreach (FLVER.Texture newTexture in matBin.Samplers.Select(sampler => new FLVER.Texture { Type = sampler.Type, Path = sampler.Path }))
                        flver.Materials[selectedMaterialIndex].Textures.Add(newTexture);
                }
                break;
            }
            UpdateTexturesTable();
        }

        private void DummyThicknessSelectorSelectedIndexChanged(object sender, EventArgs e)
        {
            if (isSettingDefaultInfo) return;
            dummyThickness = int.Parse(dummyThicknessSelector.Items[dummyThicknessSelector.SelectedIndex].ToString());
            userConfigJson["DummyThickness"] = dummyThickness;
            WriteUserConfig();
            UpdateMesh();
        }

        private void MaterialsPagePanelsContainerSplitterMoved(object sender, SplitterEventArgs e)
        {
            currMaterialsTableSplitDistance = int.Parse(e.SplitX.ToString());
            userConfigJson["MaterialsTableViewSplitDistance"] = currMaterialsTableSplitDistance;
            WriteUserConfig();
        }

        private void MirrorMesh(int nbi)
        {
            UpdateUndoState();
            float[] totals = CalculateMeshTotals();
            foreach (FLVER.Vertex v in selectedMeshIndices.SelectMany(i => flver.Meshes[i].Vertices))
            {
                v.Positions[0] = new System.Numerics.Vector3((v.Positions[0].X - (nbi == 0 && !useWorldOrigin ? totals[0] : 0)) * (nbi == 0 ? -1 : 1),
                    (v.Positions[0].Y - (nbi == 1 && !useWorldOrigin ? totals[1] : 0)) * (nbi == 1 ? -1 : 1),
                    (v.Positions[0].Z - (nbi == 2 && !useWorldOrigin ? totals[2] : 0)) * (nbi == 2 ? -1 : 1));
                v.Positions[0] = new System.Numerics.Vector3(v.Positions[0].X + (nbi == 0 && !useWorldOrigin ? totals[0] : 0),
                    v.Positions[0].Y + (nbi == 1 && !useWorldOrigin ? totals[1] : 0),
                    v.Positions[0].Z + (nbi == 2 && !useWorldOrigin ? totals[2] : 0));
                v.Normals[0] = new Vector4(v.Normals[0].X * (nbi == 0 ? -1 : 1),
                    v.Normals[0].Y * (nbi == 1 ? -1 : 1), v.Normals[0].Z * (nbi == 2 ? -1 : 1), -1);
                v.Tangents[0] = new Vector4(v.Tangents[0].X * (nbi == 0 ? -1 : 1),
                    v.Tangents[0].Y * (nbi == 1 ? -1 : 1), v.Tangents[0].Z * (nbi == 2 ? -1 : 1), 1);
            }
            ReverseFaceSets();
            UpdateMesh();
        }

        private void MirrorXCheckboxCheckedChanged(object sender, EventArgs e)
        {
            MirrorMesh(0);
        }

        private void MirrorYCheckboxCheckedChanged(object sender, EventArgs e)
        {
            MirrorMesh(1);
        }

        private void MirrorZCheckboxCheckedChanged(object sender, EventArgs e)
        {
            MirrorMesh(2);
        }

        private void ToggleAutoSaveToolStripMenuItemClick(object sender, EventArgs e)
        {
            ToggleAutoSaveState(true, true);
        }

        private void AutoSaveTimerTick(object sender, EventArgs e)
        {
            if (flverFilePath != null) SaveFLVERFile(flverFilePath);
        }

        private bool CheckAutoSaveInterval(string intervalStr)
        {
            if (!Regex.IsMatch(intervalStr, "^[0-9]*$")) return false;
            try
            {
                int newInterval = int.Parse(intervalStr);
                if (newInterval == 0 || newInterval > 60) return false;
                autoSaveTimer.Interval = newInterval * 60000;
                currAutoSaveInterval = intervalStr;
                userConfigJson["AutoSaveInterval"] = currAutoSaveInterval;
                WriteUserConfig();
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void AutoSaveIntervalSelectorTextChanged(object sender, EventArgs e)
        {
            if (isSettingDefaultInfo) return;
            if (!CheckAutoSaveInterval(autoSaveIntervalSelector.Text)) autoSaveIntervalSelector.Text = "";
        }

        private void AutoSaveIntervalSelectorKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            preferencesToolStripMenuItem.HideDropDown();
        }

        private void MainWindowKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Control)
            {
                case true when e.KeyCode == Keys.O:
                    e.SuppressKeyPress = true;
                    OpenFLVERFile();
                    break;
                case true when !e.Shift && e.KeyCode == Keys.S:
                    e.SuppressKeyPress = true;
                    SaveFLVERFile(flverFilePath);
                    break;
                case true when e.Shift && e.KeyCode == Keys.S:
                    e.SuppressKeyPress = true;
                    SaveFLVERAs();
                    break;
                case true when e.KeyCode == Keys.E:
                    e.SuppressKeyPress = true;
                    ExportFLVERAsDAE();
                    break;
                case true when e.KeyCode == Keys.I:
                    e.SuppressKeyPress = true;
                    ImportFLVERFile(true, "");
                    break;
                case true when !e.Shift && e.KeyCode == Keys.M:
                    e.SuppressKeyPress = true;
                    MergeFLVERFile();
                    break;
                case true when e.KeyCode == Keys.Z:
                    e.SuppressKeyPress = true;
                    Undo();
                    break;
                case true when e.KeyCode == Keys.Y:
                    e.SuppressKeyPress = true;
                    Redo();
                    break;
                case true when e.Shift && e.KeyCode == Keys.M:
                    ModifyAllThings(meshTable, 3);
                    break;
                case true when e.Shift && e.KeyCode == Keys.D:
                    ModifyAllThings(dummiesTable, 4);
                    break;
                case true when e.Shift && e.KeyCode == Keys.H:
                    ModifyAllThings(meshTable, 4);
                    break;
                case true when e.Shift && e.KeyCode == Keys.B:
                    SolveAllBBs();
                    break;
                case true when e.Shift && e.KeyCode == Keys.R:
                    ResetAllMesh();
                    break;
                case true when e.Shift && e.KeyCode == Keys.A:
                    SetAllBBsMaxSize();
                    break;
                case true when e.Shift && e.KeyCode == Keys.L:
                    SolveAllMeshLODs();
                    break;
                case true when e.Shift && e.KeyCode == Keys.G:
                    DisplayMaleBody();
                    break;
                case true when e.Shift && e.KeyCode == Keys.F:
                    DisplayFemaleBody();
                    break;
            }
        }

        private void TabWindowDragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void TabWindowDragDrop(object sender, DragEventArgs e)
        {
            string filePath = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            if (filePath.EndsWith(".dae") || filePath.EndsWith(".obj") || filePath.EndsWith(".fbx"))
            {
                ImportFLVERFile(false, filePath);
            }
            else
            {
                arguments.Add(filePath);
                OpenFLVERFile();
            }
        }

        private static bool PromptDeletePreset(object sender, ref Dictionary<object, object> presets)
        {
            ComboBox box = (ComboBox)sender;
            if (box.SelectedItem == null) return false;
            DialogResult result = ShowQuestionDialog("Are you sure you want to delete this preset?");
            if (result != DialogResult.Yes) return false;
            presets.Remove(box.SelectedItem);
            return true;
        }

        private void MaterialPresetsSelector_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (!PromptDeletePreset(sender, ref materialPresets)) return;
            UpdateMaterialPresets();
        }

        private void DummyPresetsSelector_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (!PromptDeletePreset(sender, ref dummyPresets)) return;
            UpdateDummyPresets();
        }

        private static void ExportPresetsFile(ref Dictionary<object, object> presets)
        {
            SaveFileDialog dialog = new SaveFileDialog { Filter = jsonFileFilter };
            if (dialog.ShowDialog() != DialogResult.OK) return;
            File.WriteAllText(dialog.FileName, JsonConvert.SerializeObject(presets, Formatting.Indented));
            ShowInformationDialog("Successfully exported presets file!");
        }

        private void ExportMaterialPresetsFileButtonClick(object sender, EventArgs e)
        {
            ExportPresetsFile(ref materialPresets);
        }

        private void ExportDummiesPresetFileButtonClick(object sender, EventArgs e)
        {
            ExportPresetsFile(ref dummyPresets);
        }

        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            if (Mono3D.f == null || !isSnapped) return;
            try
            {
                if (isSnappedRight)
                {
                    Mono3D.f.Invoke(new MethodInvoker(delegate { Mono3D.f.Left = Right; }));
                    Mono3D.f.Invoke(new MethodInvoker(delegate { Mono3D.f.Top = Top; }));
                }
                else if (isSnappedBottom) Mono3D.f.Invoke(new MethodInvoker(delegate { Mono3D.f.Top = Bottom; }));
            }
            catch { }
        }

        private void HideAllMeshesButton_MouseClick(object sender, MouseEventArgs e)
        {
            ModifyAllThings(meshTable, 4);
        }

        private void PatreonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(patreonSupportUri);
        }

        private void PayPalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(paypalSupportUri);
        }

        private void ToggleDummyIDsVisibilityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            areDummyIdsVisible = !areDummyIdsVisible;
            userConfigJson["AreDummyIDsVisible"] = areDummyIdsVisible;
            WriteUserConfig();
            ShowInformationDialog("The visibility state for Dummy IDs has now been changed!");
        }

        private void DefocusSearchBox(object sender, EventArgs e)
        {
            copyrightStr.Focus();
        }

        private void FilterDataTable(DataGridView dataTable)
        {
            string[] tokens = searchBox.Text.Split(' ');
            foreach (string token in tokens)
            {
                foreach (DataGridViewRow row in dataTable.Rows)
                    row.Visible = row.Cells[1].Value.ToString().ToLower().Contains(token.ToLower());
            }
        }

        private void UpdateSearchResults()
        {
            switch (tabWindow.SelectedIndex)
            {
                case 0:
                    FilterDataTable(bonesTable);
                    break;
                case 1:
                    FilterDataTable(materialsTable);
                    break;
                case 2 when meshTabDataTableSelector.SelectedIndex == 0:
                    FilterDataTable(meshTable);
                    break;
                case 2 when meshTabDataTableSelector.SelectedIndex == 1:
                    FilterDataTable(dummiesTable);
                    break;
            }
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            UpdateSearchResults();
        }

        private void ClearSearchResults()
        {
            searchBox.Text = "";
            UpdateSearchResults();
        }

        private void MeshTabDataTableSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isSettingDefaultInfo) return;
            isSettingDefaultInfo = true;
            int prevSelectorIndex = meshTabDataTableSelector.SelectedIndex;
            meshTabDataTableSelector.SelectedIndex = 0;
            ClearSearchResults();
            meshTabDataTableSelector.SelectedIndex = 1;
            ClearSearchResults();
            meshTabDataTableSelector.SelectedIndex = prevSelectorIndex;
            isSettingDefaultInfo = false;
        }

        private void TabWindow_SelectedIndexChanged(object sender, EventArgs e)
        {
            meshTabDataTableSelector.Visible = tabWindow.SelectedIndex == 2;
            copyrightStr.Focus();
            ClearSearchResults();
        }

        private void MainWindow_SizeChanged(object sender, EventArgs e)
        {
            userConfigJson["EditorWindowWidth"] = Size.Width;
            userConfigJson["EditorWindowHeight"] = Size.Height;
            WriteUserConfig();
        }

        private static void ToggleBodyModelDisplay(ref bool dispBodyModel)
        {
            dispBodyModel = !dispBodyModel;
            UpdateMesh();
            ShowInformationDialog(dispBodyModel ? "Body model is now visible!" : "Body model is now hidden!");
        }

        private static void DisplayMaleBody()
        {
            dispFemaleBody = false;
            ToggleBodyModelDisplay(ref dispMaleBody);
        }

        private void DisplayMaleBodyButton_Click(object sender, EventArgs e)
        {
            DisplayMaleBody();
        }

        private static void DisplayFemaleBody()
        {
            dispMaleBody = false;
            ToggleBodyModelDisplay(ref dispFemaleBody);
        }

        private void DisplayFemaleBodyButton_Click(object sender, EventArgs e)
        {
            DisplayFemaleBody();
        }

        private void MeshTable_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            MeshTableCheckboxSelected(e.RowIndex, e.ColumnIndex);
        }

        private void DummiesTable_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DummiesTableCheckboxSelected(e.RowIndex, e.ColumnIndex);
        }

        private static void AddNewMeshFaceset(FLVER.Mesh m, FLVER.FaceSet.FSFlags flags)
        {
            FLVER.FaceSet fs = Program.generateBasicFaceSet();
            fs.Flags = flags;
            fs.IndexSize = m.FaceSets[0].IndexSize;
            fs.Vertices = (uint[])m.FaceSets[0].Vertices.Clone();
            m.FaceSets.Add(fs);
        }

        private void SolveAllMeshLODs()
        {
            UpdateUndoState();
            foreach (FLVER.Mesh m in flver.Meshes)
            {
                AddNewMeshFaceset(m, FLVER.FaceSet.FSFlags.LodLevel1);
                AddNewMeshFaceset(m, FLVER.FaceSet.FSFlags.LodLevel2);
                AddNewMeshFaceset(m, FLVER.FaceSet.FSFlags.Unk80000000);
                AddNewMeshFaceset(m, FLVER.FaceSet.FSFlags.LodLevel1 | FLVER.FaceSet.FSFlags.Unk80000000);
                AddNewMeshFaceset(m, FLVER.FaceSet.FSFlags.LodLevel2 | FLVER.FaceSet.FSFlags.Unk80000000);
            }
            ShowInformationDialog("Successfully solved all mesh LODs!");
        }

        private void SolveAllMeshLODsButton_Click(object sender, EventArgs e)
        {
            SolveAllMeshLODs();
        }

        private static int GetModelPartIDFromName(string name)
        {
            string idMatch = Regex.Match(Path.GetFileName(name), @"\d+").Value;
            int.TryParse(idMatch, out int id);
            if (id == 0) return -1;
            return id;
        }

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Redo();
        }

        private void UpdateUndoState(bool clearAllRedoActions = true)
        {
            if (clearAllRedoActions)
            {
                redoFlverList.Clear();
                currRedoFlverListIndex = -1;
                redoToolStripMenuItem.Enabled = false;
            }
            undoToolStripMenuItem.Enabled = true;
            undoFlverList.Add(FLVER.Read(flver.Write()));
            currUndoFlverListIndex++;
            if (currUndoFlverListIndex <= undoRedoStackLimit) return;
            undoFlverList.RemoveAt(0);
            currUndoFlverListIndex--;
        }

        private void UpdateRedoState()
        {
            redoToolStripMenuItem.Enabled = true;
            redoFlverList.Add(FLVER.Read(flver.Write()));
            currRedoFlverListIndex++;
        }

        private void Undo()
        {
            if (currUndoFlverListIndex < 0) return;
            UpdateRedoState();
            flver = FLVER.Read(undoFlverList[currUndoFlverListIndex].Write());
            undoFlverList.RemoveAt(currUndoFlverListIndex);
            currUndoFlverListIndex--;
            UpdateUI();
            UpdateMesh();
            if (currUndoFlverListIndex != -1) return;
            undoToolStripMenuItem.Enabled = false;
        }

        private void Redo()
        {
            if (currRedoFlverListIndex < 0) return;
            UpdateUndoState(false);
            flver = FLVER.Read(redoFlverList[currRedoFlverListIndex].Write());
            redoFlverList.RemoveAt(currRedoFlverListIndex);
            currRedoFlverListIndex--;
            UpdateUI();
            UpdateMesh();
            if (currRedoFlverListIndex != -1) return;
            redoToolStripMenuItem.Enabled = false;
        }

        private void ClearUndoRedoStates()
        {
            undoFlverList.Clear();
            redoFlverList.Clear();
            currUndoFlverListIndex = -1;
            currRedoFlverListIndex = -1;
            undoToolStripMenuItem.Enabled = false;
            redoToolStripMenuItem.Enabled = false;
        }

        private void FlipYZAxisCheckboxChanged(object sender, EventArgs e)
        {
            UpdateUndoState();
            foreach (FLVER.Vertex v in selectedMeshIndices.SelectMany(i => flver.Meshes[i].Vertices))
            {
                v.Positions[0] = new System.Numerics.Vector3(v.Positions[0].X, v.Positions[0].Z, v.Positions[0].Y);
                v.Normals[0] = new Vector4(v.Normals[0].X, v.Normals[0].Z, v.Normals[0].Y, -1);
                v.Tangents[0] = new Vector4(v.Tangents[0].X, v.Tangents[0].Z, v.Tangents[0].Y, 1);
            }
            foreach (FLVER.Dummy d in selectedDummyIndices.Select(i => flver.Dummies[i]))
                d.Position = new System.Numerics.Vector3(d.Position.X, d.Position.Z, d.Position.Y);
            UpdateMesh();
            ShowInformationDialog("Successfully flipped the YZ axis!");
        }

        private void CenterMeshToWorld(int nbi)
        {
            UpdateUndoState();
            float[] totals = CalculateMeshTotals();
            foreach (FLVER.Vertex v in selectedMeshIndices.SelectMany(i => flver.Meshes[i].Vertices))
                TranslateThing(v, -totals[nbi], nbi);
            foreach (FLVER.Dummy d in selectedDummyIndices.Select(i => flver.Dummies[i]))
                TranslateThing(d, -totals[nbi], nbi);
            UpdateMesh();
        }

        private void CenterXButtonClicked(object sender, MouseEventArgs e)
        {
            CenterMeshToWorld(0);
        }

        private void CenterYButtonClicked(object sender, MouseEventArgs e)
        {
            CenterMeshToWorld(1);
        }

        private void CenterZButtonClicked(object sender, MouseEventArgs e)
        {
            CenterMeshToWorld(2);
        }

        private void ResetAllMesh()
        {
            UpdateUndoState();
            int layoutCount = flver.BufferLayouts.Count;
            FLVER.BufferLayout newBL = new FLVER.BufferLayout
            {
                new FLVER.BufferLayout.Member(0, 0, FLVER.BufferLayout.MemberType.Float3, FLVER.BufferLayout.MemberSemantic.Position, 0),
                new FLVER.BufferLayout.Member(0, 12, FLVER.BufferLayout.MemberType.Byte4B, FLVER.BufferLayout.MemberSemantic.Normal, 0),
                new FLVER.BufferLayout.Member(0, 16, FLVER.BufferLayout.MemberType.Byte4B, FLVER.BufferLayout.MemberSemantic.Tangent, 0),
                new FLVER.BufferLayout.Member(0, 20, FLVER.BufferLayout.MemberType.Byte4B, FLVER.BufferLayout.MemberSemantic.Tangent, 1),
                new FLVER.BufferLayout.Member(0, 24, FLVER.BufferLayout.MemberType.Byte4B, FLVER.BufferLayout.MemberSemantic.BoneIndices, 0),
                new FLVER.BufferLayout.Member(0, 28, FLVER.BufferLayout.MemberType.Byte4C, FLVER.BufferLayout.MemberSemantic.BoneWeights, 0),
                new FLVER.BufferLayout.Member(0, 32, FLVER.BufferLayout.MemberType.Byte4C, FLVER.BufferLayout.MemberSemantic.VertexColor, 1),
                new FLVER.BufferLayout.Member(0, 36, FLVER.BufferLayout.MemberType.UVPair, FLVER.BufferLayout.MemberSemantic.UV, 0)
            };
            flver.BufferLayouts.Add(newBL);
            foreach (FLVER.Mesh m in flver.Meshes)
            {
                m.BoundingBoxMax = new System.Numerics.Vector3(1, 1, 1);
                m.BoundingBoxMin = new System.Numerics.Vector3(-1, -1, -1);
                m.BoundingBoxUnk = new System.Numerics.Vector3();
                m.Unk1 = 0;
                m.DefaultBoneIndex = 0;
                m.Dynamic = true;
                m.VertexBuffers = new List<FLVER.VertexBuffer> { new FLVER.VertexBuffer(0, layoutCount, -1) };
                uint[] varray = m.FaceSets[0].Vertices;
                m.FaceSets = new List<FLVER.FaceSet>();
                for (int i = 0; i < m.Vertices.Count; i++)
                {
                    FLVER.Vertex vit = m.Vertices[i];
                    m.Vertices[i] = Program.generateVertexV4(new System.Numerics.Vector3(vit.Positions[0].X, vit.Positions[0].Y, vit.Positions[0].Z), vit.UVs[0], vit.UVs[0],
                        vit.Normals[0], vit.Tangents[0], 1);
                    m.Vertices[i].BoneIndices = vit.BoneIndices;
                    m.Vertices[i].BoneWeights = vit.BoneWeights;
                }
                m.FaceSets.Add(Program.generateBasicFaceSet());
                m.FaceSets[0].Vertices = varray;
                m.FaceSets[0].CullBackfaces = false;
                if (m.FaceSets[0].Vertices.Length <= 65534) continue;
                m.FaceSets[0].IndexSize = 32;
            }
            ShowInformationDialog("Successfully reset all mesh!");
        }

        private void ResetAllMeshButton_Click(object sender, EventArgs e)
        {
            ResetAllMesh();
        }

        private enum TextureFormats
        {
            DXT1 = 0,
            BC7_UNORM = 107,
            ATI1 = 103,
            ATI2 = 104,
            ATI3 = 103
        }

        public class MATBIN
        {
            public enum ParamType : uint
            {
                Bool = 0,
                Int = 4,
                Int2 = 5,
                Float = 8,
                Float2 = 9,
                Float3 = 10,
                Float4 = 11,
                Float5 = 12
            }

            public MATBIN()
            {
                ShaderPath = "";
                SourcePath = "";
                Params = new List<Param>();
                Samplers = new List<Sampler>();
            }

            public string ShaderPath { get; set; }

            public string SourcePath { get; set; }

            public uint Key { get; set; }

            public List<Param> Params { get; set; }

            public List<Sampler> Samplers { get; set; }

            public void Read(BinaryReaderEx br)
            {
                br.BigEndian = false;
                br.AssertASCII("MAB\0");
                br.AssertInt32(2);
                ShaderPath = br.GetUTF16(br.ReadInt64());
                SourcePath = br.GetUTF16(br.ReadInt64());
                Key = br.ReadUInt32();
                int paramCount = br.ReadInt32();
                int samplerCount = br.ReadInt32();
                br.ReadBytes(0x14);
                Params = new List<Param>(paramCount);
                for (int i = 0; i < paramCount; i++)
                    Params.Add(new Param(br));
                Samplers = new List<Sampler>(samplerCount);
                for (int i = 0; i < samplerCount; i++)
                    Samplers.Add(new Sampler(br));
            }

            [XmlInclude(typeof(int[]))]
            [XmlInclude(typeof(float[]))]
            public class Param
            {
                internal Param(BinaryReaderEx br)
                {
                    Name = br.GetUTF16(br.ReadInt64());
                    long valueOffset = br.ReadInt64();
                    Key = br.ReadUInt32();
                    Type = br.ReadEnum32<ParamType>();
                    br.ReadBytes(0x10);
                    br.StepIn(valueOffset);
                    {
                        switch (Type)
                        {
                            case ParamType.Bool:
                                Value = br.ReadBoolean();
                                break;
                            case ParamType.Int:
                                Value = br.ReadInt32();
                                break;
                            case ParamType.Int2:
                                Value = br.ReadInt32s(2);
                                break;
                            case ParamType.Float:
                                Value = br.ReadSingle();
                                break;
                            case ParamType.Float2:
                                Value = br.ReadSingles(2);
                                break;
                            case ParamType.Float3:
                                Value = br.ReadSingles(3);
                                break;
                            case ParamType.Float4:
                                Value = br.ReadSingles(4);
                                break;
                            case ParamType.Float5:
                                Value = br.ReadSingles(5);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    br.StepOut();
                }

                public string Name { get; set; }

                public object Value { get; set; }

                public uint Key { get; set; }

                public ParamType Type { get; set; }
            }

            public class Sampler
            {
                internal Sampler(BinaryReaderEx br)
                {
                    Type = br.GetUTF16(br.ReadInt64());
                    Path = br.GetUTF16(br.ReadInt64());
                    Key = br.ReadUInt32();
                    Unk14 = br.ReadVector2();
                    br.ReadBytes(0x14);
                }

                public string Type { get; set; }

                public string Path { get; set; }

                public uint Key { get; set; }

                public Vector2 Unk14 { get; set; }
            }
        }

        private void ToggleUseWorldOriginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            useWorldOrigin = !useWorldOrigin;
            userConfigJson["UseWorldOrigin"] = useWorldOrigin;
            WriteUserConfig();
            ShowInformationDialog("Successfully toggled using the world origin for transformations!");
        }
    }
}