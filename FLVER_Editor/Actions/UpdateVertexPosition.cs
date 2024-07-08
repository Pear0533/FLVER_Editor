using Newtonsoft.Json;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor.Actions
{
    public class UpdateVertexPosition : TransformAction
    {
        private readonly FLVER.Vertex vertex;
        private readonly Action refresher;
        private readonly Vector3 oldPosition;
        private readonly Vector3 newPosition;

        public UpdateVertexPosition(Vector3 newPosition, FLVER.Vertex vertex, Action refresher)
        {
            this.oldPosition = vertex.Position;
            this.newPosition = newPosition;
            this.vertex = vertex;
            this.refresher = refresher;
        }

        public override void Execute()
        {
            vertex.Position = newPosition;
            refresher.Invoke();
        }

        public override void Undo()
        {
            vertex.Position = oldPosition;
            refresher.Invoke();
        }
    }
}
