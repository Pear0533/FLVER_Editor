using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulsAssetPipeline.XmlStructs
{
    public class XmlStructException : Exception
    {
        public XmlStructException()
            : base()
        {

        }

        public XmlStructException(string message)
            : base(message)
        {

        }

        public XmlStructException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
