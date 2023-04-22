using System;

namespace FLVER_Editor

{
    //we use -> order, the reverse order of MAYA expression, the front one tis the parent
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
        public string name = "";

        public Transform3D parent = null;
        public Vector3D position = new Vector3D();

        //rotation unit: degree
        public Vector3D rotation = new Vector3D();

        public RotationOrder rotOrder = RotationOrder.YZX;

        public Vector3D scale = new Vector3D(1, 1, 1);

        public Vector3D[] vlist;

        public Vector3D[] getGlobalVlist()
        {
            var ans = new Vector3D[vlist.Length];
            var transMatrix = new Matrix4x4();
            {
                var rs = Matrix4x4.GenerateScaleMatrix4x4(scale);
                var rx = Matrix4x4.GenerateRotationXMatrix4x4(rotation.X);
                var ry = Matrix4x4.GenerateRotationYMatrix4x4(rotation.Y);
                var rz = Matrix4x4.GenerateRotationZMatrix4x4(rotation.Z);
                var pos = Matrix4x4.GenerateTranslationMatrix4x4(position.X, position.Y, position.Z);
                if (rotOrder == RotationOrder.XYZ)
                {
                    transMatrix = pos * (rx * (ry * (rz * rs)));
                }
                if (rotOrder == RotationOrder.XZY)
                {
                    transMatrix = pos * (rx * (rz * (ry * rs)));
                }
                if (rotOrder == RotationOrder.YXZ)
                {
                    transMatrix = pos * (ry * (rx * (rz * rs)));
                }
                if (rotOrder == RotationOrder.YZX)
                {
                    transMatrix = pos * (ry * (rz * (rx * rs)));
                }
                if (rotOrder == RotationOrder.ZXY)
                {
                    transMatrix = pos * (rz * (rx * (ry * rs)));
                }
                if (rotOrder == RotationOrder.ZYX)
                {
                    transMatrix = pos * (rz * (ry * (rx * rs)));
                }
            }
            Transform3D parentT = null;
            parentT = parent;
            while (parentT != null)
            {
                var rx = Matrix4x4.GenerateRotationXMatrix4x4(parentT.rotation.X);
                var ry = Matrix4x4.GenerateRotationYMatrix4x4(parentT.rotation.Y);
                var rz = Matrix4x4.GenerateRotationZMatrix4x4(parentT.rotation.Z);
                var pos = Matrix4x4.GenerateTranslationMatrix4x4(parentT.position.X, parentT.position.Y, parentT.position.Z);
                transMatrix = Matrix4x4.GenerateScaleMatrix4x4(scale.X, scale.Y, scale.Z) * transMatrix;
                if (rotOrder == RotationOrder.XYZ)
                {
                    transMatrix = pos * (rx * (ry * (rz * transMatrix)));
                }
                if (rotOrder == RotationOrder.XZY)
                {
                    transMatrix = pos * (rx * (rz * (ry * transMatrix)));
                }
                if (rotOrder == RotationOrder.YXZ)
                {
                    transMatrix = pos * (ry * (rx * (rz * transMatrix)));
                }
                if (rotOrder == RotationOrder.YZX)
                {
                    transMatrix = pos * (ry * (rz * (rx * transMatrix)));
                }
                if (rotOrder == RotationOrder.ZXY)
                {
                    transMatrix = pos * (rz * (rx * (ry * transMatrix)));
                }
                if (rotOrder == RotationOrder.ZYX)
                {
                    transMatrix = pos * (rz * (ry * (rx * transMatrix)));
                }
                if (parent.parent == null)
                {
                    break;
                }
                // transMatrix = pos * (rx * (ry * (rz * transMatrix)));
                parentT = parentT.parent;
            }
            for (var i = 0; i < vlist.Length; i++)
            {
                ans[i] = vlist[i].Clone();

                // Console.WriteLine("Old X " + ans[i].X + " Y " + ans[i].Y + " Z " + ans[i].Z);
                ans[i] = Matrix4x4.Matrix4x4TimesVector3D(transMatrix, ans[i]);
                // Console.WriteLine("New X " + ans[i].X + " Y " + ans[i].Y + " Z " + ans[i].Z);
            }
            return ans;
        }

