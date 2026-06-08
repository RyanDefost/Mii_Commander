using UnityEngine;

namespace BoardItems
{
    /// <summary>
    /// A component that's able to hook into the functions of a board item
    /// </summary>
    public class BoardItemComponent : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        protected BoardItem boardItem;

        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
            this.boardItem = GetComponent<BoardItem>();
            CustomOnValidate();
        }

        /// <summary>OnValidate so that BoardItemComponent's OnValidate doesnt get overwritten</summary>
        protected virtual void CustomOnValidate() {}

        /// <summary>Function for hooking into the various boardItem actions, make sure to unhook on destroy</summary>
        public virtual void ConnectToBoardItem() { }
    
        public BoardItem GetBoardItem() => this.boardItem;
        public virtual string GetAbilityName() => null;
    }
}