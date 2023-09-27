﻿using System.Numerics;
using FbxDataExtractor;
using SoulsAssetPipeline.FLVERImporting;
using SoulsFormats;
using static FLVER_Editor.Program;

namespace FLVER_Editor.FbxImporter.ViewModels;

public class FbxMeshDataViewModel
{
    public FbxMeshDataViewModel(FbxMeshData meshData)
    {
        Data = meshData;
        string[] nameParts = Data.Name.Split('|', StringSplitOptions.TrimEntries);
        Name = nameParts[0];
        if (Name == string.Empty)
        {
            Name = "Unknown";
        }

        if (nameParts.Length > 1) MTD = nameParts[1];
    }

    public string Name { get; set; }

    public string? MTD { get; }

    public FbxMeshData Data { get; }

    public static bool DoesSupportBoneWeights(string mtd)
    {
        List<FLVER2MaterialInfoBank.VertexBufferDeclaration> acceptableBufferDeclarations =
            MaterialInfoBank.MaterialDefs[mtd].AcceptableVertexBufferDeclarations;
        List<FLVER2.BufferLayout> bufferLayouts = acceptableBufferDeclarations[0].Buffers;
        return bufferLayouts.Any(x => x.Any(y => y.Semantic == FLVER.LayoutSemantic.BoneWeights));
    }

    public void ToFlverMesh(FLVER2 flver, MeshImportOptions options)
    {
        FLVER2.Material newMaterial = GetMaterialFromMTD(flver, Name, options.MTD);
        // TODO: Modify BufferLayouts when applying a material preset
        FLVER2.GXList gxList = GetGXListFromMTD(options.MTD);

        List<FLVER2MaterialInfoBank.VertexBufferDeclaration> acceptableBufferDeclarations =
            MaterialInfoBank.MaterialDefs[options.MTD].AcceptableVertexBufferDeclarations;

        List<FLVER2.BufferLayout> bufferLayouts = acceptableBufferDeclarations[0].Buffers;
        if (acceptableBufferDeclarations.Count > 1)
        {
            List<FLVER2.BufferLayout>? matchingLayouts = acceptableBufferDeclarations.FirstOrDefault(x =>
                x.Buffers.SelectMany(y => y).Count(y => y.Semantic == FLVER.LayoutSemantic.Tangent) >=
                Data.VertexData[0].Tangents.Count)?.Buffers;

            if (matchingLayouts != null) bufferLayouts = matchingLayouts;
        }

        AdjustBoneIndexBufferSize(flver, bufferLayouts);

        List<int> layoutIndices = GetLayoutIndices(flver, bufferLayouts);

        FLVER2.Mesh newMesh = new()
        {
            VertexBuffers = layoutIndices.Select(x => new FLVER2.VertexBuffer(x)).ToList(),
            MaterialIndex = flver.Materials.Count,
            Dynamic = (byte)(options.Weighting == WeightingMode.Skin ? 1 : 0)
        };

        int defaultBoneIndex = flver.Bones.IndexOf(flver.Bones.FirstOrDefault(x => x.Name == Name));
        if (defaultBoneIndex == -1)
        {
            if (options.CreateDefaultBone)
            {
                flver.Bones.Add(new FLVER.Bone { Name = Name });
                defaultBoneIndex = flver.Bones.Count - 1;
            }
            else
            {
                defaultBoneIndex = 0;
            }
        }

        newMesh.DefaultBoneIndex = defaultBoneIndex;

        HashSet<string> missingBones = new();
        foreach (FbxVertexData vertexData in Data.VertexData)
        {
            FLVER.Vertex newVertex = new()
            {
                Position = vertexData.Position,
                Normal = vertexData.Normal,
                Bitangent = new Vector4(-1, -1, -1, -1),
                Tangents = new List<Vector4>(vertexData.Tangents.Select(x => x)),
                UVs = new List<Vector3>(vertexData.UVs.Select(x => new Vector3(x.X, 1 - x.Y, 0.0f))),
                // Fbx uses RGBA, SF uses ARGB
                Colors = new List<FLVER.VertexColor>(vertexData.Colors.Select(x =>
                    new FLVER.VertexColor(x.W, x.X, x.Y, x.Z))),
            };

            FLVER.VertexBoneIndices boneIndices = new();
            FLVER.VertexBoneWeights boneWeights = new();
            List<(string Name, float Weight)> orderedWeightData = vertexData.BoneNames
                .Zip(vertexData.BoneWeights).OrderByDescending(x => x.Item2).ToList();
            for (int j = 0; j < Math.Min(orderedWeightData.Count, 4); j++)
            {
                (string boneName, float boneWeight) = orderedWeightData[j];
                int boneIndex = flver.Bones.IndexOf(flver.Bones.FirstOrDefault(x => x.Name == boneName));
                if (boneIndex == -1)
                {
                    missingBones.Add(boneName);
                    boneIndex = 0;
                }

                boneIndices[j] = boneIndex;
                boneWeights[j] = boneWeight;
            }

            if (options.Weighting == WeightingMode.Single)
            {
                newVertex.NormalW = boneWeights[0] > 0 ? boneIndices[0] : -1;
            }
            else if (options.Weighting == WeightingMode.Skin)
            {
                newVertex.BoneIndices = boneIndices;
                newVertex.BoneWeights = boneWeights;
            }

            PadVertex(newVertex, bufferLayouts);
            newMesh.Vertices.Add(newVertex);
        }
        FlipFaceSet();
        FLVER2.FaceSet.FSFlags[] faceSetFlags =
        {
            FLVER2.FaceSet.FSFlags.None,
            FLVER2.FaceSet.FSFlags.LodLevel1,
            FLVER2.FaceSet.FSFlags.LodLevel2,
            FLVER2.FaceSet.FSFlags.MotionBlur,
            FLVER2.FaceSet.FSFlags.MotionBlur | FLVER2.FaceSet.FSFlags.LodLevel1,
            FLVER2.FaceSet.FSFlags.MotionBlur | FLVER2.FaceSet.FSFlags.LodLevel2
        };
        List<FLVER2.FaceSet> faceSets = new();
        foreach (FLVER2.FaceSet.FSFlags faceSetFlag in faceSetFlags)
        {
            faceSets.Add(new FLVER2.FaceSet
            {
                Indices = Data.VertexIndices,
                CullBackfaces = false,
                Flags = faceSetFlag,
                TriangleStrip = false
            });
        }
        newMesh.FaceSets.AddRange(faceSets);
        newMesh.MaterialIndex = flver.Materials.Count;
        if (flver.GXLists.Count > 0) newMaterial.GXIndex = flver.GXLists.Count;
        else newMaterial.GXIndex = -1;
        flver.Materials.Add(newMaterial);
        if (flver.GXLists.Count > 0) flver.GXLists.Add(gxList);
        flver.Meshes.Add(newMesh);
    }

