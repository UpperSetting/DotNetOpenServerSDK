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

namespace System.Threading
{
    /// <summary>
    /// A static helper class the enables a thread to sleep for a specific period of
    /// time, manage drift and supports clock changes.
    /// </summary>
    public class ThreadHelper
    {
        /// <summary>
        /// Enables a thread to sleep for a specific period of time, manage drift and
        /// supports clock changes. This function enables the user to break from the
        /// call.
        /// </summary>
        /// <param name="startTicks">A long value that specifies the time to start sleeping.</param>
        /// <param name="intervalTicks">The long value that specifies the duration to sleep for.</param>
        /// <param name="signalStop">A reference to a bool that enables the caller to
        /// break out of this call from another thread.</param>
        /// <returns>A long value that specifies the value the caller should use for the
        /// startTicks parameters on subsequent calls into this function.</returns>
        public static long Sleep(long startTicks, long intervalTicks, ref bool signalStop)
        {
            long finalTicks = startTicks + intervalTicks;

            while (finalTicks + intervalTicks < DateTime.Now.Ticks)
            {
                finalTicks += intervalTicks;
            }

            while (!signalStop && DateTime.Now.Ticks < finalTicks)
            {
                while (finalTicks - intervalTicks > DateTime.Now.Ticks)
                {
                    finalTicks -= intervalTicks;
                }

                Thread.Sleep(1000);
            }
            return finalTicks;
        }
    }
}