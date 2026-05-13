
/// <summary>
/// Makes an object be able to be used within a objectPool
/// </summary>
public interface IPoolable
{
    public bool Active { get; set; }

    /// <summary>Triggers when an object gets set to the active state</summary>
    void OnEnableObject();
    
    /// <summary>Triggers when an object gets set to the inactive state</summary>
    void OnDisableObject();
}