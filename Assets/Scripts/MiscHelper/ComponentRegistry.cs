using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A register, so a unique named list, of components to keep safe, and make accessible.
/// </summary>
public static class ComponentRegistry
{
    private static readonly Dictionary<string, Component> Components = new();
    private static readonly List<Subscriber> Subscribers = new();

    /// <summary>Represents a subscriber for additions to the component register</summary>
    private class Subscriber
    {
        public string ComponentType;
        public Action toCall;
    }
    
    /// <param name="component">The component to make accessible globally</param>
    /// <returns>A bool that's false on failure, meaning there's already a component of the same type inside the register</returns>
    public static bool AddToRegistry(Component component)
    {
        string type = component.GetType().ToString();
        bool result = Components.TryAdd(type, component);
        TryCallSubscribers(type);
        return result;
    }

    private static void TryCallSubscribers(string type)
    {
        Subscriber[] subscriber = Subscribers.Where(s => s.ComponentType == type).ToArray();
        if (subscriber.Length == 0) return;
        foreach (Subscriber sub in subscriber)
        {
            sub.toCall?.Invoke();
            Subscribers.Remove(sub);
        }
    }

    /// <param name="component">The component to remove from global access</param>
    /// <returns>A bool that's false when the component was not found in the register</returns>
    public static bool RemoveFromRegistry(Component component) => Components.Remove(component.GetType().ToString());
    
    /// <param name="component">The component to make accessible globally</param>
    /// <returns>Should not be able to return false, but returns false on failure</returns>
    public static bool ReplaceInRegistry(Component component)
    {
        string name = component.GetType().ToString();
        Components.Remove(name);
        return AddToRegistry(component);
    }
    
    /// <typeparam name="T">Type to get from the register</typeparam>
    /// <returns>The desired component of type, or null</returns>
    public static T GetComponent<T>() where T : Component => (T)Components.GetValueOrDefault(typeof(T).ToString());

    /// <summary>Lets you call something the moment a certain component is registered</summary>
    /// <param name="toCall">This method/action will be called and removed once the type is added to the registry</param>
    /// <typeparam name="T">Type to get from the register</typeparam>
    public static void TrySubscribeForComponent<T>(Action toCall)
    {
        string type = typeof(T).ToString();
        Subscriber found = Subscribers.FirstOrDefault(s => s.ComponentType == type && s.toCall == toCall);
        if (found != null)
            return;
        Subscribers.Add(new Subscriber { ComponentType = type, toCall = toCall });
    }
}