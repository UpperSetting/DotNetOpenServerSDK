/*
Copyright 2015 Upper Setting Corporation

This file is part of DotNetOpenServer SDK.

DotNetOpenServer SDK is free software: you can redistribute it and/or modify it
under the terms of the GNU General Public License as published by the Free
Software Foundation, either version 3 of the License, or (at your option) any
later version.

DotNetOpenServer SDK is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
details.

You should have received a copy of the GNU General Public License along with
DotNetOpenServer SDK. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using US.OpenServer.Configuration;
using US.OpenServer.Protocols;

namespace US.OpenServer
{
    /// <summary>
    /// Abstract class that handles the connection session.  
    /// </summary>
    public abstract class SessionBase : IDisposable
    {
        #region Events
        /// <summary>
        /// Delegate that defines connection lost event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="ex">An Exception that contains the reason the connection was lost.</param>
        public delegate void ConnectionLostDelegate(object sender, Exception ex);

        /// <summary>
        /// Event that is triggered when the connection is lost.
        /// </summary>
        public event ConnectionLostDelegate OnConnectionLost;
        #endregion

        #region Variables
        /// <summary>
        /// A reference to the <see cref="CapabilitiesProtocol"/>.
        /// </summary>
        /// <remarks>This protocol is automatically created.</remarks>
        private CapabilitiesProtocol capabilitiesProtocol;

        /// <summary>
        /// A Dictionary of <see cref="ProtocolConfiguration"/> objects keyed by each
        /// protocol's unique identifier.
        /// </summary>
        private Dictionary<ushort, ProtocolConfiguration> protocolConfigurations;

        /// <summary>
        /// A Dictionary of <see cref="IProtocol"/> objects keyed by each protocol's
        /// unique identifier.
        /// </summary>
        private Dictionary<ushort, IProtocol> protocolImplementations = new Dictionary<ushort, IProtocol>();

        /// <summary>
        /// A user defined Object that is passed through to each <see cref="IProtocol"/>
        /// object.
        /// </summary>
        private object userData;

        /// <summary>
        /// An Object used to protect from Dispose being called multiple times from
        /// different threads.
        /// </summary>
        protected object syncObject = new object();
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a SessionBase.
        /// </summary>
        /// <param name="logger">An ILogger to log messages.</param>
        /// <param name="protocolConfigurations">A Dictionary that specifies the
        /// configured protocols.</param>
        /// <param name="userData">An optional Object the user can pass through to each
        /// protocol.</param>
        protected SessionBase(
            Dictionary<ushort, ProtocolConfiguration> protocolConfigurations,
            ILogger logger, 
            object userData = null)
        {
            capabilitiesProtocol = new CapabilitiesProtocol(this);
            this.protocolConfigurations = protocolConfigurations;
            this.Logger = logger;
            this.userData = userData;
            LastActivityAt = DateTime.Now;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets the IP address of the remote connection.
        /// </summary>
        /// <value>A string that contains the IP address of the remote connection.</value>
        public string Address { get; protected set; }

        /// <summary>
        /// Gets or sets the authentication protocol used to authenticate the user.
        /// </summary>
        /// <value>An AuthenticationProtocolBase used to authenticate the user.</value>
        public AuthenticationProtocolBase AuthenticationProtocol { get; set; }

        /// <summary>
        /// Gets or sets the session's unique identifier.
        /// </summary>      
        /// <value>An Int32 that specifies the session's unique identifier.</value>
        public int Id { get; protected set; }

        /// <summary>
        /// Gets whether the user has authenticated.
        /// </summary>
        /// <value>A Boolean that specifies whether the user has authenticated.</value>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// Gets or sets whether the connection is open or closed.
        /// </summary>
        /// <value>A Boolean that specifies whether the connection is open or
        /// closed.</value>
        public bool IsClosed { get; protected set; }

        /// <summary>
        /// Gets or sets the date and time the connection occurred or the last command
        /// packet was received.
        /// </summary>
        /// <value>A DateTime that specifies when the connection occurred or the last
        /// command packet was received.</value>
        public DateTime LastActivityAt { get; protected set; }

        /// <summary>
        /// Gets or sets the SSL/TLS 1.0 configuration properties.
        /// </summary>
        /// <value>A TlsConfiguration that specifies the configuration properties.</value>
        public TlsConfiguration TlsConfiguration { get; protected set; }

        /// <summary>
        /// Gets or sets the ILogger.
        /// </summary>
        /// <value>An ILogger.</value>
        public ILogger Logger { get; protected set; }
        
        /// <summary>
        /// Gets the user's name.
        /// </summary>
        /// <value>A string that contains the user's name.</value>
        public string UserName { get; set; }
        #endregion

        #region Public Functions
        /// <summary>
        /// Closes all the protocols allowing for command packet transmission.
        /// </summary>
        public virtual void Close()
        {
            lock (protocolImplementations)
            {
                foreach (IProtocol pl in protocolImplementations.Values)
                    pl.Close();

                protocolImplementations.Clear();
            }
            IsClosed = true;
        }

        /// <summary>
        /// Closes the specified protocol allowing for command packet transmission.
        /// </summary>
        /// <param name="protocolId">A UInt16 that specifies the unique protocol
        /// identifier.</param>
        public void Close(ushort protocolId)
        {
            IProtocol p = null;
            lock (protocolImplementations)
            {
                if (protocolImplementations.ContainsKey(protocolId))
                {
                    p = protocolImplementations[protocolId];
                    protocolImplementations.Remove(protocolId);
                }
            }

            if (p != null)
                p.Close();
        }

        /// <summary>
        /// Immediately closes all the protocols without allowing for command packet transmission.
        /// </summary>
        public virtual void Dispose()
        {
            lock (protocolImplementations)
            {
                foreach (IProtocol p in protocolImplementations.Values)
                    p.Dispose();

                protocolImplementations.Clear();
            }
            IsClosed = true;
        }

        /// <summary>
        /// Creates then initializes the protocol.
        /// </summary>
        /// <param name="protocolId">A UInt16 that specifies the unique protocol
        /// identifier.</param>
        /// <param name="userData">An object that may be used client applications to pass
        /// objects or data to client side protocol implementations.</param>
        /// <returns>An IProtocol that implements the protocol layer.</returns>
        public IProtocol Initialize(ushort protocolId, object userData = null)
        {
            IProtocol p = null;
            lock (protocolImplementations)
            {
                if (!protocolImplementations.ContainsKey(protocolId))
                {
                    if (!protocolConfigurations.ContainsKey(protocolId))
                        throw new Exception(ErrorTypes.INVALID_PROTOCOL);

                    ProtocolConfiguration pc = protocolConfigurations[protocolId];
                    p = pc.CreateInstance();
                    if (p == null)
                        throw new Exception(string.Format(ErrorTypes.CLASS_NOT_FOUND, pc));

                    protocolImplementations.Add(protocolId, p);

                    Log(Level.Debug, string.Format("Initializing protocol {0}...", protocolId));
                    if (userData == null)
                        userData = this.userData;
                    p.Initialize(this, pc, userData);
                    LastActivityAt = DateTime.Now;
                }
                else
                    p = protocolImplementations[protocolId];
            }
            return p;
        }

        /// <summary>
        /// Called when the connection has been lost or shutdown due to inactivity.
        /// </summary>
        /// <param name="ex">An exception that contains the reason the connection was lost.</param>
        public void ConnectionLost(Exception ex)
        {
            bool tmp;
            lock (syncObject)
            {
                tmp = IsClosed;
                if (!IsClosed)
                {
                    Log(Level.Critical, string.Format(ErrorTypes.CONNECTION_LOST, ex.Message));
                    Dispose();
                }
            }
            if (!tmp)
            {
                if (OnConnectionLost != null)
                    OnConnectionLost(this, ex);
            }
        }

        /// <summary>
        /// Gets a list of available protocol IDs on the remote connection from <see cref="CapabilitiesProtocol"/>.
        /// </summary>
        /// <returns>A UInt16 array of protocol IDs.</returns>
        public ushort[] GetRemoteSupportedProtocolIds()
        {
            return capabilitiesProtocol.GetRemoteSupportedProtocolIds();
        }

        /// <summary>
        /// Gets a list of locally available protocol IDs.
        /// </summary>
        /// <returns>A UInt16 array of protocol IDs.</returns>
        public ushort[] GetLocalSupportedProtocolIds()
        {
            ushort[] protocolIds = new ushort[protocolConfigurations.Count];
            protocolConfigurations.Keys.CopyTo(protocolIds, 0);
            return protocolIds;
        }

        /// <summary>
        /// Forwards remote protocol configuration errors to the local protocol.
        /// </summary>
        /// <param name="protocolId">A UInt16 that specifies the unique protocol
        /// identifier.</param>
        /// <param name="message">A String that contains the error message.</param>
        public void OnCapabilitiesError(ushort protocolId, string message)
        {
            IProtocol p = null;
            lock (protocolImplementations)
            {
                if (protocolImplementations.ContainsKey(protocolId))
                    p = protocolImplementations[protocolId];
            }
            if (p != null)
                p.OnErrorReceived(message);
        }

        /// <summary>
        /// Wraps the command packet around the Session Layer Protocol then sends the
        /// packet to the to the remote connection.
        /// </summary>
        /// <param name="payload">A MemoryStream that contains the command packet to
        /// send.</param>
        public void Send(MemoryStream payload)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms, Encoding.UTF8);

            bw.Write(SessionLayerProtocol.PROTOCAL_IDENTIFIER);

            byte[] buf = payload.ToArray();
            bw.Write(buf.Length);
            bw.Write(buf);

            buf = ms.ToArray();

            if (Logger.LogPackets)
                Log(Level.Debug, "SEND: " + ToHexString(buf, 0, buf.Length));

            Send(buf);
        }

        /// <summary>
        /// Sends the packet to the to the remote connection.
        /// </summary>
        /// <param name="buf">A byte array that contains the command packet.</param>
        protected abstract void Send(byte[] buf);
        #endregion

        #region Protected Functions
        /// <summary>
        /// Handles command packets.
        /// </summary>
        /// <param name="br">A BinaryReader that contains the command packet.</param>
        protected void OnPacketReceived(BinaryReader br)
        {
            ushort protocolId = br.ReadUInt16();
            if (protocolId == 0)
            {
                capabilitiesProtocol.OnPacketReceived(br);
                return;
            }

            try
            {
                IProtocol p;
                lock (protocolImplementations)
                {
                    if (protocolImplementations.ContainsKey(protocolId))
                        p = protocolImplementations[protocolId];
                    else
                    {
                        if (!protocolConfigurations.ContainsKey(protocolId))
                            throw new Exception(ErrorTypes.INVALID_PROTOCOL);

                        ProtocolConfiguration pc = protocolConfigurations[protocolId];
                        p = pc.CreateInstance();
                        if (p == null)
                            throw new Exception(string.Format(ErrorTypes.CLASS_NOT_FOUND, pc));

                        if (!IsAuthenticated && !(p is AuthenticationProtocolBase))
                            throw new Exception(string.Format(ErrorTypes.NOT_AUTHENTICATED, pc));

                        protocolImplementations.Add(protocolId, p);

                        Log(Level.Debug, string.Format("Initializing protocol {0}...", protocolId));
                        p.Initialize(this, pc, userData);
                    }
                    LastActivityAt = DateTime.Now;
                }
                p.OnPacketReceived(br);
            }
            catch (Exception ex)
            {
                capabilitiesProtocol.SendError(protocolId, ex.Message);
            }
        }

        /// <summary>
        /// Gets a string representation of a byte array in hexadecimal format.
        /// </summary>
        /// <param name="val">A byte array that contains the data to convert.</param>
        /// <param name="position">An Int32 that specifies the index to begin the
        /// conversion.</param>
        /// <param name="length">An Int32 that specifies the number of bytes to
        /// convert.</param>
        /// <returns>A string that contains a representation of the byte array in
        /// hexadecimal format.</returns>
        protected string ToHexString(byte[] val, int position, int length)
        {
            if (val == null || val.Length == 0)
                return string.Empty;

            StringBuilder s = new StringBuilder(val.Length * 2);
            for (int i = position; i < position + length; i++)
            {
                if (i == position)
                    s.Append(String.Format("{0:X2}", val[i]));
                else
                    s.Append(String.Format(" {0:X2}", val[i]));
            }
            return s.ToString();
        }
        #endregion

        #region Logging
        /// <summary>
        /// Log's a message.
        /// </summary>
        /// <param name="level">A Level that specifies the priority of the message.</param>
        /// <param name="message">A string that contains the message.</param>
        public void Log(Level level, string message)
        {
            Logger.Log(level, string.Format("Session [{0} {1}] - {2}", Id, Address, message));
        }

        /// <summary>
        /// Log's an exception.
        /// </summary>
        /// <param name="ex">An Exception to log.</param>
        protected void LogException(Exception ex)
        {
            Logger.Log(ex);
        }
        #endregion
    }
}
