using System;
using System.Collections.Generic;
using System.Linq;

namespace SoulsFormats
{
    /// <summary>
    /// A 3d resource list of some kind that also contains metadata.<br/>
    /// This variant is used in AC4 and ACFA.
    /// </summary>
    public class MLB_AC4 : SoulsFile<MLB_AC4>, IMLB
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
        /// Whether or not the resources are animations.<br/>
        /// Models and animations both share the same <see cref="ResourceType"/> value.<br/>
        /// If there are no entries this cannot be determined properly.
        /// </summary>
        public bool IsAnimation { get; set; }

        /// <summary>
        /// Create a new <see cref="MLB_AC4"/>.
        /// </summary>
        public MLB_AC4()
        {
            Type = ResourceType.Model;
            Resources = new List<IMlbResource>();
            IsAnimation = false;
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

            bool prevIsAnimation = false;
            bool determinedAnimation = false;
            Resources = new List<IMlbResource>(entriesCount);
            for (int i = 0; i < entriesCount; i++)
            {
                int entryOffset = entryOffsets[i];
                if (entryOffset == 0)
                {
                    // o8916 and o9995 have offsets of 0 on some entries
                    Resources.Add(new Dummy());
                    continue;
                }

                br.Position = entryOffset;
                switch (Type)
                {
                    case ResourceType.Model:
                        // Hack to determine whether its a model or an animation
                        int pathOffset = br.GetInt32(br.Position);
                        if (pathOffset == 0x80)
                        {
                            IsAnimation = false;
                            Resources.Add(new Model(br));
                        }
                        else if (pathOffset == 0x14)
                        {
                            IsAnimation = true;
                            Resources.Add(new Animation(br));
                        }
                        else
                        {
                            throw new NotSupportedException($"{nameof(ResourceType.Model)} true type could not be determined.");
                        }

                        if (determinedAnimation && prevIsAnimation != IsAnimation)
                        {
                            throw new NotSupportedException("Mixing models and animations is not supported.");
                        }

                        determinedAnimation = true;
                        prevIsAnimation = IsAnimation;
                        break;
                    case ResourceType.Texture:
                        Resources.Add(new Texture(br));
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
                if (resource is Dummy)
                {
                    // o8916 and o9995 have offsets of 0 on some entries
                    bw.FillInt32($"EntryOffset_{i}", 0);
                    continue;
                }

                bw.FillInt32($"EntryOffset_{i}", (int)bw.Position);
                switch (Type)
                {
                    case ResourceType.Model:
                        if (!IsAnimation)
                        {
                            if (resource is Model model)
                            {
                                model.Write(bw);
                            }
                            else
                            {
                                throw new NotSupportedException($"Specified {nameof(IsAnimation)} {IsAnimation} on {nameof(ResourceType)} {Type} but {nameof(Resources)}[{i}] was not a {nameof(Model)}.");
                            }
                        }
                        else
                        {
                            if (resource is Animation animation)
                            {
                                animation.Write(bw);
                            }
                            else
                            {
                                throw new NotSupportedException($"Specified {nameof(IsAnimation)} {IsAnimation} on {nameof(ResourceType)} {Type} but {nameof(Resources)}[{i}] was not a {nameof(Animation)}.");
                            }
                        }
                        break;
                    case ResourceType.Texture:
                        if (resource is Texture texture)
                        {
                            texture.Write(bw);
                        }
                        else
                        {
                            throw new NotSupportedException($"Specified {nameof(ResourceType)} {Type} but {nameof(Resources)}[{i}] was not a {nameof(Texture)}.");
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
            /// The MLB references models or animations.
            /// </summary>
            Model = 3,

            /// <summary>
            /// The MLB references textures.
            /// </summary>
            Texture = 4
        }

        #endregion

        #region Bone Order

        /// <summary>
        /// A struct for holding bone ordering information during reading.
        /// </summary>
        private struct BoneOrderIndex
        {
            /// <summary>
            /// The index of the bone.
            /// </summary>
            public int Index;

            /// <summary>
            /// The offset of the bone.
            /// </summary>
            public int Offset;
        }

        #endregion

        #region Model

        /// <summary>
        /// A model resource.
        /// </summary>
        public class Model : IMlbResource
        {
            /// <inheritdoc/>
            public string Path { get; set; }

            /// <inheritdoc/>
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
            /// Indices indicating the order of the bones.
            /// </summary>
            public List<int> OrderIndices { get; set; }

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
                OrderIndices = new List<int>();
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

                var orderList = new List<BoneOrderIndex>(boneCount);
                Bones = new List<Bone>(boneCount);
                OrderIndices = new List<int>(boneCount);
                if (boneOffset > 0 && boneCount > 0)
                {
                    int[] boneOffsets = br.GetInt32s(boneOffset, boneCount);
                    for (int i = 0; i < boneOffsets.Length; i++)
                    {
                        int offset = boneOffsets[i];

                        // o8916 and o9995 have offsets of 0 on some entries
                        if (offset > 0)
                        {
                            br.Position = offset;
                            Bones.Add(new Bone(br));
                        }
                        else
                        {
                            var dummy = new Bone();
                            dummy.IsDummy = true;
                            Bones.Add(dummy);
                        }

                        var boneOrderIndex = new BoneOrderIndex
                        {
                            Offset = offset,
                            Index = i
                        };

                        orderList.Add(boneOrderIndex);
                    }
                }

                // Offsets are ordered by skeleton, data is ordered by sibling
                var sortedOrderList = orderList.OrderBy(m => m.Offset);
                foreach (var order in sortedOrderList)
                {
                    OrderIndices.Add(order.Index);
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
                    // Offsets are ordered by skeleton, data is ordered by sibling
                    // Ensure order indices are usable
                    var orderCheck = new HashSet<int>();
                    bool allDifferent = OrderIndices.All(orderCheck.Add);
                    if (!allDifferent)
                    {
                        throw new InvalidOperationException($"{nameof(OrderIndices)} is invalid.");
                    }

                    for (int i = 0; i < Bones.Count; i++)
                    {
                        int index = OrderIndices[i];
                        var bone = Bones[index];
                        if (bone.IsDummy)
                        {
                            // o8916 and o9995 have offsets of 0 on some entries
                            bw.FillInt32($"BoneOffset_{index}", 0);
                        }
                        else
                        {
                            bw.FillInt32($"BoneOffset_{index}", (int)bw.Position);
                            bone.Write(bw);
                        }
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
                        br.AssertPattern(28, 0);
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
                        bw.WritePattern(28, 0);
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
                /// Unknown; Only seen in e3021_m.mlb in AC4.
                /// </summary>
                public UnkConfig1 Config1 { get; set; }

                /// <summary>
                /// Unknown; Only present on normal and break model bones.
                /// </summary>
                public BoneBreakConfig BreakConfig { get; set; }

                /// <summary>
                /// Unknown; Only present on collision model bones.
                /// </summary>
                public BoneCollisionConfig CollisionConfig { get; set; }

                /// <summary>
                /// Whether or not <see cref="Config1"/> is present.
                /// </summary>
                public bool HasConfig1 { get; set; }

                /// <summary>
                /// Whether or not <see cref="BreakConfig"/> is present.
                /// </summary>
                public bool HasBreakConfig { get; set; }

                /// <summary>
                /// Whether or not <see cref="CollisionConfig"/> is present.
                /// </summary>
                public bool HasCollisionConfig { get; set; }

                /// <summary>
                /// Whether or not this is an empty entry.
                /// </summary>
                public bool IsDummy { get; set; }

                /// <summary>
                /// Create a new <see cref="Bone"/>.
                /// </summary>
                public Bone()
                {
                    Name = "bone";
                    Config1 = new UnkConfig1();
                    BreakConfig = new BoneBreakConfig();
                    CollisionConfig = new BoneCollisionConfig();
                    HasConfig1 = false;
                    HasBreakConfig = false;
                    HasCollisionConfig = false;
                    IsDummy = false;
                }

                /// <summary>
                /// Reads from a stream.
                /// </summary>
                /// <param name="br">The stream reader.</param>
                internal Bone(BinaryReaderEx br)
                {
                    IsDummy = false;
                    long start = br.Position;

                    int nameOffset = br.ReadInt32();
                    int offsetConfig1 = br.ReadInt32();
                    int offsetModel = br.ReadInt32();
                    int offsetCollision = br.ReadInt32();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);

                    Name = br.GetShiftJIS(start + nameOffset);
                    if (offsetConfig1 > 0)
                    {
                        br.Position = start + offsetConfig1;
                        Config1 = new UnkConfig1(br);
                        HasConfig1 = true;
                    }
                    else
                    {
                        Config1 = new UnkConfig1();
                        HasConfig1 = false;
                    }

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
                    bw.ReserveInt32("OffsetConfig1");
                    bw.ReserveInt32("OffsetBoneBreak");
                    bw.ReserveInt32("OffsetBoneCollision");
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);

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
                /// Unknown; Only seen in e3021_m.mlb in AC4.
                /// </summary>
                public class UnkConfig1
                {
                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public byte Unk00 { get; set; }

                    /// <summary>
                    /// Create a new <see cref="UnkConfig1"/>.
                    /// </summary>
                    public UnkConfig1()
                    {
                        Unk00 = 0;
                    }

                    /// <summary>
                    /// Reads from a stream.
                    /// </summary>
                    /// <param name="br">The stream reader.</param>
                    internal UnkConfig1(BinaryReaderEx br)
                    {
                        Unk00 = br.ReadByte();
                        br.AssertPattern(31, 0);
                    }

                    /// <summary>
                    /// Writes to a stream.
                    /// </summary>
                    /// <param name="bw">The stream writer.</param>
                    internal void Write(BinaryWriterEx bw)
                    {
                        bw.WriteByte(Unk00);
                        bw.WritePattern(31, 0);
                    }
                }

                /// <summary>
                /// Unknown; Only present on normal and break model bones.
                /// </summary>
                public class BoneBreakConfig
                {
                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public byte Unk00 { get; set; }

                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public byte Unk01 { get; set; }

                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public byte Unk02 { get; set; }

                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public BreakConfig1 Config1 { get; set; }

                    /// <summary>
                    /// References a sound event from a MOSB.
                    /// </summary>
                    public BreakSoundEvent SoundEvent { get; set; }

                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public BreakConfig3 Config3 { get; set; }

                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public BreakConfig4 Config4 { get; set; }

                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public BreakConfig5 Config5 { get; set; }

                    /// <summary>
                    /// Unknown.
                    /// </summary>
                    public BreakConfig6 Config6 { get; set; }

                    /// <summary>
                    /// Whether or not <see cref="Config1"/> is present.
                    /// </summary>
                    public bool HasConfig1 { get; set; }

                    /// <summary>
                    /// Whether or not <see cref="SoundEvent"/> is present.
                    /// </summary>
                    public bool HasSoundEvent { get; set; }

                    /// <summary>
                    /// Whether or not <see cref="Config3"/> is present.
                    /// </summary>
                    public bool HasConfig3 { get; set; }

                    /// <summary>
                    /// Whether or not <see cref="Config4"/> is present.
                    /// </summary>
                    public bool HasConfig4 { get; set; }

                    /// <summary>
                    /// Whether or not <see cref="Config5"/> is present.
                    /// </summary>
                    public bool HasConfig5 { get; set; }

                    /// <summary>
                    /// Whether or not <see cref="Config6"/> is present.
                    /// </summary>
                    public bool HasConfig6 { get; set; }

                    /// <summary>
                    /// Create a new <see cref="BoneBreakConfig"/>.
                    /// </summary>
                    public BoneBreakConfig()
                    {
                        Unk00 = 0;
                        Unk01 = 0;
                        Config1 = new BreakConfig1();
                        SoundEvent = new BreakSoundEvent();
                        Config3 = new BreakConfig3();
                        Config4 = new BreakConfig4();
                        Config5 = new BreakConfig5();
                        Config6 = new BreakConfig6();
                        HasConfig1 = false;
                        HasSoundEvent = false;
                        HasConfig3 = false;
                        HasConfig4 = false;
                        HasConfig5 = false;
                        HasConfig6 = false;
                    }

                    /// <summary>
                    /// Reads from a stream.
                    /// </summary>
                    /// <param name="br">The stream reader.</param>
                    internal BoneBreakConfig(BinaryReaderEx br)
                    {
                        long start = br.Position;

                        Unk00 = br.ReadByte();
                        Unk01 = br.ReadByte();
                        Unk02 = br.ReadByte();
                        br.AssertByte(0);
                        int offsetConfig1 = br.ReadInt32();
                        int offsetSoundEventConfig = br.ReadInt32();
                        int offsetConfig3 = br.ReadInt32();
                        int offsetConfig4 = br.ReadInt32();
                        int offsetConfig5 = br.ReadInt32();
                        int offsetConfig6 = br.ReadInt32();
                        br.AssertInt32(0);

                        if (offsetConfig1 > 0)
                        {
                            br.Position = start + offsetConfig1;
                            Config1 = new BreakConfig1(br);
                            HasConfig1 = true;
                        }
                        else
                        {
                            Config1 = new BreakConfig1();
                            HasConfig1 = false;
                        }

                        if (offsetSoundEventConfig > 0)
                        {
                            br.Position = start + offsetSoundEventConfig;
                            SoundEvent = new BreakSoundEvent(br);
                            HasSoundEvent = true;
                        }
                        else
                        {
                            SoundEvent = new BreakSoundEvent();
                            HasSoundEvent = false;
                        }

                        if (offsetConfig3 > 0)
                        {
                            br.Position = start + offsetConfig3;
                            Config3 = new BreakConfig3(br);
                            HasConfig3 = true;
                        }
                        else
                        {
                            Config3 = new BreakConfig3();
                            HasConfig3 = false;
                        }

                        if (offsetConfig4 > 0)
                        {
                            br.Position = start + offsetConfig4;
                            Config4 = new BreakConfig4(br);
                            HasConfig4 = true;
                        }
                        else
                        {
                            Config4 = new BreakConfig4();
                            HasConfig4 = false;
                        }

                        if (offsetConfig5 > 0)
                        {
                            br.Position = start + offsetConfig5;
                            Config5 = new BreakConfig5(br);
                            HasConfig5 = true;
                        }
                        else
                        {
                            Config5 = new BreakConfig5();
                            HasConfig5 = false;
                        }

                        if (offsetConfig6 > 0)
                        {
                            br.Position = start + offsetConfig6;
                            Config6 = new BreakConfig6(br);
                            HasConfig6 = true;
                        }
                        else
                        {
                            Config6 = new BreakConfig6();
                            HasConfig6 = false;
                        }
                    }

                    /// <summary>
                    /// Writes to a stream.
                    /// </summary>
                    /// <param name="bw">The stream writer.</param>
                    internal void Write(BinaryWriterEx bw)
                    {
                        long start = bw.Position;

                        bw.WriteByte(Unk00);
                        bw.WriteByte(Unk01);
                        bw.WriteByte(Unk02);
                        bw.WriteByte(0);
                        bw.ReserveInt32("OffsetConfig1");
                        bw.ReserveInt32("OffsetSoundEventConfig");
                        bw.ReserveInt32("OffsetConfig3");
                        bw.ReserveInt32("OffsetConfig4");
                        bw.ReserveInt32("OffsetConfig5");
                        bw.ReserveInt32("OffsetConfig6");
                        bw.WriteInt32(0);

                        if (HasConfig1)
                        {
                            bw.FillInt32("OffsetConfig1", (int)(bw.Position - start));
                            Config1.Write(bw);
                        }
                        else
                        {
                            bw.FillInt32("OffsetConfig1", 0);
                        }

                        if (HasSoundEvent)
                        {
                            bw.FillInt32("OffsetSoundEventConfig", (int)(bw.Position - start));
                            SoundEvent.Write(bw);
                        }
                        else
                        {
                            bw.FillInt32("OffsetConfig2", 0);
                        }

                        if (HasConfig3)
                        {
                            bw.FillInt32("OffsetConfig3", (int)(bw.Position - start));
                            Config3.Write(bw);
                        }
                        else
                        {
                            bw.FillInt32("OffsetConfig3", 0);
                        }

                        if (HasConfig4)
                        {
                            bw.FillInt32("OffsetConfig4", (int)(bw.Position - start));
                            Config4.Write(bw);
                        }
                        else
                        {
                            bw.FillInt32("OffsetConfig4", 0);
                        }

                        if (HasConfig5)
                        {
                            bw.FillInt32("OffsetConfig5", (int)(bw.Position - start));
                            Config5.Write(bw);
                        }
                        else
                        {
                            bw.FillInt32("OffsetConfig5", 0);
                        }

                        if (HasConfig6)
                        {
                            bw.FillInt32("OffsetConfig6", (int)(bw.Position - start));
                            Config6.Write(bw);
                        }
                        else
                        {
                            bw.FillInt32("OffsetConfig6", 0);
                        }
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

            #region Sub Configs

            /// <summary>
            /// Unknown.
            /// </summary>
            public class BreakConfig1
            {
                /// <summary>
                /// Unknown.
                /// </summary>
                public short Unk00 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short Unk02 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short Unk04 { get; set; }

                /// <summary>
                /// Create a new <see cref="BreakConfig1"/>.
                /// </summary>
                public BreakConfig1()
                {
                    Unk00 = 0;
                    Unk02 = 0;
                    Unk04 = 0;
                }

                /// <summary>
                /// Reads from a stream.
                /// </summary>
                /// <param name="br">The stream reader.</param>
                internal BreakConfig1(BinaryReaderEx br)
                {
                    Unk00 = br.ReadInt16();
                    Unk02 = br.ReadInt16();
                    Unk04 = br.ReadInt16();
                    br.AssertInt32(26, 0);
                }

                /// <summary>
                /// Writes to a stream.
                /// </summary>
                /// <param name="bw">The stream writer.</param>
                internal void Write(BinaryWriterEx bw)
                {
                    bw.WriteInt16(Unk00);
                    bw.WriteInt16(Unk02);
                    bw.WriteInt16(Unk04);
                    bw.WritePattern(26, 0);
                }
            }

            /// <summary>
            /// References a sound event from a MOSB.
            /// </summary>
            public class BreakSoundEvent
            {
                /// <summary>
                /// The name or ID of the sound event.
                /// </summary>
                public string SoundEventName { get; set; }

                /// <summary>
                /// Whether or not <see cref="SoundEventName"/> is present.
                /// </summary>
                public bool HasSoundEventName { get; set; }

                /// <summary>
                /// Create a new <see cref="BreakSoundEvent"/>.
                /// </summary>
                public BreakSoundEvent()
                {
                    SoundEventName = string.Empty;
                    HasSoundEventName = true;
                }

                /// <summary>
                /// Reads from a stream.
                /// </summary>
                /// <param name="br">The stream reader.</param>
                internal BreakSoundEvent(BinaryReaderEx br)
                {
                    long start = br.Position;

                    int offsetSoundEventName = br.ReadInt32();
                    br.AssertPattern(28, 0);

                    if (offsetSoundEventName > 0)
                    {
                        br.Position = start + offsetSoundEventName;
                        SoundEventName = br.ReadShiftJIS();
                        HasSoundEventName = true;
                    }
                    else
                    {
                        SoundEventName = string.Empty;
                        HasSoundEventName = false;
                    }
                }

                /// <summary>
                /// Writes to a stream.
                /// </summary>
                /// <param name="bw">The stream writer.</param>
                internal void Write(BinaryWriterEx bw)
                {
                    long start = bw.Position;

                    bw.ReserveInt32("OffsetSoundEventName");
                    bw.WritePattern(28, 0);

                    if (HasSoundEventName)
                    {
                        bw.FillInt32("OffsetSoundEventName", (int)(bw.Position - start));
                        bw.WriteShiftJIS(SoundEventName, true);
                        bw.Pad(4);
                    }
                    else
                    {
                        bw.FillInt32("OffsetSoundEventName", 0);
                    }
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class BreakConfig3
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
                public byte Unk18 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte Unk19 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short Unk1A { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short Unk1C { get; set; }

                /// <summary>
                /// Create a new <see cref="BreakConfig3"/>.
                /// </summary>
                public BreakConfig3()
                {
                    Unk00 = 0f;
                    Unk04 = 0f;
                    Unk08 = 0f;
                    Unk0C = 0f;
                    Unk10 = 0f;
                    Unk14 = 0f;
                    Unk18 = 0;
                    Unk19 = 0;
                    Unk1A = 0;
                    Unk1C = 0;
                }

                /// <summary>
                /// Reads from a stream.
                /// </summary>
                /// <param name="br">The stream reader.</param>
                internal BreakConfig3(BinaryReaderEx br)
                {
                    Unk00 = br.ReadSingle();
                    Unk04 = br.ReadSingle();
                    Unk08 = br.ReadSingle();
                    Unk0C = br.ReadSingle();
                    Unk10 = br.ReadSingle();
                    Unk14 = br.ReadSingle();
                    Unk18 = br.ReadByte();
                    Unk19 = br.ReadByte();
                    Unk1A = br.ReadInt16();
                    Unk1C = br.ReadInt16();
                    br.AssertPattern(34, 0);
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
                    bw.WriteByte(Unk18);
                    bw.WriteByte(Unk19);
                    bw.WriteInt16(Unk1A);
                    bw.WriteInt16(Unk1C);
                    bw.WritePattern(34, 0);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class BreakConfig4
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
                public byte Unk10 { get; set; }

                /// <summary>
                /// Create a new <see cref="BreakConfig4"/>.
                /// </summary>
                public BreakConfig4()
                {
                    Unk00 = 0f;
                    Unk04 = 0f;
                    Unk08 = 0f;
                    Unk0C = 0f;
                    Unk10 = 0;
                }

                /// <summary>
                /// Reads from a stream.
                /// </summary>
                /// <param name="br">The stream reader.</param>
                internal BreakConfig4(BinaryReaderEx br)
                {
                    Unk00 = br.ReadSingle();
                    Unk04 = br.ReadSingle();
                    Unk08 = br.ReadSingle();
                    Unk0C = br.ReadSingle();
                    Unk10 = br.ReadByte();
                    br.AssertPattern(47, 0);
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
                    bw.WriteByte(Unk10);
                    bw.WritePattern(47, 0);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class BreakConfig5
            {
                /// <summary>
                /// Unknown.
                /// </summary>
                public short Unk00 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short Unk02 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public short Unk04 { get; set; }

                /// <summary>
                /// Create a new <see cref="BreakConfig5"/>.
                /// </summary>
                public BreakConfig5()
                {
                    Unk00 = 0;
                    Unk02 = 0;
                    Unk04 = 0;
                }

                /// <summary>
                /// Reads from a stream.
                /// </summary>
                /// <param name="br">The stream reader.</param>
                internal BreakConfig5(BinaryReaderEx br)
                {
                    Unk00 = br.ReadInt16();
                    Unk02 = br.ReadInt16();
                    Unk04 = br.ReadInt16();
                    br.AssertPattern(10, 0);
                }

                /// <summary>
                /// Writes to a stream.
                /// </summary>
                /// <param name="bw">The stream writer.</param>
                internal void Write(BinaryWriterEx bw)
                {
                    bw.WriteInt16(Unk00);
                    bw.WriteInt16(Unk02);
                    bw.WriteInt16(Unk04);
                    bw.WritePattern(10, 0);
                }
            }

            /// <summary>
            /// Unknown.
            /// </summary>
            public class BreakConfig6
            {
                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk00 { get; set; }

                /// <summary>
                /// Create a new <see cref="BreakConfig6"/>.
                /// </summary>
                public BreakConfig6()
                {
                    Unk00 = 0;
                }

                /// <summary>
                /// Reads from a stream.
                /// </summary>
                /// <param name="br">The stream reader.</param>
                internal BreakConfig6(BinaryReaderEx br)
                {
                    Unk00 = br.ReadInt32();
                    br.AssertPattern(28, 0);
                }

                /// <summary>
                /// Writes to a stream.
                /// </summary>
                /// <param name="bw">The stream writer.</param>
                internal void Write(BinaryWriterEx bw)
                {
                    bw.WriteInt32(Unk00);
                    bw.WritePattern(28, 0);
                }
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
                public byte Unk00 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte Unk01 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte Unk02 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public BreakConfig1 Config1 { get; set; }

                /// <summary>
                /// References a sound event from a MOSB.
                /// </summary>
                public BreakSoundEvent SoundEvent { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public BreakConfig3 Config3 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public BreakConfig4 Config4 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public BreakConfig6 Config6 { get; set; }

                /// <summary>
                /// Whether or not <see cref="Config1"/> is present.
                /// </summary>
                public bool HasConfig1 { get; set; }

                /// <summary>
                /// Whether or not <see cref="SoundEvent"/> is present.
                /// </summary>
                public bool HasSoundEvent { get; set; }

                /// <summary>
                /// Whether or not <see cref="Config3"/> is present.
                /// </summary>
                public bool HasConfig3 { get; set; }

                /// <summary>
                /// Whether or not <see cref="Config4"/> is present.
                /// </summary>
                public bool HasConfig4 { get; set; }

                /// <summary>
                /// Whether or not <see cref="Config6"/> is present.
                /// </summary>
                public bool HasConfig6 { get; set; }

                /// <summary>
                /// Creates a new <see cref="ModelBreakConfig"/>.
                /// </summary>
                public ModelBreakConfig()
                {
                    Config1 = new BreakConfig1();
                    SoundEvent = new BreakSoundEvent();
                    Config3 = new BreakConfig3();
                    Config4 = new BreakConfig4();
                    Config6 = new BreakConfig6();
                    HasConfig1 = false;
                    HasSoundEvent = false;
                    HasConfig3 = false;
                    HasConfig4 = false;
                    HasConfig6 = false;
                }

                /// <summary>
                /// Reads from a stream.
                /// </summary>
                /// <param name="br">The stream reader.</param>
                internal ModelBreakConfig(BinaryReaderEx br)
                {
                    long start = br.Position;

                    Unk00 = br.ReadByte();
                    Unk01 = br.ReadByte();
                    Unk02 = br.ReadByte();
                    br.AssertByte(0);
                    int offsetConfig1 = br.ReadInt32();
                    int soundEventOffset = br.ReadInt32();
                    int offsetConfig3 = br.ReadInt32();
                    int offsetConfig4 = br.ReadInt32();
                    int offsetConfig6 = br.ReadInt32();
                    br.AssertInt32(0);
                    br.AssertInt32(0);

                    if (offsetConfig1 > 0)
                    {
                        br.Position = start + offsetConfig1;
                        Config1 = new BreakConfig1(br);
                        HasConfig1 = true;
                    }
                    else
                    {
                        Config1 = new BreakConfig1();
                        HasConfig1 = false;
                    }

                    if (soundEventOffset > 0)
                    {
                        br.Position = start + soundEventOffset;
                        SoundEvent = new BreakSoundEvent(br);
                        HasSoundEvent = true;
                    }
                    else
                    {
                        SoundEvent = new BreakSoundEvent();
                        HasSoundEvent = false;
                    }

                    if (offsetConfig3 > 0)
                    {
                        br.Position = start + offsetConfig3;
                        Config3 = new BreakConfig3(br);
                        HasConfig3 = true;
                    }
                    else
                    {
                        Config3 = new BreakConfig3();
                        HasConfig3 = false;
                    }

                    if (offsetConfig4 > 0)
                    {
                        br.Position = start + offsetConfig4;
                        Config4 = new BreakConfig4(br);
                        HasConfig4 = true;
                    }
                    else
                    {
                        Config4 = new BreakConfig4();
                        HasConfig4 = false;
                    }

                    if (offsetConfig6 > 0)
                    {
                        br.Position = start + offsetConfig6;
                        Config6 = new BreakConfig6(br);
                        HasConfig6 = true;
                    }
                    else
                    {
                        Config6 = new BreakConfig6();
                        HasConfig6 = false;
                    }
                }

                /// <summary>
                /// Writes to a stream.
                /// </summary>
                /// <param name="bw">The stream writer.</param>
                internal void Write(BinaryWriterEx bw)
                {
                    long start = bw.Position;

                    bw.WriteByte(Unk00);
                    bw.WriteByte(Unk01);
                    bw.WriteByte(Unk02);
                    bw.WriteByte(0);
                    bw.ReserveInt32("OffsetConfig1");
                    bw.ReserveInt32("SoundEventOffset");
                    bw.ReserveInt32("OffsetConfig3");
                    bw.ReserveInt32("OffsetConfig4");
                    bw.ReserveInt32("OffsetConfig6");
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);

                    if (HasConfig1)
                    {
                        bw.FillInt32("OffsetConfig1", (int)(bw.Position - start));
                        Config1.Write(bw);
                    }
                    else
                    {
                        bw.FillInt32("OffsetConfig1", 0);
                    }

                    if (HasSoundEvent)
                    {
                        bw.FillInt32("SoundEventOffset", (int)(bw.Position - start));
                        SoundEvent.Write(bw);
                    }
                    else
                    {
                        bw.FillInt32("SoundEventOffset", 0);
                    }

                    if (HasConfig3)
                    {
                        bw.FillInt32("OffsetConfig3", (int)(bw.Position - start));
                        Config3.Write(bw);
                    }
                    else
                    {
                        bw.FillInt32("OffsetConfig3", 0);
                    }

                    if (HasConfig4)
                    {
                        bw.FillInt32("OffsetConfig4", (int)(bw.Position - start));
                        Config4.Write(bw);
                    }
                    else
                    {
                        bw.FillInt32("OffsetConfig4", 0);
                    }

                    if (HasConfig6)
                    {
                        bw.FillInt32("OffsetConfig6", (int)(bw.Position - start));
                        Config6.Write(bw);
                    }
                    else
                    {
                        bw.FillInt32("OffsetConfig6", 0);
                    }
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

        #region Animation

        /// <summary>
        /// An animation resource.
        /// </summary>
        public class Animation : IMlbResource
        {
            /// <inheritdoc/>
            public string Path { get; set; }

            /// <inheritdoc/>
            public string RelativePath { get; set; }

            /// <summary>
            /// The bones referenced.
            /// </summary>
            public List<Bone> Bones { get; set; }

            /// <summary>
            /// Indices indicating the order of the bones.
            /// </summary>
            public List<int> OrderIndices { get; set; }

            /// <summary>
            /// Unknown; same as <see cref="Model.ModelLodConfig"/>?
            /// </summary>
            public AnimationConfig1 Config1 { get; set; }

            /// <summary>
            /// Creates a new <see cref="Animation"/>.
            /// </summary>
            public Animation()
            {
                Path = string.Empty;
                RelativePath = string.Empty;
                Bones = new List<Bone>();
                OrderIndices = new List<int>();
                Config1 = new AnimationConfig1();
            }

            /// <summary>
            /// Reads from a stream.
            /// </summary>
            /// <param name="br">The stream reader.</param>
            internal Animation(BinaryReaderEx br)
            {
                long start = br.Position;

                int pathOffset = br.ReadInt32();
                int relativePathOffset = br.ReadInt32();
                int boneOffset = br.ReadInt32();
                int boneCount = br.ReadInt32();
                int offsetConfig1 = br.ReadInt32();

                Path = br.GetShiftJIS(start + pathOffset);
                RelativePath = br.GetShiftJIS(start + relativePathOffset);

                var orderList = new List<BoneOrderIndex>(boneCount);
                Bones = new List<Bone>(boneCount);
                OrderIndices = new List<int>(boneCount);
                if (boneOffset > 0 && boneCount > 0)
                {
                    int[] boneOffsets = br.GetInt32s(boneOffset, boneCount);
                    for (int i = 0; i < boneCount; i++)
                    {
                        var offset = boneOffsets[i];
                        br.Position = offset;
                        Bones.Add(new Bone(br));

                        var boneOrderIndex = new BoneOrderIndex
                        {
                            Offset = offset,
                            Index = i
                        };

                        orderList.Add(boneOrderIndex);
                    }
                }

                // Offsets are ordered by skeleton, data is ordered by sibling
                var sortedOrderList = orderList.OrderBy(m => m.Offset);
                foreach (var order in sortedOrderList)
                {
                    OrderIndices.Add(order.Index);
                }

                br.Position = start + offsetConfig1;
                Config1 = new AnimationConfig1(br);
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
                bw.ReserveInt32("BoneOffset");
                bw.WriteInt32(Bones.Count);
                bw.ReserveInt32("OffsetConfig1");

                bw.FillInt32("PathOffset", (int)(bw.Position - start));
                bw.WriteShiftJIS(Path, true);

                bw.FillInt32("RelativePathOffset", (int)(bw.Position - start));
                bw.WriteShiftJIS(RelativePath, true);
                bw.Pad(4);

                bw.FillInt32("OffsetConfig1", (int)(bw.Position - start));
                Config1.Write(bw);

                bw.FillInt32("BoneOffset", (int)bw.Position);
                if (Bones.Count > 0)
                {
                    // Offsets are ordered by skeleton, data is ordered by sibling
                    // Ensure order indices are usable
                    var orderCheck = new HashSet<int>();
                    bool allDifferent = OrderIndices.All(orderCheck.Add);
                    if (!allDifferent)
                    {
                        throw new InvalidOperationException($"{nameof(OrderIndices)} is invalid.");
                    }

                    for (int i = 0; i < Bones.Count; i++)
                    {
                        bw.ReserveInt32($"BoneOffset_{i}");
                    }

                    for (int i = 0; i < Bones.Count; i++)
                    {
                        int index = OrderIndices[i];
                        bw.FillInt32($"BoneOffset_{index}", (int)bw.Position);
                        Bones[index].Write(bw);
                    }
                }
            }

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
                /// Unknown.
                /// </summary>
                public UnkConfig1 Config1 { get; set; }

                /// <summary>
                /// Create a new <see cref="Bone"/>.
                /// </summary>
                public Bone()
                {
                    Name = string.Empty;
                    Config1 = new UnkConfig1();
                }

                /// <summary>
                /// Reads from a stream.
                /// </summary>
                /// <param name="br">The stream reader.</param>
                internal Bone(BinaryReaderEx br)
                {
                    long start = br.Position;

                    int nameOffset = br.ReadInt32();
                    br.AssertBoolean(true); // IsAniBone?
                    br.AssertByte(0);
                    br.AssertByte(0);
                    br.AssertByte(0);
                    int offsetConfig1 = br.ReadInt32();
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);
                    br.AssertInt32(0);

                    Name = br.GetShiftJIS(start + nameOffset);

                    br.Position = start + offsetConfig1;
                    Config1 = new UnkConfig1(br);
                }

                /// <summary>
                /// Writes to a stream.
                /// </summary>
                /// <param name="bw">The stream writer.</param>
                internal void Write(BinaryWriterEx bw)
                {
                    long start = bw.Position;

                    bw.ReserveInt32("NameOffset");
                    bw.WriteBoolean(true);
                    bw.WriteByte(0);
                    bw.WriteByte(0);
                    bw.WriteByte(0);
                    bw.ReserveInt32("OffsetConfig1");
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);
                    bw.WriteInt32(0);

                    bw.FillInt32("NameOffset", (int)(bw.Position - start));
                    bw.WriteShiftJIS(Name, true);
                    bw.Pad(4);

                    bw.FillInt32("OffsetConfig1", (int)(bw.Position - start));
                    Config1.Write(bw);
                }

                #region Configs

                /// <summary>
                /// Unknown.
                /// </summary>
                public class UnkConfig1
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
                    /// Create a new <see cref="UnkConfig1"/>.
                    /// </summary>
                    public UnkConfig1()
                    {
                        Unk00 = 0f;
                        Unk04 = 0f;
                        Unk08 = 0f;
                        Unk0C = 0f;
                        Unk10 = 0f;
                        Unk14 = 0f;
                    }

                    /// <summary>
                    /// Reads from a stream.
                    /// </summary>
                    /// <param name="br">The stream reader.</param>
                    internal UnkConfig1(BinaryReaderEx br)
                    {
                        Unk00 = br.ReadSingle();
                        Unk04 = br.ReadSingle();
                        Unk08 = br.ReadSingle();
                        Unk0C = br.ReadSingle();
                        Unk10 = br.ReadSingle();
                        Unk14 = br.ReadSingle();
                        br.AssertInt32(0);
                        br.AssertInt32(0);
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
                        bw.WriteInt32(0);
                        bw.WriteInt32(0);
                    }
                }

                #endregion
            }

            #endregion

            #region Configs

            /// <summary>
            /// Unknown; same as <see cref="Model.ModelLodConfig"/>?
            /// </summary>
            public class AnimationConfig1
            {
                /// <summary>
                /// Create a new <see cref="AnimationConfig1"/>.
                /// </summary>
                public AnimationConfig1()
                {
                
                }

                /// <summary>
                /// Reads from a stream.
                /// </summary>
                /// <param name="br">The stream reader.</param>
                internal AnimationConfig1(BinaryReaderEx br)
                {
                    br.AssertPattern(32, 0);
                }

                /// <summary>
                /// Writes to a stream.
                /// </summary>
                /// <param name="bw">The stream writer.</param>
                internal void Write(BinaryWriterEx bw)
                {
                    bw.WritePattern(32, 0);
                }
            }

            #endregion
        }

        #endregion

        #region Texture

        /// <summary>
        /// A texture resource.
        /// </summary>
        public class Texture : IMlbResource
        {
            /// <inheritdoc/>
            public string Path { get; set; }

            /// <inheritdoc/>
            public string RelativePath { get; set; }

            /// <summary>
            /// An argument indicating it is a file, always "-file".
            /// </summary>
            public string FileArgument { get; set; }

            /// <summary>
            /// An argument indicating texture type such as "_n.", "-dxt5", "[24bit]" or "[32bit]"
            /// </summary>
            public string TypeArgument { get; set; }

            /// <summary>
            /// An argument indicating the number of mipmaps such as "-nmips 0"
            /// </summary>
            public string NumMipmapsArgument { get; set; }

            /// <summary>
            /// Create a new <see cref="Texture"/>.
            /// </summary>
            public Texture()
            {
                Path = string.Empty;
                RelativePath = string.Empty;
                FileArgument = "-file";
                TypeArgument = "[32bit]";
                NumMipmapsArgument = "-nmips 0";
            }

            /// <summary>
            /// Reads from a stream.
            /// </summary>
            /// <param name="br">The stream reader.</param>
            internal Texture(BinaryReaderEx br)
            {
                long start = br.Position;

                int pathOffset = br.ReadInt32();
                int relativePathOffset = br.ReadInt32();
                int fileArgOffset = br.ReadInt32();
                int typeArgOffset = br.ReadInt32();
                int numMipmapsArgOffset = br.ReadInt32();

                Path = br.GetShiftJIS(start + pathOffset);
                RelativePath = br.GetShiftJIS(start + relativePathOffset);
                FileArgument = br.GetASCII(start + fileArgOffset);
                TypeArgument = br.GetASCII(start + typeArgOffset);
                NumMipmapsArgument = br.GetASCII(start + numMipmapsArgOffset);
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
                bw.ReserveInt32("FileArgOffset");
                bw.ReserveInt32("TypeArgOffset");
                bw.ReserveInt32("NumMipmapsArgOffset");

                bw.FillInt32("PathOffset", (int)(bw.Position - start));
                bw.WriteShiftJIS(Path, true);

                bw.FillInt32("RelativePathOffset", (int)(bw.Position - start));
                bw.WriteShiftJIS(RelativePath, true);

                bw.FillInt32("FileArgOffset", (int)(bw.Position - start));
                bw.WriteASCII(FileArgument, true);

                bw.FillInt32("TypeArgOffset", (int)(bw.Position - start));
                bw.WriteASCII(TypeArgument, true);

                bw.FillInt32("NumMipmapsArgOffset", (int)(bw.Position - start));
                bw.WriteASCII(NumMipmapsArgument, true);
                bw.Pad(4);
            }
        }

        #endregion

        #region Dummy

        /// <summary>
        /// A resource type for empty entries in MLB, not a part of the MLB spec.
        /// </summary>
        public class Dummy : IMlbResource
        {
            /// <summary>
            /// Required for the interface, not supported.
            /// </summary>
            public string Path
            {
                get => throw new NotSupportedException("Dummies do not support paths.");
                set => throw new NotSupportedException("Dummies do not support paths.");
            }

            /// <summary>
            /// Required for the interface, not supported.
            /// </summary>
            public string RelativePath
            {
                get => throw new NotSupportedException("Dummies do not support paths.");
                set => throw new NotSupportedException("Dummies do not support paths.");
            }

            /// <summary>
            /// Create a new <see cref="Dummy"/>.
            /// </summary>
            public Dummy()
            {

            }
        }

        #endregion
    }
}
