using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace FLVER_Editor.Actions
{
    public class ImportSkeletonDataAction : TransformAction
    {
        private readonly FLVER2 flver;
        private readonly List<FLVER.Node> originalNodes;
        private readonly Action refresher;

        public ImportSkeletonDataAction(FLVER2 flver, Action refresher)
        {
            this.flver = flver;
            this.originalNodes = new List<FLVER.Node>(flver.Nodes);
            this.refresher = refresher;
        }

        public override void Execute()
        {
            var importedBoneData = Importer.GetCollectedSkeletonBoneData();
            if (importedBoneData.Count == 0)
            {
                Console.WriteLine("No bone data available to import.");
                return;
            }

            flver.Nodes.Clear();

            foreach (var boneData in importedBoneData)
            {
                flver.Nodes.Add(ConvertToFLVERNode(boneData));
            }

            refresher.Invoke();
        }

        public override void Undo()
        {
            flver.Nodes.Clear();
            flver.Nodes.AddRange(originalNodes);
            refresher.Invoke();
        }

        private FLVER.Node ConvertToFLVERNode(Importer.SkeletonBoneData boneData)
        {
            return new FLVER.Node
            {
                Name = boneData.Name,
                ParentIndex = (short)boneData.ParentId,
                FirstChildIndex = (short)boneData.FirstChildId,
                Translation = new Vector3(boneData.Translation.X, boneData.Translation.Y, boneData.Translation.Z),
                Rotation = new Vector3(boneData.Rotation.X, boneData.Rotation.Y, boneData.Rotation.Z),
                Scale = new Vector3(boneData.Scale.X, boneData.Scale.Y, boneData.Scale.Z),
                BoundingBoxMin = new Vector3(boneData.BBmin.X, boneData.BBmin.Y, boneData.BBmin.Z),
                BoundingBoxMax = new Vector3(boneData.BBmax.X, boneData.BBmax.Y, boneData.BBmax.Z)
            };
        }
    }
}