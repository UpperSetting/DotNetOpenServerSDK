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

package com.us.openserver.protocols;

import com.us.openserver.*;
import java.io.IOException;

public class CapabilitiesProtocol extends ProtocolBase
{

    public static final int PROTOCOL_IDENTIFIER = 0x0000;
    public static final int TIMEOUT = 120000;

    private int[] supportedRemoteProtocolIds;

    public CapabilitiesProtocol(Session session)
    {
        this.session = session;
    }

    public int[] getRemoteSupportedProtocolIds()
    {
        synchronized (this)
        {
            BinaryWriter bw = new BinaryWriter();
            try
            {
                bw.writeUInt16(PROTOCOL_IDENTIFIER);
                bw.write((byte)CapabilitiesProtocolCommands.GET_PROTOCOL_IDS);
                
                PacketWriter pw = new PacketWriter(session, bw.toByteArray());
                pw.execute();
            }
            finally { try { bw.close(); } catch (IOException ex) { } }

            try { wait(TIMEOUT); }
            catch (InterruptedException ex) {  }

            return supportedRemoteProtocolIds;
        }
    }


    public void sendError(int protocolId, String message)
    {
        synchronized (this)
        {
            BinaryWriter bw = new BinaryWriter();
            try
            {
                bw.writeUInt16(PROTOCOL_IDENTIFIER);
                bw.write((byte)CapabilitiesProtocolCommands.ERROR);
                bw.write(protocolId);
                bw.writeString(message);
                log(Level.Notice, message);
                
                PacketWriter pw = new PacketWriter(session, bw.toByteArray());
                pw.execute();
            }
            finally { try { bw.close(); } catch (IOException ex) { } }
        }
    }


    public void onPacketReceived(BinaryReader br)
    {
        int protocolId = 0;
        String errorMessage = null;
    
        int command = br.read();
        switch (command)
        {
            case CapabilitiesProtocolCommands.GET_PROTOCOL_IDS:
                {
                    int[] protocolIds = session.getLocalSupportedProtocolIds();
                    BinaryWriter bw = new BinaryWriter();
                    try
                    {
                        bw.writeUInt16(PROTOCOL_IDENTIFIER);
                        bw.write((byte)CapabilitiesProtocolCommands.PROTOCOL_IDS);
                        bw.writeUInt16s(protocolIds);
                        
                        String str = "";
                        for (int p : protocolIds)
                            str += p + ", ";
                        log(Level.Debug, String.format("Sent Protocol IDs: %1$s", str));

                        try
                        {
                            session.send(bw.toByteArray());    
                        }
                        catch (IOException ex)
                        {
                        }
                    }
                    finally { try { bw.close(); } catch (IOException ex) { } }
                    
                    break;
                }
            case CapabilitiesProtocolCommands.PROTOCOL_IDS:
                synchronized (this)
                {
                    supportedRemoteProtocolIds = br.readUInt16s();
                    String str = "";
                    for (int p : supportedRemoteProtocolIds)
                        str += p + ", ";
                    log(Level.Debug, String.format("Received Protocol IDs: %1$s", str));
                    notifyAll();
                }
                break;

            case CapabilitiesProtocolCommands.ERROR:
                synchronized (this)
                {
                    protocolId = br.readUInt16();
                    try
                    {
                        errorMessage = br.readString();
                    }
                    catch (IOException ex)
                    {
                    }
                    log(Level.Error, errorMessage);
                    notifyAll();
                }
                break;

            default:
                log(Level.Error, String.format("Invalid or unsupported command.  Command: %1$s", command));
                break;
        }

        if (errorMessage != null && errorMessage.length() > 0)
            session.onCapabilitiesError(protocolId, errorMessage);
    }

    protected void log(Level level, String message)
    {
        session.log(level, String.format("[Capabilities] %1$s", message));
    }    
}
