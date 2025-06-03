using System.Collections.Generic;
using System.IO;

namespace SoulsFormats
{
    /// <summary>
    /// A part configuration format used in the 4th generation Armored Core.
    /// </summary>
    public partial class AcParts4 : SoulsFile<AcParts4>
    {
        /// <summary>
        /// Version to separate reading and writing AcParts in AC4 and ACFA.
        /// Values unique to each version will be defaulted.
        /// </summary>
        public enum AcParts4Version
        {
            /// <summary>
            /// Armored Core 4, has less stats than Armored Core For Answer.
            /// </summary>
            AC4,

            /// <summary>
            /// Armored Core For Answer, has more stats than Armored Core 4.
            /// </summary>
            ACFA
        }

        /// <summary>
        /// A version identifier used to separate AC4 and ACFA, not in the files.
        /// Values unique to each version will be defaulted.
        /// </summary>
        public AcParts4Version Version { get; set; }

        /// <summary>
        /// Heads in this ACPARTS file.
        /// </summary>
        public List<Head> Heads { get; set; }

        /// <summary>
        /// Cores in this ACPARTS file.
        /// </summary>
        public List<Core> Cores { get; set; }

        /// <summary>
        /// Arms in this ACPARTS file.
        /// </summary>
        public List<Arm> Arms { get; set; }

        /// <summary>
        /// Legs in this ACPARTS file.
        /// </summary>
        public List<Leg> Legs { get; set; }

        /// <summary>
        /// FCSs in this ACPARTS file.
        /// </summary>
        public List<FCS> FCSs { get; set; }

        /// <summary>
        /// Generators in this ACPARTS file.
        /// </summary>
        public List<Generator> Generators { get; set; }

        /// <summary>
        /// Main Boosters in this ACPARTS file.
        /// </summary>
        public List<MainBooster> MainBoosters { get; set; }

        /// <summary>
        /// Back Boosters in this ACPARTS file.
        /// </summary>
        public List<BackBooster> BackBoosters { get; set; }

        /// <summary>
        /// Side Boosters in this ACPARTS file.
        /// </summary>
        public List<SideBooster> SideBoosters { get; set; }

        /// <summary>
        /// Overed Boosters in this ACPARTS file.
        /// </summary>
        public List<OveredBooster> OveredBoosters { get; set; }

        /// <summary>
        /// Arm Units in this ACPARTS file.
        /// </summary>
        public List<ArmUnit> ArmUnits { get; set; }

        /// <summary>
        /// Back Units in this ACPARTS file.
        /// </summary>
        public List<BackUnit> BackUnits { get; set; }

        /// <summary>
        /// Shoulder Units in this ACPARTS file.
        /// </summary>
        public List<ShoulderUnit> ShoulderUnits { get; set; }

        /// <summary>
        /// Stabilizers on top of Head parts in this ACPARTS file.
        /// </summary>
        public List<HeadTopStabilizer> HeadTopStabilizers { get; set; }

        /// <summary>
        /// Stabilizers on the sides of Head parts in this ACPARTS file.
        /// </summary>
        public List<HeadSideStabilizer> HeadSideStabilizers { get; set; }

        /// <summary>
        /// Stabilizers on the upper sides of Core parts in this ACPARTS file.
        /// </summary>
        public List<CoreUpperSideStabilizer> CoreUpperSideStabilizers { get; set; }

        /// <summary>
        /// Stabilizers on the lower sides of Core parts in this ACPARTS file.
        /// </summary>
        public List<CoreLowerSideStabilizer> CoreLowerSideStabilizers { get; set; }

        /// <summary>
        /// Stabilizers on Arm parts in this ACPARTS file.
        /// </summary>
        public List<ArmStabilizer> ArmStabilizers { get; set; }

        /// <summary>
        /// Stabilizers on the back of Leg parts in this ACPARTS file.
        /// </summary>
        public List<LegBackStabilizer> LegBackStabilizers { get; set; }

        /// <summary>
        /// Stabilizers on the upper end of Leg parts in this ACPARTS file.
        /// </summary>
        public List<LegUpperStabilizer> LegUpperStabilizers { get; set; }

        /// <summary>
        /// Stabilizers in the middle of Leg parts in this ACPARTS file.
        /// </summary>
        public List<LegMiddleStabilizer> LegMiddleStabilizers { get; set; }

        /// <summary>
        /// Stabilizers on the lower end of Leg parts in this ACPARTS file.
        /// </summary>
        public List<LegLowerStabilizer> LegLowerStabilizers { get; set; }

