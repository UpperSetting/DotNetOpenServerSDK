package com.us.openserver.util;

public interface ILogger
{
	public boolean getLogPackets();
	public void setLogPackets(boolean value);
    public void log(Level level, String message);
    public void log(Exception ex);
}
