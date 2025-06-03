namespace SoulsFormats
{
    public partial class FLVER0
    {
        /// <summary>
        /// A single texture map used by a material.
        /// </summary>
        public class Texture : IFlverTexture
        {
            /// <summary>
            /// Indicates the type of texture map this is.
            /// </summary>
            public string Type { get; set; }

            /// <summary>
            /// Network path to the texture file; only the filename without extension is actually used.
            /// </summary>
            public string Path { get; set; }

            /// <summary>
            /// Create a new empty Texture.
            /// </summary>
            public Texture()
            {
                Type = string.Empty;
                Path = string.Empty;
            }

            /// <summary>
            /// Create a new texture with the specified type and path.
            /// </summary>
            /// <param name="type">Indicates the type of texture map.</param>
            /// <param name="path">The name of the texture file.</param>
            public Texture(string type, string path)
            {
                Type = type;
                Path = path;
            }

            /// <summary>
            /// Clone an existing Texture.
            /// </summary>
            public Texture(Texture texture)
            {
                texture.Path = Path;
                texture.Type = Type;
            }

            /// <summary>
            /// Read a texture map from a stream.
            /// </summary>
            internal Texture(BinaryReaderEx br, bool useUnicode)
            {
                int pathOffset = br.ReadInt32();
                int typeOffset = br.ReadInt32();
                br.AssertInt32(0);
                br.AssertInt32(0);

                Path = useUnicode ? br.GetUTF16(pathOffset) : br.GetShiftJIS(pathOffset);
                if (typeOffset > 0)
                    Type = useUnicode ? br.GetUTF16(typeOffset) : br.GetShiftJIS(typeOffset);
                else
                    Type = null;
            }

            /// <summary>
            /// Write this texture map to a stream.
            /// </summary>
            internal void Write(BinaryWriterEx bw, int materialIndex, int textureIndex)
            {
                bw.ReserveInt32($"Path_Offset{materialIndex}_{textureIndex}");
                bw.ReserveInt32($"Type_Offset{materialIndex}_{textureIndex}");
                bw.WriteInt32(0);
                bw.WriteInt32(0);
            }

            /// <summary>
            /// Write the strings of this texture map to a stream.
            /// </summary>
            internal void WriteStrings(BinaryWriterEx bw, int materialIndex, int textureIndex, bool useUnicode)
            {
                bw.FillInt32($"Path_Offset{materialIndex}_{textureIndex}", (int)bw.Position);
                if (useUnicode)
                    bw.WriteUTF16(Path, true);
                else
                    bw.WriteShiftJIS(Path, true);
                bw.FillInt32($"Type_Offset{materialIndex}_{textureIndex}", (int)bw.Position);
                if (useUnicode)
                    bw.WriteUTF16(Type, true);
                else
                    bw.WriteShiftJIS(Type, true);
            }
        }
    }
}
