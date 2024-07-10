using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor.Actions
{
    public class ResetAllMeshesAction : TransformAction
    {
        private record BoundingRange(Vector3 Max, Vector3 Min, Vector3 Ukn);
        private record MeshData(BoundingRange? BoundingBox, int NodeIndex, bool UseBoneWeights, List<FLVER2.FaceSet> FaceSets, List<FLVER.Vertex> Vertices);

        private Dictionary<FLVER2.Mesh, MeshData> oldMeshData = new();

        public override void Execute()
        {
            foreach (FLVER2.Mesh m in MainWindow.Flver.Meshes)
            {
                var oldVertices = new List<FLVER.Vertex>(m.Vertices);

                var oldData = new MeshData(
                    m.BoundingBox is null ? null : new BoundingRange(m.BoundingBox.Max, m.BoundingBox.Min, m.BoundingBox.Unk),
                    m.NodeIndex,
                    m.UseBoneWeights,
                    m.FaceSets,
                    oldVertices
                );

                oldMeshData.Add(m, oldData);

                m.BoundingBox ??= new FLVER2.Mesh.BoundingBoxes();
                m.BoundingBox.Max = new Vector3(1, 1, 1);
                m.BoundingBox.Min = new Vector3(-1, -1, -1);
                m.BoundingBox.Unk = new Vector3();
                m.NodeIndex = 0;
                m.UseBoneWeights = true;
                int[] varray = m.FaceSets[0].Indices.ToArray();
                m.FaceSets = new List<FLVER2.FaceSet>();
                for (int i = 0; i < m.Vertices.Count; i++)
                {
                    FLVER.Vertex vit = m.Vertices[i];
                    m.Vertices[i] = Generators.GenerateNewFlverVertexUsingNumerics(new Vector3(vit.Position.X, vit.Position.Y, vit.Position.Z), vit.Normal, vit.Tangents,
                        vit.Bitangent, vit.UVs, 1);
                    m.Vertices[i].BoneIndices = vit.BoneIndices;
                    m.Vertices[i].BoneWeights = vit.BoneWeights;
                }
                m.FaceSets.Add(Generators.GenerateBasicFaceSet());
                m.FaceSets[0].Indices = varray.ToList();
                m.FaceSets[0].CullBackfaces = false;
            }
        }

        public override void Undo()
        {
            foreach (var data in oldMeshData)
            {
                var m = data.Key;
                var meshData = data.Value;

                m.BoundingBox ??= new FLVER2.Mesh.BoundingBoxes();

                if (meshData.BoundingBox is null)
                {
                    m.BoundingBox = null;
                }
                else
                {
                    m.BoundingBox.Max = meshData.BoundingBox.Max;
                    m.BoundingBox.Min = meshData.BoundingBox.Min;
                    m.BoundingBox.Unk = meshData.BoundingBox.Ukn;
                }

                m.NodeIndex = meshData.NodeIndex;
                m.UseBoneWeights = meshData.UseBoneWeights;

                // we can assign it direction here because Execute completely removes it and assigns a new faceset list
                m.FaceSets = meshData.FaceSets;

                // we must copy the vertex one by one because the Execute overrides it one by one by making new Vertx
                for (int i = 0; i < m.Vertices.Count; i++)
                {
                    m.Vertices[i] = meshData.Vertices[i];
                }
            }
        }
    }
}
