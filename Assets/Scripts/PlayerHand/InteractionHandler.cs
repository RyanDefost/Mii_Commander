using UnityEngine;

namespace PlayerHand
{
    /// <summary>
    /// Lets the player hand interact with interactables
    /// </summary>
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
            if (Application.isPlaying)
                return;
            this.camRef = Camera.main;
            this.movementHandler = GetComponent<MovementHandler>();
            this.visualHandler = GetComponent<VisualHandler>();
            this.enabled = this.movementHandler && this.visualHandler;
        }

        private void Update()
        {
            if (IsHoldingInteractable())
                return;
            
            // Cast ray underneath hand
            RaycastHit2D[] results = new RaycastHit2D[5];
            int size = Physics2D.RaycastNonAlloc(this.transform.position, Vector3.forward, results, 20f, this.layerMaskInteractables);
            if (size <= 0)
            {
                ResetHover();
                return;
            }

            // Get closest interactable
            RaycastHit2D? found = GetClosestInteractableToCamera(results);
            if (found == null)
            {
                ResetHover();
                return;
            }

            // Log most likely to interact with
            Interactable interactable = found.Value.collider.GetComponent<Interactable>();
            if (!interactable) return;
            this.visualHandler.SetSprite("Open", this.movementHandler);
            this.CurrentHover = interactable;
        }

        /// <param name="hits">a list of raycast hits, where all hits are with a interactable object</param>
        /// <returns>Closest interactable, or null</returns>
        private RaycastHit2D? GetClosestInteractableToCamera(RaycastHit2D[] hits)
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

        private void ResetHover()
        {
            this.CurrentHover = null;
            if (!IsHoldingInteractable())
                this.visualHandler.SetSprite("Pointing", this.movementHandler);
        }

        public bool IsHoldingInteractable() => this.visualHandler.CurrentState == "Closed";
    }
}