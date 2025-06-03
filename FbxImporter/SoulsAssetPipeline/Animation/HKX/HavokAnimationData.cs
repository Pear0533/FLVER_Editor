using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace SoulsAssetPipeline.Animation
{
    
    public abstract class HavokAnimationData
    {
        public string Name { get; internal set;  }

        public RootMotionData RootMotion { get; internal set; }

        public HKX.AnimationBlendHint BlendHint = HKX.AnimationBlendHint.NORMAL;

        public bool IsAdditiveBlend => BlendHint == HKX.AnimationBlendHint.ADDITIVE ||
            BlendHint == HKX.AnimationBlendHint.ADDITIVE_DEPRECATED;

        public HKX.HKASkeleton hkaSkeleton;

        public float Duration;
        public float FrameDuration;
        public int FrameCount;

        // Index into array = hkx bone index, result = transform track index.
        public int[] HkxBoneIndexToTransformTrackMap;

        public int[] TransformTrackIndexToHkxBoneMap;

        /// <summary>
        /// Gives dictionary of (index into this anims transform tracks, bone name they're mapped to).
        /// Any tracks not mapped to bones are excluded.
        /// </summary>
        public Dictionary<int, string> GetTransformTrackBoneNameMapping(IEnumerable<string> boneNameWhitelist = null)
        {
            var res = new Dictionary<int, string>();
            var bones = hkaSkeleton.Bones.GetArrayData().Elements;
            for (int i = 0; i < TransformTrackIndexToHkxBoneMap.Length; i++)
            {
                var b = TransformTrackIndexToHkxBoneMap[i];
                if (b >= 0 && b < bones.Count)
                {
                    var bn = bones[b].Name.GetString();

                    if (boneNameWhitelist == null || boneNameWhitelist.Contains(bn))
                        res.Add(i, bn);
                }
            }
            return res;
        }

        public HavokAnimationData(string Name, HKX.HKASkeleton skeleton, HKX.HKADefaultAnimatedReferenceFrame refFrame, HKX.HKAAnimationBinding binding)
        {
            this.Name = Name;

            hkaSkeleton = skeleton;

            if (refFrame != null)
            {
                RootMotion = new RootMotionData(refFrame);
            }

            BlendHint = binding?.BlendHint ?? HKX.AnimationBlendHint.NORMAL;
        }

        public NewBlendableTransform GetTransformOnFrameByBone(int hkxBoneIndex, float frame, bool enableLooping)
        {
            var track = HkxBoneIndexToTransformTrackMap[hkxBoneIndex];

            if (track == -1)
            {
                var skeleTransform = hkaSkeleton.Transforms[hkxBoneIndex];

                NewBlendableTransform defaultBoneTransformation = new NewBlendableTransform();

                defaultBoneTransformation.Scale.X = skeleTransform.Scale.Vector.X;
                defaultBoneTransformation.Scale.Y = skeleTransform.Scale.Vector.Y;
                defaultBoneTransformation.Scale.Z = skeleTransform.Scale.Vector.Z;

                defaultBoneTransformation.Rotation = new Quaternion(
                    skeleTransform.Rotation.Vector.X,
                    skeleTransform.Rotation.Vector.Y,
                    skeleTransform.Rotation.Vector.Z,
                    skeleTransform.Rotation.Vector.W);

                defaultBoneTransformation.Translation.X = skeleTransform.Position.Vector.X;
                defaultBoneTransformation.Translation.Y = skeleTransform.Position.Vector.Y;
                defaultBoneTransformation.Translation.Z = skeleTransform.Position.Vector.Z;

                return defaultBoneTransformation;
            }

            return GetTransformOnFrame(track, frame, enableLooping);
        }
        public abstract NewBlendableTransform GetTransformOnFrame(int transformIndex, float frame, bool enableLooping);

    }

    public class HavokAnimationData_SplineCompressed : HavokAnimationData
    {
        public HavokAnimationData_SplineCompressed(string name, HKX.HKASkeleton skeleton,
           HKX.HKADefaultAnimatedReferenceFrame refFrame, HKX.HKAAnimationBinding binding, HKX.HKASplineCompressedAnimation anim)
           : base(name, skeleton, refFrame, binding)
        {
            Duration = anim.Duration;// Math.Max(anim.Duration, anim.FrameDuration * anim.FrameCount);
            FrameCount = anim.FrameCount;

            FrameDuration = anim.FrameDuration;

            BlockCount = anim.BlockCount;
            NumFramesPerBlock = anim.FramesPerBlock;

            HkxBoneIndexToTransformTrackMap = new int[skeleton.Bones.Size];
            TransformTrackIndexToHkxBoneMap = new int[binding.TransformTrackToBoneIndices.Size];

            for (int i = 0; i < binding.TransformTrackToBoneIndices.Size; i++)
            {
                TransformTrackIndexToHkxBoneMap[i] = -1;
            }

            for (int i = 0; i < skeleton.Bones.Size; i++)
            {
                HkxBoneIndexToTransformTrackMap[i] = -1;
            }

            for (int i = 0; i < binding.TransformTrackToBoneIndices.Size; i++)
            {
                short boneIndex = binding.TransformTrackToBoneIndices[i].data;
                if (boneIndex >= 0)
                    HkxBoneIndexToTransformTrackMap[boneIndex] = i;
                TransformTrackIndexToHkxBoneMap[i] = boneIndex;
            }

            Tracks = SplineCompressedAnimation.ReadSplineCompressedAnimByteBlock(
                isBigEndian: false, anim.GetData(), anim.TransformTrackCount, anim.BlockCount);
        }

        public List<SplineCompressedAnimation.TransformTrack[]> Tracks;

        public int BlockCount = 1;
        public int NumFramesPerBlock = 255;

        public int GetBlock(float frame)
        {
            return (int)((frame % (FrameCount - 1)) / (NumFramesPerBlock - 1));
        }

        private NewBlendableTransform GetTransformOnSpecificBlockAndFrame(int transformIndex, int block, float frame, bool enableLooping)
        {
            while (frame < 0)
                frame += FrameCount;

            frame = (enableLooping ? (frame % (FrameCount - 1)) : (Math.Min(frame, FrameCount))) % (NumFramesPerBlock - 1);

            NewBlendableTransform result = NewBlendableTransform.Identity;
            var track = Tracks[block][transformIndex];
            var skeleTransform = hkaSkeleton.Transforms[TransformTrackIndexToHkxBoneMap[transformIndex]];

            //result.Scale.X = track.SplineScale?.ChannelX == null
            //    ? (IsAdditiveBlend ? 1 : track.StaticScale.X) : track.SplineScale.GetValueX(frame);
            //result.Scale.Y = track.SplineScale?.ChannelY == null
            //    ? (IsAdditiveBlend ? 1 : track.StaticScale.Y) : track.SplineScale.GetValueY(frame);
            //result.Scale.Z = track.SplineScale?.ChannelZ == null
            //    ? (IsAdditiveBlend ? 1 : track.StaticScale.Z) : track.SplineScale.GetValueZ(frame);

            if (track.SplineScale != null)
            {
                result.Scale.X = track.SplineScale.GetValueX(frame)
                    ?? (IsAdditiveBlend ? 1 : skeleTransform.Scale.Vector.X);

                result.Scale.Y = track.SplineScale.GetValueY(frame)
                    ?? (IsAdditiveBlend ? 1 : skeleTransform.Scale.Vector.Y);

                result.Scale.Z = track.SplineScale.GetValueZ(frame)
                    ?? (IsAdditiveBlend ? 1 : skeleTransform.Scale.Vector.Z);
            }
            else
            {
                if (track.Mask.ScaleTypes.Contains(SplineCompressedAnimation.FlagOffset.StaticX))
                    result.Scale.X = track.StaticScale.X;
                else
                    result.Scale.X = IsAdditiveBlend ? 1 : skeleTransform.Scale.Vector.X;

                if (track.Mask.ScaleTypes.Contains(SplineCompressedAnimation.FlagOffset.StaticY))
                    result.Scale.Y = track.StaticScale.Y;
                else
                    result.Scale.Y = IsAdditiveBlend ? 1 : skeleTransform.Scale.Vector.Y;

                if (track.Mask.ScaleTypes.Contains(SplineCompressedAnimation.FlagOffset.StaticZ))
                    result.Scale.Z = track.StaticScale.Z;
                else
                    result.Scale.Z = IsAdditiveBlend ? 1 : skeleTransform.Scale.Vector.Z;
            }

            //if (IsAdditiveBlend)
            //{
            //    result.Scale.X *= skeleTransform.Scale.Vector.X;
            //    result.Scale.Y *= skeleTransform.Scale.Vector.Y;
            //    result.Scale.Z *= skeleTransform.Scale.Vector.Z;
            //}

            //if (result.Scale.LengthSquared() > (Vector3.One * 1.1f).LengthSquared())
            //{
            //    Console.WriteLine(":fatoof:");
            //}

            if (track.SplineRotation != null)//track.HasSplineRotation)
            {
                result.Rotation = track.SplineRotation.GetValue(frame);
            }
            else if (track.HasStaticRotation)
            {
                // We actually need static rotation or Gael hands become unbent among others
                result.Rotation = track.StaticRotation;
            }
            else
            {
                result.Rotation = Quaternion.Identity;
                //result.Rotation = IsAdditiveBlend ? Quaternion.Identity : new Quaternion(
                //    skeleTransform.Rotation.Vector.X,
                //    skeleTransform.Rotation.Vector.Y,
                //    skeleTransform.Rotation.Vector.Z,
                //    skeleTransform.Rotation.Vector.W);
            }

            //if (IsAdditiveBlend)
            //{
            //    result.Rotation = new Quaternion(
            //        skeleTransform.Rotation.Vector.X,
            //        skeleTransform.Rotation.Vector.Y,
            //        skeleTransform.Rotation.Vector.Z,
            //        skeleTransform.Rotation.Vector.W) * result.Rotation;
            //}

            if (track.SplinePosition != null)
            {
                //result.Translation.X = track.SplinePosition.GetValueX(frame)
                //    ?? (IsAdditiveBlend ? 0 : skeleTransform.Position.Vector.X);

                //result.Translation.Y = track.SplinePosition.GetValueY(frame)
                //    ?? (IsAdditiveBlend ? 0 : skeleTransform.Position.Vector.Y);

                //result.Translation.Z = track.SplinePosition.GetValueZ(frame)
                //    ?? (IsAdditiveBlend ? 0 : skeleTransform.Position.Vector.Z);

                result.Translation.X = track.SplinePosition.GetValueX(frame) ?? 0;

                result.Translation.Y = track.SplinePosition.GetValueY(frame) ?? 0;

                result.Translation.Z = track.SplinePosition.GetValueZ(frame) ?? 0;
            }
            else
            {
                //if (track.Mask.PositionTypes.Contains(SplineCompressedAnimation.FlagOffset.StaticX))
                //    result.Translation.X = track.StaticPosition.X;
                //else
                //    result.Translation.X = IsAdditiveBlend ? 0 : skeleTransform.Position.Vector.X;

                //if (track.Mask.PositionTypes.Contains(SplineCompressedAnimation.FlagOffset.StaticY))
                //    result.Translation.Y = track.StaticPosition.Y;
                //else
                //    result.Translation.Y = IsAdditiveBlend ? 0 : skeleTransform.Position.Vector.Y;

                //if (track.Mask.PositionTypes.Contains(SplineCompressedAnimation.FlagOffset.StaticZ))
                //    result.Translation.Z = track.StaticPosition.Z;
                //else
                //    result.Translation.Z = IsAdditiveBlend ? 0 : skeleTransform.Position.Vector.Z;

                if (track.Mask.PositionTypes.Contains(SplineCompressedAnimation.FlagOffset.StaticX))
                    result.Translation.X = track.StaticPosition.X;
                else
                    result.Translation.X = 0;

                if (track.Mask.PositionTypes.Contains(SplineCompressedAnimation.FlagOffset.StaticY))
                    result.Translation.Y = track.StaticPosition.Y;
                else
                    result.Translation.Y = 0;

                if (track.Mask.PositionTypes.Contains(SplineCompressedAnimation.FlagOffset.StaticZ))
                    result.Translation.Z = track.StaticPosition.Z;
                else
                    result.Translation.Z = 0;
            }

            //result.Translation.X = track.SplinePosition?.GetValueX(frame) ?? (IsAdditiveBlend ? 0 : track.StaticPosition.X);
            //result.Translation.Y = track.SplinePosition?.GetValueY(frame) ?? (IsAdditiveBlend ? 0 : track.StaticPosition.Y);
            //result.Translation.Z = track.SplinePosition?.GetValueZ(frame) ?? (IsAdditiveBlend ? 0 : track.StaticPosition.Z);

            //if (!IsAdditiveBlend && (!track.Mask.PositionTypes.Contains(Havok.SplineCompressedAnimation.FlagOffset.SplineX) &&
            //    !track.Mask.PositionTypes.Contains(Havok.SplineCompressedAnimation.FlagOffset.StaticX)))
            //{
            //    result.Translation.X = skeleTransform.Position.Vector.X;
            //}

            //if (!IsAdditiveBlend && (!track.Mask.PositionTypes.Contains(Havok.SplineCompressedAnimation.FlagOffset.SplineY) &&
            //    !track.Mask.PositionTypes.Contains(Havok.SplineCompressedAnimation.FlagOffset.StaticY)))
            //{
            //    result.Translation.Y = skeleTransform.Position.Vector.Y;
            //}

            //if (!IsAdditiveBlend && (!track.Mask.PositionTypes.Contains(Havok.SplineCompressedAnimation.FlagOffset.SplineZ) &&
            //    !track.Mask.PositionTypes.Contains(Havok.SplineCompressedAnimation.FlagOffset.StaticZ)))
            //{
            //    result.Translation.Z = skeleTransform.Position.Vector.Z;
            //}

            //if (IsAdditiveBlend)
            //{
            //    result.Translation.X += skeleTransform.Position.Vector.X;
            //    result.Translation.Y += skeleTransform.Position.Vector.Y;
            //    result.Translation.Z += skeleTransform.Position.Vector.Z;
            //}

            return result;
        }

        public override NewBlendableTransform GetTransformOnFrame(int transformTrackIndex, float frame, bool enableLooping)
        {
            int blockIndex = GetBlock(frame);



            float frameInBlock = (enableLooping ? (frame % (FrameCount - 1)) : (Math.Min(frame, FrameCount))) % (NumFramesPerBlock - 1);

            NewBlendableTransform currentFrame = GetTransformOnSpecificBlockAndFrame(transformTrackIndex,
                    block: blockIndex, frame, enableLooping);
            return currentFrame;

            //if (frame >= FrameCount - 1)
            //{
            //    NewBlendableTransform currentFrame = GetTransformOnSpecificBlockAndFrame(track, 
            //        block: CurrentBlock, frame: (float)Math.Floor(frame));
            //    NewBlendableTransform nextFrame = GetTransformOnSpecificBlockAndFrame(track, block: 0, frame: 0);
            //    currentFrame = NewBlendableTransform.Lerp(frame % 1, currentFrame, nextFrame);
            //    return currentFrame;
            //}
            //// Regular frame
            //else
            //{
            //    NewBlendableTransform currentFrame = GetTransformOnSpecificBlockAndFrame(track, 
            //        block: CurrentBlock, frame);
            //    return currentFrame;
            //}
        }
    }

    public class HavokAnimationData_InterleavedUncompressed : HavokAnimationData
    {
        public int TransformTrackCount { get; }
        public List<NewBlendableTransform> Transforms { get; }

        public HavokAnimationData_InterleavedUncompressed(string name, HKX.HKASkeleton skeleton,
           HKX.HKADefaultAnimatedReferenceFrame refFrame, HKX.HKAAnimationBinding binding, HKX.HKAInterleavedUncompressedAnimation anim)
           : base(name, skeleton, refFrame, binding)
        {
            Duration = anim.Duration;// Math.Max(anim.Duration, anim.FrameDuration * anim.FrameCount);
            TransformTrackCount = anim.TransformTrackCount;
            FrameCount = (int)anim.Transforms.Capacity / anim.TransformTrackCount;

            FrameDuration = Duration / (FrameCount - 1);

            HkxBoneIndexToTransformTrackMap = new int[skeleton.Bones.Size];
            TransformTrackIndexToHkxBoneMap = new int[binding.TransformTrackToBoneIndices.Size];

            for (int i = 0; i < binding.TransformTrackToBoneIndices.Size; i++)
            {
                TransformTrackIndexToHkxBoneMap[i] = -1;
            }

            for (int i = 0; i < skeleton.Bones.Size; i++)
            {
                HkxBoneIndexToTransformTrackMap[i] = -1;
            }

            for (int i = 0; i < binding.TransformTrackToBoneIndices.Size; i++)
            {
                short boneIndex = binding.TransformTrackToBoneIndices[i].data;
                if (boneIndex >= 0 && boneIndex < skeleton.Bones.Size)
                    HkxBoneIndexToTransformTrackMap[boneIndex] = i;
                TransformTrackIndexToHkxBoneMap[i] = boneIndex;
            }

            Transforms = new List<NewBlendableTransform>((int)anim.Transforms.Capacity);

            foreach (var t in anim.Transforms.GetArrayData().Elements)
            {
                Transforms.Add(new NewBlendableTransform()
                {
                    Translation = new Vector3(t.Position.Vector.X, t.Position.Vector.Y, t.Position.Vector.Z),
                    Scale = new Vector3(t.Scale.Vector.X, t.Scale.Vector.Y, t.Scale.Vector.Z),
                    Rotation = new Quaternion(t.Rotation.Vector.X, t.Rotation.Vector.Y, t.Rotation.Vector.Z, t.Rotation.Vector.W)
                });
            }
        }

        public override NewBlendableTransform GetTransformOnFrame(int transformTrackIndex, float frame, bool enableLooping)
        {
            float loopedFrame = (enableLooping ? frame % (FrameCount - 1) : frame);

            if (frame < 0)
                frame += FrameCount;

            if (frame > FrameCount)
                frame = FrameCount;



            NewBlendableTransform currentFrame = Transforms[(TransformTrackCount * (int)Math.Floor(loopedFrame)) + transformTrackIndex];
            NewBlendableTransform nextFrame = Transforms[(TransformTrackCount * (int)Math.Ceiling(loopedFrame)) + transformTrackIndex];
            return NewBlendableTransform.Lerp(currentFrame, nextFrame, loopedFrame % 1);
        }
    }
}
