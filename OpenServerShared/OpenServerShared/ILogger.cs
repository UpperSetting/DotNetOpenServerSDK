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
