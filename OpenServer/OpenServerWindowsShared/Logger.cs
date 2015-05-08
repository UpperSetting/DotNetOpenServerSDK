using System;
using log4net.Core;

namespace US.OpenServer
{
    /// <summary>
    /// Class that wraps and extends a log4net.ILog object around an <see cref="US.OpenServer.ILogger"/>
    /// interface.
    /// </summary>
    public class Logger : ILogger
    {
        #region Private Variables
        /// <summary>
        /// The Log4Net logger.
        /// </summary>
        private log4net.ILog log;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets whether to log debug messages.
        /// </summary>
        public bool LogDebug { get; set; }

        /// <summary>
        /// Gets or sets whether to log packets in hexadecimal format.
        /// </summary>
        public bool LogPackets { get; set; }
        #endregion

        #region Construction/Destruction
        /// <summary>
        /// Creates a Logger object.
        /// </summary>
        /// <remarks> When run within the debugger, automatically enables <see cref="LogDebug"/>
        /// and <see cref="LogPackets"/>. Logs a message stating the application has
        /// started and then another message showing the name of the user the process is
        /// running as.</remarks>
        /// <param name="name">A string that specifies the name of the application.</param>
        public Logger(string name)
        {
            if (log == null)
            {
                log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#if DEBUG 
                LogDebug = true;
                LogPackets = true;
#endif
                Log(Level.Info, string.Format("{0} started", name));
                Log(Level.Info, string.Format("Running as {0}", Environment.UserName));
            }
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="level">The level of the message.</param>
        /// <param name="message">The message.</param>
        public void Log(US.OpenServer.Level level, string message)
        {
            if (level == US.OpenServer.Level.Debug && !LogDebug)
                return;

            if (log == null)
                return;
            
            LoggingEventData d = new LoggingEventData();
            d.TimeStamp = DateTime.Now;
            d.Level = GetLevelFromLevel(level);
            d.Message = message;
            LoggingEvent le = new LoggingEvent(d);
            log.Logger.Log(le);
        }

        /// <summary>
        /// Logs an <see cref="US.OpenServer.Level.Error"/> message given an <see cref="System.Exception"/>.
        /// Writes the exception's message, a carriage return line feed, then the
        /// exception's stack trace.
        /// </summary>
        /// <param name="ex">The <see cref="System.Exception"/> to log.</param>
        public void Log(Exception ex)
        {
            Log(Level.Error, string.Format("{0}\r\n{1}", ex.Message, ex.StackTrace));
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// Converts a <see cref="US.OpenServer.Level"/> enumeration to log4net.Core.Level object.
        /// </summary>
        /// <param name="level">The Level to convert.</param>
        /// <returns>A log4net.Core.Level object.</returns>
        private log4net.Core.Level GetLevelFromLevel(US.OpenServer.Level level)
        {
            switch (level)
            {
                case US.OpenServer.Level.Debug:
                    return log4net.Core.Level.Debug;
                case US.OpenServer.Level.Info:
                    return log4net.Core.Level.Info;
                case US.OpenServer.Level.Notice:
                    return log4net.Core.Level.Notice;
                case US.OpenServer.Level.Critical:
                    return log4net.Core.Level.Critical;
                case US.OpenServer.Level.Error:
                    return log4net.Core.Level.Error;
                default:
                    return log4net.Core.Level.Info;
            }
        }
        #endregion
    }
}