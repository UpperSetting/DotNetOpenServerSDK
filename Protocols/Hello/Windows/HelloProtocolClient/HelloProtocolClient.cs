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

namespace US.OpenServer.Protocols.Hello
{
    /// <summary>
    /// Class that is created by Windows and Windows Mobile client applications to
    /// show a sample client-side application layer protocol implementation.
    /// </summary>
    public class HelloProtocolClient : HelloProtocol
    {
        /// <summary>
        /// Delegate that defines the <see cref="OnHelloComplete"/> event handler.
        /// </summary>
        /// <param name="serverResponse">A String that contains the server's response message.</param>
        public delegate void OnHelloCompleteDelegate(string serverResponse);

        /// <summary>
        /// Event that triggers after the server processes the client call to <see cref="HelloAsync"/>.
        /// </summary>
        public event OnHelloCompleteDelegate OnHelloComplete;

        /// <summary>
        /// A String that contains the last server's response to a Hello command packet.
        /// </summary>
        private string serverResponse;

        /// <summary>
        /// Creates a HelloProtocolClient object.
        /// </summary>
        public HelloProtocolClient()
        {
        }

        /// <summary>
        /// Handles the response to the <see cref="Hello"/> command packet. 
        /// </summary>
        /// <remarks>
        /// Reads a String, logs the response to the <see cref="US.OpenServer.ILogger"/>,
        /// then forwards the response to the <see cref="OnHelloComplete"/> event
        /// handlers.
        /// </remarks>
        /// <param name="br">A BinaryReader that contains the command packet.</param>
        public override void OnPacketReceived(BinaryReader br)
        {
            lock (this)
            {
                serverResponse = br.ReadString();
                Log(Level.Info, string.Format("Server responded: {0}", serverResponse));

                Monitor.PulseAll(this);
            }

            if (OnHelloComplete != null)
                OnHelloComplete(serverResponse);
        }

        /// <summary>
        /// A simple hello world function that sends the passed message to the server
        /// then waits for and returns the server's response.
        /// </summary>
        /// <param name="message">A String that contains the message to send to the
        /// server.</param>
        /// <returns>A String that contains the server's response.</returns>
        public string Hello(string message)
        {
            lock (this)
            {
                HelloAsync(message);
                if (!Monitor.Wait(this, 10000))
                    throw new TimeoutException();
            }
            return serverResponse;
        }

        /// <summary>
        /// A simple hello world function that sends the passed message to the server
        /// then immediately returns. To receive notification of the server's response,
        /// add an event handler to <see cref="OnHelloComplete"/>.
        /// </summary>
        /// <param name="message">A String that contains the message to send to the
        /// server.</param>
        public void HelloAsync(string message)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms, Encoding.UTF8);
            bw.Write(HelloProtocol.PROTOCOL_IDENTIFIER);
            bw.WriteString(message);
            session.Send(ms);
            Log(Level.Info, string.Format("Client says: {0}", message));
        }
    }
}
