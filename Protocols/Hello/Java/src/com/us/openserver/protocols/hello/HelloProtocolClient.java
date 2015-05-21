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

package com.us.openserver.protocols.hello;

import com.us.openserver.*;
import com.us.openserver.protocols.*;

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
