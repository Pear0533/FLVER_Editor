using Assimp;
using FbxDataExtractor;
using FLVER_Editor.Actions;
using FLVER_Editor.FbxImporter.ViewModels;
using SoulsFormats;
using System.Numerics;
using Win32Types;

namespace FLVER_Editor;

public static class Importer
{
    public static MeshImportOptions GetDefaultImportOptions()
    {
        return new MeshImportOptions(Program.MTDs[0], Program.MaterialInfoBank, WeightingMode.Skin);
    }

    private static void ShowImportErrorDialog(string fbxPath, Exception e)
    {
        MainWindow.ShowErrorDialog($"{Path.GetFileName(fbxPath)} could not be read:\n\nError: {e.Message}");
    }

    private static List<FbxMeshData> ImportFbxWithAssimp(string fbxPath)
    {
        using var importer = new AssimpContext();
        var scene = importer.ImportFile(fbxPath, PostProcessSteps.Triangulate | PostProcessSteps.GenerateSmoothNormals);
        
        if (scene == null || scene.MeshCount == 0)
            throw new Exception("Failed to import FBX file or no meshes found.");

        var meshDataList = new List<FbxMeshData>();
        var boneData = new List<string>();

        // Use the existing FbxMeshData.Import method to create FbxMeshData objects
        var importedMeshData = FbxMeshData.Import(fbxPath);

        for (int i = 0; i < Math.Min(scene.MeshCount, importedMeshData.Count); i++)
        {
            var assimpMesh = scene.Meshes[i];
            var meshData = importedMeshData[i];

            meshData.Name = assimpMesh.Name;
            UpdateFbxVertexData(meshData.VertexData, assimpMesh);
            meshData.VertexIndices = assimpMesh.GetIndices().ToList();

            meshDataList.Add(meshData);
        }

        // Collect bone data for bones.txt
        CollectBoneDataForBonesTxt(scene, boneData);

        // Export bone data to bones.txt
        ExportBoneData(boneData);

        // Process collected bone data
        ProcessCollectedSkeletonBoneData(boneData);

        return meshDataList;
    }

    private static void CollectBoneNodes(Node node, Dictionary<string, (Node Node, Node Parent)> boneNodes, Node parent = null)
    {
        // Ignore RootNode, meshes, and Assimp-generated nodes
        if (node.Name != "RootNode" && !node.MeshIndices.Any() && !node.Name.Contains("$AssimpFbx$"))
        {
            boneNodes[node.Name] = (node, parent);
        }

        foreach (var child in node.Children)
        {
            CollectBoneNodes(child, boneNodes, node);
        }
    }

    private static void CollectBoneDataForBonesTxt(Scene scene, List<string> boneData)
    {
        var boneNodes = new Dictionary<string, (Node Node, Node Parent)>();
        CollectBoneNodes(scene.RootNode, boneNodes);

        var boneList = boneNodes.ToList();
        for (int i = 0; i < boneList.Count; i++)
        {
            var (boneName, (boneNode, parentNode)) = boneList[i];
            
            boneData.Add($"Bone Index: {i}");
            boneData.Add($"Bone Name: {boneName}");

            int parentId = parentNode != null ? boneList.FindIndex(b => b.Key == parentNode.Name) : -1;
            boneData.Add($"Parent ID: {parentId}");

            int firstChildId = boneNode.Children.Any() ? boneList.FindIndex(b => b.Key == boneNode.Children[0].Name) : -1;
            boneData.Add($"First Child ID: {firstChildId}");

            var transform = boneNode.Transform;
            var (translation, scale, rotation) = DecomposeMatrixForBonesTxt(transform);

            boneData.Add($"Translation: {translation.X:F6} {translation.Y:F6} {translation.Z:F6}");
            boneData.Add($"Scale: {scale.X:F6} {scale.Y:F6} {scale.Z:F6}");
            var flippedRotation = FlipXZForBonesTxt(rotation);
            boneData.Add($"Radian: {flippedRotation.X:F6} {flippedRotation.Y:F6} {flippedRotation.Z:F6}");

            // Calculate and add BBmin and BBmax
            Vector3D bbMin = translation * -0.1f;
            Vector3D bbMax = translation * 0.1f;

            boneData.Add($"BBmin: {bbMin.X:F6} {bbMin.Y:F6} {bbMin.Z:F6}");
            boneData.Add($"BBmax: {bbMax.X:F6} {bbMax.Y:F6} {bbMax.Z:F6}");

            boneData.Add(""); // Add an empty line between bones
        }
    }

    private static Vector3D FlipXZForBonesTxt(Vector3D rotation)
    {
        return new Vector3D(rotation.Z, rotation.Y, rotation.X);
    }

