using System.IO;

namespace SoulsFormats.Utilities
{
    /// <summary>
    /// Helper methods for pathing.
    /// </summary>
    public static class PathHelper
    {
        /// <summary>
        /// Makes a backup of a file if not already found, and returns the backed-up path.
        /// </summary>
        public static string Backup(string file, bool overwrite = false)
        {
            string bak = file + ".bak";
            if (overwrite || !File.Exists(bak))
                File.Copy(file, bak, overwrite);
            return bak;
        }

        /// <summary>
        /// Returns the extension of the specified file path, removing .dcx if present.
        /// </summary>
        public static string GetRealExtension(string path)
        {
            string extension = Path.GetExtension(path);
            if (extension == ".dcx")
                extension = Path.GetExtension(Path.GetFileNameWithoutExtension(path));
            return extension;
        }

        /// <summary>
        /// Returns the file name of the specified path, removing both .dcx if present and the actual extension.
        /// </summary>
        public static string GetRealFileName(string path)
        {
            string name = Path.GetFileNameWithoutExtension(path);
            if (Path.GetExtension(path) == ".dcx")
                name = Path.GetFileNameWithoutExtension(name);
            return name;
        }
    }
}
