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
import com.us.openserver.session.Session;

import java.io.IOException;

public abstract class ProtocolBase
{
    protected Session session;
    
    public void initialize(Session session, ProtocolConfiguration pc, Object userData)
    {
        this.session = session;
    }
    
    public void close()
    {
    }
    
    public void dispose()
    {        
    }
    
    public void onPacketReceived(BinaryReader br) throws IOException
    {
    }
    
    public void onErrorReceived(String message)
    {
        synchronized(this)
        {
            log(Level.Error, message);
            notifyAll();
        }
    }
    
    protected void log(Level level, String message)
    {
        session.log(level, String.format("[ProtocolBase] %1$s", message));
    }
}
