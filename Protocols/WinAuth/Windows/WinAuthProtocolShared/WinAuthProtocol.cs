using System.IO;
using System.Text;

namespace US.OpenServer.Protocols.WinAuth
{
    /// <summary>
    /// Base abstract class for the Windows Authentication Protocol implementation.
    /// </summary>
    public abstract class WinAuthProtocol : AuthenticationProtocolBase
    {
        /// <summary>
        /// The unique protocol identifier
        /// </summary>
        public const ushort PROTOCOL_IDENTIFIER = 0x0002;

        /// <summary>
        /// Creates a WinAuthProtocol object.
        /// </summary>
        protected WinAuthProtocol()
        {
        }

        /// <summary>
        /// Creates the command packet's header and returns a BinaryWriter so the caller
        /// can write the command packet's payload.
        /// </summary>
        /// <param name="ms">A MemoryStream to write the command packet.</param>
        /// <param name="command">A WinAuthProtocolCommands that specifies the command.</param>
        /// <returns>A BinaryWriter that contains the command packet's header.</returns>
        protected BinaryWriter GetBinaryWriter(MemoryStream ms, WinAuthProtocolCommands command)
        {
            BinaryWriter bw = new BinaryWriter(ms, Encoding.UTF8);
            bw.Write(WinAuthProtocol.PROTOCOL_IDENTIFIER);
            bw.Write((byte)command);
            return bw;
        }

        /// <summary>
        /// Log's a message.
        /// </summary>
        /// <param name="level">A Level that specifies the priority of the message.</param>
        /// <param name="message">A string that contains the message.</param>
        protected void Log(Level level, string message)
        {
            session.Log(level, string.Format("[WinAuth] {0}", message));
        }
    }
}
