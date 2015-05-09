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

package com.us.openserver.util;

import com.us.openserver.Session;

public class PacketWriter implements Runnable
{
    private Session session;
    private byte[] packet;
    private Exception exception;
    private Thread t;
    private static int id;

    public PacketWriter(Session session, byte[] packet)
    {
        this.session  = session;
        this.packet = packet;
    }

    public void execute()
    {
        t = new Thread(this, "PacketWriter" + id++);
        t.start();
    }

    public void run()
    {
        try
        {
            session.send(packet);
        }
        catch (Exception ex)
        {
            exception = ex;
        }
    }

    public Exception getException()
    {
        return exception;
    }
}
