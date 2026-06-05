using System;
using BoardItems;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerHand
{
    [RequireComponent(typeof(InteractionHandler))]
    public class ItemRotationHandler : MonoBehaviour
    {
        [SerializeField]
        private InteractionHandler interactionHandler;
        [SerializeField]
        private float gamePadExtraSpeed = 2f;
        [NonSerialized]
        public InputAction rotateAction;
        private Vector2 lastStickDirection = Vector2.zero;

        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
            this.interactionHandler = GetComponent<InteractionHandler>();
        }

        private void Update()
        {
            if (this.rotateAction == null)
                return;

            Interactable currentItem = this.interactionHandler.CurrentHover;
            if (!currentItem || !this.interactionHandler.IsHoldingInteractable() || !currentItem.HasBoardItemParent())
                return;
            
            GridRotatable rotatable = currentItem.boardItem.GetBoardComponent<GridRotatable>();
            if (!rotatable)
                return;
            
            object value = this.rotateAction.ReadValueAsObject();
            switch (value)
            {
                case null:
                    this.lastStickDirection = Vector2.zero;
                    break;
                case Vector2 stickValue:
                {
                    Vector2 currentDir = stickValue.normalized;

                    if (stickValue.sqrMagnitude > 0.1f && this.lastStickDirection.sqrMagnitude > 0.1f)
                    {
                        float angleDelta = Vector2.SignedAngle(this.lastStickDirection, currentDir);
                        rotatable.RotateClockWise(-Mathf.Sign(angleDelta) * this.gamePadExtraSpeed);
                    }
                    
                    this.lastStickDirection = currentDir;
                    break;
                }
                case float buttonValue:
                    rotatable.RotateClockWise(buttonValue);
                    break;
            }
        }
    }
}