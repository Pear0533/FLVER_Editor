using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using Assimp;
using SoulsFormats;

namespace FLVER_Editor
{
    internal static partial class Program
    {
        public static bool ImportFBX(string modelFilePath, bool isLoadingMaleBody = false, bool isLoadingFemaleBody = false)
        {
            FLVER targetFlver = isLoadingMaleBody ? MainWindow.maleBodyFlver : isLoadingFemaleBody ? MainWindow.femaleBodyFlver : flver;
                var importer = new AssimpContext();
                string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string conversionTableStr = File.ReadAllText(assemblyPath + "\\boneConversion.ini");
                string[] conversionTableStrLines = conversionTableStr.Split(
                    new[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.None
                );
                var conversionTable = new Dictionary<string, string>();
                for (var i2 = 0; i2 + 1 < conversionTableStrLines.Length; i2++)
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
                boneParentList = new Dictionary<string, string>();
                printNodeStruct(md.RootNode);
                int layoutCount = targetFlver.BufferLayouts.Count;
                var newBL = new FLVER.BufferLayout
                {
                    new FLVER.BufferLayout.Member(0, 0, FLVER.BufferLayout.MemberType.Float3, FLVER.BufferLayout.MemberSemantic.Position, 0),
                    new FLVER.BufferLayout.Member(0, 12, FLVER.BufferLayout.MemberType.Byte4B, FLVER.BufferLayout.MemberSemantic.Normal, 0),
                    new FLVER.BufferLayout.Member(0, 16, FLVER.BufferLayout.MemberType.Byte4B, FLVER.BufferLayout.MemberSemantic.Tangent, 0),
                    new FLVER.BufferLayout.Member(0, 20, FLVER.BufferLayout.MemberType.Byte4B, FLVER.BufferLayout.MemberSemantic.Tangent, 1),
                    new FLVER.BufferLayout.Member(0, 24, FLVER.BufferLayout.MemberType.Byte4B, FLVER.BufferLayout.MemberSemantic.BoneIndices, 0),
                    new FLVER.BufferLayout.Member(0, 28, FLVER.BufferLayout.MemberType.Byte4C, FLVER.BufferLayout.MemberSemantic.BoneWeights, 0),
                    new FLVER.BufferLayout.Member(0, 32, FLVER.BufferLayout.MemberType.Byte4C, FLVER.BufferLayout.MemberSemantic.VertexColor, 1),
                    new FLVER.BufferLayout.Member(0, 36, FLVER.BufferLayout.MemberType.UVPair, FLVER.BufferLayout.MemberSemantic.UV, 0)
                };
                targetFlver.BufferLayouts.Add(newBL);
                int materialCount = targetFlver.Materials.Count;
                if (materialCount > 0)
                {
                    foreach (Material mat in md.Materials)
                    {
                        FLVER.Material newMaterial = GetBaseMaterial(mat.TextureDiffuse.FilePath, mat.TextureSpecular.FilePath, mat.TextureNormal.FilePath);
                        newMaterial.Name = mat.Name;
                        newMaterial.Unk18 = targetFlver.Materials[targetFlver.Materials.Count - 1].Unk18 + 1;
                        targetFlver.Materials.Add(newMaterial);
                    }
                }
                foreach (Mesh m in md.Meshes)
                {
                    var mn = new FLVER.Mesh
                    {
                        MaterialIndex = 0,
                        BoneIndices = new List<int>
                        {
                            0,
                            1
                        },
                        BoundingBoxMax = new Vector3(1, 1, 1),
                        BoundingBoxMin = new Vector3(-1, -1, -1),
                        BoundingBoxUnk = new Vector3(),
                        Unk1 = 0,
                        DefaultBoneIndex = 0,
                        Dynamic = true,
                        VertexBuffers = new List<FLVER.VertexBuffer> { new FLVER.VertexBuffer(0, layoutCount, -1) },
                        Vertices = new List<FLVER.Vertex>()
                    };
                    var verticesBoneIndices = new List<List<int>>();
                    var verticesBoneWeights = new List<List<float>>();
                    if (m.HasBones)
                    {
                        for (var i2 = 0; i2 < m.VertexCount; i2++)
                        {
                            verticesBoneIndices.Add(new List<int>());
                            verticesBoneWeights.Add(new List<float>());
                        }
                        for (var i2 = 0; i2 < m.BoneCount; i2++)
                        {
                            string boneName = m.Bones[i2].Name;
                            int boneIndex;
                            if (conversionTable.ContainsKey(m.Bones[i2].Name))
                            {
                                boneName = conversionTable[boneName];
                                boneIndex = FindBoneIndexByName(targetFlver, boneName);
                            }
                            else
                            {
                                boneIndex = FindBoneIndexByName(targetFlver, boneName);
                                for (var bp = 0; bp < boneFindParentTimes; bp++)
                                {
                                    if (boneIndex != -1) continue;
                                    if (!boneParentList.ContainsValue(boneName)) continue;
                                    if (boneParentList[boneName] == null) continue;
                                    boneName = boneParentList[boneName];
                                    if (conversionTable.ContainsKey(boneName))
                                    {
                                        boneName = conversionTable[boneName];
                                    }
                                    boneIndex = FindBoneIndexByName(targetFlver, boneName);
                                }
                            }
                            if (boneIndex == -1)
                            {
                                boneIndex = 0;
                            }
                            for (var i3 = 0; i3 < m.Bones[i2].VertexWeightCount; i3++)
                            {
                                VertexWeight vw = m.Bones[i2].VertexWeights[i3];
                                verticesBoneIndices[vw.VertexID].Add(boneIndex);
                                verticesBoneWeights[vw.VertexID].Add(vw.Weight);
                            }
                        }
                    }
                    for (var i = 0; i < m.Vertices.Count; i++)
                    {
                        Assimp.Vector3D vit = m.Vertices[i];
                        List<Assimp.Vector3D> channels = m.TextureCoordinateChannels[0];
                        var uv1 = new Vector3D();
                        var uv2 = new Vector3D();
                        if (channels != null && m.TextureCoordinateChannelCount > 0)
                        {
                            uv1 = getMyV3D(channels[i]);
                            uv1.Y = 1 - uv1.Y;
                            uv2 = getMyV3D(channels[i]);
                            uv2.Y = 1 - uv2.Y;
                        }
                        var normal = new Vector3D(0, 1, 0);
                        if (m.HasNormals && m.Normals.Count > i)
                        {
                            normal = getMyV3D(m.Normals[i]).normalize();
                        }
                        var tangent = new Vector3D(1, 0, 0);
                        if (m.Tangents.Count > i)
                        {
                            tangent = getMyV3D(m.Tangents[i]).normalize();
                        }
                        else
                        {
                            if (m.HasNormals && m.Normals.Count > i)
                                tangent = new Vector3D(crossPorduct(getMyV3D(m.Normals[i]).normalize().toXnaV3(), normal.toXnaV3())).normalize();
                        }
                        FLVER.Vertex v = generateVertex(new Vector3(vit.X, vit.Y, vit.Z), uv1.toNumV3(), uv2.toNumV3(), normal.toNumV3(),
                            tangent.toNumV3(), 1);
                        if (m.HasBones)
                        {
                            for (var j = 0; j < verticesBoneIndices[i].Count && j < 4; j++)
                            {
                                v.BoneIndices[j] = verticesBoneIndices[i][j];
                                v.BoneWeights[j] = verticesBoneWeights[i][j];
                            }
                        }
                        mn.Vertices.Add(v);
                    }
                    var faceIndices = new List<uint>();
                    for (var i = 0; i < m.FaceCount; i++)
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
                    mn.FaceSets = new List<FLVER.FaceSet>
                    {
                        generateBasicFaceSet()
                    };
                    mn.FaceSets[0].Vertices = faceIndices.ToArray();
                    if (mn.FaceSets[0].Vertices.Length > 65534) mn.FaceSets[0].IndexSize = 32;
                    mn.MaterialIndex = materialCount + m.MaterialIndex;
                    targetFlver.Meshes.Add(mn);
                }
                if (!isLoadingMaleBody && !isLoadingFemaleBody)
                    MainWindow.ShowInformationDialog("Successfully imported model into the current FLVER file!");
                return true;
        }
    }
}