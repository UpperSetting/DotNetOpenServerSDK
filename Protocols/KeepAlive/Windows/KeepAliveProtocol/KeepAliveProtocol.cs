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
using System.Text;
using System.Threading;

namespace US.OpenServer.Protocols.KeepAlive
{
    /// <summary>
    /// Class that implements the Keep-Alive Protocol.
    /// </summary>
    /// <remarks>
    /// Keeps idle sessions open and verifies connectivity (i.e. network failure).
    /// If the server does not receive a <see cref="KeepAliveProtocolCommands.KEEP_ALIVE"/>
    /// command packet from the client at least once every idle timeout (the default
    /// value is 5 minutes), the server closes the connection. The idle timeout can
    /// be overridden by setting the <see cref="US.OpenServer.SessionBase"/>.<see
    /// cref="US.OpenServer.Configuration.ServerConfiguration.IdleTimeout"/>
    /// property. <see cref="KeepAliveProtocolCommands.KEEP_ALIVE"/> command packets
    /// are sent once every 10 seconds. When there is an error sending a <see cref="KeepAliveProtocolCommands.KEEP_ALIVE"/>
    /// command packet, the <see cref="US.OpenServer.SessionBase"/>.<see
    /// cref="US.OpenServer.SessionBase.OnConnectionLost"/> event is triggered.
    /// </remarks>
    public class KeepAliveProtocol : ProtocolBase
    {
        #region Constants
        /// <summary>
        /// The unique protocol identifier
        /// </summary>
        public const ushort PROTOCOL_IDENTIFIER = 0x0001;
        
        /// <summary>
        /// Defines the number of milliseconds between 
        /// <see cref="KeepAliveProtocolCommands.KEEP_ALIVE"/> 
        /// command packets.
        /// </summary>
        private const int INTERVAL = 10000;

        /// <summary>
        /// Defines the idle timeout.  This value should be 3 times <see cref="INTERVAL"/>.
        /// </summary>
        private const int IDLE_TIMEOUT = 300000000;

        /// <summary>
        /// Debug message that is logged when a <see cref="KeepAliveProtocolCommands.KEEP_ALIVE"/> is received.
        /// </summary>
        private const string KEEPALIVE_RECEIVED = "Keep-Alive received.";

        /// <summary>
        /// Debug message that is logged when a <see cref="KeepAliveProtocolCommands.KEEP_ALIVE"/> is sent.
        /// </summary>
        private const string KEEPALIVE_SENT = "Keep-Alive sent.";

        /// <summary>
        /// Debug message that is logged when a <see cref="KeepAliveProtocolCommands.QUIT"/> is received.
        /// </summary>
        private const string QUIT_RECEIVED = "Quit received.";

        /// <summary>
        /// Debug message that is logged when a <see cref="KeepAliveProtocolCommands.QUIT"/> is sent.
        /// </summary>
        private const string QUIT_SENT = "Quit sent.";
        #endregion

        #region Private Variables
        /// <summary>
        /// Used to trigger 
        /// <see cref="KeepAliveProtocolCommands.KEEP_ALIVE"/> 
        /// command packets.
        /// </summary>
        private Timer timer;

