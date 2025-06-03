using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using FbxImporter.ViewModels;
using JetBrains.Annotations;
using MessageBox.Avalonia.BaseWindows.Base;
using MessageBox.Avalonia.Enums;
using ReactiveUI;

namespace FbxImporter.Views;

[UsedImplicitly]
public partial class FlverView : ReactiveUserControl<FlverViewModel>
{
    public FlverView()
    {
        InitializeComponent();
        this.WhenActivated(d =>
        {
            d(ViewModel!.ShowMessage.RegisterHandler(HandleShowMessageInteraction));
        });
    }

    private async Task HandleShowMessageInteraction(InteractionContext<(string, string), Unit> interaction)
    {
        (string title, string text) = interaction.Input;
        await ShowMessage(title, text);
        interaction.SetOutput(Unit.Default);
    }

    private async Task ShowMessage(string title, string text)
    {
        Window mainWindow = (Window) this.GetVisualRoot();
        IMsBoxWindow<ButtonResult>? messageBoxError = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow(title, text);
        await messageBoxError.Show(mainWindow);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}