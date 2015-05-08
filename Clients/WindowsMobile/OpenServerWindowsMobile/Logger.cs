using System;

namespace US.OpenServer.WindowsMobile
{
    public class Logger : ILogger
    {
        private static object synchObj = new object();

        public delegate void OnLogMessageDelegate(Level level, string message);
        public event OnLogMessageDelegate OnLogMessage;

        public bool LogDebug { get; set; }
        public bool LogPackets { get; set; }

        #region Public Functions
        public void Log(Level level, string message)
        {
            if (level == Level.Debug && !LogDebug)
                return;

            System.Diagnostics.Debug.WriteLine(string.Format("{0} {1}", level, message));

            if (OnLogMessage != null)
                OnLogMessage(level, message);
        }

        public void Log(Exception ex)
        {
            Log(Level.Error, string.Format("{0}\r\n{1}", ex.Message, ex.StackTrace));
        }
        #endregion
    }
}