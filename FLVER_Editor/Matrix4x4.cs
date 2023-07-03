using System;

namespace FLVER_Editor
{
    /// <summary>
    /// A Matrix4x4 class for FLVER_Editor
    /// </summary>
    public class Matrix4x4
    {
        /// <summary>
        /// A float array acting as a Maxtrix4x4 for FLVER_Editor's FE Maxtrix4x4
        /// </summary>
        public float[,] value =
        {
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 },
            { 0, 0, 0, 0 }
        };

        /// <summary>
        /// Generate a translation FE Matrix4x4
        /// </summary>
        /// <param name="x">A float representing the X the FE Matrix4x4 will use</param>
        /// <param name="y">A float representing the Y the FE Matrix4x4 will use</param>
        /// <param name="z">A float representing the Z the FE Matrix4x4 will use</param>
        /// <returns>A translation FE Matrix4x4</returns>
        public static Matrix4x4 GenerateTranslationMatrix4x4(float x, float y, float z)
        {
            var m = new Matrix4x4();
            m.value = new[,]
            {
                { 1, 0, 0, x },
                { 0, 1, 0, y },
                { 0, 0, 1, z },
                { 0, 0, 0, 1 }
            };
            return m;
        }

        /// <summary>
        /// Generate a scale FE Matrix4x4
        /// </summary>
        /// <param name="x">A float representing the X the FE Matrix4x4 will use</param>
        /// <param name="y">A float representing the Y the FE Matrix4x4 will use</param>
        /// <param name="z">A float representing the Z the FE Matrix4x4 will use</param>
        /// <returns>A scale FE Matrix4x4</returns>
        public static Matrix4x4 GenerateScaleMatrix4x4(float x, float y, float z)
        {
            var m = new Matrix4x4();
            m.value = new[,]
            {
                { x, 0, 0, 0 },
                { 0, y, 0, 0 },
                { 0, 0, z, 0 },
                { 0, 0, 0, 1 }
            };
            return m;
        }

        /// <summary>
        /// Generate a scale FE Matrix4x4
        /// </summary>
        /// <param name="v">An FE Vector3D the FE Matrix4x4 will use</param>
        /// <returns>A scale FE Matrix4x4</returns>
        public static Matrix4x4 GenerateScaleMatrix4x4(Vector3D v)
        {
            var m = new Matrix4x4();
            m.value = new[,]
            {
                { v.X, 0, 0, 0 },
                { 0, v.Y, 0, 0 },
                { 0, 0, v.Z, 0 },
                { 0, 0, 0, 1 }
            };
            return m;
        }

        /// <summary>
        /// Generate an X rotation FE Matrix4x4
        /// </summary>
        /// <param name="a">A float representing the angle the FE Matrix4x4 will use</param>
        /// <returns>An X rotation FE Matrix4x4</returns>
        public static Matrix4x4 GenerateRotationXMatrix4x4(float a)
        {
            var rad = (float)(a / 180f * Math.PI);
            var m = new Matrix4x4();
            m.value = new[,]
            {
                { 1, 0, 0, 0 },
                { 0, C(rad), -S(rad), 0 },
                { 0, S(rad), C(rad), 0 },
                { 0, 0, 0, 1 }
            };
            return m;
        }

        /// <summary>
        /// Generate an Y rotation FE Matrix4x4
        /// </summary>
        /// <param name="a">A float representing the angle the FE Matrix4x4 will use</param>
        /// <returns>An Y rotation FE Matrix4x4</returns>
        public static Matrix4x4 GenerateRotationYMatrix4x4(float a)
        {
            var rad = (float)(a / 180f * Math.PI);
            var m = new Matrix4x4();
            m.value = new[,]
            {
                { C(rad), 0, S(rad), 0 },
                { 0, 1, 0, 0 },
                { -S(rad), 0, C(rad), 0 },
                { 0, 0, 0, 1 }
            };
            return m;
        }

