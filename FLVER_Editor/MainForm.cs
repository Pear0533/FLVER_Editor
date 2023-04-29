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
using Assimp;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SoulsFormats;
using PrimitiveType = Assimp.PrimitiveType;
using Vector2 = Microsoft.Xna.Framework.Vector2;

// TODO: Allow for hierarchal models to be opened/saved

namespace FLVER_Editor
{
    public partial class MainWindow : Form
    {
        /// <summary>
        ///     Index of the edit button column in the Materials table.
        /// </summary>
        private const int MaterialEditButtonIndex = 6;

        /// <summary>
        ///     Index of the viewer highlight button column in the Materials table.
        /// </summary>
        private const int MaterialViewerHighlightButtonIndex = 7;

        /// <summary>
        ///     Index of the add material preset button column in the Materials table.
        /// </summary>
        private const int MaterialAddPresetCbIndex = 8;

        /// <summary>
        ///     Index of the apply material preset button column in the Materials table.
        /// </summary>
        private const int MaterialApplyPresetCbIndex = 9;

        /// <summary>
        ///     Index of the delete material button column in the Materials table.
        /// </summary>
        private const int MaterialDeleteCbIndex = 10;

        /// <summary>
        ///     The file filter for opening image files.
        /// </summary>
        private const string ImageFilesFilter = "DDS File (*.dds)|*.dds";

        /// <summary>
        ///     The file filter for opening JSON files.
        /// </summary>
        private const string JsonFileFilter = @"JSON File (*.json)|*.json";

        /// <summary>
        ///     The current version of the program.
        /// </summary>
        private const string Version = "1.91";

        /// <summary>
        ///     The patreon support link for Pear.
        /// </summary>
        private const string PatreonSupportUri = "https://www.patreon.com/theonlypear";

        /// <summary>
        ///     The paypal support link for Pear.
        /// </summary>
        private const string PaypalSupportUri = "https://paypal.me/realcucumberlettuce3";

        /// <summary>
        ///     The base material dict key used when loading presets from the material presets file.
        /// </summary>
        private const string BaseMaterialDictKey = "Base Material";

        /// <summary>
        ///     A list of file paths to models so the FLVER open method can be used with drag drop.
        /// </summary>
        public static List<string> Arguments;

        /// <summary>
        ///     The currently loaded FLVER2 model.
        /// </summary>
        public static FLVER2 Flver;

        /// <summary>
        ///     A list of undos for FLVER2 edits.
        /// </summary>
        public static List<FLVER2> UndoFlverList = new List<FLVER2>();

        /// <summary>
        ///     A list of redos for FLVER2 edits.
        /// </summary>
        public static List<FLVER2> RedoFlverList = new List<FLVER2>();

        /// <summary>
        ///     The current FLVER2 undo list index, used to see what to undo.
        /// </summary>
        public static int CurrentUndoFlverListIndex = -1;

        /// <summary>
        ///     The current FLVER2 redo list index, used to see what to redo.
        /// </summary>
        public static int CurrentRedoFlverListIndex = -1;

        /// <summary>
        ///     The limit of how far back and ahead undos and redos go.
        /// </summary>
        public static int UndoRedoStackLimit = 25;

        /// <summary>
        ///     A default male body FLVER2 model.
        /// </summary>
        public static FLVER2 MaleBodyFlver = new FLVER2();

        /// <summary>
        ///     A default female body FLVER2 model.
        /// </summary>
        public static FLVER2 FemaleBodyFlver = new FLVER2();

        /// <summary>
        ///     The byte data of the currently loaded FLVER2 model, used to see if changes were made and saving is needed.
        /// </summary>
        private static byte[] CurrentFlverBytes;

        /// <summary>
        ///     The current BND4 the loaded FLVER2 model file is in if applicable.
        /// </summary>
        private static BND4 FlverBnd;

        /// <summary>
        ///     The current BND4 the loaded MatBins are in if applicable.
        /// </summary>
        private static BND4 MatBinBnd;

        /// <summary>
        ///     The path to the first loaded FLVER2 file in Arguments.
        /// </summary>
        private static string FlverFilePath;

        /// <summary>
        ///     The path to the loaded MatBins' BND4 if applicable.
        /// </summary>
        private static string MatBinBndPath;

        /// <summary>
        ///     The compression type of the archive the loaded FLVER2 is in if applicable.
        /// </summary>
        private static DCX.Type FlverCompressionType;

        /// <summary>
        ///     The BinderFile index of the current FLVER2 file if it is inside of a Binder.
        /// </summary>
        private static int CurrentFlverFileBinderIndex = -1;

        /// <summary>
        ///     Determines how thick the dummy point markers appear in the viewer window.
        /// </summary>
        private static int DummyThickness = 5;

        /// <summary>
        ///     The viewer window showing the loaded model.
        /// </summary>
        private static Mono3D Viewer;

        /// <summary>
        ///     The Tab Window's current background color.
        /// </summary>
        private static Color TabWindowBackColor = DefaultBackColor;

        /// <summary>
        ///     The Tab Window's current foreground color.
        /// </summary>
        private static Color TabWindowForeColor = DefaultForeColor;

        /// <summary>
        ///     The indices of the currently selected meshes.
        /// </summary>
        private static List<int> SelectedMeshIndices = new List<int>();

        /// <summary>
        ///     The indices of the currently selected dummies.
        /// </summary>
        private static List<int> SelectedDummyIndices = new List<int>();

        /// <summary>
        ///     The indices of currently hidden meshes.
        /// </summary>
        private static List<int> HiddenMeshIndices = new List<int>();

        /// <summary>
        ///     The indices of the meshes of the currently selected materials.
        /// </summary>
        private static readonly List<int> SelectedMaterialMeshIndices = new List<int>();

        /// <summary>
        ///     The 3D positions of all bones contained within the FLVER model.
        /// </summary>
        private static readonly List<Vector3D> BonePositionList = new List<Vector3D>();

        /// <summary>
        ///     The currently loaded material presets.
        /// </summary>
        private static Dictionary<object, object> MaterialPresets;

        /// <summary>
        ///     The currently loaded dummy presets.
        /// </summary>
        private static Dictionary<object, object> DummyPresets;

        /// <summary>
        ///     Determines the order of the axes used for performing 3D rotations.
        /// </summary>
        public static RotationOrder RotationOrder = RotationOrder.YZX;

        /// <summary>
        ///     The root folder path where the exe of this program is running.
        /// </summary>
        public static readonly string RootFolderPath = Environment.CurrentDirectory;

        /// <summary>
        ///     The Resources folder path.
        /// </summary>
        public static readonly string ResourceFolderPath = $"{RootFolderPath}\\Resources";

        /// <summary>
        ///     The model folder path in Resources containing obj models that are imported.
        /// </summary>
        public static readonly string ModelResourcePath = $"{ResourceFolderPath}\\model";

        /// <summary>
        ///     The bone folder path in Resources containing bone name conversion mappings.
        /// </summary>
        public static readonly string BoneResourcePath = $"{ResourceFolderPath}\\bone";

        /// <summary>
        ///     The material folder path in Resources containing material presets and materials later.
        /// </summary>
        public static readonly string MaterialResourcePath = $"{ResourceFolderPath}\\material";

        /// <summary>
        ///     The dummy folder path in Resources containing dummy presets.
        /// </summary>
        public static readonly string DummyResourcePath = $"{ResourceFolderPath}\\dummy";

        /// <summary>
        ///     The image folder path in Resources containing images used by the program.
        /// </summary>
        public static readonly string ImageResourcePath = $"{ResourceFolderPath}\\image";

        /// <summary>
        ///     The material presets file path in Resources.
        /// </summary>
        private static readonly string MaterialPresetsFilePath = $"{MaterialResourcePath}\\materialpresets.json";

        /// <summary>
        ///     The dummy presets file path in Resources.
        /// </summary>
        private static readonly string DummyPresetsFilePath = $"{DummyResourcePath}\\dummypresets.json";

        /// <summary>
        ///     The user config file path in the root program directory.
        /// </summary>
        private static readonly string UserConfigJsonPath = $"{RootFolderPath}\\userconfig.json";

        /// <summary>
        ///     The user config.
        /// </summary>
        public static JObject UserConfigJson = new JObject();

        /// <summary>
        ///     The current materials table splitter distance, used to save splitter distance in the user config.
        /// </summary>
        private static int CurrentMaterialsTableSplitDistance;

        /// <summary>
        ///     The current interval at which to auto save user progress.
        /// </summary>
        private static string CurrentAutoSaveInterval;

        /// <summary>
        ///     Whether or not a mesh is selected currently.
        /// </summary>
        private static bool MeshIsSelected;

        /// <summary>
        ///     Whether or not a dummy is selected currently.
        /// </summary>
        private static bool DummyIsSelected;

        /// <summary>
        ///     Whether or not meshes are hidden.
        /// </summary>
        private static bool MeshIsHidden;

        /// <summary>
        ///     A condition used to block certain methods from being used when necessary.
        /// </summary>
        private static bool IsSettingDefaultInfo = true;

        /// <summary>
        ///     Whether or not to refresh textures, a user config setting to improve performance for users if disabled.
        /// </summary>
        public static bool TextureRefreshEnabled = true;

        /// <summary>
        ///     The selected material index.
        /// </summary>
        private static int SelectedMaterialIndex = -1;

        /// <summary>
        /// Holds the previously stored value of the most recently changed Modifiers panel transformation number box.
        /// </summary>
        private static float PrevNumVal;

        /// <summary>
        /// Determines if the viewer window is currently snapped to the main window.
        /// </summary>
        public static bool IsSnapped;

        /// <summary>
        /// Determines if the viewer window is only snapped to the right edge of the main window.
        /// </summary>
        public static bool IsSnappedRight = false;

        /// <summary>
        /// Determines if the viewer window is only snapped to the bottom edge of the main window.
        /// </summary>
        public static bool IsSnappedBottom = false;

        /// <summary>
        /// Determines if the viewer window is only snapped to the left edge of the main window.
        /// </summary>
        public static bool IsSnappedLeft = false;

        /// <summary>
        /// Determines if the viewer window is only snapped to the top edge of the main window.
        /// </summary>
        public static bool IsSnappedTop = false;

        /// <summary>
        /// Determines if reference IDs are displayed in the viewer for all dummy points.
        /// </summary>
        public static bool AreDummyIdsVisible = true;

        /// <summary>
        /// Determines if the default male body reference model is displayed in the viewer.
        /// </summary>
        private static bool DisplayMaleBody;

        /// <summary>
        /// Determines if the default female body reference model is displayed in the viewer.
        /// </summary>
        private static bool DisplayFemaleBody;

        /// <summary>
        /// Determines if the internal indices for all materials should not be automatically corrected.
        /// </summary>
        private static bool StopAutoInternalIndexOverride;

        /// <summary>
        /// Determines if the world origin point should be used for all mesh and dummy transformations.
        /// </summary>
        private static bool UseWorldOrigin;

        /// <summary>
        /// Changes whether newly imported materials are duplicated from pre-existing ones or not.
        /// </summary>
        public static bool ToggleDuplicateMaterialsOnMeshImport;

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
            SetDupeMatOnMeshImport();
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

        private static void ApplyBodyModelMaterial(FLVER2 bodyFlver)
        {
            bodyFlver.Materials.Add(Generators.GenerateBaseMaterial());
            bodyFlver.Meshes[0].MaterialIndex = 0;
        }

        private static void ImportBodyModels()
        {
            Importer.ImportAssimp(MaleBodyFlver, $"{ModelResourcePath}\\malebody.obj");
            Importer.ImportAssimp(FemaleBodyFlver, $"{ModelResourcePath}\\femalebody.obj");
            ApplyBodyModelMaterial(MaleBodyFlver);
            ApplyBodyModelMaterial(FemaleBodyFlver);
        }

        private void SetVersionString()
        {
            versionStr.Text += $@" {Version}";
        }

        private static void SetTextureRefreshEnabled()
        {
            string textureRefreshStr = UserConfigJson["TextureRefreshing"]?.ToString();
            if (textureRefreshStr == null) return;
            TextureRefreshEnabled = bool.Parse(textureRefreshStr);
        }

        private void SetDefaultScreenPosition()
        {
            CenterToScreen();
            if (UserConfigJson["ViewerSnapPosition"]?.ToString() == "Right") Left = 0;
            else Top = 0;
            TopMost = true;
        }

        public static void ReadUserConfig()
        {
            if (File.Exists(UserConfigJsonPath)) UserConfigJson = JObject.Parse(File.ReadAllText(UserConfigJsonPath));
        }

