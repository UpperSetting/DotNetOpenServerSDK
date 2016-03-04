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


namespace US.OpenServer
{
    /// <summary>
    /// Static class that defines error strings.
    /// </summary>
    public static class ErrorTypes
    {
        /// <summary>
        /// The specified protocol has not been configured.
        /// </summary>
        public const string INVALID_PROTOCOL = "Invalid or unsupported protocol.  Protocol ID: {0}";

        /// <summary>
        /// The specified protocol has been configured, however; the class can not be
        /// found.
        /// </summary>
        public const string CLASS_NOT_FOUND = "Unable to create protocol layer.  Class not found.  Class: {0}";

        /// <summary>
        /// The socket connection has been lost.
        /// </summary>
        public const string CONNECTION_LOST = "Connection lost.  {0}";

        /// <summary>
        /// The remote connection sent a command packet for a protocol prior to
        /// authenticating.
        /// </summary>
        public const string NOT_AUTHENTICATED = "Unable to add protocol layer.  Not Authenticated.  Class: {0}";

        /// <summary>
        /// A new command packet has been received, however; there are no more threads
        /// available to handle the packet.
        /// </summary>
        public const string NO_MORE_THREADS_AVAILABLE = "No more threads available.";
    }
}
