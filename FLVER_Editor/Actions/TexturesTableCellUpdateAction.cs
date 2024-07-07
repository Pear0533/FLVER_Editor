using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor.Actions;

public class TexturesTableCellUpdateAction : TransformAction
{
    private readonly int selectedMaterialIndex;
    private readonly int row;
    private readonly int col;
    private readonly string textureTableValue;
    private readonly Action refresher;
    private string textureTableOldValue;

    public TexturesTableCellUpdateAction(int selectedMaterialIndex, int row, int col, string textureTableValue,Action refresher)
    {
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
                textureTableOldValue = MainWindow.Flver.Materials[selectedMaterialIndex].Textures[row].Type;
                MainWindow.Flver.Materials[selectedMaterialIndex].Textures[row].Type = textureTableValue;
                break;
            case 1:
                textureTableOldValue = MainWindow.Flver.Materials[selectedMaterialIndex].Textures[row].Path;
                MainWindow.Flver.Materials[selectedMaterialIndex].Textures[row].Path = textureTableValue;
                break;
        }

        refresher.Invoke();
    }

    public override void Undo()
    {
        switch (col)
        {
            case 0:
                MainWindow.Flver.Materials[selectedMaterialIndex].Textures[row].Type = textureTableOldValue;
                break;
            case 1:
                MainWindow.Flver.Materials[selectedMaterialIndex].Textures[row].Path = textureTableOldValue;
                break;
        }
    
        refresher.Invoke();
    }
}
