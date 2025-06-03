using System;
using System.Collections.Generic;

namespace SoulsFormats
{
    /// <summary>
    /// A 3d resource list of some kind that also contains metadata.<br/>
    /// This variant is used in ACV and ACVD.
    /// </summary>
    public class MLB_AC5 : SoulsFile<MLB_AC5>, IMLB
    {
        /// <summary>
        /// The type of resource referenced by the MLB.
        /// </summary>
        public ResourceType Type { get; set; }

        /// <summary>
        /// The resources referenced by this MLB.
        /// </summary>
        public List<IMlbResource> Resources { get; set; }

        /// <summary>
        /// Create a new <see cref="MLB_AC4"/>.
        /// </summary>
        public MLB_AC5()
        {
            Type = ResourceType.Model;
            Resources = new List<IMlbResource>();
        }

        #region Read

        /// <inheritdoc/>
        protected override void Read(BinaryReaderEx br)
        {
            br.BigEndian = true;
            int entriesOffset = br.ReadInt32();
            int entriesCount = br.ReadInt32();
            Type = br.ReadEnum32<ResourceType>();
            br.AssertInt32(0);

            br.Position = entriesOffset;
            int[] entryOffsets = br.ReadInt32s(entriesCount);

            Resources = new List<IMlbResource>(entriesCount);
            for (int i = 0; i < entriesCount; i++)
            {
                int entryOffset = entryOffsets[i];
                br.Position = entryOffset;
                switch (Type)
                {
                    case ResourceType.Model:
                        Resources.Add(new Model(br));
                        break;
                    default:
                        throw new NotSupportedException($"{nameof(ResourceType)} {Type} is not supported or is invalid.");
                }
            }
        }

        #endregion

        #region Write

        /// <inheritdoc/>
        protected override void Write(BinaryWriterEx bw)
        {
            bw.BigEndian = true;
            bw.ReserveInt32("EntriesOffset");
            bw.WriteInt32(Resources.Count);
            bw.WriteInt32((int)Type);
            bw.WriteInt32(0);

            bw.FillInt32("EntriesOffset", (int)bw.Position);
            for (int i = 0; i < Resources.Count; i++)
            {
                bw.ReserveInt32($"EntryOffset_{i}");
            }

            for (int i = 0; i < Resources.Count; i++)
            {
                var resource = Resources[i];
                bw.FillInt32($"EntryOffset_{i}", (int)bw.Position);
                switch (Type)
                {
                    case ResourceType.Model:
                        if (resource is Model model)
                        {
                            model.Write(bw);
                        }
                        else
                        {
                            throw new NotSupportedException($"{nameof(IMlbResource)} {resource.GetType().Name} is not supported or is invalid.");
                        }
                        break;
                    default:
                        throw new NotSupportedException($"{nameof(ResourceType)} {Type} is not supported or is invalid.");
                }
            }
        }

        #endregion

        #region Resource

        /// <summary>
        /// MLB resource types.
        /// </summary>
        public enum ResourceType : int
        {
            /// <summary>
            /// The MLB references models.
            /// </summary>
            Model = 4,
        }

        #endregion

        #region Model

        /// <summary>
        /// A model resource.
        /// </summary>
        public class Model : IMlbResource
        {
            /// <summary>
            /// The full path to the resource.
            /// </summary>
            public string Path { get; set; }

            /// <summary>
            /// The relative path to the resource.
            /// </summary>
            public string RelativePath { get; set; }

            /// <summary>
            /// The materials referenced.
            /// </summary>
            public List<Material> Materials { get; set; }

            /// <summary>
            /// The bones referenced.
            /// </summary>
            public List<Bone> Bones { get; set; }

            /// <summary>
            /// Configures LOD.
            /// </summary>
            public ModelLodConfig LodConfig { get; set; }

            /// <summary>
            /// Unknown; Configures break models somehow.
            /// </summary>
            public ModelBreakConfig BreakConfig { get; set; }

            /// <summary>
            /// Unknown; Configures shadow meshes somehow.
            /// </summary>
            public ModelShadowConfig ShadowConfig { get; set; }

            /// <summary>
            /// Unknown; Configures collisions somehow.
            /// </summary>
            public ModelCollisionConfig CollisionConfig { get; set; }

            /// <summary>
            /// Whether or not <see cref="LodConfig"/> is present.
            /// </summary>
            public bool HasLodConfig { get; set; }

