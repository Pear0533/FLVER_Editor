using Assimp;
using SoulsFormats;
using System;
using System.Collections.Generic;
using NMatrix = System.Numerics.Matrix4x4;
using NVector3 = System.Numerics.Vector3;
using NQuaternion = System.Numerics.Quaternion;

namespace SoulsAssetPipeline.FLVERImporting
{
    public static class FLVERImportHelpers
    {
        public static void UpdateBoundingBox(this FLVER2.FLVERHeader header, NVector3 vertexPos)
        {
            var minX = Math.Min(header.BoundingBoxMin.X, vertexPos.X);
            var minY = Math.Min(header.BoundingBoxMin.Y, vertexPos.Y);
            var minZ = Math.Min(header.BoundingBoxMin.Z, vertexPos.Z);
            var maxX = Math.Max(header.BoundingBoxMax.X, vertexPos.X);
            var maxY = Math.Max(header.BoundingBoxMax.Y, vertexPos.Y);
            var maxZ = Math.Max(header.BoundingBoxMax.Z, vertexPos.Z);
            header.BoundingBoxMin = new NVector3(minX, minY, minZ);
            header.BoundingBoxMax = new NVector3(maxX, maxY, maxZ);
        }

        public static void UpdateBoundingBox(this FLVER2.Mesh mesh, NVector3 vertexPos)
        {
            var minX = Math.Min(mesh.BoundingBox.Min.X, vertexPos.X);
            var minY = Math.Min(mesh.BoundingBox.Min.Y, vertexPos.Y);
            var minZ = Math.Min(mesh.BoundingBox.Min.Z, vertexPos.Z);
            var maxX = Math.Max(mesh.BoundingBox.Max.X, vertexPos.X);
            var maxY = Math.Max(mesh.BoundingBox.Max.Y, vertexPos.Y);
            var maxZ = Math.Max(mesh.BoundingBox.Max.Z, vertexPos.Z);
            mesh.BoundingBox.Min = new NVector3(minX, minY, minZ);
            mesh.BoundingBox.Max = new NVector3(maxX, maxY, maxZ);
        }

        public static void UpdateBoundingBox(this FLVER.Bone b, List<FLVER.Bone> bones, NVector3 vertexPos)
        {
            var boneAbsoluteMatrix = b.GetAbsoluteNMatrix(bones);
            
            if (NMatrix.Invert(boneAbsoluteMatrix, out NMatrix invertexBoneMat))
            {
                var posForBBox = NVector3.Transform(vertexPos, invertexBoneMat);

                var minX = Math.Min(b.BoundingBoxMin.X, posForBBox.X);
                var minY = Math.Min(b.BoundingBoxMin.Y, posForBBox.Y);
                var minZ = Math.Min(b.BoundingBoxMin.Z, posForBBox.Z);
                var maxX = Math.Max(b.BoundingBoxMax.X, posForBBox.X);
                var maxY = Math.Max(b.BoundingBoxMax.Y, posForBBox.Y);
                var maxZ = Math.Max(b.BoundingBoxMax.Z, posForBBox.Z);

                b.BoundingBoxMin = new NVector3(minX, minY, minZ);
                b.BoundingBoxMax = new NVector3(maxX, maxY, maxZ);
            }
            //ErrorTODO: when this fails, else {}
        }

        public static NMatrix GetNMatrix(this FLVER.Bone b)
        {
            return NMatrix.CreateScale(b.Scale) *
                NMatrix.CreateRotationX(b.Rotation.X) *
                NMatrix.CreateRotationZ(b.Rotation.Z) *
                NMatrix.CreateRotationY(b.Rotation.Y) *
                NMatrix.CreateTranslation(b.Translation);
        }

        public static FLVER.Bone GetParent(this FLVER.Bone b, List<FLVER.Bone> bones)
        {
            if (b.ParentIndex >= 0 && b.ParentIndex < bones.Count)
                return bones[b.ParentIndex];
            else
                return null;
        }

        public static NMatrix GetAbsoluteNMatrix(this FLVER.Bone b, List<FLVER.Bone> bones)
        {
            NMatrix result = NMatrix.Identity;
            var parentBone = b;
            while (parentBone != null)
            {
                var m = parentBone.GetNMatrix();
                result *= m;
                parentBone = parentBone.GetParent(bones);
            }
            return result;
        }

        public static void EnsureLayoutMembers(this FLVER.Vertex v, Dictionary<FLVER.LayoutSemantic, int> members)
        {
            foreach (var m in members)
            {
                if (m.Key == FLVER.LayoutSemantic.Tangent)
                {
                    while (v.Tangents.Count < m.Value)
                        v.Tangents.Add(v.Tangents.Count > 0 ? v.Tangents[0] : System.Numerics.Vector4.Zero);
                }
                else if (m.Key == FLVER.LayoutSemantic.VertexColor)
                {
                    while (v.Colors.Count < m.Value)
                        v.Colors.Add(v.Colors.Count > 0 ? v.Colors[0] : new FLVER.VertexColor(0, 0, 0, 0));
                }
                else if (m.Key == FLVER.LayoutSemantic.UV)
                {
                    while (v.UVs.Count <= m.Value)
                        v.UVs.Add(v.UVs.Count > 0 ? v.UVs[0] : NVector3.Zero);
                }
            }
        }


