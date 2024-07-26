using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using FileDialogExtenders;
using FLVER_Editor.FbxImporter.ViewModels;
using static FLVER_Editor.Program;

namespace FLVER_Editor;

public partial class ImporterOpenFileDialog : FileDialogControlBase
{
    public readonly Dictionary<FbxMeshDataViewModel, MeshImportOptions> Meshes = new();
    public bool HasImportedModel;

    const int WM_PAINT = 15;
    const int WM_ERASEBKGND = 20;

    [DllImport("user32.dll")]
    static extern IntPtr GetWindowDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    static extern IntPtr ReleaseWindowDC(IntPtr hWnd);

    public ImporterOpenFileDialog()
    {
        InitializeComponent();
        AssignEventHandlers();
        ResetImportOptionsControls(false);
    }

    private void PopulateWeightingModeSelector()
    {
        foreach (WeightingMode weightingMode in WeightingMode.Values)
            weightingModeSelector.Items.Add(weightingMode.Name);
        weightingModeSelector.SelectedIndex = 0;
    }

    private void PopulateMeshSelector()
    {
        foreach (KeyValuePair<FbxMeshDataViewModel, MeshImportOptions> mesh in Meshes)
            meshSelector.Items.Add(mesh.Key.Name);
        meshSelector.SelectedIndex = 0;
    }

    private void PopulateMTDSelector()
    {
        foreach (string material in MTDs)
            mtdSelector.Items.Add(material);
        // TODO: Extend this to work for other games aside from just ER... (Pear)
        var default_mtd_index =
            mtdSelector.SelectedIndex = MTDs.FindIndex(i => i == "c[amsn]_e.matxml");
    }

    private KeyValuePair<FbxMeshDataViewModel, MeshImportOptions> GetSelectedMesh()
    {
        return Meshes.FirstOrDefault(i => i.Key.Name == meshSelector.SelectedItem.ToString());
    }

    private void ResetImportOptionsControls(bool enabled, bool clearMeshes = true)
    {
        boneWeightsMessage.Visible = false;
        meshSelector.Enabled = enabled;
        affectAllMeshesCheckbox.Enabled = enabled;
        mtdSelector.Enabled = enabled;
        weightingModeSelector.Enabled = enabled;
        autoAssignMtdCheckbox.Enabled = enabled;
        if (clearMeshes) Meshes.Clear();
        meshSelector.Items.Clear();
        mtdSelector.Items.Clear();
        meshSelector.ResetText();
        mtdSelector.ResetText();
    }

    private void ModifyMesh(Action<KeyValuePair<FbxMeshDataViewModel, MeshImportOptions>> action)
    {
        if (affectAllMeshesCheckbox.Checked) Meshes.ToList().ForEach(action);
        else action(GetSelectedMesh());
    }

    private void AssertBoneWeightsMessageVisibility()
    {
        WeightingMode mode = WeightingMode.Convert(weightingModeSelector.SelectedItem?.ToString());
        bool doesSupportBoneWeights = FbxMeshDataViewModel.DoesSupportBoneWeights(mtdSelector.SelectedItem?.ToString(), mode);
        bool meshHasBoneWeights = GetSelectedMesh().Key.Data.VertexData.All(i => i.BoneWeights.Any(x => x != 0));
        switch (mode == WeightingMode.Static)
        {
            case false when !doesSupportBoneWeights:
                boneWeightsMessage.Visible = true;
                boneWeightsMessage.Text = @"Warning: The selected MTD does not support bone weights!";
                break;
            case false when !meshHasBoneWeights:
                boneWeightsMessage.Visible = true;
                boneWeightsMessage.Text = @"Warning: The selected mesh does not have bone weights!";
                break;
            default:
                boneWeightsMessage.Visible = false;
                break;
        }
    }

    private void ApplyCurrentMeshOptions()
    {
        KeyValuePair<FbxMeshDataViewModel, MeshImportOptions> selectedMesh = GetSelectedMesh();
        mtdSelector.SelectedItem = selectedMesh.Value.MTD;
        weightingModeSelector.SelectedItem = selectedMesh.Value.Weighting;
    }

    private void AssignEventHandlers()
    {
        // TODO: Allow for non-triangulated mesh to be imported
        EventFileNameChanged += (_, e) =>
        {
            if (!File.Exists(e))
            {
                ResetImportOptionsControls(false);
                return;
            }
            ResetImportOptionsControls(true);
            HasImportedModel = Importer.ImportFbxAsync(this, e);
            if (!HasImportedModel) return;
            PopulateMeshSelector();
            PopulateMTDSelector();
            PopulateWeightingModeSelector();
        };
        meshSelector.SelectedIndexChanged += (_, _) =>
        {
            ApplyCurrentMeshOptions();
            KeyValuePair<FbxMeshDataViewModel, MeshImportOptions> selectedMesh = GetSelectedMesh();
            mtdSelector.SelectedItem = selectedMesh.Value.MTD;
            weightingModeSelector.SelectedItem = selectedMesh.Value.Weighting;
        };
        mtdSelector.SelectedIndexChanged += (_, _) =>
        {
            if (mtdSelector.SelectedItem == null) return;
            ModifyMesh(i => i.Value.MTD = mtdSelector.SelectedItem.ToString() ?? "");
            AssertBoneWeightsMessageVisibility();
        };
        weightingModeSelector.SelectedIndexChanged += (_, _) =>
        {
            if (weightingModeSelector.SelectedItem == null) return;
            ModifyMesh(i => i.Value.Weighting = WeightingMode.Convert(weightingModeSelector.SelectedItem.ToString()!));
            AssertBoneWeightsMessageVisibility();
        };
    }

    protected override void OnPaint(PaintEventArgs pe)
    {
        base.OnPaint(pe);
    }

    private static string ExtractAndValidateMaterialName(string meshName)
    {
        Regex regex = new(@"-| ([^-|]+)\.(mtd|matxml)$");
        Match match = regex.Match(meshName);
        if (!match.Success) return "";
        string name = match.Groups[1].Value.Trim();
        string extension = match.Groups[2].Value;
        return extension is "mtd" or "matxml" ? $"{name}.{extension}" : "";
    }

    // TODO: Make this a property so that it can affect individual meshes
    private void AutoAssignMtdCheckbox_CheckedChanged(object sender, EventArgs e)
    {
        if (autoAssignMtdCheckbox.Checked)
        {
            mtdSelector.Enabled = false;
            foreach (KeyValuePair<FbxMeshDataViewModel, MeshImportOptions> mesh in Meshes)
            {
                string mtdValue = ExtractAndValidateMaterialName(mesh.Key.Name.ToLower());
                int mtdIndex = MTDs.IndexOf(mtdValue);
                if (mtdIndex != -1) mesh.Value.MTD = mtdValue;
            }
        }
        else mtdSelector.Enabled = true;
    }

    private void boneWeightsMessage_Click(object sender, EventArgs e) { }

    private void affectAllMeshesCheckbox_CheckedChanged(object sender, EventArgs e)
    {
        if (sender is CheckBox box)
        {
            meshSelector.Enabled = !box.Checked;
            if (!box.Checked)
            {
                ApplyCurrentMeshOptions();
                return;
            }
            if (Meshes.Select(x => x.Value.MTD).Distinct().Count() != 1)
            {
                mtdSelector.SelectedIndex = -1;
            }
            if (Meshes.Select(x => x.Value.Weighting.Name).Distinct().Count() != 1)
            {
                weightingModeSelector.SelectedIndex = -1;
            }
        }
    }

    private void label1_Click(object sender, EventArgs e) { }
}