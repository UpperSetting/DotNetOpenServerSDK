package com.us.openserver.util;

public class ConsoleLogger implements ILogger
{
	private boolean logPackets;
	
	public boolean getLogPackets() 
	{
		return logPackets;
	}

	public void setLogPackets(boolean value) 
	{
		logPackets = value;
	}

	public void log(Level level, String message)
    {
    	System.out.println(String.format("%1$s %2$s", level, message));
    }
    
    public void log(Exception ex)
    {
    	System.out.println(String.format("%1$s %2$s", Level.Error, ex.getMessage()));
    }
}
