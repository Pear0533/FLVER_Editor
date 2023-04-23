using System;

namespace FLVER_Editor

{
    //we use -> order, the reverse order of MAYA expression, the front one tis the Parent
    public enum RotationOrder
    {
        XYZ,
        XZY,
        YXZ,
        YZX,
        ZXY,
        ZYX
    }

    internal class Transform3D
    {
        public string Name = "";

        public Transform3D Parent = null;
        public Vector3D Position = new Vector3D();

        //Rotation unit: degree
        public Vector3D Rotation = new Vector3D();

        public RotationOrder RotationOrder = RotationOrder.YZX;

        public Vector3D Scale = new Vector3D(1, 1, 1);

        public Vector3D[] VectorList;

        public Vector3D[] GetGlobalVectorList()
        {
            var ans = new Vector3D[VectorList.Length];
            var transformMatrix = new Matrix4x4();
            {
                var rs = Matrix4x4.GenerateScaleMatrix4x4(Scale);
                var rx = Matrix4x4.GenerateRotationXMatrix4x4(Rotation.X);
                var ry = Matrix4x4.GenerateRotationYMatrix4x4(Rotation.Y);
                var rz = Matrix4x4.GenerateRotationZMatrix4x4(Rotation.Z);
                var pos = Matrix4x4.GenerateTranslationMatrix4x4(Position.X, Position.Y, Position.Z);
                if (RotationOrder == RotationOrder.XYZ)
                {
                    transformMatrix = pos * (rx * (ry * (rz * rs)));
                }
                if (RotationOrder == RotationOrder.XZY)
                {
                    transformMatrix = pos * (rx * (rz * (ry * rs)));
                }
                if (RotationOrder == RotationOrder.YXZ)
                {
                    transformMatrix = pos * (ry * (rx * (rz * rs)));
                }
                if (RotationOrder == RotationOrder.YZX)
                {
                    transformMatrix = pos * (ry * (rz * (rx * rs)));
                }
                if (RotationOrder == RotationOrder.ZXY)
                {
                    transformMatrix = pos * (rz * (rx * (ry * rs)));
                }
                if (RotationOrder == RotationOrder.ZYX)
                {
                    transformMatrix = pos * (rz * (ry * (rx * rs)));
                }
            }
            Transform3D parentTransform = Parent;
            while (parentTransform != null)
            {
                var rx = Matrix4x4.GenerateRotationXMatrix4x4(parentTransform.Rotation.X);
                var ry = Matrix4x4.GenerateRotationYMatrix4x4(parentTransform.Rotation.Y);
                var rz = Matrix4x4.GenerateRotationZMatrix4x4(parentTransform.Rotation.Z);
                var pos = Matrix4x4.GenerateTranslationMatrix4x4(parentTransform.Position.X, parentTransform.Position.Y, parentTransform.Position.Z);
                transformMatrix = Matrix4x4.GenerateScaleMatrix4x4(Scale.X, Scale.Y, Scale.Z) * transformMatrix;
                if (RotationOrder == RotationOrder.XYZ)
                {
                    transformMatrix = pos * (rx * (ry * (rz * transformMatrix)));
                }
                if (RotationOrder == RotationOrder.XZY)
                {
                    transformMatrix = pos * (rx * (rz * (ry * transformMatrix)));
                }
                if (RotationOrder == RotationOrder.YXZ)
                {
                    transformMatrix = pos * (ry * (rx * (rz * transformMatrix)));
                }
                if (RotationOrder == RotationOrder.YZX)
                {
                    transformMatrix = pos * (ry * (rz * (rx * transformMatrix)));
                }
                if (RotationOrder == RotationOrder.ZXY)
                {
                    transformMatrix = pos * (rz * (rx * (ry * transformMatrix)));
                }
                if (RotationOrder == RotationOrder.ZYX)
                {
                    transformMatrix = pos * (rz * (ry * (rx * transformMatrix)));
                }
                if (Parent.Parent == null)
                {
                    break;
                }
                // transformMatrix = pos * (rx * (ry * (rz * transformMatrix)));
                parentTransform = parentTransform.Parent;
            }
            for (var i = 0; i < VectorList.Length; i++)
            {
                ans[i] = VectorList[i].Clone();

                // Console.WriteLine("Old X " + ans[i].X + " Y " + ans[i].Y + " Z " + ans[i].Z);
                ans[i] = Matrix4x4.Matrix4x4TimesVector3D(transformMatrix, ans[i]);
                // Console.WriteLine("New X " + ans[i].X + " Y " + ans[i].Y + " Z " + ans[i].Z);
            }
            return ans;
        }

