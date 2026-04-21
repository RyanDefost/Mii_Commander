using System;
using UnityEngine;

namespace PlayerHand
{
    public class InteractionHandler : MonoBehaviour
    {
        [SerializeField]
        private Camera camRef;
        [SerializeField]
        private MovementHandler movementHandler;
        [SerializeField]
        private VisualHandler visualHandler;
        [SerializeField]
        private LayerMask layerMaskInteractables;
        private Interactable currentHover;
        
        public Interactable CurrentHover { 
            get => this.currentHover;
            private set => this.currentHover = value; 
        }

        private void OnValidate()
        {
            this.camRef = Camera.main;
            this.movementHandler = GetComponent<MovementHandler>();
            this.visualHandler = GetComponent<VisualHandler>();
            this.enabled = this.movementHandler && this.visualHandler;
        }

        private void Update()
        {
            if (this.visualHandler.CurrentState == "Closed")
                return;
            
            RaycastHit2D[] results = new RaycastHit2D[5];
            int size = Physics2D.RaycastNonAlloc(this.transform.position, Vector3.forward, results, 20f, this.layerMaskInteractables);
            if (size <= 0)
            {
                OnNoInteractable();
                return;
            }

            RaycastHit2D? found = GetClosestToCamera(results);
            if (found == null)
            {
                OnNoInteractable();
                return;
            }

            Interactable interactable = found.Value.collider.GetComponent<Interactable>();
            if (!interactable) return;
            this.visualHandler.SetSprite("Open", this.movementHandler);
            this.CurrentHover = interactable;
        }

        private RaycastHit2D? GetClosestToCamera(RaycastHit2D[] hits)
        {
            RaycastHit2D? found = null;
            float smallestDistance = float.PositiveInfinity;
            foreach (RaycastHit2D hit in hits)
            {
                if (!hit.collider) continue;
                
                float currentDistance = Vector3.Distance(
                    new Vector3(hit.transform.position.x, hit.transform.position.y, this.camRef.transform.position.z), 
                    hit.transform.position);
                
                SpriteRenderer visual =  hit.collider.GetComponent<SpriteRenderer>();
                if (visual)
                    currentDistance += visual.sortingOrder;
                if (!(currentDistance < smallestDistance)) continue;
                found = hit;
                smallestDistance = currentDistance;
            }

            return found;
        }

        private void OnNoInteractable()
        {
            this.CurrentHover = null;
            if (this.visualHandler.CurrentState != "Closed")
                this.visualHandler.SetSprite("Pointing", this.movementHandler);
        }
    }
}