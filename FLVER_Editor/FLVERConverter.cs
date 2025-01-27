using SoulsFormats;

namespace FLVER_Editor;

public class FLVERConverter
{
    public static FLVER0.BufferLayout ConvertToFLVER0Layout(FLVER2.BufferLayout flver2BufferLayout)
    {
        FLVER0.BufferLayout flver0BufferLayout = new();
        foreach (FLVER.LayoutMember? flver2Member in flver2BufferLayout)
        {
            FLVER.LayoutMember flver0Member = new(
                type: flver2Member.Type,
                semantic: flver2Member.Semantic,
                index: flver2Member.Index,
                unk00: flver2Member.Unk00
            );
            flver0BufferLayout.Add(flver0Member);
        }
        return flver0BufferLayout;
    }

    public static FLVER2.BufferLayout ConvertToFLVER2Layout(FLVER0.BufferLayout flver0BufferLayout)
    {
        FLVER2.BufferLayout flver2BufferLayout = new();
        foreach (FLVER.LayoutMember? flver0Member in flver0BufferLayout)
        {
            FLVER.LayoutMember flver2Member = new(
                type: flver0Member.Type,
                semantic: flver0Member.Semantic,
                index: flver0Member.Index,
                unk00: flver0Member.Unk00
            );
            flver2BufferLayout.Add(flver2Member);
        }
        return flver2BufferLayout;
    }


    /// <summary>
    ///     Converts a FLVER0 model to FLVER2 format.
    /// </summary>
    /// <param name="flver0">The FLVER0 model to convert.</param>
    /// <returns>A new FLVER2 model.</returns>
    public static FLVER2 Convert(FLVER0 flver0)
    {
        FLVER2 flver2 = new()
        {
            Header = new FLVER2.FLVERHeader
            {
                BigEndian = flver0.Header.BigEndian,
                BoundingBoxMin = flver0.Header.BoundingBoxMin,
                BoundingBoxMax = flver0.Header.BoundingBoxMax,
                Unicode = flver0.Header.Unicode
            },
            Dummies = new List<FLVER.Dummy>(),
            Materials = new List<FLVER2.Material>(),
            Nodes = new List<FLVER.Node>(),
            Meshes = new List<FLVER2.Mesh>()
        };
        foreach (FLVER.Dummy? dummy in flver0.Dummies)
            flver2.Dummies.Add(dummy);
        foreach (FLVER0.Material? material in flver0.Materials)
        {
            // TODO: WIP
            flver2.BufferLayouts.AddRange(material.Layouts.Select(ConvertToFLVER2Layout));
            FLVER2.Material newMaterial = new()
            {
                Name = material.Name,
                MTD = material.MTD
                // TODO: Layout conversion method...
            };
            foreach (FLVER0.Texture? texture in material.Textures)
            {
                newMaterial.Textures.Add(new FLVER2.Texture
                {
                    Type = texture.Type,
                    Path = texture.Path
                });
            }
            flver2.Materials.Add(newMaterial);
        }
        foreach (FLVER.Node? node in flver0.Nodes)
            flver2.Nodes.Add(node);
        foreach (FLVER0.Mesh? mesh in flver0.Meshes)
        {
            FLVER2.Mesh newMesh = new()
            {
                Vertices = mesh.Vertices,
                FaceSets = mesh.VertexIndices.Select(_ => new FLVER2.FaceSet(FLVER2.FaceSet.FSFlags.None, false, false, -1, mesh.VertexIndices)).ToList(),
                BoneIndices = new List<int>(mesh.BoneIndices.Select(i => (int)i)),
                UseBoneWeights = mesh.Dynamic == 1,
                MaterialIndex = mesh.MaterialIndex
                // NodeIndex = mesh.DefaultBoneIndex,
            };
            flver2.Meshes.Add(newMesh);
        }
        return flver2;
    }

    /// <summary>
    ///     Converts a FLVER2 model to FLVER0 format.
    /// </summary>
    /// <param name="flver2">The FLVER2 model to convert.</param>
    /// <returns>A new FLVER0 model.</returns>
    public static FLVER0 ConvertToFLVER0(FLVER2 flver2)
    {
        FLVER0 flver0 = new()
        {
            Header = new FLVER0Header()
            {
                BigEndian = flver2.Header.BigEndian,
                BoundingBoxMin = flver2.Header.BoundingBoxMin,
                BoundingBoxMax = flver2.Header.BoundingBoxMax,
                Unicode = flver2.Header.Unicode,
                // TODO: WIP
                Version = 21,
                VertexIndexSize = 16
            },
            Dummies = new List<FLVER.Dummy>(),
            Materials = new List<FLVER0.Material>(),
            Nodes = new List<FLVER.Node>(),
            Meshes = new List<FLVER0.Mesh>()
        };
        foreach (FLVER.Dummy? dummy in flver2.Dummies)
            flver0.Dummies.Add(dummy);
        foreach (FLVER2.Material? material in flver2.Materials)
        {
            FLVER0.Material newMaterial = new()
            {
                Name = material.Name,
                MTD = material.MTD,
                Layouts = flver2.BufferLayouts.Select(ConvertToFLVER0Layout).ToList()
                // GXIndex = material.GXIndex,
                // Index = material.Index,
            };
            foreach (FLVER2.Texture? texture in material.Textures)
            {
                newMaterial.Textures ??= new List<FLVER0.Texture>();
                newMaterial.Textures.Add(new FLVER0.Texture
                {
                    Type = texture.Type,
                    Path = texture.Path
                });
            }
            flver0.Materials.Add(newMaterial);
        }
        foreach (FLVER.Node? node in flver2.Nodes)
            flver0.Nodes.Add(node);
        foreach (FLVER2.Mesh? mesh in flver2.Meshes)
        {
            FLVER0.Mesh newMesh = new()
            {
                Vertices = mesh.Vertices,
                VertexIndices = mesh.FaceSets[0].Indices,
                BoneIndices = mesh.BoneIndices.Select(i => (short)i).ToArray(),
                Dynamic = (byte)(mesh.UseBoneWeights ? 1 : 0),
                MaterialIndex = (byte)mesh.MaterialIndex
                // ...
            };
            flver0.Meshes.Add(newMesh);
        }
        return flver0;
    }
}