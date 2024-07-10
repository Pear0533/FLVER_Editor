using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor.Actions;

public class AddNewDummyAction : TransformAction
{
    private readonly Action refresher;
    private readonly FLVER.Dummy newDummy;

    public AddNewDummyAction(Vector3 position, Action refresher)
    {
        this.refresher = refresher;
        newDummy = new()
        {
            Position = position,
            ReferenceID = -1
        };
    }

    public override void Execute()
    {
        MainWindow.Flver.Dummies.Add(newDummy);
        refresher.Invoke();
    }

    public override void Undo()
    {
        MainWindow.Flver.Dummies.Remove(newDummy);
        refresher.Invoke();
    }
}
