using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using Assimp;
using Microsoft.Xna.Framework;
using ObjLoader.Loader.Data.Elements;
using ObjLoader.Loader.Data.VertexData;
using ObjLoader.Loader.Loaders;
using SoulsFormats;
using Face = ObjLoader.Loader.Data.Elements.Face;
using Point = System.Drawing.Point;
using Vector4 = System.Numerics.Vector4;

namespace FLVER_Editor
{
    /// <summary>
    /// Part of the program class
    /// Contains some important tools
    /// Might need to be sorted a bit more
    /// </summary>
    internal static partial class Program
    {
        /// <summary>
        /// Retrieves the default buffer layout object for a standard FLVER model file.
        /// </summary>
        /// <returns></returns>
        public static FLVER2.BufferLayout GetDefaultBufferLayout()
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
        /// Converts VertexBoneIndices to an int array
        /// Needed for some null checks
        /// </summary>
        /// <param name="boneIndices">A VertexBoneIndices struct to convert</param>
        /// <returns>A new int array from the contents of the VertexBoneIndices struct</returns>
        public static int[] BoneIndicesToIntArray(FLVER.VertexBoneIndices boneIndices)
        {
            return new int[] { boneIndices[0], boneIndices[1], boneIndices[2], boneIndices[3] };
        }

        /// <summary>
        /// Converts VertexBoneWeights to a float array
        /// Needed for some null checks
        /// </summary>
        /// <param name="boneWeights">A VertexBoneWeights struct to convert</param>
        /// <returns>A new float array from the contents of the VertexBoneWeights struct</returns>
        public static float[] BoneWeightsToFloatArray(FLVER.VertexBoneWeights boneWeights)
        {
            return new float[] { boneWeights[0], boneWeights[1], boneWeights[2], boneWeights[3] };
        }

        /// <summary>
        /// Converts a Xna Vector3 to a numerics Vector3
        /// </summary>
        /// <param name="vector3">An Xna Vector3</param>
        /// <returns>A numerics Vector3</returns>
        public static Vector3 XnaToNumericsVector3(System.Numerics.Vector3 vector3)
        {
            return new Vector3(vector3.X, vector3.Y, vector3.Z);
        }

        /// <summary>
        /// Converts an Xna Vector3 to a numerics Vector3 while swapping Z and Y
        /// </summary>
        /// <param name="vector3">An Xna Vector3</param>
        /// <returns>A numerics Vector3 with Z and Y swapped</returns>
        public static Vector3 XnaToNumericsVector3XZY(System.Numerics.Vector3 vector3)
        {
            return new Vector3(vector3.X, vector3.Z, vector3.Y);
        }

        /// <summary>
        /// Converts a numerics Vector3 to an Xna Vector3D
        /// </summary>
        /// <param name="vector3">A numerics Vector3</param>
        /// <returns>An Xna Vector3</returns>
        public static Vector3 NumericsToXnaVector3(System.Numerics.Vector3 vector3)
        {
            return new Vector3(vector3.X, vector3.Y, vector3.Z);
        }

        /// <summary>
        /// Converts a numerics Vector3 to an Xna Vector3D while swapping Z and Y
        /// </summary>
        /// <param name="vector3">A numerics Vector3</param>
        /// <returns>An Xna Vector3 with Z and Y swapped</returns>
        public static Vector3 NumericsToXnaVector3XZY(System.Numerics.Vector3 vector3)
        {
            return new Vector3(vector3.X, vector3.Z, vector3.Y);
        }

