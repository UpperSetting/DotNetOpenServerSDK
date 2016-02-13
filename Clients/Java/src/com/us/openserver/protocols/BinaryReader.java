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
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.TimeZone;

public class BinaryReader extends ByteArrayInputStream
{
    public BinaryReader(byte[] buf)
    {
        super(buf);
    }
    
    public boolean readBoolean()
    {
    	return 0 == (byte)read() ? false : true; 
    }

    public byte readByte()
    {
        return (byte)read();
    }
    
    public byte[] readBytes()
    {
    	int length = readInt32();
    	byte[] retVal = new byte[length];
        read(retVal, 0, length);
        return retVal;
    }
    
    public Date readDateTime()
    {
    	final long TICKS_AT_EPOCH = 621355968000000000L;
        final long TICKS_PER_MILLISECOND = 10000;
        
        long ticks = readLong();
        Date date = new Date((ticks - TICKS_AT_EPOCH) / TICKS_PER_MILLISECOND);
                
        return date;
    }
    
    public Date readNullableDateTime()
    {
        if (readBoolean())
            return readDateTime();
        else
            return null;
    }
    
    public Long readNullableTimeSpan()
    {
        if (readBoolean())
            return readLong();
        else
            return null;
    }

    public BigDecimal readDecimal()
    {
        return new BigDecimal(readLong());
    }
    
    public short readInt16()
    {
    	short retVal = (short)read();
        retVal |= read() << 8;
        return retVal;
    }

    public int readInt32()
    {
        int retVal = read();
        retVal |= read() << 8;
        retVal |= read() << 16;
        retVal |= read() << 24;
        return retVal;
    }
    
    public int[] readInt32s()
    {
    	int count = readInt32();
        int[] retVal = new int[count];
        for (int i = 0; i < count; i++)
            retVal[i] = readInt32();

        return retVal;
    }

    public long readLong()
    {
		long retVal = read();
        retVal |= read() << 8;
        retVal |= read() << 16;
        retVal |= 0x00000000FF000000L & read() << 24;
        retVal |= 0x000000FF00000000L & ((long)read() << 32);
        retVal |= 0x0000FF0000000000L & ((long)read() << 40);
        retVal |= 0x00FF000000000000L & ((long)read() << 48);
        retVal |= 0xFF00000000000000L & ((long)read() << 56);		
        return retVal;
    }
    
//    public BigInteger readUInt64()
//    {
//    	byte[] buf = new byte[8];
//    	int n = 7;
//    	for (int i = 0; i < 8; i++, n--)
//    		buf[n] = (byte)read();
//    	
//    	BigInteger retVal = new BigInteger(buf);
//        return retVal;
//    }

    public String readString() throws IOException
    {
        int nextCharValue = this.read();
        if(nextCharValue != -1)
        {
            StringBuilder s = new StringBuilder();

            boolean lengthKeepGoing = (nextCharValue & (1 << 7)) == (1 << 7); //look if first bit = 1

            ArrayList<Integer> lengths = new ArrayList<Integer>();
            lengths.add(nextCharValue & 0x7F);

            while(lengthKeepGoing)
            {
                nextCharValue = this.read();
                lengthKeepGoing = (nextCharValue & (1 << 7)) == (1 << 7);
                lengths.add(nextCharValue & (0x7F)); // mask: 0x7F = 0111 1111
            }

            int totalLength = 0;

            for(int i = 0; i < lengths.size(); i++)
                totalLength += lengths.get(i) << (7 * i);

            for (int i = 0; i < totalLength; i++)
                s.append((char)this.read());

            return s.toString();
        }
        return null;
    }

    public int readUInt16()
    {
        int retVal = read();
        retVal |= read() << 8;
        return retVal;
    }
    
    public int[] readUInt16s()
    {
        int count = readInt32();
        int[] retVal = new int[count];
        for (int i = 0; i < count; i++)
            retVal[i] = readUInt16();

        return retVal;
    }
}