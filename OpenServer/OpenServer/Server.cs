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
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using US.OpenServer.Configuration;
using US.OpenServer.Protocols;

namespace US.OpenServer
{
    /// <summary>
    /// Class that implements the server, accepts connections from clients and
    /// optionally enables SSL/TLS 1.2.
    /// </summary>
    public class Server
    {
        #region Properties
        /// <summary>
        /// Gets the application logger.
        /// </summary>
        public Logger Logger { get; private set; }

        /// <summary>
        /// Gets a Dictionary of <see cref="ProtocolConfiguration"/> objects keyed by
        /// each protocol's unique identifier.
        /// </summary>
        public Dictionary<ushort, ProtocolConfiguration> ProtocolConfigurations { get; private set; }

        /// <summary>
        /// Gets the server configuration.
        /// </summary>
        public ServerConfiguration ServerConfiguration { get; private set; }

        /// <summary>
        /// Gets the optional user defined Object that is passed through to each <see cref="ProtocolBase"/>
        /// object.
        /// </summary>
        public object UserData { get; private set; }
        #endregion

        #region Variables
        /// <summary>
        /// A reference to the server socket.
        /// </summary>
        private Socket socket;

        /// <summary>
        /// A list of client connection sessions.
        /// </summary>
        private List<Session> sessions = new List<Session>();

        /// <summary>
        /// A Timer responsible for closing idle connection sessions. Executes every <see cref="TIMER_INTERVAL"/>.
        /// </summary>
        private Timer idleTimeoutTimer;

        /// <summary>
        /// A flag that is used to trigger the socket listener thread to complete.
        /// </summary>
        private bool signalClose;

        /// <summary>
        /// The socket listener thread.
        /// </summary>
        private Thread t;        

        /// <summary>
        /// The interval which the idle time-out Timer runs.
        /// </summary>
        private const int TIMER_INTERVAL = 5000;
        #endregion

        #region Constructor
        /// <summary> Creates an instance of Server. </summary>
        /// <remarks> All parameters are optional. If null is passed, the object's
        /// configuration is read from the app.config file. </remarks>
        /// <param name="logger">An optional Logger to log messages. If null is passed,
        /// a <see cref="US.OpenServer.Logger"/> object is created.</param>
        /// <param name="serverConfiguration">An optional ServerConfiguration that contains the
        /// properties necessary to create the server. If null is passed, the
        /// configuration is read from the app.config's 'server' XML section
        /// node.</param>
        /// <param name="protocolConfigurations">An optional Dictionary of
        /// ProtocolConfiguration objects keyed with each protocol's unique identifier.
        /// If null is passed, the configuration is read from the app.config's
        /// 'protocols' XML section node.</param>
        /// <param name="userData">An Object the caller can pass through to each protocol.</param>
        public Server(
            ServerConfiguration serverConfiguration = null,
            Dictionary<ushort, ProtocolConfiguration> protocolConfigurations = null,
            Logger logger = null,
            object userData = null)
        {
            if (logger == null)
                logger = new ConsoleLogger();
            Logger = logger;
            Logger.Log(Level.Info, string.Format("Execution Mode: {0}", Debugger.IsAttached ? "Debug" : "Release"));

            if (serverConfiguration == null)
                serverConfiguration = (ServerConfiguration)ConfigurationManager.GetSection("server");
            if (serverConfiguration == null)
                serverConfiguration = new ServerConfiguration();
            ServerConfiguration = serverConfiguration;

            if (protocolConfigurations == null)
                protocolConfigurations = (Dictionary<ushort, ProtocolConfiguration>)ConfigurationManager.GetSection("protocols");
            if (protocolConfigurations == null)
                protocolConfigurations = new Dictionary<ushort, ProtocolConfiguration>();
            ProtocolConfigurations = protocolConfigurations;

            UserData = userData;

