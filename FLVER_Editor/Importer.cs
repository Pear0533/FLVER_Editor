using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Assimp;
using ObjLoader.Loader.Data.Elements;
using ObjLoader.Loader.Loaders;
using SoulsFormats;

namespace FLVER_Editor
{
    internal class Importer
    {
        /// <summary>
        /// Imports Assimp compatible model files into the chosen FLVER2.
        /// </summary>
        /// <param name="flver">A FLVER2 model.</param>
        /// <param name="fbxPath">A string representing the path to an FBX file.</param>
        /// <returns>A bool signaling whether or not the FBX import succeeded.</returns>
        public static bool ImportAssimp(FLVER2 flver, string fbxPath)
        {
            try
            {
                // Create an Assimp importer to get FBX model data.
                var importer = new AssimpContext();
                Scene scene = importer.ImportFile(fbxPath, PostProcessSteps.CalculateTangentSpace);

                // Take count of layouts, then add a new bufferlayout for imported FBX data.
                int layoutCount = flver.BufferLayouts.Count;
                flver.BufferLayouts.Add(Generate.GenerateDefaultBufferLayout());

                // Take count of materials, then add imported FBX data to the chosen FLVER.
                int materialCount = flver.Materials.Count;
                ImportAssimpMaterials(scene, flver);
                ImportAssimpMeshes(scene, flver, layoutCount, materialCount);
                return true;
            }
            catch
            {
                Forms.ShowErrorDialog("An error occurred while attempting to import an external model file.");
                return false;
            }
        }

        /// <summary>
        /// Imports materials from imported FBX to the chosen FLVER.
        /// </summary>
        /// <param name="scene">An Assimp scene.</param>
        /// <param name="flver">A FLVER2 model.</param>
        private static void ImportAssimpMaterials(Scene scene, FLVER2 flver)
        {
            if (flver.Materials.Count <= 0)
                return;

            // Add the material data from each Assimp material to the chosen FLVER as new materials.
            foreach (Material assimpMaterial in scene.Materials)
            {
                FLVER2.Material newMaterial = Generate.GenerateBaseMaterial(assimpMaterial.TextureDiffuse.FilePath, assimpMaterial.TextureSpecular.FilePath, assimpMaterial.TextureNormal.FilePath);
                newMaterial.Name = assimpMaterial.Name;
                newMaterial.Unk18 = flver.Materials[flver.Materials.Count - 1].Unk18 + 1;
                flver.Materials.Add(newMaterial);
            }
        }

        /// <summary>
        /// Imports meshes from imported FBX to the chosen FLVER.
        /// </summary>
        /// <param name="scene">An Assimp scene.</param>
        /// <param name="flver">A FLVER2 model.</param>
        /// <param name="layoutCount">The layout count of the FLVER FBX is being imported to before the layout count changes.</param>
        /// <param name="materialCount">The material count of the FLVER FBX is being imported to before the material count changes.</param>
        private static void ImportAssimpMeshes(Scene scene, FLVER2 flver, int layoutCount, int materialCount)
        {
            // Import Assimp scene mesh data into new FLVER meshes to be added to the chosen FLVER.
            foreach (Mesh assimpMesh in scene.Meshes)
            {
                // Create a new FLVER mesh to import Assimp mesh data to.
                var newMesh = new FLVER2.Mesh
                {
                    MaterialIndex = 0,
                    BoneIndices = new List<int> { 0, 1 },
                    DefaultBoneIndex = 0,
                    Dynamic = 1,
                    VertexBuffers = new List<FLVER2.VertexBuffer> { new FLVER2.VertexBuffer(layoutCount) },
                    Vertices = new List<FLVER.Vertex>(),
                    BoundingBox = new FLVER2.Mesh.BoundingBoxes
                    {
                        Max = new Vector3(1, 1, 1),
                        Min = new Vector3(-1, -1, -1),
                        Unk = new Vector3()
                    }
                };

                // Start importing Assimp mesh data to new FLVER mesh.
                var vertexBoneIndexList = new List<List<int>>();
                var vertexBoneWeightList = new List<List<float>>();
                ImportAssimpBones(assimpMesh, flver, vertexBoneIndexList, vertexBoneWeightList);
                ImportAssimpVertices(assimpMesh, newMesh, vertexBoneIndexList, vertexBoneWeightList);
                ImportAssimpFaces(assimpMesh, newMesh);
                
                // Set material index for new FLVER mesh and add it to the chosen FLVER's meshes.
                newMesh.MaterialIndex = materialCount + assimpMesh.MaterialIndex;
                flver.Meshes.Add(newMesh);
            }
        }

