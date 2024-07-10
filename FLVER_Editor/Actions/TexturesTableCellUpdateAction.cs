using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor.Actions;

public class TexturesTableCellUpdateAction : TransformAction
{
    private readonly FLVER2 flver;
    private readonly int selectedMaterialIndex;
    private readonly int row;
    private readonly int col;
    private readonly string textureTableValue;
    private readonly Action refresher;
    private string textureTableOldValue;

    public TexturesTableCellUpdateAction(FLVER2 flver, int selectedMaterialIndex, int row, int col, string textureTableValue, Action refresher)
    {
        this.flver = flver;
        this.selectedMaterialIndex = selectedMaterialIndex;
        this.row = row;
        this.col = col;
        this.textureTableValue = textureTableValue;
        this.refresher = refresher;
    }

    public override void Execute()
    {
        switch (col)
        {
            case 0:
                textureTableOldValue = flver.Materials[selectedMaterialIndex].Textures[row].Type;
                flver.Materials[selectedMaterialIndex].Textures[row].Type = textureTableValue;
                break;
            case 1:
                textureTableOldValue = flver.Materials[selectedMaterialIndex].Textures[row].Path;
                flver.Materials[selectedMaterialIndex].Textures[row].Path = textureTableValue;
                break;
        }

        refresher.Invoke();
    }

    public override void Undo()
    {
        switch (col)
        {
            case 0:
                flver.Materials[selectedMaterialIndex].Textures[row].Type = textureTableOldValue;
                break;
            case 1:
                flver.Materials[selectedMaterialIndex].Textures[row].Path = textureTableOldValue;
                break;
        }
    
        refresher.Invoke();
    }
}
