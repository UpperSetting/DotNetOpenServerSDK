
namespace US.OpenServer.Protocols.WinAuth
{
    /// <summary>
    /// Enumeration that defines the available commands.
    /// </summary>
    public enum WinAuthProtocolCommands : byte
    {
        /// <summary>
        /// Sent by clients to authenticate a user.
        /// </summary>
        AUTHENTICATE = 0x01,

        /// <summary>
        /// Sent by the Windows Server in response to a successful authentication
        /// request.
        /// </summary>
        AUTHENTICATED = 0x02,

        /// <summary>
        /// Sent by the Windows Server in response to a failed authentication attempt.
        /// </summary>
        ACCESS_DENIED = 0x03,

        /// <summary>
        /// Sent by the Windows Server when an authentication attempt errors or an
        /// invalid or unsupported command is received.
        /// </summary>
        ERROR = 0xFF,
    }
}
