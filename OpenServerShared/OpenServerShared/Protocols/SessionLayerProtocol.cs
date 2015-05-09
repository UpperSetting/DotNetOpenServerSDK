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


namespace US.OpenServer.Protocols
{
    /// <summary>
    /// Static class that defines constants for the Session Layer Protocol.
    /// </summary>
    public static class SessionLayerProtocol
    {
        /// <summary>
        /// The protocol identifier.
        /// </summary>
        public const ushort PROTOCAL_IDENTIFIER = 21843;//U 0x55, S 0x53

        /// <summary>
        /// The length of the protocol identifier.
        /// </summary>
        public const int PROTOCOL_IDENTIFIER_LENGTH = 2;

        /// <summary>
        /// The length of the length field.
        /// </summary>
        public const int LENGTH_LENGTH = 4;

        /// <summary>
        /// The header length.
        /// </summary>
        public const int HEADER_LENGTH = PROTOCOL_IDENTIFIER_LENGTH + LENGTH_LENGTH;

        /// <summary>
        /// The default server port.
        /// </summary>
        public const ushort PORT = 21843;
    }
}
