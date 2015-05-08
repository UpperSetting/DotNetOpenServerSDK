package com.us.openserver;

import com.us.openserver.Session;

public class SessionCloser implements Runnable
{
    private Session session;    
    private static int id;

    public SessionCloser(Session session)
    {
        this.session  = session;
    }

    public void closeAsync()
    {
        Thread t = new Thread(this, "SessionCloser" + ++id);
        t.start();
    }
    
    public void run()
    {
        close();
    }
    
    public void close()
    {
    	try { session.close(); } catch (Exception ex) { }
    }
}
