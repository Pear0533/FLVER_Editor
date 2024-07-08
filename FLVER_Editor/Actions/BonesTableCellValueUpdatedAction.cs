using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor.Actions;

public class BonesTableCellValueUpdatedAction : TransformAction
{
    private readonly string bonesTableValue;
    private readonly int row;
    private readonly int col;
    private readonly Action refresher;
    private string oldValue = string.Empty;

    public BonesTableCellValueUpdatedAction(string bonesTableValue, int row, int col, Action refresher)
    {
        this.bonesTableValue = bonesTableValue;
        this.row = row;
        this.col = col;
        this.refresher = refresher;
    }
    public override void Execute()
    {
        switch (col)
        {
            case 1:
                oldValue = MainWindow.Flver.Bones[row].Name;
                MainWindow.Flver.Bones[row].Name = bonesTableValue;
                break;
            case 2:
                oldValue = MainWindow.Flver.Bones[row].ParentIndex.ToString();
                MainWindow.Flver.Bones[row].ParentIndex = short.Parse(bonesTableValue);
                break;
            case 3:
                oldValue = MainWindow.Flver.Bones[row].ChildIndex.ToString();
                MainWindow.Flver.Bones[row].ChildIndex = short.Parse(bonesTableValue);
                break;
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
                string[] comp = bonesTableValue.Split(',');


                Vector3 vector = new(float.Parse(comp[0]), float.Parse(comp[1]), float.Parse(comp[2]));

                switch (col)
                {
                    case 4:
                        oldValue = MainWindow.Flver.Bones[row].Translation.toCSV();
                        MainWindow.Flver.Bones[row].Translation = col == 4 ? vector : MainWindow.Flver.Bones[row].Translation;
                        break;
                    case 5:
                        oldValue = MainWindow.Flver.Bones[row].Scale.toCSV();
                        MainWindow.Flver.Bones[row].Scale = col == 5 ? vector : MainWindow.Flver.Bones[row].Scale;
                        break;
                    case 6:
                        oldValue = MainWindow.Flver.Bones[row].Rotation.toCSV();
                        MainWindow.Flver.Bones[row].Rotation = col == 6 ? vector : MainWindow.Flver.Bones[row].Rotation;
                        break;
                    case 7:
                        oldValue = MainWindow.Flver.Bones[row].BoundingBoxMin.toCSV();
                        MainWindow.Flver.Bones[row].BoundingBoxMin = col == 7 ? vector : MainWindow.Flver.Bones[row].BoundingBoxMin;
                        break;
                    case 8:
                        oldValue = MainWindow.Flver.Bones[row].BoundingBoxMax.toCSV();
                        MainWindow.Flver.Bones[row].BoundingBoxMax = col == 8 ? vector : MainWindow.Flver.Bones[row].BoundingBoxMax;
                        break;
                }
                break;
        }

        refresher.Invoke();
    }

    public override void Undo()
    {
        switch (col)
        {
            case 1:
                MainWindow.Flver.Bones[row].Name = oldValue;
                break;
            case 2:
                MainWindow.Flver.Bones[row].ParentIndex = short.Parse(oldValue);
                break;
            case 3:
                MainWindow.Flver.Bones[row].ChildIndex = short.Parse(oldValue);
                break;
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
                string[] comp = oldValue.Split(',');


                Vector3 vector = new(float.Parse(comp[0]), float.Parse(comp[1]), float.Parse(comp[2]));

                switch (col)
                {
                    case 4:
                        MainWindow.Flver.Bones[row].Translation = col == 4 ? vector : MainWindow.Flver.Bones[row].Translation;
                        break;
                    case 5:
                        MainWindow.Flver.Bones[row].Scale = col == 5 ? vector : MainWindow.Flver.Bones[row].Scale;
                        break;
                    case 6:
                        MainWindow.Flver.Bones[row].Rotation = col == 6 ? vector : MainWindow.Flver.Bones[row].Rotation;
                        break;
                    case 7:
                        MainWindow.Flver.Bones[row].BoundingBoxMin = col == 7 ? vector : MainWindow.Flver.Bones[row].BoundingBoxMin;
                        break;
                    case 8:
                        MainWindow.Flver.Bones[row].BoundingBoxMax = col == 8 ? vector : MainWindow.Flver.Bones[row].BoundingBoxMax;
                        break;
                }
                break;
        }

        refresher.Invoke();
    }
}


internal static class VectorUtil
{
    internal static string toCSV(this Vector3 vector)
    {
        return string.Join(", ", vector[0], vector[1], vector[2]);
    }
}