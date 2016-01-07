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
using System.IO;

namespace US.OpenServer.Protocols.DatabaseAuth
{
    /// <summary>
    /// Class that is created by the Windows Server to enable Windows
    /// Authentication.
    /// </summary>
    public class DatabaseAuthProtocolServer : DatabaseAuthProtocol
    {
        #region Private Variables
        /// <summary>
        /// The connection session.
        /// </summary>
        private Session sessionEx;

        /// <summary>
        /// Contains a cached list of assigned roles
        /// </summary>
        private List<string> cachedAssignedRoles = new List<string>();

        /// <summary>
        /// Contains a cached list of unassigned roles.
        /// </summary>
        private List<string> cachedUnAssignedRoles = new List<string>();

        #endregion

        #region Constructor
        /// <summary>
        /// Creates a DatabaseAuthProtocolServer object.
        /// </summary>
        public DatabaseAuthProtocolServer()
        {
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Calls the base <see cref="US.OpenServer.Protocols.ProtocolBase.Initialize"/>
        /// function then saves a reference to the Session and DatabaseAuthProtocolConfigurationServer.
        /// </summary>
        /// <param name="session">A SessionBase that encapsulates the connection
        /// session. This value is casted to Session.</param>
        ///<param name="pc">A ProtocolConfiguration that contains configuration
        /// properties. This value is casted to DatabaseAuthProtocolConfigurationServer.</param>
        /// <param name="userData">An object that may be used by client applications to
        /// pass objects or data to client side protocol implementations. This parameter is
        /// not used.</param>
        public override void Initialize(SessionBase session, ProtocolConfiguration pc, object userData = null)
        {
            base.Initialize(session, pc, userData);
            this.sessionEx = (Session)session;            
        }

        /// <summary>
        /// Handles the <see cref="US.OpenServer.Protocols.DatabaseAuth.DatabaseAuthProtocolCommands.AUTHENTICATE"/>
        /// command packet request. 
        /// </summary>
        /// <param name="br">A BinaryReader that contains the command packet.</param>
        public override void OnPacketReceived(BinaryReader br)
        {
            lock (this)
            {
                if (Session == null)
                    return;

                DatabaseAuthProtocolCommands command = (DatabaseAuthProtocolCommands)br.ReadByte();
                try
                {
                    switch (command)
                    {
                        case DatabaseAuthProtocolCommands.AUTHENTICATE:

                            string userName = br.ReadString();
                            string password = br.ReadString();

                            try
                            {
                                DB db = new DB("DB");
                                UserId = db.Authenticate(userName, password);

                                UserName = userName;
                                IsAuthenticated = true;

                                Session.UserName = userName;
                                Session.IsAuthenticated = true;
                                Session.AuthenticationProtocol = this;

                                Log(Level.Info, string.Format(@"Authenticated {0}.", userName));

                                MemoryStream ms = new MemoryStream();
                                BinaryWriter bw = GetBinaryWriter(ms, DatabaseAuthProtocolCommands.AUTHENTICATED);
                                bw.Write((int)UserId);
                                Session.Send(ms);
                            }
                            catch (Exception ex)
                            {
                                Log(Level.Notice, string.Format(@"Access denied.  {0}.  User: {1}", ex.Message, userName));

                                MemoryStream ms = new MemoryStream();
                                GetBinaryWriter(ms, DatabaseAuthProtocolCommands.ACCESS_DENIED);
                                Session.Send(ms);
                            }
                            break;
                        default:
                            throw new Exception("Invalid or unsupported command.");
                    }
                }
                catch (Exception ex)
                {
                    Log(Level.Error, string.Format("{0}  Command: {1}", ex.Message, command));

                    MemoryStream ms = new MemoryStream();
                    BinaryWriter bw = GetBinaryWriter(ms, DatabaseAuthProtocolCommands.ERROR);
                    bw.Write(ex.Message);
                    Session.Send(ms);
                }
            }
        }

        /// <summary>
        /// Checks if the authenticated user is a member of the passed role.
        /// </summary>
        /// <remarks>
        /// This function is made available so application layer protocols can include
        /// fine grained security.
        /// </remarks>
        /// <param name="role">A String that contains the name of the role.</param>
        /// <returns>True if user is a member of the role, otherwise False.</returns>
        public override bool IsInRole(string role)
        {
            if (cachedAssignedRoles.Contains(role))
                return true;

            if (cachedUnAssignedRoles.Contains(role))
                return false;

            DB db = new DB("DB");
            if (db.IsInRole((int)UserId, role))
            {
                cachedAssignedRoles.Add(role);
                return true;
            }
            else
            {
                cachedUnAssignedRoles.Add(role);
                return false;
            }
        }
        #endregion
    }
}
