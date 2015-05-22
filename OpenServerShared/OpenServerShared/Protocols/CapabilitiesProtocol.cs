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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace US.OpenServer.Protocols
{
    /// <summary>
    /// Class that is created by Windows and Windows Mobile applications to retrieve
    /// a list of available protocols and pass protocol configuration errors back to
    /// the remote connection.
    /// </summary>
    public class CapabilitiesProtocol : IProtocol
    {
        /// <summary>
        /// The unique protocol identifier
        /// </summary>
        public const ushort PROTOCOL_IDENTIFIER = 0x0000;

        /// <summary>
        /// Defines the maximum number of milliseconds to wait for an authentication request. 
        /// </summary>
        private const int TIMEOUT = 120000;

        public ushort[] AvailableProtocolIds { get; private set; }

        /// <summary>
        /// Creates a CapabilitiesProtocol object.
        /// </summary>
        /// <param name="session">A SessionBase that implements the connection session.</param>
        public CapabilitiesProtocol(SessionBase session)
        {
            this.session = session;
        }

        /// <summary>
        /// Gets a list of available protocol IDs on the remote connection.
        /// </summary>
        /// <returns>A UInt16 array of protocol IDs.</returns>
        public ushort[] GetRemoteSupportedProtocolIds()
        {
            lock (this)
            {
                MemoryStream ms = new MemoryStream();
                BinaryWriter bw = GetBinaryWriter(ms, CapabilitiesProtocolCommands.GET_PROTOCOL_IDS);
                session.Send(ms);

                if (!Monitor.Wait(this, TIMEOUT))
                    throw new TimeoutException();

                return AvailableProtocolIds;
            }
        }

        /// <summary>
        /// Sends the protocol ID and message to the remote connection.
        /// </summary>
        /// <returns>A UInt16 array of protocol IDs.</returns>
        public void SendError(ushort protocolId, string message)
        {
            lock (this)
            {
                MemoryStream ms = new MemoryStream();
                BinaryWriter bw = GetBinaryWriter(ms, CapabilitiesProtocolCommands.ERROR);
                bw.Write(protocolId);
                bw.WriteString(message);
                session.Send(ms);
            }
        }

        /// <summary>
        /// Handles the <see cref="US.OpenServer.Protocols.CapabilitiesProtocolCommands.PROTOCOL_IDS"/> and
        /// <see cref="US.OpenServer.Protocols.CapabilitiesProtocolCommands.ERROR"/>
        /// command packet request. 
        /// </summary>
        /// <param name="br">A BinaryReader that contains the command packet.</param>
        public override void OnPacketReceived(BinaryReader br)
        {
            ushort protocolId = 0;
            string errorMessage = null;
            
            CapabilitiesProtocolCommands command = (CapabilitiesProtocolCommands)br.ReadByte();
            switch (command)
            {
                case CapabilitiesProtocolCommands.GET_PROTOCOL_IDS:
                    {
                        ushort[] protocolIds = session.GetLocalSupportedProtocolIds();
                        MemoryStream ms = new MemoryStream();
                        BinaryWriter bw = GetBinaryWriter(ms, CapabilitiesProtocolCommands.PROTOCOL_IDS);
                        bw.WriteUInt16s(protocolIds);
                        Log(Level.Debug, string.Format("Sent Protocol IDs: {0}", string.Join(", ", protocolIds.ToArray())));
                        session.Send(ms);
                        break;
                    }
                case CapabilitiesProtocolCommands.PROTOCOL_IDS:
                    lock (this)
                    {
                        AvailableProtocolIds = br.ReadUInt16s();
                        Log(Level.Debug, string.Format("Received Protocol IDs: {0}", string.Join(", ", AvailableProtocolIds.ToArray())));
                        Monitor.PulseAll(this);
                    }
                    break;

                case CapabilitiesProtocolCommands.ERROR:
                    lock (this)
                    {
                        protocolId = br.ReadUInt16();
                        errorMessage = br.ReadString();
                        Log(Level.Error, errorMessage);
                        Monitor.PulseAll(this);
                    }
                    break;

                default:
                    Log(Level.Error, string.Format("Invalid or unsupported command.  Command: {0}", command));
                    break;
            }

            if (!string.IsNullOrEmpty(errorMessage))
                session.OnCapabilitiesError(protocolId, errorMessage);
        }

        /// <summary>
        /// Creates the command packet's header and returns a BinaryWriter so the caller
        /// can write the command packet's payload.
        /// </summary>
        /// <param name="ms">A MemoryStream to write the command packet.</param>
        /// <param name="command">A CapabilitiesProtocolCommands that specifies the command.</param>
        /// <returns>A BinaryWriter that contains the command packet's header.</returns>
        private BinaryWriter GetBinaryWriter(MemoryStream ms, CapabilitiesProtocolCommands command)
        {
            BinaryWriter bw = new BinaryWriter(ms, Encoding.UTF8);
            bw.Write(PROTOCOL_IDENTIFIER);
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
            session.Log(level, string.Format("[Capabilities] {0}", message));
        }
    }
}
