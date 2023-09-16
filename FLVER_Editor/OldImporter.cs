using System.Text;
using Assimp;
using FbxDataExtractor;
using FLVER_Editor.FbxImporter.ViewModels;
using SoulsFormats;
using FbxVertexData = FbxDataExtractor.FbxVertexData;

namespace FLVER_Editor;

// TODO: Eliminate the old importer and merge its functionality into the new importer
public static class OldImporter
{
    public static unsafe bool ImportAssimp(FLVER2 flver, string modelFilePath)
    {
        Scene scene = new AssimpContext().ImportFile(modelFilePath);
        List<FbxMeshDataViewModel> meshes = new();
        foreach (Mesh? mesh in scene.Meshes)
        {
            fixed (byte* nameBytes = Encoding.ASCII.GetBytes(mesh.Name))
            {
                FbxMeshData meshData = new((sbyte*)nameBytes) { VertexIndices = mesh.GetIndices().ToList() };
                foreach (Assimp.Vector3D vertex in mesh.Vertices)
                {
                    FbxVertexData vertexData = new() { Position = new[] { vertex.X, vertex.Y, vertex.Z } };
                    meshData.VertexData.Add(vertexData);
                }
                meshes.Add(new FbxMeshDataViewModel(meshData));
            }
        }
        meshes.ForEach(i => i.ToFlverMesh(flver, NewImporter.GetDefaultImportOptions()));
        return true;
    }
}