using SoulsFormats;

namespace FLVER_Editor.FbxImporter.ViewModels;

public class FlverMeshViewModel
{
    public FlverMeshViewModel(FLVER2.Mesh mesh, FLVER2.Material material, FLVER2.GXList gxList)
    {
        Mesh = mesh;
        Material = material;
        GxList = gxList;
        Name = Material.Name;
    }

    public FLVER2.Mesh Mesh { get; }

    public FLVER2.Material Material { get; }

    public FLVER2.GXList GxList { get; }

    public string Name { get; set; }
}