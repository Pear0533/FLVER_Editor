using SoulsAssetPipeline.XmlStructs;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static SoulsFormats.FLVER2;

namespace SoulsAssetPipeline.FLVERImporting
{
    public class FLVER2MaterialInfoBank
    {
        public string DefaultFallbackMTDName;

        public string FallbackToDefaultMtdIfNecessary(string mtd, SapLogger logger)
        {
            if (mtd != null && MaterialDefs.ContainsKey(mtd.ToLower()))
            {
                return mtd;
            }
            else
            {
                if (mtd != null)
                    logger?.LogWarning($"MTD {mtd} did not exist. Falling back to default, which is '{DefaultFallbackMTDName}'.");
                else
                    logger?.LogWarning($"MTD was not specified. Falling back to default, which is '{DefaultFallbackMTDName}'.");
                return DefaultFallbackMTDName;
            }
        }

        public Dictionary<string, MaterialDef> MaterialDefs = new Dictionary<string, MaterialDef>();
        public Dictionary<string, XmlStructDef> GXItemStructs = new Dictionary<string, XmlStructDef>();

        public Dictionary<string, List<byte[]>> DefaultGXItemDataExamples = new Dictionary<string, List<byte[]>>();

        public string GetTexChannelName(string mtd, FlverTextureChannelType channelType)
        {
            //ERRORTODO you know the drill
            return MaterialDefs[mtd].TextureChannels[channelType];
        }

        public List<GXItem> GetDefaultGXItemsForMTD(string mtd)
        {
            if (MaterialDefs.ContainsKey(mtd) && DefaultGXItemDataExamples.ContainsKey(mtd))
            {
                var gxList = new List<GXItem>();
                var matDef = MaterialDefs[mtd];
                for (int i = 0; i < matDef.GXItems.Count; i++)
                {
                    var gx = new GXItem(matDef.GXItems[i].GXID, matDef.GXItems[i].Unk04, DefaultGXItemDataExamples[mtd][i]);
                    gxList.Add(gx);
                }
                return gxList;
            }
            else
            {
                //ERRORTODO
                throw new Exception("Default GXItems not found in material def. (UNFINISHED_ERROR_MESSAGE)");
            }
        }

        public class VertexBufferDeclaration
        {
            public List<BufferLayout> Buffers = new List<BufferLayout>();

            internal void ReadXML(XmlNode node)
            {
                var bufferLayoutNodes = node.SelectNodes("vertex_buffer");
                int bufferIndex = 0;
                Buffers.Clear();
                foreach (XmlNode bln in bufferLayoutNodes)
                {
                    var bufferLayout = new BufferLayout();
                    foreach (XmlNode memberNode in bln.ChildNodes)
                    {
                        FLVER.LayoutType memberType = (FLVER.LayoutType)Enum.Parse(typeof(FLVER.LayoutType), memberNode.Name);
                        FLVER.LayoutSemantic memberSemantic = FLVER.LayoutSemantic.Position;
                        var memberIndex = 0;
                        var memberBufferIndex = bufferIndex;



                        // Try to parse Semantic[Index] lol
                        int memberSemanticLeftBracketIndex = memberNode.InnerText.IndexOf('[');
                        int memberSemanticRightBracketIndex = memberNode.InnerText.IndexOf(']');
                        int memberSemanticIndexStrLength = memberSemanticRightBracketIndex - memberSemanticLeftBracketIndex - 1;
                        // If it has [] brackets with text inbetween them, parse index from within brackets and semantic from before brackets.
                        if (memberSemanticLeftBracketIndex >= 0 && memberSemanticRightBracketIndex >= 0 && memberSemanticIndexStrLength > 0)
                        {
                            memberIndex = int.Parse(memberNode.InnerText.Substring(memberSemanticLeftBracketIndex + 1, memberSemanticIndexStrLength));
                            memberSemantic = (FLVER.LayoutSemantic)Enum.Parse(typeof(FLVER.LayoutSemantic), 
                                memberNode.InnerText.Substring(0, memberSemanticLeftBracketIndex));
                        }
                        // Otherwise entire string parsed as semantic and index is 0.
                        else
                        {
                            memberIndex = 0;
                            memberSemantic = (FLVER.LayoutSemantic)Enum.Parse(typeof(FLVER.LayoutSemantic), memberNode.InnerText);
                        }


                        var bufferIndexText = memberNode.Attributes["from_buffer_index"]?.InnerText;
                        if (bufferIndexText != null && int.TryParse(bufferIndexText, out int specifiedBufferIndex))
                        {
                            memberBufferIndex = specifiedBufferIndex;
                        }


                        bufferLayout.Add(new FLVER.LayoutMember(memberType, memberSemantic, memberIndex, memberBufferIndex));
                    }
                    Buffers.Add(bufferLayout);
                    bufferIndex++;
                }
            }


        }

