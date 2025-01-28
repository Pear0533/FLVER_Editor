using SoulsFormats;

namespace FLVER_Editor;


public interface IBNDWrapper
{
    public IBinder BND { get; }
    public Binder.Format Format { get; }
    public string Version { get; }
    public List<BinderFile> Files { get; }
    public DCX.Type Compression { get; }

    public void Write(string path, DCX.Type compression);

}

public class BND4Wrapper : IBNDWrapper
{
    public BND4Wrapper(BND4 bnd4)
    {
        BND4 = bnd4;
    }
    public BND4 BND4;

    public IBinder BND => BND4;

    public Binder.Format Format => BND4.Format;

    public string Version => BND4.Version;

    public List<BinderFile> Files => BND4.Files;
    public DCX.Type Compression => BND4.Compression;

    public void Write(string path, DCX.Type compression)
    {
        BND4.Write(path, compression);
    }
}

public class BND3Wrapper : IBNDWrapper
{
    public BND3Wrapper(BND3 bnd3)
    {
        BND3 = bnd3;
    }
    public BND3 BND3;

    public IBinder BND => BND3;

    public Binder.Format Format => BND3.Format;

    public string Version => BND3.Version;

    public List<BinderFile> Files => BND3.Files;
    public DCX.Type Compression => BND3.Compression;

    public void Write(string path, DCX.Type compression)
    {
        BND3.Write(path, compression);
    }
}

public enum FLVERType {
    FLVER0,
    FLVER2
}