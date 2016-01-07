/*
Copyright 2015-2016 Upper Setting Corporation

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

public class Logger
{
    private ILoggerObserver callback;
    public void setILoggerObserver(ILoggerObserver callback) {
        this.callback = callback;
    }
    
    protected boolean logDebug;
    public boolean getLogDebug() {
        return logDebug;
    }
    public void setLogDebug(boolean logDebug) {
        this.logDebug = logDebug;
    }
    
    protected boolean logPackets;
    public boolean getLogPackets() {
        return logPackets;
    }
    public void setLogPackets(boolean logPackets) {
        this.logPackets = logPackets;
    }
    
    public Logger()
    {
    }
    
    public Logger(ILoggerObserver callback)
    {
        this.callback = callback;
    }
    
    public void log(Level level, String message)
    {
        if (level == Level.Debug && !logDebug)
            return;

        if (callback != null)
            callback.onLogMessage(level, message);
    }
   
    public void log(Exception ex)
    {
        log(Level.Error, String.format("%1$s\r\n%2$s", ex.getMessage(), ex.getStackTrace()));
    }
}


