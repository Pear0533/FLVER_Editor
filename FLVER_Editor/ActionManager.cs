using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FLVER_Editor;

public static class ActionManager
{
    public static Stack<TransformAction> UndoStack { get; } = new();
    public static Stack<TransformAction> RedoStack { get; } = new();

    public static void Apply(TransformAction action)
    {
        try
        {
            action.Execute();
            UndoStack.Push(action);
            RedoStack.Clear();

            MainWindow.Instance?.UpdateUndoState();
        }
        catch (Exception e)
        {
            e.LogError();
            throw;
        }
    }

    public static void Undo()
    {
        if (UndoStack.Count == 0)
        {
            return;
        }

        try
        {
            TransformAction action = UndoStack.Pop();
            action.Undo();
            RedoStack.Push(action);

            MainWindow.Instance?.UpdateRedoState();
        }
        catch (Exception e)
        {
            e.LogError();
            throw;
        }

    }

    public static void Redo()
    {
        if (RedoStack.Count == 0)
        {
            return;
        }
        try
        {
            TransformAction action = RedoStack.Pop();
            action.Execute();
            UndoStack.Push(action);

            MainWindow.Instance?.UpdateUndoState();
        }
        catch (Exception e)
        {
            e.LogError();
            throw;
        }
    }

    public static void Clear()
    {
        UndoStack.Clear();
        RedoStack.Clear();
    }
}
