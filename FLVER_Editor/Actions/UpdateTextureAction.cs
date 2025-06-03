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
    private readonly IBNDWrapper? flverBnd;
    private readonly string flverFilePath;
    private readonly string textureFilePath;
    private readonly string oldfilename;
    private TPF.Texture? newTexture;
    private TPF.Texture? oldTexture;
    private TPF.Texture? replacedTexture;
    private int textureIndex;

    public UpdateTextureAction(IBNDWrapper? flverBnd, string flverFilePath, string textureFilePath, string oldfilename, TPF.Texture? newTexture, Action<string> refresher)
    {
        this.flverBnd = flverBnd;
        this.flverFilePath = flverFilePath;
        this.textureFilePath = textureFilePath;
        this.oldfilename = oldfilename;
        this.newTexture = newTexture;
        windowRefresh = refresher;
    }

    public override void Execute()
    {
        oldTexture = null;
        replacedTexture = null;

        Tpf ??= new TPF();
        BinderFile? flverBndTpfEntry = flverBnd?.Files.FirstOrDefault(i => i.Name.EndsWith(".tpf"));

        if (textureFilePath != "")
        {
            byte[] ddsBytes = File.ReadAllBytes(textureFilePath);
            DDS dds = new(ddsBytes);
            byte formatByte = 107;
            try
            {
                formatByte = (byte)Enum.Parse(typeof(TextureFormats), dds.header10.dxgiFormat.ToString());
            }
            catch { }
            newTexture = new(Path.GetFileNameWithoutExtension(textureFilePath), formatByte, 0x00, File.ReadAllBytes(textureFilePath), TPF.TPFPlatform.PC);
        }

        textureIndex = Tpf.Textures.FindIndex(i => i.Name == newTexture?.Name);

        var oldTextureIndex = Tpf.Textures.FindIndex(i => i.Name == oldfilename);

        if (oldTextureIndex != -1)
        {
            oldTexture = Tpf.Textures[oldTextureIndex];
        }

        if (textureIndex != -1)
        {
            replacedTexture = Tpf.Textures[textureIndex];

            Tpf.Textures.RemoveAt(textureIndex);
            Tpf.Textures.Insert(textureIndex, newTexture);
        }
        else Tpf.Textures.Add(newTexture);

        if (flverBndTpfEntry is not null)
        {
            flverBnd!.Files[flverBnd.Files.IndexOf(flverBndTpfEntry)].Bytes = Tpf.Write();
        }
        else
        {
            SaveTPF();
        }

        windowRefresh?.Invoke(textureFilePath);
    }

    private void SaveTPF()
    {
        if (flverFilePath.Contains(".flver")) Tpf.Write(RemoveIndexSuffix(flverFilePath).Replace(".flver", ".tpf"));
        else if (flverFilePath.Contains(".flv")) Tpf.Write(RemoveIndexSuffix(flverFilePath).Replace(".flv", ".tpf"));
    }

    public override void Undo()
    {
        Tpf ??= new TPF();
        BinderFile? flverBndTpfEntry = flverBnd?.Files.FirstOrDefault(i => i.Name.EndsWith(".tpf"));

        if (oldTexture is null)
        {
            Tpf.Textures.Remove(newTexture);
        }
        else
        {
            Tpf.Textures.RemoveAt(textureIndex);
            Tpf.Textures.Insert(textureIndex, replacedTexture);
        }

        if (flverBndTpfEntry is not null)
        {
            flverBnd!.Files[flverBnd.Files.IndexOf(flverBndTpfEntry)].Bytes = Tpf.Write();
        }
        else
        {
            SaveTPF();
        }

        windowRefresh?.Invoke(oldTexture?.Name ?? oldfilename ?? "");
    }
}

