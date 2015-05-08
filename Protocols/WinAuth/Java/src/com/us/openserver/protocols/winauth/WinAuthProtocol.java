package com.us.openserver.protocols.winauth;

import com.us.openserver.protocols.AuthenticationProtocolBase;
import com.us.openserver.util.Level;

public class WinAuthProtocol extends AuthenticationProtocolBase
{
	public static final int PROTOCOL_IDENTIFIER = 0x0002;
	
	protected WinAuthProtocol()
    {
    }
	
	protected void log(Level level, String message)
    {
        session.log(level, String.format("[WinAuth] %1$s", message));
    }
}
