using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor
{
    public static class Transform3DOperations
    {
        public static Vector3 CreateTranslationVector(float x, float y, float z, float offset, int nbi)
        {
            return new Vector3(x + (nbi == 0 ? offset : 0), y + (nbi == 1 ? offset : 0), z + (nbi == 2 ? offset : 0));
        }

        public static Vector3 CreateScaleVector(float x, float y, float z, float offset, IReadOnlyList<float> totals, int nbi, bool uniform, bool invert)
        {
            float scalar = offset < 0 && !invert ? -(offset - 1) : invert ? offset - 1 : offset + 1;
            float newX = nbi == 0 || uniform ? x - totals[0] : x;
            float newY = nbi == 1 || uniform ? y - totals[1] : y;
            float newZ = nbi == 2 || uniform ? z - totals[2] : z;
            newX = nbi == 0 || uniform ? (offset < 0 && !invert ? newX / scalar : newX * scalar) + totals[0] : x;
            newY = nbi == 1 || uniform ? (offset < 0 && !invert ? newY / scalar : newY * scalar) + totals[1] : y;
            newZ = nbi == 2 || uniform ? (offset < 0 && !invert ? newZ / scalar : newZ * scalar) + totals[2] : z;
            return new Vector3(newX, newY, newZ);
        }

        public static dynamic CreateRotationVector(float x, float y, float z, float w, float offset, IReadOnlyList<float> totals, int nbi)
        {
            float newX = nbi == 1 ? offset : 0;
            float newY = nbi == 0 ? offset : 0;
            float newZ = nbi == 2 ? offset : 0;
            Vector3 vector = new(x - totals[0], y - totals[1], z - totals[2]);
            vector = Program.RotatePoint(vector, newY, newX, newZ);
            return w == 0 ? new Vector3(vector.X + totals[0], vector.Y + totals[1], vector.Z + totals[2]) :
                new Vector4(vector.X + totals[0], vector.Y + totals[1], vector.Z + totals[2], w);
        }

        public static void TranslateThing(dynamic thing, float offset, int nbi)
        {
            switch (thing)
            {
                case FLVER.Dummy d:
                    d.Position = CreateTranslationVector(d.Position.X, d.Position.Y, d.Position.Z, offset, nbi);
                    break;
                case FLVER.Vertex v:
                    v.Position = CreateTranslationVector(v.Position.X, v.Position.Y, v.Position.Z, offset, nbi);
                    break;
            }
        }

        public static void ScaleThing(dynamic thing, float offset, IReadOnlyList<float> totals, int nbi, bool uniform, bool invert, bool useVectorMode)
        {
            if (nbi is >= 3 and <= 5) nbi -= 3;
            switch (thing)
            {
                case FLVER.Dummy d:
                    if (useVectorMode) d.Forward = CreateTranslationVector(d.Forward.X, d.Forward.Y, d.Forward.Z, offset, nbi);
                    else d.Position = CreateScaleVector(d.Position.X, d.Position.Y, d.Position.Z, offset, totals, nbi, uniform, invert);
                    break;
                case FLVER.Vertex v:
                    v.Position = CreateScaleVector(v.Position.X, v.Position.Y, v.Position.Z, offset, totals, nbi, uniform, invert);
                    v.Normal = v.Normal with { Z = invert && nbi != 2 ? -v.Normal.Z : v.Normal.Z };
                    if (v.Tangents.Count > 0) v.Tangents[0] = new Vector4(v.Tangents[0].X, v.Tangents[0].Y, invert && nbi != 2 ? -v.Normal.Z : v.Normal.Z, v.Tangents[0].W);
                    break;
            }
        }

        public static void RotateThing(dynamic thing, float offset, IReadOnlyList<float> totals, int nbi, bool useVectorMode)
        {
            if (nbi >= 6 && nbi <= 8) nbi -= 6;
            float newX = nbi == 0 ? offset : 0;
            float newY = nbi == 1 ? offset : 0;
            float newZ = nbi == 2 ? offset : 0;
            switch (thing)
            {
                case FLVER.Dummy d:
                    if (useVectorMode) d.Forward = Program.RotatePoint(d.Forward, newX, newZ, newY);
                    else d.Position = CreateRotationVector(d.Position.X, d.Position.Y, d.Position.Z, 0, offset, totals, nbi);
                    break;
                case FLVER.Vertex v:
                    v.Position = CreateRotationVector(v.Position.X, v.Position.Y, v.Position.Z, 0, offset, totals, nbi);
                    v.Normal = CreateRotationVector(v.Normal.X, v.Normal.Y, v.Normal.Z, 0, offset, new float[3], nbi);
                    float tangentW = v.Tangents[0].W == 0 ? -1 : v.Tangents[0].W;
                    if (v.Tangents.Count > 0) v.Tangents[0] = CreateRotationVector(v.Tangents[0].X, v.Tangents[0].Y, v.Tangents[0].Z, tangentW, offset, new float[3], nbi);
                    break;
            }
        }

    }
}
