package com.us.openserver.helloclient;

import android.databinding.BaseObservable;
import android.databinding.Bindable;

import com.us.openserver.Level;

public class MessageViewModel extends BaseObservable
{
    @Bindable
    public Level level;

    @Bindable
    public String message;

    public MessageViewModel(Level level, String message)
    {
        this.level = level;
        this.message = message;
    }
}