            t = new Thread(new ThreadStart(Run));
            t.Start();
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Shuts down the server. The socket listener thread is signaled to close, the
        /// idle timeout Timer is disposed, the server socket is closed and the list of
        /// cached Sessions is cleared.
        /// </summary>
        public void Close()
        {
            signalClose = true;

            if (idleTimeoutTimer != null)
                idleTimeoutTimer.Dispose();

            if (socket != null)
                socket.Close();

            lock (sessions)
                sessions.Clear();
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// Enables SSL/TLS 1.2.
        /// </summary>
        /// <remarks> Registers the <see cref="Session.TlsCertificateValidationCallback"/>
        /// and <see cref="Session.TlsCertificateSelectionCallback"/> with the
        /// SslStream, gets the server side SSL certificate from the local
        /// certificate store, then authenticates the connection. </remarks>
        /// <param name="session">The Session object to enable TLS.</param>
        private void EnableTls(Session session)
        {
            RemoteCertificateValidationCallback validationCallback =
              new RemoteCertificateValidationCallback(session.TlsCertificateValidationCallback);

            LocalCertificateSelectionCallback selectionCallback =
              new LocalCertificateSelectionCallback(session.TlsCertificateSelectionCallback);

            SslStream sslStream = new SslStream(
                session.Stream, 
                true, 
                validationCallback, 
                selectionCallback, 
                EncryptionPolicy.AllowNoEncryption);
            session.Stream = sslStream;

            X509Certificate2 certificate = session.GetCertificateFromStore(
                string.Format("CN={0}", ServerConfiguration.TlsConfiguration.Certificate));

            ((SslStream)session.Stream).AuthenticateAsServer(
                certificate, 
                ServerConfiguration.TlsConfiguration.RequireRemoteCertificate, 
                SslProtocols.Ssl3 | SslProtocols.Tls,
                ServerConfiguration.TlsConfiguration.CheckCertificateRevocation);
        }

        /// <summary>
        /// The idle timeout Timer callback method. Checks each <see cref="Session"/>
        /// for inactivity. When the idle timeout is exceeded the session is closed.
        /// </summary>
        /// <param name="state">An object containing application-specific information
        /// relevant to the method invoked by this delegate, or null. This parameter is
        /// used</param>
        private void IdleTimeOutTimerCallback(object state)
        {
            try
            {
                List<Session> sessionsToClose = null;
                lock (sessions)
                {
                    DateTime now = DateTime.Now;

                    foreach (Session session in sessions)
                    {
                        if (session.IsClosed ||
                            now.Ticks - session.LastActivityAt.Ticks > ServerConfiguration.IdleTimeoutInTicks)
                        {
                            if (sessionsToClose == null)
                                sessionsToClose = new List<Session>();

                            sessionsToClose.Add(session);
                        }
                    }
                }

                if (sessionsToClose != null)
                {
                    foreach (Session session in sessionsToClose)
                    {
                        if (!session.IsClosed)
                            session.Log(Level.Notice, "Idle connection detected.");

                        lock (sessions)
                            sessions.Remove(session);

                        if (!session.IsClosed)
                            session.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        /// <summary>
        /// The server socket listener thread. Creates the server socket, creates the
        /// idle timeout Timer then loops accepting connections. When a client connects,
        /// creates the Session, optionally enables SSL/TLS 1.2, caches the Session and
        /// begins an asynchronous socket read operation.
        /// </summary>
        private void Run()
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.ReceiveTimeout = ServerConfiguration.ReceiveTimeoutInMS;
                socket.SendTimeout = ServerConfiguration.SendTimeoutInMS;
                if (string.IsNullOrEmpty(ServerConfiguration.Host))
                    ServerConfiguration.Host = ServerConfiguration.DEFAULT_BIND_ADDRESS;
                socket.Bind(new IPEndPoint(IPAddress.Parse(ServerConfiguration.Host), ServerConfiguration.Port));
                socket.Listen(64);//config value                
                Logger.Log(Level.Info, string.Format("Listening on {0}:{1}...", ServerConfiguration.Host, ServerConfiguration.Port));

                idleTimeoutTimer = new Timer(IdleTimeOutTimerCallback, null, TIMER_INTERVAL, TIMER_INTERVAL);

                while (!signalClose)
                {
                    Socket client = socket.Accept();
                    string address = ((IPEndPoint)client.RemoteEndPoint).Address.ToString();
                    Session session = new Session(
                        new NetworkStream(client),
                        address,
                        ServerConfiguration.TlsConfiguration,
                        ProtocolConfigurations,
                        Logger,
                        UserData);

                    if (ServerConfiguration.TlsConfiguration != null && ServerConfiguration.TlsConfiguration.Enabled)
                        EnableTls(session);

                    session.Log(Level.Info, "Connected.");

                    lock (sessions)
                        sessions.Add(session);

                    session.BeginRead();

                    if (signalClose)
                        break;
                }
            }
            catch (SocketException ex)
            {
                if (!signalClose)
                    Logger.Log(Level.Error, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Log(Level.Error, ex.Message);
            }
        }
        #endregion
    }
}
