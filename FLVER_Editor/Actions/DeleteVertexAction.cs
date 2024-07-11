using FLVER_Editor.Actions.Helpers;
using SoulsFormats;
using System.Numerics;

namespace FLVER_Editor.Actions;

public class DeleteVertexAction : TransformAction
{
    public DeleteVertexAction(FLVER2.Mesh mesh, int index, Action refresher)
    {
        this.mesh = mesh;
        this.index = index;
        this.refresher = refresher;
    }

    private readonly FLVER2.Mesh mesh;
    private readonly Action refresher;
    private readonly int index;

    private Vector3 oldVertexPosition = new(0, 0, 0);
    private Dictionary<FLVER2.FaceSet, Dictionary<int, int>> facesetBackup = new();

    public override void Execute()
    {
        facesetBackup.Clear();
        oldVertexPosition = mesh.Vertices[index].Position;
        VertexDeleteHelper.DeleteMeshVertexFaceSet(mesh, index, facesetBackup);
        mesh.Vertices[index].Position = new System.Numerics.Vector3(0, 0, 0);
        refresher.Invoke();
    }

    public override void Undo()
    {
        VertexDeleteHelper.RestoreMeshVertexFaceSet(facesetBackup);
        mesh.Vertices[index].Position = oldVertexPosition;
        refresher.Invoke();
    }
}
