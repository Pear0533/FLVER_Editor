using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLVER_Editor;

public static class ActionManager
{
    public static Stack<TransformAction> UndoStack { get; } = new();
    public static Stack<TransformAction> RedoStack { get; } = new();

    public static void Apply(TransformAction action)
    {
        action.Execute();
        UndoStack.Push(action);
        RedoStack.Clear();

        MainWindow.Instance?.UpdateUndoState();
    }

    public static void Undo()
    {
        if (UndoStack.Count == 0)
        {
            return;
        }

        TransformAction action = UndoStack.Pop();
        action.Undo();
        RedoStack.Push(action);

        MainWindow.Instance?.UpdateRedoState();
    }

    public static void Redo()
    {
        if (RedoStack.Count == 0)
        {
            return;
        }

        TransformAction action = RedoStack.Pop();
        action.Execute();
        UndoStack.Push(action);

        MainWindow.Instance?.UpdateUndoState();
    }

    public static void Clear()
    {
        UndoStack.Clear();
        RedoStack.Clear();
    }
}
