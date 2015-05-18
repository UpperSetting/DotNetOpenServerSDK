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
        public delegate void OnConnectionLostHandler(object sender, Exception ex);
        public event OnConnectionLostHandler OnConnectionLostEvent;
        #endregion

        #region Variables
        /// <summary>
        /// Gets the application logger.
        /// </summary>
        public ILogger Logger { get; private set; }

        /// <summary>
        /// Contains the required properties to connect to the TCP socket server.
        /// </summary>
        private ServerConfiguration cfg;

        /// <summary>
        /// A Dictionary of <see cref="ProtocolConfiguration"/> objects keyed by each
        /// protocol's unique identifier.
        /// </summary>
        private Dictionary<ushort, ProtocolConfiguration> protocolConfigurations = new Dictionary<ushort, ProtocolConfiguration>();

        /// <summary>
        /// Implements the connection session.
        /// </summary>
        private Session session;

        private StreamSocket streamSocket;
        #endregion

        #region Constructor
        public Client(
            ServerConfiguration cfg, 
            Dictionary<ushort, ProtocolConfiguration> protocolConfigurations,
            ILogger logger = null)
        {
            this.cfg = cfg;            
            this.protocolConfigurations = protocolConfigurations;

            if (logger == null)
                logger = new ILogger();
            Logger = logger;
        }
        #endregion

        #region Public Functions
        public void Connect()
        {
            Close();

            streamSocket = new StreamSocket();
            streamSocket.Control.NoDelay = true;
            HostName hostName = new HostName(cfg.Host);

            if (cfg.TlsConfiguration != null && cfg.TlsConfiguration.Enabled)
            {
                try
                {
                    IAsyncAction aa = streamSocket.ConnectAsync(hostName, cfg.Port.ToString(), SocketProtectionLevel.Tls12);
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
                                if (cfg.TlsConfiguration.AllowCertificateChainErrors)
                                    streamSocket.Control.IgnorableServerCertificateErrors.Add(error);
                                break;
                            case ChainValidationResult.Untrusted:
                                if (cfg.TlsConfiguration.AllowSelfSignedCertificate)
                                    streamSocket.Control.IgnorableServerCertificateErrors.Add(error);
                                break;
                            case ChainValidationResult.Revoked:
                                if (cfg.TlsConfiguration.CheckCertificateRevocation)
                                    streamSocket.Control.IgnorableServerCertificateErrors.Add(error);
                                break;
                            default:
                                throw ex.InnerException != null ? ex.InnerException : ex;
                        }
                    }

                    IAsyncAction aa = streamSocket.ConnectAsync(hostName, cfg.Port.ToString(), SocketProtectionLevel.Tls12);
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
                IAsyncAction aa = streamSocket.ConnectAsync(hostName, cfg.Port.ToString());
                try
                {
                    aa.AsTask().Wait();
                }
                catch (Exception ex)
                {
                    throw ex.InnerException != null ? ex.InnerException : ex;
                }
            }

            session = new Session(streamSocket, hostName.DisplayName, cfg.TlsConfiguration, protocolConfigurations, Logger);
            session.OnConnectionLost += session_OnConnectionLost;
            session.Log(Level.Info, string.Format("Connected to {0}:{1}...", cfg.Host, cfg.Port));
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
            if (OnConnectionLostEvent != null)
                OnConnectionLostEvent(this, ex);
        }
        #endregion
    }
}
