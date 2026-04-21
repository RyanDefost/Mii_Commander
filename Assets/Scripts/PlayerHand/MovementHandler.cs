using System;
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
            UpdateMouseBounds(0);
            AspectRatioTracker.OnAspectRatioChanged += UpdateMouseBounds;
        }

        private void UpdateMouseBounds(float _) => this.mouseBounds = GenerateBounds(this.camRef, this.mouseBounds.center);

        public void HandleDeltaOffset(Vector2 origin, Vector2 delta)
        {
            this.currentBoost = delta.sqrMagnitude < 0.001f
                ? 1f
                : Mathf.MoveTowards(this.currentBoost,
                    this.maxSensitivity,
                    this.accelerationRate * Time.deltaTime);
            
            Vector2 boostedDelta = delta * this.currentBoost * this.baseSensitivity;
            SetMousePosition(origin + boostedDelta * Time.deltaTime);
        }

        private void SetMousePosition(Vector2 newPosition)
        {
            this.transform.position = this.transform.position.OverwriteXY(newPosition);
            
            // limit hand position
            if (!this.mouseBounds.Contains(this.transform.position))
                this.transform.position = this.mouseBounds.ClosestPoint(this.transform.position);
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