namespace SoulsFormats
{
    public partial class AcParts4
    {
        /// <summary>
        /// An interface for parts with weapon stats in ACPARTS.
        /// </summary>
        public interface IWeapon : IPart
        {
            /// <summary>
            /// A Component which contains stats for weapons.
            /// </summary>
            WeaponComponent WeaponComponent { get; set; }
        }
    }
}