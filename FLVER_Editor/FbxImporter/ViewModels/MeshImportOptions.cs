using SoulsAssetPipeline.FLVERImporting;

namespace FLVER_Editor.FbxImporter.ViewModels
{
    public class MeshImportOptions
    {
        public bool CreateDefaultBone;

        public string MTD;

        public FLVER2MaterialInfoBank MaterialInfoBank;

        public bool IsStatic;

        public MeshImportOptions(bool createDefaultBone, string mtd, FLVER2MaterialInfoBank infoBank, bool isStatic)
        {
            CreateDefaultBone = createDefaultBone;
            MTD = mtd;
            MaterialInfoBank = infoBank;
            IsStatic = isStatic;
        }
    }
}