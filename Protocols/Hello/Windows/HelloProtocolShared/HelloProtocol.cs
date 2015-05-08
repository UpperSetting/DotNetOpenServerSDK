
namespace US.OpenServer.Protocols.Hello
{
    /// <summary>
    /// Base abstract class for the Hello Protocol implementation.
    /// </summary>
    public abstract class HelloProtocol : IProtocol
    {
        /// <summary>
        /// The unique protocol identifier
        /// </summary>
        public const ushort PROTOCOL_IDENTIFIER = 0x000A;

        /// <summary>
        /// Creates a HelloProtocol object.
        /// </summary>
        protected HelloProtocol()
        {
        }

        /// <summary>
        /// Log's a message.
        /// </summary>
        /// <param name="level">A Level that specifies the priority of the message.</param>
        /// <param name="message">A string that contains the message.</param>
        protected void Log(Level level, string message)
        {
            session.Log(level, string.Format("[Hello] {0}", message));
        }
    }
}
