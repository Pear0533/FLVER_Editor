﻿using System.Collections.Generic;
using SoulsAssetPipeline.FLVERImporting;

namespace FbxImporter.ViewModels;

public record MeshImportOptions
{

    public bool MirrorZ { get; init; } = false;
    
    public bool FlipFaces { get; set; } = false;

    public string MTD { get; set; } = null!;

    public FLVER2MaterialInfoBank MaterialInfoBank { get; init; } = null!;

    public WeightingMode Weighting { get; set; } = WeightingMode.Skin;

    public MeshImportOptions() { }

    public MeshImportOptions(string mtd, FLVER2MaterialInfoBank infoBank, WeightingMode weighting)
    {
        MTD = mtd;
        MaterialInfoBank = infoBank;
        Weighting = weighting;
    }
}

public class WeightingMode
{
    private WeightingMode(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public static readonly WeightingMode Static = new("Static", "Used for non-animated meshes.");
    public static readonly WeightingMode Single = new("Single Weight", "Used for meshes which are only weighted to a single bone. Assumes that the mesh is initially located at the origin and applies the bone transform.");
    public static readonly WeightingMode Skin = new("Skin", "Used for meshes with vertices weighted to multiple bones (up to a maximum of 4). Assumes that the mesh is in bind pose.");
    public static readonly List<WeightingMode> Values = new()
    {
        Skin,
        Single,
        Static
    };

    public static WeightingMode Convert(string? value)
    {
        WeightingMode mode = value switch
        {
            "Static" => Static,
            "Single Weight" => Single,
            "Skin" => Skin,
            _ => Skin
        };
        return mode;
    }
    
    public string Name { get; init; }
    public string Description { get; init; }
}