        public Vector3D getGlobalOrigin()
        {
            var ans = new Vector3D();
            var org = new Vector3D();
            var transMatrix = new Matrix4x4();
            {
                var rs = Matrix4x4.GenerateScaleMatrix4x4(scale.X, scale.Y, scale.Z);
                var rx = Matrix4x4.GenerateRotationXMatrix4x4(rotation.X);
                var ry = Matrix4x4.GenerateRotationYMatrix4x4(rotation.Y);
                var rz = Matrix4x4.GenerateRotationZMatrix4x4(rotation.Z);
                var pos = Matrix4x4.GenerateTranslationMatrix4x4(position.X, position.Y, position.Z);
                if (rotOrder == RotationOrder.XYZ)
                {
                    transMatrix = pos * (rx * (ry * (rz * rs)));
                }
                if (rotOrder == RotationOrder.XZY)
                {
                    transMatrix = pos * (rx * (rz * (ry * rs)));
                }
                if (rotOrder == RotationOrder.YXZ)
                {
                    transMatrix = pos * (ry * (rx * (rz * rs)));
                }
                if (rotOrder == RotationOrder.YZX)
                {
                    transMatrix = pos * (ry * (rz * (rx * rs)));
                }
                if (rotOrder == RotationOrder.ZXY)
                {
                    transMatrix = pos * (rz * (rx * (ry * rs)));
                }
                if (rotOrder == RotationOrder.ZYX)
                {
                    transMatrix = pos * (rz * (ry * (rx * rs)));
                }
            }
            Transform3D parentT = null;
            parentT = parent;
            while (parentT != null)
            {
                var rx = Matrix4x4.GenerateRotationXMatrix4x4(parentT.rotation.X);
                var ry = Matrix4x4.GenerateRotationYMatrix4x4(parentT.rotation.Y);
                var rz = Matrix4x4.GenerateRotationZMatrix4x4(parentT.rotation.Z);
                var pos = Matrix4x4.GenerateTranslationMatrix4x4(parentT.position.X, parentT.position.Y, parentT.position.Z);
                transMatrix = Matrix4x4.GenerateScaleMatrix4x4(parentT.scale.X, parentT.scale.Y, parentT.scale.Z) * transMatrix;
                if (rotOrder == RotationOrder.XYZ)
                {
                    transMatrix = pos * (rx * (ry * (rz * transMatrix)));
                }
                if (rotOrder == RotationOrder.XZY)
                {
                    transMatrix = pos * (rx * (rz * (ry * transMatrix)));
                }
                if (rotOrder == RotationOrder.YXZ)
                {
                    transMatrix = pos * (ry * (rx * (rz * transMatrix)));
                }
                if (rotOrder == RotationOrder.YZX)
                {
                    transMatrix = pos * (ry * (rz * (rx * transMatrix)));
                }
                if (rotOrder == RotationOrder.ZXY)
                {
                    transMatrix = pos * (rz * (rx * (ry * transMatrix)));
                }
                if (rotOrder == RotationOrder.ZYX)
                {
                    transMatrix = pos * (rz * (ry * (rx * transMatrix)));
                }
                if (parent.parent == null)
                {
                    break;
                }
                // transMatrix = pos * (rx * (ry * (rz * transMatrix)));
                parentT = parentT.parent;
            }

            //ans= org.clone();
            ans = Matrix4x4.Matrix4x4TimesVector3D(transMatrix, org);
            return ans;
        }

        public void setRotationInRad(Vector3D v3d)
        {
            rotation.X = (float)(v3d.X / Math.PI * 180);
            rotation.Y = (float)(v3d.Y / Math.PI * 180);
            rotation.Z = (float)(v3d.Z / Math.PI * 180);
        }

