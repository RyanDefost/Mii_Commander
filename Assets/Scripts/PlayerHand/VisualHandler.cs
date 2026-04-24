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

        private string currentState;
        public string CurrentState { get => this.currentState; private set => this.currentState = value; }

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

        public void SetSprite(string key, MovementHandler movementHandler)
        {
            VisualState state = this.sprites.First(a => a.key == key);
            this.visual.sprite = state.visual;
            movementHandler.SetPositionZ(state.z, this.zMovementSpeed);
            this.CurrentState = key;
        }
    }
}