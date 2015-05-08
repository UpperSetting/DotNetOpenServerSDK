package com.us.openserver.util;

import java.io.*;
import java.math.BigDecimal;
import java.util.ArrayList;
import java.util.Date;

public class BinaryReader extends ByteArrayInputStream
{
    public BinaryReader(byte[] buf)
    {
        super(buf);
    }

    public byte readByte()
    {
        return (byte)read();
    }

    
    public Date readDateTime()
    {
        return new Date(readLong());
    }

    public BigDecimal readDecimal()
    {
        return new BigDecimal(readLong());
    }

    public int readInt32()
    {
        int retVal = read();
        retVal |= read() << 8;
        retVal |= read() << 16;
        retVal |= read() << 24;
        return retVal;
    }

    public long readLong()
    {
        long retVal = read();
        retVal |= read() << 8;
        retVal |= read() << 16;
        retVal |= read() << 24;
        retVal |= read() << 32;
        retVal |= read() << 48;
        retVal |= read() << 56;
        retVal |= read() << 64;
        return retVal;
    }

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
}