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

import com.us.openserver.protocols.BinaryReader;

public class PacketHandler implements Runnable
{
    private Session session;
    private BinaryReader br;
    private Exception exception;
    private Thread t;
    private static int id;

    public PacketHandler(Session session, BinaryReader br)
    {
        this.session = session;
        this.br = br;
    }

    public void execute()
    {
        t = new Thread(this, "PacketHandler" + id++);
        t.start();
    }

    public void run()
    {
        try
        {
            session.onPacketReceived(br);
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