        public static NVector3 GetFlverBoneEulerFromQuaternion_TypeB(Quaternion quat)
        {
            //This is the code from
            //http://www.mawsoft.com/blog/?p=197
            var rotation = quat;
            double q0 = rotation.W;
            double q1 = rotation.Y;
            double q2 = rotation.X;
            double q3 = rotation.Z;

            NVector3 radAngles = new NVector3();
            radAngles.Y = (float)Math.Atan2(2 * (q0 * q1 + q2 * q3), 1 - 2 * (Math.Pow(q1, 2) + Math.Pow(q2, 2)));
            radAngles.X = (float)Math.Asin(2 * (q0 * q2 - q3 * q1));
            radAngles.Z = (float)Math.Atan2(2 * (q0 * q3 + q1 * q2), 1 - 2 * (Math.Pow(q2, 2) + Math.Pow(q3, 2)));

            return radAngles;
        }

        public static NVector3 GetFlverBoneEulerFromQuaternion(Quaternion q)
        {
            // Store the Euler angles in radians
            double yaw;
            double pitch;
            double roll;

            double sqw = q.W * q.W;
            double sqx = q.X * q.X;
            double sqy = q.Y * q.Y;
            double sqz = q.Z * q.Z;

            // If quaternion is normalised the unit is one, otherwise it is the correction factor
            double unit = sqx + sqy + sqz + sqw;
            double test = q.X * q.Y + q.Z * q.W;

            if (test > 0.4995 * unit) // 0.4999 OR 0.5 - EPSILON
            {
                // Singularity at north pole
                yaw = 2.0 * Math.Atan2(q.X, q.W);
                pitch = Math.PI * 0.5;
                roll = 0.0;
            }
            else if (test < -0.4995 * unit) // -0.4999 OR -0.5 + EPSILON
            {
                // Singularity at south pole
                yaw = -2.0 * Math.Atan2(q.X, q.W);
                pitch = -Math.PI * 0.5;
                roll = 0.0;
            }
            else
            {
                yaw = Math.Atan2(2.0 * q.Y * q.W - 2.0 * q.X * q.Z, sqx - sqy - sqz + sqw);
                pitch = Math.Asin(2.0 * test / unit);
                roll = Math.Atan2(2.0 * q.X * q.W - 2.0 * q.Y * q.Z, -sqx + sqy - sqz + sqw);
            }

            return new NVector3((float)pitch, (float)yaw, (float)roll);
        }


        public struct FLVERBoneTransform
        {
            public NVector3 Translation;
            public NVector3 Rotation;
            public NVector3 Scale;

            public static FLVERBoneTransform FromMatrix4x4(Matrix4x4 m, bool applyMemes)
            {
                var result = new FLVERBoneTransform();
                m.Decompose(out Vector3D s, out Quaternion rq, out Vector3D t);

                result.Translation = t.ToNumerics();
                if (applyMemes)
                    result.Translation.X = -result.Translation.X;

                result.Scale = s.ToNumerics();

                NMatrix mat = NMatrix.Identity;

                if (applyMemes)
                {
                    NQuaternion quat = SapMath.MirrorQuat(rq.ToNumerics());
                    mat = NMatrix.CreateFromQuaternion(quat);
                }
                else
                {
                    mat = NMatrix.CreateFromQuaternion(new NQuaternion(rq.X, rq.Y, rq.Z, rq.W));
                }

                result.Rotation = SapMath.MatrixToEulerXZY(mat);

                return result;
            }
        }

        public class FLVERMetaskeleton
        {
            public List<FLVER.Bone> Bones = new List<FLVER.Bone>();
            public List<FLVER.Dummy> DummyPoly = new List<FLVER.Dummy>();
        }

