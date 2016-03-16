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

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = null;
            try
            {
#if USE_APP_CONFIG
                client = new Client();
#else                
                ServerConfiguration cfg = new ServerConfiguration();
                cfg.Host = "localhost";
                cfg.TlsConfiguration.Enabled = false;

                Dictionary<ushort, ProtocolConfiguration> protocolConfigurations =
                    new Dictionary<ushort, ProtocolConfiguration>();

                protocolConfigurations.Add(KeepAliveProtocol.PROTOCOL_IDENTIFIER,
                    new ProtocolConfiguration(KeepAliveProtocol.PROTOCOL_IDENTIFIER, typeof(KeepAliveProtocol)));

                protocolConfigurations.Add(WinAuthProtocol.PROTOCOL_IDENTIFIER,
                    new ProtocolConfiguration(WinAuthProtocol.PROTOCOL_IDENTIFIER, typeof(WinAuthProtocolClient)));

                protocolConfigurations.Add(HelloProtocol.PROTOCOL_IDENTIFIER,
                    new ProtocolConfiguration(HelloProtocol.PROTOCOL_IDENTIFIER, typeof(HelloProtocolClient)));


                client = new Client(cfg, protocolConfigurations);
                //client.Logger.LogPackets = true;
                client.Logger.LogToDebuggerOutputView = true;
                client.Logger.LogDebug = true;
#endif

                client.Connect();

                ushort[] serverSupportedProtocolIds = client.GetServerSupportedProtocolIds();
                Console.WriteLine(string.Format(
                    "Server Supported Protocol IDs: {0}", 
                    string.Join(", ", serverSupportedProtocolIds)));

                string userName = "TestUser";
                WinAuthProtocolClient wap = client.Initialize(WinAuthProtocol.PROTOCOL_IDENTIFIER) as WinAuthProtocolClient;
                wap.Authenticate(userName, "T3stus3r", null);
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