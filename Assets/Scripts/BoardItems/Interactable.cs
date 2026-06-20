using UnityEngine;
using UnityEngine.Events;

namespace BoardItems
{
    /// <summary>
    /// A simple class for hooking into interactions, player pickup etc
    /// </summary>
    public class Interactable : MonoBehaviour
    {
        public BoardItem boardItem;
        public bool interactionOverride;
        [SerializeField]
        private UnityEvent interaction;

        [SerializeField] 
        private UnityEvent hovering;

        public void Trigger(bool canInteract)
        {
            if(!canInteract && !this.interactionOverride) return;
            this.interaction?.Invoke();  
        }

        public void OnHover() => this.hovering?.Invoke();

        public bool HasBoardItemParent() => this.boardItem;
    }
}
