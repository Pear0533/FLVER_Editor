﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using SoulsFormats;

namespace SoulsAssetPipeline.Animation
{
    /// <summary>
    /// Controls when different events happen during animations; this specific version is used in DS3. Extension: .tae
    /// </summary>
    public partial class TAE : SoulsFile<TAE>
    {
        /// <summary>
        /// Which format this file is.
        /// </summary>
        public enum TAEFormat
        {
            /// <summary>
            /// Dark Souls 1.
            /// </summary>
            DS1 = 0,
            /// <summary>
            /// Dark Souls II: Scholar of the First Sin. 
            /// Does not support 32-bit original Dark Souls II release.
            /// </summary>
            SOTFS = 1,
            /// <summary>
            /// Dark Souls III. Same value as Bloodborne.
            /// </summary>
            DS3 = 2,
            /// <summary>
            /// Bloodborne. Same value as Dark Souls III.
            /// </summary>
            BB = 2,
            /// <summary>
            /// Sekiro: Shadows Die Twice
            /// </summary>
            SDT = 3,
            /// <summary>
            /// Demon's Souls
            /// </summary>
            DES = 4,
        }

        /// <summary>
        /// The format of this file. Different between most games.
        /// </summary>
        public TAEFormat Format { get; set; }

        /// <summary>
        /// Whether the format is big endian.
        /// Only valid for DES/DS1 files.
        /// </summary>
        public bool BigEndian { get; set; }

        /// <summary>
        /// ID number of this TAE.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Unknown flags.
        /// </summary>
        public byte[] Flags { get; private set; }

        /// <summary>
        /// Unknown .hkt file.
        /// </summary>
        public string SkeletonName { get; set; }

        /// <summary>
        /// Unknown .sib file.
        /// </summary>
        public string SibName { get; set; }

        /// <summary>
        /// Animations controlled by this TAE.
        /// </summary>
        public List<Animation> Animations;

        /// <summary>
        /// What set of events this TAE uses. Can be different within the same game.
        /// Often found in OBJ TAEs.
        /// Not stored in DS1 TAE files.
        /// </summary>
        public long EventBank { get; set; }

        /// <summary>
        /// The template currently applied. Set by ApplyTemplate method.
        /// </summary>
        public Template AppliedTemplate { get; private set; }

        /// <summary>
        /// Gets the current bank being used in the currently applied template, if a template is applied.
        /// </summary>
        public Template.BankTemplate BankTemplate => AppliedTemplate?[EventBank];

        /// <summary>
        /// Applies a template to this TAE for easier editing.
        /// After applying template, use events' .Parameters property.
        /// </summary>
        public void ApplyTemplate(Template template)
        {
            if (template.Game != Format)
                throw new InvalidOperationException($"Template is for {template.Game} but this TAE is for {Format}.");

            if (template.ContainsKey(EventBank))
            {
                foreach (var anim in Animations)
                {
                    for (int i = 0; i < anim.Events.Count; i++)
                    {
                        anim.Events[i].ApplyTemplate(this, template, anim.ID, i, anim.Events[i].Type);
                    }
                }
            }
            else
            {
                throw new InvalidOperationException($"This TAE uses event bank {EventBank} but no such bank exists in the template.");
            }

            AppliedTemplate = template;
        }

        /// <summary>
        /// For use with porting to other games etc. Make sure fields are named the same between games or this will throw a KeyNotFoundException (obviously).
        /// </summary>
        public void ChangeTemplateAfterLoading(Template template)
        {
            if (template.Game != Format)
                throw new InvalidOperationException($"Template is for {template.Game} but this TAE is for {Format}.");

            if (template.ContainsKey(EventBank))
            {
                foreach (var anim in Animations)
                {
                    for (int i = 0; i < anim.Events.Count; i++)
                    {
                        anim.Events[i].ChangeTemplateAfterLoading(this, template, anim.ID, i, anim.Events[i].Type);
                    }
                }
            }
            else
            {
                throw new InvalidOperationException($"This TAE uses event bank {EventBank} but no such bank exists in the template.");
            }

            AppliedTemplate = template;
        }

        protected override bool Is(BinaryReaderEx br)
        {
            string magic = br.GetASCII(0, 4);
            return magic == "TAE ";
        }

