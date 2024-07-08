using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor.Actions
{
    public class MirrorMeshAction : TransformAction
    {
        private readonly IEnumerable<FLVER2.Mesh> targetMeshes;
        private readonly IEnumerable<FLVER.Dummy> targetDummies;
        private readonly int nbi;
        private readonly bool useWorldOrigin;
        private readonly bool vertexMode;
        private readonly Action refresher;
        private readonly float[] totals;

        public MirrorMeshAction(IEnumerable<FLVER2.Mesh> targetMeshes, IEnumerable<FLVER.Dummy> targetDummies, int nbi, float[] totals, bool useWorldOrigin, bool vertexMode, Action refresher)
        {
            this.targetMeshes = targetMeshes;
            this.targetDummies = targetDummies;
            this.nbi = nbi;
            this.useWorldOrigin = useWorldOrigin;
            this.vertexMode = vertexMode;
            this.refresher = refresher;
            this.totals = totals;
        }


        private Vector3 MirrorThing(Vector3 v, int nbi, IReadOnlyList<float> totals)
        {
            v = new Vector3((v.X - (nbi == 0 && !useWorldOrigin ? totals[0] : 0)) * (nbi == 0 ? -1 : 1),
                (v.Y - (nbi == 1 && !useWorldOrigin ? totals[1] : 0)) * (nbi == 1 ? -1 : 1),
                (v.Z - (nbi == 2 && !useWorldOrigin ? totals[2] : 0)) * (nbi == 2 ? -1 : 1));
            v = new Vector3(v.X + (nbi == 0 && !useWorldOrigin ? totals[0] : 0),
                v.Y + (nbi == 1 && !useWorldOrigin ? totals[1] : 0),
                v.Z + (nbi == 2 && !useWorldOrigin ? totals[2] : 0));
            return v;
        }

        public void MirrorMesh()
        {
            foreach (FLVER.Vertex v in targetMeshes.SelectMany(i => i.Vertices))
            {
                v.Position = MirrorThing(v.Position, nbi, totals);
                v.Normal = new Vector3(v.Normal.X * (nbi == 0 ? -1 : 1),
                    v.Normal.Y * (nbi == 1 ? -1 : 1), v.Normal.Z * (nbi == 2 ? -1 : 1));
                if (v.Tangents.Count > 0)
                    v.Tangents[0] = new Vector4(v.Tangents[0].X * (nbi == 0 ? -1 : 1),
                        v.Tangents[0].Y * (nbi == 1 ? -1 : 1), v.Tangents[0].Z * (nbi == 2 ? -1 : 1), 1);
            }
            foreach (FLVER.Dummy d in targetDummies)
            {
                if (vertexMode) d.Forward = MirrorThing(d.Forward, nbi, totals);
                else d.Position = MirrorThing(d.Position, nbi, totals);
            }

            ReverseFaceSetsAction action = new(targetMeshes.SelectMany(x => x.FaceSets), () => { });
            action.Execute();
        }

        public override void Undo()
        {
            MirrorMesh();
            refresher.Invoke();
        }

        public override void Execute()
        {
            MirrorMesh();
            refresher.Invoke();
        }
    }
}
