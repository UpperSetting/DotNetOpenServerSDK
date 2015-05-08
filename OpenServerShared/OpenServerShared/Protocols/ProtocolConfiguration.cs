using System;

namespace US.OpenServer.Protocols
{
    /// <summary>
    /// Class that encapsulates the properties necessary for Reflection to load
    /// IProtocol classes.
    /// </summary>
    public class ProtocolConfiguration
    {
        /// <summary>
        /// Gets or sets the protocol identifier.
        /// </summary>
        /// <value>A UInt16 that specifies the protocol identifier.</value>
        public ushort Id { get; protected set; }

        /// <summary>
        /// Gets or sets the class type of the IProtocol.
        /// </summary>
        /// <value>A Type that specifies the type of the IProtocol.</value>
        public Type ProtocolType { get; protected set; }

        /// <summary>
        /// Creates an instance of ProtocolConfiguration.
        /// </summary>
        protected ProtocolConfiguration()
        {
        }

        /// <summary>
        /// Creates an instance of ProtocolConfiguration given the protocol identifier
        /// and class Type.
        /// </summary>
        /// <param name="id">A UInt16 that specifies the protocol identifier.</param>
        /// <param name="protocolType">A Type that specifies the protocol class. The class
        /// must extend IProtocol.</param>
        public ProtocolConfiguration(ushort id, Type protocolType)
        {
            Id = id;
            ProtocolType = protocolType;
        }

        /// <summary>
        /// Creates an instances of the IProtocol class.
        /// </summary>
        /// <returns>An IProtocol.</returns>
        public virtual IProtocol CreateInstance()
        {
            return ProtocolType != null ? 
                (IProtocol)Activator.CreateInstance(ProtocolType) : null;
        }

        /// <summary>
        /// Returns the name of the Type of the IProtocol class.
        /// </summary>
        /// <returns>A string that specifies the name of the Type of the IProtocol
        /// class.</returns>

        public override string ToString()
        {
            return ProtocolType.Name.ToString();
        }
    }
}
