namespace SoulsFormats
{
    internal interface IBND2
    {
        /// <summary>
        /// Header info flags?
        /// </summary>
        BND2.HeaderInfoFlagsEnum HeaderInfoFlags { get; set; }

        /// <summary>
        /// File info flags.
        /// </summary>
        BND2.FileInfoFlagsEnum FileInfoFlags { get; set; }

        /// <summary>
        /// Unknown.
        /// </summary>
        byte Unk06 { get; set; }

        /// <summary>
        /// Endian?
        /// </summary>
        byte Unk07 { get; set; }

        /// <summary>
        /// The version of this <see cref="BND2"/>.
        /// <para>Only 202 and 211 have been seen.</para>
        /// </summary>
        int FileVersion { get; set; }

        /// <summary>
        /// The alignment of each <see cref="BND2.File"/>.
        /// <para>The bigger the aligment, the more empty bytes are added as padding. This increases the size of the archive.</para>
        /// </summary>
        ushort AlignmentSize { get; set; }

        /// <summary>
        /// The file path mode determining how paths are handled.
        /// </summary>
        BND2.FilePathModeEnum FilePathMode { get; set; }

        /// <summary>
        /// Unknown; Was found set to 1 on files extracted from memory.
        /// Usually set to 0.
        /// </summary>
        byte Unk1B { get; set; }

        /// <summary>
        /// The base directory of all files.
        /// <para>Only used when <see cref="BND2.FilePathModeEnum.BaseDirectory"/> is set on <see cref="FilePathMode"/>.</para>
        /// </summary>
        string BaseDirectory { get; set; }
    }
}
