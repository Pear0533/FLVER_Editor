using SoulsAssetPipeline;
using SoulsAssetPipeline.XmlStructs;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SapFlverScan
{
    class Convert
    {
        public static void USAGE()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  SapFlverScan <FlverDirectory> <XmlFileToOutput>");
            Console.WriteLine("  ");
            Console.WriteLine("    e.g. SapFlverScan \"C:\\ds1_flver_dump\" \"DS1FlverInfoDump.xml\"");
            Console.WriteLine("  ");
            Console.WriteLine("    Misc Notes:");
            Console.WriteLine("      -File filter picks up both *.flver and *.flv (\"*.flv*\").");
            Console.WriteLine("      -You can also just unpack the game and select that dirctory and all bnd files will be searched for flvers, as well.");
            Console.WriteLine("      -The XML output file will maintain a read/write handle during the scan to ensure");
            Console.WriteLine("       the write at the end succeeds.");

        }

        public class InstanceofGXItem : IEquatable<InstanceofGXItem>
        {
            public string IDString;
            public int Unk04Value;
            public int DataLength;

            public byte[] Example_Data;

            public InstanceofGXItem(FLVER2.GXItem gx)
            {
                IDString = gx.ID;
                Unk04Value = gx.Unk04;
                DataLength = gx.Data.Length;
                Example_Data = gx.Data;
            }

            public bool Equals(InstanceofGXItem other)
            {
                if (IDString != other.IDString)
                    return false;
                if (Unk04Value != other.Unk04Value)
                    return false;
                if (DataLength != other.DataLength)
                    return false;

                return true;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as InstanceofGXItem);
            }
        }

        public static bool DoListsHaveSameThingsInThem<TItem>(List<TItem> listA, List<TItem> listB)
            where TItem : IEquatable<TItem>
        {
            if (listA.Count != listB.Count)
                return false;
            for (int i = 0; i < listA.Count; i++)
            {
                if (!listA.Contains(listB[i]))
                    return false;

                if (!listB.Contains(listA[i]))
                    return false;
            }

            return true;
        }

        public class InstanceofVertexBufferSetup : IEquatable<InstanceofVertexBufferSetup>
        {
            public List<FLVER2.BufferLayout> VertexBuffers = new List<FLVER2.BufferLayout>();
            public InstanceofVertexBufferSetup(FLVER2 flver, FLVER2.Mesh m)
            {
                foreach (var vb in m.VertexBuffers)
                {
                    FLVER2.BufferLayout layout = vb.LayoutIndex >= 0 ? flver.BufferLayouts[vb.LayoutIndex] : null;
                    VertexBuffers.Add(layout);
                }
            }

            public bool Equals(InstanceofVertexBufferSetup other)
            {
                if (VertexBuffers.Count != other.VertexBuffers.Count)
                    return false;

                for (int i = 0; i < VertexBuffers.Count; i++)
                {
                    if (VertexBuffers[i].Count != other.VertexBuffers[i].Count)
                        return false;

                    for (int j = 0; j < VertexBuffers[i].Count; j++)
                    {
                        var a = VertexBuffers[i][j];
                        var b = other.VertexBuffers[i][j];
                        if (a.Semantic != b.Semantic)
                            return false;
                        if (a.Type != b.Type)
                            return false;
                        if (a.Unk00 != b.Unk00)
                            return false;
                        if (a.Index != b.Index)
                            return false;
                    }
                }

                return true;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as InstanceofVertexBufferSetup);
            }
        }

        public class InstanceofFLVER2Mesh_ExampleData
        {
            public string Example_FlverName;
            public string Example_MaterialName;
            public int Example_Flags;
            public int Example_Unk18;
            public List<InstanceofGXItem> Example_GXItems = new List<InstanceofGXItem>();
            public List<FLVER2.Texture> Example_TextureList = new List<FLVER2.Texture>();

            public void WriteXML(XmlWriter writer)
            {
                writer.WriteStartElement("material_data_example");
                {
                    writer.WriteAttributeString("flver_name", Example_FlverName.ToString());
                    writer.WriteAttributeString("name", Example_MaterialName.ToString());
                    writer.WriteAttributeString("flags", Example_Flags.ToString());
                    writer.WriteAttributeString("unk18", Example_Unk18.ToString());

                    writer.WriteStartElement("gx_item_list");
                    {
                        foreach (var gx in Example_GXItems)
                        {
                            writer.WriteStartElement("gx_item");
                            {
                                writer.WriteAttributeString("id", gx.IDString);
                                writer.WriteAttributeString("unk04", gx.Unk04Value.ToString());
                                writer.WriteAttributeString("data_length", gx.DataLength.ToString());

                                writer.WriteString(string.Join(" ", gx.Example_Data.Select(x => x.ToString("X2"))));

                                //gx.WriteTemplateGuessBasedOnExampleData(writer, gx.Example_Data);
                            }
                            writer.WriteEndElement();
                        }
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("texture_list");
                    {
                        foreach (var tex in Example_TextureList)
                        {
                            writer.WriteStartElement("texture");
                            {
                                writer.WriteAttributeString("type", tex.Type);
                                writer.WriteAttributeString("path", tex.Path);
                                writer.WriteAttributeString("scale", $"{tex.Scale.X},{tex.Scale.Y}");
                                writer.WriteAttributeString("unk10", tex.Unk10.ToString());
                                writer.WriteAttributeString("unk11", tex.Unk11.ToString());
                                writer.WriteAttributeString("unk14", tex.Unk14.ToString());
                                writer.WriteAttributeString("unk18", tex.Unk18.ToString());
                                writer.WriteAttributeString("unk1C", tex.Unk1C.ToString());
                            }
                            writer.WriteEndElement();
                        }
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        public class InstanceofTextureChannel : IEquatable<InstanceofTextureChannel>
        {
            public string Name;
            public SoulsAssetPipeline.FLVERImporting.FLVER2MaterialInfoBank.FlverTextureChannelType ChannelType;

            public InstanceofTextureChannel(string name)
            {
                Name = name;
                ChannelType = SoulsAssetPipeline.FLVERImporting.FLVER2MaterialInfoBank.MaterialDef.GetTexChannelTypeFromName(Name);
            }

            public bool Equals(InstanceofTextureChannel other)
            {
                return (Name.Trim().ToLower() == other.Name.Trim().ToLower());
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as InstanceofTextureChannel);
            }
        }

        public class InstanceofFLVER2Mesh : IEquatable<InstanceofFLVER2Mesh>
        {
            public string MTD;

            public List<InstanceofTextureChannel> TextureChannelDeclarations = new List<InstanceofTextureChannel>();
            public List<InstanceofGXItem> GXItems = new List<InstanceofGXItem>();
            //public InstanceofVertexBufferSetup VertexBufferSetup;
            public List<InstanceofVertexBufferSetup> PossibleVertexBufferSetups = new List<InstanceofVertexBufferSetup>();

            public InstanceofFLVER2Mesh_ExampleData ExampleData = new InstanceofFLVER2Mesh_ExampleData();

            public void WriteUniqueStuffXML(XmlWriter writer)
            {
                writer.WriteAttributeString("mtd", MTD);

                writer.WriteStartElement("acceptable_vertex_buffer_declarations");
                {
                    foreach (var vbs in PossibleVertexBufferSetups)
                    {
                        writer.WriteStartElement("vertex_buffer_declaration");
                        {
                            for (int i = 0; i < vbs.VertexBuffers.Count; i++)
                            {
                                writer.WriteStartElement("vertex_buffer");
                                {
                                    //writer.WriteAttributeString("index", i.ToString());
                                    foreach (var member in vbs.VertexBuffers[i])
                                    {
                                        writer.WriteStartElement(member.Type.ToString());
                                        {
                                            bool showIndex = false;
                                            foreach (var memberIndexCheck in vbs.VertexBuffers[i])
                                            {
                                                if (memberIndexCheck.Semantic == member.Semantic && memberIndexCheck.Index > 0)
                                                {
                                                    showIndex = true;
                                                    break;
                                                }
                                            }

                                            //Note: this is hidden if its same as buffer index
                                            if (member.Unk00 != i)
                                                writer.WriteAttributeString("from_buffer_index", member.Unk00.ToString());

                                            if (showIndex)
                                                writer.WriteString($"{member.Semantic}[{member.Index}]");
                                            else
                                                writer.WriteString(member.Semantic.ToString());


                                        }
                                        writer.WriteEndElement();
                                    }
                                }
                                writer.WriteEndElement();
                            }
                        }
                        writer.WriteEndElement();
                    }


                }
                writer.WriteEndElement();

                writer.WriteStartElement("texture_channel_list");
                {
                    foreach (var tc in TextureChannelDeclarations)
                    {
                        writer.WriteStartElement("texture_channel");
                        {
                            writer.WriteAttributeString("semantic", tc.ChannelType.Semantic.ToString());
                            writer.WriteAttributeString("index", tc.ChannelType.Index.ToString());
                            writer.WriteString(tc.Name);
                        }
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();

                writer.WriteStartElement("gx_item_list");
                {
                    for (int gxi = 0; gxi < GXItems.Count; gxi++)
                    {
                        writer.WriteStartElement("gx_item");
                        {
                            writer.WriteAttributeString("gxid", GXItems[gxi].IDString);
                            writer.WriteAttributeString("unk04", GXItems[gxi].Unk04Value.ToString());
                            writer.WriteAttributeString("data_length", GXItems[gxi].DataLength.ToString());
                        }
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();
            }

            public InstanceofFLVER2Mesh(string flverName, FLVER2 flver, FLVER2.Mesh mesh)
            {
                var mat = flver.Materials[mesh.MaterialIndex];
                MTD = Path.GetFileName(mat.MTD);
                if (flver.GXLists.Count > 0 && mat.GXIndex >= 0)
                {
                    var gxlist = flver.GXLists[mat.GXIndex];
                    foreach (var gx in gxlist)
                    {
                        var gxInstance = new InstanceofGXItem(gx);
                        ExampleData.Example_GXItems.Add(gxInstance);
                        if (!GXItems.Contains(gxInstance))
                            GXItems.Add(gxInstance);
                    }
                }
                ExampleData.Example_Flags = 0;
                ExampleData.Example_Unk18 = mat.Index;

                PossibleVertexBufferSetups = new List<InstanceofVertexBufferSetup>();
                PossibleVertexBufferSetups.Add(new InstanceofVertexBufferSetup(flver, mesh));

                ExampleData.Example_TextureList = mat.Textures;

                ExampleData.Example_FlverName = flverName;
                ExampleData.Example_MaterialName = mat.Name;
                foreach (var tex in mat.Textures)
                {
                    TextureChannelDeclarations.Add(new InstanceofTextureChannel(tex.Type));
                }
            }

            public bool Equals(InstanceofFLVER2Mesh other)
            {
                if (MTD.Trim().ToLower() != other.MTD.Trim().ToLower())
                    return false;

                if (!DoListsHaveSameThingsInThem(TextureChannelDeclarations, other.TextureChannelDeclarations))
                    return false;

                if (!DoListsHaveSameThingsInThem(GXItems, other.GXItems))
                    return false;

                //if (!VertexBufferSetup.Equals(other.VertexBufferSetup))
                //    return false;

                return true;
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as InstanceofFLVER2Mesh);
            }
        }

        public static XmlStructDef GetGXItemTemplateBasedOnAllExamples(List<byte[]> exampleDatas, int dataLength)
        {
            XmlStructDef fields = new XmlStructDef();



            for (int i = 0; i < dataLength && ((i + 4) < (dataLength - 1)); i += 4)
            {
                bool isThisAFloat = false;
                bool isThisAlwaysZero = true;
                bool isOptional = false;

                for (int j = 0; j < exampleDatas.Count; j++)
                {
                    byte[] bytesChecking = exampleDatas[j];

                    if (i >= bytesChecking.Length)
                    {
                        isOptional = true;
                        isThisAlwaysZero = false;
                        continue;
                    }

                    int asInt = BitConverter.ToInt32(bytesChecking, i);
                    float asFloat = BitConverter.ToSingle(bytesChecking, i);

                    if ((asFloat >= 0.0001f && asFloat < 1000000) || (asFloat <= -0.0001f && asFloat > -1000000))
                    {
                        isThisAFloat = true;
                        //break;
                    }

                    if (asInt != 0)
                        isThisAlwaysZero = false;
                }

                var f = new XmlStructDefField();

                if (isThisAFloat)
                {
                    //f.Name = $"_UnkFloat{i:X2}";
                    f.ValueType = "f32";
                }
                else
                {
                    //f.Name = $"_UnkInt{i:X2}";
                    f.ValueType = "s32";
                }

                if (isThisAlwaysZero)
                    f.AssertValue = 0;
                if (isOptional)
                    f.IsOptional = true;

                fields.Add(f);
            }

            void DoShort(int index)
            {
                bool isThisAlwaysZero = true;
                bool probablyUnsigned = false;
                bool isOptional = false;

                for (int j = 0; j < exampleDatas.Count; j++)
                {
                    byte[] bytesChecking = exampleDatas[j];

                    if (index >= bytesChecking.Length)
                    {
                        isOptional = true;
                        isThisAlwaysZero = false;
                        continue;
                    }

                    int asInt = BitConverter.ToInt16(bytesChecking, index);

                    if (asInt != 0)
                        isThisAlwaysZero = false;

                    if (asInt < -1)
                        probablyUnsigned = true;
                }

                var f = new XmlStructDefField();

                f.ValueType = probablyUnsigned ? "u16" : "s16";
                if (isThisAlwaysZero)
                    f.AssertValue = 0;
                if (isOptional)
                    f.IsOptional = true;

                fields.Add(f);
            }

            void DoByte(int index)
            {
                bool isThisAlwaysZero = true;
                bool probablySigned = false;
                bool isOptional = false;

                for (int j = 0; j < exampleDatas.Count; j++)
                {
                    byte[] bytesChecking = exampleDatas[j];

                    if (index >= bytesChecking.Length)
                    {
                        isOptional = true;
                        isThisAlwaysZero = false;
                        continue;
                    }

                    byte asByte = bytesChecking[index];


                    if (asByte != 0)
                        isThisAlwaysZero = false;

                    if (asByte == 255)
                        probablySigned = true;
                }

                var f = new XmlStructDefField();

                f.ValueType = probablySigned ? "s8" : "u8";
                if (isThisAlwaysZero)
                    f.AssertValue = 0;
                if (isOptional)
                    f.IsOptional = true;

                fields.Add(f);
            }

            int leftoverBytesStart = dataLength - (dataLength % 4);

            if (dataLength % 4 == 1)
            {
                DoByte(leftoverBytesStart);
            }
            else if (dataLength % 4 == 2)
            {
                DoShort(leftoverBytesStart);
            }
            else if (dataLength % 4 == 3)
            {
                DoShort(leftoverBytesStart);
                DoByte(leftoverBytesStart);
            }

            return fields;
        }

        /*
        public static void Main(string[] args)
        {
            // DEBUG
            //var thing = new FLVER2MaterialInfoBank();
            //thing.ReadXML(@"C:\DarkSoulsModding\_misc\SapFlverScan_Test\ds3_dump.xml");

            //Console.WriteLine("FATCAT");

            if (args.Length != 2)
            {
                USAGE();
                return;
            }

            if (!Directory.Exists(args[0]))
            {
                Console.WriteLine($"Specified directory [{args[0]}] does not exist.");
                return;
            }
            string scanDir = args[0];
            string xmlOutputFile = args[1];

            if (File.Exists(xmlOutputFile))
                File.Delete(xmlOutputFile);

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;

            int failedFlvers = 0;

            using (var writeStream = File.OpenWrite(xmlOutputFile))
            {
                using (var writer = XmlWriter.Create(writeStream, new XmlWriterSettings()
                {
                    Indent = true,
                    IndentChars = "  ",
                }))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("material_info_bank");

                    Console.WriteLine("Scanning all FLVERs...");

                    var flverFiles = Directory.GetFiles(scanDir, "*mapbnd.dcx", SearchOption.AllDirectories).ToList();
                    var partsFiles = Directory.GetFiles(scanDir, "*partsbnd.dcx", SearchOption.AllDirectories).ToList();
                    var chrFiles = Directory.GetFiles(scanDir, "*chrbnd.dcx", SearchOption.AllDirectories).ToList();
                    var geomFiles = Directory.GetFiles(scanDir, "*geombnd.dcx", SearchOption.AllDirectories).ToList();
                    var sfxFiles = Directory.GetFiles(scanDir, "sfxbnd*", SearchOption.AllDirectories).ToList();
                    flverFiles.AddRange(partsFiles);
                    flverFiles.AddRange(chrFiles);
                    flverFiles.AddRange(geomFiles);
                    flverFiles.AddRange(sfxFiles);

                    var instances = new List<InstanceofFLVER2Mesh>();

                    var exampleDataDict = new Dictionary<string, List<InstanceofFLVER2Mesh_ExampleData>>();

                    Dictionary<string, List<byte[]>> ALL_GX_ITEM_INSTANCES = new Dictionary<string, List<byte[]>>();

                    for (int i = 0; i < flverFiles.Count; i++)
                    {
                        string shortName = Path.GetFileName(flverFiles[i]);
                        Console.WriteLine($"  Scanning [{(i + 1):D5}/{flverFiles.Count:D5}]: '{shortName}'");

                        try
                        {
                            if (!BND4.IsRead(flverFiles[i], out BND4 bnd)) continue;
                            List<FLVER2?> flvers = bnd.Files.Where(x => Path.GetFileName(x.Name).Contains(".flv"))
                                .Select(x => FLVER2.IsRead(x.Bytes, out FLVER2 flver) ? flver : null).ToList();

                            foreach (FLVER2? flver in flvers)
                            {
                                if (flver is null)
                                {
                                    failedFlvers++;
                                    continue;
                                }
                                foreach (var mesh in flver.Meshes)
                                {
                                    var uniqueMaterialInstance = new InstanceofFLVER2Mesh(shortName, flver, mesh);
                                    if (!exampleDataDict.ContainsKey(uniqueMaterialInstance.MTD))
                                        exampleDataDict.Add(uniqueMaterialInstance.MTD, new List<InstanceofFLVER2Mesh_ExampleData>());
                                    exampleDataDict[uniqueMaterialInstance.MTD].Add(uniqueMaterialInstance.ExampleData);

                                    foreach (var gx in uniqueMaterialInstance.GXItems)
                                    {
                                        if (!ALL_GX_ITEM_INSTANCES.ContainsKey(gx.IDString))
                                            ALL_GX_ITEM_INSTANCES.Add(gx.IDString, new List<byte[]>());

                                        ALL_GX_ITEM_INSTANCES[gx.IDString].Add(gx.Example_Data);
                                    }

                                    var existingIndex = instances.IndexOf(uniqueMaterialInstance);

                                    if (existingIndex == -1)
                                    {
                                        instances.Add(uniqueMaterialInstance);
                                    }
                                    else
                                    {
                                        foreach (var dec in uniqueMaterialInstance.PossibleVertexBufferSetups)
                                        {
                                            if (!instances[existingIndex].PossibleVertexBufferSetups.Contains(dec))
                                                instances[existingIndex].PossibleVertexBufferSetups.Add(dec);
                                        }

                                    }
                                }
                            }
                        }
                        catch (Exception scanEx)
                        {
                            failedFlvers++;
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.WriteLine($"Scan of '{shortName}' failed:\n{scanEx}");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Black;
                        }
                    }

                    void WriteXmlOf5ExamplesOfAnMTD(string mtd)
                    {
                        if (exampleDataDict.ContainsKey(mtd))
                        {
                            writer.WriteStartElement("MATERIAL_INSTANCE_EXAMPLE");
                            {
                                writer.WriteAttributeString("mtd", mtd);
                                var chosenExamples = exampleDataDict[mtd].OrderBy(a => Guid.NewGuid()).Take(5).ToList();
                                foreach (var ce in chosenExamples)
                                    ce.WriteXML(writer);
                            }
                            writer.WriteEndElement();
                        }
                    }

                    writer.WriteStartElement("material_def_list");
                    {
                        foreach (var inst in instances)
                        {
                            writer.WriteStartElement("material_def");
                            {
                                inst.WriteUniqueStuffXML(writer);
                            }
                            writer.WriteEndElement();
                        }
                    }
                    writer.WriteEndElement();



                    writer.WriteStartElement("gx_item_struct_list");
                    {
                        foreach (var kvp in ALL_GX_ITEM_INSTANCES)
                        {
                            writer.WriteStartElement("gx_item_struct");
                            {
                                writer.WriteAttributeString("gxid", kvp.Key);

                                int length = -1;
                                foreach (var arr in kvp.Value)
                                {
                                    if (arr.Length > length)
                                        length = arr.Length;
                                }

                                var template = GetGXItemTemplateBasedOnAllExamples(kvp.Value, length);

                                foreach (var field in template)
                                {
                                    writer.WriteStartElement(field.ValueType);
                                    {
                                        if (field.Name != null)
                                            writer.WriteAttributeString("name", field.Name);
                                        if (field.AssertValue != null)
                                            writer.WriteAttributeString("assert", field.AssertValue.ToString());
                                        if (field.IsOptional)
                                            writer.WriteAttributeString("optional", "true");
                                    }
                                    writer.WriteEndElement();
                                }
                            }
                            writer.WriteEndElement();
                        }

                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("MATERIAL_INSTANCE_EXAMPLE_LIST");
                    {
                        foreach (var inst in instances)
                        {
                            WriteXmlOf5ExamplesOfAnMTD(inst.MTD);
                        }
                    }
                    writer.WriteEndElement();


                    //TESTING
                    //if (ALL_GX_ITEM_INSTANCES.ContainsKey("GXMD"))
                    //{
                    //    foreach (var gx in ALL_GX_ITEM_INSTANCES["GXMD"])
                    //    {
                    //        int itemCountTest = BitConverter.ToInt32(gx, 8);
                    //        if (itemCountTest != (gx.Length - 12) / 4)
                    //            Console.WriteLine("Length debunked");

                    //        for (int i = 12; i < gx.Length; i++)
                    //        {
                    //            float asFloat = BitConverter.ToSingle(gx, i);

                    //            if ((asFloat >= 0.0001f && asFloat < 1000000) || (asFloat <= -0.0001f && asFloat > -1000000))
                    //            {
                    //                if (asFloat != Math.Round(asFloat))
                    //                    Console.WriteLine("dynamic numbers debunked");
                    //            }
                    //        }
                    //    }
                    //}




                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            if (failedFlvers > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine($"WARNING: {failedFlvers} FLVERs failed to process.");
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
            }
            Console.WriteLine("Done. Press any key to exit.");
            Console.ReadKey();

        }
        */
    }
}
