namespace SoulsFormats
{
    /// <summary>
    /// An interface for resources in an MLB.
    /// </summary>
    public interface IMlbResource
    {
        /// <summary>
        /// The full path to the resource.
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// The relative path to the resource.
        /// </summary>
        string RelativePath { get; set; }
    }
}
