using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SoulsFormats;

namespace FLVER_Editor
{
    /// <summary>
    /// Class for several generation utilities.
    /// </summary>
    public static class Generate
    {
        /// <summary>
        /// Generates a new FLVER Vertex using an existing FLVER vertex.
        /// </summary>
        /// <param name="vertex">A FLVER vertex.</param>
        /// <returns>A new FLVER vertex.</returns>
        public static FLVER.Vertex GenerateNewFlverVertex(FLVER.Vertex vertex)
        {
            var newBoneIndices = new FLVER.VertexBoneIndices();
            newBoneIndices[0] = 0;
            newBoneIndices[1] = 0;
            newBoneIndices[2] = 0;
            newBoneIndices[3] = 0;

            var newBoneWeights = new FLVER.VertexBoneWeights();
            newBoneWeights[0] = 1;
            newBoneWeights[1] = 0;
            newBoneWeights[2] = 0;
            newBoneWeights[3] = 0;

            var newUVs = new List<System.Numerics.Vector3>();
            foreach (var UV in vertex.UVs) newUVs.Add(new System.Numerics.Vector3(UV.X, UV.Y, UV.Z));

            var newTangents = new List<System.Numerics.Vector4>();
            foreach (var tangent in vertex.Tangents) newTangents.Add(new System.Numerics.Vector4(tangent.X, tangent.Y, tangent.Z, tangent.W));

            var newColors = new List<FLVER.VertexColor>();
            foreach (var color in vertex.Colors) newColors.Add(new FLVER.VertexColor(color.A, color.R, color.G, color.B));

            FLVER.Vertex newVertex = new FLVER.Vertex
            {
                Position = new System.Numerics.Vector3(vertex.Position.X, vertex.Position.Y, vertex.Position.Z),
                BoneIndices = newBoneIndices,
                BoneWeights = newBoneWeights,
                Normal = new System.Numerics.Vector3(vertex.Normal.X, vertex.Normal.Y, vertex.Normal.Z),
                NormalW = vertex.NormalW,
                UVs = newUVs,
                Tangents = newTangents,
                Bitangent = new System.Numerics.Vector4(vertex.Bitangent.X, vertex.Bitangent.Y, vertex.Bitangent.Z, vertex.Bitangent.W),
                Colors = newColors,
            };

            return newVertex;
        }

        /// <summary>
        /// Generates a new FLVER Vertex using System.Numerics parameters
        /// BoneIndices and BoneWeights will be created new all set to zero except for the first element of the BoneWeights
        /// Colors will be generated as a single color set to 255 for all values
        /// NormalW can be set but will default to -1 if not set
        /// </summary>
        /// <param name="position">A Vector3 of the Position of the vertex</param>
        /// <param name="normal">A Vector3 of the normal of the vertex</param>
        /// <param name="tangents">A list of Vector3 tangents, all tangents in the list will be added</param>
        /// <param name="bitangent">A Vector4 representing the bitangent of the vertex</param>
        /// <param name="UVs">A list of Vector3 representing UVs of the vertex, all UVs in the list will be added</param>
        /// <param name="tangentW">An int for the tangent W of the vertex, will be -1 by default if not set</param>
        /// <param name="normalW">An int for the normalW of the vertex, will be -1 by default if not set</param>
        /// <returns>A new flver vertex</returns>
        public static FLVER.Vertex GenerateNewFlverVertexUsingNumericsTan(System.Numerics.Vector3 position, System.Numerics.Vector3 normal, List<System.Numerics.Vector3> tangents, System.Numerics.Vector4 bitangent, List<System.Numerics.Vector3> UVs, int tangentW = -1, int normalW = -1)
        {
            var newBoneIndices = new FLVER.VertexBoneIndices();
            newBoneIndices[0] = 0;
            newBoneIndices[1] = 0;
            newBoneIndices[2] = 0;
            newBoneIndices[3] = 0;

            var newBoneWeights = new FLVER.VertexBoneWeights();
            newBoneWeights[0] = 1;
            newBoneWeights[1] = 0;
            newBoneWeights[2] = 0;
            newBoneWeights[3] = 0;

            var newUVs = new List<System.Numerics.Vector3>();
            foreach (var UV in UVs) newUVs.Add(new System.Numerics.Vector3(UV.X, UV.Y, UV.Z));

            var newTangents = new List<System.Numerics.Vector4>
            {
                new System.Numerics.Vector4(tangents[0].X, tangents[0].Y, tangents[0].Z, tangentW),
                new System.Numerics.Vector4(tangents[0].X, tangents[0].Y, tangents[0].Z, tangentW)
            };

            var newColors = new List<FLVER.VertexColor>
            {
                new FLVER.VertexColor(255, 255, 255, 255)
            };

            FLVER.Vertex newVertex = new FLVER.Vertex
            {
                Position = new System.Numerics.Vector3(position.X, position.Y, position.Z),
                BoneIndices = newBoneIndices,
                BoneWeights = newBoneWeights,
                Normal = new System.Numerics.Vector3(normal.X, normal.Y, normal.Z),
                NormalW = normalW,
                UVs = newUVs,
                Tangents = newTangents,
                Bitangent = new System.Numerics.Vector4(bitangent.X, bitangent.Y, bitangent.Z, bitangent.W),
                Colors = newColors,
            };
            return newVertex;
        }

