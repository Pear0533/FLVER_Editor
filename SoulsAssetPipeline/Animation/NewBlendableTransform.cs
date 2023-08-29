using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SoulsAssetPipeline.Animation
{
    public struct NewBlendableTransform
    {
        public Vector3 Translation;
        public Vector3 Scale;
        public Quaternion Rotation;



        public NewBlendableTransform(Vector3 translation, Vector3 scale, Quaternion rotation)
        {
            Translation = translation;
            Scale = scale;
            Rotation = rotation;
        }

        public static NewBlendableTransform FromRootMotionSample(Vector4 sample)
        {
            return new NewBlendableTransform(sample.XYZ(), Vector3.One, Quaternion.CreateFromYawPitchRoll(sample.W, 0, 0));
        }

        public static NewBlendableTransform GetDelta(NewBlendableTransform from, NewBlendableTransform to)
        {
            var r = Identity;
            r.Rotation = to.Rotation * Quaternion.Inverse(from.Rotation);
            r.Translation = to.Translation - from.Translation;
            r.Scale = Vector3.One;//idk
            return r;
        }

        public static NewBlendableTransform Invert(NewBlendableTransform v)
        {
            var r = v;
            r.Translation = v.Translation * -1;
            r.Rotation = Quaternion.Inverse(v.Rotation);
            //r.Rotation = Quaternion.CreateFromRotationMatrix(Matrix4x4.CreateFromQuaternion(r.Rotation));
            //idk about scale cuz divide by 0 stuff.
            return r;
        }

        public static NewBlendableTransform ApplyFromToDeltaTransform(NewBlendableTransform v, NewBlendableTransform from, NewBlendableTransform to)
        {
            var d = GetDelta(from, to);
            var r = v;
            r.Rotation = d.Rotation * v.Rotation;
            r.Translation = v.Translation + d.Translation;
            //r.Scale = (to.Scale / from.Scale) * v.Scale;
            return r;
        }

        public static NewBlendableTransform FromHKXTransform(HKX.Transform bn)
        {
            var tr = NewBlendableTransform.Identity;

            tr.Translation = new System.Numerics.Vector3(
                            bn.Position.Vector.X,
                            bn.Position.Vector.Y,
                            bn.Position.Vector.Z);

            tr.Scale = new System.Numerics.Vector3(
                bn.Scale.Vector.X,
                bn.Scale.Vector.Y,
                bn.Scale.Vector.Z);

            tr.Rotation = new System.Numerics.Quaternion(
                bn.Rotation.Vector.X,
                bn.Rotation.Vector.Y,
                bn.Rotation.Vector.Z,
                bn.Rotation.Vector.W);

            return tr;
        }

        public static NewBlendableTransform Normalize(NewBlendableTransform v)
        {
            v.Rotation = Quaternion.Normalize(v.Rotation);
            return v;
        }

        public NewBlendableTransform Normalized()
        {
            return Normalize(this);
        }

        //public static NewBlendableTransform operator *(NewBlendableTransform a, float b)
        //{
        //    return new NewBlendableTransform()
        //    {
        //        Translation = a.Translation * b,
        //        Rotation = new Quaternion(a.Rotation.X * b, a.Rotation.Y * b, a.Rotation.Z * b, a.Rotation.W * b),
        //        Scale = a.Scale * b,
        //    };
        //}

        //public static NewBlendableTransform operator /(NewBlendableTransform a, float b)
        //{
        //    return new NewBlendableTransform()
        //    {
        //        Translation = a.Translation / b,
        //        Rotation = new Quaternion(a.Rotation.X / b, a.Rotation.Y / b, a.Rotation.Z / b, a.Rotation.W / b),
        //        Scale = a.Scale / b,
        //    };
        //}

        public static NewBlendableTransform operator *(NewBlendableTransform a, NewBlendableTransform b)
        {
            return new NewBlendableTransform()
            {
                Translation = a.Translation + b.Translation,
                Rotation = a.Rotation * b.Rotation,
                //Rotation = Quaternion.CreateFromRotationMatrix(Matrix4x4.CreateFromQuaternion(a.Rotation) * Matrix4x4.CreateFromQuaternion(b.Rotation)),
                Scale = a.Scale * b.Scale,
            };
        }

        //public static NewBlendableTransform operator /(NewBlendableTransform a, NewBlendableTransform b)
        //{
        //    return new NewBlendableTransform()
        //    {
        //        Translation = a.Translation - b.Translation,
        //        Rotation = a.Rotation / b.Rotation,
        //        Scale = a.Scale / b.Scale,
        //    };
        //}

        //public static NewBlendableTransform operator +(NewBlendableTransform a, NewBlendableTransform b)
        //{
        //    return new NewBlendableTransform()
        //    {
        //        Translation = a.Translation + b.Translation,
        //        Rotation = a.Rotation + b.Rotation,
        //        Scale = a.Scale + b.Scale,
        //    };
        //}

        public NewBlendableTransform(Matrix4x4 matrix) : this()
        {
            if (!Matrix4x4.Decompose(matrix, out Scale, out Rotation, out Translation))
            {
                var ex = new ArgumentException($"{nameof(matrix)} can't be decomposed", nameof(matrix));
                ex.Data.Add("matrix", matrix);
                throw ex;
            }
        }

        public static NewBlendableTransform Identity => new NewBlendableTransform()
        {
            Translation = Vector3.Zero,
            Rotation = Quaternion.Identity,
            Scale = Vector3.One,
        };

        public static NewBlendableTransform Zero => new NewBlendableTransform()
        {
            Translation = Vector3.Zero,
            Rotation = new Quaternion(0, 0, 0, 0),
            Scale = Vector3.Zero,
        };

        public static NewBlendableTransform Lerp(NewBlendableTransform a, NewBlendableTransform b, float s)
        {
            return new NewBlendableTransform()
            {
                Translation = Vector3.Lerp(a.Translation, b.Translation, s),
                Scale = Vector3.Lerp(a.Scale, b.Scale, s),
                Rotation = Quaternion.Slerp(a.Rotation, b.Rotation, s),
            };
        }

        public Matrix4x4 GetMatrixScale()
        {
            return Matrix4x4.CreateScale(Scale);
        }

        public Matrix4x4 GetMatrix()
        {
            return

                Matrix4x4.CreateFromQuaternion(Quaternion.Normalize(Rotation)) *
                //Matrix4x4.CreateFromQuaternion(Rotation) *
                Matrix4x4.CreateTranslation(Translation);
        }

        public Matrix4x4 GetMatrixUnnormalized()
        {
            return
                Matrix4x4.CreateFromQuaternion(Rotation) *
                //Matrix4x4.CreateFromQuaternion(Rotation) *
                Matrix4x4.CreateTranslation(Translation);
        }

        public Matrix4x4 GetDsasCamViewMatrix()
        {
            return
                Matrix4x4.CreateTranslation(-Translation) *
                Matrix4x4.CreateFromQuaternion(Quaternion.Normalize(Rotation))
                //Matrix4x4.CreateFromQuaternion(Rotation) *
                ;
        }

        public float GetWrappedYawAngle()
        {
            Vector3 direction = Vector3.Transform(Vector3.UnitX, Matrix4x4.CreateFromQuaternion(Rotation));
            return (float)Math.Atan2(-direction.Z, direction.X);
        }

        public Vector4 GetRootMotionVector4()
        {
            return new Vector4(Translation.X, Translation.Y, Translation.Z, GetWrappedYawAngle());
        }
    }

}
