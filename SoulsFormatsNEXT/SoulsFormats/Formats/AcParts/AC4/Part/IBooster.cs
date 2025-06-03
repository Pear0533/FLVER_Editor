namespace SoulsFormats
{
    public partial class AcParts4
    {
        /// <summary>
        /// An interface for booster parts in ACPARTS.
        /// </summary>
        public interface IBooster : IPart
        {
            /// <summary>
            /// Horizontal Booster stats.
            /// </summary>
            BoosterComponent HorizontalBoost { get; set; }
        }
    }
}