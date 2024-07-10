using SharpDX.MediaFoundation;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FLVER_Editor.Actions;

public class MeshTansformAction : TransformAction
{
    private readonly Action<float, bool> refresher;
    private readonly FLVER2 flver;
    private readonly IEnumerable<int> selectedMeshes;
    private readonly IEnumerable<int> selectedDummies;
    private readonly float offset;
    private readonly IReadOnlyList<float> totals;
    private readonly int nbi;
    private readonly float oldValue;
    private readonly float newValue;
    private readonly bool uniform;
    private readonly bool vectorMode;

    public MeshTansformAction(FLVER2 flver, IEnumerable<int> selectedMeshes, IEnumerable<int> selectedDummies, float offset, IReadOnlyList<float> totals, int nbi, float oldValue, float newValue, bool uniform, bool vectorMode, Action<float, bool> refresher)
    {
        this.flver = flver;
        this.selectedMeshes = selectedMeshes;
        this.selectedDummies = selectedDummies;
        this.offset = offset;
        this.totals = totals;
        this.nbi = nbi;
        this.oldValue = oldValue;
        this.newValue = newValue;
        this.uniform = uniform;
        this.vectorMode = vectorMode;
        this.refresher = refresher;
    }

    private void TransformThing(dynamic thing, float offset, IReadOnlyList<float> totals, int nbi, bool uniform, bool vectorMode)
    {
        switch (nbi)
        {
            case 0:
            case 1:
            case 2:
                Transform3DOperations.TranslateThing(thing, offset / 55, nbi);
                break;
            case 3:
            case 4:
            case 5:
                Transform3DOperations.ScaleThing(thing, offset, totals, nbi, uniform, false, vectorMode);
                break;
            case 6:
            case 7:
            case 8:
                Transform3DOperations.RotateThing(thing, offset, totals, nbi, vectorMode);
                break;
        }
    }

    public override void Execute()
    {
        foreach (var meshIndex in selectedMeshes)
        {
            var mesh = flver.Meshes[meshIndex];

            foreach (FLVER.Vertex v in mesh.Vertices)
                TransformThing(v, offset, totals, nbi, uniform, vectorMode);
        }

        foreach (int i in selectedDummies)
            TransformThing(flver.Dummies[i], offset, totals, nbi, uniform, vectorMode);

        refresher?.Invoke(newValue, uniform);
    }

    public override void Undo()
    {
        foreach (var meshIndex in selectedMeshes)
        {
            var mesh = flver.Meshes[meshIndex];

            foreach (FLVER.Vertex v in mesh.Vertices)
                TransformThing(v, -offset, totals, nbi, uniform, vectorMode);
        }

        foreach (int i in selectedDummies)
            TransformThing(flver.Dummies[i], -offset, totals, nbi, uniform, vectorMode);

        refresher?.Invoke(oldValue, uniform);
    }
}
