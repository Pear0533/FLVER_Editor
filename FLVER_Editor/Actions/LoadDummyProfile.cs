using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor.Actions;

public class LoadDummyProfile : TransformAction
{
    private readonly FLVER2 flver;
    private readonly List<FLVER.Dummy> newDummies;
    private readonly Action refresher;
    private readonly List<FLVER.Dummy> oldDummies;

    public LoadDummyProfile(FLVER2 flver, List<FLVER.Dummy> newDummies, List<FLVER.Dummy> oldDummies, Action refresher)
    {
        this.flver = flver;
        this.newDummies = newDummies;
        this.oldDummies = oldDummies;
        this.refresher = refresher;
    }

    public override void Execute()
    {
        flver.Dummies = newDummies;
        refresher.Invoke();
    }

    public override void Undo()
    {
        flver.Dummies = oldDummies;
        refresher.Invoke();
    }
}
