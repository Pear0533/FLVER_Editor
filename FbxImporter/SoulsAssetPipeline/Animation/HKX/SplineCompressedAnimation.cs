﻿//	This is an adaptation of code from the Havok Format Library
//  https://github.com/PredatorCZ/HavokLib/blob/master/source/hkaSplineDecompressor.cpp
//	Original code Copyright(C) 2016-2019 Lukas Cone
//  Adapted to C# by Meowmaritus and Katalash
//
//	This program is free software : you can redistribute it and / or modify
//	it under the terms of the GNU General Public License as published by
//	the Free Software Foundation, either version 3 of the License, or
//	(at your option) any later version.
//
//	This program is distributed in the hope that it will be useful,
//	but WITHOUT ANY WARRANTY; without even the implied warranty of
//	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//	GNU General Public License for more details.
//
//	You should have received a copy of the GNU General Public License
//	along with this program.If not, see <https://www.gnu.org/licenses/>.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using SoulsFormats;

namespace SoulsAssetPipeline.Animation
{
    public class SplineCompressedAnimation
    {
        [Flags]
        public enum FlagOffset : byte
        {
            StaticX = 0b00000001,
            StaticY = 0b00000010,
            StaticZ = 0b00000100,
            StaticW = 0b00001000,
            SplineX = 0b00010000,
            SplineY = 0b00100000,
            SplineZ = 0b01000000,
            SplineW = 0b10000000
        };

        public enum ScalarQuantizationType
        {
            BITS8 = 0,
            BITS16 = 1,
        };

        public enum RotationQuantizationType
        {
            POLAR32 = 0, //4 bytes long
            THREECOMP40 = 1, //5 bytes long
            THREECOMP48 = 2, //6 bytes long
            THREECOMP24 = 3, //3 bytes long
            STRAIGHT16 = 4, //2 bytes long
            UNCOMPRESSED = 5, //16 bytes long
        }

        static int GetRotationAlign(RotationQuantizationType qt)
        {
            switch (qt)
            {
                case RotationQuantizationType.POLAR32: return 4;
                case RotationQuantizationType.THREECOMP40: return 1;
                case RotationQuantizationType.THREECOMP48: return 2;
                case RotationQuantizationType.THREECOMP24: return 1;
                case RotationQuantizationType.STRAIGHT16: return 2;
                case RotationQuantizationType.UNCOMPRESSED: return 4;
                default: throw new NotImplementedException();
            }
        }

        static int GetRotationByteCount(RotationQuantizationType qt)
        {
            switch (qt)
            {
                case RotationQuantizationType.POLAR32: return 4;
                case RotationQuantizationType.THREECOMP40: return 5;
                case RotationQuantizationType.THREECOMP48: return 6;
                case RotationQuantizationType.THREECOMP24: return 3;
                case RotationQuantizationType.STRAIGHT16: return 2;
                case RotationQuantizationType.UNCOMPRESSED: return 16;
                default: throw new NotImplementedException();
            }
        }

        static float ReadQuantizedFloat(BinaryReaderEx bin, float min, float max, ScalarQuantizationType type)
        {
            float ratio = -1;
            switch (type)
            {
                case ScalarQuantizationType.BITS8: ratio = bin.ReadByte() / 255.0f; break;
                case ScalarQuantizationType.BITS16: ratio = bin.ReadUInt16() / 65535.0f; break;
                default: throw new NotImplementedException();
            }
            return min + ((max - min) * ratio);
        }

        // Because C# can't static cast an int to a float natively
        static float CastToFloat(uint src)
        {
            var floatbytes = BitConverter.GetBytes(src);
            return BitConverter.ToSingle(floatbytes, 0);
        }