        /// <summary>
        /// Generates a new FLVER Vertex using Microsoft.Xna.Framework parameters
        /// BoneIndices and BoneWeights will be created new all set to zero except for the first element of the BoneWeights
        /// Colors will be generated as a single color set to 255 for all values
        /// NormalW can be set but will default to -1 if not set
        /// </summary>
        /// <param name="position">An Xna Vector3 of the Position of the vertex</param>
        /// <param name="normal">An Xna Vector3 of the normal of the vertex</param>
        /// <param name="tangents">A list of Xna Vector4 tangents, all tangents in the list will be added</param>
        /// <param name="bitangent">An Xna Vector4 representing the bitangent of the vertex</param>
        /// <param name="UVs">A list of Xna Vector3 representing UVs of the vertex, all UVs in the list will be added</param>
        /// <param name="normalW">An int for the normalW of the vertex, will be -1 by default if not set</param>
        /// <param name="tangentW">An int for the tangent W of the vertex, will be -1 by default if not set</param>
        /// <returns>A new flver vertex</returns>
        public static FLVER.Vertex GenerateNewFlverVertexUsingXnaTan(Microsoft.Xna.Framework.Vector3 position, Microsoft.Xna.Framework.Vector3 normal, List<Microsoft.Xna.Framework.Vector3> tangents, Microsoft.Xna.Framework.Vector4 bitangent, List<Microsoft.Xna.Framework.Vector3> UVs, int tangentW = -1, int normalW = -1)
        {
            var newBoneIndices = new FLVER.VertexBoneIndices();
            newBoneIndices[0] = 0;
            newBoneIndices[1] = 0;
            newBoneIndices[2] = 0;
            newBoneIndices[3] = 0;

            var newBoneWeights = new FLVER.VertexBoneWeights();
            newBoneWeights[0] = 1;
            newBoneWeights[1] = 0;
            newBoneWeights[2] = 0;
            newBoneWeights[3] = 0;

            var newUVs = new List<System.Numerics.Vector3>();
            foreach (var UV in UVs) newUVs.Add(new System.Numerics.Vector3(UV.X, UV.Y, UV.Z));

            var newTangents = new List<System.Numerics.Vector4>
            {
                new System.Numerics.Vector4(tangents[0].X, tangents[0].Y, tangents[0].Z, tangentW),
                new System.Numerics.Vector4(tangents[0].X, tangents[0].Y, tangents[0].Z, tangentW)
            };

            var newColors = new List<FLVER.VertexColor>
            {
                new FLVER.VertexColor(255, 255, 255, 255)
            };

            FLVER.Vertex newVertex = new FLVER.Vertex
            {
                Position = new System.Numerics.Vector3(position.X, position.Y, position.Z),
                BoneIndices = newBoneIndices,
                BoneWeights = newBoneWeights,
                Normal = new System.Numerics.Vector3(normal.X, normal.Y, normal.Z),
                NormalW = normalW,
                UVs = newUVs,
                Tangents = newTangents,
                Bitangent = new System.Numerics.Vector4(bitangent.X, bitangent.Y, bitangent.Z, bitangent.W),
                Colors = newColors,
            };

            return newVertex;
        }

