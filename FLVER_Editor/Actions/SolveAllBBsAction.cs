using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor.Actions;

public class SolveAllBBsAction : TransformAction
{
    private void UpdateHeaderBoundingBox(FLVER2.FLVERHeader header, Vector3 vertexPos)
    {
        float minX = Math.Min(header.BoundingBoxMin.X, vertexPos.X);
        float minY = Math.Min(header.BoundingBoxMin.Y, vertexPos.Y);
        float minZ = Math.Min(header.BoundingBoxMin.Z, vertexPos.Z);
        float maxX = Math.Max(header.BoundingBoxMax.X, vertexPos.X);
        float maxY = Math.Max(header.BoundingBoxMax.Y, vertexPos.Y);
        float maxZ = Math.Max(header.BoundingBoxMax.Z, vertexPos.Z);
        header.BoundingBoxMin = new Vector3(minX, minY, minZ);
        header.BoundingBoxMax = new Vector3(maxX, maxY, maxZ);
    }

    private void UpdateMeshBoundingBox(FLVER2.Mesh mesh, Vector3 vertexPos)
    {
        mesh.BoundingBox ??= new FLVER2.Mesh.BoundingBoxes();
        float minX = Math.Min(mesh.BoundingBox.Min.X, vertexPos.X);
        float minY = Math.Min(mesh.BoundingBox.Min.Y, vertexPos.Y);
        float minZ = Math.Min(mesh.BoundingBox.Min.Z, vertexPos.Z);
        float maxX = Math.Max(mesh.BoundingBox.Max.X, vertexPos.X);
        float maxY = Math.Max(mesh.BoundingBox.Max.Y, vertexPos.Y);
        float maxZ = Math.Max(mesh.BoundingBox.Max.Z, vertexPos.Z);
        mesh.BoundingBox.Min = new Vector3(minX, minY, minZ);
        mesh.BoundingBox.Max = new Vector3(maxX, maxY, maxZ);
    }

    private System.Numerics.Matrix4x4 GetAbsoluteNMatrix(FLVER.Bone b, IReadOnlyList<FLVER.Bone> bones)
    {
        System.Numerics.Matrix4x4 result = System.Numerics.Matrix4x4.Identity;
        FLVER.Bone parentBone = b;
        while (parentBone != null)
        {
            System.Numerics.Matrix4x4 m = GetNMatrix(parentBone);
            result *= m;
            parentBone = GetParent(parentBone, bones);
        }
        return result;
    }

    private System.Numerics.Matrix4x4 GetNMatrix(FLVER.Bone b)
    {
        return System.Numerics.Matrix4x4.CreateScale(b.Scale)
            * System.Numerics.Matrix4x4.CreateRotationX(b.Rotation.X)
            * System.Numerics.Matrix4x4.CreateRotationZ(b.Rotation.Z)
            * System.Numerics.Matrix4x4.CreateRotationY(b.Rotation.Y)
            * System.Numerics.Matrix4x4.CreateTranslation(b.Translation);
    }

    private FLVER.Bone GetParent(FLVER.Bone b, IReadOnlyList<FLVER.Bone> bones)
    {
        if (b.ParentIndex >= 0 && b.ParentIndex < bones.Count) return bones[b.ParentIndex];
        return null;
    }

    private void UpdateBonesBoundingBox(FLVER.Bone b, IReadOnlyList<FLVER.Bone> bones, Vector3 vertexPos)
    {
        System.Numerics.Matrix4x4 boneAbsoluteMatrix = GetAbsoluteNMatrix(b, bones);
        if (!System.Numerics.Matrix4x4.Invert(boneAbsoluteMatrix, out System.Numerics.Matrix4x4 invertedBoneMatrix)) return;
        Vector3 posForBBox = Vector3.Transform(vertexPos, invertedBoneMatrix);
        float minX = Math.Min(b.BoundingBoxMin.X, posForBBox.X);
        float minY = Math.Min(b.BoundingBoxMin.Y, posForBBox.Y);
        float minZ = Math.Min(b.BoundingBoxMin.Z, posForBBox.Z);
        float maxX = Math.Max(b.BoundingBoxMax.X, posForBBox.X);
        float maxY = Math.Max(b.BoundingBoxMax.Y, posForBBox.Y);
        float maxZ = Math.Max(b.BoundingBoxMax.Z, posForBBox.Z);
        b.BoundingBoxMin = new Vector3(minX, minY, minZ);
        b.BoundingBoxMax = new Vector3(maxX, maxY, maxZ);
    }

    private record BoundingRange(Vector3 Max, Vector3 Min);
    private record BoneValue(BoundingRange Box, int Unk3C);

    private BoundingRange FlverHeader;
    private Dictionary<FLVER.Bone, BoneValue> oldBoneBoxes = new();
    private Dictionary<FLVER2.Mesh, BoundingRange> oldMeshBoxes = new();
    private readonly Action refresher;

    public SolveAllBBsAction(Action refresher)
    {
        this.refresher = refresher;
    }

    public override void Execute()
    {
        FlverHeader = new(MainWindow.Flver.Header.BoundingBoxMax, MainWindow.Flver.Header.BoundingBoxMin); 
        MainWindow.Flver.Header.BoundingBoxMin = new Vector3();
        MainWindow.Flver.Header.BoundingBoxMax = new Vector3();

        foreach (FLVER.Bone bone in MainWindow.Flver.Bones)
        {
            oldBoneBoxes.Add(bone, new (new (bone.BoundingBoxMax, bone.BoundingBoxMin), bone.Unk3C));
            bone.BoundingBoxMin = new Vector3();
            bone.BoundingBoxMax = new Vector3();
        }
        
        foreach (FLVER2.Mesh mesh in MainWindow.Flver.Meshes)
        {
            foreach (FLVER.Vertex vertex in mesh.Vertices)
            {
                UpdateHeaderBoundingBox(MainWindow.Flver.Header, vertex.Position);
                UpdateMeshBoundingBox(mesh, vertex.Position);
                if (Util3D.BoneIndicesToIntArray(vertex.BoneIndices) == null) continue;
                foreach (int boneIndex in Util3D.BoneIndicesToIntArray(vertex.BoneIndices))
                {
                    bool boneDoesNotExist = false;
                    if (boneIndex >= 0 && boneIndex < MainWindow.Flver.Bones.Count) MainWindow.Flver.Bones[boneIndex].Unk3C = 0;
                    else boneDoesNotExist = true;
                    if (!boneDoesNotExist) UpdateBonesBoundingBox(MainWindow.Flver.Bones[boneIndex], MainWindow.Flver.Bones, vertex.Position);
                }
            }
        }

        refresher.Invoke();
    }

    public override void Undo()
    {
        MainWindow.Flver.Header.BoundingBoxMin = FlverHeader.Min;
        MainWindow.Flver.Header.BoundingBoxMax = FlverHeader.Max;

        foreach (var item in oldBoneBoxes)
        {
            var bone = item.Key;
            var box = item.Value;

            bone.BoundingBoxMin = box.Box.Min;
            bone.BoundingBoxMax = box.Box.Max;
            bone.Unk3C= box.Unk3C;
        }

        foreach (var item in oldMeshBoxes)
        {
            var mesh = item.Key;
            var box = item.Value;

            mesh.BoundingBox.Min = box.Min;
            mesh.BoundingBox.Max = box.Max;
        }
    
        refresher.Invoke();
    }
}
