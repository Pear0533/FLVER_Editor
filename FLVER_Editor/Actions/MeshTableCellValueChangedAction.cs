using SoulsFormats;

namespace FLVER_Editor.Actions;

public class MeshTableCellValueChangedAction : TransformAction
{
    private readonly FLVER2 flver;
    private readonly string oldMaterialName;
    private readonly string newMaterialName;
    private readonly int row;
    private readonly Action refresher;

    public MeshTableCellValueChangedAction(FLVER2 flver, string oldMaterialName, string newMaterialName, int row, Action refresher)
    {
        this.flver = flver;
        this.oldMaterialName = oldMaterialName;
        this.newMaterialName = newMaterialName;
        this.row = row;
        this.refresher = refresher;
    }

    public override void Execute()
    {
        FLVER2.Mesh? mesh = flver.Meshes.ElementAtOrDefault(row);
        if (mesh != null)
        {
            FLVER2.Material? material = flver.Materials.ElementAtOrDefault(mesh.MaterialIndex);
            if (material != null) material.Name = newMaterialName;
        }
        refresher.Invoke();
    }

    public override void Undo()
    {
        FLVER2.Mesh? mesh = flver.Meshes.ElementAtOrDefault(row);
        if (mesh != null)
        {
            FLVER2.Material? material = flver.Materials.ElementAtOrDefault(mesh.MaterialIndex);
            if (material != null) material.Name = oldMaterialName;
        }
        refresher.Invoke();
    }
}