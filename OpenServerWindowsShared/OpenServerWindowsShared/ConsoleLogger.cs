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
    /// Class that logs to the console.
    /// </summary>
    public class ConsoleLogger : Logger
    {
        #region Public Functions
        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="level">The level of the message.</param>
        /// <param name="message">The message.</param>
        public override void Log(Level level, string message)
        {
            base.Log(level, message);

            if (level == Level.Debug && !LogDebug)
                return;

            Console.WriteLine(string.Format("{0} {1}", level, message));
        }
        #endregion
    }
}
