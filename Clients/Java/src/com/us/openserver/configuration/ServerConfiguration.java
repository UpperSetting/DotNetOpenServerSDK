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

package com.us.openserver.configuration;

import com.us.openserver.protocols.SessionLayerProtocol;

public class ServerConfiguration
{
    private static final int IDLE_TIMEOUT = 300;//seconds
    private static final int SOCKET_TIMEOUT = 120;//seconds

    private String host;
    private int port;
    private int idleTimeout;
    private int idleTimeoutInTicks;
    private int socketTimeout;
    private int socketTimeoutInTicks;
    private TlsConfiguration tlsConfiguration;
    
    public ServerConfiguration()
    {
        host = "localhost";
        port = SessionLayerProtocol.PORT;
        idleTimeout = IDLE_TIMEOUT;
        socketTimeout = SOCKET_TIMEOUT;
        tlsConfiguration = new TlsConfiguration();
    }

    public String getHost() 
    {
        return host;
    }

    public void setHost(String value) 
    {
        host = value;
    }

    public int getPort() 
    {
        return port;
    }

    public void setPort(int value) 
    {
        port = value;
    }

    public int getIdleTimeout() 
    {
        return idleTimeout;
    }

    public void setIdleTimeout(int value) 
    {
        idleTimeout = value;
        idleTimeoutInTicks = value * 1000;
    }

    public int getSocketTimeout() 
    {
        return socketTimeout;
    }

    public void setSocketTimeout(int value) 
    {
        socketTimeout = value;
        socketTimeoutInTicks = value * 1000;
    }

    public int getIdleTimeoutInTicks() 
    {
        return idleTimeoutInTicks;
    }

    public int getSocketTimeoutInTicks() 
    {
        return socketTimeoutInTicks;
    }
    
    public TlsConfiguration getTlsConfiguration() 
    {
        return tlsConfiguration;
    }

    public void setTlsConfiguration(TlsConfiguration value) 
    {
        tlsConfiguration = value;        
    }
}
