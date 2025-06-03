using System;
using System.IO;
using System.Xml.Serialization;

namespace SoulsFormats
{
    /// <summary>
    /// A generic From file supporting transparent DCX reading and writing.
    /// </summary>
    public abstract class SoulsFile<TFormat> : ISoulsFile where TFormat : SoulsFile<TFormat>, new()
    {
        /// <summary>
        /// The type of DCX compression to be used when writing.
        /// </summary>
        [XmlIgnore]
        public DCX.CompressionInfo Compression { get; set; } = new DCX.NoCompressionInfo();

        #region Is

        /// <summary>
        /// Returns true if the data appears to be a file of this type.
        /// </summary>
        // This should really be a static method, but interfaces do not allow static inheritance; hence the dummy objects below.
        protected virtual bool Is(BinaryReaderEx br)
        {
            throw new NotImplementedException("Is is not implemented for this format.");
        }

        /// <summary>
        /// Returns true if the <see cref="Stream"/> appears to be a file of this type.
        /// </summary>
        public static bool Is(Stream stream)
        {
            if (stream.Length == 0)
                return false;

            if (stream.Position != 0)
            {
                // Cannot ensure offset jumping for every format will work otherwise
                throw new InvalidOperationException($"Cannot safely read if stream is not at position {0}.");
            }

            using (BinaryReaderEx br = new BinaryReaderEx(false, stream, true))
            using (BinaryReaderEx dbr = SFUtil.GetDecompressedBinaryReader(br, out _))
            {
                var dummy = new TFormat();
                bool result = dummy.Is(dbr);
                if (!result)
                {
                    // Reset in case format advances during "Is"
                    stream.Position = 0;
                }

                return result;
            }
        }

        /// <summary>
        /// Returns true if the bytes appear to be a file of this type.
        /// </summary>
        public static bool Is(byte[] bytes)
        {
            if (bytes.Length == 0)
                return false;

            using (BinaryReaderEx br = new BinaryReaderEx(false, bytes))
            using (BinaryReaderEx dbr = SFUtil.GetDecompressedBinaryReader(br, out _))
            {
                var dummy = new TFormat();
                return dummy.Is(dbr);
            }
        }

        /// <summary>
        /// Returns true if the file appears to be a file of this type.
        /// </summary>
        public static bool Is(string path)
        {
            using (FileStream stream = File.OpenRead(path))
            {
                if (stream.Length == 0)
                    return false;

                using (BinaryReaderEx br = new BinaryReaderEx(false, stream, true))
                using (BinaryReaderEx dbr = SFUtil.GetDecompressedBinaryReader(br, out _))
                {
                    var dummy = new TFormat();
                    return dummy.Is(dbr);
                }
            }
        }

        #endregion

        #region Read

        /// <summary>
        /// Reads file data from a stream, automatically decompressing it if necessary.
        /// </summary>
        protected virtual void Read(BinaryReaderEx br)
        {
            throw new NotImplementedException("Read is not implemented for this format.");
        }

        /// <summary>
        /// Reads a file from a <see cref="Stream"/>, automatically decompressing it if necessary.
        /// </summary>
        public static TFormat Read(Stream stream)
        {
            if (stream.Position != 0)
            {
                // Cannot ensure offset jumping for every format will work otherwise
                throw new InvalidOperationException($"Cannot safely read if stream is not at position {0}.");
            }

            stream.Position = 0; 
            using (BinaryReaderEx br = new BinaryReaderEx(false, stream, true))
            using (BinaryReaderEx dbr = SFUtil.GetDecompressedBinaryReader(br, out DCX.CompressionInfo compression))
            {
                TFormat file = new TFormat();
                file.Compression = compression;
                file.Read(dbr);
                return file;
            }
        }

        /// <summary>
        /// Reads a file from a byte array, automatically decompressing it if necessary.
        /// </summary>
        public static TFormat Read(byte[] bytes)
        {
            using (BinaryReaderEx br = new BinaryReaderEx(false, bytes))
            using (BinaryReaderEx dbr = SFUtil.GetDecompressedBinaryReader(br, out DCX.CompressionInfo compression))
            {
                TFormat file = new TFormat();
                file.Compression = compression;
                file.Read(dbr);
                return file;
            }
        }

        /// <summary>
        /// Loads a file from the specified path, automatically decompressing it if necessary.
        /// </summary>
        public static TFormat Read(string path)
        {
            using (FileStream stream = File.OpenRead(path))
            using (BinaryReaderEx br = new BinaryReaderEx(false, stream, true))
            using (BinaryReaderEx dbr = SFUtil.GetDecompressedBinaryReader(br, out DCX.CompressionInfo compression))
            {
                TFormat file = new TFormat();
                file.Compression = compression;
                file.Read(dbr);
                return file;
            }
        }

        #endregion

        #region IsRead

