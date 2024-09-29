namespace FLVER_Editor;

// Should be developed further for better structuring of data
public static class BasicLogger
{
    static BasicLogger()
    {
        if (!Directory.Exists(DirectoryPath))
        {
            Directory.CreateDirectory(DirectoryPath);
        }

        if (!File.Exists(LogPath))
        {
            var handle = File.Create(LogPath);
            handle.Dispose();
        }
    }

    public static string DirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Flver2");
    public static string LogPath = Path.Combine(DirectoryPath, "history.log");
    public static void LogInfo(string Message)
    {
        File.AppendAllText(LogPath, "[INFO] " + Message + "\n");
    }

    public static void LogWarning(string Message)
    {
        File.AppendAllText(LogPath, "[WARNING] " + Message + "\n");
    }

    public static void LogError(this Exception error)
    {
        File.AppendAllText(LogPath, "[ERROR] " + error.Message + error.StackTrace + "\n");
    }

    public static void LogError(string Message)
    {
        File.AppendAllText(LogPath, "[ERROR] " + Message + "\n");
    }
}