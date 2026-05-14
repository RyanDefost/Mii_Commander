using PlayerHand;
using UnityEngine;

/// <summary>
/// Lets a boardItem connect to the player hand
/// </summary>
public class HandHandler : BoardItemComponent
{
    private PlayerHandManager playerHandRef;
    
    public override void ConnectToBoardItem()
    { 
        this.playerHandRef = ComponentRegistry.GetComponent<PlayerHandManager>();
        this.boardItem.OnAddToHand += OnAddToHand; 
    }

    private void OnDestroy() => this.boardItem.OnAddToHand -= OnAddToHand;

    private void OnAddToHand()
    {
        this.playerHandRef.SetStateToGrabbing();
        this.transform.SetParent(this.playerHandRef.transform);
        this.playerHandRef.OnGrabReleased += OnGrabReleased;
    }
    
    /// <summary>
    /// Frees its physics restrictions,
    /// and adds any force the hand wanted to impose on it
    /// </summary>
    private void OnGrabReleased(Vector2 handMovementDir, Vector2 throwForce)
    {
        OnGrabReleased(this.boardItem.Rb, this.boardItem, handMovementDir, throwForce);
        this.playerHandRef.OnGrabReleased -= OnGrabReleased;
    }
    
    /// <summary>
    /// Restricts its own physics,
    /// adds to the player hand,
    /// and hooks into the player hand events
    /// </summary>
    public void AddToHand()
    {
        this.boardItem.OnAddToHand?.Invoke();
    
        this.boardItem.Rb.constraints = RigidbodyConstraints.FreezePosition;
        this.transform.localPosition = Vector3.zero;
    }
    
    /// <summary>Static function that handles the throwing logic</summary>
    public static void OnGrabReleased(Rigidbody rb, BoardItem boardItem, Vector2 handMovementDir, Vector2 throwForce)
    {
        rb.transform.parent = rb.transform.parent.parent;
        rb.constraints = RigidbodyConstraints.None;
        Vector3 throwVel = throwForce;
        if (throwForce.magnitude > 0.1f)
        {
            throwVel += Quaternion.Euler(0, 0, Random.Range(-90, 90)) * handMovementDir;
            rb.AddForce(throwVel, ForceMode.Impulse);
        }
        boardItem.Initiate();
    }
}