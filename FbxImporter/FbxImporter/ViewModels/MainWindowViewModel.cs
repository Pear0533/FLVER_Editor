using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveHistory;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SoulsFormats;
using FbxDataExtractor;
using SoulsAssetPipeline.FLVERImporting;

namespace FbxImporter.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public enum GetPathMode
        {
            Open,
            Save
        }

        private readonly IHistory _history;

        private MeshImportOptions? _meshImportOptionsCache;

        public MainWindowViewModel()
        {
            _history = new StackHistory();

            IObservable<bool> isFlverLoaded = this.WhenAnyValue(x => x.Flver).Select(x => x is not null);
            IObservable<bool> canAddToFlver = this.WhenAnyValue(x => x.Fbx.IsMeshSelected).Select(x => x && Flver is not null);
            OpenFlverCommand = ReactiveCommand.CreateFromTask(OpenFlverAsync);
            SaveFlverCommand = ReactiveCommand.CreateFromTask(() => Task.Run(SaveFlver), isFlverLoaded);
            SaveFlverAsCommand = ReactiveCommand.CreateFromTask(SaveFlverAsAsync, isFlverLoaded);
            ImportFbxCommand = ReactiveCommand.CreateFromTask(ImportFbxAsync);
            AddToFlverCommand = ReactiveCommand.CreateFromTask(AddToFlverAsync, canAddToFlver);
            UndoCommand = ReactiveCommand.Create(_history.Undo, _history.CanUndo);
            RedoCommand = ReactiveCommand.Create(_history.Redo, _history.CanRedo);

            OpenFlverCommand.IsExecuting.Subscribe(x => ObserveProgress(x, "Opening Flver..."));
            SaveFlverCommand.IsExecuting.Subscribe(x => ObserveProgress(x, "Saving Flver..."));
            SaveFlverAsCommand.IsExecuting.Subscribe(x => ObserveProgress(x, "Saving Flver..."));
            ImportFbxCommand.IsExecuting.Subscribe(x => ObserveProgress(x, "Importing Fbx..."));

            ImportFbxCommand.ThrownExceptions.Subscribe(e =>
            {
                Logger.LogError(e is InvalidDataException ? e.Message : e.ToString());
            });
            AddToFlverCommand.ThrownExceptions.Subscribe(e =>
            {
                Logger.LogError(e is InvalidDataException ? e.Message : e.ToString());
            });
        }

        [Reactive] public FlverViewModel? Flver { get; set; }

        [Reactive] public FbxSceneDataViewModel? Fbx { get; set; }

        public ProgressViewModel Progress { get; } = new();

        private string? FlverPath { get; set; }

        public ObservableCollection<string> Log => Logger.Instance.Lines;

        public Interaction<GetFilePathArgs, string?> GetFilePath { get; } = new();

        public Interaction<Unit, string?> GetTargetGame { get; } = new();

        public Interaction<MeshImportOptionsViewModel, MeshImportOptions?> GetMeshImportOptions { get; } = new();

        public ReactiveCommand<Unit, Unit> SaveFlverCommand { get; }

        public ReactiveCommand<Unit, Unit> OpenFlverCommand { get; }

        public ReactiveCommand<Unit, Unit> SaveFlverAsCommand { get; }

        public ReactiveCommand<Unit, Unit> ImportFbxCommand { get; }

        public ReactiveCommand<Unit, Unit> AddToFlverCommand { get; }

        public ReactiveCommand<Unit, bool> RedoCommand { get; }

        public ReactiveCommand<Unit, bool> UndoCommand { get; }

        private void ObserveProgress(bool isActive, string status)
        {
            Progress.IsActive = isActive;
            Progress.Status = status;
        }

        private async Task AddToFlverAsync()
        {
            MeshImportOptionsViewModel optionsViewModel =
                new(Fbx!.SelectedMeshes[0].MTD, Flver!.MaterialInfoBank, _meshImportOptionsCache);
            MeshImportOptions? options = await GetMeshImportOptions.Handle(optionsViewModel);
            if (options is null) return;

            _meshImportOptionsCache = options;

            AddToFlverWithHistory(options);
        }

        private void AddToFlverWithHistory(MeshImportOptions options)
        {
            int meshIndex = Flver!.Meshes.Count;
            int meshCount = Fbx!.SelectedMeshes.Count;
            int nodeIndex = Flver.Flver.Nodes.FindLastIndex(x => !x.Flags.HasFlag(FLVER.Node.NodeFlags.Disabled)) + 1;
            int numAddedNodes = Fbx!.SelectedMeshes.Count(x => Flver.Flver.Nodes.All(y => x.Name != y.Name));

            _history.Snapshot(Undo, Redo);
            Redo();
            return;

            void Undo()
            {
                for (int i = 0; i < meshCount; i++)
                {
                    Flver.Meshes.RemoveAt(meshIndex);
                }

                if (numAddedNodes == 0) return;
                
                FLVER.Node firstNode = Flver.Flver.Nodes[nodeIndex];
                FLVER.Node prevNode = Flver.Flver.Nodes[firstNode.PreviousSiblingIndex];
                prevNode.NextSiblingIndex = -1;
                for (int i = 0; i < numAddedNodes; i++)
                {
                    Flver.Flver.Nodes.RemoveAt(nodeIndex);
                }
            }

            void Redo()
            {
                for (int i = 0; i < meshCount; i++)
                {
                    Flver.Meshes.Insert(meshIndex + i, Fbx!.SelectedMeshes[i].ToFlverMesh(Flver.Flver, options));
                }
            }
        }

        private void SaveFlver()
        {
            Flver!.Write(FlverPath!);
            Logger.Log($"Saved flver to {FlverPath}");
        }

        private async Task ImportFbxAsync()
        {
            List<FileTypeFilter> filters = new()
            {
                new FileTypeFilter("Autodesk Fbx Files", new List<string> { "fbx" }),
                new FileTypeFilter("All Files", new List<string> { "*" })
            };
            GetFilePathArgs args = new("Import Fbx", filters, GetPathMode.Open);
            string? fbxPath = await GetFilePath.Handle(args);
            if (fbxPath is null) return;

            Logger.Log($"Importing {Path.GetFileName(fbxPath)}...");
            List<FbxMeshDataViewModel> meshes;
            try
            {
                meshes = await Task.Run(() =>
                    FbxMeshData.Import(fbxPath).Select(x => new FbxMeshDataViewModel(x)).ToList());
            }
            catch (Exception e)
            {
                Logger.LogError("Fbx Import Failed: ");
                Logger.LogError(e.ToString());
                throw;
            }

            FbxSceneDataViewModel scene = new()
            {
                MeshData = new ObservableCollection<FbxMeshDataViewModel>(meshes)
            };

            Logger.Log("Import successful.");

            Fbx = scene;
        }

        private async Task OpenFlverAsync()
        {
            List<FileTypeFilter> filters = new()
            {
                new FileTypeFilter("Flver Files", new List<string> { "flver" }),
                new FileTypeFilter("All Files", new List<string> { "*" })
            };
            GetFilePathArgs args = new("Open Flver", filters, GetPathMode.Open);
            string? flverPath = await GetFilePath.Handle(args);
            if (flverPath is null) return;

            Logger.Log($"Opening {Path.GetFileName(flverPath)}...");
            FLVER2? flver;
            try
            {
                flver = await Task.Run(() => ReadFlver(flverPath));
                if (flver is null) return;
            }
            catch (InvalidDataException e)
            {
                Logger.Log("Warning: Assertion failed with error:");
                Logger.Log(e.ToString());
                Logger.Log("Attempting to load in flexible mode. Data might be invalid.");

                BinaryReaderEx.IsFlexible = true;
                try
                {
                    flver = await Task.Run(() => ReadFlver(flverPath));
                    if (flver is null) return;
                }
                catch
                {
                    return;
                }
                finally
                {
                    BinaryReaderEx.IsFlexible = false;
                }
            }
            catch
            {
                return;
            }

            FlverViewModel.FlverVersion? optionalVersion = flver.Header.Version switch
            {
                131092 => FlverViewModel.FlverVersion.DS3,
                131098 when flver.Materials.Any(x => x.MTD.Contains(".matxml")) => FlverViewModel.FlverVersion.ER,
                131098 => await GetTargetGame.Handle(Unit.Default) switch
                {
                    "Elden Ring" => FlverViewModel.FlverVersion.ER,
                    "Sekiro" => FlverViewModel.FlverVersion.SDT,
                    null => null,
                    _ => throw new ArgumentOutOfRangeException()
                },
                131099 => FlverViewModel.FlverVersion.AC6,
                _ => throw new InvalidDataException("Invalid Flver Version")
            };

            if (optionalVersion is not { } version) return;
            FLVER2MaterialInfoBank materialInfoBank = await FlverViewModel.LoadMaterialInfoBankAsync(version);

            _history.Clear();
            Flver = new FlverViewModel(flver, materialInfoBank, _history);
            FlverPath = flverPath;
            Logger.Log("Successfully opened flver");
        }

        private static FLVER2? ReadFlver(string? flverPath)
        {
            if (FLVER2.IsRead(flverPath, out FLVER2 flver)) return flver;
            Logger.LogError($"{flverPath} is not a flver file");
            return null;
        }

        private async Task SaveFlverAsAsync()
        {
            List<FileTypeFilter> filters = new()
            {
                new FileTypeFilter("Flver Files", new List<string> { "flver" }),
                new FileTypeFilter("All Files", new List<string> { "*" })
            };
            GetFilePathArgs args = new("Save Flver As...", filters, GetPathMode.Save);
            string? flverPath = await GetFilePath.Handle(args);
            if (flverPath is null) return;
            FlverPath = flverPath;
            SaveFlver();
        }

        public record GetFilePathArgs(string Title, List<FileTypeFilter> Filters, GetPathMode Mode);

        public record FileTypeFilter(string Name, List<string> Extensions);
    }
}