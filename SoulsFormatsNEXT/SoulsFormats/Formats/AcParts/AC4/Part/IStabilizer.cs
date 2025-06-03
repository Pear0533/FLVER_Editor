namespace SoulsFormats
{
    public partial class AcParts4
    {
        /// <summary>
        /// An interface for stabilizer parts in acparts.
        /// </summary>
        public interface IStabilizer : IPart
        {
            /// <summary>
            /// A Component which contains Stabilizer stats.
            /// </summary>
            StabilizerComponent StabilizerComponent { get; set; }
        }
    }
}