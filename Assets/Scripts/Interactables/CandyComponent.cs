using System;
using PlayerHand;
using UnityEngine;
using Random = UnityEngine.Random;

public class CandyComponent : Interactable
{
    [SerializeField]
    private MoveToGridPosition movement;
    [SerializeField]
    private Rigidbody rb;
    private PlayerHandManager playerHandRef;

    private void OnValidate()
    {
        if (Application.isPlaying)
            return;
        this.movement = this.transform.parent.GetComponent<MoveToGridPosition>();
        this.rb = this.transform.parent.GetComponent<Rigidbody>();
    }

    private void Start() => this.playerHandRef = ComponentRegistry.GetComponent<PlayerHandManager>();

    public void AddToHand()
    {
        this.movement.enabled = false;
        this.playerHandRef.SetStateToGrabbing();
        
        this.transform.parent.SetParent(this.playerHandRef.transform);
        this.rb.constraints = RigidbodyConstraints.FreezePosition;
        this.transform.parent.transform.localPosition = Vector3.zero;
        
        this.playerHandRef.OnGrabReleased += OnGrabReleased;
    }

    private void OnGrabReleased(Vector2 handMovementDir, Vector2 throwForce)
    {
        OnGrabReleased(this.rb, this, handMovementDir, throwForce);
        this.playerHandRef.OnGrabReleased -= OnGrabReleased;
    }

    private void Initiate() => this.movement.enabled = true;

    public static void OnGrabReleased(Rigidbody rb, CandyComponent candyComponent, Vector2 handMovementDir, Vector2 throwForce)
    {
        rb.transform.parent = rb.transform.parent.parent;
        rb.constraints = RigidbodyConstraints.None;
        Vector3 throwVel = throwForce;
        throwVel += Quaternion.Euler(0, 0, Random.Range(-90, 90)) * handMovementDir;
        rb.AddForce(throwVel, ForceMode.Impulse);
        candyComponent.Initiate();
    }
}