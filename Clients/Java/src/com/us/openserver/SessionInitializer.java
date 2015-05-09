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

package com.us.openserver;

import com.us.openserver.protocols.IProtocol;

public class SessionInitializer implements Runnable
{
    private Client client;    
    private int protocolId;
    private IProtocol p;
    private Exception exception;
    private static int id;

    public SessionInitializer(Client client, int protocolId)
    {
        this.client  = client;
        this.protocolId = protocolId;
    }

    public IProtocol initializeAsync() throws Exception
    {
    	synchronized (this)
        {
    		Thread t = new Thread(this, "SessionInitializer" + ++id);
	        t.start();
	        
	        wait(client.getServerConfiguration().getSocketTimeoutInTicks());
        }
    	if (exception != null)
    		throw exception;
    	
    	return p;    	
    }
    
    public void run()
    {
    	synchronized (this)
        {
	    	try
	    	{
	    		initialize(protocolId);
	    	}
	    	catch (Exception ex)
	    	{
	    		exception = ex;
	    	}
	    	this.notifyAll();
        }
    }
    
    public IProtocol initialize(int protocolId) throws Exception
    {
    	p = client.getSession().initialize(protocolId);
    	return p;
    }
}