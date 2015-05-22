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

using System.IO;
using System.Text;

namespace US.OpenServer.Protocols.WinAuth
{
    /// <summary>
    /// Base abstract class for the Windows Authentication Protocol implementation.
    /// </summary>
    public abstract class WinAuthProtocol : AuthenticationProtocolBase
    {
        /// <summary>
        /// The unique protocol identifier
        /// </summary>
        public const ushort PROTOCOL_IDENTIFIER = 0x0002;

        /// <summary>
        /// Creates a WinAuthProtocol object.
        /// </summary>
        protected WinAuthProtocol()
        {
        }

        /// <summary>
        /// Creates the command packet's header and returns a BinaryWriter so the caller
        /// can write the command packet's payload.
        /// </summary>
        /// <param name="ms">A MemoryStream to write the command packet.</param>
        /// <param name="command">A WinAuthProtocolCommands that specifies the command.</param>
        /// <returns>A BinaryWriter that contains the command packet's header.</returns>
        protected BinaryWriter GetBinaryWriter(MemoryStream ms, WinAuthProtocolCommands command)
        {
            BinaryWriter bw = new BinaryWriter(ms, Encoding.UTF8);
            bw.Write(WinAuthProtocol.PROTOCOL_IDENTIFIER);
            bw.Write((byte)command);
            return bw;
        }

        /// <summary>
        /// Log's a message.
        /// </summary>
        /// <param name="level">A Level that specifies the priority of the message.</param>
        /// <param name="message">A string that contains the message.</param>
        protected override void Log(Level level, string message)
        {
            session.Log(level, string.Format("[WinAuth] {0}", message));
        }
    }
}
