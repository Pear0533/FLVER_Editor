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

    private readonly DataGridView datagrid;
    private readonly FLVER2.Material? newMaterial;
    private readonly Action<bool>? refresher;

    public MaterialsTableOkAction(DataGridView datagrid, FLVER2.Material? newMaterial, Action<bool>? refresher = null)
    {
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

            string previousMaterialName = MainWindow.Flver.Materials[row.Index].Name;

            replacedMaterials.Add(row.Index, MainWindow.Flver.Materials[row.Index]);
            MainWindow.Flver.Materials[row.Index] = newMaterial;
            MainWindow.Flver.Materials[row.Index].Name = previousMaterialName;
        }

        for (int i = MainWindow.Flver.Materials.Count - 1; i >= 0; --i)
        {
            if (!(bool)datagrid.Rows[i].Cells[MainWindow.MaterialDeleteCbIndex].Value || MainWindow.Flver.Materials.Count <= 1) continue;
            deletedMaterials.Add(i, MainWindow.Flver.Materials[i]);
            MainWindow.Flver.Materials.RemoveAt(i);

            foreach (FLVER2.Mesh mesh in MainWindow.Flver.Meshes.Where(mesh => mesh.MaterialIndex >= i))
                mesh.MaterialIndex--;
        }

        refresher?.Invoke(displayMissingPresetDialog);
    }

    public override void Undo()
    {
        foreach (var row in deletedMaterials)
        {
            MainWindow.Flver.Materials.Insert(row.Key, row.Value);

            foreach (FLVER2.Mesh mesh in MainWindow.Flver.Meshes.Where(mesh => mesh.MaterialIndex >= row.Key))
                mesh.MaterialIndex++;
        }

        foreach (var row in replacedMaterials)
        {
            MainWindow.Flver.Materials[row.Key] = row.Value;
        }



        refresher?.Invoke(false);
    }
}
