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


namespace US.OpenServer.Protocols.WinAuth
{
    /// <summary>
    /// Enumeration that defines the available commands.
    /// </summary>
    public enum WinAuthProtocolCommands : byte
    {
        /// <summary>
        /// Sent by clients to authenticate a user.
        /// </summary>
        AUTHENTICATE = 0x01,

        /// <summary>
        /// Sent by the Windows Server in response to a successful authentication
        /// request.
        /// </summary>
        AUTHENTICATED = 0x02,

        /// <summary>
        /// Sent by the Windows Server in response to a failed authentication attempt.
        /// </summary>
        ACCESS_DENIED = 0x03,

        /// <summary>
        /// Sent by the Windows Server when an authentication attempt errors or an
        /// invalid or unsupported command is received.
        /// </summary>
        ERROR = 0xFF,
    }
}
