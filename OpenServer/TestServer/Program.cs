using System;
using System.Collections.Generic;
using US.OpenServer;
using US.OpenServer.Configuration;
using US.OpenServer.Protocols;
//using US.OpenServer.Protocols.Hello;
//using US.OpenServer.Protocols.KeepAlive;
//using US.OpenServer.Protocols.WinAuth;

namespace TestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = null;
            try
            {
                //load configuration from the App.config
                server = new Server();

                //set the configuration programatically
                //Logger logger = new Logger("DotNetOpenServer");
                //logger.LogPackets = true;

                //ServerConfiguration cfg = new ServerConfiguration();

                //Dictionary<ushort, ProtocolConfiguration> protocolConfigurations =
                //    new Dictionary<ushort, ProtocolConfiguration>();

                //protocolConfigurations.Add(KeepAliveProtocol.PROTOCOL_IDENTIFIER,
                //    new ProtocolConfiguration(KeepAliveProtocol.PROTOCOL_IDENTIFIER, typeof(KeepAliveProtocol)));

                //WinAuthProtocolConfigurationServer winAuthCfg =
                //    new WinAuthProtocolConfigurationServer(
                //        WinAuthProtocol.PROTOCOL_IDENTIFIER,
                //        typeof(WinAuthProtocolServer));
                //winAuthCfg.Roles.Add("Administrators");
                //winAuthCfg.Users.Add("TestUser");
                //protocolConfigurations.Add(WinAuthProtocol.PROTOCOL_IDENTIFIER, winAuthCfg);

                //protocolConfigurations.Add(HelloProtocol.PROTOCOL_IDENTIFIER,
                //    new ProtocolConfiguration(HelloProtocol.PROTOCOL_IDENTIFIER, typeof(HelloProtocolServer)));

                //Server server = new Server(logger, cfg, protocolConfigurations);

                server.Logger.Log(Level.Info, "Press any key to quit.");                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.ReadKey();
                if (server != null)
                    server.Close();
            }
        }
    }
}