        public Vector3D GetGlobalOrigin()
        {
            var ans = new Vector3D();
            var org = new Vector3D();
            var transMatrix = new Matrix4x4();
            {
                var rs = Matrix4x4.GenerateScaleMatrix4x4(Scale.X, Scale.Y, Scale.Z);
                var rx = Matrix4x4.GenerateRotationXMatrix4x4(Rotation.X);
                var ry = Matrix4x4.GenerateRotationYMatrix4x4(Rotation.Y);
                var rz = Matrix4x4.GenerateRotationZMatrix4x4(Rotation.Z);
                var pos = Matrix4x4.GenerateTranslationMatrix4x4(Position.X, Position.Y, Position.Z);
                if (RotationOrder == RotationOrder.XYZ)
                {
                    transMatrix = pos * (rx * (ry * (rz * rs)));
                }
                if (RotationOrder == RotationOrder.XZY)
                {
                    transMatrix = pos * (rx * (rz * (ry * rs)));
                }
                if (RotationOrder == RotationOrder.YXZ)
                {
                    transMatrix = pos * (ry * (rx * (rz * rs)));
                }
                if (RotationOrder == RotationOrder.YZX)
                {
                    transMatrix = pos * (ry * (rz * (rx * rs)));
                }
                if (RotationOrder == RotationOrder.ZXY)
                {
                    transMatrix = pos * (rz * (rx * (ry * rs)));
                }
                if (RotationOrder == RotationOrder.ZYX)
                {
                    transMatrix = pos * (rz * (ry * (rx * rs)));
                }
            }
            Transform3D parentTransform = null;
            parentTransform = Parent;
            while (parentTransform != null)
            {
                var rx = Matrix4x4.GenerateRotationXMatrix4x4(parentTransform.Rotation.X);
                var ry = Matrix4x4.GenerateRotationYMatrix4x4(parentTransform.Rotation.Y);
                var rz = Matrix4x4.GenerateRotationZMatrix4x4(parentTransform.Rotation.Z);
                var pos = Matrix4x4.GenerateTranslationMatrix4x4(parentTransform.Position.X, parentTransform.Position.Y, parentTransform.Position.Z);
                transMatrix = Matrix4x4.GenerateScaleMatrix4x4(parentTransform.Scale.X, parentTransform.Scale.Y, parentTransform.Scale.Z) * transMatrix;
                if (RotationOrder == RotationOrder.XYZ)
                {
                    transMatrix = pos * (rx * (ry * (rz * transMatrix)));
                }
                if (RotationOrder == RotationOrder.XZY)
                {
                    transMatrix = pos * (rx * (rz * (ry * transMatrix)));
                }
                if (RotationOrder == RotationOrder.YXZ)
                {
                    transMatrix = pos * (ry * (rx * (rz * transMatrix)));
                }
                if (RotationOrder == RotationOrder.YZX)
                {
                    transMatrix = pos * (ry * (rz * (rx * transMatrix)));
                }
                if (RotationOrder == RotationOrder.ZXY)
                {
                    transMatrix = pos * (rz * (rx * (ry * transMatrix)));
                }
                if (RotationOrder == RotationOrder.ZYX)
                {
                    transMatrix = pos * (rz * (ry * (rx * transMatrix)));
                }
                if (Parent.Parent == null)
                {
                    break;
                }
                // transformMatrix = pos * (rx * (ry * (rz * transformMatrix)));
                parentTransform = parentTransform.Parent;
            }

            //ans= org.clone();
            ans = Matrix4x4.Matrix4x4TimesVector3D(transMatrix, org);
            return ans;
        }

