using System;
using System.Collections.Generic;

namespace US.OpenServer.Protocols
{
    /// <summary>
    /// Static class that contains the list of enabled protocols and their
    /// configuration properties.
    /// </summary>
    public static class ProtocolConfigurations
    {
        private static Dictionary<ushort, ProtocolConfiguration> items = 
            new Dictionary<ushort, ProtocolConfiguration>();

        /// <summary>
        ///Sets the ProtocolConfiguration Items. Used internally by the server to
        ///during startup to add the ProtocolConfiguration items configured in the
        ///app.config file.
        /// </summary>
        /// <value>A Dictionary that specifies the configured protocols.</value>
        public static Dictionary<ushort, ProtocolConfiguration> Items
        {
            set { items = value; }
        }

        /// <summary>
        /// Determines whether Items contains the specified protocol identifier.
        /// </summary>
        /// <param name="id">A UInt16 that specifies the protocol identifier.</param>
        /// <returns>A Boolean that specifies if the protocol has been configured.</returns>
        public static bool ContainsKey(ushort id)
        {
            return items.ContainsKey(id);
        }

        /// <summary>
        /// Gets a ProtocolConfiguration given the specified protocol identifier. 
        /// </summary>
        /// <param name="id">A UInt16 that specifies the protocol identifier.</param>
        /// <returns>A ProtocolConfiguration that encapsulates the protocol
        /// configuration properties.</returns>
        public static ProtocolConfiguration Get(ushort id)
        {
            return items[id];
        }

        /// <summary>
        /// Adds a ProtocolConfiguration to Items.
        /// </summary>
        /// <param name="id">A UInt16 that specifies the protocol identifier.</param>
        /// <param name="protocolType">A Type that specifies the protocol class. The class
        /// must extend IProtocol.</param>
        /// <returns>A ProtocolConfiguration that encapsulates the protocol
        /// configuration properties.</returns>
        public static ProtocolConfiguration Add(ushort id, Type protocolType)
        {
            ProtocolConfiguration plc = new ProtocolConfiguration(id, protocolType);
            ProtocolConfigurations.items.Add(plc.Id, plc);
            return plc;
        }

        /// <summary>
        /// Adds a ProtocolConfiguration to Items.
        /// </summary>
        /// <param name="plc">A ProtocolConfiguration to add to Items.</param>
        public static void Add(ProtocolConfiguration plc)
        {
            ProtocolConfigurations.items.Add(plc.Id, plc);
        }
    }
}

