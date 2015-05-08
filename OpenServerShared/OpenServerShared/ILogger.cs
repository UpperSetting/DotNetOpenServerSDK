using System;

namespace US.OpenServer
{
    /// <summary>
    /// Interface for logging implementations.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Gets or sets whether to log packets to the console when debugging.
        /// </summary>
        /// <value>A Boolean that specifies whether to log packets to the console when
        /// debugging.</value>
        bool LogPackets { get; set; }

        /// <summary>
        /// Log's a message.
        /// </summary>
        /// <param name="level">A Level that specifies the priority of the message.</param>
        /// <param name="message">A string that contains the message.</param>
        void Log(Level level, string message);

        /// <summary>
        /// Log's an exception.
        /// </summary>
        /// <param name="ex">An Exception to log.</param>
        void Log(Exception ex);
    }
}
