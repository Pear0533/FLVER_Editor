/*
using FLVER_Editor.FbxImporter.ViewModels;
using SoulsAssetPipeline.FLVERImporting;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FbxImporter.Util;

namespace FLVER_Editor.Actions;

public class ImportMeshesToFlverAction : TransformAction
{
    private readonly FLVER2 flver;
    private readonly Dictionary<FbxMeshDataViewModel, MeshImportOptions> meshes;
    private readonly Action refresher;
    private readonly int bufferLayoutCount;
    private readonly int materialsCount;
    private readonly int gxListsCount;
    private readonly int meshesCount;
    private readonly int nodesCount;
    private readonly int baseSkeletonCount;
    private readonly int allSkeletonsCount;
    private readonly List<FLVER2.SkeletonSet.Bone> oldBaseBones = new();
    private readonly List<FLVER2.SkeletonSet.Bone> oldAllBones = new();
    private readonly Dictionary<FLVER.Node, FLVER.Node.NodeFlags> oldNodes = new();
    private readonly FLVER2.SkeletonSet oldSkeleton;
    private List<FLVER2.Mesh> cMeshes = new();

    public ImportMeshesToFlverAction(FLVER2 flver, Dictionary<FbxMeshDataViewModel, MeshImportOptions> mesh, Action refresher)
    {
        this.flver = flver;
        this.meshes = mesh;
        this.refresher = refresher;
        bufferLayoutCount = flver.BufferLayouts.Count;
        materialsCount = flver.Materials.Count;
        gxListsCount = flver.GXLists.Count;
        meshesCount = flver.Meshes.Count;
        nodesCount = flver.Nodes.Count;
        oldSkeleton = flver.Skeletons;

        if (flver.Skeletons is not null)
        {
            baseSkeletonCount = flver.Skeletons.BaseSkeleton.Count;
            allSkeletonsCount = flver.Skeletons.AllSkeletons.Count;
        }

    }

    public void Write(string path)
    {
        flver.Meshes = new List<FLVER2.Mesh>(cMeshes);
        flver.Materials = new List<FLVER2.Material>();
        flver.GXLists = new List<FLVER2.GXList>();
        foreach (FLVER2.Mesh mesh in cMeshes)
        {
            var material = flver.Materials[mesh.MaterialIndex];
            int materialIndex = flver.Materials.FindIndex(x => x.Name == material.Name && x.MTD == material.MTD);
            if (materialIndex == -1)
            {
                materialIndex = flver.Materials.Count;
                flver.Materials.Add(material);
                if (flver.Header.Version >= 131098) material.Index = materialIndex;

                var gxList = flver.GXLists.ElementAtOrDefault(material.GXIndex);
                if (gxList is not null)
                {
                    int gxListIndex = flver.GXLists.Count;
                    flver.GXLists.Add(gxList);
                    material.GXIndex = gxListIndex;
                }
            }
            mesh.MaterialIndex = materialIndex;
        }
        
        flver.FixAllBoundingBoxes();
        if (flver.Header.Version >= 131098)
        {
            flver.AddNodesToSkeletons();
            flver.SetNodeFlags();
        }

        // Soulsformats will corrupt the file if there is an exception on write so back up the file first and write it back to disk if the write fails.
        FLVER2? backupFlver;
        try
        {
            backupFlver = FLVER2.Read(path);
        }
        catch
        {
            backupFlver = null;
        }

        try
        {
            flver.Write(path);
        }
        catch
        {
            backupFlver?.Write(path);
            throw;
        }
    }

    public override void Execute()
    {
        oldNodes.Clear();
        oldBaseBones.Clear();
        oldNodes.Clear();
        flver.Skeletons ??= new();

        SaveNodeFlags(flver);
        SaveNodesToSkeleton(flver, flver.Skeletons.AllSkeletons, oldAllBones);
        SaveNodesToSkeleton(flver, flver.Skeletons.BaseSkeleton, oldBaseBones);

        cMeshes = new List<FLVER2.Mesh>();
        meshes.ToList().ForEach(i => cMeshes.Add(i.Key.ToFlverMesh(flver, i.Value)));
        Write(MainWindow.FlverFilePath);

        refresher.Invoke();
    }

    public override void Undo()
    {
        var bufferLayoutsLength = flver.BufferLayouts.Count - bufferLayoutCount;
        var materialsLength = flver.Materials.Count - materialsCount;
        var gxListsLength = flver.GXLists.Count - gxListsCount;
        var meshesLength = flver.Meshes.Count - meshesCount;
        var nodeLength = flver.Nodes.Count - nodesCount;
        var baseSkeletonLength = flver.Skeletons.BaseSkeleton.Count - baseSkeletonCount;
        var allSkeletonsLength = flver.Skeletons.AllSkeletons.Count - allSkeletonsCount;

        if (bufferLayoutsLength < 0) bufferLayoutsLength = 0;
        if (materialsLength < 0) materialsLength = 0;
        if (gxListsLength < 0) gxListsLength = 0;
        if (meshesLength < 0) meshesLength = 0;
        if (nodeLength < 0) nodeLength = 0;
        if (baseSkeletonLength < 0) baseSkeletonLength = 0;
        if (allSkeletonsLength < 0) allSkeletonsLength = 0;


        flver.BufferLayouts.RemoveRange(bufferLayoutCount, bufferLayoutsLength);
        flver.Materials.RemoveRange(materialsCount, materialsLength);
        flver.GXLists.RemoveRange(gxListsCount, gxListsLength);
        flver.Meshes.RemoveRange(meshesCount, meshesLength);
        flver.Nodes.RemoveRange(nodesCount, nodeLength);

        foreach ( var mesh in meshes)
        {
            var item = mesh.Key;
            FlipFaceSet(item);
        }

        flver.Skeletons = oldSkeleton;


        if (flver.Skeletons is not null)
        {
            UndoNodesToSkeleton(flver, flver.Skeletons.BaseSkeleton, oldBaseBones);
            UndoNodesToSkeleton(flver, flver.Skeletons.AllSkeletons, oldAllBones);
        }

        UndoNodeFlags(flver);

        refresher.Invoke();
    }

    private void FlipFaceSet(FbxMeshDataViewModel model)
    {
        for (int i = 0; i < model.Data.VertexIndices.Count; i += 3)
        {
            (model.Data.VertexIndices[i + 1], model.Data.VertexIndices[i + 2]) =
                (model.Data.VertexIndices[i + 2], model.Data.VertexIndices[i + 1]);
        }
    }

    private void SaveNodesToSkeleton(FLVER2 flver, List<FLVER2.SkeletonSet.Bone> skeleton, List<FLVER2.SkeletonSet.Bone> backup)
    {
        backup.Clear();

        for (int i = 0; i < skeleton.Count; i++)
        {
            var node = skeleton[i];

            backup.Add(new FLVER2.SkeletonSet.Bone(node.NodeIndex)
            {
                ParentIndex = node.ParentIndex,
                FirstChildIndex = node.FirstChildIndex,
                PreviousSiblingIndex = node.PreviousSiblingIndex,
                NextSiblingIndex = node.NextSiblingIndex
            });
        }
    }

    private void UndoNodesToSkeleton(FLVER2 flver, List<FLVER2.SkeletonSet.Bone> skeleton, List<FLVER2.SkeletonSet.Bone> backup)
    {
        skeleton.Clear();

        for (int i = 0; i < backup.Count; i++)
        {
            var node = backup[i];

            skeleton.Add(new FLVER2.SkeletonSet.Bone(node.NodeIndex)
            {
                ParentIndex = node.ParentIndex,
                FirstChildIndex = node.FirstChildIndex,
                PreviousSiblingIndex = node.PreviousSiblingIndex,
                NextSiblingIndex = node.NextSiblingIndex
            });
        }
    }

    private void SaveNodeFlags(FLVER2 flver)
    {
        foreach (var node in flver.Nodes)
        {
            oldNodes.Add(node, node.Flags);
        }
    }

    private void UndoNodeFlags(FLVER2 flver)
    {
        foreach (var item in oldNodes)
        {
            var node = item.Key;
            var value = item.Value;
            node.Flags = value;
        }
    }

}
*/