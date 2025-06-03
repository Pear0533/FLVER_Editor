using System.Collections.ObjectModel;
using System.Reactive.Linq;
using FbxDataExtractor;
using FbxImporter.ViewModels;
using Reactive.Bindings;
using ReactiveHistory;
using SoulsFormats;
using Win32Types;
using static FLVER_Editor.Program;

namespace FLVER_Editor;

public static class Importer
{
    public static MeshImportOptions GetDefaultImportOptions()
    {
        // TODO: Extend this to work for other games aside from just ER... (Pear)
        return new MeshImportOptions(MTDs.ElementAtOrDefault(MTDs.FindIndex(i => i == "c[amsn]_e.matxml")) ?? MTDs[0], MaterialInfoBank, WeightingMode.Skin);
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

        // ImportMeshesToFlverAction action = new(flver, meshData, refresher);
        // ActionManager.Apply(action);

        return true;
    }

    private static void AddImportDialogMeshesToFlver(FLVER2 flver, Dictionary<FbxMeshDataViewModel, MeshImportOptions> meshes, Action refresher)
    {
        // ImportMeshesToFlverAction action = new(flver, meshes, refresher);
        // ActionManager.Apply(action);
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

        // ImportMeshesToFlverAction action = new(flver, dialog.Meshes, refresher);
        // ActionManager.Apply(action);

        IEnumerable<FlverMeshViewModel> newMeshes = dialog.Meshes.Select(x => x.Key.ToFlverMesh(flver, x.Value with { FlipFaces = true }));
        FlverViewModel newFlver = new(flver, MaterialInfoBank, new StackHistory())
        {
            Meshes = new ObservableCollection<FlverMeshViewModel>(flver.Meshes.Select(x => new FlverMeshViewModel(flver, x)).Concat(newMeshes))
        };

        Flver = newFlver.Write();
        refresher.Invoke();

        MainWindow.ShowInformationDialog($"Successfully imported {Path.GetFileName(dialog.FileDlgFileName)}!");
        return true;
    }
}