using System.Drawing;

namespace SoulsFormats.Other.AC4
{
    /// <summary>
    /// A color set for a 4thgen Armored Core.<br/>
    /// Used within design saves.<br/>
    /// Files of AcColorSets are formatted as color%04d.bin.
    /// </summary>
    public class AcColorSet4 : SoulsFile<AcColorSet4>
    {
        #region ColorSets

        /// <summary>
        /// The head color set.
        /// </summary>
        public ColorSet HeadColor { get; set; }

        /// <summary>
        /// The core color set.
        /// </summary>
        public ColorSet CoreColor { get; set; }

        /// <summary>
        /// The right arm color set.
        /// </summary>
        public ColorSet ArmRightColor { get; set; }

        /// <summary>
        /// The left arm color set.
        /// </summary>
        public ColorSet ArmLeftColor { get; set; }

        /// <summary>
        /// The color set for legs.
        /// </summary>
        public ColorSet LegsColor { get; set; }

        /// <summary>
        /// The right arm unit color set.
        /// </summary>
        public ColorSet ArmUnitRightColor { get; set; }

        /// <summary>
        /// The left arm unit color set.
        /// </summary>
        public ColorSet ArmUnitLeftColor { get; set; }

        /// <summary>
        /// The right back unit color set.
        /// </summary>
        public ColorSet BackUnitRightColor { get; set; }

        /// <summary>
        /// The left back unit color set.
        /// </summary>
        public ColorSet BackUnitLeftColor { get; set; }

        /// <summary>
        /// The shoulder unit color set.
        /// </summary>
        public ColorSet ShoulderUnitColor { get; set; }

        /// <summary>
        /// The right hanger unit color set.
        /// </summary>
        public ColorSet HangerUnitRightColor { get; set; }

        /// <summary>
        /// The left hanger unit color set.
        /// </summary>
        public ColorSet HangerUnitLeftColor { get; set; }

        /// <summary>
        /// The top head stabilizer color set.
        /// </summary>
        public ColorSet HeadTopStabilizerColor { get; set; }

        /// <summary>
        /// The right side head stabilizer color set.
        /// </summary>
        public ColorSet HeadRightStabilizerColor { get; set; }

        /// <summary>
        /// The left side head stabilizer color set.
        /// </summary>
        public ColorSet HeadLeftStabilizerColor { get; set; }

        /// <summary>
        /// The upper right side core stabilizer color set.
        /// </summary>
        public ColorSet CoreUpperRightStabilizerColor { get; set; }

        /// <summary>
        /// The upper left side core stabilizer color set.
        /// </summary>
        public ColorSet CoreUpperLeftStabilizerColor { get; set; }

        /// <summary>
        /// The lower right side core stabilizer color set.
        /// </summary>
        public ColorSet CoreLowerRightStabilizerColor { get; set; }

        /// <summary>
        /// The lower left side core stabilizer color set.
        /// </summary>
        public ColorSet CoreLowerLeftStabilizerColor { get; set; }

        /// <summary>
        /// The right arm stabilizer color set.
        /// </summary>
        public ColorSet ArmRightStabilizerColor { get; set; }

        /// <summary>
        /// The left arm stabilizer color set.
        /// </summary>
        public ColorSet ArmLeftStabilizerColor { get; set; }

        /// <summary>
        /// The back leg stabilizer color set.
        /// </summary>
        public ColorSet LegsBackStabilizerColor { get; set; }

        /// <summary>
        /// The back leg stabilizer color set.
        /// </summary>
        public ColorSet LegsUpperRightStabilizerColor { get; set; }

        /// <summary>
        /// The upper left leg stabilizer color set.
        /// </summary>
        public ColorSet LegsUpperLeftStabilizerColor { get; set; }

        /// <summary>
        /// The back upper right leg stabilizer color set, unused.
        /// </summary>
        public ColorSet LegsUpperRightBackStabilizerColor { get; set; }

        /// <summary>
        /// The back upper left leg stabilizer color set, unused.
        /// </summary>
        public ColorSet LegsUpperLeftBackStabilizerColor { get; set; }

        /// <summary>
        /// The middle right side leg stabilizer color set.
        /// </summary>
        public ColorSet LegsMiddleRightStabilizerColor { get; set; }

