using System;
using UnityEngine;

namespace BoardItems
{
    /// <summary>
    /// Animates a piece of candy towards a grid position, via physics, I forgot how it works...
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class MoveToGridPosition : GridMoveable
    {
        [SerializeField] 
        private PhysicsData physicsData;
        private float currentResistanceMultiplier = 1f;
    
        [Header("Timers")]
        [SerializeField]
        private float startDelay;
        private Timer startDelayTimer;
        [SerializeField]
        private float endTimerTime = 15f;
        private Timer endTimer;
        private float usedMinimumForce;
    
        [Serializable]
        public class PhysicsData
        {
            [Header("Base movement")]
            public float movementStrength = 5f;
            public float maxVelocity = 20f;
            public float dampening = 2f;
        
            [Header("Resistance / Anti-Stuck")]
            public float stuckVelocityThreshold = 0.5f;
            public float resistanceRampRate = 15f;
            public float maxResistanceMultiplier = 5f;
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
            this.usedMinimumForce = this.physicsData.movementStrength;
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
            this.boardItem.Rb.linearVelocity = Vector3.zero;
            this.boardItem.Rb.angularVelocity = Vector3.zero;
            this.boardItem.Rb.Sleep();
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

            Vector3 velocity = this.boardItem.Rb.linearVelocity;
            float speed = velocity.magnitude;
        
            // If far from target but moving slower than the threshold, this means there's resistance
            if (distance > 0.3f && speed < this.physicsData.stuckVelocityThreshold)
            {
                this.currentResistanceMultiplier = Mathf.Min(
                    this.currentResistanceMultiplier + Time.deltaTime * this.physicsData.resistanceRampRate,
                    this.physicsData.maxResistanceMultiplier
                );
            }
            else
            {
                // Smoothly lower the multiplier because there's no resistance
                this.currentResistanceMultiplier =
                    Mathf.Max(this.currentResistanceMultiplier - Time.deltaTime * this.physicsData.dampening, 1f);
            }

            float dot = speed > 0.001f
                ? Vector3.Dot(diff.normalized, velocity.normalized)
                : 0f;
            float alignmentFactor = 1f - Mathf.Clamp01(dot); 

            // Apply Resistance Multiplier
            float baseForce = Mathf.Max(distance, this.usedMinimumForce) * alignmentFactor;
            float finalForce = baseForce * this.currentResistanceMultiplier;
        
            this.boardItem.Rb.AddForce(diff.normalized * finalForce, ForceMode.Force);
        
            // If stuck, allow a slightly higher max velocity temporarily to break free aggressively
            float effectiveMaxVelocity = this.physicsData.maxVelocity * (this.currentResistanceMultiplier > 1.5f ? 1.5f : 1f);
            if (speed > effectiveMaxVelocity)
                this.boardItem.Rb.linearVelocity = velocity.normalized * effectiveMaxVelocity;

            // Arrival, dampening
            if (!(distance < 0.2f)) return;
            this.boardItem.Rb.linearVelocity *= 1f - Time.deltaTime * this.physicsData.dampening;
            this.usedMinimumForce = Mathf.Max(
                this.usedMinimumForce - Time.deltaTime * this.physicsData.dampening,
                0f
            );
            if (this.boardItem.Rb.linearVelocity.magnitude < 0.2f)
                SnapToTarget();
        }
    }
}
