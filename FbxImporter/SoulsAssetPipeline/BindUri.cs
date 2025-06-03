//using SoulsFormats;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SoulsAssetPipeline
//{
//    public class BindUri
//    {
//        public static IBinder ReadIBinder(string path)
//        {
//            if (BND3.Is(path))
//                return BND3.Read(path);
//            else if (BND4.Is(path))
//                return BND4.Read(path);

//        }

//        public enum BindIDTypes
//        {
//            Absolute,
//            ListRange
//        }
//        public class LooseFile : BindUri
//        {
//            public string Path;
//            public override byte[] Read()
//            {
//                return File.ReadAllBytes(Path);
//            }
//            public override void Write(byte[] data)
//            {
//                File.WriteAllBytes(Path, data);
//            }
//        }

//        public class BindedFile : BindUri
//        {
//            public BindUri BinderUri;
//            public BindIDTypes BindIDType;
//            public int BindID;

//            public override byte[] Read()
//            {
//                IBinder bnd = 
//            }
//            public override void Write(byte[] data)
//            {
//                base.Write(data);
//            }
//        }

//        public virtual byte[] Read()
//        {

//        }

//        public virtual void Write(byte[] data)
//        {

//        }
//    }
//}
