using System.Collections.Generic;

namespace SoulsFormats
{
    /// <summary>
    /// A 3d resource list of some kind that also contains metadata.
    /// </summary>
    public interface IMLB
    {
        /// <summary>
        /// The resources referenced by this MLB.
        /// </summary>
        List<IMlbResource> Resources { get; }
    }
}
