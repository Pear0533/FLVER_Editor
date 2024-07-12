using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor.Actions;

public class DeleteSelectedMeshAction : TransformAction
{
    private readonly FLVER2 flver;
    private readonly List<FLVER2.Mesh> meshList;
    private readonly List<FLVER.Dummy> dummyList;
    private readonly bool deleteFacesets;
    private readonly Action? refresher;
    private Dictionary<FLVER2.FaceSet, List<int>> facesetOldIndices = new();
    private Dictionary<int, FLVER2.Mesh> deletedMeshes = new();
    private Dictionary<int, FLVER.Dummy> deletedDummies = new();

    public DeleteSelectedMeshAction(FLVER2 flver, List<FLVER2.Mesh> meshList, List<FLVER.Dummy> dummyList, bool deleteFacesets, Action? refresher)
    {
        this.flver = flver;
        this.meshList = meshList;
        this.dummyList = dummyList;
        this.deleteFacesets = deleteFacesets;
        this.refresher = refresher;
    }

    public override void Execute()
    {
        this.facesetOldIndices.Clear();
        this.deletedMeshes.Clear();
        this.deletedDummies.Clear();

        foreach (var mesh in meshList)
        {
            var meshIndex = flver.Meshes.IndexOf(mesh);
            deletedMeshes.Add(meshIndex, mesh);
        }

        if (deleteFacesets)
        {
            foreach (var mesh in meshList)
            {
                var meshIndex = flver.Meshes.IndexOf(mesh);


                foreach (FLVER2.FaceSet fs in mesh.FaceSets)
                {
                    var facesetIndices = new List<int>();

                    for (int j = 0; j < fs.Indices.Count; ++j)
                    {
                        facesetIndices.Add(fs.Indices[j]);
                        fs.Indices[j] = 1;
                    }

                    facesetOldIndices.Add(fs, facesetIndices);
                }

            }
        }
        else
        {
            foreach (var mesh in meshList)
            {
                flver.Meshes.Remove(mesh);
            }
        }

        foreach (FLVER.Dummy dummy in dummyList)
        {
            var dummyIndex = flver.Dummies.IndexOf(dummy);
            deletedDummies.Add(dummyIndex, dummy);
        }

        foreach (FLVER.Dummy dummy in dummyList)
        {
            flver.Dummies.Remove(dummy);
        }

        refresher?.Invoke();

    }

    public override void Undo()
    {
        if (deleteFacesets)
        {
            foreach (var fs in facesetOldIndices)
            {
                var facesetIndices = fs.Value;
                var faceset = fs.Key;

                for (int j = 0; j < faceset.Indices.Count; ++j)
                {
                    faceset.Indices[j] = facesetIndices[j];
                }
            }
        }
        else
        {
            foreach (var fs in deletedMeshes.OrderBy(x => x.Key))
            {
                flver.Meshes.Insert(fs.Key, fs.Value);
            }
        }

        foreach (var fs in deletedDummies.OrderBy(x => x.Key))
        {
            flver.Dummies.Insert(fs.Key, fs.Value);
        }

        refresher?.Invoke();
    }
}