        public class GXItemDef
        {
            public string GXID;
            public int Unk04;
            public int DataLength;

            public void ReadXML(XmlNode node)
            {
                GXID = node.SafeGetAttribute("gxid");
                Unk04 = node.SafeGetInt32Attribute("unk04");
                DataLength = node.SafeGetInt32Attribute("data_length");
            }
        }

        public struct FlverTextureChannelType
        {
            public TextureChannelSemantic Semantic;
            public int Index;
            public string Name;
            public override string ToString()
            {
                return $"{Name} - {Semantic}[{Index}]";
            }
        }

        public class MaterialDef
        {
            public string MTD;
            public List<VertexBufferDeclaration> AcceptableVertexBufferDeclarations 
                = new List<VertexBufferDeclaration>();
            public List<GXItemDef> GXItems = new List<GXItemDef>();


            public Dictionary<FlverTextureChannelType, string> TextureChannels 
                = new Dictionary<FlverTextureChannelType, string>();

            public static FlverTextureChannelType GetTexChannelTypeFromName(string name)
            {
                FlverTextureChannelType t = new FlverTextureChannelType
                {
                    Name = name,
                    Semantic = TextureChannelSemantic.Unknown
                };

                bool Check(params string[] check)
                {
                    foreach (var s in check)
                        if (name.Contains(s))
                            return true;

                    return false;
                }

                if (Check("Diffuse", "Albedo"))
                    t.Semantic = TextureChannelSemantic.Diffuse;

                else if (Check("Specular", "Reflectance"))
                    t.Semantic = TextureChannelSemantic.Specular;

                else if (Check("Shininess"))
                    t.Semantic = TextureChannelSemantic.Shininess;

                // Check before normals because this contains "Bumpmap"
                else if (Check("DetailBumpmap", "DetailBumpMap", "DetailNormal"))
                    t.Semantic = TextureChannelSemantic.DetailNormals;
                else if (Check("AdditionalBumpmapTexture"))
                    t.Semantic = TextureChannelSemantic.AdditionalNormals;
                // Check before normals because this contains "Bumpmap"
                else if (Check("BreakBumpmap", "DamageNormal", "DamagedNormalTexture"))
                    t.Semantic = TextureChannelSemantic.EquipmentBrokenNormals;
                else if (Check("NormalMap", "Bumpmap"))
                    t.Semantic = TextureChannelSemantic.Normals;

                else if (Check("EmissiveMask"))
                    t.Semantic = TextureChannelSemantic.Emissive;
                else if (Check("Emissive"))
                    t.Semantic = TextureChannelSemantic.Emissive;

                else if (Check("BlendMask", "Blendmask", "BlendMap"))
                    //TODO: Double check if "Blendmask" is used
                    t.Semantic = TextureChannelSemantic.Blendmask;

                else if (Check("Vector"))
                    t.Semantic = TextureChannelSemantic.Vector;

                else if (Check("_MaskTexture"))
                    t.Semantic = TextureChannelSemantic.Mask;

                else if (Check("BlendEdge"))
                    t.Semantic = TextureChannelSemantic.BlendEdge;

                else if (Check("BloodMask"))
                    t.Semantic = TextureChannelSemantic.BloodMask;

                else if (Check("Displacement"))
                    t.Semantic = TextureChannelSemantic.Displacement;

                else if (Check("ScatteringMask", "SSSMask", "g_Subsurf"))
                    t.Semantic = TextureChannelSemantic.ScatteringMask;

                else if (Check("HighLight", "Highlight"))
                    t.Semantic = TextureChannelSemantic.Highlight;

                else if (Check("OpacityTexture"))
                    t.Semantic = TextureChannelSemantic.Opacity;

                else if (Check("_MetallicMap"))
                    t.Semantic = TextureChannelSemantic.Metallic;

                else if (Check("_Mask1Map"))
                    t.Semantic = TextureChannelSemantic.SDTMask1;

                else if (Check("_Mask3Map"))
                    t.Semantic = TextureChannelSemantic.SDTMask3;

                else if (Check("_Ripple"))
                    t.Semantic = TextureChannelSemantic.SDTRipple;

                else if (Check("_AmbientOcculusionMap")) // Typo everywhere it's found in SDT
                    t.Semantic = TextureChannelSemantic.AmbientOcclusion;

                else if (Check("_Star")) // Typo everywhere it's found in SDT
                    t.Semantic = TextureChannelSemantic.SDTStar;

                else if (Check("FlowMap", "_FlowTexture"))
                    t.Semantic = TextureChannelSemantic.Flow;

                else if (Check("_アルファマップ")) // Lit. "ALPHA MAP"
                    // Going to guess alpha map is opacity
                    t.Semantic = TextureChannelSemantic.Opacity;

                else if (Check("HeightMap", "g_SnowHeightTexture", "g_Height"))
                    t.Semantic = TextureChannelSemantic.Height;

                else if (Check("_Foam1"))
                    t.Semantic = TextureChannelSemantic.SDTFoam1;

                else if (Check("_BurningMap"))
                    t.Semantic = TextureChannelSemantic.DS3Burning;

                else if (Check("_DOLTexture1", "g_Lightmap"))
                {
                    t.Semantic = TextureChannelSemantic.Lightmap1;
                    if (name.EndsWith("_DOLTexture1"))
                        t.Index = 0;
                }

                else if (Check("_DOLTexture2"))
                {
                    t.Semantic = TextureChannelSemantic.Lightmap2;
                    if (name.EndsWith("_DOLTexture2"))
                        t.Index = 0;
                }

                else if (Check("GITexture"))
                    t.Semantic = TextureChannelSemantic.GlobalIllumination;

                //if (t.Semantic == TextureChannelSemantic.Unknown)
                //    throw new NotImplementedException($"Texture channel type '{name}' not recognized.");

                if (char.IsDigit(name[name.Length - 1]))
                {
                    t.Index = int.Parse(name.Substring(name.Length - 1));
                }

                return t;
            }

