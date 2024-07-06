using FbxDataExtractor;
using FLVER_Editor.FbxImporter.ViewModels;
using SoulsFormats;
using Win32Types;
using static FLVER_Editor.Program;

namespace FLVER_Editor;

public static class Importer
{
    public static MeshImportOptions GetDefaultImportOptions()
    {
        return new MeshImportOptions(false, MTDs[DefaultMTDIndex], MaterialInfoBank, WeightingMode.Skin);
    }

    private static void ShowImportErrorDialog(string fbxPath, Exception e)
    {
        MainWindow.ShowErrorDialog($"{Path.GetFileName(fbxPath)} could not be read:\n\nError: {e.Message}");
    }

    public static bool ImportFbxAsync(ImporterOpenFileDialog dialog, string fbxPath)
    {
        // TODO: Implement exception handling for models which have no binormals
        try
        {
            FbxMeshData.Import(fbxPath).ForEach(x => dialog.Meshes.Add(new FbxMeshDataViewModel(x), GetDefaultImportOptions()));
        }
        catch (Exception e)
        {
            ShowImportErrorDialog(fbxPath, e);
            return false;
        }
        return true;
    }

    public static bool ImportFbxAsync(FLVER2 flver, string fbxPath)
    {
        List<FbxMeshDataViewModel> meshes;
        try
        {
            meshes = FbxMeshData.Import(fbxPath).Select(x => new FbxMeshDataViewModel(x)).ToList();
        }
        catch (Exception e)
        {
            ShowImportErrorDialog(fbxPath, e);
            return false;
        }
        meshes.ForEach(i => i.ToFlverMesh(flver, GetDefaultImportOptions()));
        return true;
    }

    private static void AddImportDialogMeshesToFlver(FLVER2 flver, Dictionary<FbxMeshDataViewModel, MeshImportOptions> meshes)
    {
        MainWindow.Instance?.UpdateUndoState();
        meshes.ToList().ForEach(i => i.Key.ToFlverMesh(flver, i.Value));
    }

    public static bool ImportFbxWithDialogAsync(FLVER2 flver)
    {
        ImporterOpenFileDialog dialog = new()
        {
            FileDlgFilter = @"FBX File|*.fbx",
            FileDlgType = FileDialogType.OpenFileDlg,
            FileDlgCaption = "Open FBX File"
        };
        if (dialog.ShowDialog() != DialogResult.OK || !dialog.HasImportedModel) return false;
        AddImportDialogMeshesToFlver(flver, dialog.Meshes);
        MainWindow.ShowInformationDialog($"Successfully imported {Path.GetFileName(dialog.FileDlgFileName)}!");
        return true;
    }
}