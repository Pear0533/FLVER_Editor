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
    private readonly FLVER2 flver;
    private readonly FLVER2 newFlver;
    private readonly Action refresher;
    private readonly int materialOffset;
    private readonly int layoutOffset;
    private readonly int meshOffset;

    public MergeFlversAction(FLVER2 currentFlver, FLVER2 newFlver, Action refresher)
    {
        materialOffset = currentFlver.Materials.Count;
        meshOffset = currentFlver.Meshes.Count;
        layoutOffset = currentFlver.BufferLayouts.Count;
        this.flver = currentFlver;
        this.newFlver = newFlver;
        this.refresher = refresher;
    }
    public override void Execute()
    {
        Dictionary<int, int> newFlverToCurrentFlver = new();
        for (int i = 0; i < newFlver.Nodes.Count; ++i)
        {
            FLVER.Node attachBone = newFlver.Nodes[i];
            for (int j = 0; j < flver.Nodes.Count; ++j)
            {
                if (attachBone.Name != flver.Nodes[j].Name) continue;
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
        flver.BufferLayouts = flver.BufferLayouts.Concat(newFlver.BufferLayouts).ToList();
        flver.Meshes = flver.Meshes.Concat(newFlver.Meshes).ToList();
        flver.Materials = flver.Materials.Concat(newFlver.Materials).ToList();
        refresher.Invoke();
    }

    public override void Undo()
    {
        flver.BufferLayouts.RemoveRange(layoutOffset, flver.BufferLayouts.Count - layoutOffset);
        flver.Meshes.RemoveRange(meshOffset, flver.Meshes.Count - meshOffset);
        flver.Materials.RemoveRange(materialOffset, flver.Materials.Count - materialOffset);
        refresher.Invoke();
    }
}
