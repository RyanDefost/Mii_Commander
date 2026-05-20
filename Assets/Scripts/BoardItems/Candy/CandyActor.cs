using UnityEngine;

/// <summary>
/// Manages the in game logic of a piece of candy
/// </summary>
[RequireComponent(typeof(MoveToGridPosition), typeof(HandHandler))]
public class CandyActor : BoardItem
{
    [SerializeField] private int points;
    public int Points { get => this.points; private set => this.points = value; }

    public int candyType; // index reference to lookup
    
    [SerializeField]
    private MoveToGridPosition movement;
    [SerializeField]
    private HandHandler handHandler;
    
    protected virtual void CustomOnValidate()
    {
        this.movement = GetComponent<MoveToGridPosition>();
        this.handHandler = GetComponent<HandHandler>();
    }

    public void OnGrabReleased(Rigidbody rb, Vector2 handMovementDir, Vector2 throwForce)
    {
        this.movement.moveImmunity = true;
        HandHandler.OnGrabReleased(rb, this, handMovementDir, throwForce);
        this.OnAddToBoard += RemovePlayerMoveImmunity;
    }

    private void RemovePlayerMoveImmunity()
    {
        this.movement.moveImmunity = false;
        this.OnAddToBoard -= RemovePlayerMoveImmunity;
    }
}