        /// <summary>
        /// Makes a new <see cref="AcParts4"/>.
        /// </summary>
        public AcParts4()
        {
            Heads = new List<Head>();
            Cores = new List<Core>();
            Arms = new List<Arm>();
            Legs = new List<Leg>();
            FCSs = new List<FCS>();
            Generators = new List<Generator>();
            MainBoosters = new List<MainBooster>();
            BackBoosters = new List<BackBooster>();
            SideBoosters = new List<SideBooster>();
            OveredBoosters = new List<OveredBooster>();
            ArmUnits = new List<ArmUnit>();
            BackUnits = new List<BackUnit>();
            ShoulderUnits = new List<ShoulderUnit>();
            HeadTopStabilizers = new List<HeadTopStabilizer>();
            HeadSideStabilizers = new List<HeadSideStabilizer>();
            CoreUpperSideStabilizers = new List<CoreUpperSideStabilizer>();
            CoreLowerSideStabilizers = new List<CoreLowerSideStabilizer>();
            ArmStabilizers = new List<ArmStabilizer>();
            LegBackStabilizers = new List<LegBackStabilizer>();
            LegUpperStabilizers = new List<LegUpperStabilizer>();
            LegMiddleStabilizers = new List<LegMiddleStabilizer>();
            LegLowerStabilizers = new List<LegLowerStabilizer>();
        }

        /// <summary>
        /// Loads an AcParts file from a byte array.
        /// </summary>
        public static AcParts4 Read(byte[] bytes, AcParts4Version version)
        {
            using (BinaryReaderEx br = new BinaryReaderEx(false, bytes))
            {
                AcParts4 acparts = new AcParts4();
                acparts.Read(br, version);
                return acparts;
            }
        }

        /// <summary>
        /// Loads an AcParts file from the specified path.
        /// </summary>
        public static AcParts4 Read(string path, AcParts4Version version)
        {
            using (FileStream stream = File.OpenRead(path))
            using (BinaryReaderEx br = new BinaryReaderEx(false, stream, true))
            {
                AcParts4 acparts = new AcParts4();
                acparts.Read(br, version);
                return acparts;
            }
        }

