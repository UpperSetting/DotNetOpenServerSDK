package com.us.openserver.protocols;

import java.io.IOException;

import com.us.openserver.*;
import com.us.openserver.configuration.*;
import com.us.openserver.util.*;

public abstract class IProtocol
{
	protected Session session;
	
    public void initialize(Session session, ProtocolConfiguration pc)
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
}