        /// <summary>
        /// Computes the transform a vertex should have from its bone and that bone's parent bones
        /// </summary>
        /// <param name="model">A FLVER model</param>
        /// <param name="mesh">A mesh from the FLVER model</param>
        /// <param name="vertex">A vertex from the mesh of the FromSoftware model</param>
        /// <returns>A transform for vertex from its bone and that bone's parent bones</returns>
        public static System.Numerics.Matrix4x4 ComputeTransform(FLVER2 model, FLVER2.Mesh mesh, FLVER.Vertex vertex)
        {
            if (mesh.Dynamic == 0)
            {
                int boneIndiceIndex = vertex.NormalW;

                var bone = model.Bones[mesh.BoneIndices[boneIndiceIndex]];
                System.Numerics.Matrix4x4 transform = bone.ComputeLocalTransform();
                while (bone.ParentIndex != -1)
                {
                    bone = model.Bones[bone.ParentIndex];
                    transform *= bone.ComputeLocalTransform();
                }

                return transform;
            }
            else
            {
                var transform = new System.Numerics.Matrix4x4();

                for (var i = 0; i < vertex.BoneIndices.Length; i++)
                {
                    int boneIndiceIndex = vertex.BoneIndices[i];
                    var bone = model.Bones[mesh.BoneIndices[boneIndiceIndex]];
                    var boneTransform = bone.ComputeLocalTransform();
                    while (bone.ParentIndex != -1)
                    {
                        bone = model.Bones[bone.ParentIndex];
                        boneTransform *= bone.ComputeLocalTransform();
                    }

                    transform *= boneTransform;
                }

                return transform;
            }
        }

        /// <summary>
        /// Deprecated, cannot solve tangent properly.
        /// (-- Note by WarpZephyr: Will be looking into later, the above comment is original FLVER Editor creator)
        /// </summary>
        private static void ImportObj()
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
            int layoutCount = flver.BufferLayouts.Count;
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

            flver.BufferLayouts.Add(newBufferLayout);
            int materialCount = flver.Materials.Count;
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
            var vertexNormalList = new VertexNormalList[objModel.Vertices.Count + 1];
            for (var i = 0; i < vertexNormalList.Length; i++)
            {
                vertexNormalList[i] = new VertexNormalList();
            }
            var faceIndices = new List<int>();
            var textureIndices = new int[objModel.Vertices.Count + 1];
            foreach (Group gr in objModel.Groups)
            {
                foreach (Face faces in gr.Faces)
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
                GenerateBasicFaceSet()
            };
            mesh.FaceSets[0].Indices = faceIndices;

            //Set all the vertices.
            for (var i = 0; i < objModel.Vertices.Count; i++)
            {
                // Create new vertex data
                Vertex objVertex = objModel.Vertices[i];
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
                newTangents[0] = RotatePoint(newNormal, 0, (float)Math.PI / 2, 0);

                var newBiTangentRotation = RotatePoint(newNormal, 0, (float)Math.PI / 2, 0);
                var newBiTangent = new Vector4(newBiTangentRotation.X, newBiTangentRotation.Y, newBiTangentRotation.Z, 0);

                if (objModel.Textures != null)
                {
                    if (i < objModel.Textures.Count)
                    {
                        Texture vm = objModel.Textures[textureIndices[i]];
                        newUVs[0] = new System.Numerics.Vector3(vm.X, vm.Y, 0);
                        newUVs[1] = new System.Numerics.Vector3(vm.X, vm.Y, 0);
                    }
                }

                //
                mesh.Vertices.Add(GenerateNewFlverVertexUsingNumericsTan(new System.Numerics.Vector3(objVertex.X, objVertex.Y, objVertex.Z), newNormal, newTangents, newBiTangent, newUVs));
            }
            var matnew = new JavaScriptSerializer().Deserialize<FLVER2.Material>(new JavaScriptSerializer().Serialize(flver.Materials[0]));
            matnew.Name = res.Substring(res.LastIndexOf('\\') + 1);
            flver.Materials.Add(matnew);
            mesh.MaterialIndex = materialCount;
            flver.Meshes.Add(mesh);
            MessageBox.Show("Added a custom mesh! PLease click modify to save it!");
            MainWindow.UpdateMesh();
            //mn.Vertices.Add();
        }

