using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor.Actions
{
    public class CenterMeshToWorldAction : TransformAction
    {
        private IEnumerable<FLVER.Vertex> targetVertices;
        private IEnumerable<FLVER.Dummy> targetDummies;
        private readonly int nbi;
        private readonly float[] totals;
        private readonly Action refresher;

        public CenterMeshToWorldAction(List<FLVER.Vertex> targetVertices, List<FLVER.Dummy> targetDummies, int nbi, float[] totals, Action refresher)
        {
            this.targetVertices = targetVertices;
            this.targetDummies = targetDummies;
            this.nbi = nbi;
            this.totals = totals;
            this.refresher = refresher;
        }

        public override void Execute()
        {
            foreach (FLVER.Vertex v in targetVertices)
                Transform3DOperations.TranslateThing(v, -totals[nbi], nbi);
            foreach (FLVER.Dummy d in targetDummies)
                Transform3DOperations.TranslateThing(d, -totals[nbi], nbi);

            refresher.Invoke();
        }

        public override void Undo()
        {
            foreach (FLVER.Vertex v in targetVertices)
                Transform3DOperations.TranslateThing(v, totals[nbi], nbi);
            foreach (FLVER.Dummy d in targetDummies)
                Transform3DOperations.TranslateThing(d, totals[nbi], nbi);
         
            refresher.Invoke();
        }
    }
}
