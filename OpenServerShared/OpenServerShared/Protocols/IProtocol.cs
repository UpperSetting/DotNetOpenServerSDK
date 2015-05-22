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
using System.IO;
using US.OpenServer.Configuration;

namespace US.OpenServer.Protocols
{
    /// <summary>
    /// Interface for protocol implementations.
    /// </summary>
    public abstract class IProtocol : IDisposable
    {
        /// <summary>
        /// The connection session.
        /// </summary>
        protected SessionBase session;

        /// <summary>
        /// Initializes the class.
        /// </summary>
        /// <param name="session">A SessionBase that encapsulates the connection
        /// session.</param>
        ///<param name="pc">A ProtocolConfiguration that contains configuration
        ///properties.</param>
        /// <param name="userData">An object that may be used client applications to pass
        /// objects or data to client side protocol implementations.</param>
        public virtual void Initialize(SessionBase session, ProtocolConfiguration pc, object userData = null)
        {
            this.session = session;
        }

        /// <summary>
        /// Closes the protocol layers allowing for command packet transmission (e.g. a
        /// Keep-Alive.QUIT command packet) then releases associated resources.
        /// </summary>
        public virtual void Close()
        {
        }

        /// <summary>
        /// Immediately closes the protocol layers then releases associated resources.
        /// </summary>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// Handles received command packets.
        /// </summary>
        /// <param name="br">A BinaryReader that contains the command packet.</param>
        public virtual void OnPacketReceived(BinaryReader br)
        {
        }

        /// <summary>
        /// Handles remote protocol configuration errors.
        /// </summary>
        /// <param name="message">A String that contains the error message.</param>
        public virtual void OnErrorReceived(string message)
        {
        }
    }
}
