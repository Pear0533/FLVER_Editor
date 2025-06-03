using System.Collections.ObjectModel;

namespace FbxImporter;

public class Logger
{
    private readonly ObservableCollection<string> _lines = new();

    private static Logger? _instance;
    private static readonly object InstanceLock = new();

    public ObservableCollection<string> Lines => _lines;

    public static Logger Instance
    {
        get {
            if (_instance is null) { lock (InstanceLock) { _instance = new Logger();}} 
            return _instance;
        }
    }

    private void LogInstance(string message)
    {
        _lines.Add(message);
    }
    
    public static void Log(string message)
    { 
        lock (InstanceLock)
        {
            Instance.LogInstance(message);
        }
    }
    
    public static void LogWarning(string message)
    { 
        Log($"Warning: {message}");
    }

    public static void LogError(string message)
    {
        Log($"Error: {message}");
    }
}