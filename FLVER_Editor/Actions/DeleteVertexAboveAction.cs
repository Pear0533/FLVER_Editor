using FLVER_Editor.Actions.Helpers;
using SoulsFormats;
using System.Numerics;

namespace FLVER_Editor.Actions;

public class DeleteVertexAboveAction : TransformAction
{
    public DeleteVertexAboveAction(FLVER2.Mesh mesh, float yValue, Action refresher)
    {
        this.mesh = mesh;
        this.yValue = yValue;
        this.refresher = refresher;
    }

    private readonly FLVER2.Mesh mesh;
    private readonly int index;
    private readonly float yValue;
    private readonly Action refresher;
    private Dictionary<int, Vector3> oldVertexPositions = new Dictionary<int, Vector3>();
    private Dictionary<FLVER2.FaceSet, Dictionary<int, int>> facesetBackup = new();

    public override void Execute()
    {
        facesetBackup.Clear();
        oldVertexPositions.Clear();

        for (var i = 0; i < mesh.Vertices.Count; i++)
        {
            if (mesh.Vertices[i].Position.Y > yValue)
            {
                VertexDeleteHelper.DeleteMeshVertexFaceSet(mesh, i, facesetBackup);

                oldVertexPositions.Add(i, mesh.Vertices[i].Position);
                mesh.Vertices[i].Position.Z = 0;
                mesh.Vertices[i].Position.Y = 0;
                mesh.Vertices[i].Position.X = 0;
            }
        }
        
        refresher.Invoke();
    }

    public override void Undo()
    {
        VertexDeleteHelper.RestoreMeshVertexFaceSet(facesetBackup);

        foreach (var record in oldVertexPositions)
        {
            var index = record.Key;
            var position = record.Value;

            mesh.Vertices[index].Position = position;
        }

        refresher.Invoke();
    }
}
