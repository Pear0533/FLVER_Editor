using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor.Actions
{
    public class SetAllBBsMaxSizeAction : TransformAction
    {
        private record BoundingRange(Vector3 Max, Vector3 Min);

        private BoundingRange HeaderRange = new(Vector3.Zero, Vector3.Zero);
        private Dictionary<FLVER2.Mesh, BoundingRange?> oldBoxes = new();
        private readonly FLVER2 flver;
        private readonly Action refresher;

        public SetAllBBsMaxSizeAction(FLVER2 flver, Action refresher)
        {
            this.flver = flver;
            this.refresher = refresher;
        }

        public override void Execute()
        {
            Vector3 minVector = new(0, 0, 0);
            Vector3 maxVector = new(999, 999, 999);

            HeaderRange = new(flver.Header.BoundingBoxMax, flver.Header.BoundingBoxMin);

            flver.Header.BoundingBoxMin = maxVector;
            flver.Header.BoundingBoxMax = minVector;
            foreach (var mesh in flver.Meshes)
            {
                oldBoxes.Add(mesh, mesh.BoundingBox is null ? null : new BoundingRange(mesh.BoundingBox.Max, mesh.BoundingBox.Min));
                mesh.BoundingBox ??= new FLVER2.Mesh.BoundingBoxes();
                mesh.BoundingBox.Min = maxVector;
                mesh.BoundingBox.Max = minVector;
            }

            refresher.Invoke();
        }

        public override void Undo()
        {
            flver.Header.BoundingBoxMin = HeaderRange.Min;
            flver.Header.BoundingBoxMax = HeaderRange.Max;
            foreach (FLVER2.Mesh mesh in from mesh in flver.Meshes from vertex in mesh.Vertices select mesh)
            {
                var box = oldBoxes[mesh];

                if (box is null)
                {
                    mesh.BoundingBox = null;
                }
                else if (mesh.BoundingBox is not null)
                {
                    mesh.BoundingBox.Min = box.Min;
                    mesh.BoundingBox.Max = box.Max;
                }
            }

            refresher.Invoke();
        }
    }
}
