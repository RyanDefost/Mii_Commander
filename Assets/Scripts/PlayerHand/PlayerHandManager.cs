using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerHand
{
    /// <summary>
    /// Manages all playerHand handlers, and keeps track of mouse input and the hand state
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer), typeof(VisualHandler), typeof(MovementHandler)), 
     RequireComponent(typeof(InteractionHandler), typeof(ItemRotationHandler))]
    public class PlayerHandManager : MonoBehaviour
    {
        [SerializeField]
        private float gamePadExtraSensitivity = 20;
        [SerializeField]
        private VisualHandler visualHandler;
        [SerializeField]
        private MovementHandler movementHandler;
        [SerializeField]
        private InteractionHandler interactionHandler;
        [SerializeField]
        private ItemRotationHandler itemRotationHandler;
        [Space]
        [SerializeField]
        private InputActionAsset inputActionAsset;
        private InputActionMap inputActionMap;
        private InputAction moveAction;
        private InputAction grabAction;
        
        private bool canGrab;
        private bool grabbing;
        public Action<Vector2, Vector2> OnRemovedFromHand; // passes hand movementDir, and throw force

        private DeviceTracker deviceTracker;

        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
            this.visualHandler = GetComponent<VisualHandler>();
            this.movementHandler = GetComponent<MovementHandler>();
            this.interactionHandler = GetComponent<InteractionHandler>();
            this.itemRotationHandler = GetComponent<ItemRotationHandler>();
            this.enabled = this.visualHandler.enabled && this.movementHandler.enabled && this.interactionHandler.enabled && this.inputActionAsset;
        }

        private void Awake()
        {
            ComponentRegistry.AddToRegistry(this);
            
            this.canGrab = true;
            
            this.inputActionMap = this.inputActionAsset.FindActionMap("Player");
            this.inputActionMap.Enable();
            this.moveAction = this.inputActionMap.FindAction("Move");
            this.grabAction = this.inputActionMap.FindAction("Grab");
            this.itemRotationHandler.rotateAction = this.inputActionMap.FindAction("Rotate");

            this.grabAction.performed += OnGrabActionPerformed;
        
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnDestroy()
        {
            ComponentRegistry.RemoveFromRegistry(this);
            this.grabAction.performed -= OnGrabActionPerformed;
        }

        private void Update()
        {
            if (this.moveAction != null)
            {
                this.deviceTracker ??= ComponentRegistry.GetComponent<DeviceTracker>();
                Vector2 delta = this.moveAction.ReadValue<Vector2>();
                if (this.deviceTracker && this.deviceTracker.LastDevice is Gamepad)
                    delta *= this.gamePadExtraSensitivity;
                this.movementHandler.ApplyMouseDelta(delta);
            }
            
            if (this.grabAction == null || !this.grabbing) return;
            if (CheckOnGrabReleased(ref this.grabbing, this.grabAction, this.visualHandler, this.movementHandler))
                this.OnRemovedFromHand.Invoke(this.movementHandler.MovementDirection,  this.movementHandler.ThrowForce);
        }
        
        private void OnGrabActionPerformed(InputAction.CallbackContext obj) => OnInteract(obj, this.interactionHandler);

        private void OnInteract(InputAction.CallbackContext obj, InteractionHandler interactionHandler)
        {
            if(!this.canGrab) return;
            if (!obj.ReadValueAsButton()) return;
            if (!interactionHandler.CurrentHover) return;
            interactionHandler.CurrentHover.Trigger();
        }
    
        /// <summary>
        /// Checks if grab was released
        /// </summary>
        /// <returns>true when grab was released</returns>
        private static bool CheckOnGrabReleased(ref bool grabbing, InputAction grabAction, VisualHandler visualHandler,
            MovementHandler movementHandler)
        {
            if (grabAction.ReadValue<float>() > 0) return false;
            visualHandler.SetSprite("Pointing", movementHandler);
            grabbing = false;
            return true;
        }
        
        public void SetStateToGrabbing()
        {
            this.visualHandler.SetSprite("Closed", this.movementHandler);
            this.grabbing = true;
        }
        
        public void SetCanGrab(bool canGrab) =>  this.canGrab = canGrab;
    }
}
