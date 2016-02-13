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

package com.us.openserver.protocols;

import java.io.*;
import java.math.BigDecimal;
import java.math.BigInteger;
import java.util.Date;

public class BinaryWriter extends ByteArrayOutputStream
{
    public BinaryWriter()
    {
        super();
    }
    
    public void write(boolean value)
    {
        write((byte)(value ? 0x01 : 0x00));        
    }
    
    public void writeBytes(byte[] value) throws IOException
    {
        if (value == null)
        {
            writeInt(0);
        }
        else
        {
        	writeInt(value.length);
        	write(value);
        }
    }

    public void writeString(String value)
    {
        if (value == null)
            value = "";

        char[] chars = value.toCharArray();
        int num = chars.length;
        while (num >= 128) {
            write((char) (num | 128));
            num >>= 7;
        }
        write((char) (num));

        for (int i = 0; i < chars.length; i++)
            write(chars[i]);
    }
    
    public void writeInt16(short value)
    {
        write((byte)value);
        write((byte)(value >> 8));        
    }

    public void writeInt(int value)
    {
        write((byte)value);
        write((byte)(value >> 8));
        write((byte)(value >> 16));
        write((byte)(value >> 24));
    }
    
    public void write(int[] value)
    {
        if (value == null)
        {
            write((int)0);
        }
        else
        {
            write(value.length);
            for (int i : value)
            	writeInt(i);
        }
    }
    
    public void writeUInt(int value)
    {
        write((byte)value);
        write((byte)(value >> 8));
        write((byte)(value >> 16));
        write((byte)(value >> 24));
    }

    public void write(Date date)
    {  
    	final long TICKS_AT_EPOCH = 621355968000000000L;
        final long TICKS_PER_MILLISECOND = 10000;
        
        long ticks = (date.getTime() *  TICKS_PER_MILLISECOND) + TICKS_AT_EPOCH;
        write(ticks);
    }

    public void write(long value)
    {
        write((byte)value);
        write((byte)(value >> 8));
        write((byte)(value >> 16));
        write((byte)(value >> 24));
        write((byte)(value >> 32));
        write((byte)(value >> 40));
        write((byte)(value >> 48));
        write((byte)(value >> 56));        
    }
    
//    public void write(BigInteger value)
//    {
//    	byte[] buf = value.toByteArray();
//    	
//    	int n = 7;
//    	for (int i = 0; i < 8; i++, n--)
//    		write((byte)buf[n]);
//    }

    public void write(BigDecimal value)
    {
    	writeString(value.toString());
    }
    
    public void writeUInt16(int value)
    {
        write((byte)value);
        write((byte)(value >> 8));
    }
    
    public void writeUInt16s(int[] value)
    {
        if (value == null)
        {
            write((int)0);
        }
        else
        {
            write(value.length);
            for (int i : value)
                writeUInt16(i);
        }
    }
}

