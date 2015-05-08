using System;
using System.IO;
using System.Threading;

namespace US.OpenServer.Protocols.WinAuth
{
    /// <summary>
    /// Class that is created by Windows and Windows Mobile client applications to
    /// enable Windows Authentication.
    /// </summary>
    public class WinAuthProtocolClient : WinAuthProtocol
    {
        /// <summary>
        /// Defines the maximum number of milliseconds to wait for an authentication request. 
        /// </summary>
        private const int TIMEOUT = 120000;

        /// <summary>
        /// Creates a WinAuthProtocolClient object.
        /// </summary>
        public WinAuthProtocolClient()
        {
        }

        /// <summary>
        /// Sends a request to the server to authenticate the Windows user and then blocks
        /// waiting for a response from the server.
        /// </summary>
        /// <param name="userName">A String that contains the user's name.</param>
        /// <param name="password">A String that contains the user's password.</param>
        /// <param name="domain">A String that contains the domain or local server name the
        /// user's account resides.</param>
        /// <returns>True if authenticated, otherwise False.</returns>
        public bool Authenticate(string userName, string password, string domain)
        {
            lock (this)
            {
                if (session == null)
                    return false;

                UserName = userName;
                session.UserName = userName;

                MemoryStream ms = new MemoryStream();
                BinaryWriter bw = GetBinaryWriter(ms, WinAuthProtocolCommands.AUTHENTICATE);
                bw.WriteString(userName);
                bw.WriteString(password);
                bw.WriteString(domain);
                session.Send(ms);

                if (!Monitor.Wait(this, TIMEOUT))
                    throw new TimeoutException();

                return IsAuthenticated;
            }
        }

        /// <summary>
        /// Handles the response to the <see cref="US.OpenServer.Protocols.WinAuth.WinAuthProtocolCommands.AUTHENTICATE"/>
        /// command packet. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// When an <see cref="US.OpenServer.Protocols.WinAuth.WinAuthProtocolCommands.AUTHENTICATED"/>
        /// response is received, notifies the <see cref="US.OpenServer.SessionBase"/> the
        /// session is authenticated enabling the session to allow execution of higher
        /// protocol layers. Finally, signals the <see cref="Authenticate(String, String, String)" />
        /// function to unblock the calling thread. 
        /// </para>
        /// <para>
        /// When an <see cref="US.OpenServer.Protocols.WinAuth.WinAuthProtocolCommands.ACCESS_DENIED"/>
        /// response is received, logs the error then signals the <see cref="Authenticate(String, String, String)" />
        /// function to unblock the calling thread. 
        /// </para>
        /// <para>
        /// When an <see cref="US.OpenServer.Protocols.WinAuth.WinAuthProtocolCommands.ERROR"/>
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
                if (session == null)
                    return;

                WinAuthProtocolCommands command = (WinAuthProtocolCommands)br.ReadByte();
                switch (command)
                {
                    case WinAuthProtocolCommands.AUTHENTICATED:
                        IsAuthenticated = true;
                        session.IsAuthenticated = true;
                        Log(Level.Info, "Authenticated.");
                        Monitor.PulseAll(this);
                        break;

                    case WinAuthProtocolCommands.ACCESS_DENIED:
                        Log(Level.Notice, "Access denied.");
                        Monitor.PulseAll(this);
                        break;

                    case WinAuthProtocolCommands.ERROR:
                        {
                            string errorMessage = br.ReadString();
                            Log(Level.Notice, errorMessage);
                            Monitor.PulseAll(this);
                            break;
                        }

                    default:
                        Log(Level.Error, string.Format("Invalid or unsupported command.  Command: {0}", command));
                        break;
                }
            }
        }
    }
}
