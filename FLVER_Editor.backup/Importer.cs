using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using FbxDataExtractor;
using FLVER_Editor.FbxImporter.ViewModels;
using SoulsFormats;

namespace FLVER_Editor
{
    public static class Importer
    {
        public static bool ImportFbxAsync(FLVER2 flver, string fbxPath)
        {
            List<FbxMeshDataViewModel> meshes;
            try
            {
                meshes = FbxMeshData.Import(fbxPath).Select(x => new FbxMeshDataViewModel(x)).ToList();
            }
            catch
            {
                MainWindow.ShowErrorDialog($"{Path.GetFileNameWithoutExtension(fbxPath)} could not be imported due to an unknown error.");
                return false;
            }
            FbxSceneDataViewModel scene = new()
            {
                MeshData = new ObservableCollection<FbxMeshDataViewModel>(meshes)
            };
            Console.WriteLine(scene);
            return true;
        }
    }
}