        /// <summary>
        /// Generates a new FLVER Vertex using System.Numerics parameters
        /// BoneIndices and BoneWeights will be created new all set to zero except for the first element of the BoneWeights
        /// Colors will be generated as a single color set to 255 for all values
        /// NormalW can be set but will default to -1 if not set
        /// </summary>
        /// <param name="position">A Vector3 of the Position of the vertex</param>
        /// <param name="normal">A Vector3 of the normal of the vertex</param>
        /// <param name="tangents">A list of Vector4 tangents, all tangents in the list will be added</param>
        /// <param name="bitangent">A Vector4 representing the bitangent of the vertex</param>
        /// <param name="UVs">A list of Vector3 representing UVs of the vertex, all UVs in the list will be added</param>
        /// <param name="normalW">An int for the normalW of the vertex, will be -1 by default if not set</param>
        /// <returns>A new flver vertex</returns>
        public static FLVER.Vertex GenerateNewFlverVertexUsingNumerics(System.Numerics.Vector3 position, System.Numerics.Vector3 normal, List<System.Numerics.Vector4> tangents, System.Numerics.Vector4 bitangent, List<System.Numerics.Vector3> UVs, int normalW = -1)
        {
            var newBoneIndices = new FLVER.VertexBoneIndices();
            newBoneIndices[0] = 0;
            newBoneIndices[1] = 0;
            newBoneIndices[2] = 0;
            newBoneIndices[3] = 0;

            var newBoneWeights = new FLVER.VertexBoneWeights();
            newBoneWeights[0] = 1;
            newBoneWeights[1] = 0;
            newBoneWeights[2] = 0;
            newBoneWeights[3] = 0;

            var newUVs = new List<System.Numerics.Vector3>();
            foreach (var UV in UVs) newUVs.Add(new System.Numerics.Vector3(UV.X, UV.Y, UV.Z));

            var newTangents = new List<System.Numerics.Vector4>
            {
                new System.Numerics.Vector4(tangents[0].X, tangents[0].Y, tangents[0].Z, tangents[0].W),
                new System.Numerics.Vector4(tangents[0].X, tangents[0].Y, tangents[0].Z, tangents[0].W)
            };

            var newColors = new List<FLVER.VertexColor>
            {
                new FLVER.VertexColor(255, 255, 255, 255)
            };

            FLVER.Vertex newVertex = new FLVER.Vertex
            {
                Position = new System.Numerics.Vector3(position.X, position.Y, position.Z),
                BoneIndices = newBoneIndices,
                BoneWeights = newBoneWeights,
                Normal = new System.Numerics.Vector3(normal.X, normal.Y, normal.Z),
                NormalW = normalW,
                UVs = newUVs,
                Tangents = newTangents,
                Bitangent = new System.Numerics.Vector4(bitangent.X, bitangent.Y, bitangent.Z, bitangent.W),
                Colors = newColors,
            };
            return newVertex;
        }

        /// <summary>
        /// Generates a new basic FLVER2 FaceSet.
        /// </summary>
        /// <returns>A basic faceset for FLVER2.</returns>
        public static FLVER2.FaceSet GenerateBasicFaceSet()
        {
            var newFaceSet = new FLVER2.FaceSet
            {
                TriangleStrip = false,
                CullBackfaces = true,
                Unk06 = 1
            };
            return newFaceSet;
        }

