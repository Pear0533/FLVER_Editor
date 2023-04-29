using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using SoulsFormats;

namespace FLVER_Editor
{
    /// <summary>
    ///     Class for several generation utilities.
    /// </summary>
    public static class Generators
    {
        /// <summary>
        ///     Generates a new FLVER vertex using an existing FLVER vertex.
        /// </summary>
        /// <param name="vertex">An existing FLVER vertex object</param>
        /// <returns>A new FLVER vertex object</returns>
        public static FLVER.Vertex GenerateNewFlverVertex(FLVER.Vertex vertex)
        {
            FLVER.VertexBoneIndices newBoneIndices = new FLVER.VertexBoneIndices();
            newBoneIndices[0] = 0;
            newBoneIndices[1] = 0;
            newBoneIndices[2] = 0;
            newBoneIndices[3] = 0;
            FLVER.VertexBoneWeights newBoneWeights = new FLVER.VertexBoneWeights();
            newBoneWeights[0] = 1;
            newBoneWeights[1] = 0;
            newBoneWeights[2] = 0;
            newBoneWeights[3] = 0;
            List<Vector3> newUVs = new List<Vector3>();
            foreach (Vector3 UV in vertex.UVs) newUVs.Add(new Vector3(UV.X, UV.Y, UV.Z));
            List<Vector4> newTangents = new List<Vector4>();
            foreach (Vector4 tangent in vertex.Tangents) newTangents.Add(new Vector4(tangent.X, tangent.Y, tangent.Z, tangent.W));
            List<FLVER.VertexColor> newColors = new List<FLVER.VertexColor>();
            foreach (FLVER.VertexColor color in vertex.Colors) newColors.Add(new FLVER.VertexColor(color.A, color.R, color.G, color.B));
            FLVER.Vertex newVertex = new FLVER.Vertex
            {
                Position = new Vector3(vertex.Position.X, vertex.Position.Y, vertex.Position.Z),
                BoneIndices = newBoneIndices,
                BoneWeights = newBoneWeights,
                Normal = new Vector3(vertex.Normal.X, vertex.Normal.Y, vertex.Normal.Z),
                NormalW = vertex.NormalW,
                UVs = newUVs,
                Tangents = newTangents,
                Bitangent = new Vector4(vertex.Bitangent.X, vertex.Bitangent.Y, vertex.Bitangent.Z, vertex.Bitangent.W),
                Colors = newColors
            };
            return newVertex;
        }

        /// <summary>
        ///     Generates a new FLVER vertex using the System.Numerics class.
        /// </summary>
        /// <param name="position">Position of the vertex as a Numerics Vector3</param>
        /// <param name="normal">Normal of the vertex as a Numerics Vector3</param>
        /// <param name="tangents">Tangents as a list of Numerics Vector4 objects</param>
        /// <param name="bitangent">Bitangent of the vertex as a Numerics Vector4</param>
        /// <param name="UVs">Mesh UVs as a list of Numerics Vector3 objects</param>
        /// <param name="normalW">W component of the normal, set to -1 by default</param>
        /// <param name="tangentW">W component of the tangent, set to -1 by default</param>
        /// <returns>A new FLVER vertex object</returns>
        public static FLVER.Vertex GenerateNewFlverVertexUsingNumericsTan(Vector3 position, Vector3 normal, List<Vector3> tangents, Vector4 bitangent, List<Vector3> UVs,
            int tangentW = -1, int normalW = -1)
        {
            FLVER.VertexBoneIndices newBoneIndices = new FLVER.VertexBoneIndices();
            newBoneIndices[0] = 0;
            newBoneIndices[1] = 0;
            newBoneIndices[2] = 0;
            newBoneIndices[3] = 0;
            FLVER.VertexBoneWeights newBoneWeights = new FLVER.VertexBoneWeights();
            newBoneWeights[0] = 1;
            newBoneWeights[1] = 0;
            newBoneWeights[2] = 0;
            newBoneWeights[3] = 0;
            List<Vector3> newUVs = new List<Vector3>();
            foreach (Vector3 UV in UVs) newUVs.Add(new Vector3(UV.X, UV.Y, UV.Z));
            List<Vector4> newTangents = new List<Vector4>
            {
                new Vector4(tangents[0].X, tangents[0].Y, tangents[0].Z, tangentW),
                new Vector4(tangents[0].X, tangents[0].Y, tangents[0].Z, tangentW)
            };
            List<FLVER.VertexColor> newColors = new List<FLVER.VertexColor>
            {
                new FLVER.VertexColor(255, 255, 255, 255)
            };
            FLVER.Vertex newVertex = new FLVER.Vertex
            {
                Position = new Vector3(position.X, position.Y, position.Z),
                BoneIndices = newBoneIndices,
                BoneWeights = newBoneWeights,
                Normal = new Vector3(normal.X, normal.Y, normal.Z),
                NormalW = normalW,
                UVs = newUVs,
                Tangents = newTangents,
                Bitangent = new Vector4(bitangent.X, bitangent.Y, bitangent.Z, bitangent.W),
                Colors = newColors
            };
            return newVertex;
        }

