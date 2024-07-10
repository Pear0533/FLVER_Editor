using System.Collections.Generic;
using System.Linq;
using SoulsAssetPipeline.FLVERImporting;
using SoulsFormats;

namespace FbxImporter.Util;

public static class FlverUtils
{
    public static FLVER2.BufferLayout Clone(this FLVER2.BufferLayout layout)
    {
        FLVER2.BufferLayout newLayout = new();
        newLayout.AddRange(layout.Select(x => new FLVER.LayoutMember(x.Type, x.Semantic, x.Index, x.Unk00)));
        return newLayout;
    }

    public static FLVER2.GXList Clone(this FLVER2.GXList gxList)
    {
        FLVER2.GXList newGXList = new();
        newGXList.AddRange(gxList.Select(x => new FLVER2.GXItem
        {
            Data = (byte[]) x.Data.Clone(),
            ID = x.ID,
            Unk04 = x.Unk04
        }));
        return newGXList;
    }

    public static FLVER2.Texture Clone(this FLVER2.Texture texture)
    {
        return new FLVER2.Texture
        {
            Type = texture.Type,
            Path = texture.Path,
            Scale = texture.Scale,
            Unk10 = texture.Unk10,
            Unk11 = texture.Unk11,
            Unk14 = texture.Unk14,
            Unk18 = texture.Unk18,
            Unk1C = texture.Unk1C
        };
    }

    public static FLVER2.Material Clone(this FLVER2.Material material)
    {
        return new FLVER2.Material
        {
            GXIndex = material.GXIndex,
            MTD = material.MTD,
            Name = material.Name,
            Textures = material.Textures.Select(Clone).ToList(),
            Index = material.Index
        };
    }

    public static void FlipFaceSets(this FLVER2.Mesh mesh)
    {
        foreach (FLVER2.FaceSet faceSet in mesh.FaceSets)
        {
            faceSet.Flip();
        }
    }

    public static void Flip(this FLVER2.FaceSet faceSet)
    {
        for (int i = 0; i < faceSet.Indices.Count; i += 3)
        {
            (faceSet.Indices[i + 1], faceSet.Indices[i + 2]) = (faceSet.Indices[i + 2], faceSet.Indices[i + 1]);
        }
    }
    
    public static void AddNodesToSkeleton(this FLVER2 flver, List<FLVER2.SkeletonSet.Bone> skeleton)
    {
        if (skeleton.Count == 0 || skeleton.Count == flver.Nodes.Count) return;

        for (int i = 0; i < flver.Nodes.Count; i++)
        {
            if (skeleton.Any(bone => bone.NodeIndex == i)) continue;

            FLVER.Node node = flver.Nodes[i];
            short parentIndex = (short)skeleton.FindIndex(x => x.NodeIndex == node.ParentIndex);
            short childIndex = (short)skeleton.FindIndex(x => x.NodeIndex == node.FirstChildIndex);
            short prevIndex = (short)skeleton.FindIndex(x => x.NodeIndex == node.PreviousSiblingIndex);
            short nextIndex = (short)skeleton.FindIndex(x => x.NodeIndex == node.NextSiblingIndex);

            if (prevIndex != -1)
            {
                skeleton[prevIndex].NextSiblingIndex = (short)skeleton.Count;
            }
                
            skeleton.Add(new FLVER2.SkeletonSet.Bone(i)
            {
                ParentIndex = parentIndex,
                FirstChildIndex = childIndex,
                PreviousSiblingIndex = prevIndex,
                NextSiblingIndex = nextIndex
            });
        }
    }

    public static void AddNodesToSkeletons(this FLVER2 flver)
    {
        if (flver.Skeletons == null) return;
        flver.AddNodesToSkeleton(flver.Skeletons.BaseSkeleton);
        flver.AddNodesToSkeleton(flver.Skeletons.AllSkeletons);
    }

    private static void SetNodeFlag(FLVER.Node node, FLVER.Node.NodeFlags flag)
    {
        node.Flags |= flag;
        node.Flags &= flag == FLVER.Node.NodeFlags.Disabled ? FLVER.Node.NodeFlags.Disabled : ~FLVER.Node.NodeFlags.Disabled;
    }
    
    public static void SetNodeFlags(this FLVER2 flver)
    {
        foreach (FLVER2.Mesh mesh in flver.Meshes)
        {
            foreach (FLVER.Vertex vertex in mesh.Vertices)
            {
                if (mesh.UseBoneWeights)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        int boneIndex = vertex.BoneIndices[j];
                        if (vertex.BoneWeights[j] == 0 || boneIndex < 0 || boneIndex >= flver.Nodes.Count) continue;
                        FLVER.Node node = flver.Nodes[boneIndex];
                        SetNodeFlag(node, FLVER.Node.NodeFlags.Bone);
                    }
                }
                else if (vertex.NormalW >= 0 && vertex.NormalW < flver.Nodes.Count)
                {
                    int normalW = vertex.NormalW;
                    FLVER.Node node = flver.Nodes[normalW];
                    SetNodeFlag(node, FLVER.Node.NodeFlags.Bone);
                }
            }

            int meshNodeIndex = mesh.NodeIndex;
            if (meshNodeIndex == -1) continue;
            if (meshNodeIndex < 0 || meshNodeIndex >= flver.Nodes.Count) continue;

            FLVER.Node meshNode = flver.Nodes[meshNodeIndex];
            SetNodeFlag(meshNode, FLVER.Node.NodeFlags.Mesh);
        }

        foreach (FLVER.Dummy dummy in flver.Dummies)
        {
            if (dummy.AttachBoneIndex != -1)
            {
                if (dummy.AttachBoneIndex < 0 || dummy.AttachBoneIndex >= flver.Nodes.Count) continue;
                FLVER.Node node = flver.Nodes[dummy.AttachBoneIndex];
                SetNodeFlag(node, FLVER.Node.NodeFlags.DummyOwner);
            }

            if (dummy.ParentBoneIndex != -1) continue;
            if (dummy.ParentBoneIndex < 0 || dummy.ParentBoneIndex >= flver.Nodes.Count) continue;
            {
                FLVER.Node node = flver.Nodes[dummy.ParentBoneIndex];
                SetNodeFlag(node, FLVER.Node.NodeFlags.DummyOwner);
            }
        }

        foreach (FLVER.Node node in flver.Nodes.Where(x => x is { ParentIndex: -1, FirstChildIndex: -1, PreviousSiblingIndex: -1, NextSiblingIndex: -1, Flags: 0 }))
        {
            SetNodeFlag(node, FLVER.Node.NodeFlags.Disabled);
        }
    }

    public static void FixAllBoundingBoxes(this FLVER2 flver)
    {
        flver.Header.BoundingBoxMin = new System.Numerics.Vector3();
        flver.Header.BoundingBoxMax = new System.Numerics.Vector3();
        foreach (FLVER.Node bone in flver.Nodes)
        {
            bone.BoundingBoxMin = new System.Numerics.Vector3();
            bone.BoundingBoxMax = new System.Numerics.Vector3();
        }

        foreach (FLVER2.Mesh mesh in flver.Meshes)
        {
            mesh.BoundingBox = new FLVER2.Mesh.BoundingBoxes();

            foreach (FLVER.Vertex vertex in mesh.Vertices)
            {
                flver.Header.UpdateBoundingBox(vertex.Position);
                if (mesh.BoundingBox != null)
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
    }
}