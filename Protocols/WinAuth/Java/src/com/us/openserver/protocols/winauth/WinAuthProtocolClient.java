package com.us.openserver.protocols.winauth;

import java.io.IOException;

import com.us.openserver.util.*;

public class WinAuthProtocolClient extends WinAuthProtocol
{
    public static final int TIMEOUT = 120000;
    
    public WinAuthProtocolClient()
    {
    }

    public boolean authenticate(String userName, String password, String domain)
    {
        synchronized (this)
        {
            if (session == null)
                return false;

            super.userName = userName;
            session.UserName = userName;

            BinaryWriter bw = new BinaryWriter();
            try
            {
	            bw.writeUInt16(WinAuthProtocolClient.PROTOCOL_IDENTIFIER);
	            bw.write((byte) WinAuthProtocolCommands.AUTHENTICATE);
	            bw.writeString(userName);
	            bw.writeString(password);
	            bw.writeString(domain);
	
	            PacketWriter pw = new PacketWriter(session, bw.toByteArray());
	            pw.execute();
            }
            finally { try { bw.close(); } catch (IOException ex) { } }

            try { wait(WinAuthProtocolClient.TIMEOUT); }
            catch (InterruptedException ex) {  }

            return isAuthenticated;
        }
    }

    public void onPacketReceived(BinaryReader br)
    {
        synchronized (this)
        {
            if (session == null)
                return;

            int command = br.readByte();
            switch (command)
            {
                case WinAuthProtocolCommands.AUTHENTICATED:
                    isAuthenticated = true;
                    session.IsAuthenticated = true;
                    log(Level.Info, "Authenticated.");
                    notifyAll();
                    break;

                case WinAuthProtocolCommands.ACCESS_DENIED:
                    log(Level.Notice, "Access denied.");
                    notifyAll();
                    break;

                case WinAuthProtocolCommands.ERROR:
                {
                	try
                	{
	                    String errorMessage = br.readString();
	                    log(Level.Notice, errorMessage);
                	}
                	catch (IOException ex)
                	{
                	}
                    notifyAll();
                    break;
                }
                
                default:
                    log(Level.Error, String.format("Invalid or unsupported command.  Command: %1$s", command));
                    break;
            }
        }
    }
}


