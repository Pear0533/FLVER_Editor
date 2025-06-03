// using FLVER_Editor.FbxImporter.ViewModels;
using SoulsFormats;

namespace FLVER_Editor.Actions;

public class MaterialsTableOkAction : TransformAction
{
    private readonly Dictionary<int, FLVER2.Material> deletedMaterials = new();
    private readonly FLVER2 flver;
    private readonly List<FLVER2.Material> materialsToDelete;
    private readonly List<FLVER2.Material> materialsToReplace;
    private readonly FLVER2.Material? newMaterial;
    private readonly string newMaterialMTD;
    private readonly Action<bool>? refresher;
    private readonly Dictionary<int, FLVER2.Material> replacedMaterials = new();

    public MaterialsTableOkAction(FLVER2 flver, List<FLVER2.Material> materialsToReplace, List<FLVER2.Material> materialsToDelete, FLVER2.Material? newMaterial,
        string newMaterialMTD, Action<bool>? refresher = null)
    {
        this.flver = flver;
        this.materialsToReplace = materialsToReplace;
        this.materialsToDelete = materialsToDelete;
        this.newMaterial = newMaterial;
        this.newMaterialMTD = newMaterialMTD;
        this.refresher = refresher;
    }

    public override void Execute()
    {
        deletedMaterials.Clear();
        replacedMaterials.Clear();
        bool displayMissingPresetDialog = false;
        foreach (FLVER2.Material material in materialsToReplace)
        {
            int materialIndex = flver.Materials.IndexOf(material);
            if (Program.MTDs.Contains(newMaterialMTD))
            {
                flver.Materials[materialIndex].MTD = newMaterialMTD;
                // TODO: Move this to the method that fixes buffer layouts...
                flver.Materials[materialIndex].Textures = new List<FLVER2.Texture>(Program.MaterialInfoBank.MaterialDefs[newMaterialMTD].TextureChannels
                    .Values.Select(x => new FLVER2.Texture { Type = x }));
            }
            else
            {
                if (newMaterial == null)
                {
                    displayMissingPresetDialog = true;
                    break;
                }
                string previousMaterialName = flver.Materials[materialIndex].Name;
                replacedMaterials.Add(materialIndex, flver.Materials[materialIndex]);
                flver.Materials[materialIndex] = newMaterial;
                flver.Materials[materialIndex].Name = previousMaterialName;
            }
        }
        foreach (FLVER2.Material material in materialsToDelete)
        {
            if (flver.Materials.Count - deletedMaterials.Count == 1)
            {
                break;
            }
            int materialIndex = flver.Materials.IndexOf(material);
            deletedMaterials.Add(materialIndex, material);
        }
        foreach (FLVER2.Material material in materialsToDelete)
        {
            if (flver.Materials.Count <= 1)
            {
                break;
            }
            int materialIndex = flver.Materials.IndexOf(material);
            flver.Materials.Remove(material);
            foreach (FLVER2.Mesh mesh in flver.Meshes.Where(mesh => mesh.MaterialIndex >= materialIndex))
                mesh.MaterialIndex--;
        }
        // FbxMeshDataViewModel.FixBufferLayouts();
        refresher?.Invoke(displayMissingPresetDialog);
    }

    public override void Undo()
    {
        foreach (KeyValuePair<int, FLVER2.Material> row in deletedMaterials.OrderBy(x => x.Key))
        {
            flver.Materials.Insert(row.Key, row.Value);
            foreach (FLVER2.Mesh mesh in flver.Meshes.Where(mesh => mesh.MaterialIndex >= row.Key))
                mesh.MaterialIndex++;
        }
        foreach (KeyValuePair<int, FLVER2.Material> row in replacedMaterials.OrderBy(x => x.Key))
        {
            flver.Materials[row.Key] = row.Value;
        }

        // TODO: Double check
        // FbxMeshDataViewModel.FixBufferLayouts();
        refresher?.Invoke(false);
    }
}