        /// <summary>
        ///     Generates a new FLVER vertex using the Microsoft Xna Framework class.
        /// </summary>
        /// <param name="position">Position of the vertex as an Xna Vector3</param>
        /// <param name="normal">Normal of the vertex as an Xna Vector3</param>
        /// <param name="tangents">Tangents as a list of Xna Vector4 objects</param>
        /// <param name="bitangent">Bitangent of the vertex as an Xna Vector4</param>
        /// <param name="UVs">Mesh UVs as a list of Xna Vector3 objects</param>
        /// <param name="normalW">W component of the normal, set to -1 by default</param>
        /// <param name="tangentW">W component of the tangent, set to -1 by default</param>
        /// <returns>A new FLVER vertex object</returns>
        public static FLVER.Vertex GenerateNewFlverVertexUsingXnaTan(Microsoft.Xna.Framework.Vector3 position, Microsoft.Xna.Framework.Vector3 normal,
            List<Microsoft.Xna.Framework.Vector3> tangents, Microsoft.Xna.Framework.Vector4 bitangent, List<Microsoft.Xna.Framework.Vector3> UVs, int tangentW = -1,
            int normalW = -1)
        {
            FLVER.VertexBoneIndices newBoneIndices = new FLVER.VertexBoneIndices();
            newBoneIndices[0] = 0;
            newBoneIndices[1] = 0;
            newBoneIndices[2] = 0;
            newBoneIndices[3] = 0;
            FLVER.VertexBoneWeights newBoneWeights = new FLVER.VertexBoneWeights();
            newBoneWeights[0] = 1;
            newBoneWeights[1] = 0;
            newBoneWeights[2] = 0;
            newBoneWeights[3] = 0;
            List<Vector3> newUVs = new List<Vector3>();
            foreach (Microsoft.Xna.Framework.Vector3 UV in UVs) newUVs.Add(new Vector3(UV.X, UV.Y, UV.Z));
            List<Vector4> newTangents = new List<Vector4>
            {
                new Vector4(tangents[0].X, tangents[0].Y, tangents[0].Z, tangentW),
                new Vector4(tangents[0].X, tangents[0].Y, tangents[0].Z, tangentW)
            };
            List<FLVER.VertexColor> newColors = new List<FLVER.VertexColor>
            {
                new FLVER.VertexColor(255, 255, 255, 255)
            };
            FLVER.Vertex newVertex = new FLVER.Vertex
            {
                Position = new Vector3(position.X, position.Y, position.Z),
                BoneIndices = newBoneIndices,
                BoneWeights = newBoneWeights,
                Normal = new Vector3(normal.X, normal.Y, normal.Z),
                NormalW = normalW,
                UVs = newUVs,
                Tangents = newTangents,
                Bitangent = new Vector4(bitangent.X, bitangent.Y, bitangent.Z, bitangent.W),
                Colors = newColors
            };
            return newVertex;
        }

        /// <summary>
        ///     Generates a new FLVER vertex using the System.Numerics class.
        /// </summary>
        /// <param name="position">Position of the vertex as a Numerics Vector3</param>
        /// <param name="normal">Normal of the vertex as a Numerics Vector3</param>
        /// <param name="tangents">Tangents as a list of Numerics Vector4 objects</param>
        /// <param name="bitangent">Bitangent of the vertex as a Numerics Vector4</param>
        /// <param name="UVs">Mesh UVs as a list of Numerics Vector3 objects</param>
        /// <param name="normalW">W component of the normal, set to -1 by default</param>
        /// <returns>A new FLVER vertex object</returns>
        public static FLVER.Vertex GenerateNewFlverVertexUsingNumerics(Vector3 position, Vector3 normal, List<Vector4> tangents, Vector4 bitangent, List<Vector3> UVs,
            int normalW = -1)
        {
            FLVER.VertexBoneIndices newBoneIndices = new FLVER.VertexBoneIndices();
            newBoneIndices[0] = 0;
            newBoneIndices[1] = 0;
            newBoneIndices[2] = 0;
            newBoneIndices[3] = 0;
            FLVER.VertexBoneWeights newBoneWeights = new FLVER.VertexBoneWeights();
            newBoneWeights[0] = 1;
            newBoneWeights[1] = 0;
            newBoneWeights[2] = 0;
            newBoneWeights[3] = 0;
            List<Vector3> newUVs = new List<Vector3>();
            foreach (Vector3 UV in UVs) newUVs.Add(new Vector3(UV.X, UV.Y, UV.Z));
            List<Vector4> newTangents = new List<Vector4>
            {
                new Vector4(tangents[0].X, tangents[0].Y, tangents[0].Z, tangents[0].W),
                new Vector4(tangents[0].X, tangents[0].Y, tangents[0].Z, tangents[0].W)
            };
            List<FLVER.VertexColor> newColors = new List<FLVER.VertexColor>
            {
                new FLVER.VertexColor(255, 255, 255, 255)
            };
            FLVER.Vertex newVertex = new FLVER.Vertex
            {
                Position = new Vector3(position.X, position.Y, position.Z),
                BoneIndices = newBoneIndices,
                BoneWeights = newBoneWeights,
                Normal = new Vector3(normal.X, normal.Y, normal.Z),
                NormalW = normalW,
                UVs = newUVs,
                Tangents = newTangents,
                Bitangent = new Vector4(bitangent.X, bitangent.Y, bitangent.Z, bitangent.W),
                Colors = newColors
            };
            return newVertex;
        }

