using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor.Actions;

public class BodesTableCellValueUpdatedAction : TransformAction
{
    private readonly FLVER2 flver;
    private readonly string bonesTableValue;
    private readonly int row;
    private readonly int col;
    private readonly Action refresher;
    private string oldValue = string.Empty;

    public BodesTableCellValueUpdatedAction(FLVER2 flver, string bonesTableValue, int row, int col, Action refresher)
    {
        this.flver = flver;
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
                oldValue = flver.Nodes[row].Name;
                flver.Nodes[row].Name = bonesTableValue;
                break;
            case 2:
                oldValue = flver.Nodes[row].ParentIndex.ToString();
                flver.Nodes[row].ParentIndex = short.Parse(bonesTableValue);
                break;
            case 3:
                oldValue = flver.Nodes[row].FirstChildIndex.ToString();
                flver.Nodes[row].FirstChildIndex = short.Parse(bonesTableValue);
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
                        oldValue = flver.Nodes[row].Translation.toCSV();
                        flver.Nodes[row].Translation = col == 4 ? vector : flver.Nodes[row].Translation;
                        break;
                    case 5:
                        oldValue = flver.Nodes[row].Scale.toCSV();
                        flver.Nodes[row].Scale = col == 5 ? vector : flver.Nodes[row].Scale;
                        break;
                    case 6:
                        oldValue = flver.Nodes[row].Rotation.toCSV();
                        flver.Nodes[row].Rotation = col == 6 ? vector : flver.Nodes[row].Rotation;
                        break;
                    case 7:
                        oldValue = flver.Nodes[row].BoundingBoxMin.toCSV();
                        flver.Nodes[row].BoundingBoxMin = col == 7 ? vector : flver.Nodes[row].BoundingBoxMin;
                        break;
                    case 8:
                        oldValue = flver.Nodes[row].BoundingBoxMax.toCSV();
                        flver.Nodes[row].BoundingBoxMax = col == 8 ? vector : flver.Nodes[row].BoundingBoxMax;
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
                flver.Nodes[row].Name = oldValue;
                break;
            case 2:
                flver.Nodes[row].ParentIndex = short.Parse(oldValue);
                break;
            case 3:
                flver.Nodes[row].FirstChildIndex = short.Parse(oldValue);
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
                        flver.Nodes[row].Translation = col == 4 ? vector : flver.Nodes[row].Translation;
                        break;
                    case 5:
                        flver.Nodes[row].Scale = col == 5 ? vector : flver.Nodes[row].Scale;
                        break;
                    case 6:
                        flver.Nodes[row].Rotation = col == 6 ? vector : flver.Nodes[row].Rotation;
                        break;
                    case 7:
                        flver.Nodes[row].BoundingBoxMin = col == 7 ? vector : flver.Nodes[row].BoundingBoxMin;
                        break;
                    case 8:
                        flver.Nodes[row].BoundingBoxMax = col == 8 ? vector : flver.Nodes[row].BoundingBoxMax;
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