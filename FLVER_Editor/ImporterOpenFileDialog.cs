using System.Text.RegularExpressions;
using FileDialogExtenders;
using FLVER_Editor.FbxImporter.ViewModels;
using static FLVER_Editor.Program;

namespace FLVER_Editor;

public partial class ImporterOpenFileDialog : FileDialogControlBase
{
    public readonly Dictionary<FbxMeshDataViewModel, MeshImportOptions> Meshes = new();
    public bool HasImportedModel;

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
        mtdSelector.SelectedIndex = 0;
    }

    private KeyValuePair<FbxMeshDataViewModel, MeshImportOptions> GetSelectedMesh()
    {
        return Meshes.FirstOrDefault(i => i.Key.Name == meshSelector.SelectedItem.ToString());
    }

    private void ResetImportOptionsControls(bool enabled, bool clearMeshes = true)
    {
        createDefaultBoneCheckbox.Checked = false;
        boneWeightsMessage.Visible = false;
        meshSelector.Enabled = enabled;
        affectAllMeshesCheckbox.Enabled = enabled;
        mtdSelector.Enabled = enabled;
        weightingModeSelector.Enabled = enabled;
        createDefaultBoneCheckbox.Enabled = enabled;
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
            KeyValuePair<FbxMeshDataViewModel, MeshImportOptions> selectedMesh = GetSelectedMesh();
            mtdSelector.SelectedItem = selectedMesh.Value.MTD;
            createDefaultBoneCheckbox.Checked = selectedMesh.Value.CreateDefaultBone;
            weightingModeSelector.SelectedItem = selectedMesh.Value.Weighting;
        };
        mtdSelector.SelectedIndexChanged += (_, _) =>
        {
            ModifyMesh(i => i.Value.MTD = mtdSelector.SelectedItem.ToString() ?? "");
            AssertBoneWeightsMessageVisibility();
        };
        createDefaultBoneCheckbox.CheckedChanged += (_, _) => { ModifyMesh(i => i.Value.CreateDefaultBone = createDefaultBoneCheckbox.Checked); };
        weightingModeSelector.SelectedIndexChanged += (_, _) =>
        {
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
}