        /// <summary>
        /// The middle left side leg stabilizer color set.
        /// </summary>
        public ColorSet LegsMiddleLeftStabilizerColor { get; set; }

        /// <summary>
        /// The back middle right leg stabilizer color set, unused.
        /// </summary>
        public ColorSet LegsMiddleRightBackStabilizerColor { get; set; }

        /// <summary>
        /// The back middle left leg stabilizer color set, unused.
        /// </summary>
        public ColorSet LegsMiddleLeftBackStabilizerColor { get; set; }

        /// <summary>
        /// The lower right side leg stabilizer color set.
        /// </summary>
        public ColorSet LegsLowerRightStabilizerColor { get; set; }

        /// <summary>
        /// The lower left side leg stabilizer color set.
        /// </summary>
        public ColorSet LegsLowerLeftStabilizerColor { get; set; }

        /// <summary>
        /// The back right left leg stabilizer color set, unused.
        /// </summary>
        public ColorSet LegsLowerRightBackStabilizerColor { get; set; }

        /// <summary>
        /// The back lower left leg stabilizer color set, unused.
        /// </summary>
        public ColorSet LegsLowerLeftBackStabilizerColor { get; set; }

        #endregion

        #region Color Patterns

        /// <summary>
        /// The color pattern to be applied to all color sets.
        /// </summary>
        public ColorPattern AllPattern { get; set; }

        /// <summary>
        /// The color pattern to be applied to all frame color sets.
        /// </summary>
        public ColorPattern AllFramesPattern { get; set; }

        /// <summary>
        /// The color pattern to be applied to all unit color sets.
        /// </summary>
        public ColorPattern AllUnitsPattern { get; set; }

        /// <summary>
        /// The color pattern to be applied to all stabilizer color sets.
        /// </summary>
        public ColorPattern AllStabilizersPattern { get; set; }

        /// <summary>
        /// The color pattern set for the head color set.
        /// </summary>
        public ColorPattern HeadPattern { get; set; }

        /// <summary>
        /// The color pattern set for the core color set.
        /// </summary>
        public ColorPattern CorePattern { get; set; }

        /// <summary>
        /// The color pattern set for the right arm color set.
        /// </summary>
        public ColorPattern ArmRightPattern { get; set; }

        /// <summary>
        /// The color pattern set for the left arm color set.
        /// </summary>
        public ColorPattern ArmLeftPattern { get; set; }

        /// <summary>
        /// The color pattern set for the legs color set.
        /// </summary>
        public ColorPattern LegsPattern { get; set; }

        /// <summary>
        /// The color pattern set for the right arm unit color set.
        /// </summary>
        public ColorPattern ArmUnitRightPattern { get; set; }

        /// <summary>
        /// The color pattern set for the left arm unit color set.
        /// </summary>
        public ColorPattern ArmUnitLeftPattern { get; set; }

        /// <summary>
        /// The color pattern set for the right back unit color set.
        /// </summary>
        public ColorPattern BackUnitRightPattern { get; set; }

        /// <summary>
        /// The color pattern set for the left back unit color set.
        /// </summary>
        public ColorPattern BackUnitLeftPattern { get; set; }

        /// <summary>
        /// The color pattern set for the shoulder unit color set.
        /// </summary>
        public ColorPattern ShoulderUnitPattern { get; set; }

        /// <summary>
        /// The color pattern set for the right hangar unit color set.
        /// </summary>
        public ColorPattern HangerUnitRightPattern { get; set; }

        /// <summary>
        /// The color pattern set for the left hangar unit color set.
        /// </summary>
        public ColorPattern HangerUnitLeftPattern { get; set; }

        /// <summary>
        /// The color pattern to be applied to all head stabilizer color sets.
        /// </summary>
        public ColorPattern AllHeadStabilizersPattern { get; set; }

        /// <summary>
        /// The color pattern to be applied to all core stabilizer color sets.
        /// </summary>
        public ColorPattern AllCoreStabilizersPattern { get; set; }

        /// <summary>
        /// The color pattern to be applied to all arm stabilizer color sets.
        /// </summary>
        public ColorPattern AllArmStabilizersPattern { get; set; }

        /// <summary>
        /// The color pattern to be applied to all leg stabilizer color sets.
        /// </summary>
        public ColorPattern AllLegStabilizersPattern { get; set; }

