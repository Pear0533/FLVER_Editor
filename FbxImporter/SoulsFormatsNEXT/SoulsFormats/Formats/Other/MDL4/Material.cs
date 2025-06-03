using System;
using System.Collections.Generic;

namespace SoulsFormats.Other
{
    /// <summary>
    /// A 3D model format used in early PS3/X360 games. Extension: .mdl
    /// </summary>
    public partial class MDL4
    {
        /// <summary>
        /// A material containing parameters for various material related things.
        /// </summary>
        public class Material
        {
            /// <summary>
            /// Identifies the mesh that uses this material.
            /// </summary>
            public string Name;

            /// <summary>
            /// Identifies the shader that this material uses.
            /// </summary>
            public string Shader;

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte Unk3C;

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte Unk3D;

            /// <summary>
            /// Unknown.
            /// </summary>
            public byte Unk3E;

            /// <summary>
            /// A list of material parameters.
            /// </summary>
            public List<MATParam> Params;

            /// <summary>
            /// Reads a material from an MDL4.
            /// </summary>
            internal Material(BinaryReaderEx br)
            {
                Name = br.ReadFixStr(0x1F);
                Shader = br.ReadFixStr(0x1D);
                Unk3C = br.ReadByte();
                Unk3D = br.ReadByte();
                Unk3E = br.ReadByte();
                byte paramCount = br.ReadByte();

                long paramsOffset = br.Position;
                Params = new List<MATParam>(paramCount);
                for (int i = 0; i < paramCount; i++)
                    Params.Add(new MATParam(br));
                br.Position = paramsOffset + 0x800;
            }

            /// <summary>
            /// Retrieves a dictionary containing the texture names used in a material with the names of the associated material parameters as the keys
            /// </summary>
            public Dictionary<string, string> GetTexDict()
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                for (int i = 0; i < Params.Count; i++)
                {
                    if (Params[i].Name.ToLower().Contains("texture"))
                    {
                        parameters.Add(Params[i].Name.ToLower(), (string)Params[i].Value + ".dds");
                    }
                }
                return parameters;
            }


            /// <summary>
            /// Retrieves a list containing the texture names used in a material
            /// </summary>
            public List<string> GetTexList()
            {
                List<string> parameters = new List<string>();
                for (int i = 0; i < Params.Count; i++)
                {
                    if (Params[i].Name.ToLower().Contains("texture"))
                    {
                        parameters.Add((string)Params[i].Value + ".dds");
                    }
                }
                return parameters;
            }

            /// <summary>
            /// Writes a material to an MDL4.
            /// </summary>
            internal void Write(BinaryWriterEx bw)
            {
                bw.WriteFixStr(Name, 0x1F);
                bw.WriteFixStr(Shader, 0x1D);
                bw.WriteByte(Unk3C);
                bw.WriteByte(Unk3D);
                bw.WriteByte(Unk3E);
                bw.WriteByte((byte)Params.Count);

                long paramsOffset = bw.Position;
                for (int i = 0; i < Params.Count; i++)
                    Params[i].Write(bw);
                bw.Position = paramsOffset + 0x800;
            }

            /// <summary>
            /// Material parameters for
            /// Alpha Blend, Ref, and Map Txture;
            /// Diffuse Col, Coefficient and Texture;
            /// Specular Col, Coefficient, and Power; 
            /// Environment Map Fresnel Scale and Bias;
            /// Cube Environment Map Texture;
            /// Bumpmap Texture;
            /// and Parallax Offset.
            /// </summary>
            public class MATParam
            {
                /// <summary>
                /// The type of the value for this material param.
                /// </summary>
                public ParamType Type;

                /// <summary>
                /// The name of this material param indicating what it controls.
                /// </summary>
                public string Name;

                /// <summary>
                /// The value for this material param.
                /// </summary>
                public object Value;

                /// <summary>
                /// Read a MATParam from a stream.
                /// </summary>
                internal MATParam(BinaryReaderEx br)
                {
                    long start = br.Position;
                    Type = br.ReadEnum8<ParamType>();
                    Name = br.ReadFixStr(0x1F);

                    switch (Type)
                    {
                        case ParamType.Int: Value = br.ReadInt32(); break;
                        case ParamType.Float: Value = br.ReadSingle(); break;
                        case ParamType.Float4: Value = br.ReadSingles(4); break;
                        case ParamType.String: Value = br.ReadShiftJIS(); break;

                        default:
                            throw new NotImplementedException("Unknown param type: " + Type);
                    }

                    br.Position = start + 0x40;
                }

                /// <summary>
                /// Write a MATParam to a stream.
                /// </summary>
                internal void Write(BinaryWriterEx bw)
                {
                    bw.WriteByte((byte)Type);
                    bw.WriteFixStr(Name, 0x1F);

                    switch (Type)
                    {
                        case ParamType.Int: bw.WriteInt32((int)Value); break;
                        case ParamType.Float: bw.WriteSingle((float)Value); break;
                        case ParamType.Float4: bw.WriteSingles((float[])Value); break;
                        case ParamType.String: bw.WriteShiftJIS((string)Value, true); break;

                        default:
                            throw new NotImplementedException("Unknown param type: " + Type);
                    }

                    bw.Pad(0x20);
                }
            }

            /// <summary>
            /// The type a material param is.
            /// </summary>
            public enum ParamType : byte
            {
                /// <summary>
                /// Category for:
                /// Alpha Blend and Ref.
                /// </summary>
                Int = 0,

                /// <summary>
                /// Category for:
                /// Diffuse and Coefficient;
                /// Specular Coefficient and Pwer;
                /// Environment Map Fresnel Scale and Bias;
                /// and Parallax Offset.
                /// </summary>
                Float = 1,

                /// <summary>
                /// Category for:
                /// Diffuse Col;
                /// and Specular Col.
                /// </summary>
                Float4 = 4,

                /// <summary>
                /// Category for:
                /// Diffuse Texture;
                /// Alpha Map Texture;
                /// Cube Environment Map Texture;
                /// and Bumpmap Texture.
                /// </summary>
                String = 5,
            }
        }
    }
}
