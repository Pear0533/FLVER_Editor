using Assimp;
using SoulsAssetPipeline.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NMatrix = System.Numerics.Matrix4x4;
using NVector3 = System.Numerics.Vector3;
using NQuaternion = System.Numerics.Quaternion;

namespace SoulsAssetPipeline.AnimationExporting
{
    public static class AnimationExporter
    {
        public static Scene ExportToScene(HavokAnimationData anim)
        {
            var scene = new Scene();

            scene.RootNode = new Node("scene_root");

            scene.RootNode.Metadata["FrameRate"] = new Metadata.Entry(MetaDataType.Int32, 14);
            scene.RootNode.Metadata["TimeSpanStart"] = new Metadata.Entry(MetaDataType.UInt64, (ulong)0);
            scene.RootNode.Metadata["TimeSpanStop"] = new Metadata.Entry(MetaDataType.UInt64, (ulong)0);
            scene.RootNode.Metadata["CustomFrameRate"] = new Metadata.Entry(MetaDataType.Float, 1f / anim.FrameDuration);

            scene.RootNode.Metadata["FrontAxisSign"] = new Metadata.Entry(MetaDataType.Int32, -1);
            scene.RootNode.Metadata["OriginalUnitScaleFactor"] = new Metadata.Entry(MetaDataType.Int32, 100);
            scene.RootNode.Metadata["UnitScaleFactor"] = new Metadata.Entry(MetaDataType.Int32, 100);



            var a = new Assimp.Animation();
            a.DurationInTicks = anim.Duration * 30;
            a.TicksPerSecond = 30;
            a.Name = anim.Name;

            List<Node> animTrackNodes = new List<Node>();

            for (int i = 0; i < anim.TransformTrackIndexToHkxBoneMap.Length; i++)
            {
                int boneIndex = anim.TransformTrackIndexToHkxBoneMap[i];
                if (boneIndex < 0 || boneIndex > anim.hkaSkeleton.Bones.Capacity)
                    continue;
                string trackName = anim.hkaSkeleton.Bones[boneIndex].Name.GetString();
                var animTrack = new NodeAnimationChannel();
                animTrack.NodeName = trackName;
                for (int f = 0; f < anim.FrameCount; f++)
                {
                    var t = anim.GetTransformOnFrame(i, f, enableLooping: false);
                    animTrack.PositionKeys.Add(new VectorKey(1.0 * f * anim.FrameDuration, new Vector3D(t.Translation.X * -100, t.Translation.Y * 100, t.Translation.Z * 100)));
                    animTrack.ScalingKeys.Add(new VectorKey(1.0 * f * anim.FrameDuration, new Vector3D(t.Scale.X, t.Scale.Y, t.Scale.Z)));
                    var q = t.Rotation;

                    //q.X *= -1;
                    //q.Y *= -1;
                    //q.Z *= -1;
                    //q.W *= -1;
                    q = SapMath.MirrorQuat(q);
                    //q.X *= -1;
                    //q.Y *= -1;
                    //q.Z *= -1;
                    //q.W *= -1;
                    animTrack.RotationKeys.Add(new QuaternionKey(1.0 * f * anim.FrameDuration, new Quaternion(q.W, q.X, q.Y, q.Z)));
                }

                

                a.NodeAnimationChannels.Add(animTrack);

                var fakeNode = new Node(trackName);
                animTrackNodes.Add(fakeNode);
            }

            List<Node> topLevelTrackNodes = new List<Node>();

            for (int i = 0; i < animTrackNodes.Count; i++)
            {
                var hkxBoneIndex = anim.TransformTrackIndexToHkxBoneMap[i];
                var parentBoneIndex = anim.hkaSkeleton.ParentIndices[hkxBoneIndex].data;
                if (parentBoneIndex >= 0)
                {
                    var parentTrackIndex = anim.HkxBoneIndexToTransformTrackMap[parentBoneIndex];
                    if (parentTrackIndex >= 0)
                    {
                        animTrackNodes[parentTrackIndex].Children.Add(animTrackNodes[i]);
                    }
                }
                else
                {
                    topLevelTrackNodes.Add(animTrackNodes[i]);
                }
            }

            var actualRootNode = new Node("root");

            if (anim.RootMotion != null)
            {
                var animTrack = new NodeAnimationChannel();
                animTrack.NodeName = actualRootNode.Name;

                for (int f = 0; f < anim.FrameCount; f++)
                {
                    var rootMotionOnFrame = anim.RootMotion.GetSampleClamped(f * anim.FrameDuration);

                    animTrack.PositionKeys.Add(new VectorKey(1.0 * f * anim.FrameDuration, new Vector3D(rootMotionOnFrame.X * -100, rootMotionOnFrame.Y * 100, rootMotionOnFrame.Z * 100)));

                    var q = NQuaternion.CreateFromRotationMatrix(NMatrix.CreateRotationY(rootMotionOnFrame.W));
                    animTrack.RotationKeys.Add(new QuaternionKey(1.0 * f * anim.FrameDuration, new Quaternion(q.W, q.X, q.Y, q.Z)));


                }

                a.NodeAnimationChannels.Add(animTrack);
            }

            foreach (var t in topLevelTrackNodes)
            {
                actualRootNode.Children.Add(t);
            }

            scene.RootNode.Children.Add(actualRootNode);

            scene.Animations.Add(a);

            return scene;
        }

        public static void ExportToFile(HavokAnimationData anim, string filePath, string assimpFileFormatStr)
        {
            var scene = ExportToScene(anim);
            using (var x = new AssimpContext())
            {
                
                x.ExportFile(scene, filePath, assimpFileFormatStr);
            }
        }
    }
}
