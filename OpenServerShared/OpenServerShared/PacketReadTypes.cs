
namespace US.OpenServer
{
    /// <summary>
    /// Enum of packet read states. Defines the current state of the socket reader
    /// while reading command packets from TCP packets.
    /// </summary>
    public enum PacketReadTypes
    {
        /// <summary>
        /// The initial state. A new command packet has been received. The socket reader is
        /// to read the header.
        /// </summary>
        Header,

        /// <summary>
        /// The socket reader has finished reading the header. The socket reader is to read
        /// the payload.
        /// </summary>
        HeaderComplete,

        /// <summary>
        /// The socket reader is currently reading the payload. The payload may be received
        /// over multiple TCP packets.
        /// </summary>
        Payload,
    }
}
