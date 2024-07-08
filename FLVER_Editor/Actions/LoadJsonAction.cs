using Newtonsoft.Json;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor.Actions
{
    public class LoadJsonAction : TransformAction
    {
        private readonly int type;
        private readonly string jsonText;
        private readonly Action refresher;
        private object oldValue;

        public LoadJsonAction(int type, string jsonText, Action refresher)
        {
            this.type = type;
            this.jsonText = jsonText;
            this.refresher = refresher;
        }

        public override void Execute()
        {
            switch (type)
            {
                case 0:
                    oldValue = MainWindow.Flver.Bones;
                    MainWindow.Flver.Bones = JsonConvert.DeserializeObject<List<FLVER.Bone>>(jsonText);
                    break;
                case 1:
                    oldValue = MainWindow.Flver.Materials;
                    MainWindow.Flver.Materials = JsonConvert.DeserializeObject<List<FLVER2.Material>>(jsonText);
                    break;
            }

            refresher.Invoke();
        }

        public override void Undo()
        {
            switch (type)
            {
                case 0:
                    MainWindow.Flver.Bones = (List<FLVER.Bone>) oldValue;
                    break;
                case 1:
                    MainWindow.Flver.Materials = (List<FLVER2.Material>) oldValue;
                    break;
            }

            refresher.Invoke();
        }
    }
}
