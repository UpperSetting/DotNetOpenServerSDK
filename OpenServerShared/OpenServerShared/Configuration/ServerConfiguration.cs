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

using US.OpenServer.Protocols;

namespace US.OpenServer.Configuration
{
    /// <summary>
    /// Class that contains the client/server TCP socket server configuration properties.
    /// </summary>
    public class ServerConfiguration
    {
        #region Connection Constants
        /// <summary>
        /// The default IP address to bind the TCP socket server.
        /// </summary>
        public const string DEFAULT_BIND_ADDRESS = "0.0.0.0";

        /// <summary>
        /// The default host the server is running.
        /// </summary>
        public const string DEFAULT_HOST = "localhost";

        private const int IDLE_TIMEOUT = 300;//seconds
        private const int RECEIVE_TIMEOUT = 120;//seconds
        private const int SEND_TIMEOUT = 120;//seconds
        #endregion

        #region Private Variables
        private int idleTimeout;
        private int receiveTimeout;
        private int sendTimeout;
        #endregion

        /// <summary>
        /// Creates a new instance of the ServerConfiguration class and sets the
        /// properties to their default values.
        /// </summary>
        public ServerConfiguration()
        {
            Port = SessionLayerProtocol.PORT;
            IdleTimeout = IDLE_TIMEOUT;
            ReceiveTimeout = RECEIVE_TIMEOUT;
            SendTimeout = SEND_TIMEOUT;
            TlsConfiguration = new TlsConfiguration();
        }

        /// <summary>
        /// <para> When used from a Windows server, gets and sets the IP address to
        /// bind the TCP socket server. Defaults to 0.0.0.0 (all IP addresses). </para>
        /// <para>When used from a Windows client or Windows Mobile client, gets and
        /// sets the Host the server is running. Defaults to localhost.</para>
        /// </summary>
        /// <value>Depending if used from the server or client, a string that specifies
        /// the IP address to bind to or the name of the host the server is
        /// running.</value>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the TCP port the server is either to run, when referenced from the
        /// server, or is running, when referenced from the client.
        /// </summary>
        /// <value>A UInt16 that specifies the server's TCP port. The default value is
        /// 21843.</value>
        public ushort Port { get; set; }

        /// <summary>
        /// Gets or sets the amount of time a connection can remain idle before the
        /// connection automatically closed.
        /// </summary>
        /// <value>A Int32 that specifies the maximum number of seconds allowed to pass
        /// between command packets. Once exceeded, the remote connection is closed. The
        /// default value is 300 seconds.</value>
        public int IdleTimeout
        {
            get { return idleTimeout; }
            set
            {
                idleTimeout = value;
                IdleTimeoutInTicks = value * 1000000;
            }
        }

        /// <summary>
        /// Gets or sets the amount of time that a read operation blocks waiting for data. 
        /// </summary>
        /// <value>A Int32 that specifies the amount of time, in seconds, that will elapse
        /// before a read operation fails. The default value is 120 seconds.</value>
        public int ReceiveTimeout
        {
            get { return receiveTimeout; }
            set
            {
                receiveTimeout = value;
                ReceiveTimeoutInMS = value * 1000;
            }
        }

        /// <summary>
        /// Gets or sets the amount of time that a write operation blocks waiting for data. 
        /// </summary>
        /// <value>A Int32 that specifies the amount of time, in seconds, that will elapse
        /// before a write operation fails. The default value is 120 seconds.</value>
        public int SendTimeout
        {
            get { return sendTimeout; }
            set
            {
                sendTimeout = value;
                SendTimeoutInMS = value * 1000;
            }
        }

        /// <summary>
        /// Gets or sets the SSL/TLS 1.2 configuration properties.
        /// </summary>
        /// <value>A TlsConfiguration that encapsulates the SSL/TLS 1.2 configuration
        /// properties.</value>
        public TlsConfiguration TlsConfiguration { get; set; }

        /// <summary>
        /// Gets the IdleTimeout in ticks.
        /// </summary>
        /// <value>A Int64 that specifies the IdleTimeout in ticks.</value>
        public long IdleTimeoutInTicks { get; private set; }

        /// <summary>
        /// Gets the ReceiveTimeout in milliseconds.
        /// </summary>
        /// <value>A Int32 that specifies the ReceiveTimeout in milliseconds.</value>
        public int ReceiveTimeoutInMS { get; private set; }

        /// <summary>
        /// Gets the SendTimeout in milliseconds.
        /// </summary>
        /// <value>A Int32 that specifies the SendTimeout in milliseconds.</value>
        public int SendTimeoutInMS { get; private set; }
    }
}