        /// <summary>
        /// Imports bone data from an imported FBX mesh to the chosen FLVER.
        /// </summary>
        /// <param name="assimpMesh">An Assimp mesh.</param>
        /// <param name="flver">A FLVER2 model.</param>
        /// <param name="vertexBoneIndexList">A list of vertex bone indices to add vertex bone index data to for later applying.</param>
        /// <param name="vertexBoneWeightList">A list of vertex bone weights to add vertex bone weight data to for later applying.</param>
        private static void ImportAssimpBones(Mesh assimpMesh, FLVER2 flver, List<List<int>> vertexBoneIndexList, List<List<float>> vertexBoneWeightList)
        {
            if (!assimpMesh.HasBones)
                return;

            // Generate bone Name conversion table to convert known bone names.
            var boneNameConversionTable = Generate.GenerateBoneNameConversionTable();

            // Create bone index and weight lists for each vertex in the assimp mesh.
            for (int i = 0; i < assimpMesh.VertexCount; i++)
            {
                vertexBoneIndexList.Add(new List<int>());
                vertexBoneWeightList.Add(new List<float>());
            }
            
            // Get the bone indices and weights from the assimpMesh's bones.
            for (int i = 0; i < assimpMesh.BoneCount; i++)
            {
                // Set bone Name to assimp mesh bone Name.
                string boneName = assimpMesh.Bones[i].Name;
                int boneIndex;

                // If the bone Name conversion table has a Name for the assimp mesh bone Name, use it instead.
                if (boneNameConversionTable.ContainsKey(assimpMesh.Bones[i].Name))
                    boneName = boneNameConversionTable[boneName];

                // Set the bone index using the decided bone Name.
                boneIndex = Util3D.FindBoneIndexByName(flver, boneName);

                // If the bone index is negative one set it to 0, the root bone.
                if (boneIndex == -1) boneIndex = 0;

                // Add the assimp bone's bone indices and weights to bone index and weight list for later applying.
                for (int j = 0; j < assimpMesh.Bones[i].VertexWeightCount; j++)
                {
                    VertexWeight assimpVertexWeight = assimpMesh.Bones[i].VertexWeights[j];
                    vertexBoneIndexList[assimpVertexWeight.VertexID].Add(boneIndex);
                    vertexBoneWeightList[assimpVertexWeight.VertexID].Add(assimpVertexWeight.Weight);
                }
            }
        }

