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

import java.util.HashMap;

import com.us.openserver.configuration.*;
import com.us.openserver.protocols.IProtocol;
import com.us.openserver.util.*;

public class Client
{
	private ILogger logger;
	public ILogger getLogger() { return logger; }
	
	private IClientObserver clientObserver;
	
    private ServerConfiguration cfg;
    public ServerConfiguration getServerConfiguration() { return cfg; }
    
    private Session session;
    public Session getSession() { return session; }
    
    public Client(
		IClientObserver clientObserver,
        ServerConfiguration cfg,
        HashMap<Integer, ProtocolConfiguration> protocolConfigurations)
    {
    	this.clientObserver = clientObserver;
        this.cfg = cfg;
        this.logger = new ConsoleLogger();
        ProtocolConfigurations.Items = protocolConfigurations;
    }

    public Client(
		IClientObserver clientObserver,
        ServerConfiguration cfg, 
        HashMap<Integer, ProtocolConfiguration> protocolConfigurations,
        ILogger logger)
    {
    	this.clientObserver = clientObserver;
        this.cfg = cfg;
        this.logger = logger;
        ProtocolConfigurations.Items = protocolConfigurations;
    }
        
	public void connect() throws Exception
	{
		SessionOpener sessionOpener = new SessionOpener(this);
		session = sessionOpener.connect();	
	}
	
	public void connectAsync() throws Exception
	{
		SessionOpener sessionOpener = new SessionOpener(this);
		session = sessionOpener.connectAsync();	
	}
	
	public void close()
	{
		if (session != null)
    	{
			SessionCloser sessionCloser = new SessionCloser(session);
			sessionCloser.close();
			session = null;
    	}
	}
	
	public void closeAsync()
	{
		if (session != null)
    	{
			SessionCloser sessionCloser = new SessionCloser(session);
			sessionCloser.closeAsync();
			session = null;
    	}
	}
	
	public IProtocol initialize(int protocolId) throws Exception
    {
         return session != null ? session.initialize(protocolId) : null;
    }
	
	public IProtocol initializeAsync(int protocolId) throws Exception
    {
         return session != null ? session.initialize(protocolId) : null;
    }
	
    public void onConnectionLost(Exception ex)
    {
    	if (clientObserver != null)
    		clientObserver.onConnectionLost(ex);
    }
}
