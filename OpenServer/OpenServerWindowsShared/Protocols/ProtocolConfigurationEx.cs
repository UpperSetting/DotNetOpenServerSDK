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
using System.Reflection;
using System.Xml;

namespace US.OpenServer.Protocols
{
    /// <summary>
    /// Class that contains the required properties for Reflection to
    /// dynamically load objects that implement the <see cref="US.OpenServer.Protocols.IProtocol"/>
    /// interface.
    /// </summary>
    public class ProtocolConfigurationEx : ProtocolConfiguration
    {
        /// <summary>
        /// Gets or sets the assembly the <see cref="US.OpenServer.Protocols.IProtocol"/>
        /// class is contained.
        /// </summary>
        /// <value>A string that specifies the assembly the <see cref="US.OpenServer.Protocols.IProtocol"/>
        /// class is contained.</value>
        public string Assembly { get; protected set; }

        /// <summary>
        /// Gets or sets the class path the <see cref="US.OpenServer.Protocols.IProtocol"/>
        /// class is contained.
        /// </summary>
        /// <value>A string that specifies the class path the <see cref="US.OpenServer.Protocols.IProtocol"/>
        /// class is contained.</value>
        public string ClassPath { get; protected set; }

        /// <summary>
        /// Creates an instance of ProtocolConfigurationEx when this class is extended.
        /// </summary>
        /// <remarks>
        /// This constructor is called by <see cref="US.OpenServer.Protocols.ProtocolConfigurationSectionHandler"/>
        /// when loading protocol configurations from the app.config file and the
        /// app.config file includes a configSectionAssemply and configSectionClassPath
        /// within the protocols/item section.
        /// </remarks>
        protected ProtocolConfigurationEx()
            : base ()
        {
        }

        /// <summary>
        /// Creates an instance of ProtocolConfigurationEx given the protocol identifier
        /// and class Type.
        /// </summary>
        /// <remarks>
        /// This constructor is called by <see cref="US.OpenServer.Protocols.ProtocolConfigurationSectionHandler"/>
        /// when programatically loading.
        /// </remarks>
        /// <param name="id">A UInt16 that specifies the protocol identifier.</param>
        /// <param name="protocolType">A Type that specifies the protocol class. The class
        /// must extend IProtocol.</param>
        protected ProtocolConfigurationEx(ushort id, Type protocolType)
            : base(id, protocolType)
        {
        }

        /// <summary>
        /// Creates an instance of ProtocolConfigurationEx given the protocol identifier,
        /// assembly name, and class path.
        /// </summary>
        /// <remarks>
        /// This constructor is called by <see cref="US.OpenServer.Protocols.ProtocolConfigurationSectionHandler"/>
        /// when loading from the app.config file.
        /// </remarks>
        /// <param name="id">A UInt16 that specifies the protocol identifier.</param>
        /// <param name="assembly">A string that specifies the assembly the class is
        /// contained.</param>
        /// <param name="classPath">A string that specifies the full path to the class. The
        /// class must extend IProtocol.</param>
        public ProtocolConfigurationEx(ushort id, string assembly, string classPath)
        {
            Id = id;
            Assembly = assembly;
            ClassPath = classPath;
        }

        /// <summary>
        /// Initializes an extended ProtocolConfigurationEx class. 
        /// </summary>
        /// <remarks>
        /// This function is called by <see cref="US.OpenServer.Protocols.ProtocolConfigurationSectionHandler"/>
        /// when loading protocol configurations from the app.config file and the
        /// app.config file includes a configSectionAssemply and configSectionClassPath
        /// within the protocols/item section.
        /// </remarks>
        /// <param name="id">A UInt16 that specifies the protocol identifier.</param>
        /// <param name="assembly">A string that specifies the assembly the class is
        /// contained.</param>
        /// <param name="classPath">A string that specifies the full path to the class. The
        /// class must extend IProtocol.</param>
        /// <param name="xmlNode">A XmlNode that contains the configuration properties.</param>
        public virtual void Initialize(ushort id, string assembly, string classPath, XmlNode xmlNode)
        {
            Id = id;
            Assembly = assembly;
            ClassPath = classPath;
        }

        /// <summary>
        /// Creates an instance of the <see cref="US.OpenServer.Protocols.IProtocol"/>
        /// class.
        /// </summary>
        /// <returns>An IProtocol.</returns>
        public override IProtocol CreateInstance()
        {
            IProtocol p = base.CreateInstance();
            if (p == null)
            {
                Assembly assembly = System.Reflection.Assembly.LoadFrom(Assembly);
                p = (IProtocol)assembly.CreateInstance(ClassPath);
            }
            return p;
        }
    }
}
    