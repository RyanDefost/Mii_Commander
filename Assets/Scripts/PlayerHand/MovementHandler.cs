using System;
using UnityEngine;

namespace PlayerHand
{
    /// <summary>
    /// Handles player hand movement with acceleration, velocity tracking, and throw force calculation.
    /// </summary>
    public class MovementHandler : MonoBehaviour
    {
        [Header("Required Components")]
        [SerializeField] private Camera camRef;

        [Header("Sensitivity")]
        [SerializeField] private float baseSensitivity = 1f;
        [SerializeField] private int referenceScreenHeight = 1080;

        [Header("Acceleration")]
        [SerializeField] private float accelerationRate = 8f;
        [SerializeField] private float decelerationRate = 4f;
        [SerializeField] private float minSpeedMultiplier = 1f;
        [SerializeField] private float maxSpeedMultiplier = 3f;

        [Header("Velocity & Throwing")]
        [SerializeField] private float maxVelocity = 50f;
        [SerializeField] private float velocitySmoothing = 15f;
        [SerializeField] private AnimationCurve throwForceCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        
        // Runtime state
        private Bounds movementBounds;
        private float originalZPosition;
        private float targetZPosition;
        private bool isAnimatingZ;
        private float zLerpSpeed;

        private float currentSpeedMultiplier = 1f;
        private float pixelDensityScale = 1f;

        private Vector3 previousPosition;
        private Vector3 smoothedVelocity;

        public Vector2 Velocity => this.smoothedVelocity;
        public Vector2 MovementDirection =>
            this.smoothedVelocity.magnitude > 0.001f 
            ? ((Vector2)this.smoothedVelocity).normalized 
            : Vector2.zero;
        public Vector2 ThrowForce { get; private set; }


        private void OnValidate()
        {
            if (!Application.isPlaying) 
                this.camRef = Camera.main;
        }

        private void Awake()
        {
            this.originalZPosition = this.transform.position.z;
            this.targetZPosition = this.originalZPosition;
            this.previousPosition = this.transform.position;

            RecalculateBounds();
            RecalculatePixelDensityScale();

            ResolutionTracker.OnResolutionChanged += OnResolutionChanged;
        }

        private void OnDestroy() => ResolutionTracker.OnResolutionChanged -= OnResolutionChanged;

        private void OnResolutionChanged(int width, int height)
        {
            RecalculateBounds();
            RecalculatePixelDensityScale();
        }

        private void Update()
        {
            float dt = GetSafeDeltaTime();

            UpdateVelocity(dt);
            UpdateThrowForce();
            DecaySpeedMultiplier(dt);
            AnimateZPosition(dt);

            this.previousPosition = this.transform.position;
        }

        /// <summary>
        /// Called from the input handler
        /// </summary>
        public void ApplyMouseDelta(Vector2 delta)
        {
            if (delta.sqrMagnitude < 0.0001f)
                return;

            float dt = GetSafeDeltaTime();
            float inputSpeed = delta.magnitude / dt;

            // Accelerate based on input intensity
            float accelerationTarget = Mathf.Lerp(this.minSpeedMultiplier, this.maxSpeedMultiplier, 
                Mathf.Clamp01(inputSpeed / 1000f)); // Normalize input speed

            this.currentSpeedMultiplier = Mathf.MoveTowards(this.currentSpeedMultiplier,
                accelerationTarget, this.accelerationRate * dt
            );

            // Apply sensitivity with depth and pixel density compensation
            float depthScale = GetDepthScale();
            float effectiveSensitivity = this.baseSensitivity * this.pixelDensityScale * depthScale * this.currentSpeedMultiplier;

            Vector2 movement = delta * effectiveSensitivity;
            Vector3 newPosition = this.transform.position + new Vector3(movement.x, movement.y, 0f);

            // Clamp to bounds
            this.transform.position = ClampToBounds(newPosition);
        }

        public void SetTargetZ(float z, float speed = -1f)
        {
            this.targetZPosition = z;
            if (speed > 0f) this.zLerpSpeed = speed;
            this.isAnimatingZ = true;
        }

        public void ResetToOriginalZ(float speed = -1f) => SetTargetZ(this.originalZPosition, speed);

        private void UpdateVelocity(float dt)
        {
            Vector3 rawVelocity = (this.transform.position - this.previousPosition) / dt;
            
            // Clamp raw velocity
            if (rawVelocity.magnitude > this.maxVelocity)
                rawVelocity = rawVelocity.normalized * this.maxVelocity;

            // Smooth velocity - faster interpolation when accelerating, slower when decelerating
            float lerpFactor = rawVelocity.magnitude > this.smoothedVelocity.magnitude
                ? this.velocitySmoothing * 2f
                : this.velocitySmoothing;

            this.smoothedVelocity = Vector3.Lerp(this.smoothedVelocity, rawVelocity, lerpFactor * dt);
        }

        private void UpdateThrowForce()
        {
            float normalizedSpeed = Mathf.Clamp01(this.smoothedVelocity.magnitude / this.maxVelocity);
            float forceMagnitude = this.throwForceCurve.Evaluate(normalizedSpeed);
            this.ThrowForce = this.MovementDirection * forceMagnitude;
        }

        private void DecaySpeedMultiplier(float dt) =>
            this.currentSpeedMultiplier = Mathf.MoveTowards(this.currentSpeedMultiplier,
                this.minSpeedMultiplier,
                this.decelerationRate * dt);

        private void AnimateZPosition(float dt)
        {
            if (!this.isAnimatingZ)
                return;

            Vector3 pos = this.transform.position;
            pos.z = Mathf.Lerp(pos.z, this.targetZPosition, this.zLerpSpeed * dt);
            this.transform.position = pos;

            RecalculateBounds();

            if (!(Mathf.Abs(pos.z - this.targetZPosition) < 0.001f)) return;
            pos.z = this.targetZPosition;
            this.transform.position = pos;
            this.isAnimatingZ = false;
        }

        private void RecalculateBounds()
        {
            if (!this.camRef)
                return;

            float z = Mathf.Abs(this.camRef.transform.position.z - this.transform.position.z);
            Vector3 bottomLeft = this.camRef.ScreenToWorldPoint(new Vector3(0, 0, z));
            Vector3 topRight = this.camRef.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, z));

            Vector3 size = topRight - bottomLeft;
            Vector3 center = bottomLeft + size * 0.5f;

            this.movementBounds = new Bounds(center, size);
        }

        private void RecalculatePixelDensityScale() => 
            this.pixelDensityScale = (float)Screen.height / this.referenceScreenHeight;

        private float GetDepthScale()
        {
            if (!this.camRef)
                return 1f;

            float cameraZ = this.camRef.transform.position.z;
            float currentDist = Mathf.Abs(this.transform.position.z - cameraZ);
            float originalDist = Mathf.Abs(this.originalZPosition - cameraZ);

            return originalDist > 0.001f ? currentDist / originalDist : 1f;
        }

        private Vector3 ClampToBounds(Vector3 position) => 
            this.movementBounds.Contains(position) ? position : this.movementBounds.ClosestPoint(position);

        /// <summary>
        /// clamps deltaTime between a max and min frameRate
        /// </summary>
        private static float GetSafeDeltaTime() => Mathf.Clamp(Time.deltaTime, 1f / 240f, 1f / 15f);

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(this.movementBounds.center, this.movementBounds.size);
        }
    }
}
