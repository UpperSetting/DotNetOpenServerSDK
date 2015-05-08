using System;
using System.Collections.Generic;
using US.OpenServer;
using US.OpenServer.Configuration;
using US.OpenServer.Protocols;
using US.OpenServer.Protocols.Hello;
using US.OpenServer.Protocols.KeepAlive;
using US.OpenServer.Protocols.WinAuth;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = null;
            try
            {
                //load configuration from the App.config
                //Client client = new Client();

                Logger logger = new Logger("DotNetOpenClient");

                ServerConfiguration cfg = new ServerConfiguration();
                //cfg.Host = "UpperSetting.com";

                Dictionary<ushort, ProtocolConfiguration> protocolConfigurations =
                    new Dictionary<ushort, ProtocolConfiguration>();

                protocolConfigurations.Add(KeepAliveProtocol.PROTOCOL_IDENTIFIER,
                    new ProtocolConfiguration(KeepAliveProtocol.PROTOCOL_IDENTIFIER, typeof(KeepAliveProtocol)));

                protocolConfigurations.Add(WinAuthProtocol.PROTOCOL_IDENTIFIER,
                    new ProtocolConfiguration(WinAuthProtocol.PROTOCOL_IDENTIFIER, typeof(WinAuthProtocolClient)));

                protocolConfigurations.Add(HelloProtocol.PROTOCOL_IDENTIFIER,
                    new ProtocolConfiguration(HelloProtocol.PROTOCOL_IDENTIFIER, typeof(HelloProtocolClient)));

                client = new Client(logger, cfg, protocolConfigurations);
                client.Connect();

                string userName = "TestUser";
                WinAuthProtocolClient wap = client.Initialize(WinAuthProtocol.PROTOCOL_IDENTIFIER) as WinAuthProtocolClient;
                wap.Authenticate(Environment.UserName, "T3stus3r", null);
                if (!wap.IsAuthenticated)
                    throw new Exception("Access denied.");

                client.Initialize(KeepAliveProtocol.PROTOCOL_IDENTIFIER);

                HelloProtocolClient hpc = (HelloProtocolClient)client.Initialize(HelloProtocol.PROTOCOL_IDENTIFIER);
                string serverReponse = hpc.Hello(userName);
                Console.WriteLine(serverReponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.ReadKey();
                if (client != null)
                    client.Close();
            }
        }
    }
}