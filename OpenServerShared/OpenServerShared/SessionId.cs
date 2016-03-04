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
    /// Static class used to create a unique identifier that is associated with each
    /// socket connection.
    /// </summary>
    public static class SessionId
    {
        private static ushort nextId = 0;
        private static object synchObject = new object();

        /// <summary>
        /// Gets the next unique identifier.
        /// </summary>
        /// <value>A UInt16 that specifies the unique identifier.</value>
        public static ushort NextId
        {
            get
            {
                lock (synchObject)
                {
                    if (nextId == UInt16.MaxValue)
                        nextId = UInt16.MinValue;
                    else
                        nextId++;

                    return nextId;
                }
            }
        }
    }
}