        protected override void Read(BinaryReaderEx br)
        {
            br.BigEndian = false;
            br.VarintLong = false;

            br.AssertASCII("TAE ");

            BigEndian = br.AssertByte(0, 1) == 1;
            br.BigEndian = BigEndian;

            br.AssertByte(0);
            br.AssertByte(0);

            bool is64Bit = br.AssertByte(0, 0xFF) == 0xFF;
            br.VarintLong = is64Bit;

            // 0x1000B: DeS, DS1(R)
            // 0x1000C: DS2, DS2 SOTFS, BB, DS3
            // 0x1000D: SDT
            int version = br.AssertInt32(0x1000B, 0x1000C, 0x1000D);

            if (version == 0x1000B && !is64Bit)
            {
                Format = TAEFormat.DS1;
            }
            else if (version == 0x1000C && !is64Bit)
            {
                throw new NotImplementedException("Dark Souls II 32-Bit original release not supported. Only Scholar of the First Sin.");
            }
            else if (version == 0x1000C && is64Bit)
            {
                Format = TAEFormat.DS3;
            }
            else if (version == 0x1000D)
            {
                Format = TAEFormat.SDT;
            }
            else
            {
                throw new System.IO.InvalidDataException("Invalid combination of TAE header values: " +
                    $"IsBigEndian={BigEndian}, Is64Bit={is64Bit}, Version={version}");
            }

            br.ReadInt32(); // File size
            br.AssertVarint(0x40);
            br.AssertVarint(1);
            br.AssertVarint(0x50);

            if (is64Bit)
                br.AssertVarint(0x80);
            else
                br.AssertVarint(0x70);

            if (Format == TAEFormat.DS1)
            {
                var subFormat = br.AssertInt16(0, 2);
                if (subFormat == 0)
                    Format = TAEFormat.DES;
                br.AssertInt16(1);
            }
            else
            {
                EventBank = br.ReadVarint();
            }

            br.AssertVarint(0);

            if (Format == TAEFormat.DS1 || Format == TAEFormat.DES)
            {
                br.AssertInt64(0);
                br.AssertInt64(0);
                br.AssertInt64(0);
            }

            Flags = br.ReadBytes(8);

            var unkFlagA = br.ReadBoolean();
            var unkFlagB = br.ReadBoolean();

            if (!unkFlagA && unkFlagB)
                Format = TAEFormat.SOTFS;
            else if ((unkFlagA && unkFlagB) || (!unkFlagA && !unkFlagB))
                throw new System.IO.InvalidDataException("Invalid unknown flags at 0x48.");

            for (int i = 0; i < 6; i++)
                br.AssertByte(0);

            ID = br.ReadInt32();

            int animCount = br.ReadInt32();
            long animsOffset = br.ReadVarint();
            br.ReadVarint(); // Anim groups offset

            br.AssertVarint((Format == TAEFormat.DES || Format == TAEFormat.DS1) ? 0x90 : 0xA0);
            br.AssertVarint(animCount);
            br.ReadVarint(); // First anim offset
            if (Format == TAEFormat.DS1 || Format == TAEFormat.DES)
                br.AssertInt32(0);
            br.AssertVarint(1);
            br.AssertVarint((Format == TAEFormat.DES || Format == TAEFormat.DS1) ? 0x80 : 0x90);
            if (Format == TAEFormat.DS1 || Format == TAEFormat.DES)
                br.AssertInt64(0);
            br.AssertInt32(ID);
            br.AssertInt32(ID);
            br.AssertVarint(0x50);
            br.AssertInt64(0);

            if (Format == TAEFormat.DES)
                br.AssertVarint(0xA0);
            else if (Format == TAEFormat.DS1)
                br.AssertVarint(0x98);
            else
                br.AssertVarint(0xB0);

            long skeletonNameOffset = 0;
            long sibNameOffset = 0;

            if (BigEndian)
            {
                if (Format != TAEFormat.SOTFS)
                {
                    br.AssertVarint(0);
                    br.AssertVarint(0);
                }

                skeletonNameOffset = br.ReadVarint();
                sibNameOffset = br.ReadVarint();
            }
            else
            {
                skeletonNameOffset = br.ReadVarint();
                sibNameOffset = br.ReadVarint();

                if (Format != TAEFormat.SOTFS)
                {
                    br.AssertVarint(0);
                    br.AssertVarint(0);
                }
            }

            

            if (Format != TAEFormat.SOTFS)
            {
                SkeletonName = skeletonNameOffset == 0 ? null : br.GetUTF16(skeletonNameOffset);
                SibName = sibNameOffset == 0 ? null : br.GetUTF16(sibNameOffset);
            }

            br.StepIn(animsOffset);
            {
                Animations = new List<Animation>(animCount);
                bool previousAnimNeedsParamGen = false;
                long previousAnimParamStart = 0;
                for (int i = 0; i < animCount; i++)
                {
                    Animations.Add(new Animation(br, Format, 
                        out bool lastEventNeedsParamGen, 
                        out long animFileOffset, out long lastEventParamOffset));

                    if (previousAnimNeedsParamGen)
                    {
                        br.StepIn(previousAnimParamStart);
                        Animations[i - 1].Events[Animations[i - 1].Events.Count - 1].ReadParameters(br, (int)(animFileOffset - previousAnimParamStart));
                        br.StepOut();
                    }

                    previousAnimNeedsParamGen = lastEventNeedsParamGen;
                    previousAnimParamStart = lastEventParamOffset;
                }

                // Read from very last anim's very last event's parameters offset to end of file lul
                if (previousAnimNeedsParamGen)
                {
                    br.StepIn(previousAnimParamStart);
                    Animations[Animations.Count - 1].Events[Animations[Animations.Count - 1].Events.Count - 1].ReadParameters(br, (int)(br.Length - previousAnimParamStart));
                    br.StepOut();
                }
            }
            br.StepOut();

            // Don't bother reading anim groups.
        }

