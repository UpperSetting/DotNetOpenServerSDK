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
using System.Threading.Tasks;
using US.OpenServer.Configuration;
using US.OpenServer.Protocols;
using Windows.Networking.Sockets;

namespace US.OpenServer.WindowsMobile
{
    public class Session : SessionBase
    {
        #region Variables
        StreamSocket streamSocket;
        private Stream iS;
        private Stream oS;
        #endregion

        #region Constructor
        public Session(
            StreamSocket streamSocket, 
            string address, 
            TlsConfiguration tlsConfiguration,
            Dictionary<ushort, ProtocolConfiguration> protocolConfigurations, 
            ILogger logger)
            : base(protocolConfigurations, logger)
        {
            this.streamSocket = streamSocket;
            this.iS = streamSocket.InputStream.AsStreamForRead();
            this.oS = streamSocket.OutputStream.AsStreamForWrite();
            this.Address = address;
            this.TlsConfiguration = tlsConfiguration;
            Id = SessionId.NextId;
        }
        #endregion

        #region Public Functions
        public void BeginRead()
        {
            Task.Run(() => ReadThread());       
        }

        public override void Close()
        {
            if (!IsClosed)
            {
                base.Close();
                try { iS.Dispose(); } catch (Exception) { }
                try { oS.Dispose(); } catch (Exception) { }
                try { streamSocket.Dispose(); } catch (Exception) { }
                Log(Level.Info, "Closed.");
            }
        }

        public override void Dispose()
        {
            if (!IsClosed)
            {
                base.Dispose();
                try { iS.Dispose(); } catch (Exception) { }
                try { oS.Dispose(); } catch (Exception) { }
                try { streamSocket.Dispose(); } catch (Exception) { }
                Log(Level.Info, "Disposed.");
            }
        }

        protected override void Send(byte[] buf)
        {
            lock (oS)
            {
                oS.Write(buf, 0, buf.Length);
                oS.Flush();
            }
        }
        #endregion

        #region Private Functions
        private void OnPacketReceivedThread(BinaryReader br)
        {
            try
            {
                OnPacketReceived(br);
            }
            catch (Exception ex)
            {
                LogException(ex);
                Close();
            }
        }

        private void ReadThread()
        {
            int available = 0;
            try
            {
                PacketReadTypes readState = PacketReadTypes.Header;
                byte[] buffer = new byte[8192];
                MemoryStream packet = new MemoryStream();
                int payloadLength  = 0;
                int payloadPosition = 0;
                int position;

                while ((available = iS.Read(buffer, 0, buffer.Length)) != -1)
                {
                    position = 0;

                    while (available > 0)
                    {
                        int lengthToRead;
                        if (readState == PacketReadTypes.Header)
                        {
                            lengthToRead = (int)packet.Position + available >= SessionLayerProtocol.HEADER_LENGTH ?
                                 SessionLayerProtocol.HEADER_LENGTH - (int)packet.Position :
                                 available;

                            packet.Write(buffer, position, lengthToRead);
                            position += lengthToRead;
                            available -= lengthToRead;

                            if (packet.Position >= SessionLayerProtocol.HEADER_LENGTH)
                                readState = PacketReadTypes.HeaderComplete;
                        }

                        if (readState == PacketReadTypes.HeaderComplete)
                        {
                            packet.Seek(0, SeekOrigin.Begin);
                            BinaryReader br = new BinaryReader(packet, Encoding.UTF8);

                            ushort protocolId = br.ReadUInt16();
                            if (protocolId != SessionLayerProtocol.PROTOCAL_IDENTIFIER)
                                throw new Exception(ErrorTypes.INVALID_PROTOCOL);

                            payloadLength = br.ReadInt32();
                            readState = PacketReadTypes.Payload;
                        }

                        if (readState == PacketReadTypes.Payload)
                        {
                            lengthToRead = available >= payloadLength - payloadPosition ?
                                payloadLength - payloadPosition :
                                available;

                            packet.Write(buffer, position, lengthToRead);
                            position += lengthToRead;
                            available -= lengthToRead;
                            payloadPosition += lengthToRead;

                            if (packet.Position >= SessionLayerProtocol.HEADER_LENGTH + payloadLength)
                            {
                                if (Logger.LogPackets)
                                    Log(Level.Debug, "RECV: " + ToHexString(packet.ToArray(), 0, (int)packet.Length));

                                MemoryStream handlerMS = new MemoryStream(packet.ToArray());
                                handlerMS.Seek(SessionLayerProtocol.HEADER_LENGTH, SeekOrigin.Begin);
                                BinaryReader br = new BinaryReader(handlerMS, Encoding.UTF8);

                                Task.Factory.StartNew(() => OnPacketReceivedThread(br));

                                readState = PacketReadTypes.Header;
                                packet = new MemoryStream();
                                payloadLength = 0;
                                payloadPosition = 0;
                            }
                        }
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                Close();
            }
            catch (Exception ex)
            {
                ConnectionLost(ex);
            }
        }
        #endregion
    }
}
