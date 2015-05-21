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
			ConsoleLogger logger = new ConsoleLogger();
			logger.setLogPackets(true);
			
			ServerConfiguration cfg = new ServerConfiguration();
			//cfg.setHost("UpperSetting.com");
			            
	        HashMap<Integer, ProtocolConfiguration> protocolConfigurations =
	            new HashMap<Integer, ProtocolConfiguration>();
	        
	        protocolConfigurations.put(KeepAliveProtocol.PROTOCOL_IDENTIFIER,
	            new ProtocolConfiguration(KeepAliveProtocol.PROTOCOL_IDENTIFIER, "com.us.openserver.protocols.keepalive.KeepAliveProtocol"));
	
	        protocolConfigurations.put(WinAuthProtocol.PROTOCOL_IDENTIFIER,
	            new ProtocolConfiguration(WinAuthProtocol.PROTOCOL_IDENTIFIER, "com.us.openserver.protocols.winauth.WinAuthProtocolClient"));
	
	        protocolConfigurations.put(HelloProtocol.PROTOCOL_IDENTIFIER,
	            new ProtocolConfiguration(HelloProtocol.PROTOCOL_IDENTIFIER, "com.us.openserver.protocols.hello.HelloProtocolClient"));
	
	        client = new Client(this, cfg, protocolConfigurations, logger, null);
	        client.connect();
	        
	        String userName = "TestUser";
            WinAuthProtocolClient wap = (WinAuthProtocolClient)client.initialize(WinAuthProtocol.PROTOCOL_IDENTIFIER);
            if (!wap.authenticate(userName, "T3stus3r", null))
                throw new Exception("Access denied.");
            			
	        client.initialize(KeepAliveProtocol.PROTOCOL_IDENTIFIER);
	        
	        HelloProtocolClient hpc = (HelloProtocolClient)client.initialize(HelloProtocol.PROTOCOL_IDENTIFIER);	        
	        {
	        	String serverResponse = hpc.hello("Software Engineer");
	        	System.out.println("Hello(Sync): " + serverResponse);
	        }
	        {
	        	hpc.setHelloObserver(this);
	        	hpc.helloAsync("Software Engineer");
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
    
    public void onConnectionLost(Exception ex)
    {
    	System.out.println("Connection lost: " + ex.getMessage());
    }
    
    public void onHelloComplete(String serverResponse)
    {
    	System.out.println("Hello(Async): " + serverResponse);
    }
    
    public void log(Level level, String message)
    {
    	System.out.println(String.format("%1$s %2$s", level, message));
    }
    
    public void log(Exception ex)
    {
    	System.out.println(String.format("%1$s %2$s", Level.Error, ex.getMessage()));
    }
}
