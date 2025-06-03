using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assimp;
using SoulsFormats;
using NMatrix = System.Numerics.Matrix4x4;
using NVector3 = System.Numerics.Vector3;
using NQuaternion = System.Numerics.Quaternion;

namespace SoulsAssetPipeline.FLVERImporting
{
    public class FLVER2Importer : IDisposable
    {
        const string BANK_RESOURCE_PATH = @"SapResources\FLVER2MaterialInfoBank\Bank{0}.xml";

        private AssimpContext context;

        private static Dictionary<SoulsGames, FLVER2MaterialInfoBank> MaterialInfoBankPerGame 
            = new Dictionary<SoulsGames, FLVER2MaterialInfoBank>();

        public SapLogger Logger = new SapLogger();

        private static void LoadMaterialInfoBankForGame(SoulsGames g)
        {
            if (MaterialInfoBankPerGame.ContainsKey(g))
                return;
            //string xmlPath = Path.Combine(SapUtils.AssemblyDirectory, string.Format(BANK_RESOURCE_PATH, g.ToString()));
            string xmlPath = string.Format(BANK_RESOURCE_PATH, g.ToString());
            if (File.Exists(xmlPath))
            {
                var bank = FLVER2MaterialInfoBank.ReadFromXML(xmlPath);
                MaterialInfoBankPerGame.Add(g, bank);
            }
        }

        public FLVER2Importer()
        {
            context = new AssimpContext();
        }

        public void Dispose()
        {
            context?.Dispose();
        }

        public class ImportedFLVER2Model
        {
            public FLVER2 Flver;
            public List<TPF.Texture> Textures = new List<TPF.Texture>();

            //TODO: FLESH THIS OUT
        }

        //public class ImportedFLVER0Model
        //{
        //    public FLVER0 Flver;
        //    public List<TPF.Texture> Textures = new List<TPF.Texture>();

        //    //TODO: FLESH THIS OUT
        //}

        public class FLVER2ImportSettings
        {
            public string AssetPath { get; set; }

            public float SceneScale { get; set; } = 1.0f;
            public bool ConvertFromZUp { get; set; } = true;

            public FLVER2.FLVERHeader FlverHeader { get; set; } = new FLVER2.FLVERHeader();

            public SoulsGames Game { get; set; } = SoulsGames.None;

            public string RootNodeName { get; set; } = "root";

            //TODO: Make this metaskeleton or something cuz currently it will have empty dummypoly list if you do this.
            public List<FLVER.Node> SkeletonTransformsOverride = null;

            public NMatrix SceneCorrectMatrix = NMatrix.Identity;

            public Dictionary<string, string> BoneNameRemapper = null;
        }

        public ImportedFLVER2Model ImportFBX(string fbxPath, FLVER2ImportSettings settings)
        {

            var fbx = context.ImportFile(fbxPath, PostProcessSteps.None
                | PostProcessSteps.CalculateTangentSpace 
                | PostProcessSteps.LimitBoneWeights 
                | PostProcessSteps.GlobalScale
                | PostProcessSteps.Triangulate
                //| PostProcessSteps.SortByPrimitiveType
                //| PostProcessSteps.OptimizeMeshes
                | PostProcessSteps.FindDegenerates
                | PostProcessSteps.FindInvalidData
                | PostProcessSteps.JoinIdenticalVertices

                | PostProcessSteps.ValidateDataStructure
                );
            return ImportFromAssimpScene(fbx, settings);
        }

