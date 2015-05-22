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


namespace US.OpenServer.Protocols.Hello
{
    /// <summary>
    /// Base abstract class for the Hello Protocol implementation.
    /// </summary>
    public abstract class HelloProtocol : ProtocolBase
    {
        /// <summary>
        /// The unique protocol identifier
        /// </summary>
        public const ushort PROTOCOL_IDENTIFIER = 0x000A;

        /// <summary>
        /// Creates a HelloProtocol object.
        /// </summary>
        protected HelloProtocol()
        {
        }

        /// <summary>
        /// Log's a message.
        /// </summary>
        /// <param name="level">A Level that specifies the priority of the message.</param>
        /// <param name="message">A string that contains the message.</param>
        protected override void Log(Level level, string message)
        {
            session.Log(level, string.Format("[Hello] {0}", message));
        }
    }
}