        /// <summary>
        /// Deserializes file data from a stream.
        /// </summary>
        private void Read(BinaryReaderEx br, AcParts4Version version)
        {
            br.BigEndian = true;
            Version = version;

            br.AssertUInt32(0);
            ushort headCount = br.ReadUInt16();
            ushort coreCount = br.ReadUInt16();
            ushort armCount = br.ReadUInt16();
            ushort legCount = br.ReadUInt16();
            ushort fcsCount = br.ReadUInt16();
            ushort generatorCount = br.ReadUInt16();
            ushort mainBoosterCount = br.ReadUInt16();
            ushort backBoosterCount = br.ReadUInt16();
            ushort sideBoosterCount = br.ReadUInt16();
            ushort overedBoosterCount = br.ReadUInt16();
            ushort armUnitCount = br.ReadUInt16();
            ushort backUnitCount = br.ReadUInt16();
            ushort shoulderUnitCount = br.ReadUInt16();
            ushort headTopStabilizerCount = br.ReadUInt16();
            ushort headSideStabilizerCount = br.ReadUInt16();
            ushort coreUpperSideStabilizerCount = br.ReadUInt16();
            ushort coreLowerSideStabilizerCount = br.ReadUInt16();
            ushort armStabilizerCount = br.ReadUInt16();
            ushort legBackStabilizerCount = br.ReadUInt16();
            ushort legUpperStabilizerCount = br.ReadUInt16();
            ushort legMiddleStabilizerCount = br.ReadUInt16();
            ushort legLowerStabilizerCount = br.ReadUInt16();

            Heads.Capacity = headCount;
            for (int i = 0; i < headCount; i++)
                Heads.Add(new Head(br, Version));

            Cores.Capacity = coreCount;
            for (int i = 0; i < coreCount; i++)
                Cores.Add(new Core(br, Version));
            
            Arms.Capacity = armCount;
            for (int i = 0; i < armCount; i++)
                Arms.Add(new Arm(br, Version));
            
            Legs.Capacity = legCount;
            for (int i = 0; i < legCount; i++)
                Legs.Add(new Leg(br, Version));
            
            FCSs.Capacity = fcsCount;
            for (int i = 0; i < fcsCount; i++)
                FCSs.Add(new FCS(br, Version));
            
            Generators.Capacity = generatorCount;
            for (int i = 0; i < generatorCount; i++)
                Generators.Add(new Generator(br, Version));

            MainBoosters.Capacity = mainBoosterCount;
            for (int i = 0; i < mainBoosterCount; i++)
                MainBoosters.Add(new MainBooster(br, Version));

            BackBoosters.Capacity = backBoosterCount;
            for (int i = 0; i < backBoosterCount; i++)
                BackBoosters.Add(new BackBooster(br, Version));

            SideBoosters.Capacity = sideBoosterCount;
            for (int i = 0; i < sideBoosterCount; i++)
                SideBoosters.Add(new SideBooster(br, Version));
            
            OveredBoosters.Capacity = overedBoosterCount;
            for (int i = 0; i < overedBoosterCount; i++)
                OveredBoosters.Add(new OveredBooster(br, Version));

            ArmUnits.Capacity = armUnitCount;
            for (int i = 0; i < armUnitCount; i++)
                ArmUnits.Add(new ArmUnit(br, Version));

            BackUnits.Capacity = backUnitCount;
            for (int i = 0; i < backUnitCount; i++)
                BackUnits.Add(new BackUnit(br, Version));

            ShoulderUnits.Capacity = shoulderUnitCount;
            for (int i = 0; i < shoulderUnitCount; i++)
                ShoulderUnits.Add(new ShoulderUnit(br, Version));

            HeadTopStabilizers.Capacity = headTopStabilizerCount;
            for (int i = 0; i < headTopStabilizerCount; i++)
                HeadTopStabilizers.Add(new HeadTopStabilizer(br, Version));

            HeadSideStabilizers.Capacity = headSideStabilizerCount;
            for (int i = 0; i < headSideStabilizerCount; i++)
                HeadSideStabilizers.Add(new HeadSideStabilizer(br, Version));

            CoreUpperSideStabilizers.Capacity = coreUpperSideStabilizerCount;
            for (int i = 0; i < coreUpperSideStabilizerCount; i++)
                CoreUpperSideStabilizers.Add(new CoreUpperSideStabilizer(br, Version));

            CoreLowerSideStabilizers.Capacity = coreLowerSideStabilizerCount;
            for (int i = 0; i < coreLowerSideStabilizerCount; i++)
                CoreLowerSideStabilizers.Add(new CoreLowerSideStabilizer(br, Version));

            ArmStabilizers.Capacity = armStabilizerCount;
            for (int i = 0; i < armStabilizerCount; i++)
                ArmStabilizers.Add(new ArmStabilizer(br, Version));

            LegBackStabilizers.Capacity = legBackStabilizerCount;
            for (int i = 0; i < legBackStabilizerCount; i++)
                LegBackStabilizers.Add(new LegBackStabilizer(br, Version));

            LegUpperStabilizers.Capacity = legUpperStabilizerCount;
            for (int i = 0; i < legUpperStabilizerCount; i++)
                LegUpperStabilizers.Add(new LegUpperStabilizer(br, Version));

            LegMiddleStabilizers.Capacity = legMiddleStabilizerCount;
            for (int i = 0; i < legMiddleStabilizerCount; i++)
                LegMiddleStabilizers.Add(new LegMiddleStabilizer(br, Version));

            LegLowerStabilizers.Capacity = legLowerStabilizerCount;
            for (int i = 0; i < legLowerStabilizerCount; i++)
                LegLowerStabilizers.Add(new LegLowerStabilizer(br, Version));
        }

