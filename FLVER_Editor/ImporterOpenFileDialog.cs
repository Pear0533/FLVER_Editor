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
        mirrorXCheckbox.Checked = false;
        skinnedMeshCheckbox.Checked = false;
        meshSelector.Enabled = enabled;
        modifyAllMeshesCheckbox.Enabled = enabled;
        mtdSelector.Enabled = enabled;
        createDefaultBoneCheckbox.Enabled = enabled;
        mirrorXCheckbox.Enabled = enabled;
        skinnedMeshCheckbox.Enabled = enabled;
        if (clearMeshes) Meshes.Clear();
        meshSelector.Items.Clear();
        mtdSelector.Items.Clear();
        meshSelector.ResetText();
        mtdSelector.ResetText();
    }

    private void ModifyMesh(Action<KeyValuePair<FbxMeshDataViewModel, MeshImportOptions>> action)
    {
        if (modifyAllMeshesCheckbox.Checked) Meshes.ToList().ForEach(action);
        else action(GetSelectedMesh());
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
            HasImportedModel = NewImporter.ImportFbxAsync(this, e);
            if (!HasImportedModel) return;
            PopulateMeshSelector();
            PopulateMTDSelector();
        };
        meshSelector.SelectedIndexChanged += (_, _) =>
        {
            KeyValuePair<FbxMeshDataViewModel, MeshImportOptions> selectedMesh = GetSelectedMesh();
            mtdSelector.SelectedItem = selectedMesh.Value.MTD;
            createDefaultBoneCheckbox.Checked = selectedMesh.Value.CreateDefaultBone;
            mirrorXCheckbox.Checked = selectedMesh.Value.MirrorX;
            skinnedMeshCheckbox.Checked = selectedMesh.Value.IsSkinned;
        };
        mtdSelector.SelectedIndexChanged += (_, _) => { ModifyMesh(i => i.Value.MTD = mtdSelector.SelectedItem.ToString() ?? ""); };
        createDefaultBoneCheckbox.CheckedChanged += (_, _) => { ModifyMesh(i => i.Value.CreateDefaultBone = createDefaultBoneCheckbox.Checked); };
        mirrorXCheckbox.CheckedChanged += (_, _) => { ModifyMesh(i => i.Value.MirrorX = mirrorXCheckbox.Checked); };
        skinnedMeshCheckbox.CheckedChanged += (_, _) => { ModifyMesh(i => i.Value.IsSkinned = skinnedMeshCheckbox.Checked); };
    }

    protected override void OnPaint(PaintEventArgs pe)
    {
        base.OnPaint(pe);
    }
}