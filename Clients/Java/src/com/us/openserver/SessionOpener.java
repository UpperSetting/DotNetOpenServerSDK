package com.us.openserver;

import java.net.*;
import javax.net.ssl.*;

import com.us.openserver.configuration.*;
import com.us.openserver.util.*;

public class SessionOpener implements Runnable
{
	private Client client;	
	private Session session;	
    private Exception exception;    
    private static int id;

    public SessionOpener(Client client)
    {
    	this.client  = client;
    }

    public Session connectAsync() throws Exception
    {
    	synchronized (this)
        {
	        Thread t = new Thread(this, "SessionOpenThread" + ++id);
	        t.start();
	        
	        wait(client.getServerConfiguration().getSocketTimeoutInTicks());
        }
    	if (exception != null)
    		throw exception;
    	
    	return session;
    }

    public void run()
    {
    	synchronized (this)
        {
	        try
	        {
	        	connect();        	
	        }
	        catch (Exception ex)
	        {
	            exception = ex;
	        }
	        this.notifyAll();
        }
    }
    
    public Session connect() throws Exception
    {
    	client.close();
    	
    	Socket socket;
    	ServerConfiguration cfg = client.getServerConfiguration();
        TlsConfiguration tls = cfg.getTlsConfiguration();
        
        if (!tls.isEnabled())
        {
            socket = new Socket(cfg.getHost(), cfg.getPort());
            setSocketOptions(socket);
        }
        else
        {
            SSLContext sslContext = SSLContext.getInstance("TLS");
            sslContext.init(null, new TrustManager[]{new MyTrustManager(null)}, null);

            SSLSocketFactory socketFactory = sslContext.getSocketFactory();
            socket = socketFactory.createSocket(cfg.getHost(), cfg.getPort());
            setSocketOptions(socket);

            ((SSLSocket)socket).setUseClientMode(true);
        }
        
        session = new Session(client, socket, socket.getInetAddress().getHostAddress());            
        session.log(Level.Info, String.format("Connected to %1$s:%2$s...", cfg.getHost(), cfg.getPort()));
        session.beginRead();
        
        return session;
    }

    private void setSocketOptions(Socket socket) throws SocketException
    {
        socket.setSoTimeout(client.getServerConfiguration().getSocketTimeoutInTicks());
        socket.setSoLinger(true, 10);
        socket.setTcpNoDelay(true);
    }
}
