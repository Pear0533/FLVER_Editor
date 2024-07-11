using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor.Actions;

public class ReverseFaceSetsAction : TransformAction
{
    private readonly IEnumerable<FLVER2.FaceSet> facesets;
    private readonly Action refresher;

    public ReverseFaceSetsAction(List<FLVER2.FaceSet> facesets, Action refresher)
    {
        this.facesets = facesets;
        this.refresher = refresher;
    }

    public override void Execute()
    {
        ReverseFaceSet();
        refresher.Invoke();
    }

    private void ReverseFaceSet()
    {
        foreach (FLVER2.FaceSet fs in facesets)
        {
            for (int j = 0; j < fs.Indices.Count; j += 3)
            {
                if (j > fs.Indices.Count - 2) continue;
                (fs.Indices[j + 1], fs.Indices[j + 2]) = (fs.Indices[j + 2], fs.Indices[j + 1]);
            }
        }
    }

    public override void Undo()
    {
        ReverseFaceSet();
        refresher.Invoke();
    }
}
