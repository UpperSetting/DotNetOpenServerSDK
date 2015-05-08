
namespace US.OpenServer.Protocols.KeepAlive
{
    /// <summary>
    /// Enumeration that defines the available commands.
    /// </summary>
    public enum KeepAliveProtocolCommands : byte
    {
        /// <summary>
        /// Keeps an idle session open and verifies connectivity.
        /// </summary>
        KEEP_ALIVE = 0x01,
        /// <summary>
        /// Provides notification the end point is closing the session.
        /// </summary>
        QUIT = 0xFF,
    }
}