            /// <summary>
            /// Whether or not <see cref="BreakConfig"/> is present.
            /// </summary>
            public bool HasBreakConfig { get; set; }

            /// <summary>
            /// Whether or not <see cref="ShadowConfig"/> is present.
            /// </summary>
            public bool HasShadowConfig { get; set; }

            /// <summary>
            /// Whether or not <see cref="CollisionConfig"/> is present.
            /// </summary>
            public bool HasCollisionConfig { get; set; }

            /// <summary>
            /// Creates a new <see cref="Model"/>.
            /// </summary>
            public Model()
            {
                Path = string.Empty;
                RelativePath = string.Empty;
                Materials = new List<Material>();
                Bones = new List<Bone>();
                LodConfig = new ModelLodConfig();
                BreakConfig = new ModelBreakConfig();
                ShadowConfig = new ModelShadowConfig();
                CollisionConfig = new ModelCollisionConfig();
                HasLodConfig = false;
                HasBreakConfig = false;
                HasShadowConfig = false;
                HasCollisionConfig = false;
            }

            /// <summary>
            /// Reads from a stream.
            /// </summary>
            /// <param name="br">The stream reader.</param>
            internal Model(BinaryReaderEx br)
            {
                long start = br.Position;

                int pathOffset = br.ReadInt32();
                int relativePathOffset = br.ReadInt32();
                int materialOffset = br.ReadInt32();
                int materialCount = br.ReadInt32();
                br.AssertInt32(0);
                br.AssertInt32(0);
                int boneOffset = br.ReadInt32();
                int boneCount = br.ReadInt32();
                int offsetLod = br.ReadInt32();
                int offsetBreak = br.ReadInt32();
                int offsetShadow = br.ReadInt32();
                int offsetCollision = br.ReadInt32();
                br.AssertPattern(80, 0);

                Path = br.GetShiftJIS(start + pathOffset);
                RelativePath = br.GetShiftJIS(start + relativePathOffset);

                Materials = new List<Material>(materialCount);
                if (materialOffset > 0 && materialCount > 0)
                {
                    int[] materialOffsets = br.GetInt32s(materialOffset, materialCount);
                    foreach (int offset in materialOffsets)
                    {
                        br.Position = offset;
                        Materials.Add(new Material(br));
                    }
                }

                Bones = new List<Bone>(boneCount);
                if (boneOffset > 0 && boneCount > 0)
                {
                    int[] boneOffsets = br.GetInt32s(boneOffset, boneCount);
                    for (int i = 0; i < boneOffsets.Length; i++)
                    {
                        int offset = boneOffsets[i];
                        br.Position = offset;
                        Bones.Add(new Bone(br));
                    }
                }

                if (offsetLod > 0)
                {
                    br.Position = start + offsetLod;
                    LodConfig = new ModelLodConfig(br);
                    HasLodConfig = true;
                }
                else
                {
                    LodConfig = new ModelLodConfig();
                    HasLodConfig = false;
                }

                if (offsetBreak > 0)
                {
                    br.Position = start + offsetBreak;
                    BreakConfig = new ModelBreakConfig(br);
                    HasBreakConfig = true;
                }
                else
                {
                    BreakConfig = new ModelBreakConfig();
                    HasBreakConfig = false;
                }

                if (offsetShadow > 0)
                {
                    br.Position = start + offsetShadow;
                    ShadowConfig = new ModelShadowConfig(br);
                    HasShadowConfig = true;
                }
                else
                {
                    ShadowConfig = new ModelShadowConfig();
                    HasShadowConfig = false;
                }

                if (offsetCollision > 0)
                {
                    br.Position = start + offsetCollision;
                    CollisionConfig = new ModelCollisionConfig(br);
                    HasCollisionConfig = true;
                }
                else
                {
                    CollisionConfig = new ModelCollisionConfig();
                    HasCollisionConfig = false;
                }
            }