        /// <summary>
        /// Generate an Z rotation FE Matrix4x4
        /// </summary>
        /// <param name="a">A float representing the angle the Matrix4x4 will use</param>
        /// <returns>An Z rotation FE Matrix4x4</returns>
        public static Matrix4x4 GenerateRotationZMatrix4x4(float a)
        {
            var rad = (float)(a / 180f * Math.PI);
            var m = new Matrix4x4();
            m.value = new[,]
            {
                { C(rad), -S(rad), 0, 0 },
                { S(rad), C(rad), 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 0, 1 }
            };
            return m;
        }

        /// <summary>
        /// Returns the cosine of the specified angle
        /// </summary>
        /// <param name="rad">The specified angle</param>
        /// <returns>The cosine of the specified angle</returns>
        public static float C(float rad) 
        {
            return (float)Math.Cos(rad);
        }

        /// <summary>
        /// Returns the sine of the specified angle
        /// </summary>
        /// <param name="rad">The specified angle</param>
        /// <returns>The sine of the specified angle</returns>
        public static float S(float rad)
        {
            return (float)Math.Sin(rad);
        }

        /// <summary>
        /// Multiply the chosen FE Matrix4x4 by the chosen FE Vector3D
        /// </summary>
        /// <param name="m">The chosen FE Matrix4x4 to multiply by</param>
        /// <param name="v">The chosen FE Vector3D to multiply be</param>
        /// <returns>A FE Matrix4x4 multipled by the chosen FE Vector3D</returns>
        public static Vector3D Matrix4x4TimesVector3D(Matrix4x4 m, Vector3D v)
        {
            float x = 0;
            float y = 0;
            float z = 0;
            x = m.value[0, 0] * v.X + m.value[0, 1] * v.Y + m.value[0, 2] * v.Z + m.value[0, 3] * 1;
            y = m.value[1, 0] * v.X + m.value[1, 1] * v.Y + m.value[1, 2] * v.Z + m.value[1, 3] * 1;
            z = m.value[2, 0] * v.X + m.value[2, 1] * v.Y + m.value[2, 2] * v.Z + m.value[2, 3] * 1;
            return new Vector3D(x, y, z);
        }

        /// <summary>
        /// The multiplication operator for FE Matrix4x4 
        /// </summary>
        /// <param name="m1">The first FE Matrix4x4 to multiply by</param>
        /// <param name="m2">The second FE Matrix4x4 to multiply by</param>
        /// <returns>The product of the two FE Matrix4x4 as an FE Matrix4x4</returns>
        public static Matrix4x4 operator *(
            Matrix4x4 m1, Matrix4x4 m2)
        {
            var m = new Matrix4x4();
            for (var j = 0; j < 4; j++)
            {
                for (var i = 0; i < 4; i++)
                {
                    m.value[j, i] = m1.value[j, 0] * m2.value[0, i] + m1.value[j, 1] * m2.value[1, i] + m1.value[j, 2] * m2.value[2, i] + m1.value[j, 3] * m2.value[3, i];
                }
            }
            return m;
        }

        /// <summary>
        /// Multiply two FE Matrix4x4 by each other
        /// </summary>
        /// <param name="m1">The first FE Matrix4x4 to multiply by</param>
        /// <param name="m2">The second FE Matrix4x4 to multiply by</param>
        /// <returns>The product of the two FE Matrix4x4 as an FE Matrix4x4</returns>
        public static Matrix4x4 Matrix4x4TimesMatrix4x4(Matrix4x4 m1, Matrix4x4 m2)
        {
            var m = new Matrix4x4();
            for (var j = 0; j < 4; j++)
            {
                for (var i = 0; i < 4; i++)
                {
                    m.value[j, i] = m1.value[j, 0] * m2.value[0, i] + m1.value[j, 1] * m2.value[1, i] + m1.value[j, 2] * m2.value[2, i] + m1.value[j, 3] * m2.value[3, i];
                }
            }
            return m;
        }
    }
}