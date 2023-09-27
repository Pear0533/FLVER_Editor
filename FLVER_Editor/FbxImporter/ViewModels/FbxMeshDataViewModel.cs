using System.Numerics;
using FbxDataExtractor;
using SoulsAssetPipeline.FLVERImporting;
using SoulsFormats;
using FbxVertexData = FbxDataExtractor.FbxVertexData;
using static FLVER_Editor.Program;

namespace FLVER_Editor.FbxImporter.ViewModels;

public class FbxMeshDataViewModel
{
    public FbxMeshDataViewModel(FbxMeshData meshData)
    {
        Data = meshData;
    }

    public string Name => Data.Name;

    public FbxMeshData Data { get; }

    public static bool DoesSupportBoneWeights(string mtd)
    {
        List<FLVER2MaterialInfoBank.VertexBufferDeclaration> acceptableBufferDeclarations =
            MaterialInfoBank.MaterialDefs[mtd].AcceptableVertexBufferDeclarations;
        List<FLVER2.BufferLayout> bufferLayouts = acceptableBufferDeclarations[0].Buffers;
        return bufferLayouts.Any(x => x.Any(y => y.Semantic == FLVER.LayoutSemantic.BoneWeights));
    }

    public FlverMeshViewModel ToFlverMesh(FLVER2 flver, MeshImportOptions options)
    {
        string[] nameParts = Data.Name.Split('|', StringSplitOptions.TrimEntries);
        string name = nameParts.Length > 1 ? nameParts[0] : Data.Name;
        FLVER2.Material newMaterial = new()
        {
            Name = name,
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
                x.Buffers.SelectMany(y => y).Count(y => y.Semantic == FLVER.LayoutSemantic.Tangent) >= Data.VertexData[0].Tangents.Count)?.Buffers;
            if (matchingLayouts != null)
            {
                bufferLayouts = matchingLayouts;
            }
        }
        AdjustBoneIndexBufferSize(flver, bufferLayouts);
        List<int> layoutIndices = GetLayoutIndices(flver, bufferLayouts);
        FLVER2.Mesh newMesh = new()
        {
            VertexBuffers = layoutIndices.Select(x => new FLVER2.VertexBuffer(x)).ToList(),
            Dynamic = (byte)(!options.IsStatic && bufferLayouts.Any(x => x.Any(y => y.Semantic == FLVER.LayoutSemantic.BoneWeights))
                ? 1
                : 0)
        };
        int defaultBoneIndex = flver.Bones.IndexOf(flver.Bones.FirstOrDefault(x => x.Name == Data.Name));
        if (defaultBoneIndex == -1)
        {
            if (options.CreateDefaultBone)
            {
                flver.Bones.Add(new FLVER.Bone { Name = name });
                defaultBoneIndex = flver.Bones.Count - 1;
            }
            else
            {
                defaultBoneIndex = 0;
            }
        }
        newMesh.DefaultBoneIndex = defaultBoneIndex;
        foreach (FbxVertexData vertexData in Data.VertexData)
        {
            FLVER.VertexBoneIndices boneIndices = new();
            FLVER.VertexBoneWeights boneWeights = new();
            List<(string, float)> orderedWeightData = vertexData.BoneNames
                .Select((s, i) => (s, vertexData.BoneWeights[i])).OrderByDescending(x => x.Item2).ToList();
            for (int j = 0; j < Math.Min(orderedWeightData.Count, 4); j++)
            {
                int boneIndex = GetBoneIndexFromName(flver, orderedWeightData[j].Item1);
                boneIndices[j] = boneIndex;
                boneWeights[j] = orderedWeightData[j].Item2;
            }
            int zSign = options.MirrorZ ? -1 : 1;
            FLVER.Vertex newVertex = new()
            {
                Position = vertexData.Position with { Z = vertexData.Position.Z * zSign },
                Normal = vertexData.Normal with { Z = vertexData.Normal.Z * zSign },
                Bitangent = new Vector4(-1, -1, -1, -1),
                Tangents = new List<Vector4>(vertexData.Tangents.Select(x => x with { Z = x.Z * zSign })),
                UVs = new List<Vector3>(vertexData.UVs.Select(x => new Vector3(x.X, 1 - x.Y, 0.0f))),
                // Fbx uses RGBA, SF uses ARGB
                Colors = new List<FLVER.VertexColor>(vertexData.Colors.Select(x =>
                    new FLVER.VertexColor(x.W, x.X, x.Y, x.Z))),
                BoneIndices = boneIndices,
                BoneWeights = boneWeights
            };
            PadVertex(newVertex, bufferLayouts);
            newMesh.Vertices.Add(newVertex);
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
        FlverMeshViewModel output = new(newMesh, newMaterial, gxList);
        return output;
    }

    private static void AdjustBoneIndexBufferSize(FLVER2 flver, List<FLVER2.BufferLayout> bufferLayouts)
    {
        if (flver.Bones.Count <= byte.MaxValue) return;
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
            bool isDouble = layoutMember.Semantic == FLVER.LayoutSemantic.UV && layoutMember.Type is FLVER.LayoutType.Float4 or FLVER.LayoutType.UVPair;
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
        if (usageCounts.ContainsKey(FLVER.LayoutSemantic.UV))
        {
            int missingUvCount = usageCounts[FLVER.LayoutSemantic.UV] - vertex.UVs.Count;
            for (int i = 0; i < missingUvCount; i++)
            {
                vertex.UVs.Add(Vector3.Zero);
            }
        }
        if (usageCounts.ContainsKey(FLVER.LayoutSemantic.VertexColor))
        {
            int missingColorCount = usageCounts[FLVER.LayoutSemantic.VertexColor] - vertex.Colors.Count;
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
            return x.Unk00 == y.Unk00 && x.Type == y.Type && x.Semantic == y.Semantic && x.Index == y.Index && x.Size == y.Size;
        }

        public int GetHashCode(FLVER.LayoutMember obj)
        {
            return HashCode.Combine(obj.Unk00, (int)obj.Type, (int)obj.Semantic, obj.Index, obj.Size);
        }
    }

    private void FlipFaceSet()
    {
        for (int i = 0; i < Data.VertexIndices.Count; i += 3)
        {
            (Data.VertexIndices[i + 1], Data.VertexIndices[i + 2]) =
                (Data.VertexIndices[i + 2], Data.VertexIndices[i + 1]);
        }
    }

    private static int GetBoneIndexFromName(FLVER2 flver, string boneName)
    {
        if (boneName == "0")
        {
            return 0;
        }
        int boneIndex = flver.Bones.IndexOf(flver.Bones.FirstOrDefault(x => x.Name == boneName));
        if (boneIndex != -1)
        {
            return boneIndex;
        }
        MainWindow.ShowInformationDialog($"No Bone with name {boneName} found, bone index set to 0");
        return 0;
    }
}