            /// <summary>
            /// Writes to a stream.
            /// </summary>
            /// <param name="bw">The stream writer.</param>
            internal void Write(BinaryWriterEx bw)
            {
                long start = bw.Position;
                bw.ReserveInt32("PathOffset");
                bw.ReserveInt32("RelativePathOffset");
                bw.ReserveInt32("MaterialOffset");
                bw.WriteInt32(Materials.Count);
                bw.WriteInt32(0);
                bw.WriteInt32(0);
                bw.ReserveInt32("BoneOffset");
                bw.WriteInt32(Bones.Count);
                bw.ReserveInt32("OffsetModelLod");
                bw.ReserveInt32("OffsetModelBreak");
                bw.ReserveInt32("OffsetModelShadow");
                bw.ReserveInt32("OffsetModelCollision");
                bw.WritePattern(80, 0);

                bw.FillInt32("PathOffset", (int)(bw.Position - start));
                bw.WriteShiftJIS(Path, true);

                bw.FillInt32("RelativePathOffset", (int)(bw.Position - start));
                bw.WriteShiftJIS(RelativePath, true);
                bw.Pad(4);

                if (HasLodConfig)
                {
                    bw.FillInt32("OffsetModelLod", (int)(bw.Position - start));
                    LodConfig.Write(bw);
                }
                else
                {
                    bw.FillInt32("OffsetModelLod", 0);
                }

                if (HasBreakConfig)
                {
                    bw.FillInt32("OffsetModelBreak", (int)(bw.Position - start));
                    BreakConfig.Write(bw);
                }
                else
                {
                    bw.FillInt32("OffsetModelBreak", 0);
                }

                if (HasShadowConfig)
                {
                    bw.FillInt32("OffsetModelShadow", (int)(bw.Position - start));
                    ShadowConfig.Write(bw);
                }
                else
                {
                    bw.FillInt32("OffsetModelShadow", 0);
                }

                if (HasCollisionConfig)
                {
                    bw.FillInt32("OffsetModelCollision", (int)(bw.Position - start));
                    CollisionConfig.Write(bw);
                }
                else
                {
                    bw.FillInt32("OffsetModelCollision", 0);
                }

                bw.FillInt32("MaterialOffset", (int)bw.Position);
                bool hasMaterials = Materials.Count > 0;
                if (hasMaterials)
                {
                    for (int i = 0; i < Materials.Count; i++)
                    {
                        bw.ReserveInt32($"MaterialOffset_{i}");
                    }
                }

                bw.FillInt32("BoneOffset", (int)bw.Position);
                bool hasBones = Bones.Count > 0;
                if (hasBones)
                {
                    for (int i = 0; i < Bones.Count; i++)
                    {
                        bw.ReserveInt32($"BoneOffset_{i}");
                    }
                }

                if (hasMaterials)
                {
                    for (int i = 0; i < Materials.Count; i++)
                    {
                        bw.FillInt32($"MaterialOffset_{i}", (int)bw.Position);
                        Materials[i].Write(bw);
                    }
                }

                if (hasBones)
                {
                    for (int i = 0; i < Bones.Count; i++)
                    {
                        bw.FillInt32($"BoneOffset_{i}", (int)bw.Position);
                        Bones[i].Write(bw);
                    }
                }
            }

            #region Material

            /// <summary>
            /// A material reference.
            /// </summary>
            public class Material
            {
                /// <summary>
                /// The name of the material.
                /// </summary>
                public string Name { get; set; }

                /// <summary>
                /// Unknown; Parameter data?
                /// </summary>
                public MaterialConfig1 Config1 { get; set; }

                /// <summary>
                /// Whether or not <see cref="Config1"/> is present.
                /// </summary>
                public bool HasConfig1 { get; set; }

                /// <summary>
                /// Create a new <see cref="Material"/>.
                /// </summary>
                public Material()
                {
                    Name = "01 - Default";
                    Config1 = new MaterialConfig1();
                    HasConfig1 = false;
                }

                /// <summary>
                /// Reads from a stream.
                /// </summary>
                /// <param name="br">The stream reader.</param>
                internal Material(BinaryReaderEx br)
                {
                    long start = br.Position;

                    int nameOffset = br.ReadInt32();
                    int offsetConfig1 = br.ReadInt32();
                    br.AssertPattern(24, 0);

                    Name = br.GetShiftJIS(start + nameOffset);

                    if (offsetConfig1 > 0)
                    {
                        br.Position = start + offsetConfig1;
                        Config1 = new MaterialConfig1(br);
                        HasConfig1 = true;
                    }
                    else
                    {
                        Config1 = new MaterialConfig1();
                        HasConfig1 = false;
                    }
                }

                /// <summary>
                /// Writes to a stream.
                /// </summary>
                /// <param name="bw">The stream writer.</param>
                internal void Write(BinaryWriterEx bw)
                {
                    long start = bw.Position;

                    bw.ReserveInt32("NameOffset");
                    bw.ReserveInt32("OffsetConfig1");
                    bw.WritePattern(24, 0);

                    bw.FillInt32("NameOffset", (int)(bw.Position - start));
                    bw.WriteShiftJIS(Name, true);
                    bw.Pad(4);

                    if (HasConfig1)
                    {
                        bw.FillInt32("OffsetConfig1", (int)(bw.Position - start));
                        Config1.Write(bw);
                    }
                    else
                    {
                        bw.FillInt32("OffsetConfig1", 0);
                    }
                }