        /// <summary>
        /// The color pattern set for the top head stabilizer color set.
        /// </summary>
        public ColorPattern HeadTopStabilizerPattern { get; set; }

        /// <summary>
        /// The color pattern set for the right head stabilizer color set.
        /// </summary>
        public ColorPattern HeadRightStabilizerPattern { get; set; }

        /// <summary>
        /// The color pattern set for the left head stabilizer color set.
        /// </summary>
        public ColorPattern HeadLeftStabilizerPattern { get; set; }

        /// <summary>
        /// The color pattern set for the upper right core stabilizer color set.
        /// </summary>
        public ColorPattern CoreUpperRightStabilizerPattern { get; set; }

        /// <summary>
        /// The color pattern set for the upper left core stabilizer color set.
        /// </summary>
        public ColorPattern CoreUpperLeftStabilizerPattern { get; set; }

        /// <summary>
        /// The color pattern set for the lower right core stabilizer color set.
        /// </summary>
        public ColorPattern CoreLowerRightStabilizerPattern { get; set; }

        /// <summary>
        /// The color pattern set for the lower left core stabilizer color set.
        /// </summary>
        public ColorPattern CoreLowerLeftStabilizerPattern { get; set; }

        /// <summary>
        /// The color pattern set for the right arm stabilizer color set.
        /// </summary>
        public ColorPattern ArmRightStabilizerPattern { get; set; }

        /// <summary>
        /// The color pattern set for the left arm stabilizer color set.
        /// </summary>
        public ColorPattern ArmLeftStabilizerPattern { get; set; }

        /// <summary>
        /// The color pattern set for the stabilizer on the back of legs' color set.
        /// </summary>
        public ColorPattern LegsBackStabilizerPattern { get; set; }

        /// <summary>
        /// The color pattern set for the upper right stabilizer color set.
        /// </summary>
        public ColorPattern LegsUpperRightStabilizerPattern { get; set; }

        /// <summary>
        /// The color pattern set for the upper left stabilizer color set.
        /// </summary>
        public ColorPattern LegsUpperLeftStabilizerPattern { get; set; }

        /// <summary>
        /// The color pattern set for the middle right stabilizer color set.
        /// </summary>
        public ColorPattern LegsMiddleRightStabilizerPattern { get; set; }

        /// <summary>
        /// The color pattern set for the middle left stabilizer color set.
        /// </summary>
        public ColorPattern LegsMiddleLeftStabilizerPattern { get; set; }

        /// <summary>
        /// The color pattern set for the lower right stabilizer color set.
        /// </summary>
        public ColorPattern LegsLowerRightStabilizerPattern { get; set; }

        /// <summary>
        /// The color pattern set for the lower left stabilizer color set.
        /// </summary>
        public ColorPattern LegsLowerLeftStabilizerPattern { get; set; }

        #endregion

        #region Eye Color

        /// <summary>
        /// The eye color.
        /// </summary>
        public Color EyeColor { get; set; }

        #endregion

        #region Read

