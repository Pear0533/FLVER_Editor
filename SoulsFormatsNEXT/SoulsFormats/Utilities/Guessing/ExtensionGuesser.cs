using System.IO;
using System.Text.RegularExpressions;

namespace SoulsFormats.Utilities
{
    /// <summary>
    /// Helps with guessing extensions.
    /// </summary>
    public static class ExtensionGuesser
    {
        /// <summary>
        /// Guesses the extension of a file based on its contents.
        /// </summary>
        public static string Guess(byte[] bytes, bool bigEndian = false)
        {
            bool dcx = false;
            if (DCX.Is(bytes))
            {
                dcx = true;
                bytes = DCX.Decompress(bytes);
            }

            bool checkMsb(BinaryReaderEx br)
            {
                if (br.Length < 8)
                    return false;

                int offset = br.GetInt32(4);
                if (offset < 0 || offset >= br.Length - 1)
                    return false;

                try
                {
                    return br.GetASCII(offset) == "MODEL_PARAM_ST";
                }
                catch
                {
                    return false;
                }
            }

            bool checkParam(BinaryReaderEx br)
            {
                if (br.Length < 0x2C)
                    return false;

                string param = br.GetASCII(0xC, 0x20);
                return Regex.IsMatch(param, "^[^\0]+\0 *$");
            }

            bool checkTdf(BinaryReaderEx br)
            {
                if (br.Length < 4)
                    return false;

                if (br.GetASCII(0, 1) != "\"")
                    return false;

                for (int i = 1; i < br.Length; i++)
                {
                    if (br.GetASCII(i, 1) == "\"")
                    {
                        return i < br.Length - 2 && br.GetASCII(i + 1, 2) == "\r\n";
                    }
                }
                return false;
            }

            string ext = "";
            using (var ms = new MemoryStream(bytes))
            {
                var br = new BinaryReaderEx(bigEndian, ms);
                string magic = null;
                if (br.Length >= 4)
                    magic = br.ReadASCII(4);

                if (magic == "AISD")
                    ext = ".aisd";
                else if (magic == "BDF3" || magic == "BDF4")
                    ext = ".bdt";
                else if (magic == "BHF3" || magic == "BHF4")
                    ext = ".bhd";
                else if (magic == "BND3" || magic == "BND4")
                    ext = ".bnd";
                else if (magic == "DDS ")
                    ext = ".dds";
                // ESD or FFX
                else if (magic != null && magic.ToUpper() == "DLSE")
                    ext = ".dlse";
                else if (bigEndian && magic == "\0BRD" || !bigEndian && magic == "DRB\0")
                    ext = ".drb";
                else if (magic == "EDF\0")
                    ext = ".edf";
                else if (magic == "ELD\0")
                    ext = ".eld";
                else if (magic == "ENFL")
                    ext = ".entryfilelist";
                else if (magic != null && magic.ToUpper() == "FSSL")
                    ext = ".esd";
                else if (magic == "EVD\0")
                    ext = ".evd";
                else if (br.Length >= 3 && br.GetASCII(0, 3) == "FEV" || br.Length >= 0x10 && br.GetASCII(8, 8) == "FEV FMT ")
                    ext = ".fev";
                else if (br.Length >= 6 && br.GetASCII(0, 6) == "FLVER\0")
                    ext = ".flver";
                else if (br.Length >= 3 && br.GetASCII(0, 3) == "FSB")
                    ext = ".fsb";
                else if (br.Length >= 3 && br.GetASCII(0, 3) == "GFX")
                    ext = ".gfx";
                else if (br.Length >= 0x19 && br.GetASCII(0xC, 0xE) == "ITLIMITER_INFO")
                    ext = ".itl";
                else if (br.Length >= 4 && br.GetASCII(1, 3) == "Lua")
                    ext = ".lua";
                else if (checkMsb(br))
                    ext = ".msb";
                else if (br.Length >= 0x30 && br.GetASCII(0x2C, 4) == "MTD ")
                    ext = ".mtd";
                else if (magic == "DFPN")
                    ext = ".nfd";
                else if (checkParam(br))
                    ext = ".param";
                else if (br.Length >= 4 && br.GetASCII(1, 3) == "PNG")
                    ext = ".png";
                else if (br.Length >= 0x2C && br.GetASCII(0x28, 4) == "SIB ")
                    ext = ".sib";
                else if (magic == "TAE ")
                    ext = ".tae";
                else if (checkTdf(br))
                    ext = ".tdf";
                else if (magic == "TPF\0")
                    ext = ".tpf";
                else if (magic == "#BOM")
                    ext = ".txt";
                else if (br.Length >= 5 && br.GetASCII(0, 5) == "<?xml")
                    ext = ".xml";
                // This is pretty sketchy
                else if (br.Length >= 0xC && br.GetByte(0) == 0 && br.GetByte(3) == 0 && br.GetInt32(4) == br.Length && br.GetInt16(0xA) == 0)
                    ext = ".fmg";
            }

            if (dcx)
                return ext + ".dcx";
            else
                return ext;
        }
    }
}
