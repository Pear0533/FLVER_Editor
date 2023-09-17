using SoulsAssetPipeline.FLVERImporting;

namespace FLVER_Editor.FbxImporter.ViewModels
{
    public class MeshImportOptions
    {
        public bool CreateDefaultBone;

        public bool MirrorZ;

        public string MTD;

        public FLVER2MaterialInfoBank MaterialInfoBank;

        public bool IsStatic;

        public MeshImportOptions(bool createDefaultBone, bool mirrorZ, string mtd, FLVER2MaterialInfoBank infoBank, bool isStatic)
        {
            CreateDefaultBone = createDefaultBone;
            MirrorZ = mirrorZ;
            MTD = mtd;
            MaterialInfoBank = infoBank;
            IsStatic = isStatic;
        }
    }
}