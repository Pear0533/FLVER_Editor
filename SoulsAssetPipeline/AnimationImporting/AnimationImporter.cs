using Assimp;
using SoulsAssetPipeline.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NMatrix = System.Numerics.Matrix4x4;
using NVector3 = System.Numerics.Vector3;
using NVector4 = System.Numerics.Vector4;
using NQuaternion = System.Numerics.Quaternion;

namespace SoulsAssetPipeline.AnimationImporting
{
    public static class AnimationImporter
    {
        public class AnimationImportSettings
        {
            public float SceneScale = 1.0f;
            public float RootMotionScaleOverride = 1.0f;
            public bool UseRootMotionScaleOverride = false;
            public bool ConvertFromZUp = false; // Not needed for anims? need to see if it's just my test fbx
            public Dictionary<string, NewBlendableTransform> ExistingBoneDefaults = null;
            public HavokAnimationData ExistingHavokAnimationTemplate = null;
            public double ResampleToFramerate = 60;
            public string RootMotionNodeName = "root";
            public bool EnableRotationalRootMotion = true;
            public bool ExcludeRootMotionNodeFromTransformTracks = true;
            public bool FlipQuaternionHandedness = false;

            public bool NegateQuaternionX = true;
            public bool NegateQuaternionY = true;
            public bool NegateQuaternionZ = true;
            public bool NegateQuaternionW = false;

            public List<string> BonesToFlipBackwardsAboutYAxis = new List<string>();

            public bool InitalizeUnanimatedTracksToTPose = false;
        }

        public static ImportedAnimation ImportFBX(string fbxPath, AnimationImportSettings settings)
        {
            using (var context = new AssimpContext())
            {
                var fbx = context.ImportFile(fbxPath, PostProcessSteps.CalculateTangentSpace | PostProcessSteps.GlobalScale | PostProcessSteps.OptimizeGraph);
                return ImportFromAssimpScene(fbx, settings);
            }
        }

