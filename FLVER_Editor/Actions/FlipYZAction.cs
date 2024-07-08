using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor.Actions
{
    public class FlipYZAction : TransformAction
    {
        private readonly IEnumerable<FLVER.Vertex> targetVertices;
        private readonly IEnumerable<FLVER.Dummy> targetDummies;
        private readonly Action refresher;

        public FlipYZAction(IEnumerable<FLVER.Vertex> targetVertices, IEnumerable<FLVER.Dummy> targetDummies, Action refresher)
        {
            this.targetVertices = targetVertices;
            this.targetDummies = targetDummies;
            this.refresher = refresher;
        }

        private void FlipYZAxis()
        {
            foreach (FLVER.Vertex v in targetVertices)
            {
                v.Position = new Vector3(v.Position.X, v.Position.Z, v.Position.Y);
                v.Normal = new Vector3(v.Normal.X, v.Normal.Z, v.Normal.Y);
                if (v.Tangents.Count > 0) v.Tangents[0] = new Vector4(v.Tangents[0].X, v.Tangents[0].Z, v.Tangents[0].Y, 1);
            }
            foreach (FLVER.Dummy d in targetDummies)
                d.Position = new Vector3(d.Position.X, d.Position.Z, d.Position.Y);
        }

        public override void Execute()
        {
            FlipYZAxis();
            refresher.Invoke();
        }

        public override void Undo()
        {
            FlipYZAxis();
            refresher.Invoke();
        }
    }
}