                #region Configs

                /// <summary>
                /// Unknown; Parameter data?
                /// </summary>
                public class MaterialConfig1
                {
                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public float Unk00 { get; set; }

                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public float Unk04 { get; set; }

                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public float Unk08 { get; set; }

                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public float Unk0C { get; set; }

                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public float Unk10 { get; set; }

                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public float Unk14 { get; set; }

                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public float Unk18 { get; set; }

                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public float Unk1C { get; set; }

                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public float Unk20 { get; set; }

                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public float Unk24 { get; set; }

                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public float Unk28 { get; set; }

                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public float Unk2C { get; set; }

                    /// <summary>
                    /// Creates a new <see cref="MaterialConfig1"/>.
                    /// </summary>
                    public MaterialConfig1()
                    {
                        Unk00 = 0f;
                        Unk04 = 0f;
                        Unk08 = 0f;
                        Unk0C = 0f;
                        Unk10 = 0f;
                        Unk14 = 0f;
                        Unk18 = 0f;
                        Unk1C = 0f;
                        Unk20 = 0f;
                        Unk24 = 0f;
                        Unk28 = 0f;
                        Unk2C = 0f;
                    }

                    /// <summary>
                    /// Reads from a stream.
                    /// </summary>
                    /// <param name="br">The stream reader.</param>
                    internal MaterialConfig1(BinaryReaderEx br)
                    {
                        Unk00 = br.ReadSingle();
                        Unk04 = br.ReadSingle();
                        Unk08 = br.ReadSingle();
                        Unk0C = br.ReadSingle();
                        Unk10 = br.ReadSingle();
                        Unk14 = br.ReadSingle();
                        Unk18 = br.ReadSingle();
                        Unk1C = br.ReadSingle();
                        Unk20 = br.ReadSingle();
                        Unk24 = br.ReadSingle();
                        Unk28 = br.ReadSingle();
                        Unk2C = br.ReadSingle();
                        br.AssertPattern(16, 0);
                    }

                    /// <summary>
                    /// Writes to a stream.
                    /// </summary>
                    /// <param name="bw">The stream writer.</param>
                    internal void Write(BinaryWriterEx bw)
                    {
                        bw.WriteSingle(Unk00);
                        bw.WriteSingle(Unk04);
                        bw.WriteSingle(Unk08);
                        bw.WriteSingle(Unk0C);
                        bw.WriteSingle(Unk10);
                        bw.WriteSingle(Unk14);
                        bw.WriteSingle(Unk18);
                        bw.WriteSingle(Unk1C);
                        bw.WriteSingle(Unk20);
                        bw.WriteSingle(Unk24);
                        bw.WriteSingle(Unk28);
                        bw.WriteSingle(Unk2C);
                        bw.WritePattern(16, 0);
                    }
                }

                #endregion
            }

            #endregion

            #region Bone

            /// <summary>
            /// A bone reference.
            /// </summary>
            public class Bone
            {
                /// <summary>
                /// The name of the bone.
                /// </summary>
                public string Name { get; set; }

                /// <summary>
                /// Unknown; Only present on normal and break model bones.
                /// </summary>
                public BoneBreakConfig BreakConfig { get; set; }

                /// <summary>
                /// Unknown; Only present on collision model bones.
                /// </summary>
                public BoneCollisionConfig CollisionConfig { get; set; }

                /// <summary>
                /// Whether or not <see cref="BreakConfig"/> is present.
                /// </summary>
                public bool HasBreakConfig { get; set; }

                /// <summary>
                /// Whether or not <see cref="CollisionConfig"/> is present.
                /// </summary>
                public bool HasCollisionConfig { get; set; }

                /// <summary>
                /// Create a new <see cref="Bone"/>.
                /// </summary>
                public Bone()
                {
                    Name = "bone";
                    BreakConfig = new BoneBreakConfig();
                    CollisionConfig = new BoneCollisionConfig();
                    HasBreakConfig = false;
                    HasCollisionConfig = false;
                }

