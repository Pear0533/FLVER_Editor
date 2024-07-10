using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor.Actions;

public class ImportGhostModelAction : TransformAction
{
    private readonly Action<FLVER2?> refresher;
    private readonly FLVER2? oldGhostflver;
    private readonly FLVER2 newFlver;

    /// <summary>
    /// Removes the imported Flver
    /// </summary>
    /// <param name="flver"></param>
    /// <param name="refresher"></param>
    public ImportGhostModelAction(FLVER2? flver, FLVER2 newFlver, Action<FLVER2?> refresher)
    {
        oldGhostflver = flver;
        this.newFlver = newFlver;
        this.refresher = refresher;
    }

    public override void Execute()
    {
        foreach (var mesh in newFlver.Meshes)
        {
            mesh.MaterialIndex = 0;
        }

        refresher.Invoke(newFlver);
    }

    public override void Undo()
    {
        refresher.Invoke(oldGhostflver);
    }
}

public class RemoveGhostModelAction : TransformAction
{
    private readonly Action<FLVER2?> refresher;
    private readonly FLVER2? oldGhostflver;

    /// <summary>
    /// Removes the imported Flver
    /// </summary>
    /// <param name="flver"></param>
    /// <param name="refresher"></param>
    public RemoveGhostModelAction(FLVER2? flver, Action<FLVER2?> refresher)
    {
        oldGhostflver = flver;
        this.refresher = refresher;
    }

    public override void Execute()
    {
        refresher.Invoke(null);
    }

    public override void Undo()
    {
        refresher.Invoke(oldGhostflver);
    }
}
