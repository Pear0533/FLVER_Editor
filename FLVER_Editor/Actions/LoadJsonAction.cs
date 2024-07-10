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
        private readonly FLVER2 flver;
        private readonly int type;
        private readonly string jsonText;
        private readonly Action refresher;
        private object oldValue = null!;

        public LoadJsonAction(FLVER2 flver, int type, string jsonText, Action refresher)
        {
            this.flver = flver;
            this.type = type;
            this.jsonText = jsonText;
            this.refresher = refresher;
        }

        public override void Execute()
        {
            switch (type)
            {
                case 0:
                    oldValue = flver.Nodes;
                    flver.Nodes = JsonConvert.DeserializeObject<List<FLVER.Node>>(jsonText);
                    break;
                case 1:
                    oldValue = flver.Materials;
                    flver.Materials = JsonConvert.DeserializeObject<List<FLVER2.Material>>(jsonText);
                    break;
            }

            refresher.Invoke();
        }

        public override void Undo()
        {
            switch (type)
            {
                case 0:
                    flver.Nodes = (List<FLVER.Node>) oldValue;
                    break;
                case 1:
                    flver.Materials = (List<FLVER2.Material>) oldValue;
                    break;
            }

            refresher.Invoke();
        }
    }
}