                /// <summary>
                /// Reads from a stream.
                /// </summary>
                /// <param name="br">The stream reader.</param>
                internal Bone(BinaryReaderEx br)
                {
                    long start = br.Position;

                    int nameOffset = br.ReadInt32();
                    br.AssertInt32(0);
                    int offsetModel = br.ReadInt32();
                    int offsetCollision = br.ReadInt32();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);

                    Name = br.GetShiftJIS(start + nameOffset);
                    if (offsetModel > 0)
                    {
                        br.Position = start + offsetModel;
                        BreakConfig = new BoneBreakConfig(br);
                        HasBreakConfig = true;
                    }
                    else
                    {
                        BreakConfig = new BoneBreakConfig();
                        HasBreakConfig = false;
                    }

                    if (offsetCollision > 0)
                    {
                        br.Position = start + offsetCollision;
                        CollisionConfig = new BoneCollisionConfig(br);
                        HasCollisionConfig = true;
                    }
                    else
                    {
                        CollisionConfig = new BoneCollisionConfig();
                        HasCollisionConfig = false;
                    }
                }

                /// <summary>
                /// Writes to a stream.
                /// </summary>
                /// <param name="bw">The stream writer.</param>
                internal void Write(BinaryWriterEx bw)
                {
                    long start = bw.Position;

                    bw.ReserveInt32("NameOffset");
                    bw.WriteInt32(0);
                    bw.ReserveInt32("OffsetBoneBreak");
                    bw.ReserveInt32("OffsetBoneCollision");
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);

                    bw.FillInt32("NameOffset", (int)(bw.Position - start));
                    bw.WriteShiftJIS(Name, true);
                    bw.Pad(4);

                    if (HasBreakConfig)
                    {
                        bw.FillInt32("OffsetBoneBreak", (int)(bw.Position - start));
                        BreakConfig.Write(bw);
                    }
                    else
                    {
                        bw.FillInt32("OffsetBoneBreak", 0);
                    }

                    if (HasCollisionConfig)
                    {
                        bw.FillInt32("OffsetBoneCollision", (int)(bw.Position - start));
                        CollisionConfig.Write(bw);
                    }
                    else
                    {
                        bw.FillInt32("OffsetBoneCollision", 0);
                    }
                }

                #region Configs

                /// <summary>
                /// Unknown; Only present on normal and break model bones.
                /// </summary>
                public class BoneBreakConfig
                {
                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public int Unk00 { get; set; }

                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public int Unk04 { get; set; }

                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public byte Unk08 { get; set; }

                    /// <summary>
                    /// Creates a new <see cref="BoneBreakConfig"/>.
                    /// </summary>
                    public BoneBreakConfig()
                    {
                        Unk00 = 0;
                        Unk04 = 0;
                        Unk08 = 0;
                    }

                    /// <summary>
                    /// Reads from a stream.
                    /// </summary>
                    /// <param name="br">The stream reader.</param>
                    internal BoneBreakConfig(BinaryReaderEx br)
                    {
                        Unk00 = br.ReadInt32();
                        Unk04 = br.ReadInt32();
                        Unk08 = br.ReadByte();
                        br.AssertByte(0);
                        br.AssertByte(0);
                        br.AssertByte(0);
                        br.AssertInt32(0);
                        br.AssertInt32(0);
                        br.AssertInt32(0);
                        br.AssertInt32(0);
                        br.AssertInt32(0);
                    }

                    /// <summary>
                    /// Writes to a stream.
                    /// </summary>
                    /// <param name="bw">The stream writer.</param>
                    internal void Write(BinaryWriterEx bw)
                    {
                        bw.WriteInt32(Unk00);
                        bw.WriteInt32(Unk04);
                        bw.WriteByte(Unk08);
                        bw.WriteByte(0);
                        bw.WriteByte(0);
                        bw.WriteByte(0);
                        bw.WriteInt32(0);
                        bw.WriteInt32(0);
                        bw.WriteInt32(0);
                        bw.WriteInt32(0);
                        bw.WriteInt32(0);
                    }
                }

                /// <summary>
                /// Unknown; Only present on collision model bones.
                /// </summary>
                public class BoneCollisionConfig
                {
                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public short Unk00 { get; set; }

                    /// <summary>
                    /// Create a new <see cref="BoneCollisionConfig"/>.
                    /// </summary>
                    public BoneCollisionConfig()
                    {
                        Unk00 = 0;
                    }

                    /// <summary>
                    /// Reads from a stream.
                    /// </summary>
                    /// <param name="br">The stream reader.</param>
                    internal BoneCollisionConfig(BinaryReaderEx br)
                    {
                        Unk00 = br.ReadInt16();
                        br.AssertPattern(30, 0);
                    }

