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

import com.us.openserver.configuration.*;
import com.us.openserver.protocols.*;
import com.us.openserver.session.*;

import java.util.HashMap;

public class Client
{
    private IClientObserver clientObserver;
    
    private Logger logger;
    public Logger getLogger() { return logger; }
    
    private HashMap<Integer, ProtocolConfiguration> protocolConfigurations;
    public HashMap<Integer, ProtocolConfiguration> getProtocolConfigurations() { return protocolConfigurations; }
    
    private ServerConfiguration serverConfiguration;
    public ServerConfiguration getServerConfiguration() { return serverConfiguration; }
    
    private Object userData;
    public Object getUserData() { return userData; }    
    
    private Session session;
    public Session getSession() { return session; }
    
    public Client(
        IClientObserver clientObserver,
        ServerConfiguration serverConfiguration,
        HashMap<Integer, ProtocolConfiguration> protocolConfigurations,
        Logger logger,
        Object userData)
    {
        this.clientObserver = clientObserver;
        
        if (logger == null)
            logger = new Logger();
        this.logger = logger;

        if (serverConfiguration == null)
            serverConfiguration = new ServerConfiguration();
        this.serverConfiguration = serverConfiguration;

        if (protocolConfigurations == null)
            protocolConfigurations = new HashMap<Integer, ProtocolConfiguration>();
        this.protocolConfigurations = protocolConfigurations;

        this.userData = userData;
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
    
    public void close(int protocolId)
    {
        if (session != null)
        {
            SessionCloser sessionCloser = new SessionCloser(session);
            sessionCloser.close(protocolId);
            session = null;
        }
    }
    
    public void closeAsync(int protocolId)
    {
        if (session != null)
        {
            SessionCloser sessionCloser = new SessionCloser(session);
            sessionCloser.closeAsync(protocolId);
            session = null;
        }
    }
    
    public int[] getServerSupportedProtocolIds()
    {
        if (session == null)
            return new int[0];

        return session.getRemoteSupportedProtocolIds();
    }
    
    public ProtocolBase initialize(int protocolId) throws Exception
    {
         return session != null ? session.initialize(protocolId, userData) : null;
    }
    
    public ProtocolBase initializeAsync(int protocolId) throws Exception
    {
         return session != null ? session.initialize(protocolId, userData) : null;
    }
    
    public void onConnectionLost(Exception ex)
    {
        if (clientObserver != null)
            clientObserver.onConnectionLost(ex);
    }
}
