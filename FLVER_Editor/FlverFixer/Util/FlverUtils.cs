using System.Numerics;
using SoulsFormats;

namespace FLVER_Editor.FlverFixer.Util;

public static class FlverUtils
{
    /// <summary>
    /// From The12thAvenger's FBXImporter
    /// </summary>
    public static FLVER.Vertex Pad(this FLVER.Vertex vertex, IEnumerable<FLVER2.BufferLayout> bufferLayouts)
    {
        Dictionary<FLVER.LayoutSemantic, int> usageCounts = new();
        FLVER.LayoutSemantic[] paddedProperties =
            {FLVER.LayoutSemantic.Tangent, FLVER.LayoutSemantic.UV, FLVER.LayoutSemantic.VertexColor};

        IEnumerable<FLVER.LayoutMember> layoutMembers = bufferLayouts.SelectMany(bufferLayout => bufferLayout)
            .Where(x => paddedProperties.Contains(x.Semantic));
        foreach (FLVER.LayoutMember layoutMember in layoutMembers)
        {
            bool isDouble = layoutMember.Semantic == FLVER.LayoutSemantic.UV &&
                            layoutMember.Type is FLVER.LayoutType.Float4 or FLVER.LayoutType.UVPair;
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
                vertex.Colors.Add(new FLVER.VertexColor(255, 255, 0, 255));
            }
        }

        return vertex;
    }

    /// <summary>
    /// From The12thAvenger's FBXImporter, edited to include an exception when the flver
    /// bufferLayouts list is empty.
    /// </summary>
    public static List<int> GetLayoutIndices(this FLVER2 flver, List<FLVER2.BufferLayout> bufferLayouts)
    {
        List<int> indices = new();

        foreach (FLVER2.BufferLayout referenceBufferLayout in bufferLayouts)
        {
            for (int i = 0; i < flver.BufferLayouts.Count; i++)
            {
                FLVER2.BufferLayout bufferLayout = flver.BufferLayouts[i];
                if (bufferLayout.Select(x => (x.Type, x.Semantic)).SequenceEqual(referenceBufferLayout
                        .Select(x => (x.Type, x.Semantic))))
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

            if (flver.BufferLayouts.Count == 0)
            {
                indices.Add(0);
                flver.BufferLayouts.Add(referenceBufferLayout);
            }
        }

        return indices;
    }

    /// <summary>
    /// Checks if the given GXList already exists in the flver GxLists.
    /// </summary>
    public static bool IsNewGxList(this FLVER2 flver, FLVER2.GXList gxList)
    {
        foreach (var gxl in flver.GXLists)
        {
            if (gxl.Count == gxList.Count)
            {
                for (int i = 0; i < gxl.Count; i++)
                {
                    if (gxl[i].Data.Length == gxList[i].Data.Length &&
                        gxl[i].Unk04 == gxList[i].Unk04 && gxl[i].ID.Equals(gxList[i].ID))
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }
}