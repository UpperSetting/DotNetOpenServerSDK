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
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace US.OpenServer.Protocols.WinAuth
{
    /// <summary>
    /// Class that is created by the Windows Server to enable Windows
    /// Authentication.
    /// </summary>
    public class WinAuthProtocolServer : WinAuthProtocol
    {
        #region Win32
        /// <summary>
        /// The LogonUser function attempts to log a user.
        /// </summary>
        /// <param name="userName">A string that specifies the name of the user.</param>
        /// <param name="domain">A string that specifies the name of the domain or server whose account database contains the userName account.</param>
        /// <param name="password">A string that specifies the plaintext password for the user account specified by password.</param>
        /// <param name="logonType">The type of logon operation to perform.</param>
        /// <param name="logonProvider">Specifies the logon provider.</param>
        /// <param name="token">A pointer to a handle variable that receives a handle to a token that represents the specified user.</param>
        /// <returns>True if the function succeeds, otherwise False.</returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(string userName, string domain, string password,
            int logonType, int logonProvider, ref IntPtr token);

        /// <summary>
        /// Closes an open object handle.
        /// </summary>
        /// <param name="handle">A valid handle to an open object.</param>
        /// <returns>True if the function succeeds, otherwise False.</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);

        /// <summary>
        /// This logon type is intended for high performance servers to authenticate plaintext passwords.
        /// </summary>
        private const int LOGON32_LOGON_NETWORK = 3;

        /// <summary>
        /// Use the negotiate logon provider.
        /// </summary>
        private const int LOGON32_PROVIDER_WINNT50 = 3;
        #endregion

        #region Private Variables
        /// <summary>
        /// The connection session.
        /// </summary>
        private Session sessionEx;

        /// <summary>
        /// Contains the list of users and roles allowed to connect.
        /// </summary>
        private WinAuthProtocolConfigurationServer pc;

        /// <summary>
        /// The user's WindowsPrincipal.
        /// </summary>
        private WindowsPrincipal wp;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a WinAuthProtocolServer object.
        /// </summary>
        public WinAuthProtocolServer()
        {
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Calls the base <see cref="US.OpenServer.Protocols.IProtocol.Initialize"/>
        /// function then saves a reference to the Session and WinAuthProtocolConfigurationServer.
        /// </summary>
        /// <param name="session">A SessionBase that encapsulates the connection
        /// session. This value is casted to Session.</param>
        ///<param name="pc">A ProtocolConfiguration that contains configuration
        /// properties. This value is casted to WinAuthProtocolConfigurationServer.</param>
        /// <param name="userData">An object that may be used by client applications to
        /// pass objects or data to client side protocol implementations. This parameter is
        /// not used.</param>
        public override void Initialize(SessionBase session, ProtocolConfiguration pc, object userData = null)
        {
            base.Initialize(session, pc, userData);
            this.sessionEx = (Session)session;
            this.pc = pc as WinAuthProtocolConfigurationServer;
        }

        /// <summary>
        /// Handles the <see cref="US.OpenServer.Protocols.WinAuth.WinAuthProtocolCommands.AUTHENTICATE"/>
        /// command packet request. 
        /// </summary>
        /// <param name="br">A BinaryReader that contains the command packet.</param>
        public override void OnPacketReceived(BinaryReader br)
        {
            lock (this)
            {
                if (session == null)
                    return;

                WinAuthProtocolCommands command = (WinAuthProtocolCommands)br.ReadByte();
                try
                {
                    switch (command)
                    {
                        case WinAuthProtocolCommands.AUTHENTICATE:

                            string userName = br.ReadString();
                            string password = br.ReadString();
                            string domain = br.ReadString();
                                
                            try
                            {
                                wp = GetPrincipal(userName, password, domain);                            

                                if (!Authenticate(userName))
                                    throw new Exception("Insufficient privileges.");
                                
                                UserName = userName;
                                IsAuthenticated = true;

                                session.UserName = userName;
                                session.IsAuthenticated = true;
                                session.AuthenticationProtocol = this;
                                
                                Log(Level.Info, string.Format(@"Authenticated {0}\{1}.", domain, userName));

                                MemoryStream ms = new MemoryStream();
                                GetBinaryWriter(ms, WinAuthProtocolCommands.AUTHENTICATED);
                                session.Send(ms);
                            }
                            catch (Exception ex)
                            {
                                Log(Level.Notice, string.Format(@"Access denied.  {0}.  User: {1}\{2}", ex.Message, domain, userName));

                                MemoryStream ms = new MemoryStream();
                                GetBinaryWriter(ms, WinAuthProtocolCommands.ACCESS_DENIED);
                                session.Send(ms);
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
                    BinaryWriter bw = GetBinaryWriter(ms, WinAuthProtocolCommands.ERROR);
                    bw.Write(ex.Message);
                    session.Send(ms);
                }
            }
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// Looks up the user in the list of cached users. If not found, enumerates each
        /// cached group then calls into the WindowsPrinciple to lookup the user in the
        /// group.
        /// </summary>
        /// <param name="userName">A String that contains the user's name.</param>
        /// <returns>True if authenticated, otherwise false.</returns>
        private bool Authenticate(string userName)
        {
            if (pc == null)
                return true;

            if (pc.Users.Count == 0 && pc.Roles.Count == 0)
                return true;

            if (string.IsNullOrEmpty(userName))
                return false;

            if (pc.Users.Contains(userName.ToLower()))
                return true;

            if (wp == null)
                return false;

            foreach (string group in pc.Roles)
            {
                if (wp.IsInRole(group))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Logs onto the network and returns the WindowsPrincipal for the specified account.
        /// </summary>
        /// <param name="userName">A String that contains the user's name.</param>
        /// <param name="password">A String that contains the user's password.</param>
        /// <param name="domain">A String that contains the domain or local server name the
        /// user's account resides.</param>
        /// <returns>The user's WindowsPrincipal.</returns>
        private WindowsPrincipal GetPrincipal(string userName, string password, string domain)
        {
            if (string.IsNullOrEmpty(userName))
                return new WindowsPrincipal(WindowsIdentity.GetCurrent());

            IntPtr phToken = new IntPtr(0);
            phToken = IntPtr.Zero;

            if (string.IsNullOrEmpty(domain))
                domain = System.Environment.MachineName;

            try
            {
                if (!LogonUser(userName, domain, password, LOGON32_LOGON_NETWORK, LOGON32_PROVIDER_WINNT50, ref phToken))
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    Win32Exception ex = new Win32Exception(errorCode);
                    throw new Exception(ex.Message);
                }

                WindowsIdentity wi = new WindowsIdentity(phToken);
                using (WindowsImpersonationContext impersonatedUser = wi.Impersonate())
                {                    
                    return new WindowsPrincipal(WindowsIdentity.GetCurrent());
                }
            }
            finally
            {
                if (phToken != IntPtr.Zero)
                    CloseHandle(phToken);
            }
        }
        #endregion
    }
}