        /// <summary>
        /// Imports vertex data from an imported FBX mesh to the chosen FLVER.
        /// </summary>
        /// <param name="assimpMesh">An Assimp mesh.</param>
        /// <param name="newMesh">A new FLVER2 mesh.</param>
        /// <param name="vertexBoneIndexList">A list of vertex bone indices to apply to FLVER vertices.</param>
        /// <param name="vertexBoneWeightList">A list of vertex bone weights to apply to FLVER vertices.</param>
        private static void ImportAssimpVertices(Mesh assimpMesh, FLVER2.Mesh newMesh, List<List<int>> vertexBoneIndexList, List<List<float>> vertexBoneWeightList)
        {
            for (int i = 0; i < assimpMesh.Vertices.Count; i++)
            {
                // Get current Assimp vertex and texture coordinate channels.
                Assimp.Vector3D assimpVertex = assimpMesh.Vertices[i];
                List<Assimp.Vector3D> channels = assimpMesh.TextureCoordinateChannels[0];

                // Add Assimp vertex UV data to new UVs to be added to FLVER vertices.
                var uv1 = new Vector3D();
                var uv2 = new Vector3D();
                if (channels != null && assimpMesh.TextureCoordinateChannelCount > 0)
                {
                    uv1 = new Vector3D(channels[i]);
                    uv1.Y = 1 - uv1.Y;
                    uv2 = new Vector3D(channels[i]);
                    uv2.Y = 1 - uv2.Y;
                }
                var newUVs = new List<Vector3> { uv1.ToNumericsVector3(), uv2.ToNumericsVector3() };

                // Add Assimp vertex normal data to new normals to be added to FLVER vertices.
                var normal = new Vector3D(0, 1, 0);
                if (assimpMesh.HasNormals && assimpMesh.Normals.Count > i)
                    normal = new Vector3D(assimpMesh.Normals[i]).Normalize();

                // Add Assimp vertex tangent data to new tangents to be added to FLVER vertices.
                var tangent = new Vector3D(1, 0, 0);
                if (assimpMesh.Tangents.Count > i) 
                    tangent = new Vector3D(assimpMesh.Tangents[i]).Normalize();
                else if (assimpMesh.HasNormals && assimpMesh.Normals.Count > i)
                    tangent = new Vector3D(Program.XnaCrossProduct(new Vector3D(assimpMesh.Normals[i]).Normalize().ToXnaVector3(), normal.ToXnaVector3())).Normalize();

                // Add Assimp vertex bitangent data to new bitangents to be added to FLVER vertices.
                // I am not sure if the math is correct or not on this one, I just copied the tangent code for this
                // If you know how please fix it
                // Otherwise later we should probably redo the importer anyways
                var bitangent = new Vector3D(1, 0, 0);
                if (assimpMesh.BiTangents.Count > i)
                    bitangent = new Vector3D(assimpMesh.BiTangents[i]).Normalize();
                else if (assimpMesh.HasNormals && assimpMesh.Normals.Count > i)
                    bitangent = new Vector3D(Program.XnaCrossProduct(new Vector3D(assimpMesh.Normals[i]).Normalize().ToXnaVector3(), normal.ToXnaVector3())).Normalize();

                // Add Assimp vertex positional data and convert tangents into a list for compatibility, then generate new FLVER vertex.
                var newPosition = new Vector3(assimpVertex.X, assimpVertex.Y, assimpVertex.Z);
                var newTangents = new List<Vector3> { tangent.ToNumericsVector3() };
                FLVER.Vertex vertex = Generate.GenerateNewFlverVertexUsingNumericsTan(newPosition, normal.ToNumericsVector3(), newTangents, bitangent.ToNumericsVector4(), newUVs, 1);

                // If the current Assimp mesh has bones add vertex bone indices and bone weights to new vertex, then add the new FLVER vertex to the new FLVER mesh.
                if (assimpMesh.HasBones)
                {
                    for (int j = 0; j < vertexBoneIndexList[i].Count && j < 4; j++)
                    {
                        vertex.BoneIndices[j] = vertexBoneIndexList[i][j];
                        vertex.BoneWeights[j] = vertexBoneWeightList[i][j];
                    }
                }
                newMesh.Vertices.Add(vertex);
            }
        }

        /// <summary>
        /// Imports face data from an imported FBX mesh to the chosen FLVER.
        /// </summary>
        /// <param name="assimpMesh">An Assimp mesh.</param>
        /// <param name="newMesh">A new FLVER2 mesh.</param>
        private static void ImportAssimpFaces(Mesh assimpMesh, FLVER2.Mesh newMesh)
        {
            var faceIndices = new List<int>();
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
                Generate.GenerateBasicFaceSet()
            };

            newMesh.FaceSets[0].Indices = faceIndices;
        }

