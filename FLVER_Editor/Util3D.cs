using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ObjLoader.Loader.Data.Elements;
using SoulsFormats;

namespace FLVER_Editor
{
    internal class Util3D
    {
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
        /// Converts a numerics Vector3 to an xna Vector3
        /// </summary>
        /// <param name="vector">A numerics Vector3</param>
        /// <returns>An xna Vector3</returns>
        public static Microsoft.Xna.Framework.Vector3 NumericsVector3ToXnaVector3(System.Numerics.Vector3 vector)
        {
            return new Microsoft.Xna.Framework.Vector3(vector.X, vector.Y, vector.Z);
        }

        /// <summary>
        /// Converts a numerics Vector3 to an xna Vector3 while swapping Z and Y
        /// </summary>
        /// <param name="vector">A numerics Vector3</param>
        /// <returns>An xna Vector3 with Z and Y swapped</returns>
        public static Microsoft.Xna.Framework.Vector3 NumericsVector3ToXnaVector3XZY(System.Numerics.Vector3 vector)
        {
            return new Microsoft.Xna.Framework.Vector3(vector.X, vector.Z, vector.Y);
        }

        /// <summary>
        /// Converts a xna Vector3 to a numerics Vector3
        /// </summary>
        /// <param name="vector">An xna Vector3</param>
        /// <returns>A numerics Vector3</returns>
        public static System.Numerics.Vector3 XnaVector3ToNumericsVector3(Microsoft.Xna.Framework.Vector3 vector)
        {
            return new System.Numerics.Vector3(vector.X, vector.Y, vector.Z);
        }

        /// <summary>
        /// Converts an xna Vector3 to a numerics Vector3 while swapping Z and Y
        /// </summary>
        /// <param name="vector">An xna Vector3</param>
        /// <returns>A numerics Vector3 with Z and Y swapped</returns>
        public static System.Numerics.Vector3 XnaVector3ToNumericsVector3XZY(Microsoft.Xna.Framework.Vector3 vector)
        {
            return new System.Numerics.Vector3(vector.X, vector.Z, vector.Y);
        }

        /// <summary>
        /// Compute the final transform for a vector using the starting bone index and a list of all bones.
        /// </summary>
        /// <param name="bones">The list of bones to compute the transform from.</param>
        /// <param name="startIndex">The starting bone index the vector was binded to.</param>
        /// <returns>The final transform for a vector.</returns>
        public static System.Numerics.Matrix4x4 ComputeTransform(List<FLVER.Node> bones, int startIndex)
        {
            // For now this error checking will do, Im not sure if I should throw on this or not though
            if (bones.Count < 1)
                return new System.Numerics.Matrix4x4();

            if (startIndex == -1)
                startIndex = 0;

            var bone = bones[startIndex];
            var transform = bone.ComputeLocalTransform();
            while (bone.ParentIndex != -1)
            {
                bone = bones[bone.ParentIndex];
                transform = bone.ComputeLocalTransform() * transform;
            }

            return transform;
        }

        /// <summary>
        /// Computes the transform a vertex should have from its bone and that bone's Parent bones.
        /// </summary>
        /// <param name="model">A FLVER model.</param>
        /// <param name="mesh">A mesh from the FLVER model.</param>
        /// <param name="vertex">A vertex from the mesh of the FromSoftware model.</param>
        /// <returns>A transform for vertex from its bone and that bone's Parent bones.</returns>
        public static System.Numerics.Matrix4x4 ComputeTransform(FLVER2 model, FLVER2.Mesh mesh, FLVER.Vertex vertex)
        {
            if (mesh.Dynamic == 0)
            {
                int boneIndiceIndex = vertex.NormalW;

                var bone = model.Nodes[mesh.BoneIndices[boneIndiceIndex]];
                System.Numerics.Matrix4x4 transform = bone.ComputeLocalTransform();
                while (bone.ParentIndex != -1)
                {
                    bone = model.Nodes[bone.ParentIndex];
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
                    var bone = model.Nodes[mesh.BoneIndices[boneIndiceIndex]];
                    var boneTransform = bone.ComputeLocalTransform();
                    while (bone.ParentIndex != -1)
                    {
                        bone = model.Nodes[bone.ParentIndex];
                        boneTransform *= bone.ComputeLocalTransform();
                    }

                    transform *= boneTransform;
                }

                return transform;
            }
        }

        /// <summary>
        /// Find a bone index using its Name.
        /// </summary>
        /// <param name="flver">The FLVER containing the bone index to find.</param>
        /// <param name="name">The Name of the bone to get the index of.</param>
        /// <returns>A bone index if found, or -1 if not.</returns>
        public static int FindBoneIndexByName(FLVER2 flver, string name)
        {
            for (var i = 0; i < flver.Nodes.Count; ++i)
                if (flver.Nodes[i].Name == name)
                    return i;
            return -1;
        }

        /// <summary>
        /// Filters out invalid chars in path.
        /// </summary>
        /// <param name="path">A string representing the path to filter.</param>
        /// <returns>A path string with invalid chars filtered out.</returns>
        public static string FilterInvalidPathChars(string path)
        {
            return string.Concat(path.Split(Path.GetInvalidPathChars()));
        }

        /// <summary>
        /// Gets face vertices from an ObjLoader face.
        /// </summary>
        /// <param name="face">An ObjLoader face.</param>
        /// <returns>An ObjLoader face vertex array.</returns>
        public static FaceVertex[] GetObjFaceVertices(Face face)
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
