using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor.Actions;

public class MaterialsTableOkAction : TransformAction
{
    private readonly Dictionary<int, FLVER2.Material> replacedMaterials = new();
    private readonly Dictionary<int, FLVER2.Material> deletedMaterials = new();
    private readonly FLVER2 flver;
    private readonly List<FLVER2.Material> materialsToReplace;
    private readonly List<FLVER2.Material> materialsToDelete;
    private readonly FLVER2.Material? newMaterial;
    private readonly Action<bool>? refresher;

    public MaterialsTableOkAction(FLVER2 flver, List<FLVER2.Material> materialsToReplace, List<FLVER2.Material> materialsToDelete, FLVER2.Material? newMaterial, Action<bool>? refresher = null)
    {
        this.flver = flver;
        this.materialsToReplace = materialsToReplace;
        this.materialsToDelete = materialsToDelete;
        this.newMaterial = newMaterial;
        this.refresher = refresher;

    }

    public override void Execute()
    {
        deletedMaterials.Clear();
        replacedMaterials.Clear();

        var displayMissingPresetDialog = false;

        foreach (var material in materialsToReplace)
        {
            if (newMaterial == null)
            {
                displayMissingPresetDialog = true;
                break;
            }

            var materialIndex = flver.Materials.IndexOf(material);
            string previousMaterialName = flver.Materials[materialIndex].Name;

            replacedMaterials.Add(materialIndex, flver.Materials[materialIndex]);
            flver.Materials[materialIndex] = newMaterial;
            flver.Materials[materialIndex].Name = previousMaterialName;
        }

        foreach (FLVER2.Material material in materialsToDelete)
        {
            var materialIndex = flver.Materials.IndexOf(material);
            deletedMaterials.Add(materialIndex, material);
        }

        foreach (var material in materialsToDelete)
        {
            if (flver.Materials.Count <= 1) break;

            var materialIndex = flver.Materials.IndexOf(material);
            flver.Materials.Remove(material);

                foreach (FLVER2.Mesh mesh in flver.Meshes.Where(mesh => mesh.MaterialIndex >= materialIndex))
                mesh.MaterialIndex--;
        }

        refresher?.Invoke(displayMissingPresetDialog);
    }

    public override void Undo()
    {
        foreach (var row in deletedMaterials.OrderBy(x => x.Key))
        {
            flver.Materials.Insert(row.Key, row.Value);

            foreach (FLVER2.Mesh mesh in flver.Meshes.Where(mesh => mesh.MaterialIndex >= row.Key))
                mesh.MaterialIndex++;
        }

        foreach (var row in replacedMaterials.OrderBy(x => x.Key))
        {
            flver.Materials[row.Key] = row.Value;
        }

        refresher?.Invoke(false);
    }
}
