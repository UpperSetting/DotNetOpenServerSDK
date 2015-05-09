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


namespace US.OpenServer.Configuration
{
    /// <summary>
    /// Class used to configure SSL/TLS 1.2 client and server configuration properties.
    /// </summary>
    public class TlsConfiguration
    {
        /// <summary>
        /// Gets or sets whether SSL/TLS 1.2 is enabled.
        /// </summary>
        /// <value>A Boolean that specifies whether SSL/TLS 1.2 is enabled.</value>
        public bool Enabled { get; set; }
        /// <summary>
        /// Gets or sets the X509Certificate used to authenticate.
        /// </summary>
        /// <value>A string that specifies the name of the X509Certificate used to authenticate.</value>
        public string Certificate { get; set; }
        /// <summary>
        /// Gets or sets whether the end point must supply a certificate for
        /// authentication.
        /// </summary>
        /// <value>A Boolean value that specifies whether the end point must supply a
        /// certificate for authentication.</value>
        public bool RequireRemoteCertificate { get; set; }
        /// <summary>
        /// Gets or sets whether self-signed certificates are supported.
        /// </summary>
        /// <value>A Boolean value that specifies whether self-signed certificates are
        /// supported.</value>
        public bool AllowSelfSignedCertificate { get; set; }
        /// <summary>
        /// Gets or sets whether the certificate revocation list is checked during
        /// authentication.
        /// </summary>
        /// <value>A Boolean value that specifies whether the certificate revocation list
        /// is checked during authentication.</value>
        public bool CheckCertificateRevocation { get; set; }
        /// <summary>
        /// Gets or sets whether the certificate chain is checked during authentication.
        /// </summary>
        /// <value>A Boolean value that specifies whether the certificate chain is checked
        /// during authentication.</value>
        public bool AllowCertificateChainErrors { get; set; }
    }
}
