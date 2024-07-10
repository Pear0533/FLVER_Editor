using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor.Actions
{
    public class DummyTableDataChangedAction : TransformAction
    {
        private readonly FLVER2 flver;
        private readonly short newValue;
        private readonly int row;
        private readonly int col;
        private readonly Action refresher;
        private short oldValue;

        public DummyTableDataChangedAction(FLVER2 flver, short newValue, int row, int col, Action refresher)
        {
            this.flver = flver;
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
                    oldValue = flver.Dummies[row].ReferenceID;
                    flver.Dummies[row].ReferenceID = newValue;
                    break;
                case 2:
                    oldValue = flver.Dummies[row].AttachBoneIndex;
                    flver.Dummies[row].AttachBoneIndex = newValue;
                    break;
                case 3:
                    oldValue = flver.Dummies[row].ParentBoneIndex;
                    flver.Dummies[row].ParentBoneIndex = newValue;
                    break;
            }

            refresher.Invoke();
        }

        public override void Undo()
        {
            switch (col)
            {
                case 1:
                    flver.Dummies[row].ReferenceID = oldValue;
                    break;
                case 2:
                    flver.Dummies[row].AttachBoneIndex = oldValue;
                    break;
                case 3:
                    flver.Dummies[row].ParentBoneIndex = oldValue;
                    break;
            }
         
            refresher.Invoke();
        }
    }
}
