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

package com.us.openserver.helloclient;

import android.os.Handler;
import android.os.Looper;
import android.os.Message;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.view.View;
import android.widget.EditText;
import android.widget.Toast;

import com.us.openserver.helloclient.recyclerview.DividerItemDecoration;
import com.us.openserver.helloclient.recyclerview.adapter.BindingRecyclerViewAdapter;
import com.us.openserver.helloclient.recyclerview.adapter.binder.ItemBinder;
import com.us.openserver.protocols.hello.*;

import butterknife.Bind;
import butterknife.ButterKnife;

public class MainActivity extends AppCompatActivity implements IHelloProtocolObserver
{
    @Bind(R.id.txtFullName)
    EditText txtFullName;

    @Bind(R.id.lvwMessages)
    RecyclerView lvwMessages;

    private Handler handler;

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        ButterKnife.bind(this);

        lvwMessages.setLayoutManager(new LinearLayoutManager(this));
        lvwMessages.addItemDecoration(new DividerItemDecoration(this));

        BindingRecyclerViewAdapter adapter = new BindingRecyclerViewAdapter(
                lvwMessages,
                new ItemBinder(com.us.openserver.helloclient.BR.vm, R.layout.row_message),
                LoginActivity.controller.messages);

        lvwMessages.setAdapter(adapter);

        handler = new Handler(Looper.getMainLooper())
        {
            @Override
            public void handleMessage(Message inputMessage)
            {
                switch (inputMessage.what)
                {
                    case 0:
                        onHelloCompleteEx((String)inputMessage.obj);
                        break;
                }
            }
        };

        LoginActivity.controller.setHelloProtocolObserver(this);
    }

    public void sayHello(View view)
    {
        try
        {
            LoginActivity.controller.sayHello(txtFullName.getText().toString());
        }
        catch (Exception ex)
        {
            Toast.makeText(this, ex.getMessage(), Toast.LENGTH_LONG).show();
        }
    }

    @Override
    public void onHelloComplete(String s)
    {
        handler.obtainMessage(0, s).sendToTarget();
    }

    public void onHelloCompleteEx(String response)
    {
        Toast.makeText(this, response, Toast.LENGTH_LONG).show();
    }
}
