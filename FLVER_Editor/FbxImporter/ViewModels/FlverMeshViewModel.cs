using SoulsFormats;

namespace FLVER_Editor.FbxImporter.ViewModels;

public class FlverMeshViewModel
{
    public FlverMeshViewModel(FLVER2 flver, FLVER2.Mesh mesh)
    {
        Mesh = mesh;
        Material = flver.Materials[mesh.MaterialIndex];
        if (Material.GXIndex != -1)
        {
            GxList = flver.GXLists[Material.GXIndex];
        }
        Name = Material.Name;
    }

    public FlverMeshViewModel(FLVER2.Mesh mesh, FLVER2.Material material, FLVER2.GXList gxList)
    {
        Mesh = mesh;
        Material = material;
        GxList = gxList;
        Name = Material.Name;
    }

    public FLVER2.Mesh Mesh { get; }

    public FLVER2.Material Material { get; }

    public FLVER2.GXList? GxList { get; }

    public string Name { get; set; }
}