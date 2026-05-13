using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerHand
{
    /// <summary>
    /// Keeps track of mouse input and the hand state
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer), typeof(VisualHandler), typeof(MovementHandler)), 
     RequireComponent(typeof(InteractionHandler))]
    public class PlayerHandManager : MonoBehaviour
    {
        [SerializeField]
        private VisualHandler visualHandler;
        [SerializeField]
        private MovementHandler movementHandler;
        [SerializeField]
        private InteractionHandler interactionHandler;
        [Space]
        [SerializeField]
        private InputActionAsset inputActionAsset;
        private InputActionMap inputActionMap;
        private InputAction moveAction;
        private InputAction grabAction;

        private bool CanGrab;
        private bool grabbing;
        public Action<Vector2, Vector2> OnGrabReleased; // passes hand movementDir, and throw force

        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
            this.visualHandler = GetComponent<VisualHandler>();
            this.movementHandler = GetComponent<MovementHandler>();
            this.interactionHandler = GetComponent<InteractionHandler>();
            this.enabled = this.visualHandler.enabled && this.movementHandler.enabled && this.interactionHandler.enabled && this.inputActionAsset;
        }

        private void Awake()
        {
            this.CanGrab = true;
            
            this.inputActionMap = this.inputActionAsset.FindActionMap("Player");
            this.inputActionMap.Enable();
            this.moveAction = this.inputActionMap.FindAction("Move");
            this.grabAction = this.inputActionMap.FindAction("Grab");

            this.grabAction.performed += OnGrabActionOnPerformed;
        
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            ComponentRegistry.AddToRegistry(this);
        }

        private void OnGrabActionOnPerformed(InputAction.CallbackContext obj) => OnInteract(obj, this.interactionHandler);

        private void OnDestroy()
        {
            ComponentRegistry.RemoveFromRegistry(this);
            this.grabAction.performed -= OnGrabActionOnPerformed;
        }

        private void Update()
        {
            if (this.moveAction != null) 
                this.movementHandler.ApplyMouseDelta(this.moveAction.ReadValue<Vector2>());
            
            if (this.grabAction == null || !this.grabbing) return;
            if (CheckOnGrabReleased(ref this.grabbing, this.grabAction, this.visualHandler, this.movementHandler))
                this.OnGrabReleased.Invoke(this.movementHandler.MovementDirection,  this.movementHandler.ThrowForce);
        }
            

        private void OnInteract(InputAction.CallbackContext obj, InteractionHandler interactionHandler)
        {
            if(!this.CanGrab) return;
            
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
        
        public void SetCanGrab(bool canGrab) =>  this.CanGrab = canGrab;
    }
}
