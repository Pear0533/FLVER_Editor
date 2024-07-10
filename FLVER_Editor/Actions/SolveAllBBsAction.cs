using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SoulsAssetPipeline.FLVERImporting;

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
        float minX = Math.Min(mesh.BoundingBox.Min.X, vertexPos.X);
        float minY = Math.Min(mesh.BoundingBox.Min.Y, vertexPos.Y);
        float minZ = Math.Min(mesh.BoundingBox.Min.Z, vertexPos.Z);
        float maxX = Math.Max(mesh.BoundingBox.Max.X, vertexPos.X);
        float maxY = Math.Max(mesh.BoundingBox.Max.Y, vertexPos.Y);
        float maxZ = Math.Max(mesh.BoundingBox.Max.Z, vertexPos.Z);
        mesh.BoundingBox.Min = new Vector3(minX, minY, minZ);
        mesh.BoundingBox.Max = new Vector3(maxX, maxY, maxZ);
    }

    private System.Numerics.Matrix4x4 GetAbsoluteNMatrix(FLVER.Node b, IReadOnlyList<FLVER.Node> bones)
    {
        System.Numerics.Matrix4x4 result = System.Numerics.Matrix4x4.Identity;
        FLVER.Node parentNode = b;
        while (parentNode != null)
        {
            System.Numerics.Matrix4x4 m = GetNMatrix(parentNode);
            result *= m;
            parentNode = GetParent(parentNode, bones);
        }
        return result;
    }

    private System.Numerics.Matrix4x4 GetNMatrix(FLVER.Node b)
    {
        return System.Numerics.Matrix4x4.CreateScale(b.Scale)
            * System.Numerics.Matrix4x4.CreateRotationX(b.Rotation.X)
            * System.Numerics.Matrix4x4.CreateRotationZ(b.Rotation.Z)
            * System.Numerics.Matrix4x4.CreateRotationY(b.Rotation.Y)
            * System.Numerics.Matrix4x4.CreateTranslation(b.Translation);
    }

    private FLVER.Node GetParent(FLVER.Node b, IReadOnlyList<FLVER.Node> bones)
    {
        if (b.ParentIndex >= 0 && b.ParentIndex < bones.Count) return bones[b.ParentIndex];
        return null;
    }

    private void UpdateNodesBoundingBox(FLVER.Node b, IReadOnlyList<FLVER.Node> bones, Vector3 vertexPos)
    {
        System.Numerics.Matrix4x4 boneAbsoluteMatrix = GetAbsoluteNMatrix(b, bones);
        if (!System.Numerics.Matrix4x4.Invert(boneAbsoluteMatrix, out System.Numerics.Matrix4x4 invertedNodeMatrix)) return;
        Vector3 posForBBox = Vector3.Transform(vertexPos, invertedNodeMatrix);
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
    private record NodeValue(BoundingRange Box, FLVER.Node.NodeFlags Flags);

    private BoundingRange? FlverHeader;
    private Dictionary<FLVER.Node, NodeValue> oldNodeBoxes = new();
    private Dictionary<FLVER2.Mesh, BoundingRange?> oldMeshBoxes = new();
    private readonly FLVER2 flver;
    private readonly Action refresher;

    public SolveAllBBsAction(FLVER2 flver, Action refresher)
    {
        this.flver = flver;
        this.refresher = refresher;
    }

    public override void Execute()
    {
        FlverHeader = new(flver.Header.BoundingBoxMax, flver.Header.BoundingBoxMin);
        flver.Header.BoundingBoxMin = new System.Numerics.Vector3();
        flver.Header.BoundingBoxMax = new System.Numerics.Vector3();

        foreach (FLVER.Node bone in flver.Nodes)
        {
            oldNodeBoxes.Add(bone, new(new(bone.BoundingBoxMax, bone.BoundingBoxMin), bone.Flags));
            bone.BoundingBoxMin = new System.Numerics.Vector3();
            bone.BoundingBoxMax = new System.Numerics.Vector3();
        }

        foreach (FLVER2.Mesh mesh in flver.Meshes)
        {
            oldMeshBoxes.Add(mesh, mesh.BoundingBox is null ? null : new BoundingRange(mesh.BoundingBox.Max, mesh.BoundingBox.Min));
            mesh.BoundingBox = new FLVER2.Mesh.BoundingBoxes();

            foreach (FLVER.Vertex vertex in mesh.Vertices)
            {
                flver.Header.UpdateBoundingBox(vertex.Position);
                mesh.UpdateBoundingBox(vertex.Position);

                if (mesh.UseBoneWeights)
                {
                    for (int j = 0; j < vertex.BoneIndices.Length; j++)
                    {
                        if (vertex.BoneWeights[j] == 0) continue;
                        int nodeIndex = vertex.BoneIndices[j];

                        if (nodeIndex < 0 || nodeIndex >= flver.Nodes.Count) continue;

                        FLVER.Node node = flver.Nodes[nodeIndex];
                        if (!node.Flags.HasFlag(FLVER.Node.NodeFlags.Disabled))
                        {
                            node.UpdateBoundingBox(flver.Nodes, vertex.Position);
                        }
                    }
                }
                else
                {
                    int nodeIndex = vertex.NormalW;
                    if (nodeIndex < 0 || nodeIndex >= flver.Nodes.Count) continue;

                    FLVER.Node node = flver.Nodes[nodeIndex];
                    if (!node.Flags.HasFlag(FLVER.Node.NodeFlags.Disabled))
                    {
                        node.UpdateBoundingBox(flver.Nodes, vertex.Position);
                    }
                }
            }
        }

        refresher.Invoke();
    }

    public override void Undo()
    {
        MainWindow.Flver.Header.BoundingBoxMin = FlverHeader?.Min ?? throw new Exception("Undo run without execute first");
        MainWindow.Flver.Header.BoundingBoxMax = FlverHeader?.Max ?? throw new Exception("Undo run without execute first");

        foreach (var item in oldNodeBoxes)
        {
            var bone = item.Key;
            var box = item.Value;

            bone.BoundingBoxMin = box.Box.Min;
            bone.BoundingBoxMax = box.Box.Max;
            bone.Flags = box.Flags;
        }

        foreach (var item in oldMeshBoxes)
        {
            var mesh = item.Key;
            var box = item.Value;

            if (box is null)
            {
                mesh.BoundingBox = null;
            }
            else
            {
                mesh.BoundingBox.Min = box.Min;
                mesh.BoundingBox.Max = box.Max;
            }
        }

        refresher.Invoke();
    }
}
