﻿using SoulsAssetPipeline.FLVERImporting;

namespace FLVER_Editor.FbxImporter.ViewModels
{
    public class MeshImportOptions
    {
        public bool CreateDefaultBone;

        public string MTD;

        public FLVER2MaterialInfoBank MaterialInfoBank;

        public WeightingMode Weighting;

        public MeshImportOptions(bool createDefaultBone, string mtd, FLVER2MaterialInfoBank infoBank, WeightingMode weighting)
        {
            CreateDefaultBone = createDefaultBone;
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

        public string Name { get; init; }
        public string Description { get; init; }
    }
}