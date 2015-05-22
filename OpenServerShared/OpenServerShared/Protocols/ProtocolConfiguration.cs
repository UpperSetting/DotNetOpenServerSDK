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

namespace US.OpenServer.Protocols
{
    /// <summary>
    /// Class that encapsulates the properties necessary for Reflection to load
    /// ProtocolBase classes.
    /// </summary>
    public class ProtocolConfiguration
    {
        /// <summary>
        /// Gets or sets the protocol identifier.
        /// </summary>
        /// <value>A UInt16 that specifies the protocol identifier.</value>
        public ushort Id { get; protected set; }

        /// <summary>
        /// Gets or sets the class type of the ProtocolBase.
        /// </summary>
        /// <value>A Type that specifies the type of the ProtocolBase.</value>
        public Type ProtocolType { get; protected set; }

        /// <summary>
        /// Creates an instance of ProtocolConfiguration.
        /// </summary>
        protected ProtocolConfiguration()
        {
        }

        /// <summary>
        /// Creates an instance of ProtocolConfiguration given the protocol identifier
        /// and class Type.
        /// </summary>
        /// <param name="id">A UInt16 that specifies the protocol identifier.</param>
        /// <param name="protocolType">A Type that specifies the protocol class. The class
        /// must extend ProtocolBase.</param>
        public ProtocolConfiguration(ushort id, Type protocolType)
        {
            Id = id;
            ProtocolType = protocolType;
        }

        /// <summary>
        /// Creates an instances of the ProtocolBase class.
        /// </summary>
        /// <returns>A ProtocolBase.</returns>
        public virtual ProtocolBase CreateInstance()
        {
            return ProtocolType != null ? 
                (ProtocolBase)Activator.CreateInstance(ProtocolType) : null;
        }
    }
}
