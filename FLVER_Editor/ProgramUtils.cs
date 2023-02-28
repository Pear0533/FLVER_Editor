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
    internal static partial class Program
    {
        public static Vector3 toXnaV3(System.Numerics.Vector3 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static Vector3 toXnaV3XZY(System.Numerics.Vector3 v)
        {
            return new Vector3(v.X, v.Z, v.Y);
        }

        public static System.Numerics.Vector3 findBoneTrans(List<FLVER.Bone> b, int index, System.Numerics.Vector3 v = new System.Numerics.Vector3())
        {
            /* if (bonePosList[index] != null)
             {
                 return bonePosList[index];
             }

             if (b[index].ParentIndex == -1)
             {
                 v += b[index].Translation;
                 return v;
             }
             Vector3 ans = findBoneTrans(b, b[index].ParentIndex, v);
             bonePosList[index] = ans;
             return ans;*/
            if (bonePosList[index] != null)
            {
                return bonePosList[index].toNumV3();
            }
            if (b[index].ParentIndex == -1)
            {
                bonePosList[index] = new Vector3D(b[index].Translation);
                return b[index].Translation;
            }
            System.Numerics.Vector3 ans = b[index].Translation + findBoneTrans(b, b[index].ParentIndex);
            bonePosList[index] = new Vector3D(ans);
            return ans;
        }

        /// <summary>
        ///     Deprecated, cannot solve tangent properly.
        /// </summary>
        private static void importObj()
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
            LoadResult result = objLoader.Load(fileStream);

            // ObjLoader.Loader.Data.Elements.Face f = result.Groups[0].Faces[0];
            // ObjLoader.Loader.Data.Elements.FaceVertex[] fv =getVertices(f);

            // string groups = new JavaScriptSerializer().Serialize(fv);
            //string vertices = new JavaScriptSerializer().Serialize(result.Vertices);

            //MessageBox.Show(groups,"Group info");
            // MessageBox.Show(vertices, "V info");
            fileStream.Close();

            //Step 1 add a new buffer layout for my program:
            int layoutCount = flver.BufferLayouts.Count;
            var newBL = new FLVER.BufferLayout();
            newBL.Add(new FLVER.BufferLayout.Member(0, 0, FLVER.BufferLayout.MemberType.Float3, FLVER.BufferLayout.MemberSemantic.Position, 0));
            newBL.Add(new FLVER.BufferLayout.Member(0, 12, FLVER.BufferLayout.MemberType.Byte4B, FLVER.BufferLayout.MemberSemantic.Normal, 0));
            newBL.Add(new FLVER.BufferLayout.Member(0, 16, FLVER.BufferLayout.MemberType.Byte4B, FLVER.BufferLayout.MemberSemantic.Tangent, 0));
            newBL.Add(new FLVER.BufferLayout.Member(0, 20, FLVER.BufferLayout.MemberType.Byte4B, FLVER.BufferLayout.MemberSemantic.Tangent, 1));
            newBL.Add(new FLVER.BufferLayout.Member(0, 24, FLVER.BufferLayout.MemberType.Byte4B, FLVER.BufferLayout.MemberSemantic.BoneIndices, 0));
            newBL.Add(new FLVER.BufferLayout.Member(0, 28, FLVER.BufferLayout.MemberType.Byte4C, FLVER.BufferLayout.MemberSemantic.BoneWeights, 0));
            newBL.Add(new FLVER.BufferLayout.Member(0, 32, FLVER.BufferLayout.MemberType.Byte4C, FLVER.BufferLayout.MemberSemantic.VertexColor, 1));
            newBL.Add(new FLVER.BufferLayout.Member(0, 36, FLVER.BufferLayout.MemberType.UVPair, FLVER.BufferLayout.MemberSemantic.UV, 0));
            flver.BufferLayouts.Add(newBL);
            int materialCount = flver.Materials.Count;
            var mn = new FLVER.Mesh();
            mn.MaterialIndex = 0;
            mn.BoneIndices = new List<int>();
            mn.BoneIndices.Add(0);
            mn.BoneIndices.Add(1);
            mn.BoundingBoxMax = new System.Numerics.Vector3(1, 1, 1);
            mn.BoundingBoxMin = new System.Numerics.Vector3(-1, -1, -1);
            mn.BoundingBoxUnk = new System.Numerics.Vector3();
            mn.Unk1 = 0;
            mn.DefaultBoneIndex = 0;
            mn.Dynamic = false;
            mn.VertexBuffers = new List<FLVER.VertexBuffer>();
            mn.VertexBuffers.Add(new FLVER.VertexBuffer(0, layoutCount, -1));
            mn.Vertices = new List<FLVER.Vertex>();
            // mn.Vertices.Add(generateVertex(new Vector3(1,0,0),new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(0,1,0),new Vector3(1,0,0)));
            //mn.Vertices.Add(generateVertex(new Vector3(0, 1, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 0, 0)));
            //mn.Vertices.Add(generateVertex(new Vector3(0, 0, 1), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 0, 0)));
            if (result.Groups.Count == 0)
            {
                MessageBox.Show("You imported nothing!");
                return;
            }
            MessageBox.Show("Vertice number:"
                + result.Vertices.Count
                + "Texture V number:"
                + result.Textures.Count
                + "Normal number:"
                + result.Normals.Count
                + "Face groups:"
                + result.Groups[0].Faces.Count);
            var vnlist = new VertexNormalList[result.Vertices.Count + 1];
            for (var i = 0; i < vnlist.Length; i++)
            {
                vnlist[i] = new VertexNormalList();
            }
            var faceIndexs = new List<uint>();
            var textureIndexs = new uint[result.Vertices.Count + 1];
            foreach (Group gr in result.Groups)
            {
                foreach (Face faces in gr.Faces)
                {
                    FaceVertex[] vList = getVertices(faces);
                    /*for (int i3 = 0; i3 < vList.Length - 2; i3++)
                    {
                        faceIndexs.Add((uint)(vList[i3].VertexIndex)-1);
                        faceIndexs.Add((uint)(vList[i3+1].VertexIndex)-1);
                        faceIndexs.Add((uint)(vList[i3+2].VertexIndex)-1);
                    }*/
                    if (vList.Length == 4)
                    {
                        faceIndexs.Add((uint)vList[0].VertexIndex - 1);
                        faceIndexs.Add((uint)vList[2].VertexIndex - 1);
                        faceIndexs.Add((uint)vList[1].VertexIndex - 1);

                        //record normal to help calculate vertex normals
                        var helperI = 0;
                        vnlist[(uint)vList[helperI].VertexIndex - 1].add(new Vector3D(result.Normals[vList[helperI].NormalIndex - 1].X,
                            result.Normals[vList[helperI].NormalIndex - 1].Y, result.Normals[vList[helperI].NormalIndex - 1].Z));
                        textureIndexs[vList[helperI].VertexIndex - 1] = (uint)vList[helperI].TextureIndex - 1;
                        helperI = 2;
                        vnlist[(uint)vList[helperI].VertexIndex - 1].add(new Vector3D(result.Normals[vList[helperI].NormalIndex - 1].X,
                            result.Normals[vList[helperI].NormalIndex - 1].Y, result.Normals[vList[helperI].NormalIndex - 1].Z));
                        textureIndexs[vList[helperI].VertexIndex - 1] = (uint)vList[helperI].TextureIndex - 1;
                        helperI = 1;
                        vnlist[(uint)vList[helperI].VertexIndex - 1].add(new Vector3D(result.Normals[vList[helperI].NormalIndex - 1].X,
                            result.Normals[vList[helperI].NormalIndex - 1].Y, result.Normals[vList[helperI].NormalIndex - 1].Z));
                        textureIndexs[vList[helperI].VertexIndex - 1] = (uint)vList[helperI].TextureIndex - 1;
                        faceIndexs.Add((uint)vList[2].VertexIndex - 1);
                        faceIndexs.Add((uint)vList[0].VertexIndex - 1);
                        faceIndexs.Add((uint)vList[3].VertexIndex - 1);
                        helperI = 2;
                        vnlist[(uint)vList[helperI].VertexIndex - 1].add(new Vector3D(result.Normals[vList[helperI].NormalIndex - 1].X,
                            result.Normals[vList[helperI].NormalIndex - 1].Y, result.Normals[vList[helperI].NormalIndex - 1].Z));
                        textureIndexs[vList[helperI].VertexIndex - 1] = (uint)vList[helperI].TextureIndex - 1;
                        helperI = 0;
                        vnlist[(uint)vList[helperI].VertexIndex - 1].add(new Vector3D(result.Normals[vList[helperI].NormalIndex - 1].X,
                            result.Normals[vList[helperI].NormalIndex].Y, result.Normals[vList[helperI].NormalIndex].Z));
                        textureIndexs[vList[helperI].VertexIndex - 1] = (uint)vList[helperI].TextureIndex - 1;
                        helperI = 3;
                        vnlist[(uint)vList[helperI].VertexIndex - 1].add(new Vector3D(result.Normals[vList[helperI].NormalIndex].X,
                            result.Normals[vList[helperI].NormalIndex].Y, result.Normals[vList[helperI].NormalIndex].Z));
                        textureIndexs[vList[helperI].VertexIndex - 1] = (uint)vList[helperI].TextureIndex - 1;
                    }
                    else if (vList.Length == 3)
                    {
                        faceIndexs.Add((uint)vList[0].VertexIndex - 1);
                        faceIndexs.Add((uint)vList[2].VertexIndex - 1);
                        faceIndexs.Add((uint)vList[1].VertexIndex - 1);
                        var helperI = 2;
                        vnlist[(uint)vList[helperI].VertexIndex - 1].add(new Vector3D(result.Normals[vList[helperI].NormalIndex - 1].X,
                            result.Normals[vList[helperI].NormalIndex - 1].Y, result.Normals[vList[helperI].NormalIndex - 1].Z));
                        textureIndexs[vList[helperI].VertexIndex - 1] = (uint)vList[helperI].TextureIndex - 1;
                        helperI = 0;
                        vnlist[(uint)vList[helperI].VertexIndex - 1].add(new Vector3D(result.Normals[vList[helperI].NormalIndex - 1].X,
                            result.Normals[vList[helperI].NormalIndex - 1].Y, result.Normals[vList[helperI].NormalIndex - 1].Z));
                        textureIndexs[vList[helperI].VertexIndex - 1] = (uint)vList[helperI].TextureIndex - 1;
                        helperI = 1;
                        vnlist[(uint)vList[helperI].VertexIndex - 1].add(new Vector3D(result.Normals[vList[helperI].NormalIndex - 1].X,
                            result.Normals[vList[helperI].NormalIndex - 1].Y, result.Normals[vList[helperI].NormalIndex - 1].Z));
                        textureIndexs[vList[helperI].VertexIndex - 1] = (uint)vList[helperI].TextureIndex - 1;
                    }
                }
            }
            //mn.FaceSets[0].Vertices = new uint [3]{0,1,2 };
            mn.FaceSets = new List<FLVER.FaceSet>();
            //FLVER.Vertex myv = new FLVER.Vertex();
            //myv.Colors = new List<FLVER.Vertex.Color>();
            mn.FaceSets.Add(generateBasicFaceSet());
            mn.FaceSets[0].Vertices = faceIndexs.ToArray();

            //Set all the vertices.
            for (var iv = 0; iv < result.Vertices.Count; iv++)
            {
                Vertex v = result.Vertices[iv];
                var uv1 = new System.Numerics.Vector3();
                var uv2 = new System.Numerics.Vector3();
                var normal = new System.Numerics.Vector3(0, 1, 0);
                var tangent = new System.Numerics.Vector3(1, 0, 0);
                if (result.Textures != null)
                {
                    if (iv < result.Textures.Count)
                    {
                        Texture vm = result.Textures[(int)textureIndexs[iv]];
                        uv1 = new System.Numerics.Vector3(vm.X, vm.Y, 0);
                        uv2 = new System.Numerics.Vector3(vm.X, vm.Y, 0);
                    }
                }
                normal = vnlist[iv].calculateAvgNormal().toNumV3();
                tangent = RotatePoint(normal, 0, (float)Math.PI / 2, 0);
                mn.Vertices.Add(generateVertex(new System.Numerics.Vector3(v.X, v.Y, v.Z), uv1, uv2, normal, tangent));
            }
            var matnew = new JavaScriptSerializer().Deserialize<FLVER.Material>(new JavaScriptSerializer().Serialize(flver.Materials[0]));
            matnew.Name = res.Substring(res.LastIndexOf('\\') + 1);
            flver.Materials.Add(matnew);
            mn.MaterialIndex = materialCount;
            flver.Meshes.Add(mn);
            MessageBox.Show("Added a custom mesh! PLease click modify to save it!");
            MainWindow.UpdateMesh();
            //mn.Vertices.Add();
        }

        private static void printNodeStruct(Node n, int depth = 0, string parent = null)
        {
            if (n.ChildCount == 0)
            {
                var pred = "";
                for (var i = 0; i < depth; i++)
                {
                    pred += "\t";
                }
                if (!n.Name.Contains("$AssimpFbx$"))
                {
                    if (!boneParentList.ContainsKey(n.Name))
                        boneParentList.Add(n.Name, parent);
                    if (parent == null)
                    {
                        parent = "";
                    }
                    Console.Write(pred + parent + "->" + n.Name + "\n");
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
                if (!n.Name.Contains("$AssimpFbx$"))
                {
                    nextParent = n.Name;
                    if (!boneParentList.ContainsKey(n.Name))
                        boneParentList.Add(n.Name, parent);
                    if (parent == null)
                    {
                        parent = "";
                    }
                    increase = 1;
                    Console.Write(pred + parent + "->" + n.Name + "\n");
                }
                foreach (Node nn in n.Children)
                {
                    printNodeStruct(nn, depth + increase, nextParent);
                }
            }
        }

        private static Vector3D getMyV3D(Assimp.Vector3D v)
        {
            return new Vector3D(v.X, v.Y, v.Z);
        }

        public static FLVER.FaceSet generateBasicFaceSet()
        {
            var ans = new FLVER.FaceSet();
            ans.CullBackfaces = true;
            ans.TriangleStrip = false;
            ans.Unk06 = 1;
            ans.Unk07 = 0;
            ans.IndexSize = 16;
            return ans;
        }

        public static FLVER.Vertex generateVertex(System.Numerics.Vector3 pos, System.Numerics.Vector3 uv1, System.Numerics.Vector3 uv2, System.Numerics.Vector3 normal,
            System.Numerics.Vector3 tangets, int tangentW = -1)
        {
            var ans = new FLVER.Vertex();
            ans.Positions = new List<System.Numerics.Vector3>();
            ans.Positions.Add(pos);
            ans.BoneIndices = new int[4] { 0, 0, 0, 0 };
            ans.BoneWeights = new float[4] { 1, 0, 0, 0 };
            ans.UVs = new List<System.Numerics.Vector3>();
            ans.UVs.Add(uv1);
            ans.UVs.Add(uv2);
            ans.Normals = new List<Vector4>();
            ans.Normals.Add(new Vector4(normal.X, normal.Y, normal.Z, -1f));
            ans.Tangents = new List<Vector4>();
            ans.Tangents.Add(new Vector4(tangets.X, tangets.Y, tangets.Z, tangentW));
            ans.Tangents.Add(new Vector4(tangets.X, tangets.Y, tangets.Z, tangentW));
            ans.Colors = new List<FLVER.Vertex.Color>();
            ans.Colors.Add(new FLVER.Vertex.Color(255, 255, 255, 255));
            return ans;
        }

        public static FLVER.Vertex generateVertexV4(System.Numerics.Vector3 pos, System.Numerics.Vector3 uv1, System.Numerics.Vector3 uv2, Vector4 normal, Vector4 tangets, int tangentW = -1)
        {
            FLVER.Vertex ans = new FLVER.Vertex();
            ans.Positions = new List<System.Numerics.Vector3>();
            ans.Positions.Add(pos);
            ans.BoneIndices = new int[4] { 0, 0, 0, 0 };
            ans.BoneWeights = new float[4] { 1, 0, 0, 0 };
            ans.UVs = new List<System.Numerics.Vector3>();
            ans.UVs.Add(uv1);
            ans.UVs.Add(uv2);
            ans.Normals = new List<Vector4>();
            ans.Normals.Add(new Vector4(normal.X, normal.Y, normal.Z, normal.W));
            ans.Tangents = new List<Vector4>();
            ans.Tangents.Add(new Vector4(tangets.X, tangets.Y, tangets.Z, tangets.W));
            ans.Tangents.Add(new Vector4(tangets.X, tangets.Y, tangets.Z, tangets.W));
            ans.Colors = new List<FLVER.Vertex.Color>();
            ans.Colors.Add(new FLVER.Vertex.Color(255, 255, 255, 255));
            return ans;
        }

        private static FLVER.Vertex generateVertexNew(System.Numerics.Vector3 pos, List<System.Numerics.Vector3> uvs, System.Numerics.Vector3 normal,
            System.Numerics.Vector3 tangets, int tangentW = -1)
        {
            var ans = new FLVER.Vertex();
            ans.Positions = new List<System.Numerics.Vector3>();
            ans.Positions.Add(pos);
            ans.BoneIndices = new int[4] { 0, 0, 0, 0 };
            ans.BoneWeights = new float[4] { 1, 0, 0, 0 };
            ans.UVs = new List<System.Numerics.Vector3>();
            foreach (System.Numerics.Vector3 uv in uvs)
                ans.UVs.Add(uv);
            ans.Normals = new List<Vector4>();
            ans.Normals.Add(new Vector4(normal.X, normal.Y, normal.Z, -1f));
            ans.Tangents = new List<Vector4>();
            ans.Tangents.Add(new Vector4(tangets.X, tangets.Y, tangets.Z, tangentW));
            ans.Tangents.Add(new Vector4(tangets.X, tangets.Y, tangets.Z, tangentW));
            ans.Colors = new List<FLVER.Vertex.Color>();
            ans.Colors.Add(new FLVER.Vertex.Color(255, 255, 255, 255));
            return ans;
        }

        private static FLVER.Vertex generateVertex(System.Numerics.Vector3 pos, System.Numerics.Vector3 uv1, System.Numerics.Vector3 uv2, Vector4 normal, Vector4 tangets,
            int tangentW = -1)
        {
            var ans = new FLVER.Vertex();
            ans.Positions = new List<System.Numerics.Vector3>();
            ans.Positions.Add(pos);
            ans.BoneIndices = new int[4] { 0, 0, 0, 0 };
            ans.BoneWeights = new float[4] { 1, 0, 0, 0 };
            ans.UVs = new List<System.Numerics.Vector3>();
            ans.UVs.Add(uv1);
            ans.UVs.Add(uv2);
            ans.Normals = new List<Vector4>();
            ans.Normals.Add(new Vector4(normal.X, normal.Y, normal.Z, normal.W));
            ans.Tangents = new List<Vector4>();
            ans.Tangents.Add(new Vector4(tangets.X, tangets.Y, tangets.Z, tangets.W));
            ans.Tangents.Add(new Vector4(tangets.X, tangets.Y, tangets.Z, tangets.W));
            ans.Colors = new List<FLVER.Vertex.Color>();
            ans.Colors.Add(new FLVER.Vertex.Color(255, 255, 255, 255));
            return ans;
        }

        private static FLVER.Vertex generateVertex2tan(System.Numerics.Vector3 pos, System.Numerics.Vector3 uv1, System.Numerics.Vector3 uv2, System.Numerics.Vector3 normal,
            System.Numerics.Vector3 tangets, System.Numerics.Vector3 tangets2, int tangentW = -1)
        {
            var ans = new FLVER.Vertex();
            ans.Positions = new List<System.Numerics.Vector3>();
            ans.Positions.Add(pos);
            ans.BoneIndices = new int[4] { 0, 0, 0, 0 };
            ans.BoneWeights = new float[4] { 1, 0, 0, 0 };
            ans.UVs = new List<System.Numerics.Vector3>();
            ans.UVs.Add(uv1);
            ans.UVs.Add(uv2);
            ans.Normals = new List<Vector4>();
            ans.Normals.Add(new Vector4(normal.X, normal.Y, normal.Z, -1f));
            ans.Tangents = new List<Vector4>();
            ans.Tangents.Add(new Vector4(tangets.X, tangets.Y, tangets.Z, tangentW));
            ans.Tangents.Add(new Vector4(tangets2.X, tangets2.Y, tangets2.Z, tangentW));
            ans.Colors = new List<FLVER.Vertex.Color>();
            ans.Colors.Add(new FLVER.Vertex.Color(255, 255, 255, 255));
            return ans;
        }

        /*************** Basic Tools section *****************/

        public static System.Numerics.Vector3 RotatePoint(System.Numerics.Vector3 p, float pitch, float roll, float yaw)
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
            float px = p.X;
            float py = p.Y;
            float pz = p.Z;
            ans.X = (float)(Axx * px + Axy * py + Axz * pz);
            ans.Y = (float)(Ayx * px + Ayy * py + Ayz * pz);
            ans.Z = (float)(Azx * px + Azy * py + Azz * pz);
            return ans;
        }

        public static Vector4 RotatePoint(Vector4 p, float pitch, float roll, float yaw)
        {
            var ans = new Vector4(0, 0, 0, p.W);
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
            float px = p.X;
            float py = p.Y;
            float pz = p.Z;
            ans.X = (float)(Axx * px + Axy * py + Axz * pz);
            ans.Y = (float)(Ayx * px + Ayy * py + Ayz * pz);
            ans.Z = (float)(Azx * px + Azy * py + Azz * pz);
            return ans;
        }

        public static System.Numerics.Vector3 RotateLine(System.Numerics.Vector3 p, System.Numerics.Vector3 org, System.Numerics.Vector3 direction, double theta)
        {
            double x = p.X;
            double y = p.Y;
            double z = p.Z;
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

        public static Vector3 crossPorduct(Vector3 a, Vector3 b)
        {
            float x1 = a.X;
            float y1 = a.Y;
            float z1 = a.Z;
            float x2 = b.X;
            float y2 = b.Y;
            float z2 = b.Z;
            return new Vector3(y1 * z2 - z1 * y2, z1 * x2 - x1 * z2, x1 * y2 - y1 * x2);
        }

        public static float dotProduct(Vector3 a, Vector3 b)
        {
            float x1 = a.X;
            float y1 = a.Y;
            float z1 = a.Z;
            float x2 = b.X;
            float y2 = b.Y;
            float z2 = b.Z;
            return x1 * x2 + y1 * y2 + z1 * z2;
        }

        public static void ModelSwapModule()
        {
            OpenFileDialog openFileDialog1;
            openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog1.Title = "Choose template seikiro model file.";
            //openFileDialog1.ShowDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine(openFileDialog1.FileName);
                //openFileDialog1.
            }
            else
            {
                return;
            }
            FLVER b = FLVER.Read(openFileDialog1.FileName);
            var openFileDialog2 = new OpenFileDialog();
            openFileDialog2.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog2.Title = "Choose source DS/BB model file.";
            //openFileDialog1.ShowDialog();
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine(openFileDialog2.FileName);
                //openFileDialog1.
            }
            else
            {
                return;
            }
            FLVER src = FLVER.Read(openFileDialog2.FileName);
            Console.WriteLine(b.Header);
            Console.WriteLine("Seikiro unk is:" + b.SekiroUnk);
            Console.WriteLine("Material:");
            foreach (FLVER.Material m in b.Materials)
            {
                Console.WriteLine(m.Name);
            }
            foreach (FLVER.Mesh m in b.Meshes)
            {
                Console.WriteLine("Mesh#" + m.MaterialIndex);
            }

            //* new
            //b.Header.BigEndian = src.Header.BigEndian;

            //

            //X: is not the sword axis!!!
            //Y: ++ means closer to the hand!
            //Unit: in meter(?)

            //For Moonlight sword -> threaded cane, Y+0.5f
            var f = new Form();
            var l = new Label();
            l.Text = "x,y,z offset? Y= weapon length axis,Y+=Closer to hand";
            l.Size = new Size(150, 15);
            l.Location = new Point(10, 20);
            f.Controls.Add(l);
            var t = new TextBox();
            t.Size = new Size(70, 15);
            t.Location = new Point(10, 60);
            t.Text = "0";
            f.Controls.Add(t);
            var t2 = new TextBox();
            t2.Size = new Size(70, 15);
            t2.Location = new Point(10, 100);
            t2.Text = "0";
            f.Controls.Add(t2);
            var t3 = new TextBox();
            t3.Size = new Size(70, 15);
            t3.Location = new Point(10, 140);
            t3.Text = "0";
            f.Controls.Add(t3);
            var cb1 = new CheckBox();
            cb1.Size = new Size(70, 15);
            cb1.Location = new Point(10, 160);
            cb1.Text = "Copy Material";
            f.Controls.Add(cb1);
            var cb2 = new CheckBox();
            cb2.Size = new Size(150, 15);
            cb2.Location = new Point(10, 180);
            cb2.Text = "Copy Bones";
            f.Controls.Add(cb2);
            var cb3 = new CheckBox();
            cb3.Size = new Size(150, 15);
            cb3.Location = new Point(10, 200);
            cb3.Text = "Copy Dummy";
            f.Controls.Add(cb3);
            var cb4 = new CheckBox();
            cb4.Size = new Size(350, 15);
            cb4.Location = new Point(10, 220);
            cb4.Text = "All vertex weight to first bone";
            f.Controls.Add(cb4);
            f.ShowDialog();
            float x = float.Parse(t.Text);
            float y = float.Parse(t2.Text);
            float z = float.Parse(t3.Text);
            b.Meshes = src.Meshes;
            if (cb1.Checked)
                b.Materials = src.Materials;
            if (cb2.Checked)
                b.Bones = src.Bones;
            if (cb3.Checked)
                b.Dummies = src.Dummies;
            if (cb4.Checked)
            {
                for (var i = 0; i < b.Meshes.Count; i++)
                {
                    b.Meshes[i].BoneIndices = new List<int>();
                    b.Meshes[i].BoneIndices.Add(0);
                    b.Meshes[i].BoneIndices.Add(1);
                    b.Meshes[i].DefaultBoneIndex = 1;
                    foreach (FLVER.Vertex v in b.Meshes[i].Vertices)
                    {
                        for (var j = 0; j < v.Positions.Count; j++)
                        {
                            if (v.BoneWeights == null)
                            {
                                continue;
                            }
                            v.Positions[j] = new System.Numerics.Vector3(0, 0, 0);
                            for (var k = 0; k < v.BoneWeights.Length; k++)
                            {
                                v.BoneWeights[k] = 0;
                                v.BoneIndices[k] = 0;
                            }
                            v.BoneIndices[0] = 1;
                            v.BoneWeights[0] = 1;
                        }
                    }
                    //flver.Meshes[i].Vertices = new List<FLVER.Vertex>();
                }
            }
            foreach (FLVER.Mesh m in b.Meshes)
            {
                foreach (FLVER.Vertex v in m.Vertices)
                {
                    for (var i = 0; i < v.Positions.Count; i++)
                    {
                        v.Positions[i] = new System.Numerics.Vector3(v.Positions[i].X + x, v.Positions[i].Y + y, v.Positions[i].Z + z);
                    }
                }
            }
            b.Write(openFileDialog1.FileName + "n");
            MessageBox.Show("Swap completed!", "Info");
            //Console.WriteLine("End reading");
            //Application.Exit();
        }

        private static FaceVertex[] getVertices(Face f)
        {
            var ans = new FaceVertex[f.Count];
            for (var i = 0; i < f.Count; i++)
            {
                ans[i] = f[i];
            }
            return ans;
        }

        public static void exportJson(string content, string fileName = "export.json", string endMessage = "")
        {
            var openFileDialog2 = new SaveFileDialog();
            openFileDialog2.FileName = fileName;
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var sw = new StreamWriter(openFileDialog2.FileName);
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

        private class VertexNormalList
        {
            public readonly List<Vector3D> normals = new List<Vector3D>();

            public Vector3D calculateAvgNormal()
            {
                var ans = new Vector3D();
                foreach (Vector3D n in normals)
                {
                    ans = ans + n;
                }
                return ans.normalize();
            }

            public void add(Vector3D a)
            {
                normals.Add(a);
            }
        }
    }
}