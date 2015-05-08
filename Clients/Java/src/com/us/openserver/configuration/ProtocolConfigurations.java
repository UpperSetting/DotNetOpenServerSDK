package com.us.openserver.configuration;

import java.util.HashMap;

public class ProtocolConfigurations
{
    private static ProtocolConfigurations ourInstance = new ProtocolConfigurations();

    public static ProtocolConfigurations getInstance()
    {
        return ourInstance;
    }

    private ProtocolConfigurations()
    {
    }

    public static HashMap<Integer, ProtocolConfiguration> Items = new HashMap<Integer, ProtocolConfiguration>();

    public static boolean ContainsKey(int id)
    {
        return Items.containsKey(id);
    }

    public static ProtocolConfiguration Get(int index)
    {
        return Items.get(index);
    }
}
