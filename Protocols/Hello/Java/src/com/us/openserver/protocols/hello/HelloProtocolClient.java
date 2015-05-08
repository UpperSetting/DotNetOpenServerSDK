package com.us.openserver.protocols.hello;

import com.us.openserver.util.*;

import java.io.IOException;

public class HelloProtocolClient extends HelloProtocol
{
	private IHelloProtocolObserver callbackInterface;
	public void setHelloObserver(IHelloProtocolObserver callbackInterface) 
	{
		this.callbackInterface = callbackInterface;
	}
	
	private String serverResponse;
	
	public void onPacketReceived(BinaryReader br) throws IOException
    {
		synchronized (this)
        {
	        serverResponse = br.readString();
			log(Level.Info, String.format("Server responded: %1$s", serverResponse));
			
			notifyAll();
        }
		
		if (callbackInterface != null)
			callbackInterface.onHelloComplete(serverResponse);
    }
	
	public String hello(String message) throws Exception
    {
		synchronized (this)
        {
			helloAsync(message);
	        
	        wait(10000);            
        }
		return serverResponse;
    }
	
	public void helloAsync(String message) throws Exception
    {
		BinaryWriter bw = CreateCommandPacket(message);
        try
        {
	        session.send(bw.toByteArray());
        }
        finally { try { bw.close(); } catch (IOException ex) { } }
    }
	
	public String helloBackgroundThread(String message) throws Exception
    {
		synchronized (this)
        {
			helloBackgroundThreadAsync(message);	        
	        wait(10000);            
        }
		return serverResponse;
    }
	
	public void helloBackgroundThreadAsync(String message) throws Exception
    {
		BinaryWriter bw = CreateCommandPacket(message);
        try
        {
	        new PacketWriter(session, bw.toByteArray()).execute();
        }
        finally { try { bw.close(); } catch (IOException ex) { } }
    }
	
	private BinaryWriter CreateCommandPacket(String message) throws IOException
	{
		BinaryWriter bw = new BinaryWriter();
        bw.writeUInt16(PROTOCOL_IDENTIFIER);
        bw.writeString(message);	        	        
        log(Level.Info, String.format("Client says: %1$s", message));
        return bw;
	}
}
