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