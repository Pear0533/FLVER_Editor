using SoulsFormats;
using System.Numerics;

namespace FLVER_Editor.Actions;

public class ReverseNormalsAction : TransformAction
{
    private readonly IEnumerable<FLVER.Vertex> vertexes;
    private readonly Action refresher;

    public ReverseNormalsAction(IEnumerable<FLVER.Vertex> vertexes, Action refresher)
    {
        this.vertexes = vertexes;
        this.refresher = refresher;
    }

    public override void Execute()
    {
        ReverseNormals();
        refresher.Invoke();
    }

    private void ReverseNormals()
    {
        foreach (FLVER.Vertex v in vertexes)
        {
            v.Normal = new Vector3(-v.Normal.X, -v.Normal.Y, -v.Normal.Z);
            for (int j = 0; j < v.Tangents.Count; ++j)
                v.Tangents[j] = new Vector4(-v.Tangents[j].X, -v.Tangents[j].Y, -v.Tangents[j].Z, v.Tangents[j].W);
        }

        refresher.Invoke();
    }

    public override void Undo()
    {
        ReverseNormals();
        refresher.Invoke();
    }
}