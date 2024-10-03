using FLVER_Editor.Actions.Helpers;
using SoulsFormats;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace FLVER_Editor.Actions
{
    public class DeleteConnectedVerticesAction : TransformAction
    {
        public DeleteConnectedVerticesAction(FLVER2.Mesh mesh, int startVertexIndex, Action refresher)
        {
            this.mesh = mesh;
            this.startVertexIndex = startVertexIndex;
            this.refresher = refresher;
        }

        private readonly FLVER2.Mesh mesh;
        private readonly int startVertexIndex;
        private readonly Action refresher;
        private HashSet<int> verticesToDelete = new HashSet<int>();
        private Dictionary<int, Vector3> oldVertexPositions = new Dictionary<int, Vector3>();
        private Dictionary<FLVER2.FaceSet, Dictionary<int, int>> facesetBackup = new();

        public override void Execute()
        {
            facesetBackup.Clear();
            oldVertexPositions.Clear();

            var queue = new Queue<int>();
            queue.Enqueue(startVertexIndex);

            while (queue.Count > 0)
            {
                int currentIndex = queue.Dequeue();
                if (verticesToDelete.Contains(currentIndex))
                    continue;

                verticesToDelete.Add(currentIndex);
                oldVertexPositions.Add(currentIndex, mesh.Vertices[currentIndex].Position);

                foreach (var faceSet in mesh.FaceSets)
                {
                    var indices = faceSet.Triangulate(false);
                    
                    for (int i = 0; i < indices.Count; i += 3)
                    {
                        int v1 = indices[i];
                        int v2 = indices[i + 1];
                        int v3 = indices[i + 2];

                        if (v1 == currentIndex || v2 == currentIndex || v3 == currentIndex)
                        {
                            if (v1 != currentIndex && !verticesToDelete.Contains(v1))
                                queue.Enqueue(v1);
                            if (v2 != currentIndex && !verticesToDelete.Contains(v2))
                                queue.Enqueue(v2);
                            if (v3 != currentIndex && !verticesToDelete.Contains(v3))
                                queue.Enqueue(v3);
                        }
                    }
                }
            }

            foreach (var index in verticesToDelete.OrderByDescending(i => i))
            {
                VertexDeleteHelper.DeleteMeshVertexFaceSet(mesh, index, facesetBackup);
                mesh.Vertices[index].Position = new System.Numerics.Vector3(0, 0, 0);
            }

            refresher.Invoke();
        }

        public override void Undo()
        {
            VertexDeleteHelper.RestoreMeshVertexFaceSet(facesetBackup);

            foreach (var record in oldVertexPositions)
            {
                var index = record.Key;
                var position = record.Value;

                mesh.Vertices[index].Position = position;
            }

            refresher.Invoke();
        }
    }
}
