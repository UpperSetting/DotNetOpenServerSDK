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
    /// Enum of packet read states. Defines the current state of the socket reader
    /// while reading command packets from TCP packets.
    /// </summary>
    public enum PacketReadTypes
    {
        /// <summary>
        /// The initial state. A new command packet has been received. The socket reader is
        /// to read the header.
        /// </summary>
        Header,

        /// <summary>
        /// The socket reader has finished reading the header. The socket reader is to read
        /// the payload.
        /// </summary>
        HeaderComplete,

        /// <summary>
        /// The socket reader is currently reading the payload. The payload may be received
        /// over multiple TCP packets.
        /// </summary>
        Payload,
    }
}
