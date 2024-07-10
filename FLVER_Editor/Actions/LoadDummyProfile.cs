using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor.Actions;

public class LoadDummyProfile : TransformAction
{
    private readonly List<FLVER.Dummy> newDummies;
    private readonly Action refresher;
    private readonly List<FLVER.Dummy> oldDummies;

    public LoadDummyProfile(List<FLVER.Dummy> newDummies, List<FLVER.Dummy> oldDummies, Action refresher)
    {
        this.newDummies = newDummies;
        this.newDummies = oldDummies;
        this.refresher = refresher;
    }

    public override void Execute()
    {
        MainWindow.Flver.Dummies = newDummies;
        refresher.Invoke();
    }

    public override void Undo()
    {
        MainWindow.Flver.Dummies = oldDummies;
        refresher.Invoke();
    }
}
