using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Xml.Serialization;

namespace FLVER_Editor
{
    public class MATBIN
    {
        public enum ParamType : uint
        {
            Bool = 0,
            Int = 4,
            Int2 = 5,
            Float = 8,
            Float2 = 9,
            Float3 = 10,
            Float4 = 11,
            Float5 = 12
        }

        public MATBIN()
        {
            ShaderPath = "";
            SourcePath = "";
            Params = new List<Param>();
            Samplers = new List<Sampler>();
        }

        public string ShaderPath { get; set; }

        public string SourcePath { get; set; }

        public uint Key { get; set; }

        public List<Param> Params { get; set; }

        public List<Sampler> Samplers { get; set; }

        public void Read(BinaryReaderEx br)
        {
            br.BigEndian = false;
            br.AssertASCII("MAB\0");
            br.AssertInt32(2);
            ShaderPath = br.GetUTF16(br.ReadInt64());
            SourcePath = br.GetUTF16(br.ReadInt64());
            Key = br.ReadUInt32();
            int paramCount = br.ReadInt32();
            int samplerCount = br.ReadInt32();
            br.ReadBytes(0x14);
            Params = new List<Param>(paramCount);
            for (var i = 0; i < paramCount; i++)
                Params.Add(new Param(br));
            Samplers = new List<Sampler>(samplerCount);
            for (var i = 0; i < samplerCount; i++)
                Samplers.Add(new Sampler(br));
        }

        [XmlInclude(typeof(int[]))]
        [XmlInclude(typeof(float[]))]
        public class Param
        {
            internal Param(BinaryReaderEx br)
            {
                Name = br.GetUTF16(br.ReadInt64());
                long valueOffset = br.ReadInt64();
                Key = br.ReadUInt32();
                Type = br.ReadEnum32<ParamType>();
                br.ReadBytes(0x10);
                br.StepIn(valueOffset);
                {
                    switch (Type)
                    {
                        case ParamType.Bool:
                            Value = br.ReadBoolean();
                            break;
                        case ParamType.Int:
                            Value = br.ReadInt32();
                            break;
                        case ParamType.Int2:
                            Value = br.ReadInt32s(2);
                            break;
                        case ParamType.Float:
                            Value = br.ReadSingle();
                            break;
                        case ParamType.Float2:
                            Value = br.ReadSingles(2);
                            break;
                        case ParamType.Float3:
                            Value = br.ReadSingles(3);
                            break;
                        case ParamType.Float4:
                            Value = br.ReadSingles(4);
                            break;
                        case ParamType.Float5:
                            Value = br.ReadSingles(5);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                br.StepOut();
            }

            public string Name { get; set; }

            public object Value { get; set; }

            public uint Key { get; set; }

            public ParamType Type { get; set; }
        }

        public class Sampler
        {
            internal Sampler(BinaryReaderEx br)
            {
                Type = br.GetUTF16(br.ReadInt64());
                Path = br.GetUTF16(br.ReadInt64());
                Key = br.ReadUInt32();
                Unk14 = br.ReadVector2();
                br.ReadBytes(0x14);
            }

            public string Type { get; set; }

            public string Path { get; set; }

            public uint Key { get; set; }

            public Vector2 Unk14 { get; set; }
        }
    }
}
