using PlayerHand;
using UnityEngine;

/// <summary>
/// Manages the in game logic of a piece of candy
/// </summary>
[RequireComponent(typeof(MoveToGridPosition), typeof(HandHandler))]
public class CandyActor : BoardItem
{
    [SerializeField] private int points;
    public int Points { get => points; private set => points = value; }

    public int candyType; // index reference to lookup
    
    [SerializeField]
    private MoveToGridPosition movement;
    [SerializeField]
    private HandHandler handHandler;
    
    protected override void CustomOnValidate()
    {
        this.movement = GetComponent<MoveToGridPosition>();
        this.handHandler = GetComponent<HandHandler>();
    }

    public override void ActivateAbility() { print("BASIC CANDY ABILITY"); }

    public static void OnGrabReleased(Rigidbody rb, BoardItem boardItem, Vector2 handMovementDir, Vector2 throwForce) => 
        HandHandler.OnGrabReleased(rb, boardItem, handMovementDir, throwForce);

    public void ApplyPointMultiplier(float multiplier) => this.points = Mathf.FloorToInt(this.points * multiplier);
}