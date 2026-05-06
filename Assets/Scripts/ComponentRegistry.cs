using System;
using System.Collections.Generic;
using UnityEngine;

public static class ComponentRegistry
{
    private static Dictionary<string, Component> components = new();

    public static bool AddToRegistry(Component component) => components.TryAdd(component.GetType().ToString(), component);

    public static bool RemoveFromRegistry(Component component) => components.Remove(component.GetType().ToString());
    public static bool ReplaceInRegistry(Component component)
    {
        string name = component.GetType().ToString();
        components.Remove(name);
        return AddToRegistry(component);
    }
    
    public static T GetComponent<T>() where T : Component => (T)components.GetValueOrDefault(typeof(T).ToString());

    // could add a function here where you subscribe a wait for registration.
}