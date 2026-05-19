using System;
using Grid;
using UnityEngine;

/// <summary>
/// Animates a piece of candy towards a grid position, via physics, I forgot how it works...
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class MoveToGridPosition : GridMoveable
{
    [Header("Required Components")]
    [SerializeField] 
    private Rigidbody rb;
    
    [Header("Base movement")]
    [SerializeField]
    private float movementStrength = 5f;
    [SerializeField]
    private float maxVelocity = 20f;
    [SerializeField]
    private float dampening = 2f;
    
    [Header("Timers")]
    [SerializeField]
    private float startDelay;
    private Timer startDelayTimer;
    [SerializeField]
    private float endTimerTime = 15f;
    private Timer endTimer;
    private float usedMinimumForce;
    
    [Header("Resistance / Anti-Stuck")]
    [SerializeField]
    private float stuckVelocityThreshold = 0.5f;
    [SerializeField]
    private float resistanceRampRate = 15f;
    [SerializeField]
    private float maxResistanceMultiplier = 5f;
    private float currentResistanceMultiplier = 1f;

    protected override void CustomOnValidate()
    {
        if (Application.isPlaying) return;
        this.rb = GetComponent<Rigidbody>();
    }
    
    protected override void TriggerMovementToTarget()
    {
        if (!HasTarget())
            TriggerUpdateTargetWithImmunityState();
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
        if (!UpdateTarget(!this.moveImmunity))
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
        this.currentResistanceMultiplier = 1f;
    }
    
    private void ApplyForceTowardsTarget()
    {
        if (this.Target == null)
            return;
        Vector3 targetPosition = GetTargetPosition();
        Vector3 diff = targetPosition - this.transform.position;
        float distance = diff.magnitude;

        Vector3 velocity = this.rb.linearVelocity;
        float speed = velocity.magnitude;
        
        // If far from target but moving slower than the threshold, this means there's resistance
        if (distance > 0.3f && speed < this.stuckVelocityThreshold)
        {
            this.currentResistanceMultiplier = Mathf.Min(
                this.currentResistanceMultiplier + Time.deltaTime * this.resistanceRampRate,
                this.maxResistanceMultiplier
            );
        }
        else
        {
            // Smoothly lower the multiplier because there's no resistance
            this.currentResistanceMultiplier =
                Mathf.Max(this.currentResistanceMultiplier - Time.deltaTime * this.dampening, 1f);
        }

        float dot = speed > 0.001f
            ? Vector3.Dot(diff.normalized, velocity.normalized)
            : 0f;
        float alignmentFactor = 1f - Mathf.Clamp01(dot); 

        // Apply Resistance Multiplier
        float baseForce = Mathf.Max(distance, this.usedMinimumForce) * alignmentFactor;
        float finalForce = baseForce * this.currentResistanceMultiplier;
        
        this.rb.AddForce(diff.normalized * finalForce, ForceMode.Force);
        
        // If stuck, allow a slightly higher max velocity temporarily to break free aggressively
        float effectiveMaxVelocity = this.maxVelocity * (this.currentResistanceMultiplier > 1.5f ? 1.5f : 1f);
        if (speed > effectiveMaxVelocity)
            this.rb.linearVelocity = velocity.normalized * effectiveMaxVelocity;

        // Arrival, dampening
        if (!(distance < 0.2f)) return;
        this.rb.linearVelocity *= 1f - Time.deltaTime * this.dampening;
        this.usedMinimumForce = Mathf.Max(
            this.usedMinimumForce - Time.deltaTime * this.dampening,
            0f
        );
        if (this.rb.linearVelocity.magnitude < 0.2f)
            SnapToTarget();
    }
}
