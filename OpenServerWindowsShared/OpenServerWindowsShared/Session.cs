/*
Copyright 2015-2016 Upper Setting Corporation

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
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using US.OpenServer.Configuration;
using US.OpenServer.Protocols;

namespace US.OpenServer
{
    /// <summary>
    /// Class used by both the US.OpenServer.Server and US.OpenServer.Client.Client
    /// that implements the connection session.
    /// </summary>
    public class Session : SessionBase
    {
        #region Variables
        /// <summary>
        /// Buffer used to read bytes sent from the remote end point.
        /// </summary>
        private byte[] buffer = new byte[8192];

        /// <summary>
        /// The length of a command packet's payload.
        /// </summary>
        private int payloadLength;

        /// <summary>
        /// When command packets span multiple TCP packets, the current position within the payload.
        /// </summary>
        private int payloadPosition;

        /// <summary>
        /// Contains command packets.
        /// </summary>
        private MemoryStream packet = new MemoryStream();

        /// <summary>
        /// Used to control packet read functionality.
        /// </summary>
        private PacketReadTypes readState;

        /// <summary>
        /// The socket stream.
        /// </summary>
        private Stream stream;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates the session and saves references to the passed objects.
        /// </summary>
        /// <param name="stream">The socket stream.</param>
        /// <param name="address">The IP address of the remote end point.</param>
        /// <param name="tlsConfiguration">The SSL/TLS configuration object.</param>
        /// <param name="protocolConfigurations">A Dictionary of ProtocolConfiguration
        /// objects keyed by each protocol's unique identifier.</param>
        /// <param name="logger">The Logger.</param>
        /// <param name="userData">An optional Object the user can pass through to each protocol.</param>
        public Session(
            Stream stream, 
            string address, 
            TlsConfiguration tlsConfiguration, 
            Dictionary<ushort, ProtocolConfiguration> protocolConfigurations,
            Logger logger, 
            object userData = null)
            : base(protocolConfigurations, logger, userData)
        {
            this.stream = stream;
            this.Address = address;
            this.TlsConfiguration = tlsConfiguration;
            Id = SessionId.NextId;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The socket stream. This parameters is used by both the US.OpenServer.Server
        /// and Client US.OpenServer.Client.Client to upgrade the
        /// connection to use TLS.
        /// </summary>
        public Stream Stream { get { return stream; } set { stream = value; } }
        #endregion

        #region Public Functions
        /// <summary>
        /// An asynchronous call invoked from both the US.OpenServer.Server
        /// and Client US.OpenServer.Client.Client to begin reading
        /// packets from the remote end point.
        /// </summary>
        public void BeginRead()
        {
            stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(ReadCallback), null);
        }

        /// <summary>
        /// Closes the <see cref="SessionBase"/>, closes the socket
        /// stream, then reinitializes the packet read variables.
        /// </summary>
        public override void Close()
        {
            if (!IsClosed)
            {
                base.Close();
                stream.Close();
                Reset();
                Log(Level.Info, "Closed.");
            }
        }

        /// <summary>
        /// Disposes the <see cref="SessionBase"/>, closes the socket
        /// stream, then reinitializes the packet read variables.
        /// </summary>
        public override void Dispose()
        {
            lock (syncObject)
            {
                if (!IsClosed)
                {
                    base.Dispose();
                    stream.Close();
                    Reset();
                    Log(Level.Info, "Disposed.");
                }
            }
        }

        /// <summary>
        /// Locks the socket stream to prevent multiple threads from writing to the
        /// socket stream at the same time then sends the command packet to the remote
        /// end point.
        /// </summary>
        /// <param name="buf">A Byte[] that contains the command packet to send.</param>
        protected override void Send(byte[] buf)
        {
            lock (stream)
                stream.Write(buf, 0, buf.Length);
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// A thread pool function that is queued from <see cref="US.OpenServer.Session.ReadCallback(IAsyncResult)"/>
        /// when a command or response packet is been received.
        /// </summary>
        /// <param name="userData">A BinaryReader that contains the command packet.</param>
        private void OnPacketReceivedThreadPoolCallback(Object userData)
        {
            try
            {
                OnPacketReceived((BinaryReader)userData);
            }
            catch (Exception ex)
            {
                LogException(ex);
                Close();
            }
        }

        /// <summary>
        /// A callback function that is called when a command packet is received.
        /// </summary>
        /// <param name="ar">An IAsyncResult that contains the command packet.</param>
        private void ReadCallback(IAsyncResult ar)
        {
            try
            {
                int available = stream.EndRead(ar);
                int position = 0;
                
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

                            if (!ThreadPool.QueueUserWorkItem(OnPacketReceivedThreadPoolCallback, br))
                                throw new Exception(ErrorTypes.NO_MORE_THREADS_AVAILABLE);

                            Reset();
                        }
                    }
                }

                stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(ReadCallback), null);
            }
            catch (Exception ex)
            {
                ConnectionLost(ex);
            }
        }

        /// <summary>
        /// Reinitializes the packet read variables.
        /// </summary>
        private void Reset()
        {
            readState = PacketReadTypes.Header;
            packet = new MemoryStream();
            payloadLength = 0;
            payloadPosition = 0;
        }
        #endregion

        #region TLS
        /// <summary>
        /// Called by the US.OpenServer.Server and the US.OpenServer.Client.Client
        /// to verify the remote Secure Sockets Layer (SSL) certificate used for
        /// authentication.
        /// </summary>
        /// <param name="sender">An object that contains state information for this validation.</param>
        /// <param name="certificate">The certificate used to authenticate the remote party.</param>
        /// <param name="chain">The chain of certificate authorities associated with the remote certificate.</param>
        /// <param name="sslPolicyErrors">One or more errors associated with the remote certificate.</param>
        /// <returns>True if the specified certificate is accepted for authentication, otherwise False.</returns>
        public bool TlsCertificateValidationCallback(
            object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNameMismatch))
            {
                Log(Level.Error, "[TLS] Remote certificate name mismatch.");
                return false;
            }

            if (sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNotAvailable))
            {
                if (TlsConfiguration.RequireRemoteCertificate)
                {
                    Log(Level.Error, "[TLS] Remote certificate not available.");
                    return false;
                }
                else
                {
                    Log(Level.Debug, "[TLS] Remote certificate not available.");
                }
            }

            if (sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateChainErrors))
            {
                if (!TlsConfiguration.AllowCertificateChainErrors)
                {
                    Log(Level.Error, "[TLS] Remote certificate contains chain errors.");
                    return false;
                }
                else
                {
                    Log(Level.Debug, "[TLS] Remote certificate contains chain errors.");
                }
            }
            Log(Level.Info, "[TLS] Certificate validated.");
            return true;
        }

        /// <summary>
        /// Called by the US.OpenServer.Server and the Client US.OpenServer.Client.Client
        /// to select the local Secure Sockets Layer (SSL) certificate used for
        /// authentication.
        /// </summary>
        /// <param name="sender">An object that contains state information for this validation.</param>
        /// <param name="targetHost">The host server specified by the client.</param>
        /// <param name="localCertificates">An <see cref="System.Security.Cryptography.X509Certificates.X509CertificateCollection"/>
        /// containing local certificates.</param>
        /// <param name="remoteCertificate">The certificate used to authenticate the remote party.</param>
        /// <param name="acceptableIssuers">A String array of certificate issuers acceptable to the remote party.</param>
        /// <returns>A <see cref="System.Security.Cryptography.X509Certificates.X509Certificate"/>
        /// used for establishing an SSL connection.</returns>
        public X509Certificate TlsCertificateSelectionCallback(
            object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            return localCertificates != null && localCertificates.Count > 0 ? localCertificates[0] : null;
        }

        /// <summary>
        /// Gets the named certificate from the local certificate store.
        /// </summary>
        /// <param name="name">The name of the certificate.  For example, UpperSetting.com</param>
        /// <returns>A <see cref="System.Security.Cryptography.X509Certificates.X509Certificate2"/>
        /// certificate. If the certificate is not found, null.</returns>
        public static X509Certificate2 GetCertificateFromStore(string name)
        {
            X509Store store = new X509Store(StoreLocation.CurrentUser);
            try
            {
                store.Open(OpenFlags.ReadOnly);

                X509Certificate2Collection certCollection = store.Certificates;
                X509Certificate2Collection currentCerts = certCollection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
                X509Certificate2Collection signingCert = currentCerts.Find(X509FindType.FindBySubjectDistinguishedName, name, false);
                if (signingCert.Count == 0)
                    return null;
                
                return signingCert[0];
            }
            finally
            {
                store.Close();
            }
        }
        #endregion
    }
}
