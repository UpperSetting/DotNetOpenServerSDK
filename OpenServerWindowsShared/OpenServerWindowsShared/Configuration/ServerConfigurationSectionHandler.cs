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
        private const string RECEIVETIMEOUT = "receiveTimeout";

        /// <summary> The name of the section that contains the socket write
        /// timeout.</summary>
        private const string SENDTIMEOUT = "sendTimeout";

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

            node = section.SelectSingleNode(RECEIVETIMEOUT);
            if (node != null)
                cfg.ReceiveTimeout = int.Parse(node.Attributes[VALUE].Value);

            node = section.SelectSingleNode(SENDTIMEOUT);
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
