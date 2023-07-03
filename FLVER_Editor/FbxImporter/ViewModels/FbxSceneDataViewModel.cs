using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace FLVER_Editor.FbxImporter.ViewModels;

[JsonObject(MemberSerialization.OptIn)]
public class FbxSceneDataViewModel
{
    [JsonProperty]
    public ObservableCollection<FbxMeshDataViewModel> MeshData { get; set; } = new();

    public FbxMeshDataViewModel SelectedMesh { get; set; }
}