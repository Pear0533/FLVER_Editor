using System;
using System.Linq;
using System.Numerics;

namespace SoulsAssetPipeline.Animation
{
    public class RootMotionData
    {
        //public Matrix CurrentAbsoluteRootMotion = Matrix.Identity;

        public readonly Vector4 Up;
        public readonly Vector4 Forward;
        public readonly float Duration;
        public readonly Vector4[] Frames;

        public Vector4 FirstFrame => Frames[0];
        public Vector4 LastFrame => Frames[Frames.Length - 1];

        public RootMotionData(HKX.HKADefaultAnimatedReferenceFrame refFrame) : this(refFrame.Up, refFrame.Forward, refFrame.Duration, refFrame.ReferenceFrameSamples.GetArrayData().Elements.Select(hkxVector => hkxVector.Vector).ToArray())
        {
        }

        public RootMotionData(Vector4 up, Vector4 forward, float duration, Vector4[] frames)
        {
            Up = up;
            Forward = forward;
            Duration = duration;
            Frames = frames;
        }

        private Vector4 GetSampleOnExactFrame(int frame)
        {
            if (frame < 0)
                frame = 0;
            int frameDataIndex = frame % (Frames.Length - 1);
            int loopIndex = frame / (Frames.Length - 1);
            return Frames[frameDataIndex] + (Frames[Frames.Length - 1] * loopIndex);
        }

        public Vector4 GetSampleClamped(float time)
        {
            if (Frames.Length == 1 || time <= 0)
                return Frames[0];
            else if (time >= Duration)
                return Frames[Frames.Length - 1];

            float frame = time / (Duration / (Frames.Length - 1));

            return Vector4.Lerp(
                GetSampleOnExactFrame((int)Math.Floor((float)frame)),
                GetSampleOnExactFrame((int)Math.Ceiling((float)frame)),
                frame % 1.0f);
        }
    }
}
