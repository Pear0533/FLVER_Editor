using Assimp;
using System;
using System.Linq;

namespace SoulsAssetPipeline
{
    public static class AssimpUtilities
    {
        public static bool FindChildNodeAndApplyItsTransform(Node parentNode, Func<string, bool> chooseNode, ref Matrix4x4 absoluteTransform, out Node foundNode)
        {
            var n = parentNode.Children.FirstOrDefault(nd => chooseNode.Invoke(nd.Name));

            if (n != null)
            {
                absoluteTransform *= n.Transform;
            }

            foundNode = n;
            return foundNode != null;
        }

        public static Matrix4x4 GetSceneCoordSystemMatrix(Scene scene)
        {
            var upAxis = Convert.ToInt32(scene.Metadata["UpAxis"].Data);
            var frontAxis = Convert.ToInt32(scene.Metadata["FrontAxis"].Data);
            var coordAxis = Convert.ToInt32(scene.Metadata["CoordAxis"].Data);
            var upAxisSign = Convert.ToInt32(scene.Metadata["UpAxisSign"].Data);
            var frontAxisSign = Convert.ToInt32(scene.Metadata["FrontAxisSign"].Data);
            var coordAxisSign = Convert.ToInt32(scene.Metadata["CoordAxisSign"].Data);

            Vector3D upVec = upAxis == 0 ? new Vector3D(upAxisSign, 0, 0) : upAxis == 1 ? new Vector3D(0, upAxisSign, 0) : new Vector3D(0, 0, upAxisSign);
            Vector3D forwardVec = frontAxis == 0 ? new Vector3D(frontAxisSign, 0, 0) : frontAxis == 1 ? new Vector3D(0, frontAxisSign, 0) : new Vector3D(0, 0, frontAxisSign);
            Vector3D rightVec = coordAxis == 0 ? new Vector3D(coordAxisSign, 0, 0) : coordAxis == 1 ? new Vector3D(0, coordAxisSign, 0) : new Vector3D(0, 0, coordAxisSign);
            Matrix4x4 finalSceneMatrix = new Matrix4x4(rightVec.X, rightVec.Y, rightVec.Z, 0.0f,
               upVec.X, upVec.Y, upVec.Z, 0.0f,
               forwardVec.X, forwardVec.Y, forwardVec.Z, 0.0f,
               0.0f, 0.0f, 0.0f, 1.0f);

            return finalSceneMatrix;
        }

        public static Node FindRootNode(Scene scene, string rootNodeName, out Matrix4x4 absoluteMatrixOfRootNode)
        {
            var rootNode = scene.RootNode;
            var transform = rootNode.Transform;

            const string assimpTransformNodeText = "_$AssimpFbx$_";

            while (rootNode != null)
            {
                FindChildNodeAndApplyItsTransform(rootNode,
                nd => nd.StartsWith($"{rootNodeName}{assimpTransformNodeText}") || nd == rootNodeName,
                ref transform, out Node selectedNode);
                
                if (selectedNode == null)
                {
                    absoluteMatrixOfRootNode = transform;
                    return rootNode;
                }
                else
                {
                    rootNode = selectedNode;
                }
                
            }

            //if (FindChildNodeAndApplyItsTransform(rootNode, 
            //    $"{rootNodeName}_$AssimpFbx$_PreRotation", 
            //    ref transform, out rootNode))
            //{
            //    if (FindChildNodeAndApplyItsTransform(rootNode,
            //        $"{rootNodeName}_$AssimpFbx$_Rotation",
            //        ref transform, out rootNode))
            //    {
            //        if (FindChildNodeAndApplyItsTransform(rootNode,
            //            $"{rootNodeName}",
            //            ref transform, out rootNode))
            //        {
            //            absoluteMatrixOfRootNode = transform;
            //            return rootNode;
            //        }
            //    }
            //}
            //else if (FindChildNodeAndApplyItsTransform(scene.RootNode,
            //            $"{rootNodeName}",
            //            ref transform, out rootNode))
            //{
            //    absoluteMatrixOfRootNode = transform;
            //    return rootNode;
            //}

            absoluteMatrixOfRootNode = Matrix4x4.Identity;
            return null;

        }
    }
}