    private static void AdjustBoneIndexBufferSize(FLVER2 flver, List<FLVER2.BufferLayout> bufferLayouts)
    {
        if (flver.Bones.Count <= byte.MaxValue) return;
        foreach (FLVER2.BufferLayout bufferLayout in bufferLayouts)
        {
            foreach (FLVER.LayoutMember layoutMember in bufferLayout.Where(x => x.Semantic == FLVER.LayoutSemantic.BoneIndices))
            {
                layoutMember.Type = FLVER.LayoutType.ShortBoneIndices;
            }
        }
    }

    private static void PadVertex(FLVER.Vertex vertex, IEnumerable<FLVER2.BufferLayout> bufferLayouts)
    {
        Dictionary<FLVER.LayoutSemantic, int> usageCounts = new();
        FLVER.LayoutSemantic[] paddedProperties =
            { FLVER.LayoutSemantic.Tangent, FLVER.LayoutSemantic.UV, FLVER.LayoutSemantic.VertexColor };
        IEnumerable<FLVER.LayoutMember> layoutMembers = bufferLayouts.SelectMany(bufferLayout => bufferLayout)
            .Where(x => paddedProperties.Contains(x.Semantic));
        foreach (FLVER.LayoutMember layoutMember in layoutMembers)
        {
            bool isDouble = layoutMember is { Semantic: FLVER.LayoutSemantic.UV, Type: FLVER.LayoutType.Float4 or FLVER.LayoutType.UVPair };
            int count = isDouble ? 2 : 1;
            if (usageCounts.ContainsKey(layoutMember.Semantic))
            {
                usageCounts[layoutMember.Semantic] += count;
            }
            else
            {
                usageCounts.Add(layoutMember.Semantic, count);
            }
        }
        if (usageCounts.ContainsKey(FLVER.LayoutSemantic.Tangent))
        {
            int missingTangentCount = usageCounts[FLVER.LayoutSemantic.Tangent] - vertex.Tangents.Count;
            for (int i = 0; i < missingTangentCount; i++)
            {
                vertex.Tangents.Add(Vector4.Zero);
            }
        }
        if (usageCounts.TryGetValue(FLVER.LayoutSemantic.UV, out int uvCount))
        {
            int missingUvCount = uvCount - vertex.UVs.Count;
            for (int i = 0; i < missingUvCount; i++)
            {
                vertex.UVs.Add(Vector3.Zero);
            }
        }
        if (usageCounts.TryGetValue(FLVER.LayoutSemantic.VertexColor, out int colorCount))
        {
            int missingColorCount = colorCount - vertex.Colors.Count;
            for (int i = 0; i < missingColorCount; i++)
            {
                vertex.Colors.Add(new FLVER.VertexColor(255, 255, 255, 255));
            }
        }
    }

    private static List<int> GetLayoutIndices(FLVER2 flver, List<FLVER2.BufferLayout> bufferLayouts)
    {
        List<int> indices = new();
        foreach (FLVER2.BufferLayout referenceBufferLayout in bufferLayouts)
        {
            for (int i = 0; i < flver.BufferLayouts.Count; i++)
            {
                FLVER2.BufferLayout bufferLayout = flver.BufferLayouts[i];
                if (bufferLayout.SequenceEqual(referenceBufferLayout, new LayoutMemberComparer()))
                {
                    indices.Add(i);
                    break;
                }

                if (i != flver.BufferLayouts.Count - 1) continue;

                indices.Add(i + 1);
                flver.BufferLayouts.Add(referenceBufferLayout);
                break;
            }
        }
        return indices;
    }

    private void FlipFaceSet()
    {
        for (int i = 0; i < Data.VertexIndices.Count; i += 3)
        {
            (Data.VertexIndices[i + 1], Data.VertexIndices[i + 2]) =
                (Data.VertexIndices[i + 2], Data.VertexIndices[i + 1]);
        }
    }

    private class LayoutMemberComparer : IEqualityComparer<FLVER.LayoutMember>
    {
        public bool Equals(FLVER.LayoutMember? x, FLVER.LayoutMember? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return false;
            if (y is null) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Unk00 == y.Unk00 && x.Type == y.Type && x.Semantic == y.Semantic && x.Index == y.Index &&
                   x.Size == y.Size;
        }

        public int GetHashCode(FLVER.LayoutMember obj)
        {
            return HashCode.Combine(obj.Unk00, (int)obj.Type, (int)obj.Semantic, obj.Index, obj.Size);
        }
    }
}