        public Vector3D[] getRotCircleX()
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
            if (rotOrder == RotationOrder.XYZ)
            {
                factor = 1;
            }
            if (rotOrder == RotationOrder.XZY)
            {
                factor = 1;
            }
            if (rotOrder == RotationOrder.YXZ)
            {
                factor = 0.85f;
            }
            if (rotOrder == RotationOrder.YZX)
            {
                factor = 0.7f;
            }
            if (rotOrder == RotationOrder.ZXY)
            {
                factor = 0.85f;
            }
            if (rotOrder == RotationOrder.ZYX)
            {
                factor = 0.7f;
            }
            foreach (Vector3D v in ans)
            {
                v.X *= factor;
                v.Y *= factor;
                v.Z *= factor;
            }
            var transMatrix = new Matrix4x4();
            {
                var rx = Matrix4x4.GenerateRotationXMatrix4x4(rotation.X);
                var ry = Matrix4x4.GenerateRotationYMatrix4x4(rotation.Y);
                var rz = Matrix4x4.GenerateRotationZMatrix4x4(rotation.Z);
                var pos = Matrix4x4.GenerateTranslationMatrix4x4(position.X, position.Y, position.Z);

                //   transMatrix = pos * (rx * (ry * rz));
                //transMatrix = pos * (rx );
                if (rotOrder == RotationOrder.XYZ)
                {
                    transMatrix = pos * rx;
                }
                if (rotOrder == RotationOrder.XZY)
                {
                    transMatrix = pos * rx;
                }
                if (rotOrder == RotationOrder.YXZ)
                {
                    transMatrix = pos * (ry * rx);
                }
                if (rotOrder == RotationOrder.YZX)
                {
                    transMatrix = pos * (ry * (rz * rx));
                }
                if (rotOrder == RotationOrder.ZXY)
                {
                    transMatrix = pos * (rz * rx);
                }
                if (rotOrder == RotationOrder.ZYX)
                {
                    transMatrix = pos * (rz * (ry * rx));
                }
            }
            Transform3D parentT = null;
            parentT = parent;
            while (parentT != null)
            {
                var rx = Matrix4x4.GenerateRotationXMatrix4x4(parentT.rotation.X);
                var ry = Matrix4x4.GenerateRotationYMatrix4x4(parentT.rotation.Y);
                var rz = Matrix4x4.GenerateRotationZMatrix4x4(parentT.rotation.Z);
                var pos = Matrix4x4.GenerateTranslationMatrix4x4(parentT.position.X, parentT.position.Y, parentT.position.Z);
                transMatrix = pos * (rx * (ry * (rz * transMatrix)));
                parentT = parent.parent;
            }
            for (var i = 0; i < ans.Length; i++)
            {
                ans[i] = Matrix4x4.Matrix4x4TimesVector3D(transMatrix, ans[i]);
            }
            return ans;
        }

        public Vector3D[] getRotCircleZ()
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
            if (rotOrder == RotationOrder.XYZ)
            {
                factor = 0.7f;
            }
            if (rotOrder == RotationOrder.XZY)
            {
                factor = 0.85f;
            }
            if (rotOrder == RotationOrder.YXZ)
            {
                factor = 0.7f;
            }
            if (rotOrder == RotationOrder.YZX)
            {
                factor = 0.85f;
            }
            if (rotOrder == RotationOrder.ZXY)
            {
                factor = 1f;
            }
            if (rotOrder == RotationOrder.ZYX)
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
                var rx = Matrix4x4.GenerateRotationXMatrix4x4(rotation.X);
                var ry = Matrix4x4.GenerateRotationYMatrix4x4(rotation.Y);
                var rz = Matrix4x4.GenerateRotationZMatrix4x4(rotation.Z);
                var pos = Matrix4x4.GenerateTranslationMatrix4x4(position.X, position.Y, position.Z);

                //   transMatrix = pos * (rx * (ry * rz));
                //  transMatrix = pos * (rx * (ry * rz));
                if (rotOrder == RotationOrder.XYZ)
                {
                    transMatrix = pos * (rx * (ry * rz));
                }
                if (rotOrder == RotationOrder.XZY)
                {
                    transMatrix = pos * (rx * rz);
                }
                if (rotOrder == RotationOrder.YXZ)
                {
                    transMatrix = pos * (ry * (rx * rz));
                }
                if (rotOrder == RotationOrder.YZX)
                {
                    transMatrix = pos * (ry * rz);
                }
                if (rotOrder == RotationOrder.ZXY)
                {
                    transMatrix = pos * rz;
                }
                if (rotOrder == RotationOrder.ZYX)
                {
                    transMatrix = pos * rz;
                }
            }
            Transform3D parentT = null;
            parentT = parent;
            while (parentT != null)
            {
                var rx = Matrix4x4.GenerateRotationXMatrix4x4(parentT.rotation.X);
                var ry = Matrix4x4.GenerateRotationYMatrix4x4(parentT.rotation.Y);
                var rz = Matrix4x4.GenerateRotationZMatrix4x4(parentT.rotation.Z);
                var pos = Matrix4x4.GenerateTranslationMatrix4x4(parentT.position.X, parentT.position.Y, parentT.position.Z);
                transMatrix = pos * (rx * (ry * (rz * transMatrix)));
                parentT = parent.parent;
            }
            for (var i = 0; i < ans.Length; i++)
            {
                ans[i] = Matrix4x4.Matrix4x4TimesVector3D(transMatrix, ans[i]);
            }
            return ans;
        }

        public Vector3D[] getRotCircleY()
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
            if (rotOrder == RotationOrder.XYZ)
            {
                factor = 0.85f;
            }
            if (rotOrder == RotationOrder.XZY)
            {
                factor = 0.7f;
            }
            if (rotOrder == RotationOrder.YXZ)
            {
                factor = 1f;
            }
            if (rotOrder == RotationOrder.YZX)
            {
                factor = 1f;
            }
            if (rotOrder == RotationOrder.ZXY)
            {
                factor = 0.7f;
            }
            if (rotOrder == RotationOrder.ZYX)
            {
                factor = 0.85f;
            }
            foreach (Vector3D v in ans)
            {
                v.X *= factor;
                v.Y *= factor;
                v.Z *= factor;
            }
            var transMatrix = new Matrix4x4();
            {
                var rx = Matrix4x4.GenerateRotationXMatrix4x4(rotation.X);
                var ry = Matrix4x4.GenerateRotationYMatrix4x4(rotation.Y);
                var rz = Matrix4x4.GenerateRotationZMatrix4x4(rotation.Z);
                var pos = Matrix4x4.GenerateTranslationMatrix4x4(position.X, position.Y, position.Z);

                //   transMatrix = pos * (rx * (ry * rz));
                //transMatrix = pos * (rx * ry);
                if (rotOrder == RotationOrder.XYZ)
                {
                    transMatrix = pos * (rx * ry);
                }
                if (rotOrder == RotationOrder.XZY)
                {
                    transMatrix = pos * (rx * (rz * ry));
                }
                if (rotOrder == RotationOrder.YXZ)
                {
                    transMatrix = pos * ry;
                }
                if (rotOrder == RotationOrder.YZX)
                {
                    transMatrix = pos * ry;
                }
                if (rotOrder == RotationOrder.ZXY)
                {
                    transMatrix = pos * (rz * (rx * ry));
                }
                if (rotOrder == RotationOrder.ZYX)
                {
                    transMatrix = pos * (rz * ry);
                }
            }
            Transform3D parentT = null;
            parentT = parent;
            while (parentT != null)
            {
                var rx = Matrix4x4.GenerateRotationXMatrix4x4(parentT.rotation.X);
                var ry = Matrix4x4.GenerateRotationYMatrix4x4(parentT.rotation.Y);
                var rz = Matrix4x4.GenerateRotationZMatrix4x4(parentT.rotation.Z);
                var pos = Matrix4x4.GenerateTranslationMatrix4x4(parentT.position.X, parentT.position.Y, parentT.position.Z);
                transMatrix = pos * (rx * (ry * (rz * transMatrix)));
                parentT = parent.parent;
            }
            for (var i = 0; i < ans.Length; i++)
            {
                ans[i] = Matrix4x4.Matrix4x4TimesVector3D(transMatrix, ans[i]);
            }
            return ans;
        }
    }
}
