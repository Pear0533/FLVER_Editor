using System.Numerics;
using FbxDataExtractor;
using SoulsAssetPipeline.FLVERImporting;
using SoulsFormats;
using static FLVER_Editor.Program;

namespace FLVER_Editor.FbxImporter.ViewModels;

public class FbxMeshDataViewModel
{
    public FbxMeshDataViewModel(FbxMeshData meshData)
    {
        Name = meshData.Name;
        VertexData = meshData.VertexData.Select(x =>
                new FbxVertexData(x.Position, x.Normal, x.Tangents, x.UVs, x.BoneNames.ToArray(),
                    x.BoneWeights.ToArray()))
            .ToList();
        VertexIndices = meshData.VertexIndices;
    }

    public string Name { get; set; }

    private List<FbxVertexData> VertexData { get; }

    private List<int> VertexIndices { get; }

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
            List<FLVER2.BufferLayout> matchingLayouts = acceptableBufferDeclarations.FirstOrDefault(x =>
                x.Buffers.SelectMany(y => y).Count(y => y.Semantic == FLVER.LayoutSemantic.Tangent) >= VertexData[0].Tangents.Count)?.Buffers;
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
            MaterialIndex = flver.Materials.Count,
            Dynamic = (byte)(!options.IsStatic && DoesSupportBoneWeights(options.MTD) ? 1 : 0)
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
        foreach (FbxVertexData vertexData in VertexData)
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
                Position = new Vector3(vertexData.Position[0], vertexData.Position[1], zSign * vertexData.Position[2]),
                Normal = new Vector3(vertexData.Normal[0], vertexData.Normal[1], zSign * vertexData.Normal[2]),
                Bitangent = new Vector4(-1, -1, -1, -1),
                Tangents = new List<Vector4>(vertexData.Tangents.Select(x => new Vector4(x[0], x[1], zSign * x[2], x[3]))),
                UVs = new List<Vector3>(vertexData.UVs.Select(x => new Vector3(x[0], 1 - x[1], zSign * x[2]))),
                BoneIndices = boneIndices,
                BoneWeights = boneWeights
            };
            PadVertex(newVertex, bufferLayouts);
            newMesh.Vertices.Add(newVertex);
        }
        if (!options.MirrorZ)
        {
            FlipFaceSet();
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
        if (VertexIndices.Contains(-1))
        {
            throw new InvalidDataException($"Negative vertex index found in fbx mesh {Name}");
        }
        foreach (FLVER2.FaceSet.FSFlags faceSetFlag in faceSetFlags)
        {
            faceSets.Add(new FLVER2.FaceSet
            {
                Indices = VertexIndices,
                CullBackfaces = false,
                Flags = faceSetFlag,
                TriangleStrip = false
            });
        }
        newMesh.FaceSets.AddRange(faceSets);
        newMesh.MaterialIndex = flver.Materials.Count;
        newMaterial.GXIndex = flver.GXLists.Count;
        flver.Materials.Add(newMaterial);
        flver.GXLists.Add(gxList);
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
            bool isDouble = layoutMember.Semantic == FLVER.LayoutSemantic.UV
                && (layoutMember.Type == FLVER.LayoutType.Float4 || layoutMember.Type == FLVER.LayoutType.UVPair);
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
                if (i == flver.BufferLayouts.Count - 1)
                {
                    indices.Add(i + 1);
                    flver.BufferLayouts.Add(referenceBufferLayout);
                    break;
                }
            }
        }
        return indices;
    }

    private void FlipFaceSet()
    {
        for (int i = 0; i < VertexIndices.Count; i += 3)
        {
            (VertexIndices[i + 1], VertexIndices[i + 2]) = (VertexIndices[i + 2], VertexIndices[i + 1]);
        }
    }

    private static int GetBoneIndexFromName(FLVER2 flver, string boneName)
    {
        if (boneName == "0")
        {
            return 0;
        }
        int boneIndex = flver.Bones.IndexOf(flver.Bones.FirstOrDefault(x => x.Name == boneName));
        return boneIndex != -1 ? boneIndex : 0;
    }

    private class LayoutMemberComparer : IEqualityComparer<FLVER.LayoutMember>
    {
        public bool Equals(FLVER.LayoutMember x, FLVER.LayoutMember y)
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
}