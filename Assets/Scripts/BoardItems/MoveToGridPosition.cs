using System;
using Grid;
using UnityEngine;

/// <summary>
/// Animates a piece of candy towards a grid position, via physics, I forgot how it works...
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class MoveToGridPosition : GridMoveable
{
    [SerializeField] 
    private Rigidbody rb;
    [SerializeField]
    private float movementStrength = 5f;
    [SerializeField]
    private float maxVelocity = 20f;
    [SerializeField]
    private float dampening = 2f;
    [SerializeField]
    private float startDelay;
    private Timer startDelayTimer;
    [SerializeField]
    private float endTimerTime = 15f;
    private Timer endTimer;
    private float usedMinimumForce;

    protected override void CustomOnValidate()
    {
        if (Application.isPlaying) return;
        this.rb = GetComponent<Rigidbody>();
    }
    
    protected override void TriggerMovementToTarget()
    {
        if (!HasTarget())
            TriggerUpdateTarget();
        this.startDelayTimer = new Timer(this.startDelay,  false, true, EnableMovement);
        this.onUpdate += UpdateStartDelayTimer;
    }

    protected override void StopMovementToTarget()
    {
        this.onUpdate -= UpdateStartDelayTimer;
        this.onUpdate -= ApplyForceTowardsTarget;
        this.onUpdate -= UpdateEndTimer;
        ResetTarget();
    }

    private void UpdateStartDelayTimer() => this.startDelayTimer.UpdateTime(Time.deltaTime);

    private void EnableMovement()
    {
        if (!UpdateTarget())
            return;
        this.usedMinimumForce = this.movementStrength;
        this.endTimer = new Timer(this.endTimerTime, false, true, SnapToTarget);
        
        this.onUpdate += ApplyForceTowardsTarget;
        this.onUpdate += UpdateEndTimer;
        this.onUpdate -= UpdateStartDelayTimer;
        
        this.startDelayTimer.ResetAndReplay();
    }

    private void UpdateEndTimer() => this.endTimer.UpdateTime(Time.deltaTime);

    protected override void SnapToTarget()
    {
        if (this.Target == null)
            return;
        base.SnapToTarget();
        this.rb.linearVelocity = Vector3.zero;
        this.rb.angularVelocity = Vector3.zero;
        this.rb.Sleep();
        this.onUpdate -= UpdateEndTimer;
        
        this.endTimer.ResetAndReplay();
    }
    
    private void ApplyForceTowardsTarget()
    {
        if (this.Target == null)
            return;
        Vector3 targetPosition = GetTargetPosition();
        Vector3 diff = targetPosition - this.transform.position;

        Vector3 velocity = this.rb.linearVelocity;
        float dot = velocity.sqrMagnitude > 0.001f
            ? Vector3.Dot(diff.normalized, velocity.normalized)
            : 0f;

        // Reduce force when already moving toward target
        float alignmentFactor = 1f - Mathf.Clamp01(dot); 
        // dot = 1 → factor = 0 (no extra push)
        // dot = 0 → factor = 1 (normal)
        // dot = -1 → factor = 2 (strong correction)

        float force = Mathf.Max(diff.magnitude, this.usedMinimumForce) * alignmentFactor;
        this.rb.AddForce(diff.normalized * force, ForceMode.Force);
        
        if (this.rb.linearVelocity.magnitude > this.maxVelocity)
            this.rb.linearVelocity = this.rb.linearVelocity.normalized * this.maxVelocity;

        if (!(diff.magnitude < 0.2f)) return;
        this.rb.linearVelocity *= 1f - Time.deltaTime * this.dampening;
        this.usedMinimumForce = Mathf.Max(
            this.usedMinimumForce - Time.deltaTime * this.dampening,
            0f
        );
        if (this.rb.linearVelocity.magnitude < 0.2f)
            SnapToTarget();
    }
}
