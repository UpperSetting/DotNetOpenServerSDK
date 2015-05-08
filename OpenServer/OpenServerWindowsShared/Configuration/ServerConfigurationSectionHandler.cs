using System.Configuration;
using System.Xml;

namespace US.OpenServer.Configuration
{
    /// <summary>
    /// Class that loads the TCP socket server configuration properties from the
    /// app.config file.
    /// </summary>
    public class ServerConfigurationSectionHandler : IConfigurationSectionHandler
    {
        #region Constants
        /// <summary> The name of the attribute that contains the value.</summary>
        private const string VALUE = "value";

        /// <summary> The name of the section that contains the SSL/TLS
        /// properties.</summary>
        private const string TLS = "tls";

        /// <summary> The name of the attribute that specifies, when used from a Windows
        /// server, the IP address to bind the TCP socket server, when used from a
        /// client, the hostname the server is running.</summary>
        private const string HOST = "host";

        /// <summary> The name of the section that contains port number the server's
        /// port number.</summary>
        private const string PORT = "port";

        /// <summary> The name of the section that contains the socket idle
        /// timeout.</summary>
        private const string IDLETIMEOUT = "idleTimeout";
        
        /// <summary> The name of the section that contains the socket read
        /// timeout.</summary>
        private const string READTIMEOUT = "readTimeout";

        /// <summary> The name of the section that contains the socket write
        /// timeout.</summary>
        private const string WRITETIMEOUT = "writeTimeout";

        #region TLS
        /// <summary>The name of the attribute that specifies whether the end point must
        /// supply a certificate for authentication.</summary>
        private const string CERTIFICATE = "certificate";

        /// <summary>The name of the attribute that specifies whether the end point
        /// must supply a certificate for authentication.</summary>
        private const string REQUIREREMOTECERTIFICATE = "requireRemoteCertificate";

        /// <summary>The name of the attribute that specifies whether self-signed
        /// certificates are supported.</summary>
        private const string ALLOWSELFSIGNEDCERTIFICATE = "allowSelfSignedCertificate";

        /// <summary>The name of the attribute that specifies whether the certificate
        /// revocation list is checked during authentication.</summary>
        private const string CHECKCERTIFICATEREVOCATION = "checkCertificateRevocation";

        /// <summary>The name of the attribute that specifies whether the certificate
        /// chain is checked during authentication.</summary>
        private const string ALLOWCERTIFICATECHAINERRORS = "allowCertificateChainErrors";
        #endregion
        #endregion

        /// <summary>
        /// Reads the TCP socket server configuration properties from the app.config
        /// file.
        /// </summary>
        /// <param name="parent">The parent object. This parameter is not used.</param>
        /// <param name="configContext">The Configuration context object. This parameter
        /// is not used.</param>        
        /// <param name="section">The XML section node.</param>
        /// <returns>A ServerConfiguration that contains the configuration properties.</returns>
        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            ServerConfiguration cfg = new ServerConfiguration();

            XmlNode node = section.SelectSingleNode(HOST);
            if (node != null)
                cfg.Host = node.Attributes[VALUE].Value;
            
            node = section.SelectSingleNode(PORT);
            if (node != null)
                cfg.Port = ushort.Parse(node.Attributes[VALUE].Value);

            node = section.SelectSingleNode(IDLETIMEOUT);
            if (node != null)
                cfg.IdleTimeout = int.Parse(node.Attributes[VALUE].Value);

            node = section.SelectSingleNode(READTIMEOUT);
            if (node != null)
                cfg.ReceiveTimeout = int.Parse(node.Attributes[VALUE].Value);

            node = section.SelectSingleNode(WRITETIMEOUT);
            if (node != null)
                cfg.SendTimeout = int.Parse(node.Attributes[VALUE].Value);

            node = section.SelectSingleNode(TLS);
            if (node != null)
            {
                cfg.TlsConfiguration.Enabled = bool.Parse(node.Attributes[VALUE].Value);
                if (cfg.TlsConfiguration.Enabled)
                {
                    cfg.TlsConfiguration.Certificate = node.Attributes[CERTIFICATE].Value;
                    cfg.TlsConfiguration.RequireRemoteCertificate = bool.Parse(node.Attributes[REQUIREREMOTECERTIFICATE].Value);
                    cfg.TlsConfiguration.AllowSelfSignedCertificate = bool.Parse(node.Attributes[ALLOWSELFSIGNEDCERTIFICATE].Value);
                    cfg.TlsConfiguration.CheckCertificateRevocation = bool.Parse(node.Attributes[CHECKCERTIFICATEREVOCATION].Value);
                    cfg.TlsConfiguration.AllowCertificateChainErrors = bool.Parse(node.Attributes[ALLOWCERTIFICATECHAINERRORS].Value);
                }
            }

            return cfg;
        }
    }
}
