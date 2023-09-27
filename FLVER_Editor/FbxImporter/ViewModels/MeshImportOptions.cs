using SoulsAssetPipeline.FLVERImporting;

namespace FLVER_Editor.FbxImporter.ViewModels;

public record MeshImportOptions
{
    public bool CreateDefaultBone;

    public bool MirrorZ = true;

    public string MTD = null!;

    public FLVER2MaterialInfoBank MaterialInfoBank = null!;

    public bool IsStatic;

    public MeshImportOptions(bool createDefaultBone, string mtd, FLVER2MaterialInfoBank infoBank, bool isStatic)
    {
        CreateDefaultBone = createDefaultBone;
        MTD = mtd;
        MaterialInfoBank = infoBank;
        IsStatic = isStatic;
    }
}