package com.us.openserver.util;

import com.us.openserver.*;

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
