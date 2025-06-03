namespace SoulsFormats
{
    public partial class AcParts4
    {
        /// <summary>
        /// An interface for frame parts in ACPARTS.
        /// </summary>
        public interface IFrame : IPart
        {
            /// <summary>
            /// A Component which contains Defense Stats.
            /// </summary>
            DefenseComponent DefenseComponent { get; set; }

            /// <summary>
            /// A Component which contains Primal Armor stats.
            /// </summary>
            PAComponent PAComponent { get; set; }

            /// <summary>
            /// A Component which contains frame part stats.
            /// </summary>
            FrameComponent FrameComponent { get; set; }
        }
    }
}