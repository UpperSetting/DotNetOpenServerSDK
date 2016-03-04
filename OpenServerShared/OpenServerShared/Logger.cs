/*
Copyright 2015-2016 Upper Setting Corporation

This file is part of DotNetOpenServer SDK.

DotNetOpenServer SDK is free software: you can redistribute it and/or modify it
under the terms of the GNU General Public License as published by the Free
Software Foundation, either version 3 of the License, or (at your option) any
later version.

DotNetOpenServer SDK is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
details.

You should have received a copy of the GNU General Public License along with
DotNetOpenServer SDK. If not, see <http://www.gnu.org/licenses/>.
*/

using System;

namespace US.OpenServer
{
    /// <summary>
    /// Class for logging messages.
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// Delegate that defines the event callback for the <see cref="OnLogMessage"/>
        /// event.
        /// </summary>
        /// <param name="level">A Level that specifies the priority of the message.</param>
        /// <param name="message">A string that contains the message.</param>
        public delegate void OnLogMessageDelegate(Level level, string message);

        /// <summary>
        /// Event that is triggered when a message is logged.
        /// </summary>
        /// <remarks>This event was included to enabled users to display messages in a
        /// list within the user interface. </remarks>
        public event OnLogMessageDelegate OnLogMessage;

        /// <summary>
        /// Gets or sets whether to log <see cref="Level.Debug"/> messages.
        /// </summary>
        /// <remarks>Automatically enabled when run in DEBUG mode.</remarks>
        /// <value>A Boolean that specifies whether to log <see cref="Level.Debug"/>
        /// messages.</value>
        public bool LogDebug { get; set; }

        /// <summary>
        /// Gets or sets whether to log packets.
        /// </summary>
        /// <remarks>The connection session uses this value to determine if packets
        /// should be logged. Packets are logged in hexadecimal.</remarks>
        /// <value>A Boolean that specifies whether to log packets.</value>
        public bool LogPackets { get; set; }

        /// <summary>
        /// Gets or sets whether to log to the Visual Studio debugger output view.
        /// </summary>
        /// <remarks>Automatically enabled when run in DEBUG mode.</remarks>
        /// <value>A Boolean that specifies whether to log to the Visual Studio debugger
        /// output view.</value>
        public bool LogToDebuggerOutputView { get; set; }

        /// <summary>
        /// Creates a Logger object.
        /// </summary>
        /// <remarks>When run in DEBUG mode, automatically enables <see cref="LogDebug"/>
        /// and <see cref="LogToDebuggerOutputView"/>. </remarks>
        public Logger()
        {
#if DEBUG
            LogDebug = true;            
            LogToDebuggerOutputView = true;
#endif
        }

        /// <summary>
        /// Log's a message.
        /// </summary>
        /// <param name="level">A Level that specifies the priority of the message.</param>
        /// <param name="message">A String that contains the message.</param>
        public virtual void Log(Level level, string message)
        {
            if (level == Level.Debug && !LogDebug)
                return;

            if (LogToDebuggerOutputView)
                System.Diagnostics.Debug.WriteLine(string.Format("{0} {1}", level, message));

            if (OnLogMessage != null)
                OnLogMessage(level, message);
        }

        /// <summary>
        /// Log's an exception.
        /// </summary>
        /// <param name="ex">An Exception to log.</param>
        public virtual void Log(Exception ex)
        {
            Log(Level.Error, string.Format("{0}\r\n{1}", ex.Message, ex.StackTrace));
        }
    }
}
