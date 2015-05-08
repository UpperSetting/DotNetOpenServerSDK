
namespace US.OpenServer
{
    /// <summary>
    /// Static class that defines error strings.
    /// </summary>
    public static class ErrorTypes
    {
        /// <summary>
        /// The specified protocol has not been configured.
        /// </summary>
        public const string INVALID_PROTOCOL = "Invalid or unsupported protocol.";

        /// <summary>
        /// The specified protocol has been configured, however; the class can not be
        /// found.
        /// </summary>
        public const string CLASS_NOT_FOUND = "Unable to create protocol layer.  Class not found.  Class: {0}";

        /// <summary>
        /// The socket connection has been lost.
        /// </summary>
        public const string CONNECTION_LOST = "Connection lost.  {0}";

        /// <summary>
        /// The remote connection sent a command packet for a protocol prior to
        /// authenticating.
        /// </summary>
        public const string NOT_AUTHENTICATED = "Unable to add protocol layer.  Not Authenticated.  Class: {0}";

        /// <summary>
        /// A new command packet has been received, however; there are no more threads
        /// available to handle the packet.
        /// </summary>
        public const string NO_MORE_THREADS_AVAILABLE = "No more threads available.";
    }
}
