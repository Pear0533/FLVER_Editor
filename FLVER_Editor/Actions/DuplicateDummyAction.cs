using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor.Actions;

public class DuplicateDummyAction : TransformAction
{
    private readonly int rowIndex;
    private readonly FLVER.Dummy dummy;
    private Action? windowRefresh;

    public DuplicateDummyAction(int rowIndex, Action refresh)
    {
        this.rowIndex = rowIndex;

        dummy = new()
        {
            Position = MainWindow.Flver.Dummies[rowIndex].Position,
            Forward = MainWindow.Flver.Dummies[rowIndex].Forward,
            Upward = MainWindow.Flver.Dummies[rowIndex].Upward,
            ReferenceID = MainWindow.Flver.Dummies[rowIndex].ReferenceID,
            ParentBoneIndex = MainWindow.Flver.Dummies[rowIndex].ParentBoneIndex,
            AttachBoneIndex = MainWindow.Flver.Dummies[rowIndex].AttachBoneIndex,
            Color = MainWindow.Flver.Dummies[rowIndex].Color,
            Flag1 = MainWindow.Flver.Dummies[rowIndex].Flag1,
            UseUpwardVector = MainWindow.Flver.Dummies[rowIndex].UseUpwardVector,
            Unk30 = MainWindow.Flver.Dummies[rowIndex].Unk30,
            Unk34 = MainWindow.Flver.Dummies[rowIndex].Unk34
        };

        this.windowRefresh = refresh;
    }

    public override void Execute()
    {
        MainWindow.Flver.Dummies.Add(dummy);
        windowRefresh?.Invoke();
    }

    public override void Undo()
    {
        MainWindow.Flver.Dummies.Remove(dummy);
        windowRefresh?.Invoke();
    }
}
