using System;
using Microsoft.Xna.Framework;

namespace FLVER_Editor
{
    /// <summary>
    /// A class copied from Vector3D to work with Vector4
    /// </summary>
    public class Vector4D
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        /// <summary>
        /// Create a new FE Vector4D from no values and have X, Y, Z, and W set to 0
        /// </summary>
        public Vector4D()
        {
            X = 0;
            Y = 0;
            Z = 0;
            W = 0;
        }

        /// <summary>
        /// Create a new FE Vector4D from X, Y, Z, and W values as floats
        /// </summary>
        /// <param name="x">A float representing the X of the new FE Vector4D</param>
        /// <param name="y">A float representing the Y of the new FE Vector4D</param>
        /// <param name="z">A float representing the Z of the new FE Vector4D</param>
        /// <param name="w">A float representing the W of the new FE Vector4D</param>
        public Vector4D(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Create a new FE Vector4D from an Xna Vector4
        /// </summary>
        /// <param name="vector">An Xna Vector4</param>
        public Vector4D(Vector4 vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
            W = vector.W;
        }

        /// <summary>
        /// Create a new FE Vector4D from an Xna Vector3 and a given W value as a float
        /// </summary>
        /// <param name="vector">An Xna Vector3</param>
        /// <param name="w">A float representing the W of the new FE Vector4D</param>
        public Vector4D(Vector3 vector, float w)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
            W = w;
        }

        /// <summary>
        /// Create a new FE Vector4D from an Xna Vector2 and given Z and W values as floats
        /// </summary>
        /// <param name="vector">An Xna Vector2</param>
        /// <param name="z">A float representing the Z of the new FE Vector4D</param>
        /// <param name="w">A float representing the W of the new FE Vector4D</param>
        public Vector4D(Vector2 vector, float z, float w)
        {
            X = vector.X;
            Y = vector.Y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Create a new FE Vector4D from a numerics Vector4
        /// </summary>
        /// <param name="vector">A numerics Vector4</param>
        public Vector4D(System.Numerics.Vector4 vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
            W = vector.W;
        }

        /// <summary>
        /// Create a new FE Vector4D from a numerics Vector3 and a given W value as a float
        /// </summary>
        /// <param name="vector">A numerics Vector3</param>
        /// <param name="w">A float representing the W of the new FE Vector4D</param>
        public Vector4D(System.Numerics.Vector3 vector, float w)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
            W = w;
        }

        /// <summary>
        /// Create a new FE Vector4D from a numerics Vector2 and given Z and W values as floats
        /// </summary>
        /// <param name="vector">A numerics Vector2</param>
        /// <param name="z">A float representing the Z of the new FE Vector4D</param>
        /// <param name="w">A float representing the W of the new FE Vector4D</param>
        public Vector4D(System.Numerics.Vector2 vector, float z, float w)
        {
            X = vector.X;
            Y = vector.Y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Create a new FE Vector4D from an Assimp Vector3D and a given W value as a float
        /// </summary>
        /// <param name="vector">An Assimp Vector3D</param>
        /// <param name="w">A float representing the W of the new FE Vector4D</param>
        public Vector4D(Assimp.Vector3D vector, float w)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
            W = w;
        }

        /// <summary>
        /// Create a new FE Vector4D from an Assimp Vector2D and given Z and W values as floats
        /// </summary>
        /// <param name="vector">An Assimp Vector2D</param>
        /// <param name="z">A float representing the Z of the new FE Vector4D</param>
        /// <param name="w">A float representing the W of the new FE Vector4D</param>
        public Vector4D(Assimp.Vector2D vector, float z, float w)
        {
            X = vector.X;
            Y = vector.Y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Create a new FE Vector4D from an FE Vector3D
        /// </summary>
        /// <param name="vector">An FE Vector3D</param>
        /// <param name="w">A float representing the W of the new FE Vector4D</param>
        public Vector4D(Vector3D vector, float w)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
            W = w;
        }