            public void ReadXML(XmlNode node)
            {
                MTD = node.Attributes["mtd"].InnerText.ToLower();


                AcceptableVertexBufferDeclarations.Clear();

                var vertBufferDeclarations = node.SelectNodes("acceptable_vertex_buffer_declarations/vertex_buffer_declaration");
                foreach (XmlNode vbnode in vertBufferDeclarations)
                {
                    var vb = new VertexBufferDeclaration();
                    vb.ReadXML(vbnode);
                    AcceptableVertexBufferDeclarations.Add(vb);
                }


                TextureChannels.Clear();

                var texChannelNodes = node.SelectNodes("texture_channel_list/texture_channel");
                foreach (XmlNode tcn in texChannelNodes)
                {
                    var texChannelSemanticIndex = tcn.SafeGetInt32Attribute("index");

                    string semanticAttribute = tcn.SafeGetAttribute("semantic");
                    var texChannelSemantic = (TextureChannelSemantic)Enum.Parse(typeof(TextureChannelSemantic), semanticAttribute);

                    // Try to figure out at runtime.
                    if (texChannelSemantic == TextureChannelSemantic.Unknown)
                    {
                        texChannelSemantic = GetTexChannelTypeFromName(tcn.InnerText).Semantic;
                    }

                    // See if runtime check passed.
                    if (texChannelSemantic == TextureChannelSemantic.Unknown)
                    {
                        // throw new Exception($"Semantic of texture channel '{tcn.InnerText}' not defined.");
                    }

                    var chanTypeKey = new FlverTextureChannelType()
                    {
                        Name = tcn.InnerText,
                        Semantic = texChannelSemantic,
                        Index = texChannelSemanticIndex,
                    };


                    if (!TextureChannels.ContainsKey(chanTypeKey))
                        TextureChannels.Add(chanTypeKey, tcn.InnerText);
                    //ErrorTODO else print warning maybe
                }


                GXItems.Clear();

                var gxItemNodes = node.SelectNodes("gx_item_list/gx_item");
                foreach (XmlNode gin in gxItemNodes)
                {
                    var g = new GXItemDef();
                    g.ReadXML(gin);
                    GXItems.Add(g);
                }
            }
        }

