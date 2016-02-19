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

package com.us.openserver.protocols.winauth;

import com.us.openserver.*;
import com.us.openserver.protocols.*;

import java.io.IOException;

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
                    log(Level.Error, String.format("Invalid or unsupported command.  Command: %d", command));
                    break;
            }
        }
    }
}


