using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A register, so a unique named list, of components to keep safe, and make accessible.
/// </summary>
public static class ComponentRegistry
{
    private static readonly Dictionary<string, Component> Components = new();
    
    /// <param name="component">The component to make accessible globally</param>
    /// <returns>A bool that's false on failure, meaning there's already a component of the same type inside the register</returns>
    public static bool AddToRegistry(Component component) => Components.TryAdd(component.GetType().ToString(), component);
    
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

    // TODO could add a function here where you subscribe and wait for registration.
}