        /// <summary>
        /// Returns whether or not the data appears to be a file of this type and reads it if so, automatically decompressing it if necessary.
        /// </summary>
        private static bool IsReadInternal(BinaryReaderEx br, out TFormat file)
        {
            using (BinaryReaderEx dbr = SFUtil.GetDecompressedBinaryReader(br, out DCX.CompressionInfo compression))
            {
                var test = new TFormat();
                if (test.Is(dbr))
                {
                    br.Position = 0;
                    dbr.Position = 0;
                    test.Compression = compression;
                    test.Read(dbr);
                    file = test;
                    return true;
                }
                else
                {
                    file = null;
                    return false;
                }
            }
        }

        /// <summary>
        /// Returns whether the stream appears to be a file of this type and reads it if so.
        /// </summary>
        public static bool IsRead(Stream stream, out TFormat file)
        {
            if (stream.Position != 0)
            {
                // Cannot ensure offset jumping for every format will work otherwise
                throw new InvalidOperationException($"Cannot safely read if stream is not at position {0}.");
            }

            using (var br = new BinaryReaderEx(false, stream, true))
            {
                bool result = IsReadInternal(br, out file);
                if (!result)
                {
                    // Reset in case format advances during "Is"
                    stream.Position = 0;
                }

                return result;
            }
        }

        /// <summary>
        /// Returns whether the bytes appear to be a file of this type and reads it if so.
        /// </summary>
        public static bool IsRead(byte[] bytes, out TFormat file)
        {
            using (var br = new BinaryReaderEx(false, bytes))
            {
                return IsReadInternal(br, out file);
            }
        }

        /// <summary>
        /// Returns whether the file appears to be a file of this type and reads it if so.
        /// </summary>
        public static bool IsRead(string path, out TFormat file)
        {
            using (FileStream fs = File.OpenRead(path))
            using (BinaryReaderEx br = new BinaryReaderEx(false, fs, true))
            {
                return IsReadInternal(br, out file);
            }
        }

        #endregion

        #region Write

        /// <summary>
        /// Serializes file data to a stream.
        /// </summary>
        protected virtual void Write(BinaryWriterEx bw)
        {
            throw new NotImplementedException("Write is not implemented for this format.");
        }

        /// <summary>
        /// Writes file data to a stream, compressing it afterwards if specified.
        /// </summary>
        private void Write(BinaryWriterEx bw, DCX.CompressionInfo compression)
        {
            if (compression.Type == DCX.Type.None)
            {
                Write(bw);
            }
            else
            {
                using (BinaryWriterEx dbw = new BinaryWriterEx(false))
                {
                    Write(dbw);
                    byte[] uncompressed = dbw.FinishBytes();
                    DCX.Compress(uncompressed, bw, compression);
                }
            }
        }

        /// <summary>
        /// Writes the file to an array of bytes, automatically compressing it if necessary.
        /// </summary>
        public byte[] Write()
        {
            return Write(Compression);
        }

        /// <summary>
        /// Writes the file to an array of bytes, compressing it as specified.
        /// </summary>
        public byte[] Write(DCX.CompressionInfo compression)
        {
            if (!Validate(out Exception ex))
                throw ex;

            using (BinaryWriterEx bw = new BinaryWriterEx(false))
            {
                Write(bw, compression);
                return bw.FinishBytes();
            }
        }

        /// <summary>
        /// Writes the file to the specified path, automatically compressing it if necessary.
        /// </summary>
        public void Write(string path)
        {
            Write(path, Compression);
        }

        /// <summary>
        /// Writes the file to the specified path, compressing it as specified.
        /// </summary>
        public void Write(string path, DCX.CompressionInfo compression)
        {
            if (!Validate(out Exception ex))
                throw ex;

            string dirName = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dirName))
            {
                Directory.CreateDirectory(dirName);
            }

            using (FileStream stream = File.Create(path))
            using (BinaryWriterEx bw = new BinaryWriterEx(false, stream, true))
            {
                Write(bw, compression);
                bw.Finish();
            }
        }

        #endregion

        #region Validation

        /// <summary>
        /// Checks the object for any fatal problems; Write will throw the returned exception on failure.
        /// </summary>
        public virtual bool Validate(out Exception ex)
        {
            ex = null;
            return true;
        }

        /// <summary>
        /// Returns whether the object is not null, otherwise setting ex to a NullReferenceException with the given message.
        /// </summary>
        protected static bool ValidateNull(object obj, string message, out Exception ex)
        {
            if (obj == null)
            {
                ex = new NullReferenceException(message);
                return false;
            }
            else
            {
                ex = null;
                return true;
            }
        }

        /// <summary>
        /// Returns whether the index is in range, otherwise setting ex to an IndexOutOfRangeException with the given message.
        /// </summary>
        protected static bool ValidateIndex(long count, long index, string message, out Exception ex)
        {
            if (index < 0 || index >= count)
            {
                ex = new IndexOutOfRangeException(message);
                return false;
            }
            else
            {
                ex = null;
                return true;
            }
        }

        #endregion
    }
}
