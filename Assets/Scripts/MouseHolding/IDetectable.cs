using UnityEngine;

public enum DetectableTypes
{
    GOOBER = 0,
    ENEMY = 1,
    PROP = 2,
}

public interface IDetectable
{
    public DetectableTypes DetectableType { get; }
    public GameObject GameObject { get; }
    public Rigidbody Rigidbody { get; }
    
    public void OnHold();
    public void OnReleaseHold();
}