        static Quaternion ReadQuatPOLAR32(BinaryReaderEx br)
        {
            const ulong rMask = (1 << 10) - 1;
            const float rFrac = 1.0f / rMask;
            const float fPI = 3.14159265f;
            const float fPI2 = 0.5f * fPI;
            const float fPI4 = 0.5f * fPI2;
            const float phiFrac = fPI2 / 511.0f;

            uint cVal = br.ReadUInt32();

            float R = CastToFloat((cVal >> 18) & (uint)(rMask & 0xFFFFFFFF)) * rFrac;
            R = 1.0f - (R * R);

            float phiTheta = (float)((cVal & 0x3FFFF));

            float phi = (float)Math.Floor(Math.Sqrt(phiTheta));
            float theta = 0;

            if (phi > 0.0f)
            {
                theta = fPI4 * (phiTheta - (phi * phi)) / phi;
                phi = phiFrac * phi;
            }

            float magnitude = (float)Math.Sqrt(1.0f - R * R);

            Quaternion retVal;
            retVal.X = (float)(Math.Sin(phi) * Math.Cos(theta) * magnitude);
            retVal.Y = (float)(Math.Sin(phi) * Math.Sin(theta) * magnitude);
            retVal.Z = (float)(Math.Cos(phi) * magnitude);
            retVal.W = R;

            if ((cVal & 0x10000000) > 0)
                retVal.X *= -1;

            if ((cVal & 0x20000000) > 0)
                retVal.Y *= -1;

            if ((cVal & 0x40000000) > 0)
                retVal.Z *= -1;

            if ((cVal & 0x80000000) > 0)
                retVal.W *= -1;

            return retVal;
        }

        static Quaternion ReadQuatTHREECOMP48(BinaryReaderEx br)
        {
            const ulong mask = (1 << 15) - 1;
            const float fractal = 0.000043161f;

            short x = br.ReadInt16();
            short y = br.ReadInt16();
            short z = br.ReadInt16();

            char resultShift = (char)(((y >> 14) & 2) | ((x >> 15) & 1));
            bool rSign = (z >> 15) != 0;

            x &= (short)mask;
            x -= (short)(mask >> 1);
            y &= (short)mask;
            y -= (short)(mask >> 1);
            z &= (short)mask;
            z -= (short)(mask >> 1);

            float[] tempValF = new float[3];
            tempValF[0] = (float)x * fractal;
            tempValF[1] = (float)y * fractal;
            tempValF[2] = (float)z * fractal;

            float[] retval = new float[4];

            for (int i = 0; i < 4; i++)
            {
                if (i < resultShift)
                    retval[i] = tempValF[i];
                else if (i > resultShift)
                    retval[i] = tempValF[i - 1];
            }

            retval[resultShift] = 1.0f - tempValF[0] * tempValF[0] - tempValF[1] * tempValF[1] - tempValF[2] * tempValF[2];

            if (retval[resultShift] <= 0.0f)
                retval[resultShift] = 0.0f;
            else
                retval[resultShift] = (float)Math.Sqrt(retval[resultShift]);

            if (rSign)
                retval[resultShift] *= -1;

            return new Quaternion(retval[0], retval[1], retval[2], retval[3]);
        }

        static ulong Read40BitValue(BinaryReaderEx br)
        {
            byte[] bytes = br.ReadBytes(5);
            Array.Resize(ref bytes, 8);
            return BitConverter.ToUInt64(bytes, 0);
        }

        static Quaternion ReadQuatTHREECOMP40(BinaryReaderEx br)
        {
            const ulong mask = (1 << 12) - 1;
            const ulong positiveMask = mask >> 1;
            const float fractal = 0.000345436f;
            // Read only the 5 bytes needed to prevent EndOfStreamException :fatcat:
            ulong cVal = Read40BitValue(br);

            int x = (int)(cVal & mask);
            int y = (int)((cVal >> 12) & mask);
            int z = (int)((cVal >> 24) & mask);

            int resultShift = (int)((cVal >> 36) & 3);

            x -= (int)positiveMask;
            y -= (int)positiveMask;
            z -= (int)positiveMask;

            float[] tempValF = new float[3];
            tempValF[0] = (float)x * fractal;
            tempValF[1] = (float)y * fractal;
            tempValF[2] = (float)z * fractal;

            float[] retval = new float[4];

            for (int i = 0; i < 4; i++)
            {
                if (i < resultShift)
                    retval[i] = tempValF[i];
                else if (i > resultShift)
                    retval[i] = tempValF[i - 1];
            }

            retval[resultShift] = 1.0f - tempValF[0] * tempValF[0] - tempValF[1] * tempValF[1] - tempValF[2] * tempValF[2];

            if (retval[resultShift] <= 0.0f)
                retval[resultShift] = 0.0f;
            else
                retval[resultShift] = (float)Math.Sqrt(retval[resultShift]);

            if (((cVal >> 38) & 1) > 0)
                retval[resultShift] *= -1;

            var finalQuat = new Quaternion(retval[0], retval[1], retval[2], retval[3]);

            return finalQuat;

        }