    private static (Vector3D Translation, Vector3D Scale, Vector3D Rotation) DecomposeMatrixForBonesTxt(Assimp.Matrix4x4 matrix)
    {
        Vector3D translation = new Vector3D(matrix.A4, matrix.B4, matrix.C4);

        Vector3D scale = new Vector3D(
            new Vector3D(matrix.A1, matrix.B1, matrix.C1).Length(),
            new Vector3D(matrix.A2, matrix.B2, matrix.C2).Length(),
            new Vector3D(matrix.A3, matrix.B3, matrix.C3).Length()
        );

        Vector3D rotation = MatrixToEulerForBonesTxt(matrix);

        return (translation, scale, rotation);
    }

    private static Vector3D MatrixToEulerForBonesTxt(Assimp.Matrix4x4 m)
    {
        Vector3D rotation = new Vector3D();

        // Extract rotation from matrix
        rotation.Y = (float)Math.Atan2(m.A3, m.C3);  // Yaw
        rotation.Z = (float)Math.Asin(-m.B3);        // Pitch
        rotation.X = (float)Math.Atan2(m.B1, m.B2);  // Roll

        return rotation;
    }

    private static void ExportBoneData(List<string> boneData)
    {
        string directoryPath = AppDomain.CurrentDomain.BaseDirectory;
        string filePath = Path.Combine(directoryPath, "bones.txt");

        try
        {
            if (boneData.Count > 0)
            {
                //Only for some debugging purposes, will be removed eventually.
                //File.WriteAllLines(filePath, boneData);
            }
        }
        catch (Exception ex)
        {
        }
    }

    private static void UpdateFbxVertexData(List<FbxVertexData> vertexDataList, Assimp.Mesh mesh)
    {
        for (int i = 0; i < Math.Min(mesh.VertexCount, vertexDataList.Count); i++)
        {
            var vertexData = vertexDataList[i];

            vertexData.Position = new Vector3(mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z);
            
            if (mesh.HasNormals)
                vertexData.Normal = new Vector3(mesh.Normals[i].X, mesh.Normals[i].Y, mesh.Normals[i].Z);

            if (mesh.HasTangentBasis)
            {
                vertexData.Bitangent = new Vector4(mesh.BiTangents[i].X, mesh.BiTangents[i].Y, mesh.BiTangents[i].Z, 0);
                vertexData.Tangents = new List<Vector4> { new Vector4(mesh.Tangents[i].X, mesh.Tangents[i].Y, mesh.Tangents[i].Z, 0) };
            }

            if (mesh.TextureCoordinateChannels[0] != null)
                vertexData.UVs = new List<Vector2> { new Vector2(mesh.TextureCoordinateChannels[0][i].X, mesh.TextureCoordinateChannels[0][i].Y) };

            if (mesh.HasVertexColors(0))
                vertexData.Colors = new List<Vector4> { new Vector4(mesh.VertexColorChannels[0][i].R, mesh.VertexColorChannels[0][i].G, mesh.VertexColorChannels[0][i].B, mesh.VertexColorChannels[0][i].A) };

            if (mesh.HasBones)
            {
                vertexData.BoneNames = new string[4];
                vertexData.BoneWeights = new float[4];
                int boneIndex = 0;
                foreach (var bone in mesh.Bones)
                {
                    foreach (var weight in bone.VertexWeights)
                    {
                        if (weight.VertexID == i && boneIndex < 4)
                        {
                            vertexData.BoneNames[boneIndex] = bone.Name;
                            vertexData.BoneWeights[boneIndex] = weight.Weight;
                            boneIndex++;
                        }
                    }
                }
            }
        }
    }

    public static bool ImportFbxAsync(ImporterOpenFileDialog dialog, string fbxPath)
    {
        try
        {
            ImportFbxWithAssimp(fbxPath).ForEach(x => dialog.Meshes.Add(new FbxMeshDataViewModel(x), GetDefaultImportOptions()));
        }
        catch (Exception e)
        {
            ShowImportErrorDialog(fbxPath, e);
            return false;
        }
        return true;
    }

    public static bool ImportFbxAsync(FLVER2 flver, string fbxPath, Action refresher)
    {
        List<FbxMeshDataViewModel> meshes;
        try
        {
            meshes = ImportFbxWithAssimp(fbxPath).Select(x => new FbxMeshDataViewModel(x)).ToList();
        }
        catch (Exception e)
        {
            ShowImportErrorDialog(fbxPath, e);
            return false;
        }

        MeshImportOptions meshImport = GetDefaultImportOptions();
        Dictionary<FbxMeshDataViewModel, MeshImportOptions> meshData = new();
        meshes.ForEach(x => meshData.Add(x, meshImport));

        ImportMeshesToFlverAction action = new(flver, meshData, refresher);
        ActionManager.Apply(action);

        return true;
    }