        /// <summary>
        /// Create a new FE Vector4D from an FE Vector2D and given Z and W values as floats
        /// </summary>
        /// <param name="vector">An FE Vector2D</param>
        /// <param name="z">A float representing the Z of the new FE Vector4D</param>
        /// <param name="w">A float representing the W of the new FE Vector4D</param>
        public Vector4D(Vector2D vector, float z, float w)
        {
            X = vector.X;
            Y = vector.Y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Converts a FE Vector4D to an Xna Vector4
        /// </summary>
        /// <returns>An Xna Vector4</returns>
        public Vector4 ToXnaVector4()
        {
            return new Vector4(X, Y, Z, W);
        }

        /// <summary>
        /// Converts a FE Vector4D to an Xna Vector3, discarding W
        /// </summary>
        /// <returns>An Xna Vector3</returns>
        public Vector3 ToXnaVector3()
        {
            return new Vector3(X, Y, Z);
        }

        /// <summary>
        /// Converts a FE Vector4D to an Xna Vector2, discarding Z and W
        /// </summary>
        /// <returns>An Xna Vector2</returns>
        public Vector2 ToXnaVector2()
        {
            return new Vector2(X, Y);
        }

        /// <summary>
        /// Converts a FE Vector4D to a numerics Vector4
        /// </summary>
        /// <returns>A numerics Vector4</returns>
        public System.Numerics.Vector4 ToNumericsVector4()
        {
            return new System.Numerics.Vector4(X, Y, Z, W);
        }

        /// <summary>
        /// Converts a FE Vector4D to a numerics Vector3, discarding W
        /// </summary>
        /// <returns>A numerics Vector3</returns>
        public System.Numerics.Vector3 ToNumericsVector3()
        {
            return new System.Numerics.Vector3(X, Y, Z);
        }

        /// <summary>
        /// Converts a FE Vector4D to an numerics Vector2, discarding Z and W
        /// </summary>
        /// <returns>A numerics Vector2</returns>
        public System.Numerics.Vector2 ToNumericsVector2()
        {
            return new System.Numerics.Vector2(X, Y);
        }

        /// <summary>
        /// Converts a FE Vector4D to an Assimp Vector3D, discarding W
        /// </summary>
        /// <returns>An Assimp Vector3D</returns>
        public Assimp.Vector3D ToAssimpVector3D()
        {
            return new Assimp.Vector3D(X, Y, Z);
        }

        /// <summary>
        /// Converts a FE Vector4D to an Assimp Vector2D, discarding Z and W
        /// </summary>
        /// <returns>An Assimp Vector2D</returns>
        public Assimp.Vector2D ToAssimpVector2D()
        {
            return new Assimp.Vector2D(X, Y);
        }

        /// <summary>
        /// Converts a FE Vector4D to an FE Vector3D, discarding W
        /// </summary>
        /// <returns>An FE Vector3D</returns>
        public Vector3D ToVector3D()
        {
            return new Vector3D(X, Y, Z);
        }

        /// <summary>
        /// Converts a FE Vector4D to an FE Vector3D, discarding Z and W
        /// </summary>
        /// <returns>An FE Vector2D</returns>
        public Vector2D ToVector2D()
        {
            return new Vector2D(X, Y);
        }

        /// <summary>
        /// Flips the X and Z of the chosen FE Vector4D
        /// </summary>
        /// <param name="vector">An FE Vector4D to flip</param>
        /// <returns>An FE Vector4D with X and Z flipped</returns>
        public static Vector4D FlipXZ(Vector4D vector)
        {
            return new Vector4D(vector.Z, vector.Y, vector.X, vector.W);
        }

        /// <summary>
        /// Flips the X and Y of the chosen FE Vector4D
        /// </summary>
        /// <param name="vector">An FE Vector4D to flip</param>
        /// <returns>An FE Vector4D with X and Y flipped</returns>
        public static Vector4D FlipXY(Vector4D vector)
        {
            return new Vector4D(vector.Y, vector.X, vector.Z, vector.W);
        }

        /// <summary>
        /// Flips the X and W of the chosen FE Vector4D
        /// </summary>
        /// <param name="vector">An FE Vector4D to flip</param>
        /// <returns>An FE Vector4D with X and W flipped</returns>
        public static Vector4D FlipXW(Vector4D vector)
        {
            return new Vector4D(vector.W, vector.Y, vector.Z, vector.X);
        }

        /// <summary>
        /// Flips the Y and Z of the chosen FE Vector4D
        /// </summary>
        /// <param name="vector">An FE Vector4D to flip</param>
        /// <returns>An FE Vector4D with Y and Z flipped</returns>
        public static Vector4D FlipYZ(Vector4D vector)
        {
            return new Vector4D(vector.X, vector.Z, vector.Y, vector.W);
        }

        /// <summary>
        /// Flips the Y and W of the chosen FE Vector4D
        /// </summary>
        /// <param name="vector">An FE Vector4D to flip</param>
        /// <returns>An FE Vector4D with Y and W flipped</returns>
        public static Vector4D FlipYW(Vector4D vector)
        {
            return new Vector4D(vector.X, vector.W, vector.Z, vector.Y);
        }

        /// <summary>
        /// Flips the Z and W of the chosen FE Vector4D
        /// </summary>
        /// <param name="vector">An FE Vector4D to flip</param>
        /// <returns>An FE Vector4D with Z and W flipped</returns>
        public static Vector4D FlipZW(Vector4D vector)
        {
            return new Vector4D(vector.X, vector.Y, vector.W, vector.Z);
        }

        /// <summary>
        /// Negates the chosen FE Vector4D's X value only
        /// </summary>
        /// <param name="vector">An FE Vector4D</param>
        /// <returns>An FE Vector4D with the X value negated</returns>
        public static Vector4D NegateX(Vector4D vector)
        {
            return new Vector4D(-vector.X, vector.Y, vector.Z, vector.W);
        }

        /// <summary>
        /// Negates the chosen FE Vector4D's Y value only
        /// </summary>
        /// <param name="vector">An FE Vector4D</param>
        /// <returns>An FE Vector4D with the Y value negated</returns>
        public static Vector4D NegateY(Vector4D vector)
        {
            return new Vector4D(vector.X, -vector.Y, vector.Z, vector.W);
        }

        /// <summary>
        /// Negates the chosen FE Vector4D's Z value only
        /// </summary>
        /// <param name="vector">An FE Vector4D</param>
        /// <returns>An FE Vector4D with the Z value negated</returns>
        public static Vector4D NegateZ(Vector4D vector)
        {
            return new Vector4D(vector.X, vector.Y, -vector.Z, vector.W);
        }

        /// <summary>
        /// Negates the chosen FE Vector4D's W value only
        /// </summary>
        /// <param name="vector">An FE Vector4D</param>
        /// <returns>An FE Vector4D with the W value negated</returns>
        public static Vector4D NegateW(Vector4D vector)
        {
            return new Vector4D(vector.X, vector.Y, vector.Z, -vector.W);
        }

        /// <summary>
        /// Negates the chosen FE Vector4D
        /// </summary>
        /// <param name="vector">An FE Vector4D</param>
        /// <returns>An FE Vector4D negated</returns>
        public static Vector4D Negate(Vector4D vector)
        {
            return -vector;
        }

        /// <summary>
        /// Computes the Dot Product of two FE Vector4Ds
        /// </summary>
        /// <param name="vector1">The first FE Vector4D</param>
        /// <param name="vector2">The second FE Vector4D</param>
        /// <returns>The Dot Product of the FE Vector4Ds as an FE Vector4D</returns>
        public static float DotProduct(Vector4D vector1, Vector4D vector2)
        {
            float x1 = vector1.X;
            float y1 = vector1.Y;
            float z1 = vector1.Z;
            float w1 = vector1.W;
            float x2 = vector2.X;
            float y2 = vector2.Y;
            float z2 = vector2.Z;
            float w2 = vector2.W;
            return x1 * x2 + y1 * y2 + z1 * z2 + w1 * w2;
        }

        // <summary>
        /// Returns the length of the FE Vector4D 
        /// </summary>
        /// <returns>The length of the FE Vector4D</returns>
        public float Length()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
        }