        /// <summary>
        /// Serializes file data to a stream.
        /// </summary>
        protected override void Write(BinaryWriterEx bw)
        {
            bw.BigEndian = true;
            bw.WriteUInt32(0);
            bw.WriteUInt16((ushort)Heads.Count);
            bw.WriteUInt16((ushort)Cores.Count);
            bw.WriteUInt16((ushort)Arms.Count);
            bw.WriteUInt16((ushort)Legs.Count);
            bw.WriteUInt16((ushort)FCSs.Count);
            bw.WriteUInt16((ushort)Generators.Count);
            bw.WriteUInt16((ushort)MainBoosters.Count);
            bw.WriteUInt16((ushort)BackBoosters.Count);
            bw.WriteUInt16((ushort)SideBoosters.Count);
            bw.WriteUInt16((ushort)OveredBoosters.Count);
            bw.WriteUInt16((ushort)ArmUnits.Count);
            bw.WriteUInt16((ushort)BackUnits.Count);
            bw.WriteUInt16((ushort)ShoulderUnits.Count);
            bw.WriteUInt16((ushort)HeadTopStabilizers.Count);
            bw.WriteUInt16((ushort)HeadSideStabilizers.Count);
            bw.WriteUInt16((ushort)CoreUpperSideStabilizers.Count);
            bw.WriteUInt16((ushort)CoreLowerSideStabilizers.Count);
            bw.WriteUInt16((ushort)ArmStabilizers.Count);
            bw.WriteUInt16((ushort)LegBackStabilizers.Count);
            bw.WriteUInt16((ushort)LegUpperStabilizers.Count);
            bw.WriteUInt16((ushort)LegMiddleStabilizers.Count);
            bw.WriteUInt16((ushort)LegLowerStabilizers.Count);

            for (int i = 0; i < Heads.Count; i++)
                Heads[i].Write(bw, Version);

            for (int i = 0; i < Cores.Count; i++)
                Cores[i].Write(bw, Version);

            for (int i = 0; i < Arms.Count; i++)
                Arms[i].Write(bw, Version);

            for (int i = 0; i < Legs.Count; i++)
                Legs[i].Write(bw, Version);

            for (int i = 0; i < FCSs.Count; i++)
                FCSs[i].Write(bw, Version);

            for (int i = 0; i < Generators.Count; i++)
                Generators[i].Write(bw, Version);

            for (int i = 0; i < MainBoosters.Count; i++)
                MainBoosters[i].Write(bw, Version);

            for (int i = 0; i < BackBoosters.Count; i++)
                BackBoosters[i].Write(bw, Version);

            for (int i = 0; i < SideBoosters.Count; i++)
                SideBoosters[i].Write(bw, Version);

            for (int i = 0; i < OveredBoosters.Count; i++)
                OveredBoosters[i].Write(bw, Version);

            for (int i = 0; i < ArmUnits.Count; i++)
                ArmUnits[i].Write(bw, Version);

            for (int i = 0; i < BackUnits.Count; i++)
                BackUnits[i].Write(bw, Version);

            for (int i = 0; i < ShoulderUnits.Count; i++)
                ShoulderUnits[i].Write(bw, Version);

            for (int i = 0; i < HeadTopStabilizers.Count; i++)
                HeadTopStabilizers[i].Write(bw, Version);

            for (int i = 0; i < HeadSideStabilizers.Count; i++)
                HeadSideStabilizers[i].Write(bw, Version);

            for (int i = 0; i < CoreUpperSideStabilizers.Count; i++)
                CoreUpperSideStabilizers[i].Write(bw, Version);

            for (int i = 0; i < CoreLowerSideStabilizers.Count; i++)
                CoreLowerSideStabilizers[i].Write(bw, Version);

            for (int i = 0; i < ArmStabilizers.Count; i++)
                ArmStabilizers[i].Write(bw, Version);

            for (int i = 0; i < LegBackStabilizers.Count; i++)
                LegBackStabilizers[i].Write(bw, Version);

            for (int i = 0; i < LegUpperStabilizers.Count; i++)
                LegUpperStabilizers[i].Write(bw, Version);

            for (int i = 0; i < LegMiddleStabilizers.Count; i++)
                LegMiddleStabilizers[i].Write(bw, Version);

            for (int i = 0; i < LegLowerStabilizers.Count; i++)
                LegLowerStabilizers[i].Write(bw, Version);
        }

        /// <summary>
        /// Gets the number of parts in an ACPARTS file.
        /// </summary>
        /// <returns>An int representing the number of parts in an ACPARTS file.</returns>
        public int Count()
        {
            return Heads.Count +
                Cores.Count +
                Arms.Count +
                Legs.Count +
                FCSs.Count +
                Generators.Count +
                MainBoosters.Count +
                BackBoosters.Count +
                SideBoosters.Count +
                OveredBoosters.Count +
                ArmUnits.Count +
                BackUnits.Count +
                ShoulderUnits.Count +
                HeadTopStabilizers.Count +
                HeadSideStabilizers.Count +
                CoreUpperSideStabilizers.Count +
                CoreLowerSideStabilizers.Count +
                ArmStabilizers.Count +
                LegBackStabilizers.Count +
                LegUpperStabilizers.Count +
                LegMiddleStabilizers.Count +
                LegLowerStabilizers.Count;
        }

        /// <summary>
        /// Get all the parts in this AcParts as a list of IPart.
        /// </summary>
        /// <returns>A list of IPart.</returns>
        public List<IPart> GetParts()
        {
            List<IPart> parts = new List<IPart>(Count());
            parts.AddRange(Heads);
            parts.AddRange(Cores);
            parts.AddRange(Arms);
            parts.AddRange(Legs);
            parts.AddRange(FCSs);
            parts.AddRange(Generators);
            parts.AddRange(MainBoosters);
            parts.AddRange(BackBoosters);
            parts.AddRange(SideBoosters);
            parts.AddRange(OveredBoosters);
            parts.AddRange(ArmUnits);
            parts.AddRange(BackUnits);
            parts.AddRange(ShoulderUnits);
            parts.AddRange(HeadTopStabilizers);
            parts.AddRange(HeadSideStabilizers);
            parts.AddRange(CoreUpperSideStabilizers);
            parts.AddRange(CoreLowerSideStabilizers);
            parts.AddRange(ArmStabilizers);
            parts.AddRange(LegBackStabilizers);
            parts.AddRange(LegUpperStabilizers);
            parts.AddRange(LegMiddleStabilizers);
            parts.AddRange(LegLowerStabilizers);
            return parts;
        }
    }
}