        protected override void Write(BinaryWriterEx bw)
        {
            bw.WriteASCII("TAE ");

            bw.BigEndian = BigEndian;

            bw.WriteBoolean(BigEndian);
            bw.WriteByte(0);
            bw.WriteByte(0);

            if (Format == TAEFormat.DES || Format == TAEFormat.DS1)
            {
                bw.VarintLong = false;
                bw.WriteByte(0);
            }
            else
            {
                bw.VarintLong = true;
                bw.WriteByte(0xFF);
            }

            if (Format == TAEFormat.DES || Format == TAEFormat.DS1)
                bw.WriteInt32(0x1000B);
            else if (Format == TAEFormat.DS3 || Format == TAEFormat.SOTFS)
                bw.WriteInt32(0x1000C);
            else if (Format == TAEFormat.SDT)
                bw.WriteInt32(0x1000D);

            bw.ReserveInt32("FileSize");
            bw.WriteVarint(0x40);
            bw.WriteVarint(1);
            bw.WriteVarint(0x50);
            bw.WriteVarint(bw.VarintLong ? 0x80 : 0x70);

            if ((Format == TAEFormat.DES || Format == TAEFormat.DS1))
            {
                bw.WriteInt16((short)(Format == TAEFormat.DES ? 0 : 2));
                bw.WriteInt16(1);
            }
            else
            {
                bw.WriteVarint(EventBank);
            }
            
            bw.WriteVarint(0);

            //DeS also
            if (Format == TAEFormat.DES || Format == TAEFormat.DS1)
            {
                bw.WriteInt64(0);
                bw.WriteInt64(0);
                bw.WriteInt64(0);
            }

            bw.WriteBytes(Flags);

            if (Format == TAEFormat.SOTFS)
            {
                bw.WriteByte(0);
                bw.WriteByte(1);
            }
            else
            {
                bw.WriteByte(1);
                bw.WriteByte(0);
            }

            for (int i = 0; i < 6; i++)
                bw.WriteByte(0);

            bw.WriteInt32(ID);
            bw.WriteInt32(Animations.Count);
            bw.ReserveVarint("AnimsOffset");
            bw.ReserveVarint("AnimGroupsOffset");
            bw.WriteVarint((Format == TAEFormat.DES || Format == TAEFormat.DS1) ? 0x90 : 0xA0);
            bw.WriteVarint(Animations.Count);
            bw.ReserveVarint("FirstAnimOffset");
            if (Format == TAEFormat.DES || Format == TAEFormat.DS1)
                bw.WriteInt32(0);
            bw.WriteVarint(1);
            bw.WriteVarint((Format == TAEFormat.DES || Format == TAEFormat.DS1) ? 0x80 : 0x90);
            if (Format == TAEFormat.DES || Format == TAEFormat.DS1)
                bw.WriteInt64(0);
            bw.WriteInt32(ID);
            bw.WriteInt32(ID);
            bw.WriteVarint(0x50);
            bw.WriteInt64(0);

            if (Format == TAEFormat.DES)
                bw.WriteVarint(0xA0);
            else if (Format == TAEFormat.DS1)
                bw.WriteVarint(0x98);
            else
                bw.WriteVarint(0xB0);


            if (BigEndian)
            {
                if (Format != TAEFormat.SOTFS)
                {
                    bw.WriteVarint(0);
                    bw.WriteVarint(0);
                }

                bw.ReserveVarint("SkeletonName");
                bw.ReserveVarint("SibName");
            }
            else
            {
                bw.ReserveVarint("SkeletonName");
                bw.ReserveVarint("SibName");

                if (Format != TAEFormat.SOTFS)
                {
                    bw.WriteVarint(0);
                    bw.WriteVarint(0);
                }
            }

            

            if (SkeletonName != null)
            {
                bw.FillVarint("SkeletonName", bw.Position);
                if (!string.IsNullOrEmpty(SkeletonName))
                {
                    bw.WriteUTF16(SkeletonName, true);
                    if (bw.VarintLong || Format == TAEFormat.DES)
                        bw.Pad(0x10);
                }
            }
            else
            {
                bw.FillVarint("SkeletonName", 0);
            }

            if (SibName != null)
            {
                bw.FillVarint("SibName", bw.Position);
                if (!string.IsNullOrEmpty(SibName))
                {
                    bw.WriteUTF16(SibName, true);
                    if (bw.VarintLong || Format == TAEFormat.DES)
                        bw.Pad(0x10);
                }
            }
            else
            {
                bw.FillVarint("SibName", 0);
            }

            Animations.Sort((a1, a2) => a1.ID.CompareTo(a2.ID));

            var animOffsets = new List<long>(Animations.Count);
            if (Animations.Count == 0)
            {
                bw.FillVarint("AnimsOffset", 0);
            }
            else
            {
                bw.FillVarint("AnimsOffset", bw.Position);
                for (int i = 0; i < Animations.Count; i++)
                {
                    animOffsets.Add(bw.Position);
                    Animations[i].WriteHeader(bw, i, Format);
                }
            }

            bw.FillVarint("AnimGroupsOffset", bw.Position);
            bw.ReserveVarint("AnimGroupsCount");
            bw.ReserveVarint("AnimGroupsOffset");
            int groupCount = 0;
            long groupStart = bw.Position;
            for (int i = 0; i < Animations.Count; i++)
            {
                int firstIndex = i;
                bw.WriteInt32((int)Animations[i].ID);
                while (i < Animations.Count - 1 && Animations[i + 1].ID == Animations[i].ID + 1)
                    i++;
                bw.WriteInt32((int)Animations[i].ID);
                bw.WriteVarint(animOffsets[firstIndex]);
                groupCount++;
            }
            bw.FillVarint("AnimGroupsCount", groupCount);

            if (groupCount == 0)
                bw.FillVarint("AnimGroupsOffset", 0);
            else
                bw.FillVarint("AnimGroupsOffset", groupStart);

            if (Animations.Count == 0)
            {
                bw.FillVarint("FirstAnimOffset", 0);
            }
            else
            {
                bw.FillVarint("FirstAnimOffset", bw.Position);
                for (int i = 0; i < Animations.Count; i++)
                    Animations[i].WriteBody(bw, i, Format);
            }

            for (int i = 0; i < Animations.Count; i++)
            {
                Animation anim = Animations[i];
                anim.WriteAnimFile(bw, i, Format);
                Dictionary<float, long> timeOffsets = anim.WriteTimes(bw, i, Format);
                List<long> eventHeaderOffsets = anim.WriteEventHeaders(bw, i, timeOffsets, Format);
                anim.WriteEventData(bw, i, Format);
                anim.WriteEventGroupHeaders(bw, i, Format);
                anim.WriteEventGroupData(bw, i, eventHeaderOffsets, Format);
            }

            bw.FillInt32("FileSize", (int)bw.Position);
        }

    }
}
