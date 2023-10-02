using System.Collections.ObjectModel;

namespace FLVER_Editor.FbxImporter.ViewModels;

public class FbxSceneDataViewModel
{
    public ObservableCollection<FbxMeshDataViewModel> MeshData { get; set; } = new();

    public FbxMeshDataViewModel? SelectedMesh { get; set; }
}