using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Web.Script.Serialization;
using Assimp;
using SoulsFormats;

namespace FLVER_Editor
{
    internal static partial class Program
    {
        public static bool ImportFBX(string modelFilePath, bool isLoadingMaleBody = false, bool isLoadingFemaleBody = false)
        {
            try
            {
                FLVER2 targetFlver = isLoadingMaleBody ? MainWindow.maleBodyFlver : isLoadingFemaleBody ? MainWindow.femaleBodyFlver : flver;
                AssimpContext importer = new AssimpContext();
                string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string conversionTableStr = File.ReadAllText(assemblyPath + "\\boneConversion.ini");
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
                Scene scene = importer.ImportFile(modelFilePath, PostProcessSteps.CalculateTangentSpace);
                boneParentList = new Dictionary<string, string>();
                PrintNodeStruct(scene.RootNode);
                int layoutCount = targetFlver.BufferLayouts.Count;
                FLVER2.BufferLayout newBufferLayout = new FLVER2.BufferLayout
                {
                    new FLVER.LayoutMember(FLVER.LayoutType.Float3, FLVER.LayoutSemantic.Position),
                    new FLVER.LayoutMember(FLVER.LayoutType.Byte4B, FLVER.LayoutSemantic.Normal),
                    new FLVER.LayoutMember(FLVER.LayoutType.Byte4B, FLVER.LayoutSemantic.Tangent),
                    new FLVER.LayoutMember(FLVER.LayoutType.Byte4B, FLVER.LayoutSemantic.Tangent, 1),
                    new FLVER.LayoutMember(FLVER.LayoutType.Byte4B, FLVER.LayoutSemantic.BoneIndices),
                    new FLVER.LayoutMember(FLVER.LayoutType.Byte4C, FLVER.LayoutSemantic.BoneWeights),
                    new FLVER.LayoutMember(FLVER.LayoutType.Byte4C, FLVER.LayoutSemantic.VertexColor, 1),
                    new FLVER.LayoutMember(FLVER.LayoutType.UVPair, FLVER.LayoutSemantic.UV)
                };
                targetFlver.BufferLayouts.Add(newBufferLayout);
                int materialCount = targetFlver.Materials.Count;
                if (materialCount > 0)
                {
                    for (int i = 0; i < scene.Materials.Count; ++i)
                    {
                        Material material = scene.Materials[i];
                        FLVER2.Material newMaterial;
                        if (MainWindow.toggleDuplicateMaterialsOnMeshImport)
                        {
                            newMaterial = new JavaScriptSerializer().Deserialize<FLVER2.Material>(new JavaScriptSerializer().Serialize(targetFlver.Materials[i]));
                        }
                        else
                        {
                            newMaterial = GetBaseMaterial(material.TextureDiffuse.FilePath, material.TextureSpecular.FilePath, material.TextureNormal.FilePath);
                            newMaterial.Name = material.Name;
                        }
                        newMaterial.Unk18 = targetFlver.Materials[targetFlver.Materials.Count - 1].Unk18 + 1;
                        targetFlver.Materials.Add(newMaterial);
                    }
                }
                foreach (Mesh assimpMesh in scene.Meshes)
                {
                    FLVER2.Mesh newMesh = new FLVER2.Mesh
                    {
                        MaterialIndex = 0,
                        BoneIndices = new List<int> { 0, 1 },
                        //Unk1 = 0, // Have no idea what this was meant to be, might be handled by SoulsFormats now
                        DefaultBoneIndex = 0,
                        Dynamic = 1,
                        VertexBuffers = new List<FLVER2.VertexBuffer> { new FLVER2.VertexBuffer(layoutCount) },
                        Vertices = new List<FLVER.Vertex>()
                    };
                    newMesh.BoundingBox = new FLVER2.Mesh.BoundingBoxes();
                    newMesh.BoundingBox.Max = new Vector3(1, 1, 1);
                    newMesh.BoundingBox.Min = new Vector3(-1, -1, -1);
                    newMesh.BoundingBox.Unk = new Vector3();
                    List<List<int>> verticesBoneIndices = new List<List<int>>();
                    List<List<float>> verticesBoneWeights = new List<List<float>>();
                    if (assimpMesh.HasBones)
                    {
                        for (int i2 = 0; i2 < assimpMesh.VertexCount; i2++)
                        {
                            verticesBoneIndices.Add(new List<int>());
                            verticesBoneWeights.Add(new List<float>());
                        }
                        for (int i2 = 0; i2 < assimpMesh.BoneCount; i2++)
                        {
                            string boneName = assimpMesh.Bones[i2].Name;
                            int boneIndex;
                            if (conversionTable.ContainsKey(assimpMesh.Bones[i2].Name))
                            {
                                boneName = conversionTable[boneName];
                                boneIndex = FindBoneIndexByName(targetFlver, boneName);
                            }
                            else
                            {
                                boneIndex = FindBoneIndexByName(targetFlver, boneName);
                                for (int bp = 0; bp < boneFindParentTimes; bp++)
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
                            for (int i3 = 0; i3 < assimpMesh.Bones[i2].VertexWeightCount; i3++)
                            {
                                VertexWeight vw = assimpMesh.Bones[i2].VertexWeights[i3];
                                verticesBoneIndices[vw.VertexID].Add(boneIndex);
                                verticesBoneWeights[vw.VertexID].Add(vw.Weight);
                            }
                        }
                    }
                    for (int i = 0; i < assimpMesh.Vertices.Count; i++)
                    {
                        Assimp.Vector3D assimpVertex = assimpMesh.Vertices[i];
                        List<Assimp.Vector3D> channels = assimpMesh.TextureCoordinateChannels[0];
                        Vector3D uv1 = new Vector3D();
                        Vector3D uv2 = new Vector3D();
                        if (channels != null && assimpMesh.TextureCoordinateChannelCount > 0)
                        {
                            uv1 = AssimpVector3DToFEVector3D(channels[i]);
                            uv1.Y = 1 - uv1.Y;
                            uv2 = AssimpVector3DToFEVector3D(channels[i]);
                            uv2.Y = 1 - uv2.Y;
                        }
                        List<Vector3> newUVs = new List<Vector3> { uv1.ToNumericsVector3(), uv2.ToNumericsVector3() };
                        Vector3D normal = new Vector3D(0, 1, 0);
                        if (assimpMesh.HasNormals && assimpMesh.Normals.Count > i) normal = AssimpVector3DToFEVector3D(assimpMesh.Normals[i]).Normalize();
                        Vector3D tangent = new Vector3D(1, 0, 0);
                        if (assimpMesh.Tangents.Count > i) tangent = AssimpVector3DToFEVector3D(assimpMesh.Tangents[i]).Normalize();
                        else
                        {
                            if (assimpMesh.HasNormals && assimpMesh.Normals.Count > i)
                                tangent = new Vector3D(XnaCrossProduct(AssimpVector3DToFEVector3D(assimpMesh.Normals[i]).Normalize().ToXnaVector3(), normal.ToXnaVector3()))
                                    .Normalize();
                        }

                        // I am not sure if the math is correct or not on this one, I just copied the tangent code for this
                        // If you know how please fix it
                        // Otherwise later we should probably redo the importer anyways
                        Vector3D bitangent = new Vector3D(1, 0, 0);
                        if (assimpMesh.BiTangents.Count > i) tangent = AssimpVector3DToFEVector3D(assimpMesh.BiTangents[i]).Normalize();
                        else
                        {
                            if (assimpMesh.HasNormals && assimpMesh.Normals.Count > i)
                                bitangent = new Vector3D(XnaCrossProduct(AssimpVector3DToFEVector3D(assimpMesh.Normals[i]).Normalize().ToXnaVector3(), normal.ToXnaVector3()))
                                    .Normalize();
                        }
                        Vector3 newPosition = new Vector3(assimpVertex.X, assimpVertex.Y, assimpVertex.Z);
                        List<Vector3> newTangents = new List<Vector3> { tangent.ToNumericsVector3() };
                        FLVER.Vertex vertex =
                            GenerateNewFlverVertexUsingNumericsTan(newPosition, normal.ToNumericsVector3(), newTangents, bitangent.ToNumericsVector4(), newUVs, 1);
                        if (assimpMesh.HasBones)
                        {
                            for (int j = 0; j < verticesBoneIndices[i].Count && j < 4; j++)
                            {
                                vertex.BoneIndices[j] = verticesBoneIndices[i][j];
                                vertex.BoneWeights[j] = verticesBoneWeights[i][j];
                            }
                        }
                        newMesh.Vertices.Add(vertex);
                    }
                    List<int> faceIndices = new List<int>();
                    for (int i = 0; i < assimpMesh.FaceCount; i++)
                    {
                        switch (assimpMesh.Faces[i].Indices.Count)
                        {
                            case 3:
                                faceIndices.Add(assimpMesh.Faces[i].Indices[0]);
                                faceIndices.Add(assimpMesh.Faces[i].Indices[2]);
                                faceIndices.Add(assimpMesh.Faces[i].Indices[1]);
                                break;
                            case 4:
                                faceIndices.Add(assimpMesh.Faces[i].Indices[0]);
                                faceIndices.Add(assimpMesh.Faces[i].Indices[2]);
                                faceIndices.Add(assimpMesh.Faces[i].Indices[1]);
                                faceIndices.Add(assimpMesh.Faces[i].Indices[2]);
                                faceIndices.Add(assimpMesh.Faces[i].Indices[0]);
                                faceIndices.Add(assimpMesh.Faces[i].Indices[3]);
                                break;
                        }
                    }
                    newMesh.FaceSets = new List<FLVER2.FaceSet>
                    {
                        GenerateBasicFaceSet()
                    };
                    newMesh.FaceSets[0].Indices = faceIndices;
                    //if (newMesh.FaceSets[0].Indices.Count > 65534) newMesh.FaceSets[0].IndexSize = 32; // This might be handled by SoulsFormats now, I'm not sure
                    newMesh.MaterialIndex = materialCount + assimpMesh.MaterialIndex;
                    targetFlver.Meshes.Add(newMesh);
                }
                if (!isLoadingMaleBody && !isLoadingFemaleBody)
                    MainWindow.ShowInformationDialog("Successfully imported model into the current FLVER file!");
                return true;
            }
            catch
            {
                MainWindow.ShowErrorDialog("An error occurred while attempting to import an external model file.");
                return false;
            }
        }
    }
}