        /// <summary>
        /// Returns a FE Vector4D with the same direction as the specified FE Vector4D, but with a length of 1
        /// </summary>
        /// <returns>A new normalized FE Vector4D</returns>
        public Vector4D Normalize()
        {
            float length = Length();
            if (length == 0)
            {
                return new Vector4D();
            }
            return new Vector4D(X / length, Y / length, Z / length, W / length);
        }

        /// <summary>
        /// Make a new object copy from a FE Vector4D
        /// </summary>
        /// <returns>A new object copy from a FE Vector4D</returns>
        public Vector4D Clone()
        {
            return new Vector4D(X, Y, Z, W);
        }

        /// <summary>
        /// The addition operator for FE Vector4D
        /// </summary>
        /// <param name="vector1">The first FE Vector4D to add</param>
        /// <param name="vector2">The second FE Vector4D to add</param>
        /// <returns>The sum of the two FE Vector4Ds as an FE Vector4D</returns>
        public static Vector4D operator +(Vector4D vector1, Vector4D vector2)
        {
            return new Vector4D(vector1.X + vector2.X, vector1.Y + vector2.Y, vector1.Z + vector2.Z, vector1.W + vector2.W);
        }

        /// <summary>
        /// The subtraction operator for FE Vector4D
        /// </summary>
        /// <param name="vector1">The first FE Vector4D to subtract</param>
        /// <param name="vector2">The second FE Vector4D to subtract</param>
        /// <returns>The difference of the two FE Vector4Ds as an FE Vector3D</returns>
        public static Vector4D operator -(Vector4D vector1, Vector4D vector2)
        {
            return new Vector4D(vector1.X - vector2.X, vector1.Y - vector2.Y, vector1.Z - vector2.Z, vector1.W - vector2.W);
        }

