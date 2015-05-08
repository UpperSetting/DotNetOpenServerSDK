package com.us.openserver.util;

import java.io.*;
import java.math.BigDecimal;
import java.util.GregorianCalendar;

public class BinaryWriter extends ByteArrayOutputStream
{
    public BinaryWriter()
    {
        super();
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

    public void writeInt(int value)
    {
        write((byte)value);
        write((byte)(value >> 8));
        write((byte)(value >> 16));
        write((byte)(value >> 24));
    }

    public void writeUInt(int value)
    {
        write((byte)value);
        write((byte)(value >> 8));
        write((byte)(value >> 16));
        write((byte)(value >> 24));
    }

    public void writeUInt16(int value)
    {
        write((byte)value);
        write((byte)(value >> 8));
    }

    public void write(GregorianCalendar date)
    {
        write(date.getTimeInMillis());
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
        write((byte) (value >> 56));
        write((byte)(value >> 64));
    }

    public void write(BigDecimal value)
    {
        writeString(value.toString());
    }
}

