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
        
        private bool grabbing = false;
        public Action OnGrabReleased;

        private void OnValidate()
        {
            this.visualHandler = GetComponent<VisualHandler>();
            this.movementHandler = GetComponent<MovementHandler>();
            this.interactionHandler = GetComponent<InteractionHandler>();
            this.enabled = this.visualHandler.enabled && this.movementHandler.enabled && this.interactionHandler.enabled && this.inputActionAsset;
        }

        private void Awake()
        {
            this.inputActionMap = this.inputActionAsset.FindActionMap("Player");
            this.inputActionMap.Enable();
            this.moveAction = this.inputActionMap.FindAction("Move");
            this.grabAction = this.inputActionMap.FindAction("Grab");

            this.moveAction.performed += (obj) => OnHandMove(obj, this.transform, this.movementHandler);
            this.grabAction.performed += (obj) => OnInteract(obj, this.interactionHandler);
        
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            if (this.grabAction == null || !this.grabbing) return;
            if (CheckOnGrabReleased(ref this.grabbing, this.grabAction, this.visualHandler, this.movementHandler))
                this.OnGrabReleased.Invoke();
        }

        private static void OnHandMove(InputAction.CallbackContext obj, Transform transform,
            MovementHandler movementHandler) =>
            movementHandler.HandleDeltaOffset(transform.position.XY(), obj.ReadValue<Vector2>());

        private static void OnInteract(InputAction.CallbackContext obj, InteractionHandler interactionHandler)
        {
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
    }
}
