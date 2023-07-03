using SoulsAssetPipeline.FLVERImporting;

namespace FLVER_Editor.FbxImporter.ViewModels
{
    public class MeshImportOptions
    {
        public bool CreateDefaultBone;

        public bool MirrorX;

        public string MTD;

        public FLVER2MaterialInfoBank MaterialInfoBank;

        public bool IsSkinned;

        public MeshImportOptions(bool createDefaultBone, bool mirrorX, string mtd, FLVER2MaterialInfoBank infoBank, bool isSkinned)
        {
            CreateDefaultBone = createDefaultBone;
            MirrorX = mirrorX;
            MTD = mtd;
            MaterialInfoBank = infoBank;
            IsSkinned = isSkinned;
        }
    }
}