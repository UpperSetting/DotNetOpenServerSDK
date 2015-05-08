import java.util.HashMap;

import com.us.openserver.*;
import com.us.openserver.configuration.*;
import com.us.openserver.util.*;
import com.us.openserver.protocols.hello.*;
import com.us.openserver.protocols.keepalive.*;
import com.us.openserver.protocols.winauth.*;

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
			ConsoleLogger logger = new ConsoleLogger();
			logger.setLogPackets(true);
	                    
	        HashMap<Integer, ProtocolConfiguration> protocolConfigurations =
	            new HashMap<Integer, ProtocolConfiguration>();
	        
	        protocolConfigurations.put(KeepAliveProtocol.PROTOCOL_IDENTIFIER,
	            new ProtocolConfiguration(KeepAliveProtocol.PROTOCOL_IDENTIFIER, "com.us.openserver.protocols.keepalive.KeepAliveProtocol"));
	
	        protocolConfigurations.put(WinAuthProtocol.PROTOCOL_IDENTIFIER,
	            new ProtocolConfiguration(WinAuthProtocol.PROTOCOL_IDENTIFIER, "com.us.openserver.protocols.winauth.WinAuthProtocolClient"));
	
	        protocolConfigurations.put(HelloProtocolClient.PROTOCOL_IDENTIFIER,
	            new ProtocolConfiguration(HelloProtocolClient.PROTOCOL_IDENTIFIER, "com.us.openserver.protocols.hello.HelloProtocolClient"));
	
	        client = new Client(this, cfg, protocolConfigurations, logger);
	        client.connect();
	        
			WinAuthProtocolClient wap = (WinAuthProtocolClient)client.initialize(WinAuthProtocol.PROTOCOL_IDENTIFIER);
	        if (!wap.authenticate("Michael", "Ramp5050", null))
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
	        
	        System.in.read();
	        client.close();	        
		}
		catch (Exception ex)
		{
			System.out.println(ex.getMessage());
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