        /// <summary>
        /// Prints an Assimp node structure to the console with the depth specified
        /// </summary>
        /// <param name="node">The starting child node</param>
        /// <param name="depth">The depth to go to in the node structure</param>
        /// <param name="parent">The parent node of the chosen child node, optional</param>
        private static void PrintNodeStruct(Node node, int depth = 0, string parent = null)
        {
            if (node.ChildCount == 0)
            {
                var pred = "";
                for (var i = 0; i < depth; i++)
                {
                    pred += "\t";
                }
                if (!node.Name.Contains("$AssimpFbx$"))
                {
                    if (!boneParentList.ContainsKey(node.Name))
                        boneParentList.Add(node.Name, parent);
                    if (parent == null)
                    {
                        parent = "";
                    }
                    Console.Write(pred + parent + "->" + node.Name + "\n");
                }
            }
            else
            {
                var pred = "";
                for (var i = 0; i < depth; i++)
                {
                    pred += "\t";
                }
                string nextParent = parent;
                var increase = 0;
                if (!node.Name.Contains("$AssimpFbx$"))
                {
                    nextParent = node.Name;
                    if (!boneParentList.ContainsKey(node.Name))
                        boneParentList.Add(node.Name, parent);
                    if (parent == null)
                    {
                        parent = "";
                    }
                    increase = 1;
                    Console.Write(pred + parent + "->" + node.Name + "\n");
                }
                foreach (Node nn in node.Children)
                {
                    PrintNodeStruct(nn, depth + increase, nextParent);
                }
            }
        }

        /// <summary>
        /// Converts an Assimp Vector3D to a Vector3D type by FLVER Editor
        /// </summary>
        /// <param name="vector3D">An Assimp Vector3D</param>
        /// <returns>A FLVER Editor Vector3D</returns>
        private static Vector3D AssimpVector3DToFEVector3D(Assimp.Vector3D vector3D)
        {
            return new Vector3D(vector3D.X, vector3D.Y, vector3D.Z);
        }

        /// <summary>
        /// Converts an Xna Vector3 to a Vector3D type by FLVER Editor
        /// </summary>
        /// <param name="vector3">An Xna Vector3</param>
        /// <returns>A FLVER Editor Vector3D</returns>
        private static Vector3D ConvertXnaVector3ToInternalVector3D(Microsoft.Xna.Framework.Vector3 vector3)
        {
            return new Vector3D(vector3.X, vector3.Y, vector3.Z);
        }

        /// <summary>
        /// Generates a new basic FLVER2 FaceSet
        /// </summary>
        /// <returns></returns>
        public static FLVER2.FaceSet GenerateBasicFaceSet()
        {
            var newFaceSet = new FLVER2.FaceSet
            {
                TriangleStrip = false,
                CullBackfaces = false
            };
            return newFaceSet;
        }

        /// <summary>
        /// Generates a new FLVER Vertex using an existing FLVER vertex
        /// BoneIndices and BoneWeights will be created new all set to zero except for the first element of the BoneWeights
        /// </summary>
        /// <param name="vertex">A FLVER vertex</param>
        /// <returns>A new flver vertex</returns>
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

            var newTangents = new List<Vector4>();
            foreach (var tangent in vertex.Tangents) newTangents.Add(new Vector4(tangent.X, tangent.Y, tangent.Z, tangent.W));

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
                Bitangent = new Vector4(vertex.Bitangent.X, vertex.Bitangent.Y, vertex.Bitangent.Z, vertex.Bitangent.W),
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
        /// <param name="position">A Vector3 of the position of the vertex</param>
        /// <param name="normal">A Vector3 of the normal of the vertex</param>
        /// <param name="tangents">A list of Vector4 tangents, all tangents in the list will be added</param>
        /// <param name="bitangent">A Vector4 representing the bitangent of the vertex</param>
        /// <param name="UVs">A list of Vector3 representing UVs of the vertex, all UVs in the list will be added</param>
        /// <param name="normalW">An int for the normalW of the vertex, will be -1 by default if not set</param>
        /// <returns>A new flver vertex</returns>
        public static FLVER.Vertex GenerateNewFlverVertexUsingNumerics(System.Numerics.Vector3 position, System.Numerics.Vector3 normal, List<Vector4> tangents, Vector4 bitangent, List<System.Numerics.Vector3> UVs, int normalW = -1)
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

