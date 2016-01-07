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

package com.us.openserver.session;

import com.us.openserver.session.Session;

public class SessionCloser implements Runnable
{
    private Session session; 
    private int protocolId;
    private static int id;

    public SessionCloser(Session session)
    {
        this.session  = session;
    }
    
    public void close()
    {
        close(0);        
    }
    
    public void close(int protocolId)
    {
        try 
        { 
            if (protocolId > 0)
                session.close(protocolId);
            else
                session.close();
        } 
        catch (Exception ex) 
        {
        }
    }
    
    public void closeBackgroundThread()
    {
        closeBackgroundThread(0);
    }
    
    public void closeBackgroundThread(int protocolId)
    {
        this.protocolId  = protocolId;
        Thread t = new Thread(this, "SessionCloser" + ++id);
        t.start();
    }

    public void run()
    {
        close(protocolId);        
    }
}
