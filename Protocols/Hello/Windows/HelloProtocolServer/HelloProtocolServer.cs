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
        /// Reads a String, logs the request to the <see cref="US.OpenServer.ILogger"/>,
        /// generates a response message, creates a response command packet, sends the
        /// response to the client, then finally, logs the response to the <see cref="US.OpenServer.ILogger"/>.
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
            session.Send(ms);
            Log(Level.Info, string.Format("Server responded: {0}", serverResponse));
        }
    }
}
