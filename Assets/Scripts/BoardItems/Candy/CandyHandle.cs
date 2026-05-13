using UnityEngine;

/// <summary>
/// Manages the data of a candy instance,
/// counterpart to the CandyComponent that handles its in game logic
/// </summary>
public class CandyHandle
{
    public readonly Rigidbody rb;
    public readonly CandyActor actor;
    public readonly GameObject gameObject;

    public CandyHandle(Rigidbody rb, CandyActor candyActor, GameObject gameObject)
    {
        this.rb = rb;
        this.actor = candyActor;
        this.gameObject = gameObject;
    }
}