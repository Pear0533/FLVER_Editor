using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor.Actions;

public class MergeFlversAction : TransformAction
{
    private readonly FLVER2 newFlver;
    private readonly Action refresher;
    private readonly int materialOffset;
    private readonly int layoutOffset;
    private readonly int meshOffset;

    public MergeFlversAction(FLVER2 newFlver, Action refresher)
    {
        materialOffset = MainWindow.Flver.Materials.Count;
        meshOffset = MainWindow.Flver.Meshes.Count;
        layoutOffset = MainWindow.Flver.BufferLayouts.Count;
        this.newFlver = newFlver;
        this.refresher = refresher;
    }
    public override void Execute()
    {
        Dictionary<int, int> newFlverToCurrentFlver = new();
        for (int i = 0; i < newFlver.Bones.Count; ++i)
        {
            FLVER.Bone attachBone = newFlver.Bones[i];
            for (int j = 0; j < MainWindow.Flver.Bones.Count; ++j)
            {
                if (attachBone.Name != MainWindow.Flver.Bones[j].Name) continue;
                newFlverToCurrentFlver.Add(i, j);
                break;
            }
        }
        foreach (FLVER2.Mesh m in newFlver.Meshes)
        {
            m.MaterialIndex += materialOffset;
            foreach (FLVER2.VertexBuffer vb in m.VertexBuffers)
                vb.LayoutIndex += layoutOffset;
            foreach (FLVER.Vertex v in m.Vertices.Where(v => Util3D.BoneIndicesToIntArray(v.BoneIndices) != null))
            {
                for (int i = 0; i < v.BoneIndices.Length; ++i)
                {
                    if (newFlverToCurrentFlver.ContainsKey(v.BoneIndices[i])) v.BoneIndices[i] = newFlverToCurrentFlver[v.BoneIndices[i]];
                }
            }
        }
        MainWindow.Flver.BufferLayouts = MainWindow.Flver.BufferLayouts.Concat(newFlver.BufferLayouts).ToList();
        MainWindow.Flver.Meshes = MainWindow.Flver.Meshes.Concat(newFlver.Meshes).ToList();
        MainWindow.Flver.Materials = MainWindow.Flver.Materials.Concat(newFlver.Materials).ToList();
        refresher.Invoke();
    }

    public override void Undo()
    {
        MainWindow.Flver.BufferLayouts.RemoveRange(layoutOffset, MainWindow.Flver.BufferLayouts.Count - layoutOffset);
        MainWindow.Flver.Meshes.RemoveRange(meshOffset, MainWindow.Flver.Meshes.Count - meshOffset);
        MainWindow.Flver.Materials.RemoveRange(materialOffset, MainWindow.Flver.Materials.Count - materialOffset);
        refresher.Invoke();
    }
}
