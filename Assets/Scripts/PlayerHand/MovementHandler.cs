using System;
using Unity.Mathematics;
using UnityEngine;

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
        
        private float currentBoost = 1f;
        private float originalZPosition;
        
        private float zMovementSpeed;
        private float desiredZPosition;
        private bool animatingZ;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(this.mouseBounds.center, this.mouseBounds.size);
        }

        private void OnValidate()
        {
            this.camRef = Camera.main;
            this.enabled = this.camRef;
        }

        private void Awake()
        {
            this.originalZPosition = this.transform.position.z;
            UpdateMouseBounds(0);
            AspectRatioTracker.OnAspectRatioChanged += UpdateMouseBounds;
        }

        private void UpdateMouseBounds(float _) => this.mouseBounds = GenerateBounds(this.camRef, this.transform.position);

        public void HandleDeltaOffset(Vector2 origin, Vector2 delta)
        {
            float cameraZ = this.camRef.transform.position.z;
            float currentDist = Mathf.Abs(this.transform.position.z - cameraZ);
            float originalDist = Mathf.Abs(this.originalZPosition - cameraZ);

            float depthScale = currentDist / originalDist;
            float dynamicMax = this.maxSensitivity * depthScale;
            
            this.currentBoost = delta.sqrMagnitude < 0.001f
                ? 1f
                : Mathf.MoveTowards(this.currentBoost,
                    dynamicMax,
                    this.accelerationRate * Time.deltaTime);
            
            Vector2 boostedDelta = delta * this.currentBoost * this.baseSensitivity * depthScale;
            SetMousePosition(origin + boostedDelta * Time.deltaTime);
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