            var newTangents = new List<Vector4>
            {
                new Vector4(tangents[0].X, tangents[0].Y, tangents[0].Z, tangents[0].W),
                new Vector4(tangents[0].X, tangents[0].Y, tangents[0].Z, tangents[0].W)
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
                Bitangent = new Vector4(bitangent.X, bitangent.Y, bitangent.Z, bitangent.W),
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
        /// <param name="position">An Xna Vector3 of the position of the vertex</param>
        /// <param name="normal">An Xna Vector3 of the normal of the vertex</param>
        /// <param name="tangents">A list of Xna Vector4 tangents, all tangents in the list will be added</param>
        /// <param name="bitangent">An Xna Vector4 representing the bitangent of the vertex</param>
        /// <param name="UVs">A list of Xna Vector3 representing UVs of the vertex, all UVs in the list will be added</param>
        /// <param name="normalW">An int for the normalW of the vertex, will be -1 by default if not set</param>
        /// <returns>A new flver vertex</returns>
        public static FLVER.Vertex GenerateNewFlverVertexUsingXna(Vector3 position, Vector3 normal, List<Microsoft.Xna.Framework.Vector4> tangents, Microsoft.Xna.Framework.Vector4 bitangent, List<Vector3> UVs, int normalW = -1)
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

            var newTangents = new List<Vector4>
            {
                new Vector4(tangents[0].X, tangents[0].Y, tangents[0].Z, tangents[0].W),
                new Vector4(tangents[0].X, tangents[0].Y, tangents[0].Z, tangents[0].W)
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
                Bitangent = new Vector4(bitangent.X, bitangent.Y, bitangent.Z, bitangent.W),
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
        /// <param name="position">A Vector3 of the position of the vertex</param>
        /// <param name="normal">A Vector3 of the normal of the vertex</param>
        /// <param name="tangents">A list of Vector3 tangents, all tangents in the list will be added</param>
        /// <param name="bitangent">A Vector4 representing the bitangent of the vertex</param>
        /// <param name="UVs">A list of Vector3 representing UVs of the vertex, all UVs in the list will be added</param>
        /// <param name="tangentW">An int for the tangent W of the vertex, will be -1 by default if not set</param>
        /// <param name="normalW">An int for the normalW of the vertex, will be -1 by default if not set</param>
        /// <returns>A new flver vertex</returns>
        public static FLVER.Vertex GenerateNewFlverVertexUsingNumericsTan(System.Numerics.Vector3 position, System.Numerics.Vector3 normal, List<System.Numerics.Vector3> tangents, Vector4 bitangent, List<System.Numerics.Vector3> UVs, int tangentW = -1, int normalW = -1)
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

            var newTangents = new List<Vector4>
            {
                new Vector4(tangents[0].X, tangents[0].Y, tangents[0].Z, tangentW),
                new Vector4(tangents[0].X, tangents[0].Y, tangents[0].Z, tangentW)
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
                Bitangent = new Vector4(bitangent.X, bitangent.Y, bitangent.Z, bitangent.W),
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
        /// <param name="position">An Xna Vector3 of the position of the vertex</param>
        /// <param name="normal">An Xna Vector3 of the normal of the vertex</param>
        /// <param name="tangents">A list of Xna Vector4 tangents, all tangents in the list will be added</param>
        /// <param name="bitangent">An Xna Vector4 representing the bitangent of the vertex</param>
        /// <param name="UVs">A list of Xna Vector3 representing UVs of the vertex, all UVs in the list will be added</param>
        /// <param name="normalW">An int for the normalW of the vertex, will be -1 by default if not set</param>
        /// <param name="tangentW">An int for the tangent W of the vertex, will be -1 by default if not set</param>
        /// <returns>A new flver vertex</returns>
        public static FLVER.Vertex GenerateNewFlverVertexUsingXnaTan(Vector3 position, Vector3 normal, List<Vector3> tangents, Microsoft.Xna.Framework.Vector4 bitangent, List<Vector3> UVs, int tangentW = -1, int normalW = -1)
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

            var newTangents = new List<Vector4>
            {
                new Vector4(tangents[0].X, tangents[0].Y, tangents[0].Z, tangentW),
                new Vector4(tangents[0].X, tangents[0].Y, tangents[0].Z, tangentW)
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
                Bitangent = new Vector4(bitangent.X, bitangent.Y, bitangent.Z, bitangent.W),
                Colors = newColors,
            };

            return newVertex;
        }

        /*************** Basic Tools section *****************/

        /// <summary>
        /// Rotate a numerics Vector3 point
        /// </summary>
        /// <param name="point">The numerics Vector3 point to rotate</param>
        /// <param name="pitch">The pitch to rotate by as a float</param>
        /// <param name="roll">The roll to rotate by as a float</param>
        /// <param name="yaw">The yaw to rotate by as a float</param>
        /// <returns>A rotated numerics Vector3</returns>
        public static System.Numerics.Vector3 RotatePoint(System.Numerics.Vector3 point, float pitch, float roll, float yaw)
        {
            var ans = new System.Numerics.Vector3(0, 0, 0);
            double cosa = Math.Cos(yaw);
            double sina = Math.Sin(yaw);
            double cosb = Math.Cos(pitch);
            double sinb = Math.Sin(pitch);
            double cosc = Math.Cos(roll);
            double sinc = Math.Sin(roll);
            double Axx = cosa * cosb;
            double Axy = cosa * sinb * sinc - sina * cosc;
            double Axz = cosa * sinb * cosc + sina * sinc;
            double Ayx = sina * cosb;
            double Ayy = sina * sinb * sinc + cosa * cosc;
            double Ayz = sina * sinb * cosc - cosa * sinc;
            double Azx = -sinb;
            double Azy = cosb * sinc;
            double Azz = cosb * cosc;
            float px = point.X;
            float py = point.Y;
            float pz = point.Z;
            ans.X = (float)(Axx * px + Axy * py + Axz * pz);
            ans.Y = (float)(Ayx * px + Ayy * py + Ayz * pz);
            ans.Z = (float)(Azx * px + Azy * py + Azz * pz);
            return ans;
        }

        /// <summary>
        /// Rotate a numerics Vector4 point
        /// </summary>
        /// <param name="point">The numerics Vector4 point to rotate</param>
        /// <param name="pitch">The pitch to rotate by as a float</param>
        /// <param name="roll">The roll to rotate by as a float</param>
        /// <param name="yaw">The yaw to rotate by as a float</param>
        /// <returns>A rotated numerics Vector4</returns>
        public static Vector4 RotatePoint(Vector4 point, float pitch, float roll, float yaw)
        {
            var ans = new Vector4(0, 0, 0, point.W);
            double cosa = Math.Cos(yaw);
            double sina = Math.Sin(yaw);
            double cosb = Math.Cos(pitch);
            double sinb = Math.Sin(pitch);
            double cosc = Math.Cos(roll);
            double sinc = Math.Sin(roll);
            double Axx = cosa * cosb;
            double Axy = cosa * sinb * sinc - sina * cosc;
            double Axz = cosa * sinb * cosc + sina * sinc;
            double Ayx = sina * cosb;
            double Ayy = sina * sinb * sinc + cosa * cosc;
            double Ayz = sina * sinb * cosc - cosa * sinc;
            double Azx = -sinb;
            double Azy = cosb * sinc;
            double Azz = cosb * cosc;
            float px = point.X;
            float py = point.Y;
            float pz = point.Z;
            ans.X = (float)(Axx * px + Axy * py + Axz * pz);
            ans.Y = (float)(Ayx * px + Ayy * py + Ayz * pz);
            ans.Z = (float)(Azx * px + Azy * py + Azz * pz);
            return ans;
        }

        /// <summary>
        /// Rotate a line on a shape with numerics Vector3 points
        /// </summary>
        /// <param name="point"></param>
        /// <param name="org"></param>
        /// <param name="direction"></param>
        /// <param name="theta"></param>
        /// <returns></returns>
        public static System.Numerics.Vector3 RotateLine(System.Numerics.Vector3 point, System.Numerics.Vector3 org, System.Numerics.Vector3 direction, double theta)
        {
            double x = point.X;
            double y = point.Y;
            double z = point.Z;

            double a = org.X;
            double b = org.Y;
            double c = org.Z;

            double nu = direction.X / direction.Length();
            double nv = direction.Y / direction.Length();
            double nw = direction.Z / direction.Length();

            var rP = new double[3];
            rP[0] = (a * (nv * nv + nw * nw) - nu * (b * nv + c * nw - nu * x - nv * y - nw * z)) * (1 - Math.Cos(theta))
                    + x * Math.Cos(theta)
                    + (-c * nv + b * nw - nw * y + nv * z) * Math.Sin(theta);

            rP[1] = (b * (nu * nu + nw * nw) - nv * (a * nu + c * nw - nu * x - nv * y - nw * z)) * (1 - Math.Cos(theta))
                    + y * Math.Cos(theta)
                    + (c * nu - a * nw + nw * x - nu * z) * Math.Sin(theta);

            rP[2] = (c * (nu * nu + nv * nv) - nw * (a * nu + b * nv - nu * x - nv * y - nw * z)) * (1 - Math.Cos(theta))
                    + z * Math.Cos(theta)
                    + (-b * nu + a * nv - nv * x + nu * y) * Math.Sin(theta);

            var ans = new System.Numerics.Vector3((float)rP[0], (float)rP[1], (float)rP[2]);
            return ans;
        }

        /// <summary>
        /// Computes the Dot Product of two Xna Vector3s
        /// </summary>
        /// <param name="vector1">The first Xna Vector3</param>
        /// <param name="vector2">The second Xna Vector3</param>
        /// <returns>The Dot Product of the Xna Vector3s as an Xna Vector3</returns>
        public static float XnaDotProduct(Vector3 vector1, Vector3 vector2)
        {
            float x1 = vector1.X;
            float y1 = vector1.Y;
            float z1 = vector1.Z;
            float x2 = vector2.X;
            float y2 = vector2.Y;
            float z2 = vector2.Z;
            return x1 * x2 + y1 * y2 + z1 * z2;
        }

        /// <summary>
        /// Computes the Cross Product of two Xna Vector3s
        /// </summary>
        /// <param name="vector1">The first Xna Vector3</param>
        /// <param name="vector2">The second Xna Vector3</param>
        /// <returns>The Cross Product of the Xna Vector3s as an Xna Vector3</returns>
        public static Vector3 XnaCrossProduct(Vector3 vector1, Vector3 vector2)
        {
            float x1 = vector1.X;
            float y1 = vector1.Y;
            float z1 = vector1.Z;
            float x2 = vector2.X;
            float y2 = vector2.Y;
            float z2 = vector2.Z;
            return new Vector3(y1 * z2 - z1 * y2, z1 * x2 - x1 * z2, x1 * y2 - y1 * x2);
        }

        /// <summary>
        /// Swaps a sekiro model with a Dark Souls or BloodBorne model
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
            if (copyMaterial.Checked)  seikiroDonorModel.Materials = replacementModel.Materials;
            if (copyBones.Checked)     seikiroDonorModel.Bones = replacementModel.Bones;
            if (copyDummy.Checked)     seikiroDonorModel.Dummies = replacementModel.Dummies;
            if (weightToFirstBone.Checked)
            {
                for (var i = 0; i < seikiroDonorModel.Meshes.Count; i++)
                {
                    seikiroDonorModel.Meshes[i].BoneIndices = new List<int>{0, 1};
                    seikiroDonorModel.Meshes[i].DefaultBoneIndex = 1;
                    foreach (FLVER.Vertex vertex in seikiroDonorModel.Meshes[i].Vertices)
                    {
                        if (BoneWeightsToFloatArray(vertex.BoneWeights) == null) continue;

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
        /// Gets face vertices from an ObjLoader face
        /// </summary>
        /// <param name="face">An ObjLoader face</param>
        /// <returns>An ObjLoader face vertex array</returns>
        private static FaceVertex[] GetVertices(Face face)
        {
            var ans = new FaceVertex[face.Count];
            for (var i = 0; i < face.Count; i++)
            {
                ans[i] = face[i];
            }
            return ans;
        }

        /// <summary>
        /// Export data from a single string to JSON
        /// I honestly would have used a string array for this :fatcat:
        /// </summary>
        /// <param name="content">The string to export to JSON</param>
        /// <param name="fileName">The name of the resulting JSON file, "export.json" by default</param>
        /// <param name="endMessage">A string containing a message to show in a MessageBox to the user after the export has finished</param>
        public static void ExportJson(string content, string fileName = "export.json", string endMessage = "")
        {
            var saveDialog = new SaveFileDialog
            {
                FileName = fileName
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var sw = new StreamWriter(saveDialog.FileName);
                    sw.Write(content);
                    sw.Close();
                    MessageBox.Show(endMessage, "Info");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" + $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        /// <summary>
        /// Formats a JSON string into a more readable string
        /// </summary>
        /// <param name="jsonString">A</param>
        /// <returns>A string</returns>
        public static string FormatOutput(string jsonString)
        {
            var stringBuilder = new StringBuilder();
            var escaping = false;
            var inQuotes = false;
            var indentation = 0;
            foreach (char character in jsonString)
            {
                if (escaping)
                {
                    escaping = false;
                    stringBuilder.Append(character);
                }
                else
                {
                    if (character == '\\')
                    {
                        escaping = true;
                        stringBuilder.Append(character);
                    }
                    else if (character == '\"')
                    {
                        inQuotes = !inQuotes;
                        stringBuilder.Append(character);
                    }
                    else if (!inQuotes)
                    {
                        if (character == ',')
                        {
                            stringBuilder.Append(character);
                            stringBuilder.Append("\r\n");
                            stringBuilder.Append('\t', indentation);
                        }
                        else if (character == '[' || character == '{')
                        {
                            stringBuilder.Append(character);
                            stringBuilder.Append("\r\n");
                            stringBuilder.Append('\t', ++indentation);
                        }
                        else if (character == ']' || character == '}')
                        {
                            stringBuilder.Append("\r\n");
                            stringBuilder.Append('\t', --indentation);
                            stringBuilder.Append(character);
                        }
                        else if (character == ':')
                        {
                            stringBuilder.Append(character);
                            stringBuilder.Append('\t');
                        }
                        else
                        {
                            stringBuilder.Append(character);
                        }
                    }
                    else
                    {
                        stringBuilder.Append(character);
                    }
                }
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// A class for containing a vertex normal list so that an average can be calculated
        /// </summary>
        private class VertexNormalList
        {
            public readonly List<Vector3D> normals = new List<Vector3D>();

            /// <summary>
            /// Calculate the average of all the normals added to the normals list
            /// </summary>
            /// <returns>An FE Vector3D normal that is the average of all the normals in the normals list</returns>
            public Vector3D CalculateAvgNormal()
            {
                var normalAverage = new Vector3D();
                foreach (Vector3D normal in normals)
                {
                    normalAverage += normal;
                }
                return normalAverage.Normalize();
            }

            /// <summary>
            /// Adds to the normals list of the VertexNormalList class
            /// </summary>
            /// <param name="normal">A FE Vector3D normal to add to the list</param>
            public void Add(Vector3D normal)
            {
                normals.Add(normal);
            }
        }
    }
}