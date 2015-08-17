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
using US.OpenServer;
using US.OpenServer.Configuration;
using US.OpenServer.Protocols;
using US.OpenServer.Protocols.Hello;
using US.OpenServer.Protocols.KeepAlive;
using US.OpenServer.Protocols.WinAuth;

namespace TestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = null;
            try
            {
#if USE_APP_CONFIG
                server = new Server();
#else                
                ServerConfiguration cfg = new ServerConfiguration();
                cfg.Host = "0.0.0.0";//bind address
                cfg.TlsConfiguration.Enabled = false;                

                Dictionary<ushort, ProtocolConfiguration> protocolConfigurations =
                    new Dictionary<ushort, ProtocolConfiguration>();

                protocolConfigurations.Add(KeepAliveProtocol.PROTOCOL_IDENTIFIER,
                    new ProtocolConfiguration(KeepAliveProtocol.PROTOCOL_IDENTIFIER, typeof(KeepAliveProtocol)));

                WinAuthProtocolConfigurationServer winAuthCfg =
                    new WinAuthProtocolConfigurationServer(
                        WinAuthProtocol.PROTOCOL_IDENTIFIER,
                        typeof(WinAuthProtocolServer));
                winAuthCfg.AddRole("Administrators");
                winAuthCfg.AddUser("TestUser");
                protocolConfigurations.Add(WinAuthProtocol.PROTOCOL_IDENTIFIER, winAuthCfg);

                protocolConfigurations.Add(HelloProtocol.PROTOCOL_IDENTIFIER,
                    new ProtocolConfiguration(HelloProtocol.PROTOCOL_IDENTIFIER, typeof(HelloProtocolServer)));

                server = new Server(cfg, protocolConfigurations);
#endif
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