        static Quaternion ReadQuantizedQuaternion(BinaryReaderEx br, RotationQuantizationType type)
        {
            switch (type)
            {
                case RotationQuantizationType.POLAR32:
                    return ReadQuatPOLAR32(br);
                case RotationQuantizationType.THREECOMP40:
                    return ReadQuatTHREECOMP40(br);
                case RotationQuantizationType.THREECOMP48:
                    return ReadQuatTHREECOMP48(br);
                case RotationQuantizationType.THREECOMP24:
                case RotationQuantizationType.STRAIGHT16:
                    throw new NotImplementedException();
                case RotationQuantizationType.UNCOMPRESSED:
                    return new Quaternion(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                default:
                    return Quaternion.Identity;
            }
        }

        // Algorithm A2.1 The NURBS Book 2nd edition, page 68
        static int FindKnotSpan(int degree, float value, int cPointsSize, List<byte> knots)
        {
            if (value >= knots[cPointsSize])
                return cPointsSize - 1;

            int low = degree;
            int high = cPointsSize;
            int mid = (low + high) / 2;

            while (value < knots[mid] || value >= knots[mid + 1])
            {
                if (value < knots[mid])
                    high = mid;
                else
                    low = mid;

                mid = (low + high) / 2;
            }

            return mid;
        }

        //Basis_ITS1, GetPoint_NR1, TIME-EFFICIENT NURBS CURVE EVALUATION ALGORITHMS, pages 64 & 65
        static float GetSinglePoint(int knotSpanIndex, int degree, float frame, List<byte> knots, List<float> cPoints)
        {
            float[] N = { 1, 0, 0, 0, 0 };

            for (int i = 1; i <= degree; i++)
                for (int j = i - 1; j >= 0; j--)
                {
                    
                    float A = (frame - knots[knotSpanIndex - j]) / (knots[knotSpanIndex + i - j] - knots[knotSpanIndex - j]);
                    // without multiplying A, model jitters slightly
                    float tmp = N[j] * A;
                    // without subtracting tmp, model flies away then resets to origin every few frames
                    N[j + 1] += N[j] - tmp;
                    // without setting to tmp, model either is moved from origin or grows very long limbs
                    // depending on the animation
                    N[j] = tmp;
                }

            float retVal = 0.0f;

            for (int i = 0; i <= degree; i++)
                retVal += cPoints[knotSpanIndex - i] * N[i];

            return retVal;
        }

        //Basis_ITS1, GetPoint_NR1, TIME-EFFICIENT NURBS CURVE EVALUATION ALGORITHMS, pages 64 & 65
        static Quaternion GetSinglePoint(int knotSpanIndex, int degree, float frame, List<byte> knots, List<Quaternion> cPoints)
        {
            float[] N = { 1.0f, 0.0f, 0.0f, 0.0f, 0.0f };

            for (int i = 1; i <= degree; i++)
                for (int j = i - 1; j >= 0; j--)
                {
                    float A = (frame - knots[knotSpanIndex - j]) / (knots[knotSpanIndex + i - j] - knots[knotSpanIndex - j]);
                    float tmp = N[j] * A;
                    N[j + 1] += N[j] - tmp;
                    N[j] = tmp;
                }

            Quaternion retVal = new Quaternion(Vector3.Zero, 0.0f);

            if (knotSpanIndex > 0)
            {
                for (int i = 0; i <= degree; i++)
                    retVal += cPoints[knotSpanIndex - i] * N[i];
            }

        

            return retVal;
        }

        public class SplineChannel<T>
        {
            public bool IsDynamic = true;
            public List<T> Values = new List<T>();
        }

        public class SplineTrackQuaternion
        {
            public SplineChannel<Quaternion> Channel;
            public List<byte> Knots = new List<byte>();
            public byte Degree;

            internal SplineTrackQuaternion(BinaryReaderEx br, RotationQuantizationType quantizationType)
            {
                long debug_StartOfThisSplineTrack = br.Position;

                short numItems = br.ReadInt16();
                Degree = br.ReadByte();
                int knotCount = numItems + Degree + 2;
                for (int i = 0; i < knotCount; i++)
                {
                    Knots.Add(br.ReadByte());
                }

                br.Pad(GetRotationAlign(quantizationType));

                Channel = new SplineChannel<Quaternion>();

                for (int i = 0; i <= numItems; i++)
                {
                    Channel.Values.Add(ReadQuantizedQuaternion(br, quantizationType));

                    //try
                    //{
                        
                    //}
                    //catch (System.IO.EndOfStreamException)
                    //{
                    //    // TEST
                    //    Channel.Values.Add(Quaternion.Identity);
                    //}
                }
            }

            public Quaternion GetValue(float frame)
            {
                int knotspan = FindKnotSpan(Degree, frame, Channel.Values.Count, Knots);
                return GetSinglePoint(knotspan, Degree, frame, Knots, Channel.Values);
            }
        }

        public class SplineTrackVector3
        {
            public SplineChannel<float> ChannelX;
            public SplineChannel<float> ChannelY;
            public SplineChannel<float> ChannelZ;
            public List<byte> Knots = new List<byte>();
            public byte Degree;

            internal SplineTrackVector3(BinaryReaderEx br, List<FlagOffset> channelTypes, ScalarQuantizationType quantizationType, bool isPosition)
            {
                long debug_StartOfThisSplineTrack = br.Position;

                short numItems = br.ReadInt16();
                Degree = br.ReadByte();
                int knotCount = numItems + Degree + 2;
                for (int i = 0; i < knotCount; i++)
                {
                    Knots.Add(br.ReadByte());
                }

                br.Pad(4);

                float BoundsXMin = 0;
                float BoundsXMax = 0;
                float BoundsYMin = 0;
                float BoundsYMax = 0;
                float BoundsZMin = 0;
                float BoundsZMax = 0;

                ChannelX = new SplineChannel<float>();
                ChannelY = new SplineChannel<float>();
                ChannelZ = new SplineChannel<float>();

                if (channelTypes.Contains(FlagOffset.SplineX))
                {
                    BoundsXMin = br.ReadSingle();
                    BoundsXMax = br.ReadSingle();
                }
                else if (channelTypes.Contains(FlagOffset.StaticX))
                {
                    ChannelX.Values = new List<float> { br.ReadSingle() };
                    ChannelX.IsDynamic = false;
                }
                else
                {
                    ChannelX = null;
                }

                if (channelTypes.Contains(FlagOffset.SplineY))
                {
                    BoundsYMin = br.ReadSingle();
                    BoundsYMax = br.ReadSingle();
                }
                else if (channelTypes.Contains(FlagOffset.StaticY))
                {
                    ChannelY.Values = new List<float> { br.ReadSingle() };
                    ChannelY.IsDynamic = false;
                }
                else
                {
                    ChannelY = null;
                }

                if (channelTypes.Contains(FlagOffset.SplineZ))
                {
                    BoundsZMin = br.ReadSingle();
                    BoundsZMax = br.ReadSingle();
                }
                else if (channelTypes.Contains(FlagOffset.StaticZ))
                {
                    ChannelZ.Values = new List<float> { br.ReadSingle() };
                    ChannelZ.IsDynamic = false;
                }
                else
                {
                    ChannelZ = null;
                }

                for (int i = 0; i <= numItems; i++)
                {
                    if (channelTypes.Contains(FlagOffset.SplineX))
                    {
                        ChannelX.Values.Add(ReadQuantizedFloat(br, BoundsXMin, BoundsXMax, quantizationType));
                    }

                    if (channelTypes.Contains(FlagOffset.SplineY))
                    {
                        ChannelY.Values.Add(ReadQuantizedFloat(br, BoundsYMin, BoundsYMax, quantizationType));
                    }

                    if (channelTypes.Contains(FlagOffset.SplineZ))
                    {
                        ChannelZ.Values.Add(ReadQuantizedFloat(br, BoundsZMin, BoundsZMax, quantizationType));
                    }
                }
            }

            public float? GetValueX(float frame)
            {
                if (ChannelX == null)
                    return null;

                if (ChannelX.Values.Count == 1)
                    return ChannelX.Values[0];
                int knotspan = FindKnotSpan(Degree, frame, ChannelX.Values.Count, Knots);
                return GetSinglePoint(knotspan, Degree, frame, Knots, ChannelX.Values);
            }

            public float? GetValueY(float frame)
            {
                if (ChannelY == null)
                    return null;

                if (ChannelY.Values.Count == 1)
                    return ChannelY.Values[0];
                int knotspan = FindKnotSpan(Degree, frame, ChannelY.Values.Count, Knots);
                return GetSinglePoint(knotspan, Degree, frame, Knots, ChannelY.Values);
            }

            public float? GetValueZ(float frame)
            {
                if (ChannelZ == null)
                    return null;

                if (ChannelZ.Values.Count == 1)
                    return ChannelZ.Values[0];
                int knotspan = FindKnotSpan(Degree, frame, ChannelZ.Values.Count, Knots);
                return GetSinglePoint(knotspan, Degree, frame, Knots, ChannelZ.Values);
            }
        }

        public class TransformMask
        {
            public ScalarQuantizationType PositionQuantizationType;
            public RotationQuantizationType RotationQuantizationType;
            public ScalarQuantizationType ScaleQuantizationType;
            public List<FlagOffset> PositionTypes;
            public List<FlagOffset> RotationTypes;
            public List<FlagOffset> ScaleTypes;

            internal TransformMask(BinaryReaderEx br)
            {
                PositionTypes = new List<FlagOffset>();
                RotationTypes = new List<FlagOffset>();
                ScaleTypes = new List<FlagOffset>();

                var byteQuantizationTypes = br.ReadByte();
                var bytePositionTypes = (FlagOffset)br.ReadByte();
                var byteRotationTypes = (FlagOffset)br.ReadByte();
                var byteScaleTypes = (FlagOffset)br.ReadByte();

                PositionQuantizationType = (ScalarQuantizationType)(byteQuantizationTypes & 3);
                RotationQuantizationType = (RotationQuantizationType)((byteQuantizationTypes >> 2) & 0xF);
                ScaleQuantizationType = (ScalarQuantizationType)((byteQuantizationTypes >> 6) & 3);

                foreach (var flagOffset in (FlagOffset[])Enum.GetValues(typeof(FlagOffset)))
                {
                    if ((bytePositionTypes & flagOffset) != 0)
                        PositionTypes.Add(flagOffset);

                    if ((byteRotationTypes & flagOffset) != 0)
                        RotationTypes.Add(flagOffset);

                    if ((byteScaleTypes & flagOffset) != 0)
                        ScaleTypes.Add(flagOffset);
                }
            }
        }

        public class TransformTrack
        {
            public TransformMask Mask;

            public bool HasSplinePosition;
            public bool HasSplineRotation;
            public bool HasSplineScale;

            public bool HasStaticRotation;

            public Vector3 StaticPosition = Vector3.Zero;
            public Quaternion StaticRotation = Quaternion.Identity;
            public Vector3 StaticScale = Vector3.One;
            public SplineTrackVector3 SplinePosition = null;
            public SplineTrackQuaternion SplineRotation = null;
            public SplineTrackVector3 SplineScale = null;
        }


        public static List<NewBlendableTransform> ReadSplCmpAnimBytesAndSampleToUncomp(
            byte[] animationData, int numTransformTracks, int numBlocks, int numFrames, int numFramesPerBlock)
        {
            var tracks = ReadSplineCompressedAnimByteBlock(
                isBigEndian: false, animationData, numTransformTracks, numBlocks);

            NewBlendableTransform GetTransformOnSpecificBlockAndFrame(int transformIndex, int block, float frame)
            {
                frame = (frame % numFrames) % numFramesPerBlock;

                NewBlendableTransform result = NewBlendableTransform.Identity;
                var track = tracks[block][transformIndex];

                //result.Scale.X = track.SplineScale?.ChannelX == null
                //    ? (IsAdditiveBlend ? 1 : track.StaticScale.X) : track.SplineScale.GetValueX(frame);
                //result.Scale.Y = track.SplineScale?.ChannelY == null
                //    ? (IsAdditiveBlend ? 1 : track.StaticScale.Y) : track.SplineScale.GetValueY(frame);
                //result.Scale.Z = track.SplineScale?.ChannelZ == null
                //    ? (IsAdditiveBlend ? 1 : track.StaticScale.Z) : track.SplineScale.GetValueZ(frame);

                if (track.SplineScale != null)
                {
                    result.Scale.X = track.SplineScale.GetValueX(frame) ?? 1;

                    result.Scale.Y = track.SplineScale.GetValueY(frame) ?? 1;

                    result.Scale.Z = track.SplineScale.GetValueZ(frame) ?? 1;
                }
                else
                {
                    if (track.Mask.ScaleTypes.Contains(SplineCompressedAnimation.FlagOffset.StaticX))
                        result.Scale.X = track.StaticScale.X;
                    else
                        result.Scale.X = 1;

                    if (track.Mask.ScaleTypes.Contains(SplineCompressedAnimation.FlagOffset.StaticY))
                        result.Scale.Y = track.StaticScale.Y;
                    else
                        result.Scale.Y = 1;

                    if (track.Mask.ScaleTypes.Contains(SplineCompressedAnimation.FlagOffset.StaticZ))
                        result.Scale.Z = track.StaticScale.Z;
                    else
                        result.Scale.Z = 1;
                }

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
                    //result.Rotation = IsAdditiveBlend ? Quaternion.Identity : new Quaternion(
                    //    skeleTransform.Rotation.Vector.X,
                    //    skeleTransform.Rotation.Vector.Y,
                    //    skeleTransform.Rotation.Vector.Z,
                    //    skeleTransform.Rotation.Vector.W);
                }

                if (track.SplinePosition != null)
                {
                    result.Translation.X = track.SplinePosition.GetValueX(frame) ?? 0;

                    result.Translation.Y = track.SplinePosition.GetValueY(frame) ?? 0;

                    result.Translation.Z = track.SplinePosition.GetValueZ(frame) ?? 0;
                }
                else
                {
                    if (track.Mask.PositionTypes.Contains(FlagOffset.StaticX))
                        result.Translation.X = track.StaticPosition.X;
                    else
                        result.Translation.X = 0;

                    if (track.Mask.PositionTypes.Contains(FlagOffset.StaticY))
                        result.Translation.Y = track.StaticPosition.Y;
                    else
                        result.Translation.Y = 0;

                    if (track.Mask.PositionTypes.Contains(FlagOffset.StaticZ))
                        result.Translation.Z = track.StaticPosition.Z;
                    else
                        result.Translation.Z = 0;
                }

                return result;
            }

            List<NewBlendableTransform> resultList = new List<NewBlendableTransform>();

            for (int f = 0; f < numFrames; f++)
            {
                float frame = (f % numFrames) % numFramesPerBlock;

                int currentBlock = (int)((f % numFrames) / numFramesPerBlock);

                for (int t = 0; t < numTransformTracks; t++)
                {
                    if (frame >= numFrames - 1)
                    {
                        NewBlendableTransform currentFrame = GetTransformOnSpecificBlockAndFrame(t,
                            block: currentBlock, frame: (float)Math.Floor(frame));
                        NewBlendableTransform nextFrame = GetTransformOnSpecificBlockAndFrame(t, block: 0, frame: 0);
                        currentFrame = NewBlendableTransform.Lerp(currentFrame, nextFrame, frame % 1);
                        resultList.Add(currentFrame);
                    }
                    // Regular frame
                    else
                    {
                        NewBlendableTransform currentFrame = GetTransformOnSpecificBlockAndFrame(t,
                            block: currentBlock, frame);
                        resultList.Add(currentFrame);
                    }
                }
            }

            return resultList;
        }