        public static ImportedAnimation ImportFromAssimpScene(Scene scene, 
            AnimationImportSettings settings)
        {
            ImportedAnimation result = new ImportedAnimation();

            var sceneMatrix = NMatrix.Identity;

            if (!settings.FlipQuaternionHandedness)
                sceneMatrix *= NMatrix.CreateScale(-1, 1, 1);

            if (settings.ExistingHavokAnimationTemplate == null)
            {
                throw new NotImplementedException("Reading skeleton/binding from assimp scene not supported yet. Please import using existing havok animation as template.");
            }
            else
            {
                result.hkaSkeleton = settings.ExistingHavokAnimationTemplate.hkaSkeleton;
                result.HkxBoneIndexToTransformTrackMap = settings.ExistingHavokAnimationTemplate.HkxBoneIndexToTransformTrackMap;
                result.TransformTrackIndexToHkxBoneMap = settings.ExistingHavokAnimationTemplate.TransformTrackIndexToHkxBoneMap;
            }

            

            if (settings.ConvertFromZUp)
            {
                sceneMatrix *= NMatrix.CreateRotationZ((float)(Math.PI));
                sceneMatrix *= NMatrix.CreateRotationX((float)(-Math.PI / 2.0));
            }

            var sceneMatrix_ForRootMotion = NMatrix.CreateScale(NVector3.One * settings.SceneScale) * sceneMatrix;

            if (settings.UseRootMotionScaleOverride)
            {
                sceneMatrix_ForRootMotion = NMatrix.CreateScale(NVector3.One * settings.RootMotionScaleOverride) * sceneMatrix;
            }

            sceneMatrix = NMatrix.CreateScale(NVector3.One * settings.SceneScale) * sceneMatrix;

            


            foreach (var anim in scene.Animations)
            {
                

                if (anim.HasNodeAnimations)
                {
                    // Setup framerate.
                    double tickScaler = (settings.ResampleToFramerate / anim.TicksPerSecond);

                    result.Duration = anim.DurationInTicks != 0 ? // Don't divide by 0
                        (float)(anim.DurationInTicks / anim.TicksPerSecond) : 0;
                    result.FrameDuration = (float)(1 / settings.ResampleToFramerate);

                    //result.Duration += result.FrameDuration;
                    int frameCount = (int)Math.Round(result.Duration / result.FrameDuration);

                    double resampleTickMult = settings.ResampleToFramerate / anim.TicksPerSecond;

                    Dictionary<string, int> transformTrackIndexMapping
                        = new Dictionary<string, int>();

                    List<string> transformTrackNames = new List<string>();

                    // Populate transform track names.
                    foreach (var nodeChannel in anim.NodeAnimationChannels)
                    {
                        if (nodeChannel.NodeName == settings.RootMotionNodeName && settings.ExcludeRootMotionNodeFromTransformTracks)
                            continue;

                        transformTrackNames.Add(nodeChannel.NodeName);
                    }

                    result.TransformTrackToBoneIndices.Clear();

                    if (settings.ExistingBoneDefaults != null)
                    {
                        

                        var boneNamesInExistingSkel = settings.ExistingBoneDefaults.Keys.ToList();

                        transformTrackNames = boneNamesInExistingSkel;

                        foreach (var tt in transformTrackNames)
                        {
                            result.TransformTrackToBoneIndices.Add(tt, boneNamesInExistingSkel.IndexOf(tt));
                        }

                        
                    }
                    else
                    {
                        int i = 0;
                        foreach (var t in transformTrackNames)
                        {
                            result.TransformTrackToBoneIndices.Add(t, i++);
                        }
                    }

                    // Populate transform track names.
                    foreach (var nodeChannel in anim.NodeAnimationChannels)
                    {
                        //if (nodeChannel.NodeName == settings.RootMotionNodeName && settings.ExcludeRootMotionNodeFromTransformTracks)
                        //    continue;

                        transformTrackIndexMapping.Add(nodeChannel.NodeName, transformTrackNames.IndexOf(nodeChannel.NodeName));
                    }

                    result.TransformTrackNames = transformTrackNames;

                    result.Frames = new List<ImportedAnimation.Frame>();

                    for (int i = 0; i <= frameCount; i++)
                    {
                        var f = new ImportedAnimation.Frame();
                        for (int j = 0; j < transformTrackNames.Count; j++)
                        {
                            if (settings.ExistingBoneDefaults != null && settings.ExistingBoneDefaults.ContainsKey(transformTrackNames[j]) && settings.InitalizeUnanimatedTracksToTPose)
                            {
                                f.BoneTransforms.Add(settings.ExistingBoneDefaults[transformTrackNames[j]]);
                            }
                            else
                            {
                                f.BoneTransforms.Add(NewBlendableTransform.Identity);
                            }
                        }
                        result.Frames.Add(f);
                    }

                    var rootMotionRotationFrames = new NQuaternion[frameCount + 1];

                    //DEBUGGING
                    var DEBUG_ALL_NODE_NAMES_SORTED = anim.NodeAnimationChannels.Select(n => n.NodeName).OrderBy(n => n).ToList();

                    for (int i = 0; i < anim.NodeAnimationChannelCount; i++)
                    {
                        var nodeChannel = anim.NodeAnimationChannels[i];

                        int lastKeyIndex = -1;

                        bool hasPosition = nodeChannel.HasPositionKeys;
                        bool hasRotation = nodeChannel.HasRotationKeys;
                        bool hasScale = nodeChannel.HasScalingKeys;

                        if (nodeChannel.NodeName.Contains("$AssimpFbx$_Translation"))
                        {
                            hasPosition = true;
                            hasRotation = false;
                            hasScale = false;
                        }
                        else if (nodeChannel.NodeName.Contains("$AssimpFbx$_Rotation"))
                        {
                            hasPosition = false;
                            hasRotation = true;
                            hasScale = false;
                        }
                        else if (nodeChannel.NodeName.Contains("$AssimpFbx$_Scaling"))
                        {
                            hasPosition = false;
                            hasRotation = false;
                            hasScale = true;
                        }


                        bool isRootMotionNode = nodeChannel.NodeName == settings.RootMotionNodeName || (nodeChannel.NodeName.StartsWith(settings.RootMotionNodeName) && nodeChannel.NodeName.Contains("_$AssimpFbx$_"));
                        if (isRootMotionNode)
                        {
                            if (hasPosition)
                            {
                                lastKeyIndex = -1;
                                foreach (var keyPos in nodeChannel.PositionKeys)
                                {
                                    int frame = (int)Math.Floor(keyPos.Time * resampleTickMult);
                                    result.Frames[frame].RootMotionTranslation =
                                        NVector3.Transform(keyPos.Value.ToNumerics(), sceneMatrix_ForRootMotion);

                                    //if (settings.FlipQuaternionHandedness)
                                    //{
                                    //    result.Frames[frame].RootMotionTranslation.X *= -1;
                                    //}

                                    // Fill in from the last keyframe to this one
                                    for (int f = lastKeyIndex + 1; f <= Math.Min(frame - 1, result.Frames.Count - 1); f++)
                                    {
                                        float lerpS = 1f * (f - lastKeyIndex) / (frame - lastKeyIndex);
                                        var blendFrom = result.Frames[lastKeyIndex].RootMotionTranslation;
                                        var blendTo = result.Frames[frame].RootMotionTranslation;

                                        result.Frames[f].RootMotionTranslation = NVector3.Lerp(blendFrom, blendTo, lerpS);
                                    }
                                    lastKeyIndex = frame;
                                }
                                // Fill in from last key to end of animation.
                                for (int f = lastKeyIndex + 1; f <= result.Frames.Count - 1; f++)
                                {
                                    result.Frames[f].RootMotionTranslation = result.Frames[lastKeyIndex].RootMotionTranslation;
                                }
                            }


                            if (hasRotation && settings.EnableRotationalRootMotion)
                            {
                                lastKeyIndex = -1;

                                foreach (var keyPos in nodeChannel.RotationKeys)
                                {
                                    int frame = (int)Math.Floor(keyPos.Time * resampleTickMult);

                                    var curFrameRotation = keyPos.Value.ToNumerics();
                                    curFrameRotation.Y *= -1;
                                    curFrameRotation.Z *= -1;

                                    if (settings.FlipQuaternionHandedness)
                                    {
                                        curFrameRotation = SapMath.MirrorQuat(curFrameRotation);
                                    }

                                    if (frame >= 0 && frame < frameCount)
                                        rootMotionRotationFrames[frame] = curFrameRotation;

                                    // Fill in from the last keyframe to this one
                                    for (int f = lastKeyIndex + 1; f <= Math.Min(frame - 1, result.Frames.Count - 1); f++)
                                    {
                                        float lerpS = 1f * (f - lastKeyIndex) / (frame - lastKeyIndex);
                                        var blendFrom = rootMotionRotationFrames[lastKeyIndex];
                                        var blendTo = curFrameRotation;

                                        var blended = NQuaternion.Slerp(blendFrom, blendTo, lerpS);
                                        //blended = NQuaternion.Normalize(blended);

                                        rootMotionRotationFrames[f] = blended;
                                    }
                                    lastKeyIndex = frame;
                                }
                                // Fill in from last key to end of animation.
                                for (int f = lastKeyIndex + 1; f <= result.Frames.Count - 1; f++)
                                {
                                    rootMotionRotationFrames[f] = rootMotionRotationFrames[lastKeyIndex];
                                }
                            }
                        }

                        if (isRootMotionNode)
                        {
                            hasPosition = false;
                            hasRotation = !settings.EnableRotationalRootMotion;
                        }

                        if (!(isRootMotionNode && !settings.ExcludeRootMotionNodeFromTransformTracks))
                        {
                            string nodeName = nodeChannel.NodeName;

                            int transformIndex = transformTrackIndexMapping[nodeName];

                            int memeIndex = nodeName.IndexOf("_$AssimpFbx$_");
                            if (memeIndex >= 0)
                            {
                                nodeName = nodeName.Substring(0, memeIndex);
                            }

                            

                            if (transformIndex >= 0 && transformIndex < transformTrackNames.Count)
                            {

                                // TRANSLATION
                                if (hasPosition)
                                {
                                    lastKeyIndex = -1;
                                    foreach (var keyPos in nodeChannel.PositionKeys)
                                    {
                                        int frame = (int)Math.Floor(keyPos.Time * resampleTickMult);

                                        var curFrameTransform = result.Frames[frame].BoneTransforms[transformIndex];
                                        curFrameTransform.Translation = NVector3.Transform(keyPos.Value.ToNumerics(), sceneMatrix);
                                        result.Frames[frame].BoneTransforms[transformIndex] = curFrameTransform;

                                        // Fill in from the last keyframe to this one
                                        for (int f = lastKeyIndex + 1; f <= Math.Min(frame - 1, result.Frames.Count - 1); f++)
                                        {
                                            float lerpS = 1f * (f - lastKeyIndex) / (frame - lastKeyIndex);
                                            var blendFrom = result.Frames[lastKeyIndex].BoneTransforms[transformIndex].Translation;
                                            var blendTo = curFrameTransform.Translation;

                                            var blended = NVector3.Lerp(blendFrom, blendTo, lerpS);

                                            var copyOfStruct = result.Frames[f].BoneTransforms[transformIndex];
                                            copyOfStruct.Translation = blended;
                                            result.Frames[f].BoneTransforms[transformIndex] = copyOfStruct;
                                        }
                                        lastKeyIndex = frame;
                                    }
                                    // Fill in from last key to end of animation.
                                    for (int f = lastKeyIndex + 1; f <= result.Frames.Count - 1; f++)
                                    {
                                        var x = result.Frames[f].BoneTransforms[transformIndex];
                                        x.Translation = result.Frames[lastKeyIndex].BoneTransforms[transformIndex].Translation;
                                        result.Frames[f].BoneTransforms[transformIndex] = x;
                                    }
                                }



                                // SCALE
                                if (hasScale)
                                {
                                    lastKeyIndex = -1;
                                    foreach (var keyPos in nodeChannel.ScalingKeys)
                                    {
                                        int frame = (int)Math.Floor(keyPos.Time * resampleTickMult);

                                        var curFrameTransform = result.Frames[frame].BoneTransforms[transformIndex];
                                        curFrameTransform.Scale = keyPos.Value.ToNumerics();
                                        result.Frames[frame].BoneTransforms[transformIndex] = curFrameTransform;

                                        // Fill in from the last keyframe to this one
                                        for (int f = lastKeyIndex + 1; f <= Math.Min(frame - 1, result.Frames.Count - 1); f++)
                                        {
                                            float lerpS = 1f * (f - lastKeyIndex) / (frame - lastKeyIndex);
                                            var blendFrom = result.Frames[lastKeyIndex].BoneTransforms[transformIndex].Scale;
                                            var blendTo = curFrameTransform.Scale;

                                            var blended = NVector3.Lerp(blendFrom, blendTo, lerpS);

                                            var copyOfStruct = result.Frames[f].BoneTransforms[transformIndex];
                                            copyOfStruct.Scale = blended;
                                            result.Frames[f].BoneTransforms[transformIndex] = copyOfStruct;
                                        }
                                        lastKeyIndex = frame;
                                    }
                                    // Fill in from last key to end of animation.
                                    for (int f = lastKeyIndex + 1; f <= result.Frames.Count - 1; f++)
                                    {
                                        var x = result.Frames[f].BoneTransforms[transformIndex];
                                        x.Scale = result.Frames[lastKeyIndex].BoneTransforms[transformIndex].Scale;
                                        result.Frames[f].BoneTransforms[transformIndex] = x;
                                    }
                                }

                                // ROTATION
                                if (hasRotation)
                                {
                                    lastKeyIndex = -1;

                                    foreach (var keyPos in nodeChannel.RotationKeys)
                                    {
                                        int frame = (int)Math.Floor(keyPos.Time * resampleTickMult);

                                        var curFrameTransform = result.Frames[frame].BoneTransforms[transformIndex];
                                        curFrameTransform.Rotation = keyPos.Value.ToNumerics();
                                        curFrameTransform.Rotation.Y *= -1;
                                        curFrameTransform.Rotation.Z *= -1;

                                        if (settings.FlipQuaternionHandedness)
                                        {
                                            curFrameTransform.Rotation = SapMath.MirrorQuat(curFrameTransform.Rotation);
                                        }

                                        result.Frames[frame].BoneTransforms[transformIndex] = curFrameTransform;

                                        // Fill in from the last keyframe to this one
                                        for (int f = lastKeyIndex + 1; f <= Math.Min(frame - 1, result.Frames.Count - 1); f++)
                                        {
                                            float lerpS = 1f * (f - lastKeyIndex) / (frame - lastKeyIndex);
                                            var blendFrom = result.Frames[lastKeyIndex].BoneTransforms[transformIndex].Rotation;
                                            var blendTo = curFrameTransform.Rotation;

                                            var blended = NQuaternion.Slerp(blendFrom, blendTo, lerpS);
                                            //blended = NQuaternion.Normalize(blended);

                                            var copyOfStruct = result.Frames[f].BoneTransforms[transformIndex];
                                            copyOfStruct.Rotation = blended;

                                            result.Frames[f].BoneTransforms[transformIndex] = copyOfStruct;
                                        }
                                        lastKeyIndex = frame;
                                    }
                                    // Fill in from last key to end of animation.
                                    for (int f = lastKeyIndex + 1; f <= result.Frames.Count - 1; f++)
                                    {
                                        var x = result.Frames[f].BoneTransforms[transformIndex];
                                        x.Rotation = result.Frames[lastKeyIndex].BoneTransforms[transformIndex].Rotation;
                                        result.Frames[f].BoneTransforms[transformIndex] = x;
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("unmapped transform track.");
                            }





                            
                        }

                       

                    }

                    if (settings.BonesToFlipBackwardsAboutYAxis.Count > 0)
                    {
                        var trackIndicesToFlip = result.TransformTrackNames
                                .Select((x, i) => i)
                                .Where(x => settings.BonesToFlipBackwardsAboutYAxis.Contains(result.TransformTrackNames[x]))
                                .ToList();

                        foreach (var f in result.Frames)
                        {
                            foreach (var i in trackIndicesToFlip)
                            {
                                var t = f.BoneTransforms[i];
                                t.Rotation = NQuaternion.CreateFromRotationMatrix(NMatrix.CreateFromQuaternion(f.BoneTransforms[i].Rotation) * NMatrix.CreateRotationY(SapMath.Pi));
                                t.Translation = NVector3.Transform(t.Translation, NMatrix.CreateRotationY(SapMath.Pi));
                                f.BoneTransforms[i] = t;
                            }
                        }
                    }

                    result.FrameCount = frameCount;

                    result.Name = anim.Name ?? settings.ExistingHavokAnimationTemplate?.Name ?? "SAP Custom Animation";


                    float rootMotionRot = 0;
                    for (int f = 0; f < result.Frames.Count; f++)
                    {
                        if (f > 0 && settings.EnableRotationalRootMotion)
                        {
                            var curMat = NMatrix.CreateFromQuaternion(rootMotionRotationFrames[f]);
                            var oldMat = NMatrix.CreateFromQuaternion(rootMotionRotationFrames[f - 1]);
                            if (NMatrix.Invert(oldMat, out NMatrix inverseOldMat))
                            {
                                var deltaMat = curMat * inverseOldMat;
                                var deltaVec = NVector3.Transform(NVector3.UnitX, deltaMat);
                                float deltaAngle = (float)Math.Atan2(deltaVec.Z, deltaVec.X);
                                rootMotionRot += deltaAngle;
                            }
                            

                        }
                        result.Frames[f].RootMotionRotation = rootMotionRot;
                    }

                    break;



                }
            }

            result.RootMotion = new RootMotionData(
                new NVector4(0, 1, 0, 0),
                new NVector4(0, 0, 1, 0),
                result.Duration, result.Frames.Select(f => f.RootMotion).ToArray());
            
            // Copy first frame for loop?
            //for (int i = 0; i < result.TransformTrackNames.Count; i++)
            //{
            //    result.Frames[result.Frames.Count - 1].BoneTransforms[i] = result.Frames[0].BoneTransforms[i];
            //}

            var rootMotionStart = result.Frames[0].RootMotion;

            for (int i = 0; i < result.Frames.Count; i++)
            {
                result.Frames[i].RootMotionTranslation.X -= rootMotionStart.X;
                result.Frames[i].RootMotionTranslation.Y -= rootMotionStart.Y;
                result.Frames[i].RootMotionTranslation.Z -= rootMotionStart.Z;
                result.Frames[i].RootMotionRotation -= rootMotionStart.W;

                var xyz = NVector3.Transform(result.Frames[i].RootMotion.XYZ(), NMatrix.CreateRotationY(-rootMotionStart.W));
                result.Frames[i].RootMotionTranslation.X = xyz.X;
                result.Frames[i].RootMotionTranslation.Y = xyz.Y;
                result.Frames[i].RootMotionTranslation.Z = xyz.Z;

                

                //for (int t = 0; t < result.Frames[i].BoneTransforms.Count; t++)
                //{
                //    if (i > 0 && NQuaternion.Dot(result.Frames[i - 1].BoneTransforms[t].Rotation, result.Frames[i].BoneTransforms[t].Rotation) < 0.995)
                //    {
                //        var tf = result.Frames[i].BoneTransforms[t];
                //        tf.Rotation = NQuaternion.Conjugate(result.Frames[i].BoneTransforms[t].Rotation);
                //        result.Frames[i].BoneTransforms[t] = tf;
                //    }
                //}
            }



            // HOTFIX FOR BAD FBX
            if (result.Frames.Count >= 3)
                result.Frames[result.Frames.Count - 1].RootMotionRotation = result.Frames[result.Frames.Count - 2].RootMotionRotation +
                    (result.Frames[result.Frames.Count - 2].RootMotionRotation - result.Frames[result.Frames.Count - 3].RootMotionRotation);





                //var endFrame = new ImportedAnimation.Frame();
                //foreach (var t in result.Frames[0].BoneTransforms)
                //{
                //    endFrame.BoneTransforms.Add(t);
                //}
                //endFrame.RootMotionTranslation = result.Frames[result.Frames.Count - 1].RootMotionTranslation +
                //    (result.Frames[result.Frames.Count - 1].RootMotionTranslation - result.Frames[result.Frames.Count - 2].RootMotionTranslation);

                //endFrame.RootMotionRotation = result.Frames[result.Frames.Count - 1].RootMotionRotation +
                //    (result.Frames[result.Frames.Count - 1].RootMotionRotation - result.Frames[result.Frames.Count - 2].RootMotionRotation);
                ////endFrame.RootMotionRotation = result.Frames[result.Frames.Count - 1].RootMotionRotation;

                //result.Frames.Add(endFrame);

            return result;
        }
    }
}
