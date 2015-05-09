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

package com.us.openserver.helloclient;

import android.app.AlertDialog;
import android.os.*;
import android.support.v7.app.ActionBarActivity;
import android.view.*;
import android.widget.*;

import com.us.openserver.*;
import com.us.openserver.configuration.*;
import com.us.openserver.protocols.hello.*;
import com.us.openserver.protocols.keepalive.*;
import com.us.openserver.protocols.winauth.*;

import java.util.HashMap;

public class MainActivity extends ActionBarActivity implements IClientObserver
{
    private EditText txtHost;
    private EditText txtUserName;
    private EditText txtPassword;
    private Button btnConnect;
    private Client client;
    private Handler handler;

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        txtHost = (EditText)findViewById(R.id.txtHost);
        txtUserName = (EditText)findViewById(R.id.txtUserName);
        txtPassword = (EditText)findViewById(R.id.txtPassword);
        btnConnect = (Button)findViewById(R.id.btnConnect);

        handler = new Handler(Looper.getMainLooper())
        {
            @Override
            public void handleMessage(Message inputMessage)
            {
                switch (inputMessage.what)
                {
                    case 0://connection lost
                        onConnectionLostEx((Exception)inputMessage.obj);
                        break;

                }
            }
        };
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu)
    {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_main, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item)
    {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();

        //noinspection SimplifiableIfStatement
        if (id == R.id.action_settings)
        {
            return true;
        }

        return super.onOptionsItemSelected(item);
    }

    public void onConnect(View v)
    {
        try
        {
            if (btnConnect.getText() == "Disconnect")
            {
                if (client != null)
                    client.closeAsync();

                btnConnect.setText("Connect");
            }
            else
                connect();
        }
        catch (Exception ex)
        {
            showMessageBox(ex.getMessage());
        }
    }

    private void connect() throws Exception
    {
        ServerConfiguration cfg = new ServerConfiguration();
        cfg.setHost(txtHost.getText().toString());
        /*
        TlsConfiguration tls = cfg.getTlsConfiguration();
        tls.setEnabled(false);
        tls.setAllowCertificateChainErrors(true);
        tls.setAllowSelfSignedCertificate(true);
        tls.setCheckCertificateRevocation(false);
        tls.setRequireRemoteCertificate(true);
        */

        HashMap<Integer, ProtocolConfiguration> protocolConfigurations =
            new HashMap<Integer, ProtocolConfiguration>();

        protocolConfigurations.put(KeepAliveProtocol.PROTOCOL_IDENTIFIER,
            new ProtocolConfiguration(KeepAliveProtocol.PROTOCOL_IDENTIFIER, "com.us.openserver.protocols.keepalive.KeepAliveProtocol"));

        protocolConfigurations.put(WinAuthProtocol.PROTOCOL_IDENTIFIER,
            new ProtocolConfiguration(WinAuthProtocol.PROTOCOL_IDENTIFIER, "com.us.openserver.protocols.winauth.WinAuthProtocolClient"));

        protocolConfigurations.put(HelloProtocolClient.PROTOCOL_IDENTIFIER,
            new ProtocolConfiguration(HelloProtocolClient.PROTOCOL_IDENTIFIER, "com.us.openserver.protocols.hello.HelloProtocolClient"));

        client = new Client(this, cfg, protocolConfigurations);

        try
        {
            client.connectAsync();

            WinAuthProtocolClient wap = (WinAuthProtocolClient)client.initializeAsync(WinAuthProtocol.PROTOCOL_IDENTIFIER);
            if (!wap.authenticate(txtUserName.getText().toString(), txtPassword.getText().toString(), null))
                throw new Exception("Access denied.");

            client.initializeAsync(KeepAliveProtocol.PROTOCOL_IDENTIFIER);

            HelloProtocolClient hpc = (HelloProtocolClient)client.initializeAsync(HelloProtocol.PROTOCOL_IDENTIFIER);
            String serverResponse = hpc.hello(txtUserName.getText().toString());
            showMessageBox(serverResponse);

            btnConnect.setText("Disconnect");
        }
        catch (Exception ex)
        {
            client.closeAsync();
            throw ex;
        }
    }

    public void onConnectionLost(Exception ex)
    {
        handler.obtainMessage(0, ex).sendToTarget();
    }

    private void onConnectionLostEx(Exception ex)
    {
        showMessageBox("Connection Lost\r\n\r\n" + ex.getMessage());
        btnConnect.setText("Connect");
    }

    private void showMessageBox(String message)
    {
        AlertDialog.Builder dlgAlert  = new AlertDialog.Builder(this);

        dlgAlert.setMessage(message);
        dlgAlert.setTitle("DotNetOpenServer");
        dlgAlert.setPositiveButton("OK", null);
        dlgAlert.setCancelable(true);
        dlgAlert.create().show();
    }
}