        /// <summary>
        /// Deserializes file data from a stream.
        /// </summary>
        protected override void Read(BinaryReaderEx br)
        {
            HeadColor = new ColorSet(br);
            CoreColor = new ColorSet(br);
            ArmRightColor = new ColorSet(br);
            ArmLeftColor = new ColorSet(br);
            LegsColor = new ColorSet(br);
            ArmUnitRightColor = new ColorSet(br);
            ArmUnitLeftColor = new ColorSet(br);
            BackUnitRightColor = new ColorSet(br);
            BackUnitLeftColor = new ColorSet(br);
            ShoulderUnitColor = new ColorSet(br);
            HangerUnitRightColor = new ColorSet(br);
            HangerUnitLeftColor = new ColorSet(br);
            HeadTopStabilizerColor = new ColorSet(br);
            HeadRightStabilizerColor = new ColorSet(br);
            HeadLeftStabilizerColor = new ColorSet(br);
            CoreUpperRightStabilizerColor = new ColorSet(br);
            CoreUpperLeftStabilizerColor = new ColorSet(br);
            CoreLowerRightStabilizerColor = new ColorSet(br);
            CoreLowerLeftStabilizerColor = new ColorSet(br);
            ArmRightStabilizerColor = new ColorSet(br);
            ArmLeftStabilizerColor = new ColorSet(br);
            LegsBackStabilizerColor = new ColorSet(br);
            LegsUpperRightStabilizerColor = new ColorSet(br);
            LegsUpperLeftStabilizerColor = new ColorSet(br);
            LegsUpperRightBackStabilizerColor = new ColorSet(br);
            LegsUpperLeftBackStabilizerColor = new ColorSet(br);
            LegsMiddleRightStabilizerColor = new ColorSet(br);
            LegsMiddleLeftStabilizerColor = new ColorSet(br);
            LegsMiddleRightBackStabilizerColor = new ColorSet(br);
            LegsMiddleLeftBackStabilizerColor = new ColorSet(br);
            LegsLowerRightStabilizerColor = new ColorSet(br);
            LegsLowerLeftStabilizerColor = new ColorSet(br);
            LegsLowerRightBackStabilizerColor = new ColorSet(br);
            LegsLowerLeftBackStabilizerColor = new ColorSet(br);
            AllPattern = br.ReadEnum8<ColorPattern>();
            AllFramesPattern = br.ReadEnum8<ColorPattern>();
            AllUnitsPattern = br.ReadEnum8<ColorPattern>();
            AllStabilizersPattern = br.ReadEnum8<ColorPattern>();
            HeadPattern = br.ReadEnum8<ColorPattern>();
            CorePattern = br.ReadEnum8<ColorPattern>();
            ArmRightPattern = br.ReadEnum8<ColorPattern>();
            ArmLeftPattern = br.ReadEnum8<ColorPattern>();
            LegsPattern = br.ReadEnum8<ColorPattern>();
            ArmUnitRightPattern = br.ReadEnum8<ColorPattern>();
            ArmUnitLeftPattern = br.ReadEnum8<ColorPattern>();
            BackUnitRightPattern = br.ReadEnum8<ColorPattern>();
            BackUnitLeftPattern = br.ReadEnum8<ColorPattern>();
            ShoulderUnitPattern = br.ReadEnum8<ColorPattern>();
            HangerUnitRightPattern = br.ReadEnum8<ColorPattern>();
            HangerUnitLeftPattern = br.ReadEnum8<ColorPattern>();
            AllHeadStabilizersPattern = br.ReadEnum8<ColorPattern>();
            AllCoreStabilizersPattern = br.ReadEnum8<ColorPattern>();
            AllArmStabilizersPattern = br.ReadEnum8<ColorPattern>();
            AllLegStabilizersPattern = br.ReadEnum8<ColorPattern>();
            HeadTopStabilizerPattern = br.ReadEnum8<ColorPattern>();
            HeadRightStabilizerPattern = br.ReadEnum8<ColorPattern>();
            HeadLeftStabilizerPattern = br.ReadEnum8<ColorPattern>();
            CoreUpperRightStabilizerPattern = br.ReadEnum8<ColorPattern>();
            CoreUpperLeftStabilizerPattern = br.ReadEnum8<ColorPattern>();
            CoreLowerRightStabilizerPattern = br.ReadEnum8<ColorPattern>();
            CoreLowerLeftStabilizerPattern = br.ReadEnum8<ColorPattern>();
            ArmRightStabilizerPattern = br.ReadEnum8<ColorPattern>();
            ArmLeftStabilizerPattern = br.ReadEnum8<ColorPattern>();
            LegsBackStabilizerPattern = br.ReadEnum8<ColorPattern>();
            LegsUpperRightStabilizerPattern = br.ReadEnum8<ColorPattern>();
            LegsUpperLeftStabilizerPattern = br.ReadEnum8<ColorPattern>();
            LegsMiddleRightStabilizerPattern = br.ReadEnum8<ColorPattern>();
            LegsMiddleLeftStabilizerPattern = br.ReadEnum8<ColorPattern>();
            LegsLowerRightStabilizerPattern = br.ReadEnum8<ColorPattern>();
            LegsLowerLeftStabilizerPattern = br.ReadEnum8<ColorPattern>();
            EyeColor = br.ReadRGBA();
        }

        #endregion

        #region Write

