using FLVER_Editor.FbxImporter.ViewModels;
using SoulsAssetPipeline.FLVERImporting;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    private readonly int bonesCount;

    public ImportMeshesToFlverAction(FLVER2 flver, Dictionary<FbxMeshDataViewModel, MeshImportOptions> mesh, Action refresher)
    {
        this.flver = flver;
        this.meshes = mesh;
        this.refresher = refresher;
        bufferLayoutCount = flver.BufferLayouts.Count;
        materialsCount = flver.Materials.Count;
        gxListsCount = flver.GXLists.Count;
        meshesCount = flver.Meshes.Count;
        bonesCount = flver.Bones.Count;
    }

    public override void Execute()
    {
        meshes.ToList().ForEach(i => i.Key.ToFlverMesh(flver, i.Value));
        refresher.Invoke();
    }

    public override void Undo()
    {
        var bufferLayoutsLength = flver.BufferLayouts.Count - bufferLayoutCount;
        var materialsLength = flver.Materials.Count - materialsCount;
        var gxListsLength = flver.GXLists.Count - gxListsCount;
        var meshesLength = flver.Meshes.Count - meshesCount;
        var boneLength = flver.Bones.Count - bonesCount;

        if (bufferLayoutsLength < 0) bufferLayoutsLength = 0; 
        if (materialsLength < 0) materialsLength = 0; 
        if (gxListsLength < 0) gxListsLength = 0; 
        if (meshesLength < 0) meshesLength = 0; 
        if (boneLength < 0) boneLength = 0;

        flver.BufferLayouts.RemoveRange(bufferLayoutCount, bufferLayoutsLength);
        flver.Materials.RemoveRange(materialsCount, materialsLength);
        flver.GXLists.RemoveRange(gxListsCount, gxListsLength);
        flver.Meshes.RemoveRange(meshesCount, meshesLength);
        flver.Bones.RemoveRange(bonesCount, boneLength);
     
        refresher.Invoke();
    }
}