        internal void ReadXML(string xmlFile)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlFile);

            DefaultFallbackMTDName = xml.SelectSingleNode("material_info_bank/default_fallback_mtd_name")?.InnerText.ToLower();

            var materialDefNodes = xml.SelectNodes("material_info_bank/material_def_list/material_def");

            MaterialDefs.Clear();

            foreach (XmlNode mdn in materialDefNodes)
            {
                var mat = new MaterialDef();
                mat.ReadXML(mdn);
                if (!MaterialDefs.ContainsKey(mat.MTD))
                    MaterialDefs.Add(mat.MTD, mat);
                else
                    //throw new Exception($"MTD '{mat.MTD}' defined twice in material info bank.");
                    MaterialDefs[mat.MTD] = mat;
            }

            GXItemStructs.Clear();

            var gxItemNodes = xml.SelectNodes("material_info_bank/gx_item_struct_list/gx_item_struct");
            foreach (XmlNode gin in gxItemNodes)
            {
                string gxid = gin.SafeGetAttribute("gxid");
                var structDef = new XmlStructDef(gin);
                GXItemStructs.Add(gxid, structDef);
            }

            DefaultGXItemDataExamples.Clear();

            var matExamples = xml.SelectNodes("material_info_bank/MATERIAL_INSTANCE_EXAMPLE_LIST/MATERIAL_INSTANCE_EXAMPLE");
            foreach (XmlNode matExNode in matExamples)
            {
                var mtd = matExNode.SafeGetAttribute("mtd").ToLower();
                var exNodes = matExNode.SelectNodes("material_data_example");

                List<byte[]> gxExamplesForThisMtd = new List<byte[]>();

                foreach (XmlNode mx in exNodes)
                {
                    var gxExamples = mx.SelectNodes("gx_item_list/gx_item");
                    foreach (XmlNode gn in gxExamples)
                    {
                        byte[] gxData = gn.InnerText.Split(' ').Select(x => byte.Parse(x, System.Globalization.NumberStyles.HexNumber)).ToArray();
                        gxExamplesForThisMtd.Add(gxData);
                    }

                    break; // Read only one for now
                }

                if (!DefaultGXItemDataExamples.ContainsKey(mtd))
                    DefaultGXItemDataExamples.Add(mtd, gxExamplesForThisMtd);
            }
        }

        public static FLVER2MaterialInfoBank ReadFromXML(string xmlFile)
        {
            var bank = new FLVER2MaterialInfoBank();
            bank.ReadXML(xmlFile);
            return bank;
        }
    }
}
