package com.us.openserver.protocols.keepalive;

import com.us.openserver.*;
import com.us.openserver.configuration.*;
import com.us.openserver.protocols.*;
import com.us.openserver.util.*;

import java.io.IOException;
import java.util.concurrent.*;

public class KeepAliveProtocol extends IProtocol implements Runnable
{
	public static final int PROTOCOL_IDENTIFIER = 0x0001;
    public static final int INTERVAL = 10000;
    
    private ScheduledExecutorService timer;
    
    public KeepAliveProtocol()
    {
    }

    public void initialize(Session session, ProtocolConfiguration pc)
    {
    	super.initialize(session, pc);
    	
        synchronized (this)
        {
            this.session = session;

            if (timer != null)
                timer.shutdown();

            timer = Executors.newScheduledThreadPool(1);
            timer.scheduleAtFixedRate(this, KeepAliveProtocol.INTERVAL, KeepAliveProtocol.INTERVAL, TimeUnit.MILLISECONDS);
        }
    }

    public void close()
    {
        synchronized (this)
        {
            if (timer != null)
                timer.shutdown();
            
            BinaryWriter bw = new BinaryWriter();
            try 
            {
	            bw.writeUInt16(KeepAliveProtocol.PROTOCOL_IDENTIFIER);
	            bw.write((byte)KeepAliveProtocolCommands.QUIT);
	            try { session.send(bw.toByteArray()); } catch (IOException ex) { }
	            log(Level.Debug, "Quit sent.");
            }
            finally 
            {
            	try { bw.close(); } catch (IOException ex) { }
            }
        }
    }
    
    public void dispose()
    {
    	synchronized (this) 
    	{
            if (timer != null)
            	timer.shutdown();
        }
    }

    public void onPacketReceived(BinaryReader br)
    {
    	boolean dispose = false;
        synchronized (this) 
        {
            if (session == null)
                return;

            int command = br.read();
            switch (command)
            {
                case KeepAliveProtocolCommands.KEEP_ALIVE:
                    log(Level.Debug, "Received.");
                    break;
                case KeepAliveProtocolCommands.QUIT:
                	log(Level.Info, "Quit received.");
                    dispose = true;
                    break;
                default:
                	log(Level.Error, String.format("Invalid or unsupported command.  Command: %1$s", command));
                    break;
            }
        }
        
        if (dispose)
            session.dispose();
    }

	private void log(Level level, String message)
	{
	    session.log(level, String.format("[Keep-Alive] %1$s", message));
	}

    public void run()
    {
    	Exception connectionLostException = null;
        synchronized (this)
        {
            try
            {
                BinaryWriter bw = new BinaryWriter();
                try
                {
	                bw.writeUInt16(KeepAliveProtocol.PROTOCOL_IDENTIFIER);
	                bw.write((byte) KeepAliveProtocolCommands.KEEP_ALIVE);
	                session.send(bw.toByteArray());
	                log(Level.Debug, "Sent.");
                }
                finally {
                	try { bw.close(); } catch (IOException ex) { }
                }                
            }
            catch (Exception ex)
            {
            	timer.shutdown();
            	connectionLostException = ex;
                
            }
        }
        if (connectionLostException != null)
        	session.connectionLost(connectionLostException);
    }
}