                    /// <summary>
                    /// Writes to a stream.
                    /// </summary>
                    /// <param name="bw">The stream writer.</param>
                    internal void Write(BinaryWriterEx bw)
                    {
                        bw.WriteInt16(Unk00);
                        bw.WritePattern(30, 0);
                    }
                }

                #endregion
            }

            #endregion

            #region Configs

            /// <summary>
            /// Configures LOD.
            /// </summary>
            public class ModelLodConfig
            {
                /// <summary>
                /// Unknown; Assumed to be the distance you need to be to see this model.
                /// </summary>
                public float Distance { get; set; }

                /// <summary>
                /// Creates a new <see cref="ModelLodConfig"/>.
                /// </summary>
                public ModelLodConfig()
                {
                    Distance = 0;
                }

                /// <summary>
                /// Reads from a stream.
                /// </summary>
                /// <param name="br">The stream reader.</param>
                internal ModelLodConfig(BinaryReaderEx br)
                {
                    Distance = br.ReadSingle();
                    br.AssertPattern(28, 0);
                }

                /// <summary>
                /// Writes to a stream.
                /// </summary>
                /// <param name="bw">The stream writer.</param>
                internal void Write(BinaryWriterEx bw)
                {
                    bw.WriteSingle(Distance);
                    bw.WritePattern(28, 0);
                }
            }

            /// <summary>
            /// Unknown; Configures break models somehow.
            /// </summary>
            public class ModelBreakConfig
            {
                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk00 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk04 { get; set; }

                /// <summary>
                /// Creates a new <see cref="ModelBreakConfig"/>.
                /// </summary>
                public ModelBreakConfig()
                {
                    Unk00 = 0;
                    Unk04 = 0;
                }

                /// <summary>
                /// Reads from a stream.
                /// </summary>
                /// <param name="br">The stream reader.</param>
                internal ModelBreakConfig(BinaryReaderEx br)
                {
                    Unk00 = br.ReadInt32();
                    Unk04 = br.ReadInt32();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                }

                /// <summary>
                /// Writes to a stream.
                /// </summary>
                /// <param name="bw">The stream writer.</param>
                internal void Write(BinaryWriterEx bw)
                {
                    bw.WriteInt32(Unk00);
                    bw.WriteInt32(Unk04);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                }
            }

            /// <summary>
            /// Unknown; Configures shadow meshes somehow.
            /// </summary>
            public class ModelShadowConfig
            {
                /// <summary>
                /// Unknown.
                /// </summary>
                public float Unk00 { get; set; }

                /// <summary>
                /// Creates a new <see cref="ModelShadowConfig"/>.
                /// </summary>
                public ModelShadowConfig()
                {
                    Unk00 = 0;
                }

                /// <summary>
                /// Reads from a stream.
                /// </summary>
                /// <param name="br">The stream reader.</param>
                internal ModelShadowConfig(BinaryReaderEx br)
                {
                    Unk00 = br.ReadSingle();
                    br.AssertPattern(28, 0);
                }

                /// <summary>
                /// Writes to a stream.
                /// </summary>
                /// <param name="bw">The stream writer.</param>
                internal void Write(BinaryWriterEx bw)
                {
                    bw.WriteSingle(Unk00);
                    bw.WritePattern(28, 0);
                }
            }

            /// <summary>
            /// Unknown; Configures collisions somehow.
            /// </summary>
            public class ModelCollisionConfig
            {
                /// <summary>
                /// Unknown.
                /// </summary>
                public short Unk00 { get; set; }

                /// <summary>
                /// Creates a new <see cref="ModelCollisionConfig"/>.
                /// </summary>
                public ModelCollisionConfig()
                {
                    Unk00 = 0;
                }

                /// <summary>
                /// Reads from a stream.
                /// </summary>
                /// <param name="br">The stream reader.</param>
                internal ModelCollisionConfig(BinaryReaderEx br)
                {
                    Unk00 = br.ReadInt16();
                    br.AssertPattern(30, 0);
                }

                /// <summary>
                /// Writes to a stream.
                /// </summary>
                /// <param name="bw">The stream writer.</param>
                internal void Write(BinaryWriterEx bw)
                {
                    bw.WriteInt16(Unk00);
                    bw.WritePattern(30, 0);
                }
            }

            #endregion
        }

        #endregion
    }
}