        /// <summary>
        /// Swaps a sekiro model with a Dark Souls or BloodBorne model.
        /// </summary>
        public static void ModelSwapModule()
        {
            OpenFileDialog openSeikiroModelDonorDialog;
            openSeikiroModelDonorDialog = new OpenFileDialog
            {
                InitialDirectory = Directory.GetCurrentDirectory(),
                Title = "Choose template seikiro model file."
            };
            if (openSeikiroModelDonorDialog.ShowDialog() == DialogResult.OK) Console.WriteLine(openSeikiroModelDonorDialog.FileName);
            else return;
            FLVER2 seikiroDonorModel = FLVER2.Read(openSeikiroModelDonorDialog.FileName);

            var openReplacementModelDialog = new OpenFileDialog
            {
                InitialDirectory = Directory.GetCurrentDirectory(),
                Title = "Choose source DS/BB model file."
            };
            if (openReplacementModelDialog.ShowDialog() == DialogResult.OK) Console.WriteLine(openSeikiroModelDonorDialog.FileName);
            else return;
            FLVER2 replacementModel = FLVER2.Read(openReplacementModelDialog.FileName);

            Console.WriteLine(seikiroDonorModel.Header);
            Console.WriteLine("Seikiro unk is:" + seikiroDonorModel.SekiroUnk);
            Console.WriteLine("Material:");

            foreach (FLVER2.Material material in seikiroDonorModel.Materials) Console.WriteLine(material.Name);
            foreach (FLVER2.Mesh material in seikiroDonorModel.Meshes) Console.WriteLine("Mesh#" + material.MaterialIndex);

            //* new
            //b.Header.BigEndian = src.Header.BigEndian;

            //

            //X: is not the sword axis!!!
            //Y: ++ means closer to the hand!
            //Unit: in meter(?)

            //For Moonlight sword -> threaded cane, Y+0.5f
            var form = new Form();


            var label = Forms.MakeLabel(new Size(150, 15), new Point(10, 20), "x,y,z offset? Y= weapon length axis,Y+=Closer to hand");
            form.Controls.Add(label);

            var textbox1 = Forms.MakeTextBox(new Size(70, 15), new Point(10, 60), "0");
            var textbox2 = Forms.MakeTextBox(new Size(70, 15), new Point(10, 100), "0");
            var textbox3 = Forms.MakeTextBox(new Size(70, 15), new Point(10, 140), "0");
            form.Controls.Add(textbox1);
            form.Controls.Add(textbox2);
            form.Controls.Add(textbox3);

            var copyMaterial = Forms.MakeCheckBox(new Size(70, 15), new Point(10, 160), "Copy Material");
            var copyBones = Forms.MakeCheckBox(new Size(150, 15), new Point(10, 180), "Copy Bones");
            var copyDummy = Forms.MakeCheckBox(new Size(150, 15), new Point(10, 200), "Copy Dummy");
            var weightToFirstBone = Forms.MakeCheckBox(new Size(), new Point(10, 220), "All vertex weight to first bone");
            form.Controls.Add(copyMaterial);
            form.Controls.Add(copyBones);
            form.Controls.Add(copyDummy);
            form.Controls.Add(weightToFirstBone);

            form.ShowDialog();
            float x = float.Parse(textbox1.Text);
            float y = float.Parse(textbox2.Text);
            float z = float.Parse(textbox3.Text);

            seikiroDonorModel.Meshes = replacementModel.Meshes;
            if (copyMaterial.Checked) seikiroDonorModel.Materials = replacementModel.Materials;
            if (copyBones.Checked) seikiroDonorModel.Bones = replacementModel.Bones;
            if (copyDummy.Checked) seikiroDonorModel.Dummies = replacementModel.Dummies;
            if (weightToFirstBone.Checked)
            {
                for (var i = 0; i < seikiroDonorModel.Meshes.Count; i++)
                {
                    seikiroDonorModel.Meshes[i].BoneIndices = new List<int> { 0, 1 };
                    seikiroDonorModel.Meshes[i].DefaultBoneIndex = 1;
                    foreach (FLVER.Vertex vertex in seikiroDonorModel.Meshes[i].Vertices)
                    {
                        if (Util3D.BoneWeightsToFloatArray(vertex.BoneWeights) == null) continue;

                        vertex.Position = new System.Numerics.Vector3(0, 0, 0);
                        for (var k = 0; k < vertex.BoneWeights.Length; k++)
                        {
                            vertex.BoneWeights[k] = 0;
                            vertex.BoneIndices[k] = 0;
                        }
                        vertex.BoneIndices[0] = 1;
                        vertex.BoneWeights[0] = 1;
                    }
                    //flver2.Meshes[i].Vertices = new List<FLVER.Vertex>();
                }
            }
            foreach (FLVER2.Mesh mesh in seikiroDonorModel.Meshes)
            {
                foreach (FLVER.Vertex vertex in mesh.Vertices)
                {
                    vertex.Position = new System.Numerics.Vector3(vertex.Position.X + x, vertex.Position.Y + y, vertex.Position.Z + z);
                }
            }
            seikiroDonorModel.Write(openSeikiroModelDonorDialog.FileName + "n");
            MessageBox.Show("Swap completed!", "Info");
            //Console.WriteLine("End reading");
            //Application.Exit();
        }