        public static FLVERMetaskeleton GenerateFlverMetaskeletonFromRootNode(
            Node rootNode, Matrix4x4 rootNodeAbsoluteMatrix, float importScale)
        {
            var bonesAssimp = new List<Node>();
            var skel = new FLVERMetaskeleton();
            var dummyAttachBoneNames = new List<string>();

            NMatrix matrixScale = NMatrix.CreateScale(importScale, importScale, importScale);

            // Returns index of bone in master bone list if boneNode is a bone.
            // Returns -1 if boneNode is a DummyPoly (denoted with a node name starting with "DUMMY_POLY").
            int AddBone(Node boneNode, Node parentBoneNode, Matrix4x4 parentAbsoluteMatrix)
            {
                short parentBoneIndex = (short)(bonesAssimp.IndexOf(parentBoneNode));

                var thisBoneMatrix = boneNode.Transform;
                var thisNodeAbsoluteMatrix = thisBoneMatrix * parentAbsoluteMatrix;
                

                var boneTrans = FLVERBoneTransform.FromMatrix4x4(
                    (parentBoneIndex == -1 ? thisNodeAbsoluteMatrix : thisBoneMatrix), true);

                if (boneNode.Name.StartsWith("DUMMY_POLY"))
                {
                    // TODO

                    thisNodeAbsoluteMatrix.Decompose(out Vector3D dummyScale, out Quaternion dummyQuat, out Vector3D dummyTranslation);
                    var dmy = new FLVER.Dummy();
                    dmy.ParentBoneIndex = parentBoneIndex;
                    dmy.Position = dummyTranslation.ToNumerics();

                    // Format: "DUMMY_POLY|<RefID>|<AttachBoneName>"
                    // Example: "DUMMY_POLY|220|Spine1"
                    string[] dummyNameParts = boneNode.Name.Split('|');

                    //ErrorTODO: TryParse
                    dmy.ReferenceID = short.Parse(dummyNameParts[1].Trim());

                    if (dummyNameParts.Length == 3)
                        dummyAttachBoneNames.Add(dummyNameParts[2]);
                    else
                        dummyAttachBoneNames.Add(null);

                    //NOTE: Maybe this should be specifiable? I forget what the point of false is here.
                    dmy.UseUpwardVector = true;

                    var sceneRotation = NMatrix.CreateRotationX(boneTrans.Rotation.X) *
                        NMatrix.CreateRotationZ(boneTrans.Rotation.Z) *
                        NMatrix.CreateRotationY(boneTrans.Rotation.Y);

                    dmy.Upward = NVector3.Transform(new NVector3(0, 1, 0), sceneRotation);
                    //TODO: Check if forward vector3 should be 1 or -1;
                    dmy.Forward = NVector3.Transform(new NVector3(0, 0, 1), sceneRotation);

                    skel.DummyPoly.Add(dmy);

                    return -1;
                }
                else
                {
                    bonesAssimp.Add(boneNode);

                    int thisBoneIndex = bonesAssimp.Count - 1;

                    var flverBone = new FLVER.Bone();

                    if (parentBoneNode != null)
                        flverBone.ParentIndex = parentBoneIndex;

                    flverBone.Name = boneNode.Name;
                    flverBone.BoundingBoxMin = new NVector3(float.MaxValue, float.MaxValue, float.MaxValue);
                    flverBone.BoundingBoxMax = new NVector3(float.MinValue, float.MinValue, float.MinValue);
                    flverBone.Translation = boneTrans.Translation * importScale;
                    flverBone.Rotation = boneTrans.Rotation;
                    flverBone.Scale = boneTrans.Scale;

                    skel.Bones.Add(flverBone);

                    List<int> childBoneIndices = new List<int>();

                    foreach (var c in boneNode.Children)
                    {
                        int cIndex = AddBone(c, boneNode, thisNodeAbsoluteMatrix);

                        //cIndex will be -1 if the child node was a DummyPoly instead of a bone.
                        if (cIndex >= 0)
                            childBoneIndices.Add(cIndex);
                    }

                    if (childBoneIndices.Count > 0)
                    {
                        flverBone.ChildIndex = (short)childBoneIndices[0];

                        for (int i = 0; i < childBoneIndices.Count; i++)
                        {
                            var thisChildBone = skel.Bones[childBoneIndices[i]];
                            if (i == 0)
                                thisChildBone.PreviousSiblingIndex = -1;
                            else
                                thisChildBone.PreviousSiblingIndex = (short)(childBoneIndices[i - 1]);

                            if (i == childBoneIndices.Count - 1)
                                thisChildBone.NextSiblingIndex = -1;
                            else
                                thisChildBone.NextSiblingIndex = (short)(childBoneIndices[i + 1]);
                        }
                    }

                    return thisBoneIndex;
                }

                
               
            }

            //if (rootNode.Children == null)
            //    throw new InvalidDataException("Assimp scene has no heirarchy.");

            var root = rootNode;
            //var master = root.Children[0];
            //foreach (var c in root.Children)
            //{
            //    AddBone(c, null, root.Transform * rootNodeAbsoluteMatrix);
            //}
            AddBone(root, null, rootNodeAbsoluteMatrix);
            // Apply parent bone transforms to DummyPoly
            foreach (var d in skel.DummyPoly)
            {
                if (d.ParentBoneIndex >= 0)
                {
                    var parentMat = skel.Bones[d.ParentBoneIndex].GetAbsoluteNMatrix(skel.Bones);
                    d.Position = NVector3.Transform(d.Position, parentMat);
                    d.Upward = NVector3.TransformNormal(d.Upward, parentMat);
                    d.Forward = NVector3.TransformNormal(d.Forward, parentMat);
                }
            }

            return skel;
        }

    }
}
