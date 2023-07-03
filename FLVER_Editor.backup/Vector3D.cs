using System;
using Microsoft.Xna.Framework;

namespace FLVER_Editor
{
    /// <summary>
    /// A class made for FLVER_Editor to act as a middle man between all the Vector3 types
    /// </summary>
    public class Vector3D
    {
        public float X;
        public float Y;
        public float Z;

        /// <summary>
        /// Create a new FE Vector3D from no values and have X, Y, and Z set to 0
        /// </summary>
        public Vector3D()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

        /// <summary>
        /// Create a new FE Vector3D from X, Y, and Z values as floats
        /// </summary>
        /// <param name="x">A float representing the X of the new FE Vector3D</param>
        /// <param name="y">A float representing the Y of the new FE Vector3D</param>
        /// <param name="z">A float representing the Z of the new FE Vector3D</param>
        public Vector3D(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Create a new FE Vector3D from an Xna Vector4, discarding W
        /// </summary>
        /// <param name="vector">An Xna Vector4</param>
        public Vector3D(Vector4 vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        /// <summary>
        /// Create a new FE Vector3D from an Xna Vector3
        /// </summary>
        /// <param name="vector">An Xna Vector3</param>
        public Vector3D(Vector3 vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        /// <summary>
        /// Create a new FE Vector3D from an Xna Vector2 and a given Z value as a float
        /// </summary>
        /// <param name="vector">An Xna Vector2</param>
        /// <param name="z">A float representing the Z of the new FE Vector3D</param>
        public Vector3D(Vector2 vector, float z)
        {
            X = vector.X;
            Y = vector.Y;
            Z = z;
        }

        /// <summary>
        /// Create a new FE Vector3D from a numerics Vector4, discarding W
        /// </summary>
        /// <param name="vector">A numerics Vector4</param>
        public Vector3D(System.Numerics.Vector4 vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        /// <summary>
        /// Create a new FE Vector3D from a numerics Vector3
        /// </summary>
        /// <param name="vector">A numerics Vector3</param>
        public Vector3D(System.Numerics.Vector3 vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        /// <summary>
        /// Create a new FE Vector3D from a numerics Vector2 and a given Z value as a float
        /// </summary>
        /// <param name="vector">A numerics Vector2</param>
        /// <param name="z">A float representing the Z of the new FE Vector3D</param>
        public Vector3D(System.Numerics.Vector2 vector, float z)
        {
            X = vector.X;
            Y = vector.Y;
            Z = z;
        }

        /// <summary>
        /// Create a new FE Vector3D from an Assimp Vector3D
        /// </summary>
        /// <param name="vector">An Assimp Vector3D</param>
        public Vector3D(Assimp.Vector3D vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        /// <summary>
        /// Create a new FE Vector3D from an Assimp Vector2D and a given Z value as a float
        /// </summary>
        /// <param name="vector">An Assimp Vector2D</param>
        /// <param name="z">A float representing the Z of the new FE Vector3D</param>
        public Vector3D(Assimp.Vector2D vector, float z)
        {
            X = vector.X;
            Y = vector.Y;
            Z = z;
        }

        /// <summary>
        /// Create a new FE Vector3D from an FE Vector4D, discarding W
        /// </summary>
        /// <param name="vector">An FE Vector4D</param>
        public Vector3D(Vector4D vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        /// <summary>
        /// Create a new FE Vector3D from an FE Vector2D
        /// </summary>
        /// <param name="vector">An FE Vector2D</param>
        /// <param name="z">A float representing the Z of the new FE Vector3D</param>
        public Vector3D(Vector2D vector, float z)
        {
            X = vector.X;
            Y = vector.Y;
            Z = z;
        }

        /// <summary>
        /// Converts a FE Vector3D to an Xna Vector4, adding 0 as the W value
        /// </summary>
        /// <returns>An Xna Vector4</returns>
        public Vector4 ToXnaVector4()
        {
            return new Vector4(X, Y, Z, 0);
        }

        /// <summary>
        /// Converts a FE Vector3D to an Xna Vector3
        /// </summary>
        /// <returns>An Xna Vector3</returns>
        public Vector3 ToXnaVector3()
        {
            return new Vector3(X, Y, Z);
        }

        /// <summary>
        /// Converts a FE Vector3D to an Xna Vector2, discarding Z
        /// </summary>
        /// <returns>An Xna Vector2</returns>
        public Vector2 ToXnaVector2()
        {
            return new Vector2(X, Y);
        }

        /// <summary>
        /// Converts a FE Vector3D to a numerics Vector4, adding 0 as the W value
        /// </summary>
        /// <returns>A numerics Vector4</returns>
        public System.Numerics.Vector4 ToNumericsVector4()
        {
            return new System.Numerics.Vector4(X, Y, Z, 0);
        }

        /// <summary>
        /// Converts a FE Vector3D to a numerics Vector3
        /// </summary>
        /// <returns>A numerics Vector3</returns>
        public System.Numerics.Vector3 ToNumericsVector3()
        {
            return new System.Numerics.Vector3(X, Y, Z);
        }

        /// <summary>
        /// Converts a FE Vector3D to an numerics Vector2, discarding Z
        /// </summary>
        /// <returns>A numerics Vector2</returns>
        public System.Numerics.Vector2 ToNumericsVector2()
        {
            return new System.Numerics.Vector2(X, Y);
        }

        /// <summary>
        /// Converts a FE Vector3D to an Assimp Vector3D
        /// </summary>
        /// <returns>An Xna Vector3</returns>
        public Assimp.Vector3D ToAssimpVector3D()
        {
            return new Assimp.Vector3D(X, Y, Z);
        }

        /// <summary>
        /// Converts a FE Vector3D to an Assimp Vector2D, discarding Z
        /// </summary>
        /// <returns>An Assimp Vector2D</returns>
        public Assimp.Vector2D ToAssimpVector2D()
        {
            return new Assimp.Vector2D(X, Y);
        }

        /// <summary>
        /// Converts a FE Vector3D to an FE Vector4D, adding 0 as the W value
        /// </summary>
        /// <returns>An FE Vector2D</returns>
        public Vector4D ToVector4D()
        {
            return new Vector4D(X, Y, Z, 0);
        }

        /// <summary>
        /// Converts a FE Vector3D to an FE Vector3D, discarding Z
        /// </summary>
        /// <returns>An FE Vector2D</returns>
        public Vector2D ToVector2D()
        {
            return new Vector2D(X, Y);
        }

        /// <summary>
        /// Flips the X and Z of the chosen FE Vector3D
        /// </summary>
        /// <param name="vector">An FE Vector3D to flip</param>
        /// <returns>An FE Vector3D with X and Z flipped</returns>
        public static Vector3D FlipXZ(Vector3D vector)
        {
            return new Vector3D(vector.Z, vector.Y, vector.X);
        }

        /// <summary>
        /// Flips the X and Y of the chosen FE Vector3D
        /// </summary>
        /// <param name="vector">An FE Vector3D to flip</param>
        /// <returns>An FE Vector3D with X and Y flipped</returns>
        public static Vector3D FlipXY(Vector3D vector)
        {
            return new Vector3D(vector.Y, vector.X, vector.Z);
        }

        /// <summary>
        /// Flips the Y and Z of the chosen FE Vector3D
        /// </summary>
        /// <param name="vector">An FE Vector3D to flip</param>
        /// <returns>An FE Vector3D with Y and Z flipped</returns>
        public static Vector3D FlipYZ(Vector3D vector)
        {
            return new Vector3D(vector.X, vector.Z, vector.Y);
        }

        /// <summary>
        /// Negates the chosen FE Vector3D's X value only
        /// </summary>
        /// <param name="vector">An FE Vector3D</param>
        /// <returns>An FE Vector3D with the X value negated</returns>
        public static Vector3D NegateX(Vector3D vector)
        {
            return new Vector3D(-vector.X, vector.Y, vector.Z);
        }

        /// <summary>
        /// Negates the chosen FE Vector3D's Y value only
        /// </summary>
        /// <param name="vector">An FE Vector3D</param>
        /// <returns>An FE Vector3D with the Y value negated</returns>
        public static Vector3D NegateY(Vector3D vector)
        {
            return new Vector3D(vector.X, -vector.Y, vector.Z);
        }

        /// <summary>
        /// Negates the chosen FE Vector3D's Z value only
        /// </summary>
        /// <param name="vector">An FE Vector3D</param>
        /// <returns>An FE Vector3D with the Z value negated</returns>
        public static Vector3D NegateZ(Vector3D vector)
        {
            return new Vector3D(vector.X, vector.Y, -vector.Z);
        }

        /// <summary>
        /// Negates the chosen FE Vector3D
        /// </summary>
        /// <param name="vector">An FE Vector3D</param>
        /// <returns>An FE Vector3D negated</returns>
        public static Vector3D Negate(Vector3D vector)
        {
            return -vector;
        }

        /// <summary>
        /// Computes the Dot Product of two FE Vector3Ds
        /// </summary>
        /// <param name="vector1">The first FE Vector3D</param>
        /// <param name="vector2">The second FE Vector3D</param>
        /// <returns>The Dot Product of the FE Vector3Ds as an FE Vector3D</returns>
        public static float DotProduct(Vector3D vector1, Vector3D vector2)
        {
            float x1 = vector1.X;
            float y1 = vector1.Y;
            float z1 = vector1.Z;
            float x2 = vector2.X;
            float y2 = vector2.Y;
            float z2 = vector2.Z;
            return x1 * x2 + y1 * y2 + z1 * z2;
        }

        /// <summary>
        /// Computes the Cross Product of two FE Vector3Ds
        /// </summary>
        /// <param name="vector1">The first FE Vector3D</param>
        /// <param name="vector2">The second FE Vector3D</param>
        /// <returns>The Cross Product of the FE Vector3Ds as an FE Vector3D</returns>
        public static Vector3D CrossProduct(Vector3D vector1, Vector3D vector2)
        {
            float x1 = vector1.X;
            float y1 = vector1.Y;
            float z1 = vector1.Z;
            float x2 = vector2.X;
            float y2 = vector2.Y;
            float z2 = vector2.Z;
            return new Vector3D(y1 * z2 - z1 * y2, z1 * x2 - x1 * z2, x1 * y2 - y1 * x2);
        }

        /// <summary>
        /// Returns the length of the FE Vector3D 
        /// </summary>
        /// <returns>The length of the FE Vector3D </returns>
        public float Length()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        /// <summary>
        /// Returns a FE Vector3D with the same direction as the specified FE Vector3D, but with a length of 1
        /// </summary>
        /// <returns>A new normalized FE Vector3D</returns>
        public Vector3D Normalize()
        {
            float length = Length();
            if (length == 0)
            {
                return new Vector3D();
            }
            return new Vector3D(X / length, Y / length, Z / length);
        }

        /// <summary>
        /// Make a new object copy from a FE Vector3D
        /// </summary>
        /// <returns>A new object copy from a FE Vector3D</returns>
        public Vector3D Clone()
        {
            return new Vector3D(X, Y, Z);
        }

        /// <summary>
        /// The addition operator for FE Vector3D
        /// </summary>
        /// <param name="vector1">The first FE Vector3D to add</param>
        /// <param name="vector2">The second FE Vector3D to add</param>
        /// <returns>The sum of the two FE Vector3Ds as an FE Vector3D</returns>
        public static Vector3D operator +(Vector3D vector1, Vector3D vector2)
        {
            return new Vector3D(vector1.X + vector2.X, vector1.Y + vector2.Y, vector1.Z + vector2.Z);
        }

        /// <summary>
        /// The subtraction operator for FE Vector3D
        /// </summary>
        /// <param name="vector1">The first FE Vector3D to subtract</param>
        /// <param name="vector2">The second FE Vector3D to subtract</param>
        /// <returns>The difference of the two FE Vector3Ds as an FE Vector3D</returns>
        public static Vector3D operator -(Vector3D vector1, Vector3D vector2)
        {
            return new Vector3D(vector1.X - vector2.X, vector1.Y - vector2.Y, vector1.Z - vector2.Z);
        }

        /// <summary>
        /// The subtraction operator for FE Vector3D for negation
        /// </summary>
        /// <param name="vector">The FE Vector3D to negate</param>
        /// <returns>A negated FE Vector3D</returns>
        public static Vector3D operator -(Vector3D vector)
        {
            return new Vector3D(-vector.X, -vector.Y, -vector.Z);
        }

        /// <summary>
        /// The multiplication operator for FE Vector3D
        /// </summary>
        /// <param name="vector1">The first FE Vector3D to multiply by</param>
        /// <param name="vector2">The second FE Vector3D to multiply by</param>
        /// <returns>The product of the two FE Vector3Ds as an FE Vector3D</returns>
        public static Vector3D operator *(Vector3D vector1, Vector3D vector2)
        {
            return new Vector3D(vector1.X * vector2.X, vector1.Y * vector2.Y, vector1.Z * vector2.Z);
        }

        /// <summary>
        /// The multiplication operator for FE Vector3D
        /// </summary>
        /// <param name="vector">The FE Vector3D to multiply</param>
        /// <param name="b">A float representing the number to multiply the FE Vector3D by</param>
        /// <returns>The product of the FE Vector3D as an FE Vector3D</returns>
        public static Vector3D operator *(Vector3D vector, float b)
        {
            return new Vector3D(vector.X * b, vector.Y * b, vector.Z * b);
        }

        /// <summary>
        /// The multiplication operator for FE Vector3D
        /// </summary>
        /// <param name="b">A float representing the number to multiply the FE Vector3D by</param>
        /// <param name="vector">The FE Vector3D to multiply</param>
        /// <returns>The product of the FE Vector3D as an FE Vector3D</returns>
        public static Vector3D operator *(float b, Vector3D vector)
        {
            return new Vector3D(vector.X * b, vector.Y * b, vector.Z * b);
        }

        /// <summary>
        /// The division operator for FE Vector3D
        /// </summary>
        /// <param name="vector1">The first FE Vector3D to divide by</param>
        /// <param name="vector2">The second FE Vector3D to divide by</param>
        /// <returns>The quotient of the two FE Vector3Ds as an FE Vector3D</returns>
        public static Vector3D operator /(Vector3D vector1, Vector3D vector2)
        {
            return new Vector3D(vector1.X / vector2.X, vector1.Y / vector2.Y, vector1.Z / vector2.Z);
        }

        /// <summary>
        /// The division operator for FE Vector3D
        /// </summary>
        /// <param name="vector">The FE Vector3D to divide</param>
        /// <param name="b">A float representing the number to divide the FE Vector3D by</param>
        /// <returns>The product of the FE Vector3D as an FE Vector3D</returns>
        public static Vector3D operator /(Vector3D vector, float b)
        {
            return new Vector3D(vector.X / b, vector.Y / b, vector.Z / b);
        }

        /// <summary>
        /// The division operator for FE Vector3D
        /// </summary>
        /// <param name="b">A float representing the number to divide the FE Vector3D by</param>
        /// <param name="vector">The FE Vector3D to divide</param>
        /// <returns>The product of the FE Vector3D as an FE Vector3D</returns>
        public static Vector3D operator /(float b, Vector3D vector)
        {
            return new Vector3D(b / vector.X, b / vector.Y, b / vector.Z);
        }

        public static float CalculateDistanceFromLine(Vector3D point, Vector3D x1, Vector3D x2)
        {
            return CrossProduct(point - x1, point - x2).Length() / (x2 - x1).Length();
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