        public static List<TransformTrack[]> ReadSplineCompressedAnimByteBlock(
            bool isBigEndian, byte[] animationData, int numTransformTracks, int numBlocks)
        {
            List<TransformTrack[]> blocks = new List<TransformTrack[]>();

            var br = new BinaryReaderEx(isBigEndian, animationData);

            for (int blockIndex = 0; blockIndex < numBlocks; blockIndex++)
            {
                var TransformTracks = new TransformTrack[numTransformTracks];

                for (int i = 0; i < numTransformTracks; i++)
                {
                    TransformTracks[i] = new TransformTrack();
                }

                for (int i = 0; i < numTransformTracks; i++)
                {
                    TransformTracks[i].Mask = new TransformMask(br);
                }

                br.Pad(4);

                for (int i = 0; i < numTransformTracks; i++)
                {
                    var m = TransformTracks[i].Mask;
                    var track = TransformTracks[i];

                    track.HasSplinePosition = m.PositionTypes.Contains(FlagOffset.SplineX)
                        || m.PositionTypes.Contains(FlagOffset.SplineY)
                        || m.PositionTypes.Contains(FlagOffset.SplineZ);

                    track.HasSplineRotation = m.RotationTypes.Contains(FlagOffset.SplineX)
                        || m.RotationTypes.Contains(FlagOffset.SplineY)
                        || m.RotationTypes.Contains(FlagOffset.SplineZ)
                        || m.RotationTypes.Contains(FlagOffset.SplineW);

                    track.HasStaticRotation = m.RotationTypes.Contains(FlagOffset.StaticX)
                        || m.RotationTypes.Contains(FlagOffset.StaticY)
                        || m.RotationTypes.Contains(FlagOffset.StaticZ)
                        || m.RotationTypes.Contains(FlagOffset.StaticW);

                    track.HasSplineScale = m.ScaleTypes.Contains(FlagOffset.SplineX)
                        || m.ScaleTypes.Contains(FlagOffset.SplineY)
                        || m.ScaleTypes.Contains(FlagOffset.SplineZ);

                    if (track.HasSplinePosition)
                    {
                        track.SplinePosition = new SplineTrackVector3(br, m.PositionTypes, m.PositionQuantizationType, isPosition: true);
                    }
                    else
                    {
                        if (m.PositionTypes.Contains(FlagOffset.StaticX))
                        {
                            track.StaticPosition.X = br.ReadSingle();
                        }

                        if (m.PositionTypes.Contains(FlagOffset.StaticY))
                        {
                            track.StaticPosition.Y = br.ReadSingle();
                        }

                        if (m.PositionTypes.Contains(FlagOffset.StaticZ))
                        {
                            track.StaticPosition.Z = br.ReadSingle();
                        }
                    }

                    br.Pad(4);



                    if (track.HasSplineRotation)
                    {
                        track.SplineRotation = new SplineTrackQuaternion(br, m.RotationQuantizationType);
                    }
                    else
                    {
                        if (track.HasStaticRotation)
                        {
                            br.Pad(GetRotationAlign(m.RotationQuantizationType));
                            track.StaticRotation = ReadQuantizedQuaternion(br, m.RotationQuantizationType); //br.ReadBytes(GetRotationByteCount(m.RotationQuantizationType));
                        }
                    }

                    br.Pad(4);

                    if (track.HasSplineScale)
                    {
                        track.SplineScale = new SplineTrackVector3(br, m.ScaleTypes, m.ScaleQuantizationType, isPosition: false);
                    }
                    else
                    {
                        if (m.ScaleTypes.Contains(FlagOffset.StaticX))
                        {
                            track.StaticScale.X = br.ReadSingle();
                        }

                        if (m.ScaleTypes.Contains(FlagOffset.StaticY))
                        {
                            track.StaticScale.Y = br.ReadSingle();
                        }

                        if (m.ScaleTypes.Contains(FlagOffset.StaticZ))
                        {
                            track.StaticScale.Z = br.ReadSingle();
                        }
                    }

                    br.Pad(4);
                }

                br.Pad(16);

                blocks.Add(TransformTracks);
            }

            return blocks;
        }
    }
}
