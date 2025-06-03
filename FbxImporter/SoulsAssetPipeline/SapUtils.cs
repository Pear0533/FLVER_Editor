using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SoulsAssetPipeline
{
    public static class SapUtils
    {
        private static string _assemblyDirectory = null;
        public static string AssemblyDirectory
        {
            get
            {
                if (_assemblyDirectory == null)
                    _assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                return _assemblyDirectory;
            }
        }
    }
}