        /// <summary>
        /// Serializes file data to a stream.
        /// </summary>
        protected override void Write(BinaryWriterEx bw)
        {
            HeadColor.Write(bw);
            CoreColor.Write(bw);
            ArmRightColor.Write(bw);
            ArmLeftColor.Write(bw);
            LegsColor.Write(bw);
            ArmUnitRightColor.Write(bw);
            ArmUnitLeftColor.Write(bw);
            BackUnitRightColor.Write(bw);
            BackUnitLeftColor.Write(bw);
            ShoulderUnitColor.Write(bw);
            HangerUnitRightColor.Write(bw);
            HangerUnitLeftColor.Write(bw);
            HeadTopStabilizerColor.Write(bw);
            HeadRightStabilizerColor.Write(bw);
            HeadLeftStabilizerColor.Write(bw);
            CoreUpperRightStabilizerColor.Write(bw);
            CoreUpperLeftStabilizerColor.Write(bw);
            CoreLowerRightStabilizerColor.Write(bw);
            CoreLowerLeftStabilizerColor.Write(bw);
            ArmRightStabilizerColor.Write(bw);
            ArmLeftStabilizerColor.Write(bw);
            LegsBackStabilizerColor.Write(bw);
            LegsUpperRightStabilizerColor.Write(bw);
            LegsUpperLeftStabilizerColor.Write(bw);
            LegsUpperRightBackStabilizerColor.Write(bw);
            LegsUpperLeftBackStabilizerColor.Write(bw);
            LegsMiddleRightStabilizerColor.Write(bw);
            LegsMiddleLeftStabilizerColor.Write(bw);
            LegsMiddleRightBackStabilizerColor.Write(bw);
            LegsMiddleLeftBackStabilizerColor.Write(bw);
            LegsLowerRightStabilizerColor.Write(bw);
            LegsLowerLeftStabilizerColor.Write(bw);
            LegsLowerRightBackStabilizerColor.Write(bw);
            LegsLowerLeftBackStabilizerColor.Write(bw);
            bw.WriteByte((byte)AllPattern);
            bw.WriteByte((byte)AllFramesPattern);
            bw.WriteByte((byte)AllUnitsPattern);
            bw.WriteByte((byte)AllStabilizersPattern);
            bw.WriteByte((byte)HeadPattern);
            bw.WriteByte((byte)CorePattern);
            bw.WriteByte((byte)ArmRightPattern);
            bw.WriteByte((byte)ArmLeftPattern);
            bw.WriteByte((byte)LegsPattern);
            bw.WriteByte((byte)ArmUnitRightPattern);
            bw.WriteByte((byte)ArmUnitLeftPattern);
            bw.WriteByte((byte)BackUnitRightPattern);
            bw.WriteByte((byte)BackUnitLeftPattern);
            bw.WriteByte((byte)ShoulderUnitPattern);
            bw.WriteByte((byte)HangerUnitRightPattern);
            bw.WriteByte((byte)HangerUnitLeftPattern);
            bw.WriteByte((byte)AllHeadStabilizersPattern);
            bw.WriteByte((byte)AllCoreStabilizersPattern);
            bw.WriteByte((byte)AllArmStabilizersPattern);
            bw.WriteByte((byte)AllLegStabilizersPattern);
            bw.WriteByte((byte)HeadTopStabilizerPattern);
            bw.WriteByte((byte)HeadRightStabilizerPattern);
            bw.WriteByte((byte)HeadLeftStabilizerPattern);
            bw.WriteByte((byte)CoreUpperRightStabilizerPattern);
            bw.WriteByte((byte)CoreUpperLeftStabilizerPattern);
            bw.WriteByte((byte)CoreLowerRightStabilizerPattern);
            bw.WriteByte((byte)CoreLowerLeftStabilizerPattern);
            bw.WriteByte((byte)ArmRightStabilizerPattern);
            bw.WriteByte((byte)ArmLeftStabilizerPattern);
            bw.WriteByte((byte)LegsBackStabilizerPattern);
            bw.WriteByte((byte)LegsUpperRightStabilizerPattern);
            bw.WriteByte((byte)LegsUpperLeftStabilizerPattern);
            bw.WriteByte((byte)LegsMiddleRightStabilizerPattern);
            bw.WriteByte((byte)LegsMiddleLeftStabilizerPattern);
            bw.WriteByte((byte)LegsLowerRightStabilizerPattern);
            bw.WriteByte((byte)LegsLowerLeftStabilizerPattern);
            bw.WriteRGBA(EyeColor);
        }

        #endregion

        #region ColorPattern Enum

