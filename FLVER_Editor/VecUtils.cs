using System.Numerics;
using SoulsFormats;

namespace FLVER_Editor;

public static class VecUtils
{
    // Rotation logic taken from cannon.js:
    // https://github.com/schteppe/cannon.js/blob/master/src/math/Quaternion.js#L249
    public static Vector3 RotateVector(Vector3 v, Vector3 r)
    {
        var quat = Quaternion.CreateFromYawPitchRoll(r.X, r.Y, r.Z);

        var target = new Vector3();

        var x = v.X;
        var y = v.Y;
        var z = v.Z;

        var qx = quat.X;
        var qy = quat.Y;
        var qz = quat.Z;
        var qw = quat.W;

        // q*v
        var ix = qw * x + qy * z - qz * y;
        var iy = qw * y + qz * x - qx * z;
        var iz = qw * z + qx * y - qy * x;
        var iw = -qx * x - qy * y - qz * z;

        target.X = ix * qw + iw * -qx + iy * -qz - iz * -qy;
        target.Y = iy * qw + iw * -qy + iz * -qx - ix * -qz;
        target.Z = iz * qw + iw * -qz + ix * -qy - iy * -qx;

        return target;
    }

    // Offset a vector position by the rotation and translation of a bone it is attached to
    public static Vector3 RecursiveBoneOffset(Vector3 pos, FLVER.Node bone, FLVER2 flver)
    {
        pos = RotateVector(pos, bone.Rotation) + bone.Translation;
        if (bone.ParentIndex >= 0)
            return RecursiveBoneOffset(pos, flver.Nodes[bone.ParentIndex], flver);
        return pos;
    }
}