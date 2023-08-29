using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NMatrix = System.Numerics.Matrix4x4;
using NVector3 = System.Numerics.Vector3;
using NVector4 = System.Numerics.Vector4;
using NQuaternion = System.Numerics.Quaternion;

namespace SoulsAssetPipeline.Animation
{
    public class RootMotionDataPlayer
    {
        public readonly RootMotionData Data;

        public NVector4 CurrentTransform { get; private set; } = NVector4.Zero;
        public NVector4 PreviousFrameTransform { get; private set; } = NVector4.Zero;
        public NVector4 TransformFrameDelta => CurrentTransform - PreviousFrameTransform;
        public NVector4 LoopStartTransform { get; private set; } = NVector4.Zero;

        public float CurrentTime { get; private set; } = 0;
        public int CurrentLoopNum { get; private set; } = 0;
        public float CurrentLoopStartTime => CurrentLoopNum * Data.Duration;
        public float CurrentLoopEndTime => (CurrentLoopNum + 1) * Data.Duration;

        private NVector4 SampleRootMotionData(float localTime)
        {
            return Data?.GetSampleClamped(localTime) ?? NVector4.Zero;
        }

        public void ResetToStart(NVector4 startTransform)
        {
            CurrentTime = 0;
            CurrentLoopNum = 0;
            LoopStartTransform = CurrentTransform = PreviousFrameTransform = startTransform;
        }


        public RootMotionDataPlayer(RootMotionData data)
        {
            Data = data;
        }

        private void SetTransformToTimeWithinCurrentLoop(float localTime)
        {
            // Get sample at specified time.
            var sampleAtTime = SampleRootMotionData(localTime);

            // Rotate sample translation to be relative to loop start transform rotation.
            var translation = NVector3.Transform(sampleAtTime.XYZ(), NMatrix.CreateRotationY(LoopStartTransform.W));
            sampleAtTime.X = translation.X;
            sampleAtTime.Y = translation.Y;
            sampleAtTime.Z = translation.Z;
            
            CurrentTransform = LoopStartTransform + sampleAtTime;
        }

        private void GoToNextLoop()
        {
            int nextLoop = CurrentLoopNum + 1;
            float nextLoopStartTime = nextLoop * Data.Duration;
            SetTransformToTimeWithinCurrentLoop(Data.Duration);
            CurrentTime = nextLoopStartTime;
            LoopStartTransform = CurrentTransform;
            CurrentLoopNum++;
        }

        private void GoToPreviousLoop()
        {
            float startOfCurrentLoop = CurrentLoopNum * Data.Duration;
            SetTransformToTimeWithinCurrentLoop(0);
            CurrentTime = startOfCurrentLoop;

            LoopStartTransform = GetLoopStartFromLoopEnd(CurrentTransform);
            //SetTransformToTimeWithinCurrentLoop(Data.Duration);
            CurrentLoopNum--;
        }

        /// <summary>
        /// TODO: Check if this actually works.
        /// </summary>
        private NVector4 GetLoopStartFromLoopEnd(NVector4 loopEnd)
        {

            var angleAtStart = loopEnd.W + (Data.FirstFrame.W - Data.LastFrame.W);
            // Find the delta from end to start
            var deltaStartToEnd = (Data.LastFrame - Data.FirstFrame);

            // Rotate delta translation by the delta from end to start 
            var translation = NVector3.Transform(deltaStartToEnd.XYZ(), NMatrix.CreateRotationY(angleAtStart));
            deltaStartToEnd.X = loopEnd.X - translation.X;
            deltaStartToEnd.Y = loopEnd.Y - translation.Y;
            deltaStartToEnd.Z = loopEnd.Z - translation.Z;
            deltaStartToEnd.W = angleAtStart;

            return deltaStartToEnd;
        }

        public void ApplyExternalTransformSuchThatCurrentTransformMatches(NVector4 desiredCurrentTransform)
        {
            ApplyExternalTransform(desiredCurrentTransform - CurrentTransform);
        }

        public void ApplyExternalTransform(float rotation, NVector3 translation)
        {
            ApplyExternalTransform(new NVector4(translation.X, translation.Y, translation.Z, rotation));
        }

        public void ApplyExternalTransform(NVector4 transform)
        {
            // Find where loop start is relative to current location
            var deltaCurrentToLoopStart = (LoopStartTransform - CurrentTransform);


            // Rotate loop start such that the current location is pivoted
            var newTranslation = NVector3.Transform(deltaCurrentToLoopStart.XYZ(), NMatrix.CreateRotationY(transform.W));
            deltaCurrentToLoopStart.X = CurrentTransform.X + newTranslation.X + transform.X;
            deltaCurrentToLoopStart.Y = CurrentTransform.Y + newTranslation.Y + transform.Y;
            deltaCurrentToLoopStart.Z = CurrentTransform.Z + newTranslation.Z + transform.Z;
            deltaCurrentToLoopStart.W = LoopStartTransform.W + transform.W;

            LoopStartTransform = deltaCurrentToLoopStart;

            var tr = CurrentTransform;
            tr += transform;
            CurrentTransform = tr;

            tr = PreviousFrameTransform;
            tr += transform;
            PreviousFrameTransform = tr;

            //tr = LoopStartTransform;
            //tr.W += rotation;
            //LoopStartTransform = tr;
        }

        public void Scrub(float deltaTime)
        {
            SetTime(CurrentTime + deltaTime);
        }

        public void SetTime(float newTime)
        {
            if (newTime == CurrentTime)
                return;

            PreviousFrameTransform = CurrentTransform;

            if (Data == null)
            {
                ResetToStart(CurrentTransform);
                CurrentTime = newTime;
                return;
            }

            while (CurrentTime != newTime)
            {
                int newLoopNum = (int)Math.Floor(newTime / Data.Duration);
                if (newLoopNum > CurrentLoopNum)
                {
                    GoToNextLoop();
                }
                else if (newLoopNum < CurrentLoopNum)
                {
                    GoToPreviousLoop();
                }
                else
                {
                    SetTransformToTimeWithinCurrentLoop(newTime % Data.Duration);
                    CurrentTime = newTime;
                }
            }
        }
    }
}