        public ImportedFLVER2Model ImportFromAssimpScene(Scene scene, FLVER2ImportSettings settings)
        {
            LoadMaterialInfoBankForGame(settings.Game);

            var result = new ImportedFLVER2Model();
            var flver = result.Flver = new FLVER2();

            flver.Header.BigEndian = settings.FlverHeader.BigEndian;
            flver.Header.BoundingBoxMax = new NVector3(float.MinValue);
            flver.Header.BoundingBoxMin = new NVector3(float.MaxValue);
            flver.Header.Unicode = settings.FlverHeader.Unicode;
            flver.Header.Unk4A = settings.FlverHeader.Unk4A;
            flver.Header.Unk4C = settings.FlverHeader.Unk4C;
            flver.Header.Unk5C = settings.FlverHeader.Unk5C;
            flver.Header.Unk5D = settings.FlverHeader.Unk5D;
            flver.Header.Unk68 = settings.FlverHeader.Unk68;
            flver.Header.Version = settings.FlverHeader.Version;

            var flverSceneMatrix = NMatrix.CreateScale(NVector3.One * settings.SceneScale);

            

            if (settings.ConvertFromZUp)
            {
                flverSceneMatrix *= SapMath.ZUpToYUpNMatrix;

            }

            //flverSceneMatrix *= NMatrix.CreateRotationY(SapMath.Pi);

            flverSceneMatrix *= settings.SceneCorrectMatrix;



            flverSceneMatrix *= NMatrix.CreateScale(1, 1, -1);


            var coordMat = AssimpUtilities.GetSceneCoordSystemMatrix(scene);

            scene.RootNode.Transform *= coordMat;

            var skeletonRootNode = AssimpUtilities.FindRootNode(scene, settings.RootNodeName, out Matrix4x4 skeletonRootNodeMatrix);


            var metaskeleton = FLVERImportHelpers.GenerateFlverMetaskeletonFromRootNode(
                skeletonRootNode, skeletonRootNodeMatrix, settings.SceneScale);

            flver.Nodes = metaskeleton.Bones;
            flver.Dummies = metaskeleton.DummyPoly;

            foreach (var b in flver.Nodes)
            {
                // Mark as dummied-out bone until iterating over them later and seeing which are weighted to meshes.
                if (b.ParentIndex == -1)
                    b.Flags = FLVER.Node.NodeFlags.Disabled;
            }

            var usesIndirectBones = flver.Header.Version <= 0x20010;
             
            if (settings.SkeletonTransformsOverride != null)
            {
                flver.Nodes = settings.SkeletonTransformsOverride;
            }


            

            //var flverMaterialList = new List<FLVER2.Material>();

            foreach (var material in scene.Materials)
            {
                string[] materialNameSplit = material.Name.Split('|');
                string mtd = materialNameSplit.Length > 1 ? materialNameSplit[1].Trim() + ".mtd" : null;

                // If MTD doesn't exist, use original
                mtd = MaterialInfoBankPerGame[settings.Game].FallbackToDefaultMtdIfNecessary(mtd, Logger);

                //ErrorTODO: materialNameSplit should be 2 items long.
                var flverMaterial = new FLVER2.Material(materialNameSplit[0].Trim(), mtd, 0);


                void AddTextureSlot(TextureSlot slot, string ingameSlot)
                {
                    flverMaterial.Textures.Add(new FLVER2.Texture(type: ingameSlot,
                    path: slot.FilePath != null ? Path.GetFullPath(slot.FilePath) : "",
                    scale: System.Numerics.Vector2.One,
                        1, true, 0, 0, 0));

                    string texName = Path.GetFileNameWithoutExtension(slot.FilePath);
                    byte[] texData = scene.GetEmbeddedTexture(slot.FilePath)?.CompressedData;

                    if (texData != null)
                    {
                        var ddsFormat = TPFTextureFormatFinder.GetTpfFormatFromDdsBytes(texData);

                        result.Textures.Add(new TPF.Texture(texName, format: ddsFormat, flags1: 0, bytes: texData, TPF.TPFPlatform.PC));
                    }
                }

                var materialDefinition = MaterialInfoBankPerGame[settings.Game].MaterialDefs[mtd.ToLower()];
                var texChanDefs = materialDefinition.TextureChannels;
                foreach (var kvp in texChanDefs)
                {
                    if (kvp.Key.Index == 0)
                    {
                        if (kvp.Key.Semantic == TextureChannelSemantic.Diffuse)
                            AddTextureSlot(material.TextureDiffuse, kvp.Value);
                        else if (kvp.Key.Semantic == TextureChannelSemantic.Specular)
                            AddTextureSlot(material.TextureSpecular, kvp.Value);
                        else if (kvp.Key.Semantic == TextureChannelSemantic.Normals)
                            AddTextureSlot(material.TextureNormal, kvp.Value);
                        else if (kvp.Key.Semantic == TextureChannelSemantic.Emissive)
                            AddTextureSlot(material.TextureEmissive, kvp.Value);
                        else
                        {
                            flverMaterial.Textures.Add(new FLVER2.Texture(type: kvp.Value,
                                path: string.Empty,
                                scale: System.Numerics.Vector2.One,
                                0, false, 0, 0, 0));
                        }
                    }
                }

                if (materialDefinition.GXItems.Count > 0)
                {
                    flverMaterial.GXIndex = flver.GXLists.Count;
                    var gxList = new FLVER2.GXList();

                    for (int i = 0; i < materialDefinition.GXItems.Count; i++)
                    {
                        var gxid = materialDefinition.GXItems[i].GXID;
                        var unk04 = materialDefinition.GXItems[i].Unk04;
                        byte[] data = MaterialInfoBankPerGame[settings.Game].DefaultGXItemDataExamples[mtd][i];
                        gxList.Add(new FLVER2.GXItem(gxid, unk04, data));
                    }
                    flver.GXLists.Add(gxList);
                }

                flver.Materials.Add(flverMaterial);
                //flverMaterialList.Add(flverMaterial);
            }

            //var properBoneParentRegistry = new Dictionary<Bone, string>();

            //foreach (var mesh in scene.Meshes)
            //{
            //    foreach (var b in mesh.Bones)
            //    {
            //        bool alreadyRegistered = false;
            //        foreach (var bone in properBoneParentRegistry.Keys)
            //        {
            //            if (bone.Name == b.Name)
            //            {
            //                alreadyRegistered = true;
            //                break;
            //            }
            //        }
            //        if (alreadyRegistered)
            //            continue;
            //        mesh.
            //        properBoneParentRegistry.Add(b, b.)
            //    }
            //}

            if (settings.BoneNameRemapper != null)
            {
                foreach (var bn in settings.BoneNameRemapper)
                {
                    var bone = flver.Nodes.FindIndex(b => b.Name == bn.Key);
                    if (bone >= 0)
                    {
                        flver.Nodes[bone].Name = bn.Value;
                    }
                }
            }

            foreach (var mesh in scene.Meshes)
            {
                var flverMesh = new FLVER2.Mesh();

                flverMesh.BoundingBox = new FLVER2.Mesh.BoundingBoxes();

                //TODO: ACTUALLY READ FROM THINGS
                flverMesh.UseBoneWeights = true;



                // Register mesh transform bone:
                //flverMesh.DefaultBoneIndex = flver.Bones.Count;
                //int flverLastRootBoneIndex = flver.Bones.FindLastIndex(b => b.ParentIndex == -1);
                //// Register this new bone as a sibling.
                //if (flverLastRootBoneIndex >= 0)
                //    flver.Bones[flverLastRootBoneIndex].NextSiblingIndex = (short)flverMesh.DefaultBoneIndex;
                //flver.Bones.Add(new FLVER.Bone()
                //{
                //    Name = mesh.Name,
                //    Translation = NVector3.Zero,
                //    Rotation = NVector3.Zero,
                //    Scale = NVector3.One,
                //    BoundingBoxMin = NVector3.One * -0.05f,
                //    BoundingBoxMax = NVector3.One * 0.05f,
                //    // Cross-register sibling from above.
                //    PreviousSiblingIndex = (short)flverLastRootBoneIndex,
                //    NextSiblingIndex = -1,
                //    ParentIndex = -1,
                //    ChildIndex = -1,
                //    Unk3C = 1,
                //});



                int meshUVCount = 0;
                for (int i = 0; i < mesh.UVComponentCount.Length; i++)
                {
                    if (mesh.UVComponentCount[i] > 0)
                        meshUVCount++;
                }
                if (mesh.PrimitiveType != PrimitiveType.Triangle)
                {
                    Console.WriteLine();
                }

                var flverFaceSet = new FLVER2.FaceSet();

                //flverFaceSet.TriangleStrip = true;

                // Handle vertex buffers / layouts:
                flverMesh.MaterialIndex = mesh.MaterialIndex;
                //var newMat = flverMaterialList[mesh.MaterialIndex];
                //var indexOfNewMat = flver.Materials.IndexOf(newMat);
                //if (indexOfNewMat >= 0)
                //{
                //    flverMesh.MaterialIndex = indexOfNewMat;
                //}
                //else
                //{
                //    flverMesh.MaterialIndex = flver.Materials.Count;
                //    flver.Materials.Add(newMat);
                //}
                

                var flverMaterial = flver.Materials[flverMesh.MaterialIndex];
                var matDefinition = MaterialInfoBankPerGame[settings.Game].MaterialDefs[flverMaterial.MTD.ToLower()];
                var defaultBufferDeclaration = matDefinition.AcceptableVertexBufferDeclarations[0];

                Dictionary<FLVER.LayoutSemantic, int> requiredVertexBufferMembers =
                    new Dictionary<FLVER.LayoutSemantic, int>();

                foreach (var buff in defaultBufferDeclaration.Buffers)
                {
                    foreach (var m in buff)
                    {
                        if (!requiredVertexBufferMembers.ContainsKey(m.Semantic))
                            requiredVertexBufferMembers.Add(m.Semantic, 0);
                        requiredVertexBufferMembers[m.Semantic]++;
                    }

                    int nextLayoutIndex = flver.BufferLayouts.Count;
                    flver.BufferLayouts.Add(buff);
                    var vertBuffer = new FLVER2.VertexBuffer(nextLayoutIndex);
                    flverMesh.VertexBuffers.Add(vertBuffer);
                }




                flverMesh.Vertices = new List<FLVER.Vertex>(mesh.VertexCount);

                for (int i = 0; i < mesh.VertexCount; i++)
                {
                    var newVert = new FLVER.Vertex(uvCapacity: meshUVCount,
                        //TODO: Figure out what multiple tangents are used for etc and implement all
                        //      of that into the XML vert layout system stuff etc etc.
                        tangentCapacity: mesh.HasTangentBasis ? 1 : 0, 
                        colorCapacity: mesh.VertexColorChannelCount);

                    newVert.Position = NVector3.Transform(mesh.Vertices[i].ToNumerics(), flverSceneMatrix);

                    flver.Header.UpdateBoundingBox(newVert.Position);
                    if (flverMesh.BoundingBox != null)
                        flverMesh.UpdateBoundingBox(newVert.Position);

                    newVert.Normal = NVector3.Normalize(NVector3.TransformNormal(mesh.Normals[i].ToNumerics(), flverSceneMatrix));

                    //TODO: TEST THIS AGAINST OTHER GAMES ETC
                    //newVert.NormalW = 127;

                    if (mesh.HasTangentBasis)
                    {
                        //ErrorTODO: Throw error if mesh somehow has tangents but not normals.
                        var tan = mesh.Tangents[i];
                        var bitanXYZ = mesh.BiTangents[i];
                        //TODO: Check Bitangent W calculation
                        var bitanW = Vector3D.Dot(Vector3D.Cross(tan, mesh.Normals[i]), bitanXYZ) >= 0 ? 1 : -1;
                        var bitanXYZTransformed = NVector3.Normalize(NVector3.TransformNormal(bitanXYZ.ToNumerics(), flverSceneMatrix));
                        newVert.Tangents.Add(new System.Numerics.Vector4(bitanXYZTransformed, bitanW));
                        //TODO: CHECK THIS AND SEE WTF IT EVEN IS SUPPOSED TO BE
                        newVert.Bitangent = new System.Numerics.Vector4(
                            NVector3.TransformNormal(tan.ToNumerics(), flverSceneMatrix), 0);
                    }
                    
                    for (int j = 0; j < meshUVCount; j++)
                    {
                        var uv = mesh.TextureCoordinateChannels[j][i];
                        newVert.UVs.Add(new NVector3(uv.X, 1 - uv.Y, uv.Z));
                    }

                    for (int j = 0; j < mesh.VertexColorChannelCount; j++)
                    {
                        newVert.Colors.Add(mesh.VertexColorChannels[j][i].ToFlverVertexColor());
                    }

                    for (int j = 0; j < 4; j++)
                    {
                        newVert.BoneIndices[j] = -1;
                    }

                    newVert.EnsureLayoutMembers(requiredVertexBufferMembers);

                    flverMesh.Vertices.Add(newVert);
                }

                if (usesIndirectBones)
                {
                    var bonesInMesh = mesh.Bones.OrderByDescending(mb => mb.VertexWeightCount).ToList();
                    foreach (var bone in bonesInMesh)
                    {
                        var boneIndex = flver.Nodes.FindIndex(b => b.Name == bone.Name);

                        if (!flverMesh.BoneIndices.Contains(boneIndex))
                            flverMesh.BoneIndices.Add(boneIndex);
                    }

                    flverMesh.BoneIndices = flverMesh.BoneIndices.OrderBy(idx => idx).ToList();
                }

                




                foreach (var bone in mesh.Bones)
                {
                    var boneIndex = flver.Nodes.FindIndex(b => b.Name == bone.Name);

                    if (boneIndex == -1)
                    {
                        Logger.LogWarning($"No bone with exact name '{bone.Name}' found. Looking for a bone that starts with that name");
                        boneIndex = flver.Nodes.FindIndex(b => b.Name.StartsWith(bone.Name));

                    }

                    var boneDoesNotExist = false;

                    // Mark bone as not-dummied-out since there is geometry skinned to it.
                    if (boneIndex >= 0 && boneIndex < flver.Nodes.Count)
                    {
                        flver.Nodes[boneIndex].Flags = 0;
                    }
                    else
                    {
                        Logger.LogWarning($"Vertex skinned to bone '{bone.Name}' which does NOT exist in the skeleton.");
                        boneDoesNotExist = true;
                    }
                    
                    int GetNextAvailableBoneSlotOfVert(int vertIndex)
                    {
                        if (flverMesh.Vertices[vertIndex].BoneIndices[0] < 0)
                            return 0;
                        else if (flverMesh.Vertices[vertIndex].BoneIndices[1] < 0)
                            return 1;
                        else if (flverMesh.Vertices[vertIndex].BoneIndices[2] < 0)
                            return 2;
                        else if (flverMesh.Vertices[vertIndex].BoneIndices[3] < 0)
                            return 3;
                        else
                            return -1;
                    }

                    foreach (var weight in bone.VertexWeights)
                    {
                        int boneSlot = GetNextAvailableBoneSlotOfVert(weight.VertexID);
                        if (boneSlot >= 0)
                        {
                            var indexToAssign = usesIndirectBones ? flverMesh.BoneIndices.IndexOf(boneIndex) : boneIndex;
                            if (indexToAssign == -1)
                                Console.WriteLine("fatcat");
                            flverMesh.Vertices[weight.VertexID].BoneIndices[boneSlot] = boneDoesNotExist ? 0 : indexToAssign;
                            flverMesh.Vertices[weight.VertexID].BoneWeights[boneSlot] = boneDoesNotExist ? 0 : weight.Weight;
                            if (!boneDoesNotExist)
                                flver.Nodes[boneIndex].UpdateBoundingBox(flver.Nodes, flverMesh.Vertices[weight.VertexID].Position);
                        }
                    }
                }

                for (int i = 0; i < flverMesh.Vertices.Count; i++)
                {
                    float weightMult = 1 / (
                        flverMesh.Vertices[i].BoneWeights[0] + 
                        flverMesh.Vertices[i].BoneWeights[1] + 
                        flverMesh.Vertices[i].BoneWeights[2] + 
                        flverMesh.Vertices[i].BoneWeights[3]);

                    for (int j = 0; j < 4; j++)
                    {
                        //flverMesh.Vertices[i].BoneWeights[j] = flverMesh.Vertices[i].BoneWeights[j] * weightMult;
                        if (flverMesh.Vertices[i].BoneIndices[j] < 0)
                            flverMesh.Vertices[i].BoneIndices[j] = 0;
                    }

                    //TODO: TEST THIS AGAINST OTHER GAMES ETC
                    if (!requiredVertexBufferMembers.ContainsKey(FLVER.LayoutSemantic.BoneIndices))
                    {
                        flverMesh.Vertices[i].NormalW = flverMesh.Vertices[i].BoneIndices[0];
                    }
                }

                //foreach (var face in mesh.Faces)
                //{
                //    //TODO: See if resets need to be added inbetween or anything.
                //    flverFaceSet.Indices.AddRange(face.Indices);
                //}

                flverFaceSet.Indices.AddRange(mesh.GetIndices());

                flverMesh.FaceSets.Add(flverFaceSet);
                GenerateLodAndMotionBlurFacesets(flverMesh);

                flver.Meshes.Add(flverMesh);
            }

            // DEBUGGING 

            //flver.Bones.RemoveAt(0);
            //foreach (var mm in flver.Meshes)
            //    for (int mbi = 0; mbi < mm.BoneIndices.Count; mbi++)
            //        mm.BoneIndices[mbi] = mm.BoneIndices[mbi] - 1;
            //foreach (var b in flver.Bones)
            //{
            //    if (b.ParentIndex >= 0)
            //        b.ParentIndex--;
            //    if (b.ChildIndex >= 0)
            //        b.ChildIndex--;
            //    if (b.NextSiblingIndex >= 0)
            //        b.NextSiblingIndex--;
            //    if (b.PreviousSiblingIndex >= 0)
            //        b.PreviousSiblingIndex--;
            //}

            ///////////////////

            foreach (var b in flver.Nodes)
            {
                if (settings.SkeletonTransformsOverride != null)
                {
                    var match = settings.SkeletonTransformsOverride.FindIndex(bn => bn.Name == b.Name);
                    if (match >= 0)
                    {
                        b.Translation = settings.SkeletonTransformsOverride[match].Translation;
                        b.Rotation = settings.SkeletonTransformsOverride[match].Rotation;
                        b.Scale = settings.SkeletonTransformsOverride[match].Scale;
                    }
                }

                if (float.IsInfinity(b.BoundingBoxMin.X) || float.IsInfinity(b.BoundingBoxMin.Y) || float.IsInfinity(b.BoundingBoxMin.Z)
                    || float.IsInfinity(b.BoundingBoxMax.X) || float.IsInfinity(b.BoundingBoxMax.Y) || float.IsInfinity(b.BoundingBoxMax.Z))
                {
                    b.BoundingBoxMin = NVector3.One * -0.1f;
                    b.BoundingBoxMax = NVector3.One * 0.1f;
                }
            }

            return result;
        }

