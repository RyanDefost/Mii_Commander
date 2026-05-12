using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;

/// <summary>
/// Simple object pool, used for reusing poolable objects
/// </summary>
/// <typeparam name="T">Poolable, an object that has the ability to be pooled</typeparam>
public class ObjectPool<T> where T : class, IPoolable
{
    private readonly List<T> activePool = new();
    private readonly List<T> inactivePool = new();

    /// <summary>
    /// Calls enable, and sets active on the object
    /// Removes them from the inactive pool if possible
    /// And registers inside the active pool
    /// </summary>
    /// <param name="obj">The poolable object</param>
    /// <returns>The mutated object</returns>
    public T ActivateObject(T obj)
    {
        obj.OnEnableObject();
        obj.Active = true;

        if (this.inactivePool.Contains(obj)) 
            this.inactivePool.Remove(obj);
        this.activePool.Add(obj);
        return obj;
    }

    /// <summary>
    /// Calls disable, and sets inactive on the object
    /// Removes them from the active pool if possible
    /// And registers inside the inactive pool
    /// </summary>
    /// <param name="obj">The poolable object</param>
    /// <returns>The mutated object</returns>
    public T DeactivateObject(T obj)
    {
        obj.OnDisableObject();
        obj.Active = false;

        if (this.activePool.Contains(obj)) 
            this.activePool.Remove(obj);
        this.inactivePool.Add(obj);
        return obj;
    }

    /// <summary>
    /// Sets the object to inactive and removes it from the pool entirely
    /// </summary>
    /// <param name="obj">The poolable object</param>
    public void RemoveFromPool(T obj)
    {
        if (obj.Active)
        {
            obj.OnDisableObject();
            obj.Active = false;
        }
        
        if (this.activePool.Contains(obj)) 
            this.activePool.Remove(obj);
        
        if (this.inactivePool.Contains(obj)) 
            this.inactivePool.Remove(obj);
    }

    /// <summary>
    /// Adds a range of poolable objects to the pool
    /// </summary>
    /// <param name="collection">The collection of poolable objects to add</param>
    public void AddRangeToPool(IEnumerable<T> collection)
    {
        foreach (T obj in collection)
        {
            if (obj.Active)
                ActivateObject(obj);
            else
                DeactivateObject(obj);
        }
    }
    
    /// <summary>Used for getting an obj to reuse</summary>
    /// <returns>Tries to get an object from the inactive pool, returns null when unable</returns>
    public T RequestObject() => this.inactivePool.Count > 0 ? ActivateObject(this.inactivePool[0]) : null;

    /// <returns>A list of all objects within the pool</returns>
    public List<T> GetAllObjects()
    {
        List<T> itemList = new();
        itemList.AddRange(this.activePool);
        itemList.AddRange(this.inactivePool);

        return itemList;
    }
}