using System;
using System.Linq;
using UnityEngine;

namespace PlayerHand
{
    /// <summary>
    /// Used for setting and keeping track of the player hand visuals
    /// </summary>
    public class VisualHandler : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer visual;
        [SerializeField]
        private VisualState[] sprites;
        [SerializeField] 
        private float zMovementSpeed;

        [Serializable]
        private struct VisualState
        {
            public string key;
            public Sprite visual;
            public float z;
        }

        public string CurrentState { get; private set; }

        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
            
            this.visual = GetComponent<SpriteRenderer>();
            
            bool hasEmptySprites = false;
            foreach (VisualState sprite in this.sprites)
            {
                if (sprite.visual) continue;
                hasEmptySprites = true;
                break;
            }
            
            this.enabled = this.visual && !hasEmptySprites && this.sprites.Length > 0;
        }

        /// <param name="key">Name of the desired visual state</param>
        /// <param name="movementHandler">Requires movement handle, to update Z position</param>
        public void SetSprite(string key, MovementHandler movementHandler)
        {
            VisualState state = this.sprites.First(a => a.key == key);
            this.visual.sprite = state.visual;
            movementHandler.SetTargetZ(state.z, this.zMovementSpeed);
            this.CurrentState = key;
        }
    }
}