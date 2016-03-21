package com.us.openserver.helloclient.recyclerview.adapter.binder;

public class ItemBinder<T>
{
      protected final int bindingVariable;
      protected final int layoutId;

      public ItemBinder(int bindingVariable, int layoutId)
      {
            this.bindingVariable = bindingVariable;
            this.layoutId = layoutId;
      }

      public int getLayoutRes(T model)
      {
            return layoutId;
      }

      public int getBindingVariable(T model)
      {
            return bindingVariable;
      }
}
