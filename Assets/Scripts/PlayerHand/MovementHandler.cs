using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

namespace PlayerHand
{
    /// <summary>
    /// Handles and limits player hand movement
    /// </summary>
    public class MovementHandler : MonoBehaviour
    {
        [Header("Required components")]
        [SerializeField]
        private Camera camRef;
        [SerializeField]
        private Bounds mouseBounds;
        
        [Header("Velocity Settings")]
        [SerializeField] private float baseSensitivity = 1f;
        [SerializeField] private float maxSensitivity = 3f;
        [SerializeField] private float accelerationRate = 2f;
        [Space]
        [SerializeField] private int referenceWindowHeight = 1080;
        
        private float liveBaseSensitivity;
        private float liveMaxSensitivity;
        private float liveAccelerationRate;
        private float currentSensitivity;
        
        private float currentBoost = 1f;
        private float originalZPosition;
        
        private float zMovementSpeed;
        private Vector3 desiredPos;
        private float desiredZPosition;
        private bool animatingZ;

        private Vector3 lastPosition;
        public Vector2 MovementDirection { get; private set;  }
        public Vector2 Velocity { get; private set;  }
        public Vector2 ThrowForce { get; private set;  }
        
        private Vector3 desiredVelocity;
        [SerializeField]
        private float maxVelocity = 50f;
        [SerializeField]
        private AnimationCurve throwForceRange;

        [SerializeField] private Text tempText;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(this.mouseBounds.center, this.mouseBounds.size);
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
            this.camRef = Camera.main;
            this.enabled = this.camRef;
        }

        private void Awake()
        {
            this.originalZPosition = this.transform.position.z;
            UpdateMouseBounds();
            UpdateLiveSensitivityVars();
            ResolutionTracker.OnResolutionChanged += UpdateMouseBounds;
            ResolutionTracker.OnResolutionChanged += UpdateLiveSensitivityVars;
        }
        
        private void OnDestroy()
        {
            ResolutionTracker.OnResolutionChanged -= UpdateMouseBounds;
            ResolutionTracker.OnResolutionChanged -= UpdateLiveSensitivityVars;
        }

        private void UpdateMouseBounds(int _ = 1, int __ = 1) => this.mouseBounds = GenerateBounds(this.camRef, this.transform.position);
        
        private void UpdateLiveSensitivityVars(int _ = 1, int screenHeight = 1)
        {
            if (screenHeight == 1)
                screenHeight = this.referenceWindowHeight;
            float resolutionScale = (float)screenHeight / this.referenceWindowHeight;
            this.liveBaseSensitivity = this.baseSensitivity * resolutionScale;
            this.liveMaxSensitivity = this.maxSensitivity * resolutionScale;
            this.liveAccelerationRate = this.accelerationRate * resolutionScale;
        }

        public void HandleDeltaOffset(Vector2 origin, Vector2 delta)
        {
            float cameraZ = this.camRef.transform.position.z;
            float currentDist = Mathf.Abs(this.transform.position.z - cameraZ);
            float originalDist = Mathf.Abs(this.originalZPosition - cameraZ);

            float depthScale = currentDist / originalDist;
            float dynamicMax = this.liveMaxSensitivity * depthScale;
            
            this.currentBoost = delta.sqrMagnitude < 0.001f
                ? 1f
                : Mathf.MoveTowards(this.currentBoost,
                    dynamicMax,
                    this.liveAccelerationRate * Time.deltaTime);
            this.currentSensitivity = this.currentBoost * this.liveBaseSensitivity;
            
            Vector2 boostedDelta = delta * this.currentSensitivity * Time.deltaTime;
            SetMousePosition(origin + boostedDelta);
        }

        private void SetMousePosition(Vector2 newPosition)
        {
            this.transform.position = this.transform.position.OverwriteXY(newPosition);
            
            // limit hand position
            if (!this.mouseBounds.Contains(this.transform.position))
                this.transform.position = this.mouseBounds.ClosestPoint(this.transform.position);
        }
        
        public void SetPositionZ(float newZ, float movementSpeed)
        {
            this.desiredZPosition = newZ;
            this.zMovementSpeed = movementSpeed;
            this.animatingZ = true;
        }

        private void Update()
        {
            UpdateVelocity();
            AnimateZ();
                    
            this.lastPosition = this.transform.position;
        }

        private void UpdateVelocity()
        {
            Vector3 diff = this.transform.position - this.lastPosition;
            this.MovementDirection = diff.normalized;
            
            Vector3 newVelocity = diff.normalized * Mathf.Min(diff.magnitude / Time.deltaTime, this.maxVelocity);
            if (newVelocity.magnitude > this.Velocity.magnitude)
                this.Velocity = newVelocity;
            else
            {
                if (this.desiredVelocity.magnitude > this.Velocity.magnitude)
                    this.desiredVelocity = newVelocity;
                this.Velocity = Vector3.Lerp(this.Velocity, this.desiredVelocity, Time.deltaTime * this.accelerationRate);
            }

            this.tempText.text = this.Velocity.ToString();

            this.ThrowForce = this.MovementDirection * this.throwForceRange.Evaluate(this.Velocity.magnitude / this.maxVelocity);
            
            // lower current acceleration
            this.currentBoost -= this.accelerationRate * Time.deltaTime;
            if (this.currentBoost < this.baseSensitivity)
                this.currentBoost = this.baseSensitivity;
        }

        private void AnimateZ()
        {
            if (!this.animatingZ)
                return;
            
            Vector3 vector3 = this.transform.position;
            vector3.z = Mathf.Lerp(vector3.z, this.desiredZPosition, Time.deltaTime * this.zMovementSpeed);
            this.transform.position = vector3;
            
            UpdateMouseBounds(0);
            
            if (Mathf.Approximately(this.desiredZPosition, vector3.z))
                this.animatingZ = false;
        }

        private static Bounds GenerateBounds(Camera camRef, Vector3 origin)
        {
            float z = Mathf.Abs(camRef.transform.position.z - origin.z);

            Vector3 bottomLeft = camRef.ScreenToWorldPoint(new Vector3(0, 0, z));
            Vector3 topRight = camRef.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, z));

            Vector3 size = topRight - bottomLeft;
            Vector3 center = bottomLeft + size * 0.5f;

            return new Bounds(center, size);
        }
    }
}