using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor.Actions
{
    public class ToggleBackFacesAction : TransformAction
    {
        private readonly FLVER2 flver;
        private readonly List<FLVER2.FaceSet> facesets;
        private readonly Action refresher;

        public ToggleBackFacesAction(FLVER2 flver, List<FLVER2.FaceSet> facesets, Action refresher)
        {
            this.flver = flver;
            this.facesets = facesets;
            this.refresher = refresher;
        }

        public override void Execute()
        {
            foreach (FLVER2.FaceSet fs in facesets)
                fs.CullBackfaces = !fs.CullBackfaces;

            refresher.Invoke();
        }

        public override void Undo()
        {
            foreach (FLVER2.FaceSet fs in facesets)
                fs.CullBackfaces = !fs.CullBackfaces;

            refresher.Invoke();
        }
    }
}