        public void SetRotationInRadians(Vector3D vector)
        {
            Rotation.X = (float)(vector.X / Math.PI * 180);
            Rotation.Y = (float)(vector.Y / Math.PI * 180);
            Rotation.Z = (float)(vector.Z / Math.PI * 180);
        }

        public Vector3D[] GetRotationCircleX()
        {
            var ans = new Vector3D[8];

            //This is for Y circle
            /*ans[0] = new Vector3D(2,0,0);
            ans[1] = new Vector3D(0, 0, 2);
            ans[2] = new Vector3D(-2, 0, 0);
            ans[3] = new Vector3D(0, 0, -2);*/
            ans[0] = new Vector3D(0, 2, 0);
            ans[1] = new Vector3D(0, 1.5f, 1.5f);
            ans[2] = new Vector3D(0, 0, 2);
            ans[3] = new Vector3D(0, -1.5f, 1.5f);
            ans[4] = new Vector3D(0, -2, 0);
            ans[5] = new Vector3D(0, -1.5f, -1.5f);
            ans[6] = new Vector3D(0, 0, -2);
            ans[7] = new Vector3D(0, 1.5f, -1.5f);
            var factor = 1f;
            if (RotationOrder == RotationOrder.XYZ)
            {
                factor = 1;
            }
            if (RotationOrder == RotationOrder.XZY)
            {
                factor = 1;
            }
            if (RotationOrder == RotationOrder.YXZ)
            {
                factor = 0.85f;
            }
            if (RotationOrder == RotationOrder.YZX)
            {
                factor = 0.7f;
            }
            if (RotationOrder == RotationOrder.ZXY)
            {
                factor = 0.85f;
            }
            if (RotationOrder == RotationOrder.ZYX)
            {
                factor = 0.7f;
            }
            foreach (Vector3D v in ans)
            {
                v.X *= factor;
                v.Y *= factor;
                v.Z *= factor;
            }
            var transformMatrix = new Matrix4x4();
            {
                var rx = Matrix4x4.GenerateRotationXMatrix4x4(Rotation.X);
                var ry = Matrix4x4.GenerateRotationYMatrix4x4(Rotation.Y);
                var rz = Matrix4x4.GenerateRotationZMatrix4x4(Rotation.Z);
                var pos = Matrix4x4.GenerateTranslationMatrix4x4(Position.X, Position.Y, Position.Z);

                //   transformMatrix = pos * (rx * (ry * rz));
                //transformMatrix = pos * (rx );
                if (RotationOrder == RotationOrder.XYZ)
                {
                    transformMatrix = pos * rx;
                }
                if (RotationOrder == RotationOrder.XZY)
                {
                    transformMatrix = pos * rx;
                }
                if (RotationOrder == RotationOrder.YXZ)
                {
                    transformMatrix = pos * (ry * rx);
                }
                if (RotationOrder == RotationOrder.YZX)
                {
                    transformMatrix = pos * (ry * (rz * rx));
                }
                if (RotationOrder == RotationOrder.ZXY)
                {
                    transformMatrix = pos * (rz * rx);
                }
                if (RotationOrder == RotationOrder.ZYX)
                {
                    transformMatrix = pos * (rz * (ry * rx));
                }
            }
            Transform3D parentTransform = null;
            parentTransform = Parent;
            while (parentTransform != null)
            {
                var rx = Matrix4x4.GenerateRotationXMatrix4x4(parentTransform.Rotation.X);
                var ry = Matrix4x4.GenerateRotationYMatrix4x4(parentTransform.Rotation.Y);
                var rz = Matrix4x4.GenerateRotationZMatrix4x4(parentTransform.Rotation.Z);
                var pos = Matrix4x4.GenerateTranslationMatrix4x4(parentTransform.Position.X, parentTransform.Position.Y, parentTransform.Position.Z);
                transformMatrix = pos * (rx * (ry * (rz * transformMatrix)));
                parentTransform = Parent.Parent;
            }
            for (var i = 0; i < ans.Length; i++)
            {
                ans[i] = Matrix4x4.Matrix4x4TimesVector3D(transformMatrix, ans[i]);
            }
            return ans;
        }

