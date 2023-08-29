using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulsAssetPipeline
{
    public class SapLogger
    {
        public class SapLogEventArgs : EventArgs
        {
            public VerboseLevels Verbosity;
            public string Message;
            public SapLogEventArgs(VerboseLevels verbosity, string msg)
            {
                Verbosity = verbosity;
                Message = msg;
            }
        }
        public enum VerboseLevels
        {
            None = 0,
            Error = 1,
            Warning = 2,
            Info = 3,
        }

        public VerboseLevels Verbosity = VerboseLevels.Warning;

        public event EventHandler<SapLogEventArgs> MessageLogged;

        public void Log(VerboseLevels verbosity, string message)
        {
            if (verbosity > Verbosity)
                return;

            MessageLogged?.Invoke(this, new SapLogEventArgs(verbosity, message));
        }

        public void LogInfo(string message) => Log(VerboseLevels.Info, message);
        public void LogWarning(string message) => Log(VerboseLevels.Warning, message);
        public void LogError(string message) => Log(VerboseLevels.Error, message);
    }
}
