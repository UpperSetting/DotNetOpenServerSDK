package com.us.openserver.helloclient.recyclerview.adapter;

import android.databinding.DataBindingUtil;
import android.databinding.ObservableArrayList;
import android.databinding.ObservableList;
import android.databinding.ViewDataBinding;
import android.support.annotation.Nullable;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import com.us.openserver.helloclient.recyclerview.adapter.binder.ItemBinder;

import java.lang.ref.WeakReference;
import java.util.Collection;

public class BindingRecyclerViewAdapter<T> extends RecyclerView.Adapter<BindingRecyclerViewAdapter.ViewHolder>
{
    private static final int ITEM_MODEL = -124;
    private final WeakReferenceOnListChangedCallback onListChangedCallback;
    private final ItemBinder<T> itemBinder;
    protected ObservableList<T> items;
    protected LayoutInflater inflater;
    private int selectedItem = -1;
    private final RecyclerView view;


    public static class ViewHolder extends RecyclerView.ViewHolder
    {
        public final ViewDataBinding binding;

        protected ViewHolder(ViewDataBinding binding)
        {
            super(binding.getRoot());
            this.binding = binding;
        }
    }

    public BindingRecyclerViewAdapter(RecyclerView view, ItemBinder<T> itemBinder, @Nullable Collection<T> items)
    {
        this.view = view;
        this.itemBinder = itemBinder;
        this.onListChangedCallback = new WeakReferenceOnListChangedCallback<>(this);
        setItems(items);
    }

    public ObservableList<T> getItems()
    {
        return items;
    }

    public void setItems(@Nullable Collection<T> items)
    {
        if (this.items == items)
        {
            return;
        }

        if (this.items != null)
        {
            this.items.removeOnListChangedCallback(onListChangedCallback);
            notifyItemRangeRemoved(0, this.items.size());
        }

        if (items instanceof ObservableList)
        {
            this.items = (ObservableList<T>) items;
            notifyItemRangeInserted(0, this.items.size());
            this.items.addOnListChangedCallback(onListChangedCallback);
        }
        else if (items != null)
        {
            this.items = new ObservableArrayList<>();
            this.items.addOnListChangedCallback(onListChangedCallback);
            this.items.addAll(items);
        }
        else
        {
            this.items = null;
        }
    }

    @Override
    public void onDetachedFromRecyclerView(RecyclerView recyclerView)
    {
        if (items != null)
        {
            items.removeOnListChangedCallback(onListChangedCallback);
        }
    }

    @Override
    public ViewHolder onCreateViewHolder(ViewGroup viewGroup, int layoutId)
    {
        if (inflater == null)
        {
            inflater = LayoutInflater.from(viewGroup.getContext());
        }

        ViewDataBinding binding = DataBindingUtil.inflate(inflater, layoutId, viewGroup, false);
        return new ViewHolder(binding);
    }

    @Override
    public void onBindViewHolder(ViewHolder viewHolder, int position)
    {
        final T item = items.get(position);
        viewHolder.binding.setVariable(itemBinder.getBindingVariable(item), item);
        viewHolder.binding.getRoot().setTag(ITEM_MODEL, item);
        viewHolder.binding.executePendingBindings();
    }

    @Override
    public int getItemViewType(int position)
    {
        return itemBinder.getLayoutRes(items.get(position));
    }

    @Override
    public int getItemCount()
    {
        return items == null ? 0 : items.size();
    }

    public void setSelectedItem(int position)
    {
        selectedItem = position;

    }

    private static class WeakReferenceOnListChangedCallback<T> extends ObservableList.OnListChangedCallback
    {
        private final WeakReference<BindingRecyclerViewAdapter<T>> adapterReference;
        private final RecyclerView view;

        public WeakReferenceOnListChangedCallback(BindingRecyclerViewAdapter<T> bindingRecyclerViewAdapter)
        {
            this.adapterReference = new WeakReference<>(bindingRecyclerViewAdapter);
            this.view = bindingRecyclerViewAdapter.view;
        }

        @Override
        public void onChanged(ObservableList sender)
        {
            RecyclerView.Adapter adapter = adapterReference.get();
            if (adapter != null)
            {
                adapter.notifyDataSetChanged();
            }
        }

        @Override
        public void onItemRangeChanged(ObservableList sender, int positionStart, int itemCount)
        {
            RecyclerView.Adapter adapter = adapterReference.get();
            if (adapter != null)
            {
                adapter.notifyItemRangeChanged(positionStart, itemCount);
            }
        }

        @Override
        public void onItemRangeInserted(ObservableList sender, int positionStart, int itemCount)
        {
            BindingRecyclerViewAdapter adapter = adapterReference.get();
            if (adapter != null)
            {
                adapter.notifyItemRangeInserted(positionStart, itemCount);
                adapter.setSelectedItem(positionStart);
                view.scrollToPosition(positionStart);
            }
        }

        @Override
        public void onItemRangeMoved(ObservableList sender, int fromPosition, int toPosition, int itemCount)
        {
            RecyclerView.Adapter adapter = adapterReference.get();
            if (adapter != null)
            {
                adapter.notifyItemMoved(fromPosition, toPosition);
            }
        }

        @Override
        public void onItemRangeRemoved(ObservableList sender, int positionStart, int itemCount)
        {
            RecyclerView.Adapter adapter = adapterReference.get();
            if (adapter != null)
            {
                adapter.notifyItemRangeRemoved(positionStart, itemCount);
            }
        }
    }
}