        private DateTime lastHeartBeatReceivedAt = DateTime.Now;
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a KeepAliveProtocol object.
        /// </summary>
        public KeepAliveProtocol()
        {
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Saves a reference to the SessionBase then creates a Timer to send
        /// <see cref="KeepAliveProtocolCommands.KEEP_ALIVE"/>
        /// command packets.
        /// </summary>
        /// <param name="session">A SessionBase that encapsulates the connection
        /// session.</param>
        ///<param name="pc">A ProtocolConfiguration that contains configuration
        /// properties. This parameter is not used.</param>
        /// <param name="userData">An object that may be used by client applications to
        /// pass objects or data to client side protocol implementations. This parameter is
        /// not used.</param>
        public override void Initialize(SessionBase session, ProtocolConfiguration pc, object userData = null)
        {
            base.Initialize(session, pc, userData);

            lock (this)
            {
                if (timer != null)
                    timer.Dispose();

                timer = new Timer(TimerCallback, null, KeepAliveProtocol.INTERVAL, KeepAliveProtocol.INTERVAL);
            }
        }

        /// <summary>
        /// Shuts down the Keep-Alive Timer then sends a
        /// <see cref="KeepAliveProtocolCommands.QUIT"/> 
        /// command packet to the end point.
        /// </summary>
        public override void Close()
        {
            lock (this)
            {
                if (timer != null)
                    timer.Dispose();

                MemoryStream ms = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(ms, Encoding.UTF8);
                bw.Write(KeepAliveProtocol.PROTOCOL_IDENTIFIER);
                bw.Write((byte)KeepAliveProtocolCommands.QUIT);
                if (Session.Logger.LogDebug)
                    Log(Level.Debug, QUIT_SENT);

                try
                {
                    Session.Send(ms);
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Shuts down the Keep-Alive Timer.
        /// </summary>
        public override void Dispose()
        {
            lock (this)
            {
                if (timer != null)
                    timer.Dispose();
            }
        }

        /// <summary>
        /// Handles received command packets. Logs 
        /// <see cref="KeepAliveProtocolCommands.KEEP_ALIVE"/>
        /// command packets and closes the session when a 
        /// <see cref="KeepAliveProtocolCommands.QUIT"/> 
        /// command packet is received.
        /// </summary>
        /// <param name="br">A BinaryReader that contains the command packet.</param>
        public override void OnPacketReceived(BinaryReader br)
        {
            bool dispose = false;
            lock (this)
            {
                if (Session == null)
                    return;

                KeepAliveProtocolCommands command = (KeepAliveProtocolCommands)br.ReadByte();
                switch (command)
                {
                    case KeepAliveProtocolCommands.KEEP_ALIVE:
                        lastHeartBeatReceivedAt = DateTime.Now;
                        if (Session.Logger.LogDebug)
                            Log(Level.Debug, KEEPALIVE_RECEIVED);
                        break;
                    case KeepAliveProtocolCommands.QUIT:
                        Log(Level.Info, QUIT_RECEIVED);
                        dispose = true;
                        break;
                    default:
                        Log(Level.Error, string.Format("Invalid or unsupported command.  Command: {0}", command));
                        break;
                }
            }

            if (dispose)
                Session.ConnectionLost(new Exception("The connection was terminated by the remote end point."));
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// Log's a message.
        /// </summary>
        /// <param name="level">A Level that specifies the priority of the message.</param>
        /// <param name="message">A string that contains the message.</param>
        protected override void Log(Level level, string message)
        {
            Session.Log(level, string.Format("[Keep-Alive] {0}", message));
        }

        /// <summary>
        /// The Timer callback function that sends <see cref="KeepAliveProtocolCommands.KEEP_ALIVE"/>
        /// command packets to the end point.
        /// </summary>
        /// <param name="state">An object containing information to be used by the
        /// callback method, or null. This parameter is not used.</param>
        private void TimerCallback(object state)
        {
            Exception connectionLostException = null;
            lock (this)
            {
                try
                {
                    if (DateTime.Now.Ticks - lastHeartBeatReceivedAt.Ticks > IDLE_TIMEOUT)
                        throw new Exception("Idle connection detected.");

                    MemoryStream ms = new MemoryStream();
                    BinaryWriter bw = new BinaryWriter(ms, Encoding.UTF8);
                    bw.Write(KeepAliveProtocol.PROTOCOL_IDENTIFIER);
                    bw.Write((byte)KeepAliveProtocolCommands.KEEP_ALIVE);
                    Session.Send(ms);
                    Log(Level.Debug, "Sent.");
                }
                catch (Exception ex)
                {
                    timer.Dispose();
                    connectionLostException = ex;
                }
            }

            if (connectionLostException != null)
                Session.ConnectionLost(connectionLostException);
        }
        #endregion
    }
}
