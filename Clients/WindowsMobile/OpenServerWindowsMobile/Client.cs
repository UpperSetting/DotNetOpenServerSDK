using System;
using System.Collections.Generic;
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
        private ILogger logger;
        private ServerConfiguration cfg;
        private Session session;
        private StreamSocket streamSocket;
        #endregion

        #region Constructor
        public Client(
            ServerConfiguration cfg,
            Dictionary<ushort, ProtocolConfiguration> protocolConfigurations)
        {
            this.cfg = cfg;
            this.logger = new Logger();
            ProtocolConfigurations.Items = protocolConfigurations;
        }

        public Client(
            ServerConfiguration cfg, 
            ILogger logger, 
            Dictionary<ushort, ProtocolConfiguration> protocolConfigurations)
        {
            this.cfg = cfg;
            this.logger = logger;
            ProtocolConfigurations.Items = protocolConfigurations;
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

            session = new Session(streamSocket, hostName.DisplayName, cfg.TlsConfiguration, logger);
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
