using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor.Actions;

public class MeshTableCellValueChangedAction : TransformAction
{
    private readonly FLVER2 flver;
    private readonly int newBoneWeight;
    private readonly int row;
    private readonly Action<int> refresher;
    private readonly int oldDisplayedIndex;
    private readonly int newdisplayedIndex;
    private bool oldUseBoneWeights;
    private bool removeBoneIndice;

    private record OldValues(FLVER.VertexBoneWeights Weights, FLVER.VertexBoneIndices Indices);
    private Dictionary<FLVER.Vertex, OldValues> OldBoneData = new();


    public MeshTableCellValueChangedAction(FLVER2 flver, int newBoneWeight, int row, int oldDisplayedIndex, int newdisplayedIndex, Action<int> refresher)
    {
        this.flver = flver;
        this.newBoneWeight = newBoneWeight;
        this.row = row;
        this.refresher = refresher;
        this.oldDisplayedIndex = oldDisplayedIndex;
        this.newdisplayedIndex = newdisplayedIndex;
    }

    public override void Execute()
    {
        foreach (FLVER.Vertex v in flver.Meshes[row].Vertices)
        {
            OldBoneData.Add(v, new(v.BoneWeights, v.BoneIndices));

            if (Util3D.BoneWeightsToFloatArray(v.BoneWeights) == null)
            {
                v.BoneWeights = new FLVER.VertexBoneWeights();
                v.BoneIndices = new FLVER.VertexBoneIndices();
            }
            for (int j = 0; j < v.BoneWeights.Length; ++j)
                v.BoneWeights[j] = 0;

            v.BoneIndices[0] = newBoneWeight;
            v.BoneWeights[0] = 1;
        }

        if (!flver.Meshes[row].BoneIndices.Contains(newBoneWeight))
        {
            flver.Meshes[row].BoneIndices.Add(newBoneWeight);
            removeBoneIndice = true;
        }

        oldUseBoneWeights = flver.Meshes[row].UseBoneWeights;
        flver.Meshes[row].UseBoneWeights = true;

        refresher.Invoke(newdisplayedIndex);
    }

    public override void Undo()
    {
        foreach (FLVER.Vertex v in flver.Meshes[row].Vertices)
        {
            var data = OldBoneData[v];

            v.BoneIndices = data.Indices;
            v.BoneWeights = data.Weights;
        }

        if (!removeBoneIndice)
        {
            flver.Meshes[row].BoneIndices.Remove(newBoneWeight);
        }

        flver.Meshes[row].UseBoneWeights = oldUseBoneWeights;

        refresher.Invoke(oldDisplayedIndex);
    }
}
