using ReactiveUI.Fody.Helpers;

namespace FbxImporter.ViewModels;

public class ProgressViewModel : ViewModelBase
{
    [Reactive] public string Status { get; set; } = "";

    [Reactive] public bool IsActive { get; set; }
}