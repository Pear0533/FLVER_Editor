namespace FLVER_Editor.FbxImporter.ViewModels;

public class FbxVertexData
{
    public float[] Position;
    public float[] Normal;
    public List<float[]> Tangents;
    public List<float[]> UVs;
    public string[] BoneNames;
    public float[] BoneWeights;

    public FbxVertexData(float[] position, float[] normal, List<float[]> tangents, List<float[]> uvs, string[] boneNames, float[] boneWeights)
    {
        Position = position;
        Normal = normal;
        Tangents = tangents;
        UVs = uvs;
        BoneNames = boneNames;
        BoneWeights = boneWeights;
    }
}