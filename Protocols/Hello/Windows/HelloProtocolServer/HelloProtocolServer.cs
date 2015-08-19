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

using System.IO;
using System.Text;

namespace US.OpenServer.Protocols.Hello
{
    /// <summary>
    /// Class that is created by the Windows Server to show a sample server-side
    /// application layer protocol implementation.
    /// </summary>
    public class HelloProtocolServer : HelloProtocol
    {
        /// <summary>
        /// Creates a HelloProtocolServer object.
        /// </summary>
        public HelloProtocolServer()
        {
        }

        /// <summary>
        /// Handles the Hello command packet request.
        /// </summary>
        /// <remarks>
        /// Reads a String, logs the request to the <see cref="US.OpenServer.Logger"/>,
        /// generates a response message, creates a response command packet, sends the
        /// response to the client, then finally, logs the response to the <see cref="US.OpenServer.Logger"/>.
        /// </remarks>
        /// <param name="br">A BinaryReader that contains the command packet.</param>
        public override void OnPacketReceived(BinaryReader br)
        {
            string clientHello = br.ReadString();
            Log(Level.Info, string.Format("Client says: {0}", clientHello));
            string serverResponse = string.Format("Hello {0}", clientHello);
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms, Encoding.UTF8);
            bw.Write(HelloProtocol.PROTOCOL_IDENTIFIER);
            bw.Write(serverResponse);
            Session.Send(ms);
            Log(Level.Info, string.Format("Server responded: {0}", serverResponse));
        }
    }
}