        public Vector3D[] GetRotationCircleZ()
        {
            var ans = new Vector3D[8];

            //This is for Y circle
            /*ans[0] = new Vector3D(2,0,0);
            ans[1] = new Vector3D(0, 0, 2);
            ans[2] = new Vector3D(-2, 0, 0);
            ans[3] = new Vector3D(0, 0, -2);*/
            ans[0] = new Vector3D(2, 0, 0);
            ans[1] = new Vector3D(1.5f, 1.5f, 0);
            ans[2] = new Vector3D(0, 2, 0);
            ans[3] = new Vector3D(-1.5f, 1.5f, 0);
            ans[4] = new Vector3D(-2, 0, 0);
            ans[5] = new Vector3D(-1.5f, -1.5f, 0);
            ans[6] = new Vector3D(0, -2, 0);
            ans[7] = new Vector3D(1.5f, -1.5f, 0);
            float factor = 1;
            if (RotationOrder == RotationOrder.XYZ)
            {
                factor = 0.7f;
            }
            if (RotationOrder == RotationOrder.XZY)
            {
                factor = 0.85f;
            }
            if (RotationOrder == RotationOrder.YXZ)
            {
                factor = 0.7f;
            }
            if (RotationOrder == RotationOrder.YZX)
            {
                factor = 0.85f;
            }
            if (RotationOrder == RotationOrder.ZXY)
            {
                factor = 1f;
            }
            if (RotationOrder == RotationOrder.ZYX)
            {
                factor = 1f;
            }
            foreach (Vector3D v in ans)
            {
                v.X *= factor;
                v.Y *= factor;
                v.Z *= factor;
            }
            var transMatrix = new Matrix4x4();
            {
                var rx = Matrix4x4.GenerateRotationXMatrix4x4(Rotation.X);
                var ry = Matrix4x4.GenerateRotationYMatrix4x4(Rotation.Y);
                var rz = Matrix4x4.GenerateRotationZMatrix4x4(Rotation.Z);
                var pos = Matrix4x4.GenerateTranslationMatrix4x4(Position.X, Position.Y, Position.Z);

                //   transformMatrix = pos * (rx * (ry * rz));
                //  transformMatrix = pos * (rx * (ry * rz));
                if (RotationOrder == RotationOrder.XYZ)
                {
                    transMatrix = pos * (rx * (ry * rz));
                }
                if (RotationOrder == RotationOrder.XZY)
                {
                    transMatrix = pos * (rx * rz);
                }
                if (RotationOrder == RotationOrder.YXZ)
                {
                    transMatrix = pos * (ry * (rx * rz));
                }
                if (RotationOrder == RotationOrder.YZX)
                {
                    transMatrix = pos * (ry * rz);
                }
                if (RotationOrder == RotationOrder.ZXY)
                {
                    transMatrix = pos * rz;
                }
                if (RotationOrder == RotationOrder.ZYX)
                {
                    transMatrix = pos * rz;
                }
            }
            Transform3D parentTransform = Parent;
            while (parentTransform != null)
            {
                var rx = Matrix4x4.GenerateRotationXMatrix4x4(parentTransform.Rotation.X);
                var ry = Matrix4x4.GenerateRotationYMatrix4x4(parentTransform.Rotation.Y);
                var rz = Matrix4x4.GenerateRotationZMatrix4x4(parentTransform.Rotation.Z);
                var pos = Matrix4x4.GenerateTranslationMatrix4x4(parentTransform.Position.X, parentTransform.Position.Y, parentTransform.Position.Z);
                transMatrix = pos * (rx * (ry * (rz * transMatrix)));
                parentTransform = Parent.Parent;
            }
            for (var i = 0; i < ans.Length; i++)
            {
                ans[i] = Matrix4x4.Matrix4x4TimesVector3D(transMatrix, ans[i]);
            }
            return ans;
        }

