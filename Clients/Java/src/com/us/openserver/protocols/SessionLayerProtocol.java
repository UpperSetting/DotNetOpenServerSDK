package com.us.openserver.protocols;

public class SessionLayerProtocol
{
    public static final int PROTOCAL_IDENTIFIER = 21843;//U 0x55, S 0x53

    public static final int PROTOCOL_IDENTIFIER_LENGTH = 2;
    public static final int LENGTH_LENGTH = 4;
    public static final int HEADER_LENGTH = PROTOCOL_IDENTIFIER_LENGTH + LENGTH_LENGTH;

    public static final int PORT = 21843;
}