        /// <summary>
        ///     Generates a new basic FLVER2 FaceSet.
        /// </summary>
        /// <returns>A basic faceset for FLVER2.</returns>
        public static FLVER2.FaceSet GenerateBasicFaceSet()
        {
            FLVER2.FaceSet newFaceSet = new FLVER2.FaceSet
            {
                TriangleStrip = false,
                CullBackfaces = true,
                Unk06 = 1
            };
            return newFaceSet;
        }

        /// <summary>
        ///     Retrieves the default buffer layout object for a standard FLVER2 model file.
        /// </summary>
        /// <returns>A new FLVER2 BufferLayout</returns>
        public static FLVER2.BufferLayout GenerateDefaultBufferLayout()
        {
            return new FLVER2.BufferLayout
            {
                new FLVER.LayoutMember(FLVER.LayoutType.Float3, FLVER.LayoutSemantic.Position),
                new FLVER.LayoutMember(FLVER.LayoutType.Byte4B, FLVER.LayoutSemantic.Normal, 12),
                new FLVER.LayoutMember(FLVER.LayoutType.Byte4B, FLVER.LayoutSemantic.Tangent, 16),
                new FLVER.LayoutMember(FLVER.LayoutType.Byte4B, FLVER.LayoutSemantic.Tangent, 20, 1),
                new FLVER.LayoutMember(FLVER.LayoutType.Byte4B, FLVER.LayoutSemantic.BoneIndices, 24),
                new FLVER.LayoutMember(FLVER.LayoutType.Byte4C, FLVER.LayoutSemantic.BoneWeights, 28),
                new FLVER.LayoutMember(FLVER.LayoutType.Byte4C, FLVER.LayoutSemantic.VertexColor, 32, 1),
                new FLVER.LayoutMember(FLVER.LayoutType.UVPair, FLVER.LayoutSemantic.UV, 36)
            };
        }

        /// <summary>
        ///     Generates a bone Name conversion dictonary.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GenerateBoneNameConversionTable()
        {
            string conversionTableStr = File.ReadAllText($"{MainWindow.BoneResourcePath}\\boneConversion.ini");
            string[] conversionTableStrLines = conversionTableStr.Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );
            Dictionary<string, string> boneNameConversionTable = new Dictionary<string, string>();
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
        ///     Creates a new FLVER material using the chosen texture path options.
        /// </summary>
        /// <param name="albedoPath">The texture albedo/diffuse path to set.</param>
        /// <param name="metallicPath">The texture metallic/specular path to set.</param>
        /// <param name="normalPath">The  texture normal path to set.</param>
        /// <returns>A new FLVER material with the chosen material texture paths set.</returns>
        public static FLVER2.Material GenerateBaseMaterial(string albedoPath = null, string metallicPath = null, string normalPath = null)
        {
            FLVER2.Material baseMaterial = new FLVER2.Material("", "C[AMSN]_e.mtd", 390);
            SetMaterialPath(baseMaterial, "C_AMSN__snp_Texture2D_2_AlbedoMap_0",
                albedoPath != null ? Path.GetFileNameWithoutExtension(Util3D.FilterInvalidPathChars(albedoPath)) + ".tif" : "");
            SetMaterialPath(baseMaterial, "C_AMSN__snp_Texture2D_0_MetallicMap_0",
                metallicPath != null ? Path.GetFileNameWithoutExtension(Util3D.FilterInvalidPathChars(metallicPath)) + ".tif" : "");
            SetMaterialPath(baseMaterial, "C_AMSN__snp_Texture2D_7_NormalMap_4",
                normalPath != null ? Path.GetFileNameWithoutExtension(Util3D.FilterInvalidPathChars(normalPath)) + ".tif" : "");
            return baseMaterial;
        }

        /// <summary>
        ///     Sets the chosen type's path for the chosen material.
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
            Vector2 scale = new Vector2(1.0f, 1.0f);
            FLVER2.Texture newTexture = new FLVER2.Texture
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