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