using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor.Actions
{
    public class DummyTableDataChangedAction : TransformAction
    {
        private readonly short newValue;
        private readonly int row;
        private readonly int col;
        private readonly Action refresher;
        private short oldValue;

        public DummyTableDataChangedAction(short newValue, int row, int col, Action refresher)
        {
            this.newValue = newValue;
            this.row = row;
            this.col = col;
            this.refresher = refresher;
        }

        public override void Execute()
        {
            switch (col)
            {
                case 1:
                    oldValue = MainWindow.Flver.Dummies[row].ReferenceID;
                    MainWindow.Flver.Dummies[row].ReferenceID = newValue;
                    break;
                case 2:
                    oldValue = MainWindow.Flver.Dummies[row].AttachBoneIndex;
                    MainWindow.Flver.Dummies[row].AttachBoneIndex = newValue;
                    break;
                case 3:
                    oldValue = MainWindow.Flver.Dummies[row].ParentBoneIndex;
                    MainWindow.Flver.Dummies[row].ParentBoneIndex = newValue;
                    break;
            }

            refresher.Invoke();
        }

        public override void Undo()
        {
            switch (col)
            {
                case 1:
                    MainWindow.Flver.Dummies[row].ReferenceID = oldValue;
                    break;
                case 2:
                    MainWindow.Flver.Dummies[row].AttachBoneIndex = oldValue;
                    break;
                case 3:
                    MainWindow.Flver.Dummies[row].ParentBoneIndex = oldValue;
                    break;
            }
         
            refresher.Invoke();
        }
    }
}
