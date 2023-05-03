using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Web.Script.Serialization;
using Assimp;
using SoulsFormats;

namespace FLVER_Editor
{
    public static class Importer
    {
        private static int FindBoneIndexByName(FLVER2 flverFile, string name)
        {
            for (int i = 0; i < flverFile.Bones.Count; ++i)
                if (flverFile.Bones[i].Name == name)
                    return i;
            return -1;
        }

        public static bool ImportAssimp(FLVER2 flver, string modelFilePath)
        {
            AssimpContext importer = new AssimpContext();
            string conversionTableStr = File.ReadAllText(MainWindow.BoneResourcePath + "\\boneConversion.ini");
            string[] conversionTableStrLines = conversionTableStr.Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );
            Dictionary<string, string> conversionTable = new Dictionary<string, string>();
            for (int i2 = 0; i2 + 1 < conversionTableStrLines.Length; i2++)
            {
                string target = conversionTableStrLines[i2];
                if (target.IndexOf('#') == 0)
                {
                    continue;
                }
                Console.WriteLine(target + @"->" + conversionTableStrLines[i2 + 1]);
                conversionTable.Add(target, conversionTableStrLines[i2 + 1]);
                i2++;
            }
            Scene md = importer.ImportFile(modelFilePath, PostProcessSteps.CalculateTangentSpace);
            int layoutCount = flver.BufferLayouts.Count;
            flver.BufferLayouts.Add(Generators.GenerateDefaultBufferLayout());
            int materialCount = flver.Materials.Count;
            if (materialCount > 0)
            {
                for (int i = 0; i < md.Materials.Count; ++i)
                {
                    Material material = md.Materials[i];
                    FLVER2.Material newMaterial;
                    if (MainWindow.ToggleDuplicateMaterialsOnMeshImport)
                    {
                        newMaterial = new JavaScriptSerializer().Deserialize<FLVER2.Material>(new JavaScriptSerializer().Serialize(flver.Materials[i]));
                    }
                    else
                    {
                        newMaterial = Generators.GenerateBaseMaterial(material.TextureDiffuse.FilePath, material.TextureSpecular.FilePath, material.TextureNormal.FilePath);
                        newMaterial.Name = material.Name;
                    }
                    newMaterial.Unk18 = flver.Materials[flver.Materials.Count - 1].Unk18 + 1;
                    flver.Materials.Add(newMaterial);
                }
            }
            foreach (Mesh m in md.Meshes)
            {
                FLVER2.Mesh mn = new FLVER2.Mesh
                {
                    MaterialIndex = 0,
                    BoneIndices = new List<int>
                    {
                        0,
                        1
                    },
                    BoundingBox = new FLVER2.Mesh.BoundingBoxes
                    {
                        Max = new Vector3(1, 1, 1),
                        Min = new Vector3(-1, -1, -1),
                        Unk = new Vector3()
                    },
                    DefaultBoneIndex = 0,
                    Dynamic = 1,
                    VertexBuffers = new List<FLVER2.VertexBuffer> { new FLVER2.VertexBuffer(layoutCount - 1) },
                    Vertices = new List<FLVER.Vertex>()
                };
                List<List<int>> verticesBoneIndices = new List<List<int>>();
                List<List<float>> verticesBoneWeights = new List<List<float>>();
                if (m.HasBones)
                {
                    for (int i2 = 0; i2 < m.VertexCount; i2++)
                    {
                        verticesBoneIndices.Add(new List<int>());
                        verticesBoneWeights.Add(new List<float>());
                    }
                    for (int i2 = 0; i2 < m.BoneCount; i2++)
                    {
                        string boneName = m.Bones[i2].Name;
                        int boneIndex;
                        if (conversionTable.ContainsKey(m.Bones[i2].Name))
                        {
                            boneName = conversionTable[boneName];
                            boneIndex = FindBoneIndexByName(flver, boneName);
                        }
                        else
                        {
                            boneIndex = FindBoneIndexByName(flver, boneName);
                            for (int bp = 0; bp < Program.BoneFindParentTimes; bp++)
                            {
                                if (boneIndex != -1) continue;
                                if (!Program.BoneParentList.ContainsValue(boneName)) continue;
                                if (Program.BoneParentList[boneName] == null) continue;
                                boneName = Program.BoneParentList[boneName];
                                if (conversionTable.ContainsKey(boneName))
                                {
                                    boneName = conversionTable[boneName];
                                }
                                boneIndex = FindBoneIndexByName(flver, boneName);
                            }
                        }
                        if (boneIndex == -1)
                        {
                            boneIndex = 0;
                        }
                        for (int i3 = 0; i3 < m.Bones[i2].VertexWeightCount; i3++)
                        {
                            VertexWeight vw = m.Bones[i2].VertexWeights[i3];
                            verticesBoneIndices[vw.VertexID].Add(boneIndex);
                            verticesBoneWeights[vw.VertexID].Add(vw.Weight);
                        }
                    }
                }
                for (int i = 0; i < m.Vertices.Count; i++)
                {
                    Assimp.Vector3D vit = m.Vertices[i];
                    List<Assimp.Vector3D> channels = m.TextureCoordinateChannels[0];
                    Vector3D uv1 = new Vector3D();
                    Vector3D uv2 = new Vector3D();
                    if (channels != null && m.TextureCoordinateChannelCount > 0)
                    {
                        uv1 = new Vector3D(channels[i]);
                        uv1.Y = 1 - uv1.Y;
                        uv2 = new Vector3D(channels[i]);
                        uv2.Y = 1 - uv2.Y;
                    }
                    Vector3D normal = new Vector3D(0, 1, 0);
                    if (m.HasNormals && m.Normals.Count > i)
                    {
                        normal = new Vector3D(m.Normals[i]).Normalize();
                    }
                    Vector3D tangent = new Vector3D(1, 0, 0);
                    if (m.Tangents.Count > i)
                    {
                        tangent = new Vector3D(m.Tangents[i]).Normalize();
                    }
                    else
                    {
                        if (m.HasNormals && m.Normals.Count > i)
                            tangent = new Vector3D(Program.XnaCrossProduct(new Vector3D(m.Normals[i]).Normalize().ToXnaVector3(), normal.ToXnaVector3())).Normalize();
                    }
                    FLVER.Vertex v = Generators.GenerateNewFlverVertexUsingNumerics(new Vector3(vit.X, vit.Y, vit.Z), normal.ToNumericsVector3(),
                        new List<Vector4> { tangent.ToNumericsVector4() }, new Vector4(), new List<Vector3> { uv1.ToNumericsVector3(), uv2.ToNumericsVector3() }, 1);
                    if (m.HasBones)
                    {
                        for (int j = 0; j < verticesBoneIndices[i].Count && j < 4; j++)
                        {
                            v.BoneIndices[j] = verticesBoneIndices[i][j];
                            v.BoneWeights[j] = verticesBoneWeights[i][j];
                        }
                    }
                    mn.Vertices.Add(v);
                }
                List<uint> faceIndices = new List<uint>();
                for (int i = 0; i < m.FaceCount; i++)
                {
                    switch (m.Faces[i].Indices.Count)
                    {
                        case 3:
                            faceIndices.Add((uint)m.Faces[i].Indices[0]);
                            faceIndices.Add((uint)m.Faces[i].Indices[2]);
                            faceIndices.Add((uint)m.Faces[i].Indices[1]);
                            break;
                        case 4:
                            faceIndices.Add((uint)m.Faces[i].Indices[0]);
                            faceIndices.Add((uint)m.Faces[i].Indices[2]);
                            faceIndices.Add((uint)m.Faces[i].Indices[1]);
                            faceIndices.Add((uint)m.Faces[i].Indices[2]);
                            faceIndices.Add((uint)m.Faces[i].Indices[0]);
                            faceIndices.Add((uint)m.Faces[i].Indices[3]);
                            break;
                    }
                }
                mn.FaceSets = new List<FLVER2.FaceSet>
                {
                    Generators.GenerateBasicFaceSet()
                };
                mn.FaceSets[0].Indices = faceIndices.ToArray().Select(i => (int)i).ToList();
                mn.MaterialIndex = materialCount + m.MaterialIndex;
                flver.Meshes.Add(mn);
            }
            return true;
        }
    }
}