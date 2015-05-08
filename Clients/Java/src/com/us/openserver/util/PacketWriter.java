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
