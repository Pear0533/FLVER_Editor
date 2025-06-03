using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace SoulsFormats
{
    public partial class MSBAC4
    {
        /// <summary>
        /// The different types of regions.
        /// </summary>
        internal enum RegionType
        {
            /// <summary>
            /// Unknown; Associated with estimated distance?
            /// </summary>
            Distance = 0,

            /// <summary>
            /// A route start or goal point referenced by a route.
            /// </summary>
            RoutePoint = 1,

            /// <summary>
            /// Unknown; Seems to usually reference trigger or action bounds?
            /// </summary>
            Action = 50,

            /// <summary>
            /// A bounding area that, if left, causes the player to fail or die.
            /// </summary>
            OperationalArea = 100,

            /// <summary>
            /// A bounding area that will show a warning to the player if the player has left the normal area.
            /// </summary>
            WarningArea = 101,

            /// <summary>
            /// The normal bounding area the player plays in.
            /// </summary>
            AttentionArea = 102,

            /// <summary>
            /// An area that forces death.
            /// </summary>
            DeathField = 150,

            /// <summary>
            /// A spawn point.
            /// </summary>
            Spawn = 200,

            /// <summary>
            /// Unknown; A camera point?
            /// </summary>
            Camera = 300,

            /// <summary>
            /// An SFX region of some kind.
            /// </summary>
            SFX = 500,

            /// <summary>
            /// An area turrets cannot fire into.
            /// </summary>
            NoTurretArea = 700
        }

        /// <summary>
        /// A collection of points and trigger volumes used by scripts and events.
        /// </summary>
        public class PointParam : Param<Region>, IMsbParam<IMsbRegion>
        {
            /// <summary>
            /// Unknown.
            /// </summary>
            public List<Region.DistanceRegion> DistanceRegions { get; set; }

            /// <summary>
            /// Route start and end points referenced by routes.
            /// </summary>
            public List<Region.RoutePoint> RoutePoints { get; set; }

            /// <summary>
            /// Unknown; Seems to usually reference trigger or action bounds?
            /// </summary>
            public List<Region.Action> Actions { get; set; }

            /// <summary>
            /// Bounds the player must be in or they will fail.
            /// </summary>
            public List<Region.OperationalArea> OperationalAreas { get; set; }

            /// <summary>
            /// Bounds that show a warning if the player has left normal play areas.
            /// </summary>
            public List<Region.WarningArea> WarningAreas { get; set; }

            /// <summary>
            /// Bounds the player will be in for normal gameplay.
            /// </summary>
            public List<Region.AttentionArea> AttentionAreas { get; set; }

            /// <summary>
            /// Bounds that will kill the player if they enter them.
            /// </summary>
            public List<Region.DeathField> DeathFields { get; set; }

            /// <summary>
            /// Spawn points; Usually for the player.
            /// </summary>
            public List<Region.SpawnPoint> SpawnPoints { get; set; }

            /// <summary>
            /// Unknown; Camera points?
            /// </summary>
            public List<Region.CameraPoint> CameraPoints { get; set; }

            /// <summary>
            /// SFX points of some kind.
            /// </summary>
            public List<Region.SFXRegion> SFXRegions { get; set; }

            /// <summary>
            /// Areas that turrets cannot fire into.
            /// </summary>
            public List<Region.NoTurretAreaRegion> NoTurretAreas { get; set; }

            /// <summary>
            /// Creates a new, empty PointParam with default values.
            /// </summary>
            public PointParam() : base(10001002, "POINT_PARAM_ST")
            {
                DistanceRegions = new List<Region.DistanceRegion>();
                RoutePoints = new List<Region.RoutePoint>();
                Actions = new List<Region.Action>();
                OperationalAreas = new List<Region.OperationalArea>();
                WarningAreas = new List<Region.WarningArea>();
                AttentionAreas = new List<Region.AttentionArea>();
                DeathFields = new List<Region.DeathField>();
                SpawnPoints = new List<Region.SpawnPoint>();
                CameraPoints = new List<Region.CameraPoint>();
                SFXRegions = new List<Region.SFXRegion>();
                NoTurretAreas = new List<Region.NoTurretAreaRegion>();
            }

            /// <summary>
            /// Adds a region to the appropriate list for its type; returns the region.
            /// </summary>
            public Region Add(Region region)
            {
                switch (region)
                {
                    case Region.DistanceRegion r: DistanceRegions.Add(r); break;
                    case Region.RoutePoint r: RoutePoints.Add(r); break;
                    case Region.Action r: Actions.Add(r); break;
                    case Region.OperationalArea r: OperationalAreas.Add(r); break;
                    case Region.WarningArea r: WarningAreas.Add(r); break;
                    case Region.AttentionArea r: AttentionAreas.Add(r); break;
                    case Region.DeathField r: DeathFields.Add(r); break;
                    case Region.SpawnPoint r: SpawnPoints.Add(r); break;
                    case Region.CameraPoint r: CameraPoints.Add(r); break;
                    case Region.SFXRegion r: SFXRegions.Add(r); break;
                    case Region.NoTurretAreaRegion r: NoTurretAreas.Add(r); break;
                    default: throw new ArgumentException($"Unrecognized type {region.GetType()}.", nameof(region));
                }
                return region;
            }
            IMsbRegion IMsbParam<IMsbRegion>.Add(IMsbRegion region) => Add((Region)region);

            /// <summary>
            /// Returns every region in the order they'll be written.
            /// </summary>
            public override List<Region> GetEntries() => SFUtil.ConcatAll<Region>(DistanceRegions, RoutePoints, Actions, OperationalAreas, WarningAreas,
                                                                                  AttentionAreas, DeathFields, SpawnPoints, CameraPoints, SFXRegions, NoTurretAreas);
            IReadOnlyList<IMsbRegion> IMsbParam<IMsbRegion>.GetEntries() => GetEntries();

            internal override Region ReadEntry(BinaryReaderEx br, int version)
            {
                RegionType type = br.GetEnum32<RegionType>(br.Position + 8);
                switch (type)
                {
                    case RegionType.Distance:
                        return DistanceRegions.EchoAdd(new Region.DistanceRegion(br));
                    case RegionType.RoutePoint:
                        return RoutePoints.EchoAdd(new Region.RoutePoint(br));
                    case RegionType.Action:
                        return Actions.EchoAdd(new Region.Action(br));
                    case RegionType.OperationalArea:
                        return OperationalAreas.EchoAdd(new Region.OperationalArea(br));
                    case RegionType.WarningArea:
                        return WarningAreas.EchoAdd(new Region.WarningArea(br));
                    case RegionType.AttentionArea:
                        return AttentionAreas.EchoAdd(new Region.AttentionArea(br));
                    case RegionType.DeathField:
                        return DeathFields.EchoAdd(new Region.DeathField(br));
                    case RegionType.Spawn:
                        return SpawnPoints.EchoAdd(new Region.SpawnPoint(br));
                    case RegionType.Camera:
                        return CameraPoints.EchoAdd(new Region.CameraPoint(br));
                    case RegionType.SFX:
                        return SFXRegions.EchoAdd(new Region.SFXRegion(br));
                    case RegionType.NoTurretArea:
                        return NoTurretAreas.EchoAdd(new Region.NoTurretAreaRegion(br));
                    default:
                        throw new NotImplementedException($"Unimplemented region type: {type}");
                }
            }
        }

        /// <summary>
        /// A point or volume that triggers some sort of interaction.
        /// </summary>
        public abstract class Region : ParamEntry, IMsbRegion
        {
            /// <summary>
            /// The type of the region.
            /// </summary>
            private protected abstract RegionType Type { get; }

            /// <summary>
            /// Describes the space encompassed by the region.
            /// </summary>
            public MSB.Shape Shape
            {
                get => _shape;
                set
                {
                    if (value is MSB.Shape.Composite)
                        throw new ArgumentException("Armored Core For Answer does not support composite shapes.");
                    _shape = value;
                }
            }
            private MSB.Shape _shape;

            /// <summary>
            /// Location of the region.
            /// </summary>
            public Vector3 Position { get; set; }

            /// <summary>
            /// Rotation of the region, in degrees.
            /// </summary>
            public Vector3 Rotation { get; set; }

            /// <summary>
            /// Identifies this point uniquely by id regardless of type.
            /// <para>Used by routes.</para>
            /// </summary>
            public int UniqueID { get; set; }

            /// <summary>
            /// Identifies the region in external files such as scripts.
            /// <para>Set to -1 when unused.</para>
            /// </summary>
            public int PointID { get; set; }

            /// <summary>
            /// Names of the previous points.
            /// </summary>
            [MSBReference(ReferenceType = typeof(Region))]
            public List<string> PreviousPointNames { get; set; }
            private ushort[] PreviousPointIndices { get; set; }

            /// <summary>
            /// Names of the next points.
            /// </summary>
            [MSBReference(ReferenceType = typeof(Region))]
            public List<string> NextPointNames { get; set; }
            private ushort[] NextPointIndices { get; set; }

            /// <summary>
            /// Unknown; Layer config?
            /// </summary>
            public UnkConfig3 Config3 { get; set; }

            private protected Region(string name)
            {
                Name = name;
                Shape = new MSB.Shape.Point();
                UniqueID = 0;
                PointID = -1;
                PreviousPointNames = new List<string>();
                NextPointNames = new List<string>();
                Config3 = new UnkConfig3();
            }

            /// <summary>
            /// Creates a deep copy of the region.
            /// </summary>
            public Region DeepCopy()
            {
                var region = (Region)MemberwiseClone();
                region.Shape = Shape.DeepCopy();
                region.PreviousPointNames = new List<string>(PreviousPointNames);
                region.NextPointNames = new List<string>(NextPointNames);
                region.Config3 = Config3.DeepCopy();
                DeepCopyTo(region);
                return region;
            }
            IMsbRegion IMsbRegion.DeepCopy() => DeepCopy();

            private protected virtual void DeepCopyTo(Region region) { }

            private protected Region(BinaryReaderEx br)
            {
                long start = br.Position;

                int nameOffset = br.ReadInt32();
                UniqueID = br.ReadInt32();
                br.AssertInt32((int)Type);
                br.ReadInt32(); // ID
                MSB.ShapeType shapeType = br.ReadEnum32<MSB.ShapeType>();
                Position = br.ReadVector3();
                Rotation = br.ReadVector3();
                int offsetLinkPrevious = br.ReadInt32();
                int offsetLinkNext = br.ReadInt32();
                PointID = br.ReadInt32();
                int shapeDataOffset = br.ReadInt32();
                int offsetArea = br.ReadInt32();
                int offsetSpawn = br.ReadInt32();
                int offsetCamera = br.ReadInt32();
                br.AssertInt32(0); // Probably unknown and unused type offset
                int offsetSFX = br.ReadInt32();
                br.AssertInt32(0); // Probably unknown and unused type offset, offsetSlowdown?
                int offsetNoTurretArea = br.ReadInt32();
                br.AssertInt32(0); // Probably unknown and unused type offset
                int offsetUnkConfig3 = br.ReadInt32();
                br.AssertInt32(0); // Probably unknown and unused type offset, offsetAntiAirArea?
                br.AssertInt32(0); // Probably unknown and unused type offset, offsetAntiAirCamera?
                br.AssertInt32(0); // Probably unknown and unused data offset, offsetUnkConfig4?
                br.AssertInt32(0); // Probably unknown and unused type offset, offsetHerd?
                br.AssertInt32(0); // Probably unknown and unused type offset, offsetSkirtRoom?
                br.AssertInt32(0); // Probably unknown and unused type offset
                br.AssertInt32(0); // Probably unknown and unused type offset
                br.AssertInt32(0); // Probably unknown and unused type offset

                Shape = MSB.Shape.Create(shapeType);

                if (nameOffset == 0)
                    throw new InvalidDataException($"{nameof(nameOffset)} must not be 0 in type {GetType()}.");
                if (offsetLinkPrevious == 0)
                    throw new InvalidDataException($"{nameof(offsetLinkPrevious)} must not be 0 in type {GetType()}.");
                if (offsetLinkNext == 0)
                    throw new InvalidDataException($"{nameof(offsetLinkNext)} must not be 0 in type {GetType()}.");
                if (Shape.HasShapeData ^ shapeDataOffset != 0)
                    throw new InvalidDataException($"Unexpected {nameof(shapeDataOffset)} 0x{shapeDataOffset:X} in type {GetType()}.");
                if (offsetUnkConfig3 == 0)
                    throw new InvalidDataException($"{nameof(offsetUnkConfig3)} must not be 0 in type {GetType()}.");
                
                br.Position = start + nameOffset;
                Name = br.ReadShiftJIS();

                br.Position = start + offsetLinkPrevious;
                ushort previousCount = br.ReadUInt16();
                PreviousPointIndices = br.ReadUInt16s(previousCount);
                br.Pad(4);

                br.Position = start + offsetLinkNext;
                ushort nextCount = br.ReadUInt16();
                NextPointIndices = br.ReadUInt16s(nextCount);
                br.Pad(4);

                if (Shape.HasShapeData)
                {
                    br.Position = start + shapeDataOffset;
                    Shape.ReadShapeData(br);
                }

                if (offsetArea > 0)
                {
                    if (Type != RegionType.OperationalArea &&
                        Type != RegionType.WarningArea &&
                        Type != RegionType.AttentionArea)
                        throw new InvalidDataException($"{nameof(offsetArea)} must be 0 for type {GetType()}");

                    br.Position = start + offsetArea;
                    ReadTypeData(br);
                }

                if (offsetSpawn > 0)
                {
                    if (Type != RegionType.Spawn)
                        throw new InvalidDataException($"{nameof(offsetSpawn)} must be 0 for type {GetType()}");

                    br.Position = start + offsetSpawn;
                    ReadTypeData(br);
                }

                if (offsetCamera > 0)
                {
                    if (Type != RegionType.Camera)
                        throw new InvalidDataException($"{nameof(offsetCamera)} must be 0 for type {GetType()}");

                    br.Position = start + offsetCamera;
                    ReadTypeData(br);
                }

                if (offsetSFX > 0)
                {
                    if (Type != RegionType.SFX)
                        throw new InvalidDataException($"{nameof(offsetSFX)} must be 0 for type {GetType()}");

                    br.Position = start + offsetSFX;
                    ReadTypeData(br);
                }

                if (offsetNoTurretArea > 0)
                {
                    if (Type != RegionType.NoTurretArea)
                        throw new InvalidDataException($"{nameof(offsetNoTurretArea)} must be 0 for type {GetType()}");

                    br.Position = start + offsetNoTurretArea;
                    ReadTypeData(br);
                }

                br.Position = start + offsetUnkConfig3;
                Config3 = new UnkConfig3(br);
            }

            private protected virtual void ReadTypeData(BinaryReaderEx br)
                => throw new NotImplementedException($"Type {GetType()} missing valid {nameof(ReadTypeData)}.");

            internal override void Write(BinaryWriterEx bw, int version, int id)
            {
                long start = bw.Position;

                bw.ReserveInt32("NameOffset");
                bw.WriteInt32(UniqueID);
                bw.WriteInt32((int)Type);
                bw.WriteInt32(id);
                bw.WriteUInt32((uint)Shape.Type);
                bw.WriteVector3(Position);
                bw.WriteVector3(Rotation);
                bw.ReserveInt32("OffsetLinkPrevious");
                bw.ReserveInt32("OffsetLinkNext");
                bw.WriteInt32(PointID);
                bw.ReserveInt32("ShapeDataOffset");
                bw.ReserveInt32("OffsetArea");
                bw.ReserveInt32("OffsetSpawn");
                bw.ReserveInt32("OffsetCamera");
                bw.WriteInt32(0); // Probably unknown and unused type offset
                bw.ReserveInt32("OffsetSFX");
                bw.WriteInt32(0); // Probably unknown and unused data offset, OffsetSlowdown?
                bw.ReserveInt32("OffsetNoTurretArea");
                bw.WriteInt32(0); // Probably unknown and unused type offset
                bw.ReserveInt32("OffsetUnkConfig3");
                bw.WriteInt32(0); // Probably unknown and unused data offset, OffsetAntiAirArea?
                bw.WriteInt32(0); // Probably unknown and unused data offset, OffsetAntiAirCamera?
                bw.WriteInt32(0); // Probably unknown and unused data offset, OffsetUnkConfig4?
                bw.WriteInt32(0); // Probably unknown and unused type offset, OffsetHerd?
                bw.WriteInt32(0); // Probably unknown and unused type offset, OffsetSkirtRoom?
                bw.WriteInt32(0); // Probably unknown and unused type offset
                bw.WriteInt32(0); // Probably unknown and unused type offset
                bw.WriteInt32(0); // Probably unknown and unused type offset

                bw.FillInt32("NameOffset", (int)(bw.Position - start));
                bw.WriteShiftJIS(MSB.ReambiguateName(Name), true);
                bw.Pad(4);

                bw.FillInt32("OffsetLinkPrevious", (int)(bw.Position - start));
                bw.WriteUInt16((ushort)PreviousPointIndices.Length);
                bw.WriteUInt16s(PreviousPointIndices);
                bw.Pad(4);

                bw.FillInt32("OffsetLinkNext", (int)(bw.Position - start));
                bw.WriteUInt16((ushort)NextPointIndices.Length);
                bw.WriteUInt16s(NextPointIndices);
                bw.Pad(4);

                if (Shape.HasShapeData)
                {
                    bw.FillInt32("ShapeDataOffset", (int)(bw.Position - start));
                    Shape.WriteShapeData(bw);
                }
                else
                {
                    bw.FillInt32("ShapeDataOffset", 0);
                }

                FillTypeDataOffset(bw, start, "OffsetArea", Type == RegionType.OperationalArea || Type == RegionType.WarningArea || Type == RegionType.AttentionArea);
                FillTypeDataOffset(bw, start, "OffsetSpawn", Type == RegionType.Spawn);
                FillTypeDataOffset(bw, start, "OffsetCamera", Type == RegionType.Camera);
                // Probably unknown type
                FillTypeDataOffset(bw, start, "OffsetSFX", Type == RegionType.SFX);
                // Probably unknown type, OffsetSlowdown?
                FillTypeDataOffset(bw, start, "OffsetNoTurretArea", Type == RegionType.NoTurretArea);
                // Probably unknown type
                bw.FillInt32("OffsetUnkConfig3", (int)(bw.Position - start));
                Config3.Write(bw);

                // Probably unknown type, OffsetAntiAirArea?
                // Probably unknown type, OffsetAntiAirCamera?
                // Probably unknown data, OffsetUnkConfig4?
                // Probably unknown type, OffsetHerd?
                // Probably unknown type, OffsetSkirtRoom?
                // Probably unknown type
                // Probably unknown type
                // Probably unknown type
            }

            private void FillTypeDataOffset(BinaryWriterEx bw, long start, string name, bool hasType)
            {
                if (hasType)
                {
                    bw.FillInt32(name, (int)(bw.Position - start));
                    WriteTypeData(bw);
                }
                else
                {
                    bw.FillInt32(name, 0);
                }
            }

            private protected virtual void WriteTypeData(BinaryWriterEx bw)
                => throw new NotImplementedException($"Type {GetType()} missing valid {nameof(WriteTypeData)}.");

            internal virtual void GetNames(Entries entries)
            {
                PreviousPointNames = new List<string>(MSB.FindNames(entries.Regions, PreviousPointIndices));
                NextPointNames = new List<string>(MSB.FindNames(entries.Regions, NextPointIndices));
            }

            internal virtual void GetIndices(Entries entries)
            {
                int[] previousIndices = MSB.FindIndices(entries.Regions, PreviousPointNames);
                int[] nextIndices = MSB.FindIndices(entries.Regions, NextPointNames);
                PreviousPointIndices = new ushort[previousIndices.Length];
                NextPointIndices = new ushort[nextIndices.Length];
                GetIndices(PreviousPointNames, previousIndices, PreviousPointIndices);
                GetIndices(NextPointNames, nextIndices, NextPointIndices);
            }

            private static void GetIndices(List<string> names, int[] indices, ushort[] newIndices)
            {
                for (int i = 0; i < indices.Length; i++)
                {
                    if (indices[i] == -1)
                    {
                        throw new Exception($"Index could not be found: {names[i]}.");
                    }

                    if (indices[i] < 0 || indices[i] > ushort.MaxValue)
                    {
                        throw new IndexOutOfRangeException($"Index too small or too large: {indices[i]}");
                    }

                    newIndices[i] = (ushort)indices[i];
                }
            }

            #region Region Sub Structs

            /// <summary>
            /// Unknown; Layer config?
            /// </summary>
            public class UnkConfig3
            {
                /// <summary>
                /// Unknown.
                /// </summary>
                public sbyte Unk00 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public sbyte Unk01 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public sbyte Unk02 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public sbyte Unk03 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk04 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk08 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk0C { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk10 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk14 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk18 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk1C { get; set; }

                public UnkConfig3()
                {
                    Unk00 = -1;
                    Unk01 = -1;
                    Unk02 = 0;
                    Unk03 = -1;
                    Unk04 = 0;
                    Unk08 = 0;
                    Unk0C = 0;
                    Unk10 = 0;
                    Unk14 = 0;
                    Unk18 = 0;
                    Unk1C = 0;
                }

                /// <summary>
                /// Creates a deep copy of the struct.
                /// </summary>
                public UnkConfig3 DeepCopy()
                {
                    return (UnkConfig3)MemberwiseClone();
                }

                internal UnkConfig3(BinaryReaderEx br)
                {
                    Unk00 = br.ReadSByte();
                    Unk01 = br.ReadSByte();
                    Unk02 = br.ReadSByte();
                    Unk03 = br.ReadSByte();
                    Unk04 = br.ReadInt32();
                    Unk08 = br.ReadInt32();
                    Unk0C = br.ReadInt32();
                    Unk10 = br.ReadInt32();
                    Unk14 = br.ReadInt32();
                    Unk18 = br.ReadInt32();
                    Unk1C = br.ReadInt32();
                }

                internal void Write(BinaryWriterEx bw)
                {
                    bw.WriteSByte(Unk00);
                    bw.WriteSByte(Unk01);
                    bw.WriteSByte(Unk02);
                    bw.WriteSByte(Unk03);
                    bw.WriteInt32(Unk04);
                    bw.WriteInt32(Unk08);
                    bw.WriteInt32(Unk0C);
                    bw.WriteInt32(Unk10);
                    bw.WriteInt32(Unk14);
                    bw.WriteInt32(Unk18);
                    bw.WriteInt32(Unk1C);
                }
            }

            /// <summary>
            /// Type data for area points.
            /// </summary>
            public class AreaConfig
            {
                /// <summary>
                /// Unknown.
                /// </summary>
                public sbyte Unk00 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public sbyte Unk01 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public sbyte Unk02 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public sbyte Unk03 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte Unk04 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte Unk05 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte Unk06 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public byte Unk07 { get; set; }

                public AreaConfig()
                {
                    Unk00 = 0;
                    Unk01 = 0;
                    Unk02 = 0;
                    Unk03 = 0;
                    Unk04 = 0;
                    Unk05 = 0;
                    Unk06 = 0;
                    Unk07 = 0;
                }

                /// <summary>
                /// Creates a deep copy of the struct.
                /// </summary>
                public AreaConfig DeepCopy()
                {
                    return (AreaConfig)MemberwiseClone();
                }

                internal AreaConfig(BinaryReaderEx br)
                {
                    Unk00 = br.ReadSByte();
                    Unk01 = br.ReadSByte();
                    Unk02 = br.ReadSByte();
                    Unk03 = br.ReadSByte();
                    Unk04 = br.ReadByte();
                    Unk05 = br.ReadByte();
                    Unk06 = br.ReadByte();
                    Unk07 = br.ReadByte();
                    br.AssertPattern(24, 0);
                }

                internal void Write(BinaryWriterEx bw)
                {
                    bw.WriteSByte(Unk00);
                    bw.WriteSByte(Unk01);
                    bw.WriteSByte(Unk02);
                    bw.WriteSByte(Unk03);
                    bw.WriteByte(Unk04);
                    bw.WriteByte(Unk05);
                    bw.WriteByte(Unk06);
                    bw.WriteByte(Unk07);
                    bw.WritePattern(24, 0);
                }
            }

            /// <summary>
            /// Type data for spawn points.
            /// </summary>
            public class SpawnConfig
            {
                /// <summary>
                /// Unknown; Usually seems to be the index of the spawn point.
                /// </summary>
                public byte ID { get; set; }

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
                public byte Unk03 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk04 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk08 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk0C { get; set; }

                public SpawnConfig()
                {
                    ID = 0;
                    Unk01 = 0;
                    Unk02 = 0;
                    Unk03 = 0;
                    Unk04 = 0;
                    Unk08 = 0;
                    Unk0C = 0;
                }

                public SpawnConfig(byte id)
                {
                    ID = id;
                    Unk01 = 0;
                    Unk02 = 0;
                    Unk03 = 0;
                    Unk04 = 0;
                    Unk08 = 0;
                    Unk0C = 0;
                }

                /// <summary>
                /// Creates a deep copy of the struct.
                /// </summary>
                public SpawnConfig DeepCopy()
                {
                    return (SpawnConfig)MemberwiseClone();
                }

                internal SpawnConfig(BinaryReaderEx br)
                {
                    ID = br.ReadByte();
                    Unk01 = br.ReadByte();
                    Unk02 = br.ReadByte();
                    Unk03 = br.ReadByte();
                    Unk04 = br.ReadInt32();
                    Unk08 = br.ReadInt32();
                    Unk0C = br.ReadInt32();
                }

                internal void Write(BinaryWriterEx bw)
                {
                    bw.WriteByte(ID);
                    bw.WriteByte(Unk01);
                    bw.WriteByte(Unk02);
                    bw.WriteByte(Unk03);
                    bw.WriteInt32(Unk04);
                    bw.WriteInt32(Unk08);
                    bw.WriteInt32(Unk0C);
                }
            }

            /// <summary>
            /// Type data for camera points.
            /// </summary>
            public class CameraConfig
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
                public int Unk08 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk0C { get; set; }

                public CameraConfig()
                {
                    Unk00 = 0;
                    Unk04 = 0;
                    Unk08 = 0;
                    Unk0C = 0;
                }

                /// <summary>
                /// Creates a deep copy of the struct.
                /// </summary>
                public CameraConfig DeepCopy()
                {
                    return (CameraConfig)MemberwiseClone();
                }

                internal CameraConfig(BinaryReaderEx br)
                {
                    Unk00 = br.ReadInt32();
                    Unk04 = br.ReadInt32();
                    Unk08 = br.ReadInt32();
                    Unk0C = br.ReadInt32();
                }

                internal void Write(BinaryWriterEx bw)
                {
                    bw.WriteInt32(Unk00);
                    bw.WriteInt32(Unk04);
                    bw.WriteInt32(Unk08);
                    bw.WriteInt32(Unk0C);
                }
            }

            /// <summary>
            /// Type data for SFX points.
            /// </summary>
            public class SFXConfig
            {
                /// <summary>
                /// The ID of the SFX to use.
                /// </summary>
                public int SFXID { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk04 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk08 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk0C { get; set; }

                public SFXConfig()
                {
                    SFXID = -1;
                    Unk04 = 0;
                    Unk08 = 0;
                    Unk0C = 0;
                }

                public SFXConfig(int sfxID)
                {
                    SFXID = sfxID;
                    Unk04 = 0;
                    Unk08 = 0;
                    Unk0C = 0;
                }

                /// <summary>
                /// Creates a deep copy of the struct.
                /// </summary>
                public SFXConfig DeepCopy()
                {
                    return (SFXConfig)MemberwiseClone();
                }

                internal SFXConfig(BinaryReaderEx br)
                {
                    SFXID = br.ReadInt32();
                    Unk04 = br.ReadInt32();
                    Unk08 = br.ReadInt32();
                    Unk0C = br.ReadInt32();
                }

                internal void Write(BinaryWriterEx bw)
                {
                    bw.WriteInt32(SFXID);
                    bw.WriteInt32(Unk04);
                    bw.WriteInt32(Unk08);
                    bw.WriteInt32(Unk0C);
                }
            }

            /// <summary>
            /// Type data for no turret area points.
            /// </summary>
            public class NoTurretAreaConfig
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
                /// Unknown.
                /// </summary>
                public short Unk06 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk08 { get; set; }

                /// <summary>
                /// Unknown.
                /// </summary>
                public int Unk0C { get; set; }

                public NoTurretAreaConfig()
                {
                    Unk00 = 0;
                    Unk02 = 0;
                    Unk04 = 0;
                    Unk06 = 0;
                    Unk08 = 0;
                    Unk0C = 0;
                }

                /// <summary>
                /// Creates a deep copy of the struct.
                /// </summary>
                public NoTurretAreaConfig DeepCopy()
                {
                    return (NoTurretAreaConfig)MemberwiseClone();
                }

                internal NoTurretAreaConfig(BinaryReaderEx br)
                {
                    Unk00 = br.ReadInt16();
                    Unk02 = br.ReadInt16();
                    Unk04 = br.ReadInt16();
                    Unk06 = br.ReadInt16();
                    Unk08 = br.ReadInt32();
                    Unk0C = br.ReadInt32();
                }

                internal void Write(BinaryWriterEx bw)
                {
                    bw.WriteInt16(Unk00);
                    bw.WriteInt16(Unk02);
                    bw.WriteInt16(Unk04);
                    bw.WriteInt16(Unk06);
                    bw.WriteInt32(Unk08);
                    bw.WriteInt32(Unk0C);
                }
            }

            #endregion

            #region RegionType Structs

            /// <summary>
            /// Unknown; Seems to be a default type.
            /// </summary>
            public class DistanceRegion : Region
            {
                private protected override RegionType Type => RegionType.Distance;

                public DistanceRegion(string name) : base(name) { }
                public DistanceRegion() : this("default") { }

                private protected override void DeepCopyTo(Region region){}

                internal DistanceRegion(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// A route start or goal point referenced by a route.
            /// </summary>
            public class RoutePoint : Region
            {
                private protected override RegionType Type => RegionType.RoutePoint;

                public RoutePoint(string name) : base(name) { }
                public RoutePoint() : this("route") { }

                private protected override void DeepCopyTo(Region region) { }

                internal RoutePoint(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// Unknown; Seems to usually reference trigger or action bounds?
            /// </summary>
            public class Action : Region
            {
                private protected override RegionType Type => RegionType.Action;

                public Action(string name) : base(name) { }
                public Action() : this("action") { }

                private protected override void DeepCopyTo(Region region) { }

                internal Action(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// A bounding area that, if left, causes the player to fail or die.
            /// </summary>
            public class OperationalArea : Region
            {
                private protected override RegionType Type => RegionType.OperationalArea;

                public AreaConfig Area { get; set; }

                public OperationalArea(string name) : base(name)
                {
                    Area = new AreaConfig();
                }

                public OperationalArea() : this("operational area") { }

                private protected override void DeepCopyTo(Region region)
                {
                    var operationalArea = (OperationalArea)region;
                    operationalArea.Area = Area.DeepCopy();
                }

                internal OperationalArea(BinaryReaderEx br) : base(br) { }

                private protected override void ReadTypeData(BinaryReaderEx br) => Area = new AreaConfig(br);
                private protected override void WriteTypeData(BinaryWriterEx bw) => Area.Write(bw);
            }

            /// <summary>
            /// A bounding area that will show a warning to the player if the player has left the normal area.
            /// </summary>
            public class WarningArea : Region
            {
                private protected override RegionType Type => RegionType.WarningArea;

                public AreaConfig Area { get; set; }

                public WarningArea(string name) : base(name)
                {
                    Area = new AreaConfig();
                }

                public WarningArea() : this("warning area") { }

                private protected override void DeepCopyTo(Region region)
                {
                    var warningArea = (WarningArea)region;
                    warningArea.Area = Area.DeepCopy();
                }

                internal WarningArea(BinaryReaderEx br) : base(br) { }

                private protected override void ReadTypeData(BinaryReaderEx br) => Area = new AreaConfig(br);
                private protected override void WriteTypeData(BinaryWriterEx bw) => Area.Write(bw);
            }

            /// <summary>
            /// The normal bounding area the player plays in.
            /// </summary>
            public class AttentionArea : Region
            {
                private protected override RegionType Type => RegionType.AttentionArea;

                public AreaConfig Area { get; set; }

                public AttentionArea(string name) : base(name)
                {
                    Area = new AreaConfig();
                }

                public AttentionArea() : base("attention area") { }

                private protected override void DeepCopyTo(Region region)
                {
                    var attentionArea = (AttentionArea)region;
                    attentionArea.Area = Area.DeepCopy();
                }

                internal AttentionArea(BinaryReaderEx br) : base(br) { }

                private protected override void ReadTypeData(BinaryReaderEx br) => Area = new AreaConfig(br);
                private protected override void WriteTypeData(BinaryWriterEx bw) => Area.Write(bw);
            }

            /// <summary>
            /// An area that forces death.
            /// </summary>
            public class DeathField : Region
            {
                private protected override RegionType Type => RegionType.DeathField;

                public DeathField(string name) : base(name) { }
                public DeathField() : this("forced death field") { }

                private protected override void DeepCopyTo(Region region) { }

                internal DeathField(BinaryReaderEx br) : base(br) { }
            }

            /// <summary>
            /// A spawn point.
            /// </summary>
            public class SpawnPoint : Region
            {
                private protected override RegionType Type => RegionType.Spawn;

                public SpawnConfig Spawn { get; set; }

                public SpawnPoint(string name) : base(name)
                {
                    Spawn = new SpawnConfig();
                }

                public SpawnPoint() : base("spawn point") { }

                private protected override void DeepCopyTo(Region region)
                {
                    var spawn = (SpawnPoint)region;
                    spawn.Spawn = Spawn.DeepCopy();
                }

                internal SpawnPoint(BinaryReaderEx br) : base(br) { }

                private protected override void ReadTypeData(BinaryReaderEx br) => Spawn = new SpawnConfig(br);
                private protected override void WriteTypeData(BinaryWriterEx bw) => Spawn.Write(bw);
            }

            /// <summary>
            /// Unknown; A camera point?
            /// </summary>
            public class CameraPoint : Region
            {
                private protected override RegionType Type => RegionType.Camera;

                public CameraConfig Camera { get; set; }

                public CameraPoint(string name) : base(name)
                {
                    Camera = new CameraConfig();
                }

                public CameraPoint() : this("camera") { }

                private protected override void DeepCopyTo(Region region)
                {
                    var camera = (CameraPoint)region;
                    camera.Camera = Camera.DeepCopy();
                }

                internal CameraPoint(BinaryReaderEx br) : base(br) { }

                private protected override void ReadTypeData(BinaryReaderEx br) => Camera = new CameraConfig(br);
                private protected override void WriteTypeData(BinaryWriterEx bw) => Camera.Write(bw);
            }

            /// <summary>
            /// An SFX region of some kind.
            /// </summary>
            public class SFXRegion : Region
            {
                private protected override RegionType Type => RegionType.SFX;

                public SFXConfig SFX { get; set; }

                public SFXRegion(string name) : base(name)
                {
                    SFX = new SFXConfig();
                }

                public SFXRegion() : this("sfx") { }

                private protected override void DeepCopyTo(Region region)
                {
                    var sfx = (SFXRegion)region;
                    sfx.SFX = SFX.DeepCopy();
                }

                internal SFXRegion(BinaryReaderEx br) : base(br) { }

                private protected override void ReadTypeData(BinaryReaderEx br) => SFX = new SFXConfig(br);
                private protected override void WriteTypeData(BinaryWriterEx bw) => SFX.Write(bw);
            }

            /// <summary>
            /// An area turrets cannot fire into.
            /// </summary>
            public class NoTurretAreaRegion : Region
            {
                private protected override RegionType Type => RegionType.NoTurretArea;

                public NoTurretAreaConfig NoTurretArea { get; set; }

                public NoTurretAreaRegion(string name) : base(name)
                {
                    NoTurretArea = new NoTurretAreaConfig();
                }

                public NoTurretAreaRegion() : base("no turret area") { }

                private protected override void DeepCopyTo(Region region)
                {
                    var noTurretArea = (NoTurretAreaRegion)region;
                    noTurretArea.NoTurretArea = NoTurretArea.DeepCopy();
                }

                internal NoTurretAreaRegion(BinaryReaderEx br) : base(br) { }

                private protected override void ReadTypeData(BinaryReaderEx br) => NoTurretArea = new NoTurretAreaConfig(br);
                private protected override void WriteTypeData(BinaryWriterEx bw) => NoTurretArea.Write(bw);
            }

            #endregion
        }
    }
}