        public Vector3D[] GetRotationCircleY()
        {
            var ans = new Vector3D[8];

            //This is for Y circle
            /*ans[0] = new Vector3D(1.7f,0,0);
            ans[1] = new Vector3D(0, 0, 1.7f);
            ans[2] = new Vector3D(-1.7f, 0, 0);
            ans[3] = new Vector3D(0, 0, -1.7f);
            */
            ans[0] = new Vector3D(2, 0, 0);
            ans[1] = new Vector3D(1.5f, 0, 1.5f);
            ans[2] = new Vector3D(0, 0, 2);
            ans[3] = new Vector3D(-1.5f, 0, 1.5f);
            ans[4] = new Vector3D(-2, 0, 0);
            ans[5] = new Vector3D(-1.5f, 0, -1.5f);
            ans[6] = new Vector3D(0, 0, -2);
            ans[7] = new Vector3D(1.5f, 0, -1.5f);
            float factor = 1;
            if (RotationOrder == RotationOrder.XYZ)
            {
                factor = 0.85f;
            }
            if (RotationOrder == RotationOrder.XZY)
            {
                factor = 0.7f;
            }
            if (RotationOrder == RotationOrder.YXZ)
            {
                factor = 1f;
            }
            if (RotationOrder == RotationOrder.YZX)
            {
                factor = 1f;
            }
            if (RotationOrder == RotationOrder.ZXY)
            {
                factor = 0.7f;
            }
            if (RotationOrder == RotationOrder.ZYX)
            {
                factor = 0.85f;
            }
            foreach (Vector3D v in ans)
            {
                v.X *= factor;
                v.Y *= factor;
                v.Z *= factor;
            }
            var transformMatrix = new Matrix4x4();
            {
                var rx = Matrix4x4.GenerateRotationXMatrix4x4(Rotation.X);
                var ry = Matrix4x4.GenerateRotationYMatrix4x4(Rotation.Y);
                var rz = Matrix4x4.GenerateRotationZMatrix4x4(Rotation.Z);
                var pos = Matrix4x4.GenerateTranslationMatrix4x4(Position.X, Position.Y, Position.Z);

                //   transformMatrix = pos * (rx * (ry * rz));
                //transformMatrix = pos * (rx * ry);
                if (RotationOrder == RotationOrder.XYZ)
                {
                    transformMatrix = pos * (rx * ry);
                }
                if (RotationOrder == RotationOrder.XZY)
                {
                    transformMatrix = pos * (rx * (rz * ry));
                }
                if (RotationOrder == RotationOrder.YXZ)
                {
                    transformMatrix = pos * ry;
                }
                if (RotationOrder == RotationOrder.YZX)
                {
                    transformMatrix = pos * ry;
                }
                if (RotationOrder == RotationOrder.ZXY)
                {
                    transformMatrix = pos * (rz * (rx * ry));
                }
                if (RotationOrder == RotationOrder.ZYX)
                {
                    transformMatrix = pos * (rz * ry);
                }
            }
            Transform3D parentTransform = Parent;
            while (parentTransform != null)
            {
                var rx = Matrix4x4.GenerateRotationXMatrix4x4(parentTransform.Rotation.X);
                var ry = Matrix4x4.GenerateRotationYMatrix4x4(parentTransform.Rotation.Y);
                var rz = Matrix4x4.GenerateRotationZMatrix4x4(parentTransform.Rotation.Z);
                var pos = Matrix4x4.GenerateTranslationMatrix4x4(parentTransform.Position.X, parentTransform.Position.Y, parentTransform.Position.Z);
                transformMatrix = pos * (rx * (ry * (rz * transformMatrix)));
                parentTransform = Parent.Parent;
            }
            for (var i = 0; i < ans.Length; i++)
            {
                ans[i] = Matrix4x4.Matrix4x4TimesVector3D(transformMatrix, ans[i]);
            }
            return ans;
        }
    }
}