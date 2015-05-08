package com.us.openserver.protocols.hello;

import com.us.openserver.protocols.IProtocol;
import com.us.openserver.util.Level;

public class HelloProtocol extends IProtocol
{
	public static final int PROTOCOL_IDENTIFIER = 0x000A;	
	
	protected HelloProtocol()
    {
    }
	
	protected void log(Level level, String message)
	{
	    session.log(level, String.format("[Hello] %1$s", message));
	}
}
