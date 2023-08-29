using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Text;

namespace HKX2
{
    public interface IHavokObject
    {
        uint Signature { get; }

        void Read(PackFileDeserializer des, BinaryReaderEx br);

        void Write(PackFileSerializer s, BinaryWriterEx bw);
    }
}
