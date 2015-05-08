
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
