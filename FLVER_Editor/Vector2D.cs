using System;
using Microsoft.Xna.Framework;

namespace FLVER_Editor
{
    /// <summary>
    /// A class copied from Vector3D to work with Vector2
    /// </summary>
    public class Vector2D
    {
        public float X;
        public float Y;

        /// <summary>
        /// Create a new FE Vector3D from no values and have X and Y set to 0
        /// </summary>
        public Vector2D()
        {
            X = 0;
            Y = 0;
        }

        /// <summary>
        /// Create a new FE Vector2D from X and Y values as floats
        /// </summary>
        /// <param name="x">A float representing the X of the new FE Vector3D</param>
        /// <param name="y">A float representing the Y of the new FE Vector3D</param>
        public Vector2D(float x, float y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Create a new FE Vector2D from an Xna Vector4, discarding Z and W
        /// </summary>
        /// <param name="vector">An Xna Vector4</param>
        public Vector2D(Vector4 vector)
        {
            X = vector.X;
            Y = vector.Y;
        }

        /// <summary>
        /// Create a new FE Vector2D from an Xna Vector3, discarding Z
        /// </summary>
        /// <param name="vector">An Xna Vector3</param>
        public Vector2D(Vector3 vector)
        {
            X = vector.X;
            Y = vector.Y;
        }

        /// <summary>
        /// Create a new FE Vector2D from an Xna Vector2
        /// </summary>
        /// <param name="vector">An Xna Vector2</param>
        public Vector2D(Vector2 vector)
        {
            X = vector.X;
            Y = vector.Y;
        }

        /// <summary>
        /// Create a new FE Vector2D from a numerics Vector4, discarding Z and W
        /// </summary>
        /// <param name="vector">A numerics Vector4</param>
        public Vector2D(System.Numerics.Vector4 vector)
        {
            X = vector.X;
            Y = vector.Y;
        }

        /// <summary>
        /// Create a new FE Vector2D from a numerics Vector3, discarding Z
        /// </summary>
        /// <param name="vector">A numerics Vector3</param>
        public Vector2D(System.Numerics.Vector3 vector)
        {
            X = vector.X;
            Y = vector.Y;
        }

        /// <summary>
        /// Create a new FE Vector2D from a numerics Vector2
        /// </summary>
        /// <param name="vector">A numerics Vector2</param>
        public Vector2D(System.Numerics.Vector2 vector)
        {
            X = vector.X;
            Y = vector.Y;
        }

        /// <summary>
        /// Create a new FE Vector2D from an Assimp Vector3D, discarding Z
        /// </summary>
        /// <param name="vector">An Assimp Vector3D</param>
        public Vector2D(Assimp.Vector3D vector)
        {
            X = vector.X;
            Y = vector.Y;
        }

        /// <summary>
        /// Create a new FE Vector2D from an Assimp Vector2D
        /// </summary>
        /// <param name="vector">An Assimp Vector2D</param>
        public Vector2D(Assimp.Vector2D vector)
        {
            X = vector.X;
            Y = vector.Y;
        }

        /// <summary>
        /// Create a new FE Vector2D from an FE Vector4D, discarding Z and W
        /// </summary>
        /// <param name="vector">An FE Vector4D</param>
        public Vector2D(Vector4D vector)
        {
            X = vector.X;
            Y = vector.Y;
        }

        /// <summary>
        /// Create a new FE Vector2D from an FE Vector3D, discarding Z
        /// </summary>
        /// <param name="vector">An FE Vector3D</param>
        public Vector2D(Vector3D vector)
        {
            X = vector.X;
            Y = vector.Y;
        }

        /// <summary>
        /// Converts a FE Vector2D to an Xna Vector4, adding 0 as the Z and W values
        /// </summary>
        /// <returns>An Xna Vector4</returns>
        public Vector4 ToXnaVector4()
        {
            return new Vector4(X, Y, 0, 0);
        }

        /// <summary>
        /// Converts a FE Vector2D to an Xna Vector3, adding 0 as the Z value
        /// </summary>
        /// <returns>An Xna Vector3</returns>
        public Vector3 ToXnaVector3()
        {
            return new Vector3(X, Y, 0);
        }

        /// <summary>
        /// Converts a FE Vector2D to an Xna Vector2
        /// </summary>
        /// <returns>An Xna Vector2</returns>
        public Vector2 ToXnaVector2()
        {
            return new Vector2(X, Y);
        }

        /// <summary>
        /// Converts a FE Vector2D to an numerics Vector4, adding 0 as the Z and W values
        /// </summary>
        /// <returns>A numerics Vector4</returns>
        public System.Numerics.Vector4 ToNumericsVector4()
        {
            return new System.Numerics.Vector4(X, Y, 0, 0);
        }

        /// <summary>
        /// Converts a FE Vector2D to an numerics Vector3, adding 0 as the Z value
        /// </summary>
        /// <returns>A numerics Vector3</returns>
        public System.Numerics.Vector3 ToNumericsVector3()
        {
            return new System.Numerics.Vector3(X, Y, 0);
        }

        /// <summary>
        /// Converts a FE Vector2D to an numerics Vector2
        /// </summary>
        /// <returns>A numerics Vector2</returns>
        public System.Numerics.Vector2 ToNumericsVector2()
        {
            return new System.Numerics.Vector2(X, Y);
        }

        /// <summary>
        /// Converts a FE Vector2D to an Assimp Vector3D, adding 0 as the Z value
        /// </summary>
        /// <returns>An Assimp Vector2D</returns>
        public Assimp.Vector3D ToAssimpVector3D()
        {
            return new Assimp.Vector3D(X, Y, 0);
        }

        /// <summary>
        /// Converts a FE Vector2D to an Assimp Vector2D
        /// </summary>
        /// <returns>An Assimp Vector2D</returns>
        public Assimp.Vector2D ToAssimpVector2D()
        {
            return new Assimp.Vector2D(X, Y);
        }

        /// <summary>
        /// Converts a FE Vector2D to an FE Vector4D, adding 0 as the Z and W values
        /// </summary>
        /// <returns>An FE Vector4D</returns>
        public Vector4D ToVector4D()
        {
            return new Vector4D(X, Y, 0, 0);
        }

        /// <summary>
        /// Converts a FE Vector2D to an Xna Vector3, adding 0 as the Z value
        /// </summary>
        /// <returns>An FE Vector3D</returns>
        public Vector3D ToVector3D()
        {
            return new Vector3D(X, Y, 0);
        }

        /// <summary>
        /// Flips the X and Y of the chosen FE Vector2D
        /// </summary>
        /// <param name="vector">An FE Vector2D to flip</param>
        /// <returns>An FE Vector2D with X and Y flipped</returns>
        public static Vector2D FlipXY(Vector2D vector)
        {
            return new Vector2D(vector.Y, vector.X);
        }

        /// <summary>
        /// Negates the chosen FE Vector2D's X value only
        /// </summary>
        /// <param name="vector">An FE Vector2D</param>
        /// <returns>An FE Vector2D with the X value negated</returns>
        public static Vector2D NegateX(Vector2D vector)
        {
            return new Vector2D(-vector.X, vector.Y);
        }

        /// <summary>
        /// Negates the chosen FE Vector2D's Y value only
        /// </summary>
        /// <param name="vector">An FE Vector2D</param>
        /// <returns>An FE Vector2D with the Y value negated</returns>
        public static Vector2D NegateY(Vector2D vector)
        {
            return new Vector2D(vector.X, -vector.Y);
        }

        /// <summary>
        /// Negates the chosen FE Vector2D
        /// </summary>
        /// <param name="vector">An FE Vector2D</param>
        /// <returns>An FE Vector2D negated</returns>
        public static Vector2D Negate(Vector2D vector)
        {
            return -vector;
        }

        /// <summary>
        /// Computes the Dot Product of two FE Vector2Ds
        /// </summary>
        /// <param name="vector1">The first FE Vector2D</param>
        /// <param name="vector2">The second FE Vector2D</param>
        /// <returns>The Dot Product of the FE Vector2Ds as an FE Vecto23D</returns>
        public static float DotProduct(Vector2D vector1, Vector2D vector2)
        {
            float x1 = vector1.X;
            float y1 = vector1.Y;
            float x2 = vector2.X;
            float y2 = vector2.Y;
            return x1 * x2 + y1 * y2;
        }

        /// <summary>
        /// Returns the length of the FE Vector2D
        /// </summary>
        /// <returns>The length of the FE Vector2D</returns>
        public float Length()
        {
            return (float)Math.Sqrt(X * X + Y * Y);
        }

        /// <summary>
        /// Returns a FE Vector2D with the same direction as the specified FE Vector2D, but with a length of 1
        /// </summary>
        /// <returns>A new normalized FE Vector2D</returns>
        public Vector2D Normalize()
        {
            float length = Length();
            if (length == 0)
            {
                return new Vector2D();
            }
            return new Vector2D(X / length, Y / length);
        }

        /// <summary>
        /// Make a new object copy from a FE Vector2D
        /// </summary>
        /// <returns>A new object copy from a FE Vector2D</returns>
        public Vector2D Clone()
        {
            return new Vector2D(X, Y);
        }

        /// <summary>
        /// The addition operator for FE Vector2D
        /// </summary>
        /// <param name="vector1">The first FE Vector2D to add</param>
        /// <param name="vector2">The second FE Vector2D to add</param>
        /// <returns>The sum of the two FE Vector2Ds as an FE Vector2D</returns>
        public static Vector2D operator +(Vector2D vector1, Vector2D vector2)
        {
            return new Vector2D(vector1.X + vector2.X, vector1.Y + vector2.Y);
        }

        /// <summary>
        /// The subtraction operator for FE Vector2D
        /// </summary>
        /// <param name="vector1">The first FE Vector2D to subtract</param>
        /// <param name="vector2">The second FE Vector2D to subtract</param>
        /// <returns>The difference of the two FE Vector2Ds as an FE Vector2D</returns>
        public static Vector2D operator -(Vector2D vector1, Vector2D vector2)
        {
            return new Vector2D(vector1.X - vector2.X, vector1.Y - vector2.Y);
        }

        /// <summary>
        /// The subtraction operator for FE Vector2D for negation
        /// </summary>
        /// <param name="vector">The FE Vector2D to negate</param>
        /// <returns>A negated FE Vector2D</returns>
        public static Vector2D operator -(Vector2D vector)
        {
            return new Vector2D(-vector.X, -vector.Y);
        }

        /// <summary>
        /// The multiplication operator for FE Vector2D
        /// </summary>
        /// <param name="vector1">The first FE Vector2D to multiply by</param>
        /// <param name="vector2">The second FE Vector2D to multiply by</param>
        /// <returns>The product of the two FE Vector2Ds as an FE Vector2D</returns>
        public static Vector2D operator *(Vector2D vector1, Vector2D vector2)
        {
            return new Vector2D(vector1.X * vector2.X, vector1.Y * vector2.Y);
        }

        /// <summary>
        /// The multiplication operator for FE Vector2D
        /// </summary>
        /// <param name="vector">The FE Vector2D to multiply</param>
        /// <param name="b">A float representing the number to multiply the FE Vector2D by</param>
        /// <returns>The product of the FE Vector2D as an FE Vector2D</returns>
        public static Vector2D operator *(Vector2D vector, float b)
        {
            return new Vector2D(vector.X * b, vector.Y * b);
        }

        /// <summary>
        /// The multiplication operator for FE Vector2D
        /// </summary>
        /// <param name="b">A float representing the number to multiply the FE Vector2D by</param>
        /// <param name="vector">The FE Vector2D to multiply</param>
        /// <returns>The product of the FE Vector2D as an FE Vector2D</returns>
        public static Vector2D operator *(float b, Vector2D vector)
        {
            return new Vector2D(vector.X * b, vector.Y * b);
        }

        /// <summary>
        /// The division operator for FE Vector2D
        /// </summary>
        /// <param name="vector1">The first FE Vector2D to divide by</param>
        /// <param name="vector2">The second FE Vector2D to divide by</param>
        /// <returns>The quotient of the two FE Vector2Ds as an FE Vector2D</returns>
        public static Vector2D operator /(Vector2D vector1, Vector2D vector2)
        {
            return new Vector2D(vector1.X / vector2.X, vector1.Y / vector2.Y);
        }

        /// <summary>
        /// The division operator for FE Vector2D
        /// </summary>
        /// <param name="vector">The FE Vector2D to divide</param>
        /// <param name="b">A float representing the number to divide the FE Vector2D by</param>
        /// <returns>The quotient of the FE Vector2D as an FE Vector2D</returns>
        public static Vector2D operator /(Vector2D vector, float b)
        {
            return new Vector2D(vector.X / b, vector.Y / b);
        }

        /// <summary>
        /// The division operator for FE Vector2D
        /// </summary>
        /// <param name="b">A float representing the number to divide the FE Vector2D by</param>
        /// <param name="vector">The FE Vector2D to divide</param>
        /// <returns>The quotient of the FE Vector2D as an FE Vector2D</returns>
        public static Vector2D operator /(float b, Vector2D vector)
        {
            return new Vector2D(b / vector.X, b / vector.Y);
        }
    }
}