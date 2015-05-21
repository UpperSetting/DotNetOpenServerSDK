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
using US.OpenServer.Configuration;
using US.OpenServer.Protocols;

namespace US.OpenServer
{
    /// <summary>
    /// Class that connects to the server and optionally enables SSL/TLS 1.2.
    /// </summary>
    public class Client
    {
        #region Events
        /// <summary>
        /// Delegate that defines the event callback for the <see cref="OnConnectionLost"/> event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="ex">An Exception that contains the reason the connection was lost.</param>
        public delegate void OnConnectionLostHandler(object sender, Exception ex);

        /// <summary>
        /// Event that is triggered when the connection to the server is lost.
        /// </summary>
        public event OnConnectionLostHandler OnConnectionLost;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the application logger.
        /// </summary>
        public ILogger Logger { get; private set; }

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
        /// Gets the optional user defined Object that is passed through to each <see cref="IProtocol"/>
        /// object.
        /// </summary>
        public object UserData { get; private set; }
        #endregion

        #region Variables
        /// <summary>
        /// Implements the connection session.
        /// </summary>
        private Session session;
        #endregion

        #region Constructor
        /// <summary> Creates an instance of Client. </summary>
        /// <remarks> All parameters are optional. If null is passed, the object's
        /// configuration is read from the app.config file. </remarks>
        /// <param name="logger">An optional ILogger to log messages. If null is passed,
        /// a <see cref="US.OpenServer.Logger"/> object is created.</param>
        /// <param name="serverConfiguration">An optional ServerConfiguration that contains the
        /// properties necessary to connect to the server. If null is passed, the
        /// configuration is read from the app.config's 'server' XML section
        /// node.</param>
        /// <param name="protocolConfigurations">An optional Dictionary of
        /// ProtocolConfiguration objects keyed with each protocol's unique identifier.
        /// If null is passed, the configuration is read from the app.config's
        /// 'protocols' XML section node.</param>
        /// <param name="userData">An Object the caller can pass through to each protocol.</param>
        public Client(
            ServerConfiguration serverConfiguration = null,
            Dictionary<ushort, ProtocolConfiguration> protocolConfigurations = null,
            ILogger logger = null,
            object userData = null)
        {
            if (logger == null)
                logger = new ConsoleLogger();
            Logger = logger;
            Logger.Log(Level.Info, string.Format("Execution Mode: {0}", Debugger.IsAttached ? "Debug" : "Release"));

            if (serverConfiguration == null)
                serverConfiguration = (ServerConfiguration)ConfigurationManager.GetSection("server");
            ServerConfiguration = serverConfiguration;

            if (protocolConfigurations == null)
                protocolConfigurations = (Dictionary<ushort, ProtocolConfiguration>)ConfigurationManager.GetSection("protocols");
            ProtocolConfigurations = protocolConfigurations;

            UserData = userData;
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Connects to the server, creates a Session, optionally enables SSL/TLS 1.2
        /// and begins an asynchronous socket read operation.
        /// </summary>
        public void Connect()
        {
            Close();

            Logger.Log(Level.Info, string.Format("Connecting to {0}:{1}...", ServerConfiguration.Host, ServerConfiguration.Port));

            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.ReceiveTimeout = ServerConfiguration.ReceiveTimeoutInMS;
            server.SendTimeout = ServerConfiguration.SendTimeoutInMS;
            server.LingerState = new LingerOption(true, 10);
            server.NoDelay = true;
            if (string.IsNullOrEmpty(ServerConfiguration.Host))
                ServerConfiguration.Host = ServerConfiguration.DEFAULT_HOST;
            server.Connect(ServerConfiguration.Host, ServerConfiguration.Port);
            string address = ((IPEndPoint)server.RemoteEndPoint).Address.ToString();
            
            session = new Session(
                new NetworkStream(server), 
                address, 
                ServerConfiguration.TlsConfiguration, 
                ProtocolConfigurations, 
                Logger,
                UserData);

            session.OnConnectionLost += session_OnConnectionLost;

            if (ServerConfiguration.TlsConfiguration != null && ServerConfiguration.TlsConfiguration.Enabled)
                EnableTls();

            Logger.Log(Level.Info, string.Format("Connected to {0}:{1}.", ServerConfiguration.Host, ServerConfiguration.Port));

            session.BeginRead();
        }

        /// <summary>
        /// A function that wraps the <see cref="US.OpenServer.SessionBase.Initialize(ushort, object = null)"/>
        /// function which creates then initializes the protocol.
        /// </summary>
        /// <param name="protocolId">A UInt16 that specifies the unique protocol
        /// identifier.</param>
        /// <returns>An IProtocol that implements the protocol layer.</returns>
        public IProtocol Initialize(ushort protocolId)
        {
             return session != null ? session.Initialize(protocolId, UserData) : null;
        }

        /// <summary>
        /// Closes the protocol.
        /// </summary>
        /// <param name="protocolId">A UInt16 that specifies the unique protocol
        /// identifier.</param>
        public void Close(ushort protocolId)
        {
            if (session != null)
            {
                session.Close(protocolId);
            }
        }

        /// <summary>
        /// Closes the <see cref="Session"/>.
        /// </summary>
        public void Close()
        {
            if (session != null)
            {
                session.Close();
                session = null;
            }
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// Event handler for <see cref="SessionBase.OnConnectionLost"/> events.
        /// </summary>
        /// <remarks> When a connection is lost, the Exception is forwarded to objects
        /// that have subscribed to <see cref="OnConnectionLost"/> events.</remarks>
        /// <param name="sender">An object that contains state information for this validation.</param>
        /// <param name="ex">An Exception that contains the error the connection was lost.</param>
        private void session_OnConnectionLost(object sender, Exception ex)
        {
            if (OnConnectionLost != null)
                OnConnectionLost(this, ex);
        }
        #endregion

        #region TLS
        /// <summary>
        /// Enables SSL/TLS 1.2.
        /// </summary>
        /// <remarks> Registers the <see cref="Session.TlsCertificateValidationCallback"/>
        /// and <see cref="Session.TlsCertificateSelectionCallback"/> with the
        /// SslStream, optionally gets a client side SSL certificate from the local
        /// certificate store, then authenticates the connection. </remarks>
        private void EnableTls()
        {
            RemoteCertificateValidationCallback validationCallback =
              new RemoteCertificateValidationCallback(session.TlsCertificateValidationCallback);

            LocalCertificateSelectionCallback selectionCallback =
              new LocalCertificateSelectionCallback(session.TlsCertificateSelectionCallback);

            SslStream sslStream = new SslStream(
                session.Stream, true, validationCallback, selectionCallback, EncryptionPolicy.RequireEncryption);
            session.Stream = sslStream;

            X509Certificate2 certificate = session.GetCertificateFromStore(
                string.Format("CN={0}", ServerConfiguration.TlsConfiguration.Certificate));

            X509CertificateCollection certificates = new X509CertificateCollection();
            if (certificate != null)
                certificates.Add(certificate);

            ((SslStream)session.Stream).AuthenticateAsClient(
                ServerConfiguration.Host, certificates, SslProtocols.Tls, ServerConfiguration.TlsConfiguration.CheckCertificateRevocation);
        }
        #endregion
    }
}
