using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;

public class ObjectPool<T> where T : class, IPoolable
{
    private readonly List<T> activePool = new();
    private readonly List<T> inactivePool = new();

    public T ActivateObject(T item)
    {
        item.OnEnableObject();
        item.Active = true;

        if (this.inactivePool.Contains(item))
        {
            this.inactivePool.Remove(item);
        }
        this.activePool.Add(item);
        return item;
    }

    public T DeactivateObject(T item)
    {
        item.OnDisableObject();
        item.Active = false;

        if (this.activePool.Contains(item))
        {
            this.activePool.Remove(item);
        }
        this.inactivePool.Add(item);
        return item;
    }
    
    public T RequestObject() => this.inactivePool.Count > 0 ? ActivateObject(this.inactivePool[0]) : null;

    public List<T> GetAllItems()
    {
        List<T> ItemList = new List<T>();
        ItemList.AddRange(this.activePool);
        ItemList.AddRange(this.inactivePool);

        return ItemList;
    }
}