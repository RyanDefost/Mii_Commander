using System;

public interface IUserInterfaceValueGetter
{
    /// <summary>
    /// Gets a value inside the class based on its name
    /// </summary>
    /// <param name="valueName">name of the desired value, a key of sorts</param>
    /// <param name="returnType">which type the returned obj is</param>
    /// <returns>desired object</returns>
    public object GetValue(string valueName, out Type returnType);
    
    public void SubscribeOnChangeValue(string valueName, Action<object, Type> callback);
    public void UnSubscribeOnChangeValue(string valueName, Action<object, Type> callback);
}