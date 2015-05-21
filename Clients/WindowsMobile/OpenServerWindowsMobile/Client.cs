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
using System.Diagnostics;
using US.OpenServer;
using US.OpenServer.Configuration;
using US.OpenServer.Protocols;
using Windows.Foundation;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Security.Cryptography.Certificates;

namespace US.OpenServer.WindowsMobile
{
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

        /// <summary>
        /// The connection socket.
        /// </summary>
        private StreamSocket streamSocket;
        #endregion

        #region Constructor
        public Client(
            ServerConfiguration serverConfiguration = null,
            Dictionary<ushort, ProtocolConfiguration> protocolConfigurations = null,
            ILogger logger = null,
            object userData = null)
        {
            if (logger == null)
                logger = new ILogger();
            Logger = logger;
            Logger.Log(Level.Info, string.Format("Execution Mode: {0}", Debugger.IsAttached ? "Debug" : "Release"));

            ServerConfiguration = serverConfiguration;
            ProtocolConfigurations = protocolConfigurations;
            UserData = userData;
        }
        #endregion

        #region Public Functions
        public void Connect()
        {
            Close();

            streamSocket = new StreamSocket();
            streamSocket.Control.NoDelay = true;
            HostName hostName = new HostName(ServerConfiguration.Host);

            if (ServerConfiguration.TlsConfiguration != null && ServerConfiguration.TlsConfiguration.Enabled)
            {
                try
                {
                    IAsyncAction aa = streamSocket.ConnectAsync(hostName, ServerConfiguration.Port.ToString(), SocketProtectionLevel.Tls12);
                    aa.AsTask().Wait();
                }
                catch (Exception ex)
                {
                    if (streamSocket.Information.ServerCertificateErrorSeverity != SocketSslErrorSeverity.Ignorable)
                        throw ex.InnerException != null ? ex.InnerException : ex;

                    foreach (ChainValidationResult error in streamSocket.Information.ServerCertificateErrors)
                    {
                        switch (error)
                        {
                            case ChainValidationResult.IncompleteChain:
                                if (ServerConfiguration.TlsConfiguration.AllowCertificateChainErrors)
                                    streamSocket.Control.IgnorableServerCertificateErrors.Add(error);
                                break;
                            case ChainValidationResult.Untrusted:
                                if (ServerConfiguration.TlsConfiguration.AllowSelfSignedCertificate)
                                    streamSocket.Control.IgnorableServerCertificateErrors.Add(error);
                                break;
                            case ChainValidationResult.Revoked:
                                if (ServerConfiguration.TlsConfiguration.CheckCertificateRevocation)
                                    streamSocket.Control.IgnorableServerCertificateErrors.Add(error);
                                break;
                            default:
                                throw ex.InnerException != null ? ex.InnerException : ex;
                        }
                    }

                    IAsyncAction aa = streamSocket.ConnectAsync(hostName, ServerConfiguration.Port.ToString(), SocketProtectionLevel.Tls12);
                    try
                    {
                        aa.AsTask().Wait();
                    }
                    catch (Exception ex2)
                    {
                        throw ex2.InnerException != null ? ex2.InnerException : ex2;
                    }
                }
            }
            else
            {
                IAsyncAction aa = streamSocket.ConnectAsync(hostName, ServerConfiguration.Port.ToString());
                try
                {
                    aa.AsTask().Wait();
                }
                catch (Exception ex)
                {
                    throw ex.InnerException != null ? ex.InnerException : ex;
                }
            }

            session = new Session(streamSocket, hostName.DisplayName, ServerConfiguration.TlsConfiguration, ProtocolConfigurations, Logger, UserData);
            session.OnConnectionLost += session_OnConnectionLost;
            session.Log(Level.Info, string.Format("Connected to {0}:{1}...", ServerConfiguration.Host, ServerConfiguration.Port));
            session.BeginRead();
        }

        public IProtocol Initialize(ushort protocolId, object userData = null)
        {
             return session != null ? session.Initialize(protocolId, userData) : null;
        }

        public void Close(ushort protocolId)
        {
            if (session != null)
            {
                session.Close(protocolId);
            }
        }

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
        private void session_OnConnectionLost(object sender, Exception ex)
        {
            if (OnConnectionLost != null)
                OnConnectionLost(this, ex);
        }
        #endregion
    }
}