        /// <summary>
        /// Twelve color pattern options that color categories can choose from.
        /// </summary>
        public enum ColorPattern : byte
        {
            /// <summary>
            /// The solid color pattern, on by default.
            /// </summary>
            Solid = 0,

            /// <summary>
            /// The second color pattern option.
            /// </summary>
            Pattern2 = 1,

            /// <summary>
            /// The third color pattern option.
            /// </summary>
            Pattern3 = 2,

            /// <summary>
            /// The fourth color pattern option.
            /// </summary>
            Pattern4 = 3,

            /// <summary>
            /// The fifth color pattern option.
            /// </summary>
            Pattern5 = 4,

            /// <summary>
            /// The sixth color pattern option.
            /// </summary>
            Pattern6 = 5,

            /// <summary>
            /// The seventh color pattern option.
            /// </summary>
            Pattern7 = 6,

            /// <summary>
            /// The eighth color pattern option.
            /// </summary>
            Pattern8 = 7,

            /// <summary>
            /// The ninth color pattern option.
            /// </summary>
            Pattern9 = 8,

            /// <summary>
            /// The tenth color pattern option.
            /// </summary>
            Pattern10 = 9,

            /// <summary>
            /// The eleventh color pattern option.
            /// </summary>
            Pattern11 = 10,

            /// <summary>
            /// The twelfth color pattern option.
            /// </summary>
            Pattern12 = 11
        };

        #endregion

        #region ColorSet

        /// <summary>
        /// A color set containing six colors.
        /// </summary>
        public struct ColorSet
        {
            /// <summary>
            /// The main color.
            /// </summary>
            public Color Main { get; set; }

            /// <summary>
            /// The sub color.
            /// </summary>
            public Color Sub { get; set; }

            /// <summary>
            /// The support color.
            /// </summary>
            public Color Support { get; set; }

            /// <summary>
            /// The optional color.
            /// </summary>
            public Color Optional { get; set; }

            /// <summary>
            /// The joint color.
            /// </summary>
            public Color Joint { get; set; }

            /// <summary>
            /// The device color.
            /// </summary>
            public Color Device { get; set; }

            /// <summary>
            /// Create a new ColorSet using six RGBA order colors.
            /// </summary>
            /// <param name="main">The main color.</param>
            /// <param name="sub">The sub color.</param>
            /// <param name="support">The support color.</param>
            /// <param name="optional">The optional color.</param>
            /// <param name="joint">The joint color.</param>
            /// <param name="device">The device color.</param>
            public ColorSet(Color main, Color sub, Color support, Color optional, Color joint, Color device)
            {
                Main = main;
                Sub = sub;
                Support = support;
                Optional = optional;
                Joint = joint;
                Device = device;
            }

            /// <summary>
            /// Create a new copy of an existing colorset.
            /// </summary>
            /// <param name="colorset">A colorset.</param>
            public ColorSet(ColorSet colorset)
            {
                Main = colorset.Main;
                Sub = colorset.Sub;
                Support = colorset.Support;
                Optional = colorset.Optional;
                Joint = colorset.Joint;
                Device = colorset.Device;
            }

            /// <summary>
            /// Make a new color set with all values set the specified color.
            /// </summary>
            /// <param name="color">A color.</param>
            public ColorSet(Color color)
            {
                Main = color;
                Sub = color;
                Support = color;
                Optional = color;
                Joint = color;
                Device = color;
            }

            /// <summary>
            /// Read from a stream.
            /// </summary>
            /// <param name="br">The stream reader.</param>
            internal ColorSet(BinaryReaderEx br)
            {
                Main = br.ReadRGBA();
                Sub = br.ReadRGBA();
                Support = br.ReadRGBA();
                Optional = br.ReadRGBA();
                Joint = br.ReadRGBA();
                Device = br.ReadRGBA();
            }

            /// <summary>
            /// Write to a stream.
            /// </summary>
            /// <param name="bw">The stream writer.</param>
            internal void Write(BinaryWriterEx bw)
            {
                bw.WriteRGBA(Main);
                bw.WriteRGBA(Sub);
                bw.WriteRGBA(Support);
                bw.WriteRGBA(Optional);
                bw.WriteRGBA(Joint);
                bw.WriteRGBA(Device);
            }
        }

        #endregion
    }
}
