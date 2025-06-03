﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FbxDataExtractor;
using FbxImporter.Views;
using SoulsAssetPipeline.FLVERImporting;
using SoulsFormats;

namespace FbxImporter.ViewModels;

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
            Logger.Log("Encountered mesh with no name, renaming to \"Unknown\".");
        }

        if (nameParts.Length > 1) MTD = nameParts[1];

        if (Data.VertexData.Count == 0)
        {
            Logger.LogWarning($"Mesh {Name} contains no vertices.");
        }
    }

    public string Name { get; }

    public string? MTD { get; }

    public FbxMeshData Data { get; }

    public static bool DoesSupportBoneWeights(string? mtd, WeightingMode mode, KeyValuePair<FbxMeshDataViewModel, MeshImportOptions> mesh)
    {
        if (mesh.Value == null) return false;
        List<FLVER2MaterialInfoBank.VertexBufferDeclaration> acceptableBufferDeclarations =
            mesh.Value.MaterialInfoBank.MaterialDefs[mtd ?? ""].AcceptableVertexBufferDeclarations;
        List<FLVER2.BufferLayout> bufferLayouts = acceptableBufferDeclarations[0].Buffers;
        return mode != WeightingMode.Skin || bufferLayouts.SelectMany(x => x).Any(x => x.Semantic == FLVER.LayoutSemantic.BoneWeights);
    }

    public FlverMeshViewModel ToFlverMesh(FLVER2 flver, MeshImportOptions options)
    {
        FLVER2.Material newMaterial = new()
        {
            Name = Name,
            MTD = options.MTD,
            Textures = new List<FLVER2.Texture>(options.MaterialInfoBank.MaterialDefs[options.MTD].TextureChannels
                .Values.Select(x => new FLVER2.Texture { Type = x }))
        };

        FLVER2.GXList gxList = new();
        gxList.AddRange(options.MaterialInfoBank.GetDefaultGXItemsForMTD(options.MTD));

        List<FLVER2MaterialInfoBank.VertexBufferDeclaration> acceptableBufferDeclarations =
            options.MaterialInfoBank.MaterialDefs[options.MTD].AcceptableVertexBufferDeclarations;

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

        if (options.Weighting == WeightingMode.Skin && bufferLayouts.SelectMany(x => x)
                .All(x => x.Semantic != FLVER.LayoutSemantic.BoneWeights))
        {
            Logger.LogWarning("The selected material does not support the skin weighting mode.");
        }

        FLVER2.Mesh newMesh = new()
        {
            VertexBuffers = layoutIndices.Select(x => new FLVER2.VertexBuffer(x)).ToList(),
            UseBoneWeights = options.Weighting == WeightingMode.Skin
        };

        int nodeIndex = flver.Nodes.IndexOf(flver.Nodes.FirstOrDefault(x => x.Name == Name));
        if (nodeIndex == -1)
        {
            nodeIndex = flver.Nodes.FindIndex(x => x.Flags.HasFlag(FLVER.Node.NodeFlags.Disabled));
            if (nodeIndex == -1)
            {
                nodeIndex = flver.Nodes.Count;
            }

            int lastRootNodeIndex = flver.Nodes.FindLastIndex(x =>
                x is { ParentIndex: -1, NextSiblingIndex: -1 } && !x.Flags.HasFlag(FLVER.Node.NodeFlags.Disabled));
            
            flver.Nodes.Insert(nodeIndex, new FLVER.Node
            {
                Name = Name,
                Flags = flver.Header.Version >= 131098 ? FLVER.Node.NodeFlags.Mesh : 0,
                PreviousSiblingIndex = (short)lastRootNodeIndex
            });

            if (lastRootNodeIndex != -1)
            {
                flver.Nodes[lastRootNodeIndex].NextSiblingIndex = (short)nodeIndex;
            }
        }

        newMesh.NodeIndex = nodeIndex;

        bool foundWeights = false;
        HashSet<string> missingBones = new();
        foreach (FbxVertexData vertexData in Data.VertexData)
        {
            int zSign = options.MirrorZ ? -1 : 1;

            FLVER.Vertex newVertex = new()
            {
                Position = vertexData.Position with { Z = vertexData.Position.Z * zSign },
                Normal = vertexData.Normal with { Z = vertexData.Normal.Z * zSign },
                //bitangent is usually not an actual bitangent
                //Bitangent = vertexData.Bitangent with { Z = vertexData.Bitangent.Z * zSign },
                Bitangent = new Vector4(-1.0f),
                Tangents = vertexData.Tangents.Select(x => x with { Z = x.Z * zSign }).ToList(),
                UVs = vertexData.UVs.Select(x => new Vector3(x.X, 1 - x.Y, 0.0f)).ToList(),
                // Fbx uses RGBA, SF uses ARGB
                Colors = vertexData.Colors.Select(x => new FLVER.VertexColor(x.W, x.X, x.Y, x.Z)).ToList(),
            };


            FLVER.VertexBoneIndices boneIndices = new();
            FLVER.VertexBoneWeights boneWeights = new();
            List<(string Name, float Weight)> orderedWeightData = vertexData.BoneNames
                .Zip(vertexData.BoneWeights).OrderByDescending(x => x.Item2).ToList();
            for (int j = 0; j < Math.Min(orderedWeightData.Count, 4); j++)
            {
                (string boneName, float boneWeight) = orderedWeightData[j];
                int boneIndex = flver.Nodes.IndexOf(flver.Nodes.FirstOrDefault(x => x.Name == boneName));
                if (boneIndex == -1)
                {
                    missingBones.Add(boneName);
                    boneIndex = 0;
                }

                if (!foundWeights && boneWeight > 0) foundWeights = true;

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

        if (!foundWeights && options.Weighting != WeightingMode.Static)
        {
            Logger.LogWarning("Non-static weighting mode selected but mesh contains no bone weights.");
        }

        foreach (string missingBone in missingBones)
        {
            Logger.LogWarning($"No bone with name {missingBone} found, bone index set to 0");
        }

        FLVER2.FaceSet.FSFlags[] faceSetFlags =
        {
            FLVER2.FaceSet.FSFlags.None,
            FLVER2.FaceSet.FSFlags.LodLevel1,
            FLVER2.FaceSet.FSFlags.LodLevel2,
            FLVER2.FaceSet.FSFlags.MotionBlur,
            FLVER2.FaceSet.FSFlags.MotionBlur | FLVER2.FaceSet.FSFlags.LodLevel1,
            FLVER2.FaceSet.FSFlags.MotionBlur | FLVER2.FaceSet.FSFlags.LodLevel2
        };

        List<int> vertexIndices = new(Data.VertexIndices);
        if (options.FlipFaces)
        {
            FlipFaceSet(vertexIndices);
        }
        
        List<FLVER2.FaceSet> faceSets = new();
        foreach (FLVER2.FaceSet.FSFlags faceSetFlag in faceSetFlags)
        {
            faceSets.Add(new FLVER2.FaceSet
            {
                Indices = vertexIndices,
                CullBackfaces = false,
                Flags = faceSetFlag,
                TriangleStrip = false
            });
        }

        newMesh.FaceSets.AddRange(faceSets);

        FlverMeshViewModel output = new(newMesh, newMaterial, gxList);
        return output;
    }

    private static void AdjustBoneIndexBufferSize(FLVER2 flver, List<FLVER2.BufferLayout> bufferLayouts)
    {
        if (flver.Nodes.FindLastIndex(x => !x.Flags.HasFlag(FLVER.Node.NodeFlags.Disabled)) <= byte.MaxValue) return;

        foreach (FLVER2.BufferLayout bufferLayout in bufferLayouts)
        {
            foreach (FLVER.LayoutMember layoutMember in bufferLayout.Where(x =>
                         x.Semantic == FLVER.LayoutSemantic.BoneIndices))
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
            bool isDouble = layoutMember is
                { Semantic: FLVER.LayoutSemantic.UV, Type: FLVER.LayoutType.Float4 or FLVER.LayoutType.UVPair };
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

        if (usageCounts.TryGetValue(FLVER.LayoutSemantic.Tangent, out int tangentCount))
        {
            int missingTangentCount = tangentCount - vertex.Tangents.Count;
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

    private static void FlipFaceSet(IList<int> vertexIndices)
    {
        for (int i = 0; i < vertexIndices.Count; i += 3)
        {
            (vertexIndices[i + 1], vertexIndices[i + 2]) =
                (vertexIndices[i + 2], vertexIndices[i + 1]);
        }
    }
}