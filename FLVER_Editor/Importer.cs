using FbxDataExtractor;
using FLVER_Editor.Actions;
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

    public static bool ImportFbxAsync(FLVER2 flver, string fbxPath, Action refresher)
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

        MeshImportOptions meshImport = GetDefaultImportOptions();
        Dictionary<FbxMeshDataViewModel, MeshImportOptions> meshData = new();
        meshes.ForEach(x => meshData.Add(x, meshImport));

        ImportMeshesToFlverAction action = new(flver, meshData, refresher);
        ActionManager.Apply(action);

        return true;
    }

    private static void AddImportDialogMeshesToFlver(FLVER2 flver, Dictionary<FbxMeshDataViewModel, MeshImportOptions> meshes, Action refresher)
    {
        ImportMeshesToFlverAction action = new(flver, meshes, refresher);
        ActionManager.Apply(action);
    }

    public static bool ImportFbxWithDialogAsync(FLVER2 flver, Action refresher)
    {
        ImporterOpenFileDialog dialog = new()
        {
            FileDlgFilter = @"FBX File|*.fbx",
            FileDlgType = FileDialogType.OpenFileDlg,
            FileDlgCaption = "Open FBX File"
        };
        if (dialog.ShowDialog() != DialogResult.OK || !dialog.HasImportedModel) return false;
        ImportMeshesToFlverAction action = new(flver, dialog.Meshes, refresher);
        ActionManager.Apply(action);
        MainWindow.ShowInformationDialog($"Successfully imported {Path.GetFileName(dialog.FileDlgFileName)}!");
        return true;
    }
}