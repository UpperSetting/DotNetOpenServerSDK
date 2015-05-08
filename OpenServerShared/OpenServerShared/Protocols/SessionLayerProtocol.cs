
namespace US.OpenServer.Protocols
{
    /// <summary>
    /// Static class that defines constants for the Session Layer Protocol.
    /// </summary>
    public static class SessionLayerProtocol
    {
        /// <summary>
        /// The protocol identifier.
        /// </summary>
        public const ushort PROTOCAL_IDENTIFIER = 21843;//U 0x55, S 0x53

        /// <summary>
        /// The length of the protocol identifier.
        /// </summary>
        public const int PROTOCOL_IDENTIFIER_LENGTH = 2;

        /// <summary>
        /// The length of the length field.
        /// </summary>
        public const int LENGTH_LENGTH = 4;

        /// <summary>
        /// The header length.
        /// </summary>
        public const int HEADER_LENGTH = PROTOCOL_IDENTIFIER_LENGTH + LENGTH_LENGTH;

        /// <summary>
        /// The default server port.
        /// </summary>
        public const ushort PORT = 21843;
    }
}