    public static bool ImportFbxWithDialogAsync(FLVER2 flver, Action refresher)
    {
        ImporterOpenFileDialog dialog = new()
        {
            FileDlgFilter = @"FBX File|*.fbx",
            FileDlgType = FileDialogType.OpenFileDlg,
            FileDlgCaption = "Open FBX File"
        };
        if (dialog.ShowDialog() != DialogResult.OK || !dialog.HasImportedModel) return false;
        ImportMeshesToFlverAction action = new(flver, dialog.Meshes, refresher);
        ActionManager.Apply(action);
        MainWindow.ShowInformationDialog($"Successfully imported {Path.GetFileName(dialog.FileDlgFileName)}!");
        return true;
    }

    // Change this from struct to class
    public class SkeletonBoneData
    {
        public int Index;
        public string Name;
        public int ParentId;
        public int FirstChildId;
        public Vector3D Translation;
        public Vector3D Scale;
        public Vector3D Rotation;
        public Vector3D BBmin;
        public Vector3D BBmax;
    }

    private static List<SkeletonBoneData> collectedSkeletonBoneData = new List<SkeletonBoneData>();

    private static void ProcessCollectedSkeletonBoneData(List<string> boneData)
    {
        collectedSkeletonBoneData.Clear();

        if (boneData.Count == 0)
        {
            return;
        }

        SkeletonBoneData currentBone = null;

        for (int i = 0; i < boneData.Count; i++)
        {
            string line = boneData[i];
            string[] parts = line.Split(new[] { ':' }, 2);
            
            if (parts.Length != 2)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    if (currentBone != null)
                    {
                        collectedSkeletonBoneData.Add(currentBone);
                        currentBone = null;
                    }
                }
                continue;
            }

            string key = parts[0].Trim();
            string value = parts[1].Trim();

            if (key == "Bone Index")
            {
                if (currentBone != null)
                {
                    collectedSkeletonBoneData.Add(currentBone);
                }
                currentBone = new SkeletonBoneData();
            }

            if (currentBone == null) continue;

            switch (key)
            {
                case "Bone Index":
                    currentBone.Index = ParseIntOrDefault(value, -1);
                    break;
                case "Bone Name":
                    currentBone.Name = value;
                    break;
                case "Parent ID":
                    currentBone.ParentId = ParseIntOrDefault(value, -1);
                    break;
                case "First Child ID":
                    currentBone.FirstChildId = ParseIntOrDefault(value, -1);
                    break;
                case "Translation":
                    currentBone.Translation = ParseVector3D(value);
                    break;
                case "Scale":
                    currentBone.Scale = ParseVector3D(value);
                    break;
                case "Radian":
                    currentBone.Rotation = ParseVector3D(value);
                    break;
                case "BBmin":
                    currentBone.BBmin = ParseVector3D(value);
                    break;
                case "BBmax":
                    currentBone.BBmax = ParseVector3D(value);
                    break;
            }
        }

        if (currentBone != null)
        {
            collectedSkeletonBoneData.Add(currentBone);
        }
    }

    private static int ParseIntOrDefault(string input, int defaultValue)
    {
        return int.TryParse(input, out int result) ? result : defaultValue;
    }

    private static Vector3D ParseVector3D(string input)
    {
        var values = input.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => float.TryParse(s, out float f) ? f : 0f)
                        .ToArray();
        
        if (values.Length >= 3)
        {
            return new Vector3D(values[0], values[1], values[2]);
        }
        else
        {
            return new Vector3D(0, 0, 0);
        }
    }

    public static List<SkeletonBoneData> GetCollectedSkeletonBoneData()
    {
        return new List<SkeletonBoneData>(collectedSkeletonBoneData);
    }

    public static List<object[]> GetSkeletonBoneDataForTable()
    {
        var tableData = new List<object[]>();
        foreach (var bone in collectedSkeletonBoneData)
        {
            tableData.Add(new object[]
            {
                bone.Index,
                bone.Name,
                bone.ParentId,
                bone.FirstChildId,
                bone.Translation.X, bone.Translation.Y, bone.Translation.Z,
                bone.Rotation.X, bone.Rotation.Y, bone.Rotation.Z,
                bone.Scale.X, bone.Scale.Y, bone.Scale.Z,
                bone.BBmin.X, bone.BBmin.Y, bone.BBmin.Z,
                bone.BBmax.X, bone.BBmax.Y, bone.BBmax.Z
            });
        }
        return tableData;
    }
}

public class ImportException : Exception
{
    public ImportException(string message) : base(message) { }
    public ImportException(string message, Exception innerException) : base(message, innerException) { }
}