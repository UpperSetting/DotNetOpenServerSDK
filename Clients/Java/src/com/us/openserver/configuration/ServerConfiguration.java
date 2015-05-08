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
