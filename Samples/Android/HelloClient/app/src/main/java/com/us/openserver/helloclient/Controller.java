package com.us.openserver.helloclient;


import android.databinding.BaseObservable;
import android.databinding.Bindable;
import android.databinding.ObservableArrayList;
import android.os.Handler;
import android.os.Looper;
import android.os.Message;

import com.us.openserver.Client;
import com.us.openserver.IClientObserver;
import com.us.openserver.ILoggerObserver;
import com.us.openserver.Level;
import com.us.openserver.configuration.ServerConfiguration;
import com.us.openserver.configuration.TlsConfiguration;
import com.us.openserver.protocols.ProtocolConfiguration;
import com.us.openserver.protocols.hello.*;
import com.us.openserver.protocols.keepalive.KeepAliveProtocol;
import com.us.openserver.protocols.winauth.WinAuthProtocol;
import com.us.openserver.protocols.winauth.WinAuthProtocolClient;

import java.util.HashMap;

public class Controller extends BaseObservable implements IClientObserver, ILoggerObserver
{
    private Client client;
    private HelloProtocolClient hpc;
    private ServerConfiguration cfg;
    private HashMap<Integer, ProtocolConfiguration> protocolConfigurations = new HashMap<>();

    private String host;
    private String userName;
    private String password;

    private Handler handler;

    @Bindable
    public ObservableArrayList<MessageViewModel> messages = new ObservableArrayList<>();

    public Controller()
    {
        cfg = new ServerConfiguration();

        TlsConfiguration tls = cfg.getTlsConfiguration();
        tls.setEnabled(false);
        tls.setAllowCertificateChainErrors(true);
        tls.setAllowSelfSignedCertificate(true);
        tls.setCheckCertificateRevocation(false);
        tls.setRequireRemoteCertificate(true);

        protocolConfigurations.put(KeepAliveProtocol.PROTOCOL_IDENTIFIER,
                new ProtocolConfiguration(KeepAliveProtocol.PROTOCOL_IDENTIFIER, "com.us.openserver.protocols.keepalive.KeepAliveProtocol"));

        protocolConfigurations.put(WinAuthProtocol.PROTOCOL_IDENTIFIER,
                new ProtocolConfiguration(WinAuthProtocol.PROTOCOL_IDENTIFIER, "com.us.openserver.protocols.winauth.WinAuthProtocolClient"));

        protocolConfigurations.put(HelloProtocol.PROTOCOL_IDENTIFIER,
                new ProtocolConfiguration(HelloProtocol.PROTOCOL_IDENTIFIER, "com.us.openserver.protocols.hello.HelloProtocolClient"));

        handler = new Handler(Looper.getMainLooper())
        {
            @Override
            public void handleMessage(Message inputMessage)
            {
                switch (inputMessage.what)
                {
                    case 0:
                        onLogMessage((Object[])inputMessage.obj);
                        break;
                }
            }
        };
    }

    public void login(String host, String userName, String password) throws Exception
    {
        cfg.setHost(host);
        client = new Client(this, cfg, protocolConfigurations);
        client.getLogger().setILoggerObserver(this);
        client.getLogger().setLogDebug(true);
        client.getLogger().setLogPackets(true);
        client.connect();

        try
        {
            WinAuthProtocolClient wap = (WinAuthProtocolClient) client.initialize(WinAuthProtocol.PROTOCOL_IDENTIFIER);
            if (!wap.authenticate(userName, password, null))
                throw new Exception("Access denied.");

            client.initialize(KeepAliveProtocol.PROTOCOL_IDENTIFIER);

            hpc = (HelloProtocolClient) client.initialize(HelloProtocol.PROTOCOL_IDENTIFIER);

            this.host = host;
            this.userName = userName;
            this.password = password;
        } catch (Exception ex)
        {
            client.close();
            throw ex;
        }
    }

    @Override
    public void onConnectionLost(Exception e)
    {
        try
        {
            while (!client.getIsConnected())
            {
                try
                {
                    Thread.sleep(3000);
                } catch (InterruptedException ex)
                {
                }

                try
                {
                    login(host, userName, password);
                } catch (Exception ex)
                {
                }
            }
        } catch (Exception ex)
        {
        }
    }

    @Override
    public void onLogMessage(Level level, String message)
    {
        handler.obtainMessage(0, new Object[]{level, message}).sendToTarget();
    }

    private void onLogMessage(Object[] params)
    {
        messages.add(0, new MessageViewModel((Level) params[0], (String) params[1]));
    }

    public void setHelloProtocolObserver(IHelloProtocolObserver observer)
    {
        hpc.setHelloObserver(observer);
    }

    public void sayHello(String fullName) throws Exception
    {
        hpc.helloAsync(fullName);
    }
}
