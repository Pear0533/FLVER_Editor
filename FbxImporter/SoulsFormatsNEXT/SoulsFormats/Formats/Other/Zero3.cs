using SoulsFormats.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace SoulsFormats.Other
{
    /// <summary>
    /// A multi-file container format used in Armored Core 4 and Murakumo: Renegade Mech Pursuit.
    /// </summary>
    public class Zero3
    {
        #region Constants

        /// <summary>
        /// The default <see cref="SizeAlign"/>.
        /// </summary>
        public const int DefaultSizeAlign = 0x10;

        /// <summary>
        /// The default <see cref="OffsetAlign"/>.
        /// </summary>
        public const int DefaultOffsetAlign = 0x10;

        /// <summary>
        /// The default <see cref="MaxFileSize"/> for Armored Core 4.
        /// </summary>
        public const int DefaultMaxFileSizeAC4 = 0x800000;

        /// <summary>
        /// The default <see cref="MaxFileSize"/> for Murakumo: Renegade Mech Pursuit.
        /// </summary>
        public const int DefaultMaxFileSizeMurakumo = 0x300000;

        /// <summary>
        /// The default <see cref="HeaderDataAlign"/> for Armored Core 4.
        /// </summary>
        public const int DefaultHeaderDataAlignAC4 = 0x1;

        /// <summary>
        /// The default <see cref="HeaderDataAlign"/> for Murakumo: Renegade Mech Pursuit.
        /// </summary>
        public const int DefaultHeaderDataAlignMurakumo = 0x10000;

        /// <summary>
        /// The length of names for Armored Core 4.
        /// </summary>
        private const int NameLengthAC4 = 64;

        /// <summary>
        /// The length of names for Murakumo: Renegade Mech Pursuit.
        /// </summary>
        private const int NameLengthMurakumo = 48;

        /// <summary>
        /// The default wrapper version to use.
        /// </summary>
        public const string DefaultWrapperVersion = "07B17Q00";

        /// <summary>
        /// The default wrapped name to use.
        /// </summary>
        private const string DefaultWrappedName = "default.000";

        #endregion

        #region Properties

        /// <summary>
        /// The files in this <see cref="Zero3"/>.
        /// </summary>
        public List<File> Files { get; set; }

        /// <summary>
        /// The amount to align offsets by.
        /// </summary>
        public int OffsetAlign { get; set; }

        /// <summary>
        /// The amount to align sizes by.
        /// </summary>
        public int SizeAlign { get; set; }

        /// <summary>
        /// The maximum size each container piece can be.
        /// </summary>
        public int MaxFileSize { get; set; }

        /// <summary>
        /// The format version of the <see cref="Zero3"/>.
        /// </summary>
        public FormatVersion Version { get; set; }

        /// <summary>
        /// Whether or not to store data in the first container.
        /// </summary>
        public bool StoreInHeader { get; set; }

        /// <summary>
        /// The alignment of data in the first container if storing data there.
        /// </summary>
        public int HeaderDataAlign { get; set; }

        /// <summary>
        /// Whether or not the container parts should be wrapped.
        /// </summary>
        public bool IsWrapped { get; set; }

        /// <summary>
        /// The version to set on the containers wrapping the <see cref="Zero3"/> containers.
        /// </summary>
        public string WrapperVersion { get; set; }

        /// <summary>
        /// The name to use for wrappers when writing to bytes.
        /// </summary>
        public string WrappedName { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new <see cref="Zero3"/> formatted for Armored Core 4.
        /// </summary>
        public Zero3()
        {
            Files = new List<File>();
            OffsetAlign = DefaultOffsetAlign;
            SizeAlign = DefaultSizeAlign;
            MaxFileSize = DefaultMaxFileSizeAC4;
            Version = FormatVersion.ArmoredCore4;
            StoreInHeader = true;
            HeaderDataAlign = DefaultHeaderDataAlignAC4;
            IsWrapped = true;
            WrapperVersion = DefaultWrapperVersion;
            WrappedName = DefaultWrappedName;
        }

        /// <summary>
        /// Read a <see cref="Zero3"/> from an <see cref="Array"/> of containers.
        /// </summary>
        /// <param name="containers">The container readers.</param>
        /// <param name="version">The container version.</param>
        /// <param name="header">The container header.</param>
        /// <param name="wrapped">Whether or not the containers were wrapped.</param>
        /// <param name="wrapperVersion">The version of the containers wrapping these containers.</param>
        /// <param name="wrappedName">The name to use for wrappers when writing to bytes.</param>
        /// <exception cref="NotSupportedException">The version was unknown.</exception>
        private Zero3(BinaryReaderEx[] containers, FormatVersion version, ContainerHeader header, bool wrapped, string wrapperVersion, string wrappedName)
        {
            Version = version;
            OffsetAlign = header.OffsetAlign;
            SizeAlign = header.SizeAlign;
            MaxFileSize = header.MaxFileSize;
            IsWrapped = wrapped;
            WrapperVersion = wrapperVersion;
            WrappedName = wrappedName;

            int nameLength;
            switch (version)
            {
                case FormatVersion.Murakumo:
                    HeaderDataAlign = DefaultHeaderDataAlignMurakumo;
                    nameLength = NameLengthMurakumo;
                    break;
                case FormatVersion.ArmoredCore4:
                    HeaderDataAlign = DefaultHeaderDataAlignAC4;
                    nameLength = NameLengthAC4;
                    break;
                default:
                    throw new NotSupportedException($"Unknown {nameof(FormatVersion)}: {version}");
            }

            var br = containers[0];
            int fileCount = header.FileCount;
            Files = new List<File>(fileCount);
            for (int i = 0; i < fileCount; i++)
            {
                string fileName = br.ReadFixStr(nameLength);
                int containerIndex = br.ReadInt32();
                int fileOffset = br.ReadInt32();
                int paddedFileSize = br.ReadInt32();
                int fileSize = br.ReadInt32();

                if (containerIndex < 0)
                {
                    throw new IndexOutOfRangeException($"Container index must be non-negative: {containerIndex}");
                }

                if (containerIndex >= containers.Length)
                {
                    throw new IndexOutOfRangeException($"Container index out of expected range: {containerIndex}");
                }

                if (containerIndex == 0)
                {
                    StoreInHeader = true;
                }

                var cbr = containers[containerIndex];
                var pos = cbr.Position;
                cbr.Position = fileOffset * OffsetAlign;
                byte[] bytes = cbr.ReadBytes(fileSize);
                Files.Add(new File(fileName, bytes));
                cbr.Position = pos;
            }
        }

        #endregion

        #region Read

        /// <summary>
        /// Read a <see cref="Zero3"/> container starting from it's first piece from the specified path.
        /// </summary>
        /// <param name="path">The path to read the first container from.</param>
        /// <returns>A <see cref="Zero3"/>.</returns>
        /// <exception cref="InvalidDataException">The file path had an unexpected extension or the wrapped file name wasn't the same as the wrapper file name.</exception>
        public static Zero3 Read(string path)
        {
            if (!path.EndsWith(".000"))
            {
                throw new InvalidDataException($"Unexpected file path extension: {path}");
            }

            var br = GetUnwrapped(new BinaryReaderEx(true, path), out bool wrapped, out string wrapperVersion, out string wrappedName);
            string fileName = Path.GetFileName(wrappedName);
            if (!fileName.StartsWith(wrappedName))
            {
                throw new InvalidDataException($"Wrapped file name is not the same as wrapper file name: {wrappedName}");
            }

            var version = GetFormatVersion(br);
            var header = new ContainerHeader(br, version, wrapped);
            int maxContainerIndex = GetMaxContainerIndex(br, header.FileCount, version);
            var containers = GatherContainers(br, maxContainerIndex, path, wrapped);

            var archive = new Zero3(containers, version, header, wrapped, wrapperVersion, wrappedName);
            foreach (var container in containers)
            {
                container.Dispose();
            }

            return archive;
        }

        /// <summary>
        /// Read a <see cref="Zero3"/> from an ordered enumerable of containers.
        /// </summary>
        /// <param name="containersBytes">The ordered enumerable of containers.</param>
        /// <returns>A <see cref="Zero3"/>.</returns>
        public static Zero3 Read(IEnumerable<byte[]> containersBytes)
        {
            var enumerator = containersBytes.GetEnumerator();
            var firstContainerBytes = enumerator.Current;
            enumerator.MoveNext();

            var br = GetUnwrapped(new BinaryReaderEx(true, firstContainerBytes), out bool wrapped, out string wrapperVersion, out string wrappedName);
            var version = GetFormatVersion(br);
            var header = new ContainerHeader(br, version, wrapped);
            int maxContainerIndex = GetMaxContainerIndex(br, header.FileCount, version);
            var containers = GatherContainers(br, maxContainerIndex, containersBytes, wrapped);  

            var archive = new Zero3(containers, version, header, wrapped, wrapperVersion, wrappedName);
            foreach (var container in containers)
            {
                container.Dispose();
            }

            return archive;
        }

        #endregion

        #region Write

        /// <summary>
        /// Write to the specified path.
        /// </summary>
        /// <param name="path">The path to write to.</param>
        /// <exception cref="InvalidDataException">The file extension was unexpected.</exception>
        public void Write(string path)
        {
            // Ensure path is valid
            if (!path.EndsWith(".000"))
            {
                throw new InvalidDataException($"Unexpected file path extension: {path}");
            }

            // Get write info
            GetWriteInfo(out ContainerHeader header, out ContainerHeader writeHeader, out bool bigEndian, out int remainingHeaderLength, out int nameLength);
            BinaryWriterEx bw;
            if (IsWrapped)
            {
                bw = new BinaryWriterEx(bigEndian);
            }
            else
            {
                bw = new BinaryWriterEx(bigEndian, path);
            }

            // Write headers
            WriteHeaders(bw, writeHeader, remainingHeaderLength, nameLength);

            // Write containers
            string containerPathBase = path.Substring(0, path.Length - 3);
            WriteContainers(bw, header, containerPathBase);

            // Finish up the header container
            if (IsWrapped)
            {
                Wrap(bw.FinishBytes(), path, WrapperVersion);
            }
            else
            {
                bw.Finish();
            }
        }

        /// <summary>
        /// Write to a list of containers.
        /// </summary>
        /// <param name="containers">A list of containers.</param>
        public void Write(List<byte[]> containers)
        {
            // Get write info
            GetWriteInfo(out ContainerHeader header, out ContainerHeader writeHeader, out bool bigEndian, out int remainingHeaderLength, out int nameLength);
            var bw = new BinaryWriterEx(bigEndian);

            // Write headers
            WriteHeaders(bw, writeHeader, remainingHeaderLength, nameLength);

            // Write containers
            WriteContainers(bw, header, containers);

            // Finish up the header container
            if (IsWrapped)
            {
                containers[0] = bw.FinishBytes();
            }
            else
            {
                containers[0] = bw.FinishBytes();
            }
        }

        #endregion

        #region Read Helpers

        /// <summary>
        /// Determine the format version of the container.
        /// </summary>
        /// <param name="br">The stream reader.</param>
        /// <returns>The format version.</returns>
        private static FormatVersion GetFormatVersion(BinaryReaderEx br)
        {
            var pos = br.Position;
            br.Position += 4;
            var offsetAlign = br.ReadInt32();
            var sizeAlign = br.ReadInt32();
            var maxFileSize = br.ReadInt32();
            br.Position = pos;

            return (offsetAlign != 0 && sizeAlign != 0 && maxFileSize != 0) ? FormatVersion.ArmoredCore4 : FormatVersion.Murakumo;
        }

        /// <summary>
        /// Get the maximum container index.
        /// </summary>
        /// <param name="br">The stream reader.</param>
        /// <param name="fileCount">The file count.</param>
        /// <param name="version">The container version.</param>
        /// <returns>The max container index.</returns>
        /// <exception cref="NotSupportedException">The version was unknown.</exception>
        private static int GetMaxContainerIndex(BinaryReaderEx br, int fileCount, FormatVersion version)
        {
            if (fileCount < 1)
            {
                return 0;
            }

            int containerIndexOffset;
            int fileHeaderSize;
            switch (version)
            {
                case FormatVersion.Murakumo:
                    containerIndexOffset = 48;
                    fileHeaderSize = 64;
                    break;
                case FormatVersion.ArmoredCore4:
                    containerIndexOffset = 64;
                    fileHeaderSize = 80;
                    break;
                default:
                    throw new NotSupportedException($"Unknown {nameof(FormatVersion)}: {version}");
            }

            var pos = br.Position;
            int lastFile = fileCount - 1;
            int lastFileOffset = lastFile * fileHeaderSize;
            br.Position += (lastFileOffset + containerIndexOffset);
            int containerIndex = br.ReadInt32();
            br.Position = pos;

            return containerIndex;
        }

        /// <summary>
        /// Gather the containers of the <see cref="Zero3"/>.
        /// </summary>
        /// <param name="br">The first container.</param>
        /// <param name="maxContainerIndex">The max container index.</param>
        /// <param name="path">The path to the first container.</param>
        /// <param name="wrapped">Whether or not the containers are wrapped.</param>
        /// <returns>An array of containers.</returns>
        /// <exception cref="InvalidDataException">Could not find a container.</exception>
        private static BinaryReaderEx[] GatherContainers(BinaryReaderEx br, int maxContainerIndex, string path, bool wrapped)
        {
            int containerCount = maxContainerIndex + 1;
            var containers = new BinaryReaderEx[containerCount];
            containers[0] = br;

            string containerPathBase = path.Substring(0, path.Length - 3);
            for (int i = 1; i < containerCount; i++)
            {
                string containerPath = $"{containerPathBase}{i:D3}";
                if (!System.IO.File.Exists(containerPath))
                {
                    throw new InvalidDataException($"Could not find container part file: {i:D3}");
                }

                BinaryReaderEx cbr;
                if (wrapped)
                {
                    var bnd = BND3.Read(containerPath);
                    // Don't check wrapper versions, as some can be different even for the same set of Zero3 pieces
                    cbr = Unwrap(bnd, i);
                }
                else
                {
                    cbr = new BinaryReaderEx(br.BigEndian, containerPath);
                }

                containers[i] = cbr;
            }

            return containers;
        }

        /// <summary>
        /// Gather the containers of the <see cref="Zero3"/>.
        /// </summary>
        /// <param name="br">The first container.</param>
        /// <param name="maxContainerIndex">The max container index.</param>
        /// <param name="containersBytes">The container bytes.</param>
        /// <param name="wrapped">Whether or not the containers are wrapped.</param>
        /// <returns>An array of containers.</returns>
        /// <exception cref="InvalidDataException">Could not find a container.</exception>
        private static BinaryReaderEx[] GatherContainers(BinaryReaderEx br, int maxContainerIndex, IEnumerable<byte[]> containersBytes, bool wrapped)
        {
            int containerCount = maxContainerIndex + 1;
            var containers = new BinaryReaderEx[containerCount];
            containers[0] = br;
            int index = 1;
            foreach (var containerBytes in containersBytes)
            {
                if (index > maxContainerIndex)
                {
                    throw new ArgumentException(nameof(containersBytes), $"{nameof(containersBytes)} does not have the specified number of container parts: {maxContainerIndex + 1}");
                }

                if (wrapped)
                {
                    var bnd = BND3.Read(containerBytes);
                    // Don't check wrapper versions, as some can be different even for the same set of Zero3 pieces
                    containers[index] = Unwrap(bnd, index);
                }
                else
                {
                    containers[index] = new BinaryReaderEx(br.BigEndian, containerBytes);
                }

                index++;
            }

            if (index != maxContainerIndex)
            {
                throw new ArgumentException(nameof(containersBytes), $"{nameof(containersBytes)} does not have the specified number of container parts: {maxContainerIndex + 1}");
            }

            return containers;
        }

        /// <summary>
        /// Unwrap the stream reader if necessary.
        /// </summary>
        /// <param name="br">The stream reader.</param>
        /// <param name="wrapped">Whether or not it was wrapped.</param>
        /// <param name="wrappingVersion">The version of the container wrapping this one.</param>
        /// <param name="wrappedName">The wrapped name.</param>
        /// <returns>The unwrapped stream reader.</returns>
        private static BinaryReaderEx GetUnwrapped(BinaryReaderEx br, out bool wrapped, out string wrappingVersion, out string wrappedName)
        {
            var pos = br.Position;
            string magic = br.ReadASCII(4);
            br.Position = pos;

            if (magic == "BND3")
            {
                wrapped = true;
                var bnd = BND3.Read(br.Stream);
                wrappingVersion = bnd.Version;
                if (bnd.Files.Count != 1)
                {
                    throw new InvalidDataException($"Unexpected number of files in wrapped {nameof(Zero3)}: {bnd.Files.Count}");
                }

                string bname = bnd.Files[0].Name;
                wrappedName = bname.Substring(0, bname.Length - 4);
                return Unwrap(bnd, 0);
            }
            else
            {
                wrapped = false;
                wrappingVersion = string.Empty;
                wrappedName = string.Empty;
                return br;
            }
        }

        /// <summary>
        /// Unwrap a container.
        /// </summary>
        /// <param name="bnd">The wrapped container.</param>
        /// <param name="index">The index of the container to unwrap.</param>
        /// <returns>The unwrapped stream reader.</returns>
        /// <exception cref="InvalidDataException">Unexpected file conditions were encountered.</exception>
        private static BinaryReaderEx Unwrap(BND3 bnd, int index)
        {
            if (bnd.Files.Count != 1)
            {
                throw new InvalidDataException($"Unexpected number of files in wrapped {nameof(Zero3)}: {bnd.Files.Count}");
            }

            var file = bnd.Files[0];
            if (!file.Name.EndsWith($".{index:D3}"))
            {
                throw new InvalidDataException($"Unexpected wrapped {nameof(Zero3)} file name: {file.Name}");
            }

            return new BinaryReaderEx(true, file.Bytes);
        }

        #endregion

        #region Write Helpers

        /// <summary>
        /// Get the info needed for writing.
        /// </summary>
        /// <param name="header">The header to determine how to write.</param>
        /// <param name="writeHeader">The header to write.</param>
        /// <param name="bigEndian">Whether or not big endianness is to be used.</param>
        /// <param name="remainingHeaderLength">The remaining header length after writing the container header.</param>
        /// <param name="nameLength">The length of names in file entries.</param>
        /// <exception cref="NotSupportedException">The version was unknown.</exception>
        private void GetWriteInfo(out ContainerHeader header, out ContainerHeader writeHeader, out bool bigEndian, out int remainingHeaderLength, out int nameLength)
        {
            switch (Version)
            {
                case FormatVersion.Murakumo:
                    bigEndian = false;
                    header = new ContainerHeader(Files.Count, DefaultOffsetAlign, DefaultSizeAlign, DefaultMaxFileSizeMurakumo);
                    writeHeader = new ContainerHeader(Files.Count, 0, 0, 0);
                    remainingHeaderLength = 48;
                    nameLength = NameLengthMurakumo;
                    break;
                case FormatVersion.ArmoredCore4:
                    bigEndian = true;
                    header = new ContainerHeader(Files.Count, OffsetAlign, SizeAlign, MaxFileSize);
                    writeHeader = header;
                    remainingHeaderLength = 64;
                    nameLength = NameLengthAC4;
                    break;
                default:
                    throw new NotSupportedException($"Unknown {nameof(FormatVersion)}: {Version}");
            }
        }

        /// <summary>
        /// Write the headers of the container files and containers.
        /// </summary>
        /// <param name="bw">The header container writer.</param>
        /// <param name="writeHeader">The header to write for containers.</param>
        /// <param name="remainingHeaderLength">The remaining header length after writing the container header.</param>
        /// <param name="nameLength">The length of names in file entries.</param>
        /// <exception cref="InvalidDataException">A file name was too long.</exception>
        private void WriteHeaders(BinaryWriterEx bw, ContainerHeader writeHeader, int remainingHeaderLength, int nameLength)
        {
            // Write container header
            bw.WriteInt32(writeHeader.FileCount);
            bw.WriteInt32(writeHeader.OffsetAlign);
            bw.WriteInt32(writeHeader.SizeAlign);
            bw.WriteInt32(writeHeader.MaxFileSize);
            bw.WritePattern(remainingHeaderLength, 0);

            // Write file headers
            for (int i = 0; i < Files.Count; i++)
            {
                var file = Files[i];
                if (file.Name.Length > nameLength)
                {
                    throw new InvalidDataException($"File name too long: {file.Name}");
                }

                bw.WriteFixStr(file.Name, nameLength);
                bw.ReserveInt32($"ContainerIndex{i}");
                bw.ReserveInt32($"FileOffset{i}");
                bw.ReserveInt32($"PaddedFileSize{i}");
                bw.WriteInt32(file.Bytes.Length);
            }
        }

        /// <summary>
        /// Write the containers for a <see cref="Zero3"/>.
        /// </summary>
        /// <param name="bw">The header container writer.</param>
        /// <param name="header">The header that determines how to write.</param>
        /// <param name="pathBase">The base for all container paths.</param>
        /// <exception cref="InvalidDataException">Padded file size was greater than an entire container can hold.</exception>
        private void WriteContainers(BinaryWriterEx bw, ContainerHeader header, string pathBase)
        {
            // Local variables and functions
            int containerIndex;
            string path;
            BinaryWriterEx cbw;
            void CreateContainer()
            {
                if (IsWrapped)
                {
                    cbw = new BinaryWriterEx(false);
                }
                else
                {
                    cbw = new BinaryWriterEx(false, path);
                }
            }

            void SaveContainer()
            {
                if (IsWrapped)
                {
                    Wrap(cbw.FinishBytes(), path, WrapperVersion);
                }
                else
                {
                    cbw.Finish();
                }
            }

            // Determine if we need to start writing data in the header container
            if (StoreInHeader)
            {
                containerIndex = 0;
                path = $"{pathBase}{containerIndex:D3}";
                cbw = bw;
                cbw.Pad(HeaderDataAlign);
            }
            else
            {
                containerIndex = 1;
                path = $"{pathBase}{containerIndex:D3}";
                CreateContainer();
            }

            // Write files and containers
            for (int i = 0; i < Files.Count; i++)
            {
                // Ensure file size is supported
                var file = Files[i];
                var paddedFileSize = MathHelper.Align(file.Bytes.Length, header.SizeAlign);
                if (paddedFileSize > header.MaxFileSize)
                {
                    throw new InvalidDataException($"Padded file size is greater than max container size: {header.MaxFileSize}");
                }

                // Check if we need to advance the container
                var containerNextSize = MathHelper.Align(cbw.Length, header.OffsetAlign) + paddedFileSize;
                if (containerNextSize > header.MaxFileSize)
                {
                    // We still need to write to the header file
                    // So make sure we aren't disposing it just yet
                    if (containerIndex != 0)
                    {
                        SaveContainer();
                    }

                    // Advance to the next container
                    containerIndex++;
                    path = $"{pathBase}{containerIndex:D3}";
                    CreateContainer();
                }

                // Write data
                WriteData(bw, cbw, file, i, containerIndex, header.OffsetAlign, header.SizeAlign);
            }

            // Save the last container
            // We still need to write to the header file
            // So make sure we aren't disposing it just yet
            if (containerIndex != 0)
            {
                SaveContainer();
            }
        }

        /// <summary>
        /// Write the containers for a <see cref="Zero3"/>.
        /// </summary>
        /// <param name="bw">The header container writer.</param>
        /// <param name="header">The header that determines how to write.</param>
        /// <param name="containers">A list of containers to add to.</param>
        /// <exception cref="InvalidDataException">Padded file size was greater than an entire container can hold.</exception>
        private void WriteContainers(BinaryWriterEx bw, ContainerHeader header, List<byte[]> containers)
        {
            // Local variables and functions
            int containerIndex;
            BinaryWriterEx cbw;

            byte[] SaveContainer()
            {
                if (IsWrapped)
                {
                    return Wrap(cbw.FinishBytes(), containerIndex, WrapperVersion);
                }
                else
                {
                    return cbw.FinishBytes();
                }
            }

            // Add dud for header container
            containers.Add(Array.Empty<byte>());

            // Determine if we need to start writing data in the header container
            if (StoreInHeader)
            {
                containerIndex = 0;
                cbw = bw;
                cbw.Pad(HeaderDataAlign);
            }
            else
            {
                containerIndex = 1;
                cbw = new BinaryWriterEx(false);
            }

            for (int i = 0; i < Files.Count; i++)
            {
                // Ensure file size is supported
                var file = Files[i];
                var paddedFileSize = MathHelper.Align(file.Bytes.Length, header.SizeAlign);
                if (paddedFileSize > header.MaxFileSize)
                {
                    throw new InvalidDataException($"Padded file size is greater than max container size: {header.MaxFileSize}");
                }

                // Check if we need to advance the container
                var containerNextSize = MathHelper.Align(cbw.Length, header.OffsetAlign) + paddedFileSize;
                if (containerNextSize > header.MaxFileSize)
                {
                    // We still need to write to the header file
                    // So make sure we aren't disposing it just yet
                    if (containerIndex != 0)
                    {
                        containers.Add(SaveContainer());
                    }

                    // Advance to the next container
                    containerIndex++;
                    cbw = new BinaryWriterEx(false);
                }

                // Write data
                WriteData(bw, cbw, file, i, containerIndex, header.OffsetAlign, header.SizeAlign);
            }

            // Save the last container
            // We still need to write to the header file
            // So make sure we aren't disposing it just yet
            if (containerIndex != 0)
            {
                containers.Add(SaveContainer());
            }
        }

        /// <summary>
        /// Write the data of a file.
        /// </summary>
        /// <param name="bw">The header container writer.</param>
        /// <param name="cbw">A writer for the container of the data.</param>
        /// <param name="file">The file to write.</param>
        /// <param name="index">The index of the file to write.</param>
        /// <param name="containerIndex">The container index.</param>
        /// <param name="offsetAlign">The offset alignment.</param>
        /// <param name="sizeAlign">The size alignment.</param>
        private void WriteData(BinaryWriterEx bw, BinaryWriterEx cbw, File file, int index, int containerIndex, int offsetAlign, int sizeAlign)
        {
            bw.FillInt32($"ContainerIndex{index}", containerIndex);

            long start = cbw.Position;
            cbw.Pad(offsetAlign);
            bw.FillInt32($"FileOffset{index}", (int)(start / offsetAlign));
            cbw.WriteBytes(file.Bytes);
            cbw.Pad(sizeAlign);
            bw.FillInt32($"PaddedFileSize{index}", (int)((cbw.Position - start) / sizeAlign));
        }

        /// <summary>
        /// Wrap a container and write the result.
        /// </summary>
        /// <param name="bytes">The bytes to wrap.</param>
        /// <param name="path">The path to write to.</param>
        /// <param name="wrapperVersion">The wrapper version.</param>
        private void Wrap(byte[] bytes, string path, string wrapperVersion)
        {
            var bnd = new BND3();
            var flags = Binder.Format.BigEndian | Binder.Format.IDs | Binder.Format.Names1 | Binder.Format.Compression;
            bnd.BigEndian = false;
            bnd.BitBigEndian = false;
            bnd.Version = wrapperVersion;
            bnd.Format = flags;
            bnd.WriteFileHeadersEnd = false;
            bnd.Unk18 = 0;

            var fileFlags = Binder.FileFlags.Compressed | Binder.FileFlags.Flag1;
            var file = new BinderFile(fileFlags, 0, Path.GetFileName(path), bytes);
            bnd.Files.Add(file);
            bnd.Write(path);
        }

        /// <summary>
        /// Wrap a container and write the result.
        /// </summary>
        /// <param name="bytes">The bytes to wrap.</param>
        /// <param name="containerIndex">The index of the container.</param>
        /// <param name="wrapperVersion">The wrapper version.</param>
        private byte[] Wrap(byte[] bytes, int containerIndex, string wrapperVersion)
        {
            var bnd = new BND3();
            var flags = Binder.Format.BigEndian | Binder.Format.IDs | Binder.Format.Names1 | Binder.Format.Compression;
            bnd.BigEndian = false;
            bnd.BitBigEndian = false;
            bnd.Version = wrapperVersion;
            bnd.Format = flags;
            bnd.WriteFileHeadersEnd = false;
            bnd.Unk18 = 0;

            var fileFlags = Binder.FileFlags.Compressed | Binder.FileFlags.Flag1;
            var file = new BinderFile(fileFlags, 0, $"{WrappedName}.{containerIndex:D3}", bytes);
            bnd.Files.Add(file);
            return bnd.Write();
        }

        #endregion

        #region Types

        /// <summary>
        /// Different versions of <see cref="Zero3"/>.
        /// </summary>
        public enum FormatVersion
        {
            /// <summary>
            /// The version used in Murakumo: Renegade Mech Pursuit.
            /// </summary>
            Murakumo,

            /// <summary>
            /// The version used in Armored Core 4.
            /// </summary>
            ArmoredCore4
        }

        /// <summary>
        /// The header of a <see cref="Zero3"/>.
        /// </summary>
        private struct ContainerHeader
        {
            /// <summary>
            /// The number of files.
            /// </summary>
            public int FileCount;

            /// <summary>
            /// The offset alignment,
            /// </summary>
            public int OffsetAlign;

            /// <summary>
            /// The size alignment.
            /// </summary>
            public int SizeAlign;

            /// <summary>
            /// The max file size.
            /// </summary>
            public int MaxFileSize;

            /// <summary>
            /// Reads a new <see cref="ContainerHeader"/> from a stream.
            /// </summary>
            /// <param name="br">The stream reader.</param>
            /// <param name="version">The version of the container.</param>
            /// <param name="wrapped">Whether or not the container was wrapped.</param>
            /// <exception cref="InvalidDataException">Unexpected wrapping conditions for the version.</exception>
            /// <exception cref="NotSupportedException">The version was unknown.</exception>
            internal ContainerHeader(BinaryReaderEx br, FormatVersion version, bool wrapped)
            {
                switch (version)
                {
                    case FormatVersion.Murakumo:
                        if (wrapped)
                        {
                            throw new InvalidDataException($"Unexpected wrapping conditions for version: {FormatVersion.Murakumo}");
                        }

                        br.BigEndian = false;
                        FileCount = br.ReadInt32();
                        OffsetAlign = DefaultOffsetAlign;
                        SizeAlign = DefaultSizeAlign;
                        MaxFileSize = DefaultMaxFileSizeMurakumo;
                        br.AssertPattern(60, 0);
                        break;
                    case FormatVersion.ArmoredCore4:
                        FileCount = br.ReadInt32();
                        OffsetAlign = br.ReadInt32();
                        SizeAlign = br.ReadInt32();
                        MaxFileSize = br.ReadInt32();
                        br.AssertPattern(64, 0);
                        break;
                    default:
                        throw new NotSupportedException($"Unknown {nameof(FormatVersion)}: {version}");
                }
            }

            /// <summary>
            /// Creates a new <see cref="ContainerHeader"/> from specified fields.
            /// </summary>
            /// <param name="fileCount">The file count.</param>
            /// <param name="offsetAlign">The offset alignment.</param>
            /// <param name="sizeAlign">The size alignment.</param>
            /// <param name="maxFileSize">The max file size.</param>
            internal ContainerHeader(int fileCount, int offsetAlign, int sizeAlign, int maxFileSize)
            {
                FileCount = fileCount;
                OffsetAlign = offsetAlign;
                SizeAlign = sizeAlign;
                MaxFileSize = maxFileSize;
            }
        }

        /// <summary>
        /// A file in a <see cref="Zero3"/>.
        /// </summary>
        public class File
        {
            /// <summary>
            /// The name of this <see cref="File"/>.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// The bytes of this <see cref="File"/>.
            /// </summary>
            public byte[] Bytes { get; set; }

            /// <summary>
            /// Create a new <see cref="File"/>.
            /// </summary>
            /// <param name="name">The name of this <see cref="File"/>.</param>
            /// <param name="bytes">The bytes of this <see cref="File"/>.</param>
            public File(string name, byte[] bytes)
            {
                Name = name;
                Bytes = bytes;
            }
        }

        #endregion

    }
}
