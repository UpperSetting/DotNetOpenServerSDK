
namespace US.OpenServer.Protocols
{
    /// <summary>
    /// Base class for all authentication protocol implementations.
    /// </summary>
    public abstract class AuthenticationProtocolBase : IProtocol
    {
        /// <summary>
        /// Gets or sets whether user has been authenticated.
        /// </summary>
        /// <value>A Boolean that specifies whether the user has been
        /// authenticated.</value>
        public bool IsAuthenticated { get; protected set; }

        /// <summary>
        /// Gets or sets the user's name.
        /// </summary>
        /// <value>A string that specifies the user's name.</value>
        public string UserName { get; protected set; }
    }
}
