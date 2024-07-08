using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor.Actions;

public class MeshTableCellValueChangedAction : TransformAction
{
    private readonly int newBoneWeight;
    private readonly int row;
    private readonly Action<int> refresher;
    private readonly int oldDisplayedIndex;
    private readonly int newdisplayedIndex;
    private byte oldDynamic;
    private bool removeBoneIndice;

    private record OldValues(FLVER.VertexBoneWeights Weights, FLVER.VertexBoneIndices Indices);
    private Dictionary<FLVER.Vertex, OldValues> OldBoneData = new();


    public MeshTableCellValueChangedAction(int newBoneWeight, int row, int oldDisplayedIndex, int newdisplayedIndex, Action<int> refresher)
    {
        this.newBoneWeight = newBoneWeight;
        this.row = row;
        this.refresher = refresher;
        this.oldDisplayedIndex = oldDisplayedIndex;
        this.newdisplayedIndex = newdisplayedIndex;
    }

    public override void Execute()
    {
        foreach (FLVER.Vertex v in MainWindow.Flver.Meshes[row].Vertices)
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

        if (!MainWindow.Flver.Meshes[row].BoneIndices.Contains(newBoneWeight))
        {
            MainWindow.Flver.Meshes[row].BoneIndices.Add(newBoneWeight);
            removeBoneIndice = true;
        }

        oldDynamic = MainWindow.Flver.Meshes[row].Dynamic;
        MainWindow.Flver.Meshes[row].Dynamic = 1;

        refresher.Invoke(newdisplayedIndex);
    }

    public override void Undo()
    {
        foreach (FLVER.Vertex v in MainWindow.Flver.Meshes[row].Vertices)
        {
            var data = OldBoneData[v];

            v.BoneIndices = data.Indices;
            v.BoneWeights = data.Weights;
        }

        if (!removeBoneIndice)
        {
            MainWindow.Flver.Meshes[row].BoneIndices.Remove(newBoneWeight);
        }

        MainWindow.Flver.Meshes[row].Dynamic = oldDynamic;

        refresher.Invoke(oldDisplayedIndex);
    }
}
