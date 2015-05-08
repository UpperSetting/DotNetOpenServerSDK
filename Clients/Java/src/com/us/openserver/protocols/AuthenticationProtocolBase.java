package com.us.openserver.protocols;

public abstract class AuthenticationProtocolBase extends IProtocol
{
    protected boolean isAuthenticated;
    public boolean getIsAuthenticated()
    {
        return isAuthenticated;
    }

    protected String userName;
    public String getUserName()
    {
        return userName;
    }
}