        /// <summary>
        /// Retrieves the default buffer layout object for a standard FLVER2 model file.
        /// </summary>
        /// <returns>A new FLVER2 BufferLayout</returns>
        public static FLVER2.BufferLayout GenerateDefaultBufferLayout()
        {
            return new FLVER2.BufferLayout
            {
                new FLVER.LayoutMember(FLVER.LayoutType.Float3, FLVER.LayoutSemantic.Position),
                new FLVER.LayoutMember(FLVER.LayoutType.Byte4B, FLVER.LayoutSemantic.Normal, 12),
                new FLVER.LayoutMember(FLVER.LayoutType.Byte4B, FLVER.LayoutSemantic.Tangent, 16),
                new FLVER.LayoutMember(FLVER.LayoutType.Byte4B, FLVER.LayoutSemantic.Tangent, 16, 1),
                new FLVER.LayoutMember(FLVER.LayoutType.Byte4B, FLVER.LayoutSemantic.BoneIndices, 24),
                new FLVER.LayoutMember(FLVER.LayoutType.Byte4C, FLVER.LayoutSemantic.BoneWeights, 28),
                new FLVER.LayoutMember(FLVER.LayoutType.Byte4C, FLVER.LayoutSemantic.VertexColor, 32, 1),
                new FLVER.LayoutMember(FLVER.LayoutType.UVPair, FLVER.LayoutSemantic.UV, 36),
            };
        }

        /// <summary>
        /// Generates a bone Name conversion dictonary.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GenerateBoneNameConversionTable()
        {
            string conversionTableStr = File.ReadAllText($"{MainWindow.BoneResourcePath}\\boneConversion.ini");
            string[] conversionTableStrLines = conversionTableStr.Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );

            var boneNameConversionTable = new Dictionary<string, string>();
            for (int i = 0; i + 1 < conversionTableStrLines.Length; i++)
            {
                string target = conversionTableStrLines[i];
                if (target.IndexOf('#') == 0)
                {
                    continue;
                }
                Console.WriteLine(target + @"->" + conversionTableStrLines[i + 1]);
                boneNameConversionTable.Add(target, conversionTableStrLines[i + 1]);
                i++;
            }

            return boneNameConversionTable;
        }

        /// <summary>
        /// Creates a new FLVER material using the chosen texture path options.
        /// </summary>
        /// <param name="albedoPath">The texture albedo/diffuse path to set.</param>
        /// <param name="metallicPath">The texture metallic/specular path to set.</param>
        /// <param name="normalPath">The  texture normal path to set.</param>
        /// <returns>A new FLVER material with the chosen material texture paths set.</returns>
        public static FLVER2.Material GenerateBaseMaterial(string albedoPath = null, string metallicPath = null, string normalPath = null)
        {
            var baseMaterial = new FLVER2.Material("", "C[AMSN]_e.mtd", 390);
            SetMaterialPath(baseMaterial, "C_AMSN__snp_Texture2D_2_AlbedoMap_0",
                albedoPath != null ? Path.GetFileNameWithoutExtension(Util3D.FilterInvalidPathChars(albedoPath)) + ".tif" : "");
            SetMaterialPath(baseMaterial, "C_AMSN__snp_Texture2D_0_MetallicMap_0",
                metallicPath != null ? Path.GetFileNameWithoutExtension(Util3D.FilterInvalidPathChars(metallicPath)) + ".tif" : "");
            SetMaterialPath(baseMaterial, "C_AMSN__snp_Texture2D_7_NormalMap_4",
                normalPath != null ? Path.GetFileNameWithoutExtension(Util3D.FilterInvalidPathChars(normalPath)) + ".tif" : "");
            return baseMaterial;
        }

        /// <summary>
        /// Sets the chosen type's path for the chosen material.
        /// </summary>
        /// <param name="material">A material to set the path for.</param>
        /// <param name="typeName">A the texture type to set the path for in the material.</param>
        /// <param name="newPath">The path to set in the material.</param>
        public static void SetMaterialPath(FLVER2.Material material, string typeName, string newPath)
        {
            foreach (FLVER2.Texture texture in material.Textures.Where(texture => texture.Type == typeName))
            {
                texture.Path = newPath;
                return;
            }
            var scale = new System.Numerics.Vector2(1.0f, 1.0f);

            var newTexture = new FLVER2.Texture
            {
                Type = typeName,
                Path = newPath,
                Scale = scale,
                Unk10 = 1,
                Unk11 = true
            };

            material.Textures.Add(newTexture);
        }
    }
}