        private static void GenerateLodAndMotionBlurFacesets(FLVER2.Mesh mesh)
        {
            var newFacesetsToAdd = new List<SoulsFormats.FLVER2.FaceSet>();
            foreach (var faceset in mesh.FaceSets)
            {
                var lod1 = new SoulsFormats.FLVER2.FaceSet()
                {
                    CullBackfaces = faceset.CullBackfaces,
                    Flags = FLVER2.FaceSet.FSFlags.LodLevel1,
                    TriangleStrip = faceset.TriangleStrip,
                    Indices = faceset.Indices
                };

                var lod2 = new FLVER2.FaceSet()
                {
                    CullBackfaces = faceset.CullBackfaces,
                    Flags = FLVER2.FaceSet.FSFlags.LodLevel2,
                    TriangleStrip = faceset.TriangleStrip,
                    Indices = faceset.Indices
                };

                var mblur = new FLVER2.FaceSet()
                {
                    CullBackfaces = faceset.CullBackfaces,
                    Flags = FLVER2.FaceSet.FSFlags.MotionBlur,
                    TriangleStrip = faceset.TriangleStrip,
                    Indices = faceset.Indices
                };

                var mblurlod1 = new FLVER2.FaceSet()
                {
                    CullBackfaces = faceset.CullBackfaces,
                    Flags = FLVER2.FaceSet.FSFlags.LodLevel1 | FLVER2.FaceSet.FSFlags.MotionBlur,
                    TriangleStrip = faceset.TriangleStrip,
                    Indices = faceset.Indices
                };

                var mblurlod2 = new FLVER2.FaceSet()
                {
                    CullBackfaces = faceset.CullBackfaces,
                    Flags = FLVER2.FaceSet.FSFlags.LodLevel2 | FLVER2.FaceSet.FSFlags.MotionBlur,
                    TriangleStrip = faceset.TriangleStrip,
                    Indices = faceset.Indices
                };

                newFacesetsToAdd.Add(lod1);
                newFacesetsToAdd.Add(lod2);
                newFacesetsToAdd.Add(mblur);
                newFacesetsToAdd.Add(mblurlod1);
                newFacesetsToAdd.Add(mblurlod2);
            }

            foreach (var lod in newFacesetsToAdd)
            {
                mesh.FaceSets.Add(lod);
            }
        }
    }
}
