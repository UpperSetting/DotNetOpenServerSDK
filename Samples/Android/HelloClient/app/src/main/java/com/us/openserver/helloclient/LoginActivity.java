package com.us.openserver.helloclient;

import android.app.Activity;
import android.app.ProgressDialog;
import android.content.Intent;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.os.Message;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

import java.net.SocketTimeoutException;

import butterknife.Bind;
import butterknife.ButterKnife;

public class LoginActivity extends Activity
{
    @Bind(R.id.input_host)
    EditText txtHost;
    @Bind(R.id.input_user)
    EditText txtUser;
    @Bind(R.id.input_password)
    EditText txtPassword;
    @Bind(R.id.btn_login)
    Button btnLogin;

    public static Controller controller;

    private Handler handler;

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);
        ButterKnife.bind(this);

        controller = new Controller();

        handler = new Handler(Looper.getMainLooper())
        {
            @Override
            public void handleMessage(Message inputMessage)
            {
                switch (inputMessage.what)
                {
                    case 0://On Connect Complete
                        onConnectCompleteEx((Exception) inputMessage.obj);
                        break;
                }
            }
        };
    }

    public void login(View v)
    {
        try
        {
            if (!validate())
            {
                btnLogin.setEnabled(true);
                return;
            }

            btnLogin.setEnabled(false);

            String host = txtHost.getText().toString();
            String userName = txtUser.getText().toString();
            String password = txtPassword.getText().toString();

            new MyTask(this, host, userName, password).execute();
        }
        catch (Exception ex)
        {
            txtPassword.setText("");
            btnLogin.setEnabled(true);
            Toast.makeText(this, ex.getMessage(), Toast.LENGTH_LONG).show();
        }
    }

    public boolean validate()
    {
        boolean valid = true;

        String host = txtHost.getText().toString();
        String userName = txtUser.getText().toString();
        String password = txtPassword.getText().toString();

        if (host.isEmpty()) {
            txtHost.setError("enter a host or IP");
            valid = false;
        } else {
            txtHost.setError(null);
        }

        if (userName.isEmpty()) {
            txtUser.setError("enter a username");
            valid = false;
        } else {
            txtUser.setError(null);
        }

        if (password.isEmpty()) {
            txtPassword.setError("enter a password");
            valid = false;
        } else {
            txtPassword.setError(null);
        }

        return valid;
    }

    public void onConnectComplete(Exception ex)
    {
        handler.obtainMessage(0, ex).sendToTarget();
    }

    public void onConnectCompleteEx(Exception ex)
    {
        if (ex == null)
        {
            startActivity(new Intent(this, MainActivity.class));
        }
        else
        {
            txtPassword.setText("");
            btnLogin.setEnabled(true);
            Toast.makeText(this, ex.getMessage(), Toast.LENGTH_LONG).show();
        }
    }

    private class MyTask extends AsyncTask<String, Void, Boolean>
    {
        private LoginActivity activity;
        private ProgressDialog dialog;
        private String host;
        private String userName;
        private String password;

        public MyTask(LoginActivity activity, String host, String userName, String password)
        {
            this.activity = activity;
            this.host = host;
            this.userName = userName;
            this.password = password;
            dialog = new ProgressDialog(activity);
        }

        protected void onPreExecute() {
            this.dialog.setMessage("Connecting...");
            this.dialog.show();
        }

        protected Boolean doInBackground(final String... args)
        {
            try
            {
                controller.login(host, userName, password);
                dialog.dismiss();
                activity.onConnectComplete(null);
                return true;
            }
            catch (SocketTimeoutException ex)
            {
                dialog.dismiss();
                activity.onConnectComplete(new Exception("Unable to connect to " + host));
            }
            catch (Exception ex)
            {
                dialog.dismiss();
                activity.onConnectComplete(ex);
            }
            return false;
        }
    }
}
