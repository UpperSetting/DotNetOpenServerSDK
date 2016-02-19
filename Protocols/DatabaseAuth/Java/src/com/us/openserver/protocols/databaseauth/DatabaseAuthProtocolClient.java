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

package com.us.openserver.protocols.databaseauth;

import com.us.openserver.*;
import com.us.openserver.protocols.*;

import java.io.IOException;

public class DatabaseAuthProtocolClient extends DatabaseAuthProtocol
{
    public static final int TIMEOUT = 120000;
    
    public DatabaseAuthProtocolClient()
    {
    }

    public boolean authenticate(String userName, String password) throws IOException
    {
        synchronized (this)
        {
            if (session == null)
                return false;

            super.userName = userName;
            session.UserName = userName;

            BinaryWriter bw = createAuthenticatePacket(password);
            try
            {
                session.send(bw.toByteArray());                
            }
            catch (IOException ex) { return false; }
            finally { try { bw.close(); } catch (IOException ex) { } }
            
            try { wait(DatabaseAuthProtocolClient.TIMEOUT); }
            catch (InterruptedException ex) {  }

            return isAuthenticated;
        }
    }
    
    public boolean authenticateBG(String userName, String password)
    {
        synchronized (this)
        {
            if (session == null)
                return false;

            super.userName = userName;
            session.UserName = userName;

            BinaryWriter bw = createAuthenticatePacket(password);
            try
            {
            	PacketWriter pw = new PacketWriter(session, bw.toByteArray());
                pw.execute();
            }
            finally { try { bw.close(); } catch (IOException ex) { } }

            try { wait(DatabaseAuthProtocolClient.TIMEOUT); }
            catch (InterruptedException ex) {  }

            return isAuthenticated;
        }
    }
    
    private BinaryWriter createAuthenticatePacket(String password)
    {
    	BinaryWriter bw = new BinaryWriter();
        bw.writeUInt16(DatabaseAuthProtocolClient.PROTOCOL_IDENTIFIER);
        bw.write((byte) DatabaseAuthProtocolCommands.AUTHENTICATE);
        bw.writeString(userName);
        bw.writeString(password);
        return bw;
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
                case DatabaseAuthProtocolCommands.AUTHENTICATED:
                	userId = br.readInt32();
                    isAuthenticated = true;
                    session.IsAuthenticated = true;
                    log(Level.Info, "Authenticated.");
                    notifyAll();
                    break;

                case DatabaseAuthProtocolCommands.ACCESS_DENIED:
                    log(Level.Notice, "Access denied.");
                    notifyAll();
                    break;

                case DatabaseAuthProtocolCommands.ERROR:
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
                    log(Level.Error, String.format("Invalid or unsupported command.  Command: %d", command));
                    break;
            }
        }
    }
}


