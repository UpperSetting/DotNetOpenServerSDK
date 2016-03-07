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

using System;
using System.IO;
using System.Threading;

namespace US.OpenServer.Protocols.DatabaseAuth
{
    /// <summary>
    /// Class that is created by Windows and Windows Mobile client applications to
    /// enable database authentication.
    /// </summary>
    public class DatabaseAuthProtocolClient : DatabaseAuthProtocol
    {
        /// <summary>
        /// Defines the maximum number of milliseconds to wait for an authentication request. 
        /// </summary>
        private const int TIMEOUT = 120000;

        /// <summary>
        /// Contains the last command result.
        /// </summary>
        public DatabaseAuthProtocolCommands LastCommandResult { get; private set; }

        /// <summary>
        /// Contains the last error message.
        /// </summary>
        public string LastErrorMessage { get; private set; }

        /// <summary>
        /// Creates a DatabaseAuthProtocolClient object.
        /// </summary>
        public DatabaseAuthProtocolClient()
        {
        }

        /// <summary>
        /// Sends a request to the server to authenticate the user and then blocks
        /// waiting for a response from the server.
        /// </summary>
        /// <param name="userName">A String that contains the user's name.</param>
        /// <param name="password">A String that contains the user's password.</param>
        /// user's account resides.</param>
        /// <returns>True if authenticated, otherwise False.</returns>
        public bool Authenticate(string userName, string password)
        {
            lock (this)
            {
                if (Session == null)
                    return false;

                if (IsAuthenticated)
                    throw new Exception("Already authenticated.");

                LastErrorMessage = null;
                UserName = userName;
                Session.UserName = userName;

                MemoryStream ms = new MemoryStream();
                BinaryWriter bw = GetBinaryWriter(ms, DatabaseAuthProtocolCommands.AUTHENTICATE);
                bw.WriteString(userName);
                bw.WriteString(password);
                Session.Send(ms);

                if (!Monitor.Wait(this, TIMEOUT))
                    throw new TimeoutException();

                return IsAuthenticated;
            }
        }

        /// <summary>
        /// Handles the response to the <see cref="US.OpenServer.Protocols.DatabaseAuth.DatabaseAuthProtocolCommands.AUTHENTICATE"/>
        /// command packet. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// When an <see cref="US.OpenServer.Protocols.DatabaseAuth.DatabaseAuthProtocolCommands.AUTHENTICATED"/>
        /// response is received, notifies the <see cref="US.OpenServer.SessionBase"/> the
        /// client is authenticated enabling the session to allow execution of higher
        /// protocol layers. Finally, signals the <see cref="Authenticate(String, String, String)" />
        /// function to unblock the calling thread. 
        /// </para>
        /// <para>
        /// When an <see cref="US.OpenServer.Protocols.DatabaseAuth.DatabaseAuthProtocolCommands.ACCESS_DENIED"/>
        /// response is received, logs the error then signals the <see cref="Authenticate(String, String, String)" />
        /// function to unblock the calling thread. 
        /// </para>
        /// <para>
        /// When an <see cref="US.OpenServer.Protocols.DatabaseAuth.DatabaseAuthProtocolCommands.ERROR"/>
        /// response is received, logs the error then signals the <see cref="Authenticate(String, String, String)" />
        /// function to unblock the calling thread. 
        /// </para>
        /// <para>
        /// Prior to authentication the session disallows all protocols commands.
        /// </para>
        /// </remarks>
        /// <param name="br">A BinaryReader that contains the command packet.</param>
        public override void OnPacketReceived(BinaryReader br)
        {
            lock (this)
            {
                if (Session == null)
                    return;

                LastCommandResult = (DatabaseAuthProtocolCommands)br.ReadByte();
                switch (LastCommandResult)
                {
                    case DatabaseAuthProtocolCommands.AUTHENTICATED:
                        UserId = br.ReadInt32();
                        IsAuthenticated = true;
                        Session.IsAuthenticated = true;
                        Log(Level.Info, "Authenticated.");
                        Monitor.PulseAll(this);
                        break;

                    case DatabaseAuthProtocolCommands.ACCESS_DENIED:
                        LastErrorMessage = DatabaseAuthProtocol.ERROR_ACCESS_DENIED;
                        Log(Level.Notice, LastErrorMessage);
                        Monitor.PulseAll(this);
                        break;

                    case DatabaseAuthProtocolCommands.ACCESS_DENIED_USER_NOT_FOUND:
                        LastErrorMessage = DatabaseAuthProtocol.ERROR_USER_NOT_FOUND;
                        Log(Level.Notice, LastErrorMessage);
                        Monitor.PulseAll(this);
                        break;

                    case DatabaseAuthProtocolCommands.ACCESS_DENIED_INVALID_PASSWORD:
                        LastErrorMessage = DatabaseAuthProtocol.ERROR_INVALID_PASSWORD;
                        Log(Level.Notice, LastErrorMessage);
                        Monitor.PulseAll(this);
                        break;

                    case DatabaseAuthProtocolCommands.ACCESS_DENIED_EMAIL_NOT_VERIFIED:
                        LastErrorMessage = DatabaseAuthProtocol.ERROR_NOT_VERIFIED;
                        Log(Level.Notice, LastErrorMessage);
                        Monitor.PulseAll(this);
                        break;

                    case DatabaseAuthProtocolCommands.ERROR:
                        LastErrorMessage = br.ReadString();
                        Log(Level.Notice, LastErrorMessage);
                        Monitor.PulseAll(this);
                        break;

                    default:
                        Log(Level.Error, string.Format("Invalid or unsupported command.  Command: {0}", LastCommandResult));
                        break;
                }
            }
        }
    }
}
