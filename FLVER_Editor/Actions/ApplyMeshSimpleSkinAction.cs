using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor.Actions;

public class ApplyMeshSimpleSkinAction : TransformAction
{
    private readonly int boneIndex;
    private readonly List<FLVER.Vertex> unweightedVerts = new();
    private readonly List<FLVER.Vertex> oldUnweightedVerts = new();


    public ApplyMeshSimpleSkinAction(int boneIndex, List<FLVER.Vertex> unweightedVerts)
    {
        this.boneIndex = boneIndex;
        this.unweightedVerts = unweightedVerts;

        foreach (var v in unweightedVerts)
        {
            oldUnweightedVerts.Add(new FLVER.Vertex(v));
        }
    }

    public override void Execute()
    {
        foreach (FLVER.Vertex v in unweightedVerts)
        {
            v.BoneIndices[0] = boneIndex;
            v.BoneWeights = new FLVER.VertexBoneWeights
            {
                [0] = 1,
                [1] = 0,
                [2] = 0,
                [3] = 0
            };
        }
    }

    public override void Undo()
    {
        for (int i = 0; i < unweightedVerts.Count; i++)
        {
            var original = oldUnweightedVerts[i];
            var target = unweightedVerts[i];

            target.BoneIndices[0] = original.BoneIndices[0];
            target.BoneWeights = new FLVER.VertexBoneWeights
            {
                [0] = original.BoneIndices[0],
                [1] = original.BoneIndices[1],
                [2] = original.BoneIndices[2],
                [3] = original.BoneIndices[3]
            };
        }
    }
}
