namespace SoulsFormats
{
    public partial class AcParts4
    {
        /// <summary>
        /// A Component which contains frame part stats.
        /// </summary>
        public class FrameComponent
        {
            /// <summary>
            /// The value the tune slider will set the mentioned stat to at the end of the slider, 50 FRS points in.
            /// </summary>
            public ushort TuneMaxRectification { get; set; }

            /// <summary>
            /// Whether or not putting FRS points into tuning the stat will change it or not.
            /// </summary>
            public ushort TuneEfficiencyRectification { get; set; }

            /// <summary>
            /// Durability. The larger this value, the more durable the part.
            /// </summary>
            public ushort AP { get; set; }

            /// <summary>
            /// Unknown; Is always 0.
            /// </summary>
            public ushort Unk06 { get; set; }

            /// <summary>
            /// How much drag the frame part has, slowing it down when moving forward.
            /// </summary>
            public float DragCoefficient { get; set; }

            /// <summary>
            /// The front weight balancing of the frame part.
            /// </summary>
            public ushort WeightBalanceFront { get; set; }

            /// <summary>
            /// The back weight balancing of the frame part.
            /// </summary>
            public ushort WeightBalanceBack { get; set; }

            /// <summary>
            /// The right side weight balancing of the frame part.
            /// </summary>
            public ushort WeightBalanceRight { get; set; }

            /// <summary>
            /// The left side weight balancing of the frame part.
            /// </summary>
            public ushort WeightBalanceLeft { get; set; }

            /// <summary>
            /// Makes a new <see cref="FrameComponent"/>.
            /// </summary>
            public FrameComponent()
            {

            }

            /// <summary>
            /// Reads a Frame component from a stream.
            /// </summary>
            /// <param name="br">A binary reader.</param>
            internal FrameComponent(BinaryReaderEx br)
            {
                TuneMaxRectification = br.ReadUInt16();
                TuneEfficiencyRectification = br.ReadUInt16();
                AP = br.ReadUInt16();
                Unk06 = br.ReadUInt16();
                DragCoefficient = br.ReadSingle();
                WeightBalanceFront = br.ReadUInt16();
                WeightBalanceBack = br.ReadUInt16();
                WeightBalanceRight = br.ReadUInt16();
                WeightBalanceLeft = br.ReadUInt16();
            }

            /// <summary>
            /// Writes a Frame component to a stream.
            /// </summary>
            /// <param name="bw">A binary writer.</param>
            public void Write(BinaryWriterEx bw)
            {
                bw.WriteUInt16(TuneMaxRectification);
                bw.WriteUInt16(TuneEfficiencyRectification);
                bw.WriteUInt16(AP);
                bw.WriteUInt16(Unk06);
                bw.WriteSingle(DragCoefficient);
                bw.WriteUInt16(WeightBalanceFront);
                bw.WriteUInt16(WeightBalanceBack);
                bw.WriteUInt16(WeightBalanceRight);
                bw.WriteUInt16(WeightBalanceLeft);
            }
        }
    }
}
