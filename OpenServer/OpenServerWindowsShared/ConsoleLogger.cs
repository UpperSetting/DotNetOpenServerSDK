/*
Copyright 2015 Upper Setting Corporation

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
    /// A simple ILogger class that logs to the console.
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        /// <summary>
        /// Gets or sets whether to log packets in hexadecimal format.
        /// </summary>
        public bool LogPackets { get; set; }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="level">The level of the message.</param>
        /// <param name="message">The message.</param>
        public void Log(Level level, string message)
        {
            Console.WriteLine(string.Format("{0} {1}", level, message));
        }
        
        /// <summary>
        /// Logs an <see cref="US.OpenServer.Level.Error"/> message given an <see cref="System.Exception"/>.
        /// Writes the exception's message, a carriage return line feed, then the
        /// exception's stack trace.
        /// </summary>
        /// <param name="ex">The <see cref="System.Exception"/> to log.</param>
        public void Log(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
