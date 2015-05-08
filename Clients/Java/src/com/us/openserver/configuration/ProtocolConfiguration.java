package com.us.openserver.configuration;

public class ProtocolConfiguration
{
    private int id;
    private String classPath;

    protected ProtocolConfiguration()
    {
    }

    public ProtocolConfiguration(int id, String classPath)
    {
        this();

        this.id = id;
        this.classPath = classPath;
    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }
    
    public String getClassPath() {
        return classPath;
    }

    public void setClassPath(String classPath) {
        this.classPath = classPath;
    }
}
