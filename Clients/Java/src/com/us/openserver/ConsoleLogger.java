/*
Copyright 2015 Upper Setting Corporation

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

package com.us.openserver;

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
