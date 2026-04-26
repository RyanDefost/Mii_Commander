using System;
using PlayerHand;
using UnityEngine;
using Random = UnityEngine.Random;

public class CandyComponent : MonoBehaviour
{
    [SerializeField]
    private Interactable interactable;
    [SerializeField]
    private MoveToGridPosition movement;
    [SerializeField]
    private Rigidbody rb;
    private PlayerHandManager playerHandRef;

    private void OnValidate()
    {
        if (Application.isPlaying)
            return;
        this.movement = GetComponent<MoveToGridPosition>();
        this.rb = GetComponent<Rigidbody>();
        this.interactable = GetComponentInChildren<Interactable>();
    }

    private void Start() => this.playerHandRef = ComponentRegistry.GetComponent<PlayerHandManager>();

    public void AddToHand()
    {
        this.movement.SetEnabled(false);
        this.playerHandRef.SetStateToGrabbing();

        this.transform.SetParent(this.playerHandRef.transform);
        this.rb.constraints = RigidbodyConstraints.FreezePosition;
        this.transform.localPosition = Vector3.zero;
        
        this.playerHandRef.OnGrabReleased += OnGrabReleased;
    }

    private void OnGrabReleased(Vector2 handMovementDir, Vector2 throwForce)
    {
        OnGrabReleased(this.rb, this, handMovementDir, throwForce);
        this.playerHandRef.OnGrabReleased -= OnGrabReleased;
    }

    private void Initiate() => this.movement.SetEnabled(true);

    public static void OnGrabReleased(Rigidbody rb, CandyComponent candyComponent, Vector2 handMovementDir, Vector2 throwForce)
    {
        rb.transform.parent = rb.transform.parent.parent;
        rb.constraints = RigidbodyConstraints.None;
        Vector3 throwVel = throwForce;
        if (throwForce.magnitude > 0.1f)
        {
            throwVel += Quaternion.Euler(0, 0, Random.Range(-90, 90)) * handMovementDir;
            rb.AddForce(throwVel, ForceMode.Impulse);
        }
        candyComponent.Initiate();
    }
}