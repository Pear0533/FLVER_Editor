using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FLVER_Editor.Program;

namespace FLVER_Editor.Actions;

public class UpdateTextureAction : TransformAction
{
    private Action<string> windowRefresh;
    private readonly string textureFilePath;

    private TPF.Texture? newTexture;
    private TPF.Texture? oldTexture;
    private int textureIndex;

    public UpdateTextureAction(string textureFilePath, Action<string> refresher)
    {
        this.textureFilePath = textureFilePath;
        windowRefresh = refresher;
    }

    public override void Execute()
    {
        if (Tpf == null) Tpf = new TPF();
        BinderFile? flverBndTpfEntry = MainWindow.FlverBnd.Files.FirstOrDefault(i => i.Name.EndsWith(".tpf"));

        if (flverBndTpfEntry is null) 
            return;

        byte[] ddsBytes = File.ReadAllBytes(textureFilePath);
        DDS dds = new(ddsBytes);
        byte formatByte = 107;
        try
        {
            formatByte = (byte)Enum.Parse(typeof(TextureFormats), dds.header10.dxgiFormat.ToString());
        }
        catch { }

        newTexture = new(Path.GetFileNameWithoutExtension(textureFilePath), formatByte, 0x00, File.ReadAllBytes(textureFilePath));
        textureIndex = Tpf.Textures.FindIndex(i => i.Name == newTexture.Name);

        if (textureIndex != -1)
        {
            oldTexture = Tpf.Textures[textureIndex];

            Tpf.Textures.RemoveAt(textureIndex);
            Tpf.Textures.Insert(textureIndex, newTexture);
        }
        else Tpf.Textures.Add(newTexture);

        MainWindow.FlverBnd.Files[MainWindow.FlverBnd.Files.IndexOf(flverBndTpfEntry)].Bytes = Tpf.Write();

        windowRefresh?.Invoke(textureFilePath);
    }

    public override void Undo()
    {
        if (oldTexture is null)
        {
            Tpf.Textures.Remove(newTexture);
        }
        else
        {
            Tpf.Textures.RemoveAt(textureIndex);
            Tpf.Textures.Insert(textureIndex, oldTexture);
        }

        windowRefresh?.Invoke(oldTexture.Name);


    }
}