        /// <summary>
        /// The subtraction operator for FE Vector4D for negation
        /// </summary>
        /// <param name="vector">The FE Vector4D to negate</param>
        /// <returns>A negated FE Vector4D</returns>
        public static Vector4D operator -(Vector4D vector)
        {
            return new Vector4D(-vector.X, -vector.Y, -vector.Z, -vector.W);
        }

        /// <summary>
        /// The multiplication operator for FE Vector4D
        /// </summary>
        /// <param name="vector1">The first FE Vector4D to multiply by</param>
        /// <param name="vector2">The second FE Vector4D to multiply by</param>
        /// <returns>The product of the two FE Vector4Ds as an FE Vector4D</returns>
        public static Vector4D operator *(Vector4D vector1, Vector4D vector2)
        {
            return new Vector4D(vector1.X * vector2.X, vector1.Y * vector2.Y, vector1.Z * vector2.Z, vector1.W * vector2.W);
        }

        /// <summary>
        /// The multiplication operator for FE Vector4D
        /// </summary>
        /// <param name="vector">The FE Vector4D to multiply</param>
        /// <param name="b">A float representing the number to multiply the FE Vector4D by</param>
        /// <returns>The product of the FE Vector4D as an FE Vector4D</returns>
        public static Vector4D operator *(Vector4D vector, float b)
        {
            return new Vector4D(vector.X * b, vector.Y * b, vector.Z * b, vector.W * b);
        }

        /// <summary>
        /// The multiplication operator for FE Vector4D
        /// </summary>
        /// <param name="b">A float representing the number to multiply the FE Vector4D by</param>
        /// <param name="vector">The FE Vector4D to multiply</param>
        /// <returns>The product of the FE Vector4D as an FE Vector4D</returns>
        public static Vector4D operator *(float b, Vector4D vector)
        {
            return new Vector4D(vector.X * b, vector.Y * b, vector.Z * b, vector.W * b);
        }

        /// <summary>
        /// The division operator for FE Vector4D
        /// </summary>
        /// <param name="vector1">The first FE Vector4D to divide by</param>
        /// <param name="vector2">The second FE Vector4D to divide by</param>
        /// <returns>The quotient of the two FE Vector4Ds as an FE Vector4D</returns>
        public static Vector4D operator /(Vector4D vector1, Vector4D vector2)
        {
            return new Vector4D(vector1.X / vector2.X, vector1.Y / vector2.Y, vector1.Z / vector2.Z, vector1.W / vector2.W);
        }

        /// <summary>
        /// The division operator for FE Vector4D
        /// </summary>
        /// <param name="vector">The FE Vector4D to divide</param>
        /// <param name="b">A float representing the number to divide the FE Vector4D by</param>
        /// <returns>The quotient of the FE Vector4D as an FE Vector4D</returns>
        public static Vector4D operator /(Vector4D vector, float b)
        {
            return new Vector4D(vector.X / b, vector.Y / b, vector.Z / b, vector.W / b);
        }

        /// <summary>
        /// The division operator for FE Vector4D
        /// </summary>
        /// <param name="b">A float representing the number to divide the FE Vector4D by</param>
        /// <param name="vector">The FE Vector4D to divide</param>
        /// <returns>The quotient of the FE Vector4D as an FE Vector4D</returns>
        public static Vector4D operator /(float b, Vector4D vector)
        {
            return new Vector4D(b / vector.X, b / vector.Y, b / vector.Z, b / vector.W);
        }
    }
}
