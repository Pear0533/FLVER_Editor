using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FbxImporter.ViewModels;

namespace FbxImporter.Views;

public partial class FbxSceneDataView : ReactiveUserControl<FbxSceneDataViewModel>
{
    public FbxSceneDataView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}