using SoulsFormats;

namespace FLVER_Editor.Actions;

public class MergeFlversAction : TransformAction
{
    private readonly string _flverFilePath;
    private readonly string _newFlverFilePath;
    private readonly FLVER2 flver;
    private readonly int layoutOffset;
    private readonly int materialOffset;
    private readonly int meshOffset;
    private readonly FLVER2 newFlver;
    private readonly Action refresher;

    public MergeFlversAction(FLVER2 currentFlver, FLVER2 newFlver, string flverFilePath, string newFlverFilePath, Action refresher)
    {
        materialOffset = currentFlver.Materials.Count;
        meshOffset = currentFlver.Meshes.Count;
        layoutOffset = currentFlver.BufferLayouts.Count;
        flver = currentFlver;
        _flverFilePath = flverFilePath;
        _newFlverFilePath = newFlverFilePath;
        this.newFlver = newFlver;
        this.refresher = refresher;
    }

    public override void Execute()
    {
        Dictionary<int, int> newFlverToCurrentFlver = new();
        for (int i = 0; i < newFlver.Nodes.Count; ++i)
        {
            FLVER.Node attachBone = newFlver.Nodes[i];
            for (int j = 0; j < flver.Nodes.Count; ++j)
            {
                if (attachBone.Name != flver.Nodes[j].Name) continue;
                newFlverToCurrentFlver.Add(i, j);
                break;
            }
        }
        foreach (FLVER2.Mesh m in newFlver.Meshes)
        {
            m.MaterialIndex += materialOffset;
            foreach (FLVER2.VertexBuffer vb in m.VertexBuffers)
                vb.LayoutIndex += layoutOffset;
            foreach (FLVER.Vertex v in m.Vertices.Where(v => Util3D.BoneIndicesToIntArray(v.BoneIndices) != null))
            {
                for (int i = 0; i < v.BoneIndices.Length; ++i)
                {
                    if (newFlverToCurrentFlver.ContainsKey(v.BoneIndices[i])) v.BoneIndices[i] = newFlverToCurrentFlver[v.BoneIndices[i]];
                }
            }
        }
        foreach (FLVER2.Material material in newFlver.Materials)
            material.GXIndex += flver.GXLists.Count;
        flver.BufferLayouts = flver.BufferLayouts.Concat(newFlver.BufferLayouts).ToList();
        flver.Meshes = flver.Meshes.Concat(newFlver.Meshes).ToList();
        flver.Materials = flver.Materials.Concat(newFlver.Materials).ToList();
        flver.GXLists = flver.GXLists.Concat(newFlver.GXLists).ToList();

        // TODO: WIP
        TPF newFlverTpf = new();
        if (_newFlverFilePath.EndsWith(".dcx"))
        {
            BND4? newFlverBnd = BND4.Read(_newFlverFilePath);
            BinderFile? newFlverTpfEntry = newFlverBnd.Files.Find(i => i.Name.EndsWith(".tpf"));
            if (newFlverTpfEntry != null) newFlverTpf = TPF.Read(newFlverTpfEntry.Bytes);
        }
        else if (_newFlverFilePath.EndsWith(".flver"))
        {
            newFlverTpf = TPF.Read(_newFlverFilePath.Replace(".flver", ".tpf"));
        }
        if (Program.Tpf == null)
            Program.Tpf = TPF.Read(_flverFilePath.Replace(".flver", ".tpf"));
        foreach (TPF.Texture tex in newFlverTpf)
        {
            if (Program.Tpf.Textures.All(i => i.Name != tex.Name))
                MainWindow.InjectTextureIntoTPF(tex);
        }
        refresher.Invoke();
    }

    public override void Undo()
    {
        foreach (FLVER2.Mesh m in newFlver.Meshes)
        {
            m.MaterialIndex -= materialOffset;
            foreach (FLVER2.VertexBuffer vb in m.VertexBuffers)
                vb.LayoutIndex -= layoutOffset;
        }
        flver.BufferLayouts.RemoveRange(layoutOffset, flver.BufferLayouts.Count - layoutOffset);
        flver.Meshes.RemoveRange(meshOffset, flver.Meshes.Count - meshOffset);
        flver.Materials.RemoveRange(materialOffset, flver.Materials.Count - materialOffset);
        refresher.Invoke();
    }
}