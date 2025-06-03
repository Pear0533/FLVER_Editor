using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI.Fody.Helpers;

namespace FbxImporter.ViewModels;

public class FbxSceneDataViewModel : ViewModelBase
{
    public FbxSceneDataViewModel()
    {
        SelectedMeshes.ToObservableChangeSet().ToCollection().Select(x => x.Count > 0)
            .ToPropertyEx(this, x => x.IsMeshSelected);
    }
    
    [Reactive] public ObservableCollection<FbxMeshDataViewModel> MeshData { get; set; } = new();

    [Reactive] public ObservableCollection<FbxMeshDataViewModel> SelectedMeshes { get; set; } = new();
    
    [ObservableAsProperty] public bool IsMeshSelected { get; set; }
}