        public static void WriteUserConfig()
        {
            File.WriteAllText(UserConfigJsonPath, JsonConvert.SerializeObject(UserConfigJson, Formatting.Indented));
        }

        private static void SetUseWorldOrigin()
        {
            string useWorldOriginStr = UserConfigJson["UseWorldOrigin"]?.ToString();
            if (useWorldOriginStr == null) return;
            UseWorldOrigin = bool.Parse(useWorldOriginStr);
        }

        private static void SetDupeMatOnMeshImport()
        {
            string dupeMatOnMeshImportStr = UserConfigJson["DupeMatOnMeshImport"]?.ToString();
            if (dupeMatOnMeshImportStr == null) return;
            ToggleDuplicateMaterialsOnMeshImport = bool.Parse(dupeMatOnMeshImportStr);
        }

        private void SetAutoSaveEnabled()
        {
            string autoSaveEnabledStr = UserConfigJson["IsAutoSaveEnabled"]?.ToString();
            if (autoSaveEnabledStr == null) return;
            autoSaveTimer.Enabled = !bool.Parse(autoSaveEnabledStr);
            ToggleAutoSaveState(false, false);
        }

        private static void SetDummyIDsVisibility()
        {
            string dummyIdsVisibilityStr = UserConfigJson["AreDummyIDsVisible"]?.ToString();
            if (dummyIdsVisibilityStr == null) return;
            AreDummyIdsVisible = bool.Parse(dummyIdsVisibilityStr);
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
            UserConfigJson["IsAutoSaveEnabled"] = autoSaveTimer.Enabled;
            if (writeToConfig) UserConfigJson["IsAutoSaveEnabled"] = autoSaveTimer.Enabled;
            WriteUserConfig();
        }

        private void SetAutoSaveInterval()
        {
            try
            {
                CheckAutoSaveInterval(UserConfigJson["AutoSaveInterval"]?.ToString());
            }
            catch { }
            autoSaveIntervalSelector.Text = (autoSaveTimer.Interval / 60000).ToString();
        }

        private void SetDummyThickness()
        {
            string dummyThicknessStr = UserConfigJson["DummyThickness"]?.ToString();
            if (dummyThicknessStr != null)
            {
                DummyThickness = int.Parse(dummyThicknessStr);
                if (DummyThickness > dummyThicknessSelector.Items.Count) DummyThickness = 5;
            }
            dummyThicknessSelector.SelectedIndex = DummyThickness - 1;
        }

        private void SetEditorWindowSize()
        {
            string editorWindowWidthStr = UserConfigJson["EditorWindowWidth"]?.ToString();
            string editorWindowHeightStr = UserConfigJson["EditorWindowHeight"]?.ToString();
            if (editorWindowWidthStr == null || editorWindowHeightStr == null) return;
            Size = new Size(int.Parse(editorWindowWidthStr), int.Parse(editorWindowHeightStr));
        }

        private void SetMaterialsTableView()
        {
            string materialsTableViewSplitDistanceStr = UserConfigJson["MaterialsTableViewSplitDistance"]?.ToString();
            if (materialsTableViewSplitDistanceStr == null) return;
            materialsPagePanelsContainer.SplitterDistance = int.Parse(materialsTableViewSplitDistanceStr);
        }

