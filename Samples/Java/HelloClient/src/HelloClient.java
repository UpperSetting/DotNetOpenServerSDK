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

import com.us.openserver.*;
import com.us.openserver.configuration.*;
import com.us.openserver.protocols.*;
import com.us.openserver.protocols.hello.*;
import com.us.openserver.protocols.keepalive.*;
import com.us.openserver.protocols.winauth.*;

import java.io.IOException;
import java.util.HashMap;

public class HelloClient implements IClientObserver, IHelloProtocolObserver
{
    private Client client;
    
    public static void main(String[] args) 
    {
        new HelloClient(args);
    }
    
    public HelloClient(String[] args) 
    {
        try
        {
            ServerConfiguration cfg = new ServerConfiguration();
            //cfg.setHost("yourserver.com");
            //TlsConfiguration tls = cfg.getTlsConfiguration();
            //tls.setEnabled(true);
                        
            HashMap<Integer, ProtocolConfiguration> protocolConfigurations =
                new HashMap<Integer, ProtocolConfiguration>();
            
            protocolConfigurations.put(KeepAliveProtocol.PROTOCOL_IDENTIFIER,
                new ProtocolConfiguration(KeepAliveProtocol.PROTOCOL_IDENTIFIER, "com.us.openserver.protocols.keepalive.KeepAliveProtocol"));
    
            protocolConfigurations.put(WinAuthProtocol.PROTOCOL_IDENTIFIER,
                new ProtocolConfiguration(WinAuthProtocol.PROTOCOL_IDENTIFIER, "com.us.openserver.protocols.winauth.WinAuthProtocolClient"));
    
            protocolConfigurations.put(HelloProtocol.PROTOCOL_IDENTIFIER,
                new ProtocolConfiguration(HelloProtocol.PROTOCOL_IDENTIFIER, "com.us.openserver.protocols.hello.HelloProtocolClient"));
    
            client = new Client(this, cfg, protocolConfigurations);
            client.connect();
            
            int[] protocolIds = client.getServerSupportedProtocolIds();
            for (int protocolId : protocolIds)
            	System.out.println(String.format("Server Supports Protocol ID: %d", protocolId));

            String userName = "TestUser";
            WinAuthProtocolClient wap = (WinAuthProtocolClient)client.initialize(WinAuthProtocol.PROTOCOL_IDENTIFIER);
            if (!wap.authenticate(userName, "T3stus3r", null))
                throw new Exception("Access denied.");

            client.initialize(KeepAliveProtocol.PROTOCOL_IDENTIFIER);
            
            HelloProtocolClient hpc = (HelloProtocolClient)client.initialize(HelloProtocol.PROTOCOL_IDENTIFIER);            
            {
                String serverResponse = hpc.hello(userName);
                System.out.println("Hello(Sync): " + serverResponse);
            }
            {
                hpc.setHelloObserver(this);
                hpc.helloAsync(userName);
            }
        }
        catch (Exception ex)
        {
            System.out.println(ex.getMessage());
        }
        finally
        {
            try 
            { 
                System.in.read(); 
                if (client != null)
                    client.close();
            } 
            catch (IOException ex) 
            {
            }
        }
    }
    
    @Override
    public void onConnectionLost(Exception ex)
    {
        System.out.println("Connection lost. " + ex.getMessage());
    }
    
    @Override
    public void onHelloComplete(String serverResponse)
    {
        System.out.println("Hello(Async): " + serverResponse);
    }
}