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

package com.us.openserver.protocols.keepalive;

import com.us.openserver.*;
import com.us.openserver.session.*;
import com.us.openserver.protocols.*;

import java.io.IOException;
import java.util.concurrent.*;

public class KeepAliveProtocol extends ProtocolBase implements Runnable
{
    public static final int PROTOCOL_IDENTIFIER = 0x0001;
    public static final int INTERVAL = 10000;
    private static final int IDLE_TIMEOUT = 30000;
    
    private ScheduledExecutorService timer;
    private long lastHeartBeatReceivedAt = System.currentTimeMillis();
    
    public KeepAliveProtocol()
    {
    }

    public void initialize(Session session, ProtocolConfiguration pc, Object userData)
    {
        super.initialize(session, pc, userData);
        
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
                	lastHeartBeatReceivedAt = System.currentTimeMillis();
                    log(Level.Debug, "Received.");
                    break;
                case KeepAliveProtocolCommands.QUIT:
                    log(Level.Info, "Quit received.");
                    dispose = true;
                    break;
                default:
                    log(Level.Error, String.format("Invalid or unsupported command.  Command: %d", command));
                    break;
            }
        }
        
        if (dispose)
            session.dispose();
    }

    protected void log(Level level, String message)
    {
        session.log(level, String.format("[Keep-Alive] %s", message));
    }

    public void run()
    {
        Exception connectionLostException = null;
        synchronized (this)
        {
            try
            {
            	if (System.currentTimeMillis() - lastHeartBeatReceivedAt > IDLE_TIMEOUT)
                    throw new Exception("Idle connection detected.");
            	
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
