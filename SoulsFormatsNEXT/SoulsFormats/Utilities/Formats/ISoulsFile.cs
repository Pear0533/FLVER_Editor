namespace SoulsFormats
{
    /// <summary>
    /// A writeable file supporting transparent compression.
    /// </summary>
    public interface ISoulsFile
    {
        /// <summary>
        /// The compression to use if none is specified.
        /// </summary>
        DCX.CompressionInfo Compression { get; set; }

        /// <summary>
        /// Writes the file to an array of bytes using the stored compression type.
        /// </summary>
        byte[] Write();

        /// <summary>
        /// Writes the file to an array of bytes using the given compression type.
        /// </summary>
        byte[] Write(DCX.CompressionInfo compression);

        /// <summary>
        /// Writes the file to disk using the stored compression type.
        /// </summary>
        void Write(string path);

        /// <summary>
        /// Writes the file to disk using the given compression type.
        /// </summary>
        void Write(string path, DCX.CompressionInfo compression);
    }
}
