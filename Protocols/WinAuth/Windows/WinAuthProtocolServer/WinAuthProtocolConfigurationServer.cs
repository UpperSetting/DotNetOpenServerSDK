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
using System.Collections.Generic;
using System.Xml;

namespace US.OpenServer.Protocols.WinAuth
{
    /// <summary>
    /// Class that is created by the Windows Server that contains the list of users
    /// and roles allowed to connect to the server.
    /// </summary>
    public class WinAuthProtocolConfigurationServer : ProtocolConfigurationEx
    {
        #region Private Constants
        /// <summary>
        /// The XML key that contains the permissions.
        /// </summary>
        private const string PERMISSIONS = "permissions";

        /// <summary>
        /// The XML key that contains the roles.
        /// </summary>
        private const string ROLES = "roles";

        /// <summary>
        /// The XML key that contains the users.
        /// </summary>
        private const string USERS = "users";

        /// <summary>
        /// The XML attribute value that contains user or role name.
        /// </summary>
        private const string VALUE = "value";
        #endregion

        #region Properties
        /// <summary>
        /// A list of roles that are allowed to access the US.OpenServer.ServerServer.
        /// </summary>
        public HashSet<string> Roles { get; private set; }

        /// <summary>
        /// A list of users that are allowed to access the US.OpenServer.ServerServer.
        /// </summary>
        public HashSet<string> Users { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a WinAuthProtocolConfigurationServer object.
        /// </summary>
        /// <remarks>
        /// This constructor is called by <see cref="US.OpenServer.Protocols.ProtocolConfigurationSectionHandler"/>
        /// when loading protocol configurations from the app.config file and the
        /// app.config file includes a configSectionAssemply and configSectionClassPath
        /// within the protocols/item section.
        /// </remarks>
        public WinAuthProtocolConfigurationServer()
        {
            Roles = new HashSet<string>();
            Users = new HashSet<string>();
        }

        /// <summary>
        /// Creates a WinAuthProtocolConfigurationServer object.
        /// </summary>
        /// <remarks>
        /// This constructor is called by <see cref="US.OpenServer.Protocols.ProtocolConfigurationSectionHandler"/>
        /// when programatically loading.
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="protocolType"></param>
        public WinAuthProtocolConfigurationServer(ushort id, Type protocolType)
            : base (id, protocolType)
        {
            Roles = new HashSet<string>();
            Users = new HashSet<string>();
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Initializes this class then loads the permissions from the app.config file.
        /// This function is called by the <see cref="US.OpenServer.Protocols.ProtocolConfigurationSectionHandler"/> 
        /// when loading the configuration from the app.config file.
        /// </summary>
        /// <remarks>
        /// To configure add configSectionAssemply and
        /// configSectionClassPath to the app.config's protocols/item section. Use the
        /// following syntax:
        /// <code>
        /// <protocols>
        ///     <item id="2" assembly="WinAuthServer.dll" classPath="US.OpenServer.Protocols.WinAuth.WinAuthProtocolServer"
        ///         configSectionAssemply="WinAuthServer.dll" configSectionClassPath="US.OpenServer.Protocols.WinAuth.WinAuthProtocolConfigurationServer">
        ///         <permissions>
        ///             <roles>
        ///                 <role value="Administrators" />
        ///                 <role value="Power Users" />
        ///             </roles>
        ///             <users>
        ///                 <user value="User1" />
        ///                 <user value="User2" />
        ///             </users>
        ///         </permissions>
        ///     </item>
        /// </protocols>
        /// </code>
        /// </remarks>
        /// <param name="id">A UInt16 that specifies the protocol identifier.</param>
        /// <param name="assembly">A string that specifies the assembly the class is
        /// contained.</param>
        /// <param name="classPath">A string that specifies the full path to the class. The
        /// class must extend IProtocol.</param>
        /// <param name="node">A XmlNode that contains the permissions.</param>
        public override void Initialize(ushort id, string assembly, string classPath, XmlNode node)
        {
            base.Initialize(id, assembly, classPath, node);

            XmlNode permissionsNode = node.SelectSingleNode(PERMISSIONS);
            if (permissionsNode == null)
                return;

            HashSet<string> roles = new HashSet<string>();
            XmlNode rolesNode = permissionsNode.SelectSingleNode(ROLES);
            if (rolesNode != null)
            {
                foreach (XmlNode roleNode in rolesNode.ChildNodes)
                {
                    string role = roleNode.Attributes[VALUE].Value;
                    if (!string.IsNullOrEmpty(role))
                    {
                        if (!roles.Contains(role))
                            roles.Add(role);
                    }
                }
            }
            Roles = roles;

            HashSet<string> users = new HashSet<string>();
            XmlNode usersNode = permissionsNode.SelectSingleNode(USERS);
            if (usersNode != null)
            {
                foreach (XmlNode userNode in usersNode.ChildNodes)
                {
                    string user = userNode.Attributes[VALUE].Value;
                    if (!string.IsNullOrEmpty(user))
                    {
                        user = user.ToLower();
                        if (!users.Contains(user))
                            users.Add(user);
                    }
                }
            }
            Users = users;
        }
        #endregion
    }
}

