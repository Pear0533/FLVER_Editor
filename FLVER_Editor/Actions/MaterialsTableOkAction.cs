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
    private readonly DataGridView datagrid;
    private readonly FLVER2.Material? newMaterial;
    private readonly Action<bool>? refresher;

    public MaterialsTableOkAction(FLVER2 flver, DataGridView datagrid, FLVER2.Material? newMaterial, Action<bool>? refresher = null)
    {
        this.flver = flver;
        this.datagrid = datagrid;
        this.newMaterial = newMaterial;
        this.refresher = refresher;

    }

    public override void Execute()
    {
        deletedMaterials.Clear();
        replacedMaterials.Clear();

        var displayMissingPresetDialog = false;


        foreach (DataGridViewRow row in datagrid.Rows)
        {
            if (!(bool)row.Cells[MainWindow.MaterialApplyPresetCbIndex].Value) continue;
            if (newMaterial == null)
            {
                displayMissingPresetDialog = true;
                break;
            }

            string previousMaterialName = flver.Materials[row.Index].Name;

            replacedMaterials.Add(row.Index, flver.Materials[row.Index]);
            flver.Materials[row.Index] = newMaterial;
            flver.Materials[row.Index].Name = previousMaterialName;
        }

        for (int i = flver.Materials.Count - 1; i >= 0; --i)
        {
            if (!(bool)datagrid.Rows[i].Cells[MainWindow.MaterialDeleteCbIndex].Value || flver.Materials.Count <= 1) continue;
            deletedMaterials.Add(i, flver.Materials[i]);
            flver.Materials.RemoveAt(i);

            foreach (FLVER2.Mesh mesh in flver.Meshes.Where(mesh => mesh.MaterialIndex >= i))
                mesh.MaterialIndex--;
        }

        refresher?.Invoke(displayMissingPresetDialog);
    }

    public override void Undo()
    {
        foreach (var row in deletedMaterials)
        {
            flver.Materials.Insert(row.Key, row.Value);

            foreach (FLVER2.Mesh mesh in flver.Meshes.Where(mesh => mesh.MaterialIndex >= row.Key))
                mesh.MaterialIndex++;
        }

        foreach (var row in replacedMaterials)
        {
            flver.Materials[row.Key] = row.Value;
        }



        refresher?.Invoke(false);
    }
}