        /// <summary>
        /// Currently cannot solve tangent properly, will be looking into it when I have time for it.
        /// </summary>
        private static void ImportObj(FLVER2 flver2)
        {
            var openFileDialog2 = new OpenFileDialog();
            var res = "";
            if (openFileDialog2.ShowDialog() == DialogResult.No)
            {
                return;
            }
            res = openFileDialog2.FileName;
            var objLoaderFactory = new ObjLoaderFactory();
            var msp = new MaterialStreamProvider();
            var openFileDialog3 = new OpenFileDialog();
            openFileDialog3.Title = "Choose MTL file:";
            if (openFileDialog3.ShowDialog() == DialogResult.No)
            {
                return;
            }
            msp.Open(openFileDialog3.FileName);
            IObjLoader objLoader = objLoaderFactory.Create(msp);
            var fileStream = new FileStream(res, FileMode.Open);
            LoadResult objModel = objLoader.Load(fileStream);

            // ObjLoader.Loader.Data.Elements.Face f = result.Groups[0].Faces[0];
            // ObjLoader.Loader.Data.Elements.FaceVertex[] fv =getVertices(f);

            // string groups = new JavaScriptSerializer().Serialize(fv);
            //string vertices = new JavaScriptSerializer().Serialize(result.Vertices);

            //MessageBox.Show(groups,"Group info");
            // MessageBox.Show(vertices, "V info");
            fileStream.Close();

            //Step 1 add a new buffer layout for my program:
            int layoutCount = flver2.BufferLayouts.Count;
            var newBufferLayout = new FLVER2.BufferLayout
            {
                    new FLVER.LayoutMember(FLVER.LayoutType.Float3, FLVER.LayoutSemantic.Position, 0),
                    new FLVER.LayoutMember(FLVER.LayoutType.Byte4B, FLVER.LayoutSemantic.Normal, 0),
                    new FLVER.LayoutMember(FLVER.LayoutType.Byte4B, FLVER.LayoutSemantic.Tangent, 0),
                    new FLVER.LayoutMember(FLVER.LayoutType.Byte4B, FLVER.LayoutSemantic.Tangent, 1),
                    new FLVER.LayoutMember(FLVER.LayoutType.Byte4B, FLVER.LayoutSemantic.BoneIndices, 0),
                    new FLVER.LayoutMember(FLVER.LayoutType.Byte4C, FLVER.LayoutSemantic.BoneWeights, 0),
                    new FLVER.LayoutMember(FLVER.LayoutType.Byte4C,FLVER.LayoutSemantic.VertexColor, 1),
                    new FLVER.LayoutMember(FLVER.LayoutType.UVPair, FLVER.LayoutSemantic.UV, 0)
            };

            flver2.BufferLayouts.Add(newBufferLayout);
            int materialCount = flver2.Materials.Count;
            var mesh = new FLVER2.Mesh
            {
                MaterialIndex = 0,
                BoneIndices = new List<int> { 0, 1 },
                DefaultBoneIndex = 0,
                Dynamic = 0,
                VertexBuffers = new List<FLVER2.VertexBuffer> { new FLVER2.VertexBuffer(layoutCount) },
                Vertices = new List<FLVER.Vertex>()
            };
            mesh.BoundingBox.Max = new System.Numerics.Vector3(1, 1, 1);
            mesh.BoundingBox.Min = new System.Numerics.Vector3(-1, -1, -1);
            mesh.BoundingBox.Unk = new System.Numerics.Vector3();
            //mesh.Unk1 = 0;

            if (objModel.Groups.Count == 0)
            {
                MessageBox.Show("You imported nothing!");
                return;
            }

            var objNormals = objModel.Normals;

            MessageBox.Show("Vertice number:"
                + objModel.Vertices.Count
                + "Texture V number:"
                + objModel.Textures.Count
                + "Normal number:"
                + objNormals.Count
                + "Face groups:"
                + objModel.Groups[0].Faces.Count);
            var vertexNormalList = new Program.VertexNormalList[objModel.Vertices.Count + 1];
            for (var i = 0; i < vertexNormalList.Length; i++)
            {
                vertexNormalList[i] = new Program.VertexNormalList();
            }
            var faceIndices = new List<int>();
            var textureIndices = new int[objModel.Vertices.Count + 1];
            foreach (Group gr in objModel.Groups)
            {
                foreach (ObjLoader.Loader.Data.Elements.Face faces in gr.Faces)
                {
                    FaceVertex[] objFaceVertexList = GetVertices(faces);

                    var index = 0;
                    if (objFaceVertexList.Length == 4)
                    {
                        faceIndices.Add(objFaceVertexList[0].VertexIndex - 1);
                        faceIndices.Add(objFaceVertexList[2].VertexIndex - 1);
                        faceIndices.Add(objFaceVertexList[1].VertexIndex - 1);

                        //record normal to help calculate vertex normals
                        var x1 = objNormals[objFaceVertexList[index].NormalIndex - 1].X;
                        var y1 = objNormals[objFaceVertexList[index].NormalIndex - 1].Y;
                        var z1 = objNormals[objFaceVertexList[index].NormalIndex - 1].Z;
                        vertexNormalList[objFaceVertexList[index].VertexIndex - 1].Add(new Vector3D(x1, y1, z1));
                        textureIndices[objFaceVertexList[index].VertexIndex - 1] = objFaceVertexList[index].TextureIndex - 1;

                        index = 2;
                        var x2 = objNormals[objFaceVertexList[index].NormalIndex - 1].X;
                        var y2 = objNormals[objFaceVertexList[index].NormalIndex - 1].Y;
                        var z2 = objNormals[objFaceVertexList[index].NormalIndex - 1].Z;
                        vertexNormalList[objFaceVertexList[index].VertexIndex - 1].Add(new Vector3D(x2, y2, z2));
                        textureIndices[objFaceVertexList[index].VertexIndex - 1] = objFaceVertexList[index].TextureIndex - 1;

                        index = 1;
                        var x3 = objNormals[objFaceVertexList[index].NormalIndex - 1].X;
                        var y3 = objNormals[objFaceVertexList[index].NormalIndex - 1].Y;
                        var z3 = objNormals[objFaceVertexList[index].NormalIndex - 1].Z;
                        vertexNormalList[objFaceVertexList[index].VertexIndex - 1].Add(new Vector3D(x3, y3, z3));
                        textureIndices[objFaceVertexList[index].VertexIndex - 1] = objFaceVertexList[index].TextureIndex - 1;

                        faceIndices.Add(objFaceVertexList[2].VertexIndex - 1);
                        faceIndices.Add(objFaceVertexList[0].VertexIndex - 1);
                        faceIndices.Add(objFaceVertexList[3].VertexIndex - 1);

                        index = 2;
                        var x4 = objNormals[objFaceVertexList[index].NormalIndex - 1].X;
                        var y4 = objNormals[objFaceVertexList[index].NormalIndex - 1].Y;
                        var z4 = objNormals[objFaceVertexList[index].NormalIndex - 1].Z;
                        vertexNormalList[objFaceVertexList[index].VertexIndex - 1].Add(new Vector3D(x4, y4, z4));
                        textureIndices[objFaceVertexList[index].VertexIndex - 1] = objFaceVertexList[index].TextureIndex - 1;

                        index = 0;
                        var x5 = objNormals[objFaceVertexList[index].NormalIndex - 1].X;
                        var y5 = objNormals[objFaceVertexList[index].NormalIndex].Y;
                        var z5 = objNormals[objFaceVertexList[index].NormalIndex].Z;
                        vertexNormalList[objFaceVertexList[index].VertexIndex - 1].Add(new Vector3D(x5, y5, z5));
                        textureIndices[objFaceVertexList[index].VertexIndex - 1] = objFaceVertexList[index].TextureIndex - 1;

                        index = 3;
                        vertexNormalList[objFaceVertexList[index].VertexIndex - 1].Add(new Vector3D(objNormals[objFaceVertexList[index].NormalIndex].X,
                            objNormals[objFaceVertexList[index].NormalIndex].Y, objNormals[objFaceVertexList[index].NormalIndex].Z));
                        textureIndices[objFaceVertexList[index].VertexIndex - 1] = (objFaceVertexList[index].TextureIndex - 1);
                    }
                    else if (objFaceVertexList.Length == 3)
                    {
                        faceIndices.Add(objFaceVertexList[0].VertexIndex - 1);
                        faceIndices.Add(objFaceVertexList[2].VertexIndex - 1);
                        faceIndices.Add(objFaceVertexList[1].VertexIndex - 1);

                        index = 2;
                        var x1 = objNormals[objFaceVertexList[index].NormalIndex - 1].X;
                        var y1 = objNormals[objFaceVertexList[index].NormalIndex - 1].Y;
                        var z1 = objNormals[objFaceVertexList[index].NormalIndex - 1].Z;
                        vertexNormalList[objFaceVertexList[index].VertexIndex - 1].Add(new Vector3D(x1, y1, z1));
                        textureIndices[objFaceVertexList[index].VertexIndex - 1] = objFaceVertexList[index].TextureIndex - 1;

                        index = 0;
                        var x2 = objNormals[objFaceVertexList[index].NormalIndex - 1].X;
                        var y2 = objNormals[objFaceVertexList[index].NormalIndex - 1].Y;
                        var z2 = objNormals[objFaceVertexList[index].NormalIndex - 1].Z;
                        vertexNormalList[objFaceVertexList[index].VertexIndex - 1].Add(new Vector3D(x2, y2, z2));
                        textureIndices[objFaceVertexList[index].VertexIndex - 1] = objFaceVertexList[index].TextureIndex - 1;

                        index = 1;
                        var x3 = objNormals[objFaceVertexList[index].NormalIndex - 1].X;
                        var y3 = objNormals[objFaceVertexList[index].NormalIndex - 1].Y;
                        var z3 = objNormals[objFaceVertexList[index].NormalIndex - 1].Z;
                        vertexNormalList[objFaceVertexList[index].VertexIndex - 1].Add(new Vector3D(x3, y3, z3));
                        textureIndices[objFaceVertexList[index].VertexIndex - 1] = objFaceVertexList[index].TextureIndex - 1;
                    }
                }
            }

            mesh.FaceSets = new List<FLVER2.FaceSet>
            {
                Generate.GenerateBasicFaceSet()
            };
            mesh.FaceSets[0].Indices = faceIndices;

            //Set all the vertices.
            for (var i = 0; i < objModel.Vertices.Count; i++)
            {
                // Create new vertex data
                ObjLoader.Loader.Data.VertexData.Vertex objVertex = objModel.Vertices[i];
                var newNormal = new System.Numerics.Vector3(0, 1, 0);
                var newTangents = new List<System.Numerics.Vector3>
                {
                    new System.Numerics.Vector3(1, 0, 0)
                };

                var newUVs = new List<System.Numerics.Vector3>
                {
                    new System.Numerics.Vector3(), new System.Numerics.Vector3()
                };

                // Prepare new vertex data
                newNormal = vertexNormalList[i].CalculateAvgNormal().ToNumericsVector3();
                newTangents[0] = Program.RotatePoint(newNormal, 0, (float)Math.PI / 2, 0);

                var newBiTangentRotation = Program.RotatePoint(newNormal, 0, (float)Math.PI / 2, 0);
                var newBiTangent = new Vector4(newBiTangentRotation.X, newBiTangentRotation.Y, newBiTangentRotation.Z, 0);

                if (objModel.Textures != null)
                {
                    if (i < objModel.Textures.Count)
                    {
                        ObjLoader.Loader.Data.VertexData.Texture vm = objModel.Textures[textureIndices[i]];
                        newUVs[0] = new System.Numerics.Vector3(vm.X, vm.Y, 0);
                        newUVs[1] = new System.Numerics.Vector3(vm.X, vm.Y, 0);
                    }
                }

                //
                mesh.Vertices.Add(Generate.GenerateNewFlverVertexUsingNumericsTan(new System.Numerics.Vector3(objVertex.X, objVertex.Y, objVertex.Z), newNormal, newTangents, newBiTangent, newUVs));
            }
            var matnew = new JavaScriptSerializer().Deserialize<FLVER2.Material>(new JavaScriptSerializer().Serialize(flver2.Materials[0]));
            matnew.Name = res.Substring(res.LastIndexOf('\\') + 1);
            flver2.Materials.Add(matnew);
            mesh.MaterialIndex = materialCount;
            flver2.Meshes.Add(mesh);
            MessageBox.Show("Added a custom mesh! PLease click modify to save it!");
            MainWindow.UpdateMesh();
            //mn.Vertices.Add();
        }

        /// <summary>
        /// Gets face vertices from an ObjLoader face
        /// </summary>
        /// <param name="face">An ObjLoader face</param>
        /// <returns>An ObjLoader face vertex array</returns>
        private static FaceVertex[] GetVertices(ObjLoader.Loader.Data.Elements.Face face)
        {
            var ans = new FaceVertex[face.Count];
            for (var i = 0; i < face.Count; i++)
            {
                ans[i] = face[i];
            }
            return ans;
        }
    }
}
