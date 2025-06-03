using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using FbxImporter.ViewModels;
using MessageBox.Avalonia.BaseWindows.Base;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Models;
using ReactiveUI;

namespace FbxImporter.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainWindowViewModel();
            DataContext = ViewModel;
            this.WhenActivated(d =>
            {
                d(ViewModel!.GetFilePath.RegisterHandler(GetFilePathAsync));
                d(ViewModel!.GetMeshImportOptions.RegisterHandler(GetMeshImportOptionsAsync));
                d(ViewModel!.GetTargetGame.RegisterHandler(GetTargetGame));
            });
        }

        private async Task GetTargetGame(InteractionContext<Unit, string?> interaction)
        {
            List<ButtonDefinition> buttonDefinitions = new()
            {
                new ButtonDefinition
                {
                    Name = "Elden Ring",
                    IsDefault = true
                },
                new ButtonDefinition
                {
                    Name = "Sekiro"
                }
            };

            MessageBoxCustomParams messageBoxParams = new()
            {
                ButtonDefinitions = buttonDefinitions,
                CanResize = false,
                ContentHeader = "Unable to determine target game",
                ContentMessage = "Could not determine target game, please specify."
            };
            IMsBoxWindow<string>? messageBox = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxCustomWindow(messageBoxParams);

            Window mainWindow = (Window)this.GetVisualRoot();
            if (mainWindow is null) throw new Exception("Main Window is null");
            string game = await messageBox.ShowDialog(mainWindow);
            interaction.SetOutput(game);
        }

        private async Task GetFilePathAsync(InteractionContext<MainWindowViewModel.GetFilePathArgs, string?> interaction)
        {
            string? path;
            switch (interaction.Input.Mode)
            {
                case MainWindowViewModel.GetPathMode.Open:
                {
                    OpenFileDialog openFileDialog = new()
                    {
                        Title = interaction.Input.Title,
                        Filters = interaction.Input.Filters
                            .Select(x => new FileDialogFilter {Name = x.Name, Extensions = x.Extensions}).ToList(),
                        AllowMultiple = false
                    };

                    path = (await openFileDialog.ShowAsync(this))?[0];
                    break;
                }
                case MainWindowViewModel.GetPathMode.Save:
                    SaveFileDialog saveFileDialog = new()
                    {
                        Title = interaction.Input.Title,
                        Filters = interaction.Input.Filters
                            .Select(x => new FileDialogFilter {Name = x.Name, Extensions = x.Extensions}).ToList()
                    };

                    path = await saveFileDialog.ShowAsync(this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(interaction));
            }

            interaction.SetOutput(path);
        }

        private async Task GetMeshImportOptionsAsync(
            InteractionContext<MeshImportOptionsViewModel, MeshImportOptions?> interaction)
        {
            MeshImportOptionsView meshImportView = new()
            {
                DataContext = interaction.Input
            };
            MeshImportOptions? options = await meshImportView.ShowDialog<MeshImportOptions?>(this);
            interaction.SetOutput(options);
        }

        private void Log_OnScrollChanged(object? sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentDelta == Vector.Zero) return;
            
            ScrollViewer? log = sender as ScrollViewer;
            log?.ScrollToEnd();
        }
        
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}