        private void ChangeTheme(Control control, Color backColor, Color foreColor)
        {
            TabWindowBackColor = backColor;
            TabWindowForeColor = foreColor;
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

        public static void UpdateMesh()
        {
            if (DisplayMaleBody) Flver.Meshes.Add(MaleBodyFlver.Meshes[0]);
            else if (DisplayFemaleBody) Flver.Meshes.Add(FemaleBodyFlver.Meshes[0]);
            List<VertexPositionColor> vertexPosColorList = new List<VertexPositionColor>();
            List<VertexPositionColor> faceSetPosColorList = new List<VertexPositionColor>();
            List<VertexPositionColorTexture> faceSetPosColorTexList = new List<VertexPositionColorTexture>();
            List<VertexTexMap> vertexTexMapList = new List<VertexTexMap>();
            for (int i = 0; i < Flver.Meshes.Count; ++i)
            {
                if (Flver.Meshes[i] == null) continue;
                bool renderBackFaces = Flver.Meshes[i].FaceSets.Count > 0 && !Flver.Meshes[i].FaceSets[0].CullBackfaces;
                foreach (FLVER.Vertex[] vertexArr in Flver.Meshes[i].GetFaces())
                {
                    if (HiddenMeshIndices.IndexOf(i) != -1) continue;
                    Microsoft.Xna.Framework.Color colorLine = Microsoft.Xna.Framework.Color.Black;
                    if (MeshIsSelected && SelectedMeshIndices.IndexOf(i) != -1)
                    {
                        colorLine.R = colorLine.G = 255;
                    }
                    else if (SelectedMaterialMeshIndices.IndexOf(i) != -1)
                    {
                        colorLine.G = 0;
                        colorLine.R = colorLine.B = 255;
                    }
                    colorLine.A = 125;
                    vertexPosColorList.AddRange(new[]
                    {
                        new VertexPositionColor(Util3D.NumericsVector3ToXnaVector3XZY(vertexArr[0].Position), colorLine),
                        new VertexPositionColor(Util3D.NumericsVector3ToXnaVector3XZY(vertexArr[1].Position), colorLine),
                        new VertexPositionColor(Util3D.NumericsVector3ToXnaVector3XZY(vertexArr[0].Position), colorLine),
                        new VertexPositionColor(Util3D.NumericsVector3ToXnaVector3XZY(vertexArr[2].Position), colorLine),
                        new VertexPositionColor(Util3D.NumericsVector3ToXnaVector3XZY(vertexArr[1].Position), colorLine),
                        new VertexPositionColor(Util3D.NumericsVector3ToXnaVector3XZY(vertexArr[2].Position), colorLine)
                    });
                    Microsoft.Xna.Framework.Color faceSetColor = new Microsoft.Xna.Framework.Color();
                    Microsoft.Xna.Framework.Vector3 vectorA = Util3D.NumericsVector3ToXnaVector3(vertexArr[1].Position)
                        - Util3D.NumericsVector3ToXnaVector3(vertexArr[0].Position);
                    Microsoft.Xna.Framework.Vector3 vectorB = Util3D.NumericsVector3ToXnaVector3(vertexArr[2].Position)
                        - Util3D.NumericsVector3ToXnaVector3(vertexArr[0].Position);
                    Microsoft.Xna.Framework.Vector3 normalVector = Program.XnaCrossProduct(vectorA, vectorB);
                    normalVector.Normalize();
                    Microsoft.Xna.Framework.Vector3 lightVector = new Microsoft.Xna.Framework.Vector3(Viewer.lightX, Viewer.lightY, Viewer.lightZ);
                    lightVector.Normalize();
                    int faceSetColorVal = 125 + (int)(125 * Program.XnaDotProduct(normalVector, lightVector));
                    faceSetColorVal = faceSetColorVal > 255 ? 255 : faceSetColorVal < 0 ? 0 : Viewer.flatShading ? 255 : faceSetColorVal;
                    faceSetColor.R = faceSetColor.G = faceSetColor.B = (byte)faceSetColorVal;
                    faceSetColor.A = 255;
                    if (MeshIsSelected && SelectedMeshIndices.IndexOf(i) != -1)
                    {
                        faceSetColor.B = 0;
                    }
                    else if (SelectedMaterialMeshIndices.IndexOf(i) != -1)
                    {
                        faceSetColor.B = 255;
                        faceSetColor.G = 0;
                    }
                    faceSetPosColorList.AddRange(
                        new[]
                        {
                            new VertexPositionColor(Util3D.NumericsVector3ToXnaVector3XZY(vertexArr[0].Position), faceSetColor),
                            new VertexPositionColor(Util3D.NumericsVector3ToXnaVector3XZY(vertexArr[2].Position), faceSetColor),
                            new VertexPositionColor(Util3D.NumericsVector3ToXnaVector3XZY(vertexArr[1].Position), faceSetColor),
                            new VertexPositionColor(Util3D.NumericsVector3ToXnaVector3XZY(vertexArr[0].Position), faceSetColor),
                            new VertexPositionColor(Util3D.NumericsVector3ToXnaVector3XZY(vertexArr[2].Position), faceSetColor),
                            new VertexPositionColor(Util3D.NumericsVector3ToXnaVector3XZY(vertexArr[1].Position), faceSetColor)
                        });
                    faceSetPosColorTexList.AddRange(
                        new[]
                        {
                            new VertexPositionColorTexture(Util3D.NumericsVector3ToXnaVector3XZY(vertexArr[0].Position), faceSetColor,
                                new Vector2(vertexArr[0].UVs[0].X, vertexArr[0].UVs[0].Y)),
                            new VertexPositionColorTexture(Util3D.NumericsVector3ToXnaVector3XZY(vertexArr[2].Position), faceSetColor,
                                new Vector2(vertexArr[2].UVs[0].X, vertexArr[2].UVs[0].Y)),
                            new VertexPositionColorTexture(Util3D.NumericsVector3ToXnaVector3XZY(vertexArr[1].Position), faceSetColor,
                                new Vector2(vertexArr[1].UVs[0].X, vertexArr[1].UVs[0].Y))
                        });
                    if (!renderBackFaces) continue;
                    faceSetPosColorList.AddRange(
                        new[]
                        {
                            new VertexPositionColor(Util3D.NumericsVector3ToXnaVector3XZY(vertexArr[0].Position), faceSetColor),
                            new VertexPositionColor(Util3D.NumericsVector3ToXnaVector3XZY(vertexArr[1].Position), faceSetColor),
                            new VertexPositionColor(Util3D.NumericsVector3ToXnaVector3XZY(vertexArr[2].Position), faceSetColor)
                        });
                    faceSetPosColorTexList.AddRange(
                        new[]
                        {
                            new VertexPositionColorTexture(Util3D.NumericsVector3ToXnaVector3XZY(vertexArr[0].Position), faceSetColor,
                                new Vector2(vertexArr[0].UVs[0].X, vertexArr[0].UVs[0].Y)),
                            new VertexPositionColorTexture(Util3D.NumericsVector3ToXnaVector3XZY(vertexArr[1].Position), faceSetColor,
                                new Vector2(vertexArr[1].UVs[0].X, vertexArr[1].UVs[0].Y)),
                            new VertexPositionColorTexture(Util3D.NumericsVector3ToXnaVector3XZY(vertexArr[2].Position), faceSetColor,
                                new Vector2(vertexArr[2].UVs[0].X, vertexArr[2].UVs[0].Y))
                        });
                }
                for (int j = 0; j < Flver.Meshes[i].Vertices.Count; ++j)
                {
                    Program.Vertices.Add(Flver.Meshes[i].Vertices[j]);
                    Program.VerticesInfo.Add(new VertexInfo { MeshIndex = i, VertexIndex = j });
                }
                FLVER2.Material currMaterial = Flver.Materials.ElementAtOrDefault(Flver.Meshes[i].MaterialIndex);
                if (currMaterial == null) continue;
                List<FLVER2.Texture> texList = currMaterial.Textures;
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
            for (int i = 0; i < BonePositionList.Count; ++i)
                BonePositionList[i] = null;
            // TODO: Investigate the maximum threshold on bones
            Transform3D[] bonesTransform = new Transform3D[Flver.Bones.Count];
            for (int i = 0; i < Flver.Bones.Count; ++i)
            {
                bonesTransform[i] = new Transform3D { RotationOrder = RotationOrder, Position = new Vector3D(Flver.Bones[i].Translation) };
                bonesTransform[i].SetRotationInRadians(new Vector3D(Flver.Bones[i].Rotation));
                bonesTransform[i].Scale = new Vector3D(Flver.Bones[i].Scale);
                if (Flver.Bones[i].ParentIndex < 0) continue;
                bonesTransform[i].Parent = bonesTransform[Flver.Bones[i].ParentIndex];
                Vector3D absolutePos = bonesTransform[i].GetGlobalOrigin();
                if (bonesTransform[Flver.Bones[i].ParentIndex] == null) continue;
                Vector3D parentPos = bonesTransform[Flver.Bones[i].ParentIndex].GetGlobalOrigin();
                vertexPosColorList.Add(new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(parentPos.X - 0.005f, parentPos.Z - 0.005f, parentPos.Y),
                    Microsoft.Xna.Framework.Color.Purple));
                vertexPosColorList.Add(new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(absolutePos.X, absolutePos.Z, absolutePos.Y),
                    Microsoft.Xna.Framework.Color.Purple));
                vertexPosColorList.Add(new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(parentPos.X + 0.005f, parentPos.Z + 0.005f, parentPos.Y),
                    Microsoft.Xna.Framework.Color.Purple));
                vertexPosColorList.Add(new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(absolutePos.X, absolutePos.Z, absolutePos.Y),
                    Microsoft.Xna.Framework.Color.Purple));
            }
            for (int i = 0; i < Flver.Dummies.Count; ++i)
            {
                FLVER.Dummy dummy = Flver.Dummies[i];
                bool shouldSelectDummy = DummyIsSelected && SelectedDummyIndices.IndexOf(i) != -1;
                Microsoft.Xna.Framework.Color dummyColor = shouldSelectDummy ? Microsoft.Xna.Framework.Color.Yellow : Microsoft.Xna.Framework.Color.Purple;
                float baseDummyYPos = dummy.Position.Y;
                const float posStep = 0.0005f;
                for (int j = 0; j < DummyThickness; ++j)
                {
                    vertexPosColorList.AddRange(new[]
                    {
                        new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(dummy.Position.X - 0.025f, dummy.Position.Z, baseDummyYPos), dummyColor),
                        new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(dummy.Position.X + 0.025f, dummy.Position.Z, baseDummyYPos), dummyColor),
                        new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(dummy.Position.X, dummy.Position.Z - 0.025f, baseDummyYPos), dummyColor),
                        new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(dummy.Position.X, dummy.Position.Z + 0.025f, baseDummyYPos), dummyColor),
                        new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(dummy.Position.X, dummy.Position.Z, baseDummyYPos), Microsoft.Xna.Framework.Color.Green),
                        new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(dummy.Position.X + dummy.Forward.X, dummy.Position.Z + dummy.Forward.Z,
                                baseDummyYPos + dummy.Forward.Y),
                            Microsoft.Xna.Framework.Color.Green)
                    });
                    baseDummyYPos -= posStep;
                }
            }
            if (Program.UseCheckingPoint)
            {
                Vector3 checkingPoint = Program.CheckingPoint;
                Vector3 checkingPointNormal = Program.CheckingPointNormal;
                vertexPosColorList.Add(new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(checkingPoint.X - 0.05f, checkingPoint.Z - 0.05f, checkingPoint.Y),
                    Microsoft.Xna.Framework.Color.AntiqueWhite));
                vertexPosColorList.Add(new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(checkingPoint.X + 0.05f, checkingPoint.Z + 0.05f, checkingPoint.Y),
                    Microsoft.Xna.Framework.Color.AntiqueWhite));
                vertexPosColorList.Add(new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(checkingPoint.X - 0.05f, checkingPoint.Z + 0.05f, checkingPoint.Y),
                    Microsoft.Xna.Framework.Color.AntiqueWhite));
                vertexPosColorList.Add(new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(checkingPoint.X + 0.05f, checkingPoint.Z - 0.05f, checkingPoint.Y),
                    Microsoft.Xna.Framework.Color.AntiqueWhite));
                vertexPosColorList.Add(new VertexPositionColor(new Microsoft.Xna.Framework.Vector3(checkingPoint.X, checkingPoint.Z, checkingPoint.Y),
                    Microsoft.Xna.Framework.Color.Blue));
                vertexPosColorList.Add(new VertexPositionColor(
                    new Microsoft.Xna.Framework.Vector3(checkingPoint.X + 0.2f * checkingPointNormal.X, checkingPoint.Z + 0.2f * checkingPointNormal.Z,
                        checkingPoint.Y + 0.2f * checkingPointNormal.Y),
                    Microsoft.Xna.Framework.Color.Blue));
            }
            Viewer.vertices = vertexPosColorList.ToArray();
            Viewer.vertexTexMapList = vertexTexMapList.ToArray();
            Viewer.faceSets = faceSetPosColorList.ToArray();
            if (DisplayMaleBody) Flver.Meshes.Remove(MaleBodyFlver.Meshes[0]);
            else if (DisplayFemaleBody) Flver.Meshes.Remove(FemaleBodyFlver.Meshes[0]);
        }

        private static void ClearViewerMaterialHighlight()
        {
            SelectedMaterialMeshIndices.Clear();
        }

        private void DeselectAllSelectedThings()
        {
            IsSettingDefaultInfo = true;
            MeshIsSelected = false;
            DummyIsSelected = false;
            MeshIsHidden = false;
            SelectedMeshIndices.Clear();
            SelectedDummyIndices.Clear();
            HiddenMeshIndices.Clear();
            meshModifiersContainer.Enabled = false;
            IsSettingDefaultInfo = false;
        }

        private static void LoadViewer()
        {
            if (Viewer == null)
            {
                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    Viewer = new Mono3D();
                    UpdateMesh();
                    Viewer.RefreshTextures();
                    Viewer.Run();
                }).Start();
                return;
            }
            UpdateMesh();
            Viewer.RefreshTextures();
        }

        private static bool LoadPresets(ComboBox selector, ref Dictionary<object, object> dict, string filePath)
        {
            selector.Items.Clear();
            bool hasRead = true;
            try
            {
                dict = new JavaScriptSerializer().Deserialize<Dictionary<object, object>>(File.ReadAllText(filePath));
                if (dict == MaterialPresets && !dict.ContainsKey(BaseMaterialDictKey)) dict.Add(BaseMaterialDictKey, Generators.GenerateBaseMaterial());
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
            bool hasRead = LoadPresets(materialPresetsSelector, ref MaterialPresets, MaterialPresetsFilePath);
            materialsTable.Columns[MaterialAddPresetCbIndex].Visible = hasRead;
        }

        private void LoadDummyPresets()
        {
            dummyPresetsSelector.Enabled = dummiesTableOKButton.Enabled =
                addAllDummiesToPresetsButton.Enabled = LoadPresets(dummyPresetsSelector, ref DummyPresets, DummyPresetsFilePath);
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
            IsSettingDefaultInfo = true;
            applyMatBinTexturesButton.Enabled = false;
            bonesTable.Rows.Clear();
            materialsTable.Rows.Clear();
            texturesTable.Rows.Clear();
            meshTable.Rows.Clear();
            dummiesTable.Rows.Clear();
            for (int i = 0; i < Flver.Bones.Count; ++i)
            {
                FLVER.Bone bone = Flver.Bones[i];
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
            for (int i = 0; i < Flver.Materials.Count; ++i)
            {
                FLVER2.Material material = Flver.Materials[i];
                DataGridViewRow row = new DataGridViewRow();
                if (!StopAutoInternalIndexOverride) material.Unk18 = i;
                row.Cells.AddRange(new DataGridViewTextBoxCell { Value = i }, new DataGridViewTextBoxCell { Value = material.Name },
                    new DataGridViewTextBoxCell { Value = GetModelMask(material.Name) }, new DataGridViewTextBoxCell { Value = material.Flags },
                    new DataGridViewTextBoxCell { Value = material.MTD }, new DataGridViewTextBoxCell { Value = material.Unk18 });
                for (int j = 0; j < 3; ++j)
                    row.Cells.Add(new DataGridViewButtonCell { Value = materialsTable.Columns[j + 6].HeaderText });
                for (int j = 0; j < 2; ++j)
                    row.Cells.Add(new DataGridViewCheckBoxCell { Value = false });
                materialsTable.Rows.Add(row);
            }
            for (int i = 0; i < Flver.Meshes.Count; ++i)
            {
                FLVER2.Mesh mesh = Flver.Meshes[i];
                if (mesh.MaterialIndex < 0) mesh.MaterialIndex = 0;
                DataGridViewRow row = new DataGridViewRow();
                row.Cells.AddRange(new DataGridViewTextBoxCell { Value = i },
                    new DataGridViewTextBoxCell { Value = Flver.Materials[mesh.MaterialIndex].Name });
                row.Cells.Add(new DataGridViewButtonCell { Value = "Apply" });
                try
                {
                    row.Cells.Add(new DataGridViewCheckBoxCell { Value = SelectedMeshIndices[i] >= 0 });
                }
                catch
                {
                    row.Cells.Add(new DataGridViewCheckBoxCell { Value = false });
                }
                row.Cells.Add(new DataGridViewCheckBoxCell { Value = false });
                meshTable.Rows.Add(row);
            }
            for (int i = 0; i < Flver.Dummies.Count; ++i)
            {
                FLVER.Dummy dummy = Flver.Dummies[i];
                DataGridViewRow row = new DataGridViewRow();
                row.Cells.AddRange(new DataGridViewTextBoxCell { Value = i },
                    new DataGridViewTextBoxCell { Value = dummy.ReferenceID },
                    new DataGridViewTextBoxCell { Value = dummy.AttachBoneIndex },
                    new DataGridViewTextBoxCell { Value = dummy.ParentBoneIndex });
                row.Cells.Add(new DataGridViewCheckBoxCell { Value = false });
                row.Cells.Add(new DataGridViewButtonCell { Value = dummiesTable.Columns[5].HeaderText });
                dummiesTable.Rows.Add(row);
            }
            if (SelectedMaterialIndex != -1)
            {
                applyMatBinTexturesButton.Enabled = true;
                UpdateTexturesTable();
            }
            IsSettingDefaultInfo = false;
        }

        private static void BackupFLVERFile()
        {
            string backupFilePath = FlverFilePath;
            if (backupFilePath.Contains(".flver")) backupFilePath = backupFilePath.Replace(".flver", ".flver.bak");
            else if (backupFilePath.Contains(".flv")) backupFilePath = backupFilePath.Replace(".flv", ".flv.bak");
            else backupFilePath = backupFilePath.Replace(".dcx", ".dcx.bak");
            if (!File.Exists(backupFilePath)) File.Copy(FlverFilePath, backupFilePath);
        }

        private static void UpdateWindowTitle(string flverPath)
        {
            Program.Window.Text = $@"{Program.WindowTitle} - {Path.GetFileName(flverPath)}";
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

        private FLVER2 ReadFLVERFromDCXPath(string filePath, bool setMainFlverCompressionType, bool setBinderIndex, bool wantsTpf)
        {
            List<BinderFile> flverFiles = new List<BinderFile>();
            BND4 newFlverBnd = null;
            try
            {
                newFlverBnd = SoulsFile<BND4>.Read(filePath);
                FlverBnd = newFlverBnd;
            }
            catch
            {
                try
                {
                    newFlverBnd = SoulsFile<BND4>.Read(DCX.Decompress(filePath));
                    FlverBnd = newFlverBnd;
                }
                catch { }
            }
            if (newFlverBnd != null)
            {
                if (setMainFlverCompressionType) FlverCompressionType = FlverBnd.Compression;
                int binderIndex = 0;
                foreach (BinderFile file in FlverBnd.Files)
                {
                    if (IsFLVERPath(file.Name))
                    {
                        flverFiles.Add(file);
                        if (CurrentFlverFileBinderIndex == -1 && setBinderIndex) CurrentFlverFileBinderIndex = binderIndex;
                    }
                    else if (file.Name.EndsWith(".tpf") && wantsTpf)
                    {
                        try
                        {
                            Program.Tpf = TPF.Read(file.Bytes);
                        }
                        catch { }
                    }
                    binderIndex++;
                }
                if (flverFiles.Count == 1) return FLVER2.Read(flverFiles[0].Bytes);
                if (flverFiles.Count > 1)
                {
                    int selectedFlverIndex = ShowSelectorDialog("Pick a FLVER", flverFiles);
                    if (!setBinderIndex || selectedFlverIndex == -1) return selectedFlverIndex == -1 ? null : FLVER2.Read(flverFiles[selectedFlverIndex].Bytes);
                    int binderWiseSelectedFlverIndex = FlverBnd.Files.FindIndex(i =>
                        i.Name.IndexOf(flverFiles[selectedFlverIndex].Name, StringComparison.OrdinalIgnoreCase) != -1);
                    CurrentFlverFileBinderIndex = binderWiseSelectedFlverIndex;
                    return FLVER2.Read(flverFiles[selectedFlverIndex].Bytes);
                }
            }
            ShowInformationDialog("No FLVER files were found in the DCX archive.");
            return null;
        }

        private bool OpenFLVERFile()
        {
            IsSettingDefaultInfo = true;
            StopAutoInternalIndexOverride = false;
            if (Arguments.Count > 0)
            {
                FlverFilePath = Arguments[0].ToLower();
                Arguments = new List<string>();
            }
            else
            {
                FlverFilePath = PromptFLVERModel();
                if (FlverFilePath == "") return false;
            }
            if (Program.Window != null) UpdateWindowTitle(FlverFilePath);
            Mono3D.textureMap = new Dictionary<string, Texture2D>();
            Program.Tpf = null;
            Program.FilePath = FlverFilePath;
            if (IsFLVERPath(FlverFilePath))
            {
                Flver = FLVER2.Read(FlverFilePath);
                Program.Flver = Flver;
            }
            else
            {
                FLVER2 newFlver = ReadFLVERFromDCXPath(FlverFilePath, true, true, true);
                if (newFlver == null) return false;
                Flver = newFlver;
                Program.Flver = Flver;
            }
            CurrentFlverBytes = Flver.Write();
            saveToolStripMenuItem.Enabled = saveAsToolStripMenuItem.Enabled = true;
            MatBinBndPath = null;
            DeselectAllSelectedThings();
            UpdateUI();
            ClearViewerMaterialHighlight();
            ClearUndoRedoStates();
            LoadMaterialPresets();
            LoadDummyPresets();
            LoadViewer();
            IsSettingDefaultInfo = false;
            StopAutoInternalIndexOverride = true;
            return true;
        }

        private void OpenButtonClicked(object sender, EventArgs e)
        {
            OpenFLVERFile();
        }

        private void UpdateTexturesTable()
        {
            if (SelectedMaterialIndex == -1) return;
            texturesTable.Rows.Clear();
            for (int i = 0; i < Flver.Materials[SelectedMaterialIndex].Textures.Count; ++i)
            {
                FLVER2.Material material = Flver.Materials[SelectedMaterialIndex];
                DataGridViewRow row = new DataGridViewRow();
                row.Cells.AddRange(new DataGridViewTextBoxCell { Value = material.Textures[i].Type },
                    new DataGridViewTextBoxCell { Value = material.Textures[i].Path });
                row.Cells.Add(new DataGridViewButtonCell { Value = texturesTable.Columns[2].HeaderText });
                texturesTable.Rows.Add(row);
            }
        }

        private void UpdateMaterialPresets()
        {
            File.WriteAllText(MaterialPresetsFilePath, JToken.Parse(new JavaScriptSerializer().Serialize(MaterialPresets)).ToString());
            LoadMaterialPresets();
        }

        private void UpdateDummyPresets()
        {
            File.WriteAllText(DummyPresetsFilePath, new JavaScriptSerializer().Serialize(DummyPresets));
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
                case MaterialEditButtonIndex:
                    applyMatBinTexturesButton.Enabled = true;
                    SelectedMaterialIndex = e.RowIndex;
                    UpdateTexturesTable();
                    break;
                case MaterialViewerHighlightButtonIndex:
                    bool unhighlighted = Flver.Meshes.Any(mesh => SelectedMaterialMeshIndices.IndexOf(Flver.Meshes.IndexOf(mesh)) == -1 && mesh.MaterialIndex == e.RowIndex);
                    ClearViewerMaterialHighlight();
                    if (unhighlighted)
                    {
                        foreach (FLVER2.Mesh mesh in Flver.Meshes.Where(mesh => mesh.MaterialIndex == e.RowIndex))
                            SelectedMaterialMeshIndices.Add(Flver.Meshes.IndexOf(mesh));
                    }
                    UpdateMesh();
                    break;
                case MaterialAddPresetCbIndex when !MaterialPresets.ContainsKey(Flver.Materials[e.RowIndex].Name):
                    string presetName = PromptForPresetName();
                    if (presetName == "") break;
                    if (!MaterialPresets.ContainsKey(presetName))
                    {
                        MaterialPresets.Add(presetName, Flver.Materials[e.RowIndex]);
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
            ToggleCheckboxesInDataTable(materialsTable, MaterialApplyPresetCbIndex);
        }

        private void MaterialsTableDeleteAllButtonClicked(object sender, MouseEventArgs e)
        {
            ToggleCheckboxesInDataTable(materialsTable, MaterialDeleteCbIndex);
        }

        private static void InjectTextureIntoTPF(string textureFilePath)
        {
            if (Program.Tpf == null) Program.Tpf = new TPF();
            BinderFile flverBndTpfEntry = FlverBnd?.Files.FirstOrDefault(i => i.Name.EndsWith(".tpf"));
            byte[] ddsBytes = File.ReadAllBytes(textureFilePath);
            DDS dds = new DDS(ddsBytes);
            byte formatByte = 107;
            try
            {
                formatByte = (byte)Enum.Parse(typeof(Program.TextureFormats), dds.header10.dxgiFormat.ToString());
            }
            catch { }
            TPF.Texture texture = new TPF.Texture(Path.GetFileNameWithoutExtension(textureFilePath), formatByte, 0x00, File.ReadAllBytes(textureFilePath));
            int existingTextureIndex = Program.Tpf.Textures.FindIndex(i => i.Name == texture.Name);
            if (existingTextureIndex != -1)
            {
                Program.Tpf.Textures.RemoveAt(existingTextureIndex);
                Program.Tpf.Textures.Insert(existingTextureIndex, texture);
            }
            else Program.Tpf.Textures.Add(texture);
            if (flverBndTpfEntry != null) FlverBnd.Files[FlverBnd.Files.IndexOf(flverBndTpfEntry)].Bytes = Program.Tpf.Write();
            else
            {
                if (FlverFilePath.Contains(".flver")) Program.Tpf.Write(FlverFilePath.Replace("_1.", ".").Replace(".flver", ".tpf"));
                else if (FlverFilePath.Contains(".flv")) Program.Tpf.Write(FlverFilePath.Replace("_1.", ".").Replace(".flv", ".tpf"));
            }
        }

        private void TexturesTableButtonClicked(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != 2) return;
            OpenFileDialog dialog = new OpenFileDialog { Filter = ImageFilesFilter };
            if (dialog.ShowDialog() != DialogResult.OK) return;
            UpdateUndoState();
            Flver.Materials[SelectedMaterialIndex].Textures[e.RowIndex].Path = $"{Path.GetFileNameWithoutExtension(dialog.FileName)}.tif";
            InjectTextureIntoTPF(dialog.FileName);
            UpdateTexturesTable();
            UpdateMesh();
            Viewer.RefreshTextures();
        }

        private void ResetModifierNumBoxValues()
        {
            foreach (NumericUpDown box in meshModifiersNumBoxesContainer.Controls)
                box.Value = 0;
        }

        private void EnableDisableExtraModifierOptions()
        {
            reverseFacesetsCheckbox.Enabled = reverseNormalsCheckbox.Enabled = toggleBackfacesCheckbox.Enabled =
                deleteFacesetsCheckbox.Enabled = SelectedMeshIndices.Count != 0;
            vectorModeCheckbox.Enabled = SelectedDummyIndices.Count != 0;
            uniformScaleCheckbox.Enabled = !vectorModeCheckbox.Checked;
            if (!uniformScaleCheckbox.Enabled) uniformScaleCheckbox.Checked = false;
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
            if (IsSettingDefaultInfo) return;
            if (DummyIsSelected)
            {
                IsSettingDefaultInfo = true;
                bool hasIndices = SelectedDummyIndices.Count != 0 || SelectedMeshIndices.Count > 0;
                ResetModifierNumBoxValues();
                meshModifiersContainer.Enabled = hasIndices;
                if (hasIndices)
                {
                    EnableDisableExtraModifierOptions();
                    scaleXNumBox.Value = scaleYNumBox.Value = scaleZNumBox.Value = 100;
                    rotXNumBox.Value = rotYNumBox.Value = rotZNumBox.Value = 0;
                }
                IsSettingDefaultInfo = false;
            }
            UpdateMesh();
        }

        private void UpdateSelectedMeshes()
        {
            if (IsSettingDefaultInfo) return;
            if (MeshIsSelected)
            {
                IsSettingDefaultInfo = true;
                bool hasIndices = SelectedMeshIndices.Count != 0 || SelectedDummyIndices.Count > 0;
                ResetModifierNumBoxValues();
                meshModifiersContainer.Enabled = hasIndices;
                if (hasIndices)
                {
                    EnableDisableExtraModifierOptions();
                    scaleXNumBox.Value = scaleYNumBox.Value = scaleZNumBox.Value = 100;
                    rotXNumBox.Value = rotYNumBox.Value = rotZNumBox.Value = 0;
                }
                IsSettingDefaultInfo = false;
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
            int boneIndex = ShowSelectorDialog("Pick a Bone", Flver.Bones);
            if (boneIndex == -1) return;
            List<FLVER.Vertex> unweightedVerts = Flver.Meshes[meshIndex].Vertices.Where(v =>
                Util3D.BoneIndicesToIntArray(v.BoneIndices) != null
                && !Util3D.BoneIndicesToIntArray(v.BoneIndices).Contains(boneIndex)
                && Util3D.BoneWeightsToFloatArray(v.BoneWeights) != null
                && Util3D.BoneWeightsToFloatArray(v.BoneWeights).All(i => i == 0)).ToList();
            if (!unweightedVerts.Any())
            {
                ShowInformationDialog("Found no unweighted vertices to apply default weights to.");
                return;
            }
            UpdateUndoState();
            foreach (FLVER.Vertex v in unweightedVerts)
            {
                v.BoneIndices[0] = boneIndex;
                v.BoneWeights = new FLVER.VertexBoneWeights
                {
                    [0] = 1,
                    [1] = 0,
                    [2] = 0,
                    [3] = 0
                };
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
                    SelectedMeshIndices = UpdateIndicesList(meshTable, SelectedMeshIndices, columnIndex, rowIndex, ref MeshIsSelected);
                    UpdateSelectedMeshes();
                    break;
                case 4:
                    HiddenMeshIndices = UpdateIndicesList(meshTable, HiddenMeshIndices, columnIndex, rowIndex, ref MeshIsHidden);
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
                    SelectedDummyIndices = UpdateIndicesList(dummiesTable, SelectedDummyIndices, columnIndex, rowIndex, ref DummyIsSelected);
                    UpdateSelectedDummies();
                    break;
                case 5:
                    UpdateUndoState();
                    FLVER.Dummy duplicatedDummy = new FLVER.Dummy
                    {
                        Position = Flver.Dummies[rowIndex].Position,
                        Forward = Flver.Dummies[rowIndex].Forward,
                        Upward = Flver.Dummies[rowIndex].Upward,
                        ReferenceID = Flver.Dummies[rowIndex].ReferenceID,
                        ParentBoneIndex = Flver.Dummies[rowIndex].ParentBoneIndex,
                        AttachBoneIndex = Flver.Dummies[rowIndex].AttachBoneIndex,
                        Color = Flver.Dummies[rowIndex].Color,
                        Flag1 = Flver.Dummies[rowIndex].Flag1,
                        UseUpwardVector = Flver.Dummies[rowIndex].UseUpwardVector,
                        Unk30 = Flver.Dummies[rowIndex].Unk30,
                        Unk34 = Flver.Dummies[rowIndex].Unk34
                    };
                    Flver.Dummies.Add(duplicatedDummy);
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

        private static Vector3 CreateTranslationVector(float x, float y, float z, float offset, int nbi)
        {
            return new Vector3(x + (nbi == 0 ? offset : 0), y + (nbi == 1 ? offset : 0), z + (nbi == 2 ? offset : 0));
        }

        private static Vector3 CreateScaleVector(float x, float y, float z, float offset, IReadOnlyList<float> totals, int nbi, bool uniform, bool invert)
        {
            float scalar = offset < 0 && !invert ? -(offset - 1) : invert ? offset - 1 : offset + 1;
            float newX = nbi == 0 || uniform ? x - totals[0] : x;
            float newY = nbi == 1 || uniform ? y - totals[1] : y;
            float newZ = nbi == 2 || uniform ? z - totals[2] : z;
            newX = nbi == 0 || uniform ? (offset < 0 && !invert ? newX / scalar : newX * scalar) + totals[0] : x;
            newY = nbi == 1 || uniform ? (offset < 0 && !invert ? newY / scalar : newY * scalar) + totals[1] : y;
            newZ = nbi == 2 || uniform ? (offset < 0 && !invert ? newZ / scalar : newZ * scalar) + totals[2] : z;
            return new Vector3(newX, newY, newZ);
        }

        private static dynamic CreateRotationVector(float x, float y, float z, float w, float offset, IReadOnlyList<float> totals, int nbi)
        {
            float newX = nbi == 1 ? offset : 0;
            float newY = nbi == 0 ? offset : 0;
            float newZ = nbi == 2 ? offset : 0;
            Vector3 vector = new Vector3(x - totals[0], y - totals[1], z - totals[2]);
            vector = Program.RotatePoint(vector, newY, newX, newZ);
            return w == 0 ? (dynamic)new Vector3(vector.X + totals[0], vector.Y + totals[1], vector.Z + totals[2]) :
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
                    v.Position = CreateTranslationVector(v.Position.X, v.Position.Y, v.Position.Z, offset, nbi);
                    break;
            }
        }

        private static void ScaleThing(dynamic thing, float offset, IReadOnlyList<float> totals, int nbi, bool uniform, bool invert, bool useVectorMode)
        {
            if (nbi >= 3 && nbi <= 5) nbi -= 3;
            switch (thing)
            {
                case FLVER.Dummy d:
                    if (useVectorMode) d.Forward = CreateTranslationVector(d.Forward.X, d.Forward.Y, d.Forward.Z, offset, nbi);
                    else d.Position = CreateScaleVector(d.Position.X, d.Position.Y, d.Position.Z, offset, totals, nbi, uniform, invert);
                    break;
                case FLVER.Vertex v:
                    v.Position = CreateScaleVector(v.Position.X, v.Position.Y, v.Position.Z, offset, totals, nbi, uniform, invert);
                    v.Normal = new Vector3(v.Normal.X, v.Normal.Y, invert && nbi != 2 ? -v.Normal.Z : v.Normal.Z);
                    v.Tangents[0] = new Vector4(v.Tangents[0].X, v.Tangents[0].Y, invert && nbi != 2 ? -v.Normal.Z : v.Normal.Z, v.Tangents[0].W);
                    break;
            }
        }

        private static void RotateThing(dynamic thing, float offset, IReadOnlyList<float> totals, int nbi, bool useVectorMode)
        {
            if (nbi >= 6 && nbi <= 8) nbi -= 6;
            float newX = nbi == 0 ? offset : 0;
            float newY = nbi == 1 ? offset : 0;
            float newZ = nbi == 2 ? offset : 0;
            switch (thing)
            {
                case FLVER.Dummy d:
                    if (useVectorMode) d.Forward = Program.RotatePoint(d.Forward, newX, newZ, newY);
                    else d.Position = CreateRotationVector(d.Position.X, d.Position.Y, d.Position.Z, 0, offset, totals, nbi);
                    break;
                case FLVER.Vertex v:
                    v.Position = CreateRotationVector(v.Position.X, v.Position.Y, v.Position.Z, 0, offset, totals, nbi);
                    v.Normal = CreateRotationVector(v.Normal.X, v.Normal.Y, v.Normal.Z, 0, offset, new float[3], nbi);
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
                    ScaleThing(thing, offset, totals, nbi, uniformScaleCheckbox.Checked, false, vectorModeCheckbox.Checked);
                    if (uniformScaleCheckbox.Checked) scaleXNumBox.Value = scaleYNumBox.Value = scaleZNumBox.Value = newValue;
                    break;
                case 6:
                case 7:
                case 8:
                    RotateThing(thing, offset, totals, nbi, vectorModeCheckbox.Checked);
                    break;
            }
        }

        private static float[] CalculateMeshTotals()
        {
            if (UseWorldOrigin) return new float[3];
            float vertexCount = 0, xSum = 0, ySum = 0, zSum = 0;
            List<int> meshIndices = new List<int>();
            bool shouldAffectAllMesh = SelectedMeshIndices.Count == 0;
            if (shouldAffectAllMesh)
            {
                for (int i = 0; i < Flver.Meshes.Count; ++i)
                    meshIndices.Add(i);
            }
            else meshIndices = SelectedMeshIndices;
            foreach (int i in meshIndices)
            {
                foreach (FLVER.Vertex v in Flver.Meshes[i].Vertices)
                {
                    xSum += v.Position.X;
                    ySum += v.Position.Y;
                    zSum += v.Position.Z;
                }
                vertexCount += Flver.Meshes[i].Vertices.Count;
            }
            return new[] { xSum / vertexCount, ySum / vertexCount, zSum / vertexCount };
        }

        // TODO: Reflect the correct values when performing an undo/redo action
        private void ModifierNumBoxValueChanged(object sender, EventArgs e)
        {
            if (IsSettingDefaultInfo) return;
            UpdateUndoState();
            NumericUpDown numBox = (NumericUpDown)sender;
            int nbi = meshModifiersNumBoxesContainer.GetRow(numBox) * meshModifiersNumBoxesContainer.ColumnCount
                + meshModifiersNumBoxesContainer.GetColumn(numBox);
            float newNumVal = (float)(numBox == rotXNumBox || numBox == rotYNumBox || numBox == rotZNumBox ? ToRadians(numBox.Value) : numBox.Value);
            if (numBox == rotYNumBox && SelectedMeshIndices.Count != 0) newNumVal = -newNumVal;
            if (numBox == scaleXNumBox || numBox == scaleYNumBox || numBox == scaleZNumBox) newNumVal = (float)(numBox.Value / 100);
            float offset = newNumVal < PrevNumVal ?
                -Math.Abs(newNumVal - PrevNumVal)
                : Math.Abs(newNumVal - PrevNumVal);
            float[] totals = CalculateMeshTotals();
            foreach (FLVER.Vertex v in SelectedMeshIndices.SelectMany(i => Flver.Meshes[i].Vertices))
                TransformThing(v, offset, totals, nbi, numBox.Value);
            foreach (int i in SelectedDummyIndices)
                TransformThing(Flver.Dummies[i], offset, totals, nbi, numBox.Value);
            UpdateMesh();
            PrevNumVal = newNumVal;
        }

        private void ModifierNumBoxFocused(object sender, EventArgs e)
        {
            NumericUpDown numBox = (NumericUpDown)sender;
            PrevNumVal = (float)(numBox == rotXNumBox || numBox == rotYNumBox || numBox == rotZNumBox ? ToRadians(numBox.Value) : numBox.Value);
            if (numBox == rotYNumBox && SelectedMeshIndices.Count != 0) PrevNumVal = -PrevNumVal;
            if (numBox == scaleXNumBox || numBox == scaleYNumBox || numBox == scaleZNumBox) PrevNumVal = (float)(numBox.Value / 100);
        }

        private void MaterialsTableOkButtonClicked(object sender, MouseEventArgs e)
        {
            try
            {
                UpdateUndoState();
                foreach (DataGridViewRow row in materialsTable.Rows)
                {
                    if (!(bool)row.Cells[MaterialApplyPresetCbIndex].Value) continue;
                    string prevName = Flver.Materials[row.Index].Name;
                    Flver.Materials[row.Index] = new JavaScriptSerializer().Deserialize<FLVER2.Material>(
                        new JavaScriptSerializer().Serialize(MaterialPresets.Values.ToArray()[materialPresetsSelector.SelectedIndex]));
                    Flver.Materials[row.Index].Name = prevName;
                }
                for (int i = Flver.Materials.Count - 1; i >= 0; --i)
                {
                    if (!(bool)materialsTable.Rows[i].Cells[MaterialDeleteCbIndex].Value || Flver.Materials.Count <= 1) continue;
                    Flver.Materials.RemoveAt(i);
                    foreach (FLVER2.Mesh mesh in Flver.Meshes.Where(mesh => mesh.MaterialIndex > 0))
                        mesh.MaterialIndex--;
                }
                ClearViewerMaterialHighlight();
                UpdateUI();
                UpdateMesh();
                Viewer.RefreshTextures();
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
                        HiddenMeshIndices = UpdateIndicesList(meshTable, HiddenMeshIndices, columnIndex, row.Index, ref MeshIsHidden);
                        break;
                    case 4 when table == dummiesTable:
                        SelectedDummyIndices = UpdateIndicesList(dummiesTable, SelectedDummyIndices, columnIndex, row.Index, ref DummyIsSelected);
                        break;
                    case 3:
                        SelectedMeshIndices = UpdateIndicesList(meshTable, SelectedMeshIndices, columnIndex, row.Index, ref MeshIsSelected);
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
            IsSettingDefaultInfo = true;
            ToggleCheckboxesInDataTable(table, columnIndex);
            IsSettingDefaultInfo = false;
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
            for (int i = Flver.Meshes.Count - 1; i >= 0; --i)
            {
                if (!(bool)meshTable.Rows[i].Cells[3].Value) continue;
                if (deleteFacesetsCheckbox.Checked)
                {
                    foreach (FLVER2.FaceSet fs in Flver.Meshes[i].FaceSets)
                        for (int j = 0; j < fs.Indices.Count; ++j)
                            fs.Indices[j] = 1;
                }
                else
                {
                    SelectedMeshIndices.RemoveAt(SelectedMeshIndices.IndexOf(i));
                    Flver.Meshes.RemoveAt(i);
                }
            }
            for (int i = Flver.Dummies.Count - 1; i >= 0; --i)
            {
                if (!(bool)dummiesTable.Rows[i].Cells[4].Value) continue;
                SelectedDummyIndices.RemoveAt(SelectedDummyIndices.IndexOf(i));
                Flver.Dummies.RemoveAt(i);
            }
            meshModifiersContainer.Enabled = MeshIsSelected = DummyIsSelected = false;
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
                Flver.Write(filePath);
            }
            else if (filePath.EndsWith(".dcx"))
            {
                BackupFLVERFile();
                Flver.Write(filePath);
                FlverBnd.Files[CurrentFlverFileBinderIndex].Bytes = File.ReadAllBytes(filePath);
                FlverBnd.Write(filePath, FlverCompressionType);
            }
        }

        private void SaveFLVERAs()
        {
            string bndFilter = FlverFilePath.EndsWith(".dcx") ? "|BND File (*.dcx)|*.dcx" : "";
            SaveFileDialog dialog = new SaveFileDialog
                { Filter = $@"FLVER File (*.flver, *.flv)|*.flver;*.flv{bndFilter}", FileName = Path.GetFileNameWithoutExtension(FlverFilePath.Replace(".dcx", "")) };
            if (dialog.ShowDialog() != DialogResult.OK) return;
            string modelFilePath = dialog.FileName;
            if (FlverFilePath.EndsWith(".dcx"))
            {
                int newPartID = GetModelPartIDFromName(modelFilePath);
                if (newPartID != 0)
                {
                    int ogModelPartID = GetModelPartIDFromName(FlverFilePath);
                    if (ogModelPartID != -1)
                    {
                        foreach (BinderFile file in FlverBnd.Files)
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
            Arguments.Add(modelFilePath);
            OpenFLVERFile();
        }

        private void SaveButtonClicked(object sender, EventArgs e)
        {
            SaveFLVERFile(FlverFilePath);
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
            if (IsSettingDefaultInfo || !IsTextBoxCell(sender, e.ColumnIndex, e.RowIndex)) return;
            try
            {
                UpdateUndoState();
                string bonesTableValue = bonesTable[e.ColumnIndex, e.RowIndex].Value?.ToString();
                if (bonesTableValue != null)
                {
                    switch (e.ColumnIndex)
                    {
                        case 1:
                            Flver.Bones[e.RowIndex].Name = bonesTableValue;
                            break;
                        case 2:
                            Flver.Bones[e.RowIndex].ParentIndex = short.Parse(bonesTableValue);
                            break;
                        case 3:
                            Flver.Bones[e.RowIndex].ChildIndex = short.Parse(bonesTableValue);
                            break;
                        case 4:
                        case 5:
                        case 6:
                        case 7:
                        case 8:
                            string[] comp = bonesTableValue.Split(',');
                            Vector3 vector = new Vector3(float.Parse(comp[0]), float.Parse(comp[1]), float.Parse(comp[2]));
                            Flver.Bones[e.RowIndex].Translation = e.ColumnIndex == 4 ? vector : Flver.Bones[e.RowIndex].Translation;
                            Flver.Bones[e.RowIndex].Scale = e.ColumnIndex == 5 ? vector : Flver.Bones[e.RowIndex].Scale;
                            Flver.Bones[e.RowIndex].Rotation = e.ColumnIndex == 6 ? vector : Flver.Bones[e.RowIndex].Rotation;
                            Flver.Bones[e.RowIndex].BoundingBoxMin = e.ColumnIndex == 7 ? vector : Flver.Bones[e.RowIndex].BoundingBoxMin;
                            Flver.Bones[e.RowIndex].BoundingBoxMax = e.ColumnIndex == 8 ? vector : Flver.Bones[e.RowIndex].BoundingBoxMax;
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
            if (IsSettingDefaultInfo || !IsTextBoxCell(sender, e.ColumnIndex, e.RowIndex)) return;
            try
            {
                UpdateUndoState();
                string materialName = Flver.Materials[e.RowIndex].Name;
                string materialsTableValue = materialsTable[e.ColumnIndex, e.RowIndex].Value?.ToString();
                if (materialsTableValue != null)
                {
                    switch (e.ColumnIndex)
                    {
                        case 1:
                            Flver.Materials[e.RowIndex].Name = materialsTableValue;
                            break;
                        case 2:
                            Flver.Materials[e.RowIndex].Name = ReplaceModelMask(materialName, materialsTableValue);
                            break;
                        case 3:
                            Flver.Materials[e.RowIndex].Flags = int.Parse(materialsTableValue);
                            break;
                        case 4:
                            Flver.Materials[e.RowIndex].MTD = materialsTableValue;
                            break;
                        case 5:
                            Flver.Materials[e.RowIndex].Unk18 = int.Parse(materialsTableValue);
                            break;
                    }
                }
            }
            catch { }
            UpdateUI();
            UpdateMesh();
            Viewer.RefreshTextures();
        }

        private void TexturesTableCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (IsSettingDefaultInfo || !IsTextBoxCell(sender, e.ColumnIndex, e.RowIndex)) return;
            try
            {
                UpdateUndoState();
                string textureTableValue = texturesTable[e.ColumnIndex, e.RowIndex].Value?.ToString() ?? "";
                switch (e.ColumnIndex)
                {
                    case 0:
                        Flver.Materials[SelectedMaterialIndex].Textures[e.RowIndex].Type = textureTableValue;
                        break;
                    case 1:
                        Flver.Materials[SelectedMaterialIndex].Textures[e.RowIndex].Path = textureTableValue;
                        break;
                }
            }
            catch { }
            UpdateMesh();
            Viewer.RefreshTextures();
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
            if (IsSettingDefaultInfo || !IsTextBoxCell(sender, e.ColumnIndex, e.RowIndex) || e.ColumnIndex != 2) return;
            try
            {
                UpdateUndoState();
                string boneWeightValue = meshTable[2, e.RowIndex].Value?.ToString();
                if (boneWeightValue != null)
                {
                    int newBoneWeight = int.Parse(boneWeightValue);
                    foreach (FLVER.Vertex v in Flver.Meshes[e.RowIndex].Vertices)
                    {
                        if (Util3D.BoneWeightsToFloatArray(v.BoneWeights) == null)
                        {
                            v.BoneWeights = new FLVER.VertexBoneWeights();
                            v.BoneIndices = new FLVER.VertexBoneIndices();
                        }
                        for (int j = 0; j < v.BoneWeights.Length; ++j)
                            v.BoneWeights[j] = 0;
                        v.BoneIndices[0] = newBoneWeight;
                        v.BoneWeights[0] = 1;
                    }
                    if (!Flver.Meshes[e.RowIndex].BoneIndices.Contains(newBoneWeight)) Flver.Meshes[e.RowIndex].BoneIndices.Add(newBoneWeight);
                    Flver.Meshes[e.RowIndex].Dynamic = 1;
                }
            }
            catch { }
            UpdateUI();
            UpdateMesh();
        }

        private void ReverseFaceSets()
        {
            UpdateUndoState();
            foreach (FLVER2.FaceSet fs in SelectedMeshIndices.SelectMany(i => Flver.Meshes[i].FaceSets))
            {
                for (int j = 0; j < fs.Indices.Count; j += 3)
                    (fs.Indices[j + 1], fs.Indices[j + 2]) = (fs.Indices[j + 2], fs.Indices[j + 1]);
            }
            UpdateMesh();
        }

        private void ReverseFaceSetsCheckboxChanged(object sender, EventArgs e)
        {
            if (IsSettingDefaultInfo) return;
            ReverseFaceSets();
        }

        private void ReverseNormalsCheckboxChanged(object sender, EventArgs e)
        {
            if (IsSettingDefaultInfo) return;
            UpdateUndoState();
            foreach (FLVER.Vertex v in SelectedMeshIndices.SelectMany(i => Flver.Meshes[i].Vertices))
            {
                v.Normal = new Vector3(-v.Normal.X, -v.Normal.Y, -v.Normal.Z);
                for (int j = 0; j < v.Tangents.Count; ++j)
                    v.Tangents[j] = new Vector4(-v.Tangents[j].X, -v.Tangents[j].Y, -v.Tangents[j].Z, v.Tangents[j].W);
            }
            ShowInformationDialog("Mesh normals have been reversed!");
            UpdateMesh();
        }

        private void ToggleBackFacesCheckboxChanged(object sender, EventArgs e)
        {
            if (IsSettingDefaultInfo) return;
            foreach (FLVER2.FaceSet fs in SelectedMeshIndices.SelectMany(i => Flver.Meshes[i].FaceSets))
                fs.CullBackfaces = !fs.CullBackfaces;
            ShowInformationDialog("Mesh backfaces have been toggled!");
        }

        private static void UpdateHeaderBoundingBox(FLVER2.FLVERHeader header, Vector3 vertexPos)
        {
            float minX = Math.Min(header.BoundingBoxMin.X, vertexPos.X);
            float minY = Math.Min(header.BoundingBoxMin.Y, vertexPos.Y);
            float minZ = Math.Min(header.BoundingBoxMin.Z, vertexPos.Z);
            float maxX = Math.Max(header.BoundingBoxMax.X, vertexPos.X);
            float maxY = Math.Max(header.BoundingBoxMax.Y, vertexPos.Y);
            float maxZ = Math.Max(header.BoundingBoxMax.Z, vertexPos.Z);
            header.BoundingBoxMin = new Vector3(minX, minY, minZ);
            header.BoundingBoxMax = new Vector3(maxX, maxY, maxZ);
        }

        private static void UpdateMeshBoundingBox(FLVER2.Mesh mesh, Vector3 vertexPos)
        {
            float minX = Math.Min(mesh.BoundingBox.Min.X, vertexPos.X);
            float minY = Math.Min(mesh.BoundingBox.Min.Y, vertexPos.Y);
            float minZ = Math.Min(mesh.BoundingBox.Min.Z, vertexPos.Z);
            float maxX = Math.Max(mesh.BoundingBox.Max.X, vertexPos.X);
            float maxY = Math.Max(mesh.BoundingBox.Max.Y, vertexPos.Y);
            float maxZ = Math.Max(mesh.BoundingBox.Max.Z, vertexPos.Z);
            mesh.BoundingBox.Min = new Vector3(minX, minY, minZ);
            mesh.BoundingBox.Max = new Vector3(maxX, maxY, maxZ);
        }

        private static System.Numerics.Matrix4x4 GetNMatrix(FLVER.Bone b)
        {
            return System.Numerics.Matrix4x4.CreateScale(b.Scale)
                * System.Numerics.Matrix4x4.CreateRotationX(b.Rotation.X)
                * System.Numerics.Matrix4x4.CreateRotationZ(b.Rotation.Z)
                * System.Numerics.Matrix4x4.CreateRotationY(b.Rotation.Y)
                * System.Numerics.Matrix4x4.CreateTranslation(b.Translation);
        }

        private static FLVER.Bone GetParent(FLVER.Bone b, IReadOnlyList<FLVER.Bone> bones)
        {
            if (b.ParentIndex >= 0 && b.ParentIndex < bones.Count) return bones[b.ParentIndex];
            return null;
        }

        private static System.Numerics.Matrix4x4 GetAbsoluteNMatrix(FLVER.Bone b, IReadOnlyList<FLVER.Bone> bones)
        {
            System.Numerics.Matrix4x4 result = System.Numerics.Matrix4x4.Identity;
            FLVER.Bone parentBone = b;
            while (parentBone != null)
            {
                System.Numerics.Matrix4x4 m = GetNMatrix(parentBone);
                result *= m;
                parentBone = GetParent(parentBone, bones);
            }
            return result;
        }

        private static void UpdateBonesBoundingBox(FLVER.Bone b, IReadOnlyList<FLVER.Bone> bones, Vector3 vertexPos)
        {
            System.Numerics.Matrix4x4 boneAbsoluteMatrix = GetAbsoluteNMatrix(b, bones);
            if (!System.Numerics.Matrix4x4.Invert(boneAbsoluteMatrix, out System.Numerics.Matrix4x4 invertedBoneMatrix)) return;
            Vector3 posForBBox = Vector3.Transform(vertexPos, invertedBoneMatrix);
            float minX = Math.Min(b.BoundingBoxMin.X, posForBBox.X);
            float minY = Math.Min(b.BoundingBoxMin.Y, posForBBox.Y);
            float minZ = Math.Min(b.BoundingBoxMin.Z, posForBBox.Z);
            float maxX = Math.Max(b.BoundingBoxMax.X, posForBBox.X);
            float maxY = Math.Max(b.BoundingBoxMax.Y, posForBBox.Y);
            float maxZ = Math.Max(b.BoundingBoxMax.Z, posForBBox.Z);
            b.BoundingBoxMin = new Vector3(minX, minY, minZ);
            b.BoundingBoxMax = new Vector3(maxX, maxY, maxZ);
        }

        private void SolveAllBBs()
        {
            UpdateUndoState();
            Flver.Header.BoundingBoxMin = new Vector3();
            Flver.Header.BoundingBoxMax = new Vector3();
            foreach (FLVER.Bone bone in Flver.Bones)
            {
                bone.BoundingBoxMin = new Vector3();
                bone.BoundingBoxMax = new Vector3();
            }
            foreach (FLVER2.Mesh mesh in Flver.Meshes)
            {
                foreach (FLVER.Vertex vertex in mesh.Vertices)
                {
                    UpdateHeaderBoundingBox(Flver.Header, vertex.Position);
                    UpdateMeshBoundingBox(mesh, vertex.Position);
                    if (Util3D.BoneIndicesToIntArray(vertex.BoneIndices) == null) continue;
                    foreach (int boneIndex in Util3D.BoneIndicesToIntArray(vertex.BoneIndices))
                    {
                        bool boneDoesNotExist = false;
                        if (boneIndex >= 0 && boneIndex < Flver.Bones.Count) Flver.Bones[boneIndex].Unk3C = 0;
                        else boneDoesNotExist = true;
                        if (!boneDoesNotExist) UpdateBonesBoundingBox(Flver.Bones[boneIndex], Flver.Bones, vertex.Position);
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
            string dummyJson = new JavaScriptSerializer().Serialize(DummyPresets.Values.ToArray()[dummyPresetsSelector.SelectedIndex]);
            Flver.Dummies = new JavaScriptSerializer().Deserialize<List<FLVER.Dummy>>(dummyJson);
            UpdateUI();
            UpdateMesh();
        }

        private void AddAllDummiesToPresetsButtonClicked(object sender, MouseEventArgs e)
        {
            string presetName = PromptForPresetName();
            if (presetName == "" || DummyPresets.ContainsKey(presetName)) return;
            DummyPresets.Add(presetName, new JavaScriptSerializer().Deserialize<object>(new JavaScriptSerializer().Serialize(Flver.Dummies)));
            File.WriteAllText(DummyPresetsFilePath, new JavaScriptSerializer().Serialize(DummyPresets));
            LoadDummyPresets();
        }

        private static void ExportFLVERAsDAE()
        {
            SaveFileDialog dialog = new SaveFileDialog { FileName = $"{Path.GetFileNameWithoutExtension(FlverFilePath)}.dae", Filter = @"Collada DAE File (*.dae)|*.dae" };
            if (dialog.ShowDialog() != DialogResult.OK) return;
            try
            {
                Scene scene = new Scene { RootNode = new Node() };
                foreach (FLVER2.Material m in Flver.Materials)
                    scene.Materials.Add(new Material { Name = m.Name });
                for (int i = 0; i < Flver.Meshes.Count; ++i)
                {
                    FLVER2.Mesh m = Flver.Meshes[i];
                    Mesh newMesh = new Mesh("Mesh_M" + i, PrimitiveType.Triangle);
                    foreach (FLVER.Vertex v in m.Vertices)
                    {
                        newMesh.Vertices.Add(new Assimp.Vector3D(v.Position.X, v.Position.Y, v.Position.Z));
                        newMesh.Normals.Add(new Assimp.Vector3D(v.Normal.X, v.Normal.Y, v.Normal.Z));
                        newMesh.Tangents.Add(new Assimp.Vector3D(v.Tangents[0].X, v.Tangents[0].Y, v.Tangents[0].Z));
                        for (int j = 0; j < v.UVs.Count; ++j)
                            newMesh.TextureCoordinateChannels[j].Add(new Assimp.Vector3D(v.UVs[j].X, 1 - v.UVs[j].Y, 0));
                    }
                    foreach (FLVER2.FaceSet faceSet in m.FaceSets)
                        newMesh.Faces.Add(new Face(new[] { faceSet.Indices[0], faceSet.Indices[1], faceSet.Indices[2] }));
                    newMesh.MaterialIndex = m.MaterialIndex;
                    scene.Meshes.Add(newMesh);
                    Node nodeBase = new Node { Name = "M_" + i + "_" + Flver.Materials[m.MaterialIndex].Name };
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

        private void ImportFLVERFile(bool prompt, string filePath)
        {
            UpdateUndoState();
            if (prompt)
            {
                OpenFileDialog dialog = new OpenFileDialog { Filter = @"3D Object|*.dae;*.obj;*.fbx" };
                if (dialog.ShowDialog() != DialogResult.OK) return;
                if (!Importer.ImportAssimp(Program.Flver, dialog.FileName)) return;
            }
            else if (!Importer.ImportAssimp(Program.Flver, filePath)) return;
            ShowInformationDialog("Successfully imported model into the current FLVER file!");
            UpdateUndoState();
            Flver = Program.Flver;
            DeselectAllSelectedThings();
            UpdateUI();
            UpdateMesh();
            Viewer.RefreshTextures();
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
                FLVER2 newFlver = IsFLVERPath(newFlverFilePath) ? FLVER2.Read(newFlverFilePath) :
                    ReadFLVERFromDCXPath(newFlverFilePath, false, false, false);
                if (newFlver == null) return;
                int materialOffset = Flver.Materials.Count;
                int layoutOffset = Flver.BufferLayouts.Count;
                Dictionary<int, int> newFlverToCurrentFlver = new Dictionary<int, int>();
                for (int i = 0; i < newFlver.Bones.Count; ++i)
                {
                    FLVER.Bone attachBone = newFlver.Bones[i];
                    for (int j = 0; j < Flver.Bones.Count; ++j)
                    {
                        if (attachBone.Name != Flver.Bones[j].Name) continue;
                        newFlverToCurrentFlver.Add(i, j);
                        break;
                    }
                }
                foreach (FLVER2.Mesh m in newFlver.Meshes)
                {
                    m.MaterialIndex += materialOffset;
                    foreach (FLVER2.VertexBuffer vb in m.VertexBuffers)
                        vb.LayoutIndex += layoutOffset;
                    foreach (FLVER.Vertex v in m.Vertices.Where(v => Util3D.BoneIndicesToIntArray(v.BoneIndices) != null))
                    {
                        for (int i = 0; i < v.BoneIndices.Length; ++i)
                        {
                            if (newFlverToCurrentFlver.ContainsKey(v.BoneIndices[i])) v.BoneIndices[i] = newFlverToCurrentFlver[v.BoneIndices[i]];
                        }
                    }
                }
                Flver.BufferLayouts = Flver.BufferLayouts.Concat(newFlver.BufferLayouts).ToList();
                Flver.Meshes = Flver.Meshes.Concat(newFlver.Meshes).ToList();
                Flver.Materials = Flver.Materials.Concat(newFlver.Materials).ToList();
                ShowInformationDialog(@"Successfully attached new FLVER to the current one!");
                DeselectAllSelectedThings();
                UpdateUI();
                UpdateMesh();
                Viewer.RefreshTextures();
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
            byte[] newFlverBytes = Flver.Write();
            if (newFlverBytes.SequenceEqual(CurrentFlverBytes)) return true;
            DialogResult result = MessageBox.Show(@"Do you want to save changes to the FLVER before quitting?", @"Warning", MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
            switch (result)
            {
                case DialogResult.Yes:
                    SaveFLVERFile(FlverFilePath);
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
                Position = Flver.Dummies.Count > 0 ? Flver.Dummies[Flver.Dummies.Count - 1].Position : new Vector3(0, 0, 0),
                ReferenceID = -1
            };
            Flver.Dummies.Add(newDummy);
            DeselectAllSelectedThings();
            UpdateUI();
            UpdateMesh();
        }

        private void DummiesTableCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (IsSettingDefaultInfo || !IsTextBoxCell(sender, e.ColumnIndex, e.RowIndex)) return;
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
                            Flver.Dummies[e.RowIndex].ReferenceID = parsed;
                            break;
                        case 2:
                            Flver.Dummies[e.RowIndex].AttachBoneIndex = parsed;
                            break;
                        case 3:
                            Flver.Dummies[e.RowIndex].ParentBoneIndex = parsed;
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
            OpenFileDialog dialog = new OpenFileDialog { Filter = JsonFileFilter };
            if (dialog.ShowDialog() != DialogResult.OK) return;
            try
            {
                UpdateUndoState();
                string jsonText = File.ReadAllText(dialog.FileName);
                switch (type)
                {
                    case 0:
                        Flver.Bones = JsonConvert.DeserializeObject<List<FLVER.Bone>>(jsonText);
                        break;
                    case 1:
                        Flver.Materials = JsonConvert.DeserializeObject<List<FLVER2.Material>>(jsonText);
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
            SaveFileDialog dialog = new SaveFileDialog { Filter = JsonFileFilter };
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
            ExportJSON(Flver.Bones);
        }

        private void ExportMaterialsJSONButtonClicked(object sender, EventArgs e)
        {
            ExportJSON(Flver.Materials);
        }

        private void BrowsePresetsFile(bool materialPresetsFile)
        {
            OpenFileDialog dialog = new OpenFileDialog { Filter = JsonFileFilter, Multiselect = false };
            if (dialog.ShowDialog() != DialogResult.OK) return;
            File.WriteAllText(materialPresetsFile ? MaterialPresetsFilePath : DummyPresetsFilePath, File.ReadAllText(dialog.FileName));
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
            TextureRefreshEnabled = !TextureRefreshEnabled;
            UserConfigJson["TextureRefreshing"] = TextureRefreshEnabled;
            WriteUserConfig();
            if (TextureRefreshEnabled)
            {
                ShowInformationDialog("Texture refreshing is now enabled!");
                Viewer.RefreshTextures();
            }
            else ShowInformationDialog("Texture refreshing is now disabled to help improve performance!");
        }

        private void MergePresets(bool materialPresetsFile)
        {
            OpenFileDialog dialog = new OpenFileDialog { Filter = JsonFileFilter, Multiselect = false };
            if (dialog.ShowDialog() != DialogResult.OK) return;
            Dictionary<object, object> newPresets = new JavaScriptSerializer().Deserialize<Dictionary<object, object>>(File.ReadAllText(dialog.FileName));
            Dictionary<object, object> presets = materialPresetsFile ? MaterialPresets : DummyPresets;
            foreach (KeyValuePair<object, object> preset in newPresets.Where(preset => !presets.ContainsKey(preset.Key)))
                presets.Add(preset.Key, preset.Value);
            File.WriteAllText(materialPresetsFile ? MaterialPresetsFilePath : DummyPresetsFilePath, new JavaScriptSerializer().Serialize(presets));
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
            Vector3 minVector = new Vector3(0, 0, 0);
            Vector3 maxVector = new Vector3(999, 999, 999);
            Flver.Header.BoundingBoxMin = maxVector;
            Flver.Header.BoundingBoxMax = minVector;
            foreach (FLVER2.Mesh mesh in from mesh in Flver.Meshes from vertex in mesh.Vertices select mesh)
            {
                mesh.BoundingBox.Min = maxVector;
                mesh.BoundingBox.Max = minVector;
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
            SolidBrush backColor = new SolidBrush(TabWindowBackColor);
            e.Graphics.FillRectangle(backColor, rec);
            Font fntTab = e.Font;
            for (int i = 0; i < tabWindow.TabPages.Count; i++)
            {
                RectangleF tabTextArea = tabWindow.GetTabRect(i);
                SolidBrush fontColor = new SolidBrush(TabWindowForeColor);
                e.Graphics.DrawString(tabWindow.TabPages[i].Text, fntTab, fontColor, tabTextArea, StrFormat);
            }
        }

        private void MainWindowLoad(object sender, EventArgs e)
        {
            UpdateWindowTitle(FlverFilePath);
            tabWindow.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabWindow.DrawItem += TabWindowDrawItem;
            TopMost = false;
        }

        // TODO: Investigate conflicts with detail overlay textures
        private void ApplyMATBINTexturesButtonClicked(object sender, EventArgs e)
        {
            if (MatBinBndPath == null)
            {
                try
                {
                    string matBinBndPathStr = UserConfigJson["MatBinBndPath"]?.ToString();
                    MatBinBndPath = matBinBndPathStr ?? throw new Exception();
                    File.ReadAllBytes(MatBinBndPath);
                }
                catch
                {
                    MatBinBndPath = null;
                    OpenFileDialog dialog = new OpenFileDialog { Filter = @"MATBIN BND (*.matbinbnd.dcx)|*.matbinbnd.dcx" };
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        MatBinBndPath = dialog.FileName;
                        UserConfigJson["MatBinBndPath"] = MatBinBndPath;
                        WriteUserConfig();
                    }
                    else
                    {
                        return;
                    }
                }
            }
            if (MatBinBnd == null) MatBinBnd = BND4.Read(MatBinBndPath);
            foreach (BinderFile matBinFile in MatBinBnd.Files)
            {
                string rawMaterialFileName = Path.GetFileNameWithoutExtension(Flver.Materials[SelectedMaterialIndex].MTD);
                string rawMatBinFileName = Path.GetFileNameWithoutExtension(matBinFile.Name);
                if (rawMaterialFileName != rawMatBinFileName) continue;
                MATBIN matBin = new MATBIN();
                matBin.Read(new BinaryReaderEx(false, matBinFile.Bytes));
                if (matBin.Samplers.Any(sampler => sampler.Path != ""))
                {
                    Flver.Materials[SelectedMaterialIndex].Textures.Clear();
                    foreach (FLVER2.Texture newTexture in matBin.Samplers.Select(sampler => new FLVER2.Texture { Type = sampler.Type, Path = sampler.Path }))
                        Flver.Materials[SelectedMaterialIndex].Textures.Add(newTexture);
                }
                break;
            }
            UpdateTexturesTable();
        }

        private void DummyThicknessSelectorSelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsSettingDefaultInfo) return;
            DummyThickness = int.Parse(dummyThicknessSelector.Items[dummyThicknessSelector.SelectedIndex].ToString());
            UserConfigJson["DummyThickness"] = DummyThickness;
            WriteUserConfig();
            UpdateMesh();
        }

        private void MaterialsPagePanelsContainerSplitterMoved(object sender, SplitterEventArgs e)
        {
            CurrentMaterialsTableSplitDistance = int.Parse(e.SplitX.ToString());
            UserConfigJson["MaterialsTableViewSplitDistance"] = CurrentMaterialsTableSplitDistance;
            WriteUserConfig();
        }

        private static Vector3 MirrorThing(Vector3 v, int nbi, IReadOnlyList<float> totals)
        {
            v = new Vector3((v.X - (nbi == 0 && !UseWorldOrigin ? totals[0] : 0)) * (nbi == 0 ? -1 : 1),
                (v.Y - (nbi == 1 && !UseWorldOrigin ? totals[1] : 0)) * (nbi == 1 ? -1 : 1),
                (v.Z - (nbi == 2 && !UseWorldOrigin ? totals[2] : 0)) * (nbi == 2 ? -1 : 1));
            v = new Vector3(v.X + (nbi == 0 && !UseWorldOrigin ? totals[0] : 0),
                v.Y + (nbi == 1 && !UseWorldOrigin ? totals[1] : 0),
                v.Z + (nbi == 2 && !UseWorldOrigin ? totals[2] : 0));
            return v;
        }

        private void MirrorMesh(int nbi)
        {
            UpdateUndoState();
            float[] totals = CalculateMeshTotals();
            foreach (FLVER.Vertex v in SelectedMeshIndices.SelectMany(i => Flver.Meshes[i].Vertices))
            {
                v.Position = MirrorThing(v.Position, nbi, totals);
                v.Normal = new Vector3(v.Normal.X * (nbi == 0 ? -1 : 1),
                    v.Normal.Y * (nbi == 1 ? -1 : 1), v.Normal.Z * (nbi == 2 ? -1 : 1));
                v.Tangents[0] = new Vector4(v.Tangents[0].X * (nbi == 0 ? -1 : 1),
                    v.Tangents[0].Y * (nbi == 1 ? -1 : 1), v.Tangents[0].Z * (nbi == 2 ? -1 : 1), 1);
            }
            foreach (FLVER.Dummy d in SelectedDummyIndices.Select(i => Flver.Dummies[i]))
            {
                if (vectorModeCheckbox.Checked) d.Forward = MirrorThing(d.Forward, nbi, totals);
                else d.Position = MirrorThing(d.Position, nbi, totals);
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
            if (FlverFilePath != null) SaveFLVERFile(FlverFilePath);
        }

        private bool CheckAutoSaveInterval(string intervalStr)
        {
            if (!Regex.IsMatch(intervalStr, "^[0-9]*$")) return false;
            try
            {
                int newInterval = int.Parse(intervalStr);
                if (newInterval == 0 || newInterval > 60) return false;
                autoSaveTimer.Interval = newInterval * 60000;
                CurrentAutoSaveInterval = intervalStr;
                UserConfigJson["AutoSaveInterval"] = CurrentAutoSaveInterval;
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
            if (IsSettingDefaultInfo) return;
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
                    SaveFLVERFile(FlverFilePath);
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
                    SetDisplayMaleBody();
                    break;
                case true when e.Shift && e.KeyCode == Keys.F:
                    SetDisplayFemaleBody();
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
                Arguments.Add(filePath);
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
            if (!PromptDeletePreset(sender, ref MaterialPresets)) return;
            UpdateMaterialPresets();
        }

        private void DummyPresetsSelector_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (!PromptDeletePreset(sender, ref DummyPresets)) return;
            UpdateDummyPresets();
        }

        private static void ExportPresetsFile(ref Dictionary<object, object> presets)
        {
            SaveFileDialog dialog = new SaveFileDialog { Filter = JsonFileFilter };
            if (dialog.ShowDialog() != DialogResult.OK) return;
            File.WriteAllText(dialog.FileName, JsonConvert.SerializeObject(presets, Formatting.Indented));
            ShowInformationDialog("Successfully exported presets file!");
        }

        private void ExportMaterialPresetsFileButtonClick(object sender, EventArgs e)
        {
            ExportPresetsFile(ref MaterialPresets);
        }

        private void ExportDummiesPresetFileButtonClick(object sender, EventArgs e)
        {
            ExportPresetsFile(ref DummyPresets);
        }

        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            if (Mono3D.f == null || !IsSnapped) return;
            try
            {
                if (IsSnappedRight)
                {
                    Mono3D.f.Invoke(new MethodInvoker(delegate { Mono3D.f.Left = Right; }));
                    Mono3D.f.Invoke(new MethodInvoker(delegate { Mono3D.f.Top = Top; }));
                }
                else if (IsSnappedBottom) Mono3D.f.Invoke(new MethodInvoker(delegate { Mono3D.f.Top = Bottom; }));
            }
            catch { }
        }

        private void HideAllMeshesButton_MouseClick(object sender, MouseEventArgs e)
        {
            ModifyAllThings(meshTable, 4);
        }

        private void PatreonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(PatreonSupportUri);
        }

        private void PayPalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(PaypalSupportUri);
        }

        private void ToggleDummyIDsVisibilityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AreDummyIdsVisible = !AreDummyIdsVisible;
            UserConfigJson["AreDummyIDsVisible"] = AreDummyIdsVisible;
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
            if (IsSettingDefaultInfo) return;
            IsSettingDefaultInfo = true;
            int prevSelectorIndex = meshTabDataTableSelector.SelectedIndex;
            meshTabDataTableSelector.SelectedIndex = 0;
            ClearSearchResults();
            meshTabDataTableSelector.SelectedIndex = 1;
            ClearSearchResults();
            meshTabDataTableSelector.SelectedIndex = prevSelectorIndex;
            IsSettingDefaultInfo = false;
        }

        private void TabWindow_SelectedIndexChanged(object sender, EventArgs e)
        {
            meshTabDataTableSelector.Visible = tabWindow.SelectedIndex == 2;
            copyrightStr.Focus();
            ClearSearchResults();
        }

        private void MainWindow_SizeChanged(object sender, EventArgs e)
        {
            UserConfigJson["EditorWindowWidth"] = Size.Width;
            UserConfigJson["EditorWindowHeight"] = Size.Height;
            WriteUserConfig();
        }

        private static void ToggleBodyModelDisplay(ref bool dispBodyModel)
        {
            dispBodyModel = !dispBodyModel;
            UpdateMesh();
            ShowInformationDialog(dispBodyModel ? "Body model is now visible!" : "Body model is now hidden!");
        }

        private static void SetDisplayMaleBody()
        {
            DisplayFemaleBody = false;
            ToggleBodyModelDisplay(ref DisplayMaleBody);
        }

        private void DisplayMaleBodyButton_Click(object sender, EventArgs e)
        {
            SetDisplayMaleBody();
        }

        private static void SetDisplayFemaleBody()
        {
            DisplayMaleBody = false;
            ToggleBodyModelDisplay(ref DisplayFemaleBody);
        }

        private void DisplayFemaleBodyButton_Click(object sender, EventArgs e)
        {
            SetDisplayFemaleBody();
        }

        private void MeshTable_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            MeshTableCheckboxSelected(e.RowIndex, e.ColumnIndex);
        }

        private void DummiesTable_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DummiesTableCheckboxSelected(e.RowIndex, e.ColumnIndex);
        }

        private static void AddNewMeshFaceset(FLVER2.Mesh m, FLVER2.FaceSet.FSFlags flags, List<int> vertexIndices)
        {
            FLVER2.FaceSet fs = Generators.GenerateBasicFaceSet();
            fs.Flags = flags;
            fs.Indices = vertexIndices;
            m.FaceSets.Add(fs);
        }

        private void SolveAllMeshLODs()
        {
            UpdateUndoState();
            FLVER2.FaceSet.FSFlags[] faceSetFlags =
            {
                FLVER2.FaceSet.FSFlags.None,
                FLVER2.FaceSet.FSFlags.LodLevel1,
                FLVER2.FaceSet.FSFlags.LodLevel2,
                FLVER2.FaceSet.FSFlags.MotionBlur,
                FLVER2.FaceSet.FSFlags.MotionBlur | FLVER2.FaceSet.FSFlags.LodLevel1,
                FLVER2.FaceSet.FSFlags.MotionBlur | FLVER2.FaceSet.FSFlags.LodLevel2
            };
            foreach (FLVER2.Mesh m in SelectedMeshIndices.Select(i => Flver.Meshes[i]))
            {
                List<int> vertexIndices = m.FaceSets[0].Indices;
                m.FaceSets.Clear();
                foreach (FLVER2.FaceSet.FSFlags flag in faceSetFlags)
                    AddNewMeshFaceset(m, flag, vertexIndices);
            }
            ShowInformationDialog("Created missing LODs from existing mesh data!");
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
                RedoFlverList.Clear();
                CurrentRedoFlverListIndex = -1;
                redoToolStripMenuItem.Enabled = false;
            }
            undoToolStripMenuItem.Enabled = true;
            UndoFlverList.Add(FLVER2.Read(Flver.Write()));
            CurrentUndoFlverListIndex++;
            if (CurrentUndoFlverListIndex <= UndoRedoStackLimit) return;
            UndoFlverList.RemoveAt(0);
            CurrentUndoFlverListIndex--;
        }

        private void UpdateRedoState()
        {
            redoToolStripMenuItem.Enabled = true;
            RedoFlverList.Add(FLVER2.Read(Flver.Write()));
            CurrentRedoFlverListIndex++;
        }

        private void Undo()
        {
            if (CurrentUndoFlverListIndex < 0) return;
            UpdateRedoState();
            Flver = FLVER2.Read(UndoFlverList[CurrentUndoFlverListIndex].Write());
            UndoFlverList.RemoveAt(CurrentUndoFlverListIndex);
            CurrentUndoFlverListIndex--;
            UpdateUI();
            UpdateMesh();
            if (CurrentUndoFlverListIndex != -1) return;
            undoToolStripMenuItem.Enabled = false;
        }

        private void Redo()
        {
            if (CurrentRedoFlverListIndex < 0) return;
            UpdateUndoState(false);
            Flver = FLVER2.Read(RedoFlverList[CurrentRedoFlverListIndex].Write());
            RedoFlverList.RemoveAt(CurrentRedoFlverListIndex);
            CurrentRedoFlverListIndex--;
            UpdateUI();
            UpdateMesh();
            if (CurrentRedoFlverListIndex != -1) return;
            redoToolStripMenuItem.Enabled = false;
        }

        private void ClearUndoRedoStates()
        {
            UndoFlverList.Clear();
            RedoFlverList.Clear();
            CurrentUndoFlverListIndex = -1;
            CurrentRedoFlverListIndex = -1;
            undoToolStripMenuItem.Enabled = false;
            redoToolStripMenuItem.Enabled = false;
        }

        private void FlipYZAxisCheckboxChanged(object sender, EventArgs e)
        {
            UpdateUndoState();
            foreach (FLVER.Vertex v in SelectedMeshIndices.SelectMany(i => Flver.Meshes[i].Vertices))
            {
                v.Position = new Vector3(v.Position.X, v.Position.Z, v.Position.Y);
                v.Normal = new Vector3(v.Normal.X, v.Normal.Z, v.Normal.Y);
                v.Tangents[0] = new Vector4(v.Tangents[0].X, v.Tangents[0].Z, v.Tangents[0].Y, 1);
            }
            foreach (FLVER.Dummy d in SelectedDummyIndices.Select(i => Flver.Dummies[i]))
                d.Position = new Vector3(d.Position.X, d.Position.Z, d.Position.Y);
            UpdateMesh();
            ShowInformationDialog("Successfully flipped the YZ axis!");
        }

        private void CenterMeshToWorld(int nbi)
        {
            UpdateUndoState();
            float[] totals = CalculateMeshTotals();
            foreach (FLVER.Vertex v in SelectedMeshIndices.SelectMany(i => Flver.Meshes[i].Vertices))
                TranslateThing(v, -totals[nbi], nbi);
            foreach (FLVER.Dummy d in SelectedDummyIndices.Select(i => Flver.Dummies[i]))
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
            Flver.BufferLayouts.Add(Generators.GenerateDefaultBufferLayout());
            foreach (FLVER2.Mesh m in Flver.Meshes)
            {
                m.BoundingBox.Max = new Vector3(1, 1, 1);
                m.BoundingBox.Min = new Vector3(-1, -1, -1);
                m.BoundingBox.Unk = new Vector3();
                m.DefaultBoneIndex = 0;
                m.Dynamic = 1;
                int[] varray = m.FaceSets[0].Indices.ToArray();
                m.FaceSets = new List<FLVER2.FaceSet>();
                for (int i = 0; i < m.Vertices.Count; i++)
                {
                    FLVER.Vertex vit = m.Vertices[i];
                    m.Vertices[i] = Generators.GenerateNewFlverVertexUsingNumerics(new Vector3(vit.Position.X, vit.Position.Y, vit.Position.Z), vit.Normal, vit.Tangents,
                        vit.Bitangent, vit.UVs, 1);
                    m.Vertices[i].BoneIndices = vit.BoneIndices;
                    m.Vertices[i].BoneWeights = vit.BoneWeights;
                }
                m.FaceSets.Add(Generators.GenerateBasicFaceSet());
                m.FaceSets[0].Indices = varray.ToList();
                m.FaceSets[0].CullBackfaces = false;
            }
            ShowInformationDialog("Successfully reset all mesh!");
        }

        private void ResetAllMeshButton_Click(object sender, EventArgs e)
        {
            ResetAllMesh();
        }

        private void ToggleUseWorldOriginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UseWorldOrigin = !UseWorldOrigin;
            UserConfigJson["UseWorldOrigin"] = UseWorldOrigin;
            WriteUserConfig();
            ShowInformationDialog("Successfully toggled using the world origin for transformations!");
        }

        private void VectorModeCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableExtraModifierOptions();
        }

        private void ToggleDuplicateMaterialsOnMeshImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleDuplicateMaterialsOnMeshImport = !ToggleDuplicateMaterialsOnMeshImport;
            UserConfigJson["DupeMatOnMeshImport"] = ToggleDuplicateMaterialsOnMeshImport;
            WriteUserConfig();
            ShowInformationDialog(ToggleDuplicateMaterialsOnMeshImport ? "Automatic duplication of materials on mesh import is now enabled!"
                : "Automatic duplication of materials on mesh import is now disabled!");
        }
    }
}