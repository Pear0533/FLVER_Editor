using System;
using Microsoft.Xna.Framework;

namespace FLVER_Editor
{
    public class Vector3D
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3D()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

        public Vector3D(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3D(Vector3 a)
        {
            X = a.X;
            Y = a.Y;
            Z = a.Z;
        }

        public Vector3D(System.Numerics.Vector3 a)
        {
            X = a.X;
            Y = a.Y;
            Z = a.Z;
        }

        public Vector3 toXnaV3()
        {
            return new Vector3(X, Y, Z);
        }

        public System.Numerics.Vector3 toNumV3()
        {
            return new System.Numerics.Vector3(X, Y, Z);
        }

        public static float dotProduct(Vector3D a, Vector3D b)
        {
            float x1 = a.X;
            float y1 = a.Y;
            float z1 = a.Z;
            float x2 = b.X;
            float y2 = b.Y;
            float z2 = b.Z;
            return x1 * x2 + y1 * y2 + z1 * z2;
        }

        public static Vector3D crossPorduct(Vector3D a, Vector3D b)
        {
            float x1 = a.X;
            float y1 = a.Y;
            float z1 = a.Z;
            float x2 = b.X;
            float y2 = b.Y;
            float z2 = b.Z;
            return new Vector3D(y1 * z2 - z1 * y2, z1 * x2 - x1 * z2, x1 * y2 - y1 * x2);
        }

        public float length()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public Vector3D normalize()
        {
            float l = length();
            if (l == 0)
            {
                return new Vector3D();
            }
            return new Vector3D(X / l, Y / l, Z / l);
        }

        public Vector3D clone()
        {
            return new Vector3D(X, Y, Z);
        }

        public static Vector3D operator +(Vector3D a,
            Vector3D b)
        {
            return new Vector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
            ;
        }

        public static Vector3D operator -(Vector3D a,
            Vector3D b)
        {
            return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
            ;
        }

        public static Vector3D operator *(Vector3D a,
            float b)
        {
            return new Vector3D(a.X * b, a.Y * b, a.Z * b);
            ;
        }

        public static Vector3D operator *(
            float b, Vector3D a)
        {
            return new Vector3D(a.X * b, a.Y * b, a.Z * b);
            ;
        }

        public static float calculateDistanceFromLine(Vector3D point, Vector3D x1, Vector3D x2)
        {
            return crossPorduct(point - x1, point - x2).length() / (x2 - x1).length();
        }

        public static System.Numerics.Vector3 RotateLine(System.Numerics.Vector3 p, System.Numerics.Vector3 org, System.Numerics.Vector3 direction, double theta)
        {
            double x = p.X;
            double y = p.Y;
            double z = p.Z;
            double a = org.X;
            double b = org.Y;
            double c = org.Z;
            double nu = direction.X / direction.Length();
            double nv = direction.Y / direction.Length();
            double nw = direction.Z / direction.Length();
            var rP = new double[3];
            rP[0] = (a * (nv * nv + nw * nw) - nu * (b * nv + c * nw - nu * x - nv * y - nw * z)) * (1 - Math.Cos(theta))
                + x * Math.Cos(theta)
                + (-c * nv + b * nw - nw * y + nv * z) * Math.Sin(theta);
            rP[1] = (b * (nu * nu + nw * nw) - nv * (a * nu + c * nw - nu * x - nv * y - nw * z)) * (1 - Math.Cos(theta))
                + y * Math.Cos(theta)
                + (c * nu - a * nw + nw * x - nu * z) * Math.Sin(theta);
            rP[2] = (c * (nu * nu + nv * nv) - nw * (a * nu + b * nv - nu * x - nv * y - nw * z)) * (1 - Math.Cos(theta))
                + z * Math.Cos(theta)
                + (-b * nu + a * nv - nv * x + nu * y) * Math.Sin(theta);
            var ans = new System.Numerics.Vector3((float)rP[0], (float)rP[1], (float)rP[2]);
            return ans;
        }

        public static System.Numerics.Vector3 RotatePoint(System.Numerics.Vector3 p, float pitch, float roll, float yaw)
        {
            var ans = new System.Numerics.Vector3(0, 0, 0);
            double cosa = Math.Cos(yaw);
            double sina = Math.Sin(yaw);
            double cosb = Math.Cos(pitch);
            double sinb = Math.Sin(pitch);
            double cosc = Math.Cos(roll);
            double sinc = Math.Sin(roll);
            double Axx = cosa * cosb;
            double Axy = cosa * sinb * sinc - sina * cosc;
            double Axz = cosa * sinb * cosc + sina * sinc;
            double Ayx = sina * cosb;
            double Ayy = sina * sinb * sinc + cosa * cosc;
            double Ayz = sina * sinb * cosc - cosa * sinc;
            double Azx = -sinb;
            double Azy = cosb * sinc;
            double Azz = cosb * cosc;
            float px = p.X;
            float py = p.Y;
            float pz = p.Z;
            ans.X = (float)(Axx * px + Axy * py + Axz * pz);
            ans.Y = (float)(Ayx * px + Ayy * py + Ayz * pz);
            ans.Z = (float)(Azx * px + Azy * py + Azz * pz);
            return ans;
        }
    }
}