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

    public AddNewDummyAction(Action refresher)
    {
        this.refresher = refresher;
    }

    public override void Execute()
    {
        FLVER.Dummy newDummy = new()
        {
            Position = MainWindow.Flver.Dummies.Count > 0 ? MainWindow.Flver.Dummies[MainWindow.Flver.Dummies.Count - 1].Position : new Vector3(0, 0, 0),
            ReferenceID = -1
        };

        MainWindow.Flver.Dummies.Add(newDummy);

        refresher.Invoke();
    }

    public override void Undo()
    {
        MainWindow.Flver.Dummies.RemoveAt(MainWindow.Flver.Dummies.Count - 1);
        refresher.Invoke();
    }
}
