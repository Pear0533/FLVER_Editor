using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor.Actions
{
    public class SolveAllMeshLODsAction : TransformAction
    {
        private readonly IEnumerable<FLVER2.Mesh> targetMeshes;
        private Dictionary<FLVER2.Mesh, List<FLVER2.FaceSet>> faces = new();

        public SolveAllMeshLODsAction(IEnumerable<FLVER2.Mesh> targetMeshes)
        {
            this.targetMeshes = targetMeshes;
        }

        private static void AddNewMeshFaceset(FLVER2.Mesh m, FLVER2.FaceSet.FSFlags flags, List<int> vertexIndices)
        {
            FLVER2.FaceSet fs = Generators.GenerateBasicFaceSet();
            fs.Flags = flags;
            fs.Indices = vertexIndices;
            m.FaceSets.Add(fs);
        }

        public override void Execute()
        {
            FLVER2.FaceSet.FSFlags[] faceSetFlags =
            {
                FLVER2.FaceSet.FSFlags.None,
                FLVER2.FaceSet.FSFlags.LodLevel1,
                FLVER2.FaceSet.FSFlags.LodLevel2,
                FLVER2.FaceSet.FSFlags.MotionBlur,
                FLVER2.FaceSet.FSFlags.MotionBlur | FLVER2.FaceSet.FSFlags.LodLevel1,
                FLVER2.FaceSet.FSFlags.MotionBlur | FLVER2.FaceSet.FSFlags.LodLevel2
            };

            foreach (FLVER2.Mesh m in targetMeshes)
            {
                List<int> vertexIndices = m.FaceSets[0].Indices;
                var oldFaceSets = new List<FLVER2.FaceSet>();

                foreach (var facesets in m.FaceSets)
                {
                    oldFaceSets.Add(facesets);
                }
                faces.Add(m, oldFaceSets);
                m.FaceSets.Clear();
                foreach (FLVER2.FaceSet.FSFlags flag in faceSetFlags)
                    AddNewMeshFaceset(m, flag, vertexIndices);
            }
        }

        public override void Undo()
        {
            foreach (FLVER2.Mesh m in targetMeshes)
            {
                var faceSets = faces[m];

                m.FaceSets.Clear();
                foreach (var faceSet in faceSets)
                {
                    m.FaceSets.Add(faceSet);
                }
            }
        }
    }
}
