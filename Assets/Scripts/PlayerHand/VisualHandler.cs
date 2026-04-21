using System;
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
        private Sprite[] sprites;

        private void OnValidate()
        {
            this.visual = GetComponent<SpriteRenderer>();
            
            bool hasEmptySprites = false;
            foreach (Sprite sprite in this.sprites)
            {
                if (sprite) continue;
                hasEmptySprites = true;
                break;
            }
            
            this.enabled = this.visual && !hasEmptySprites && this.sprites.Length > 0;
        }

        public void SetSprite(string key)
        {
            this.visual.sprite = key switch
            {
                "Pointing" => this.sprites[0],
                "Open" => this.sprites[1],
                "Closed" => this.sprites[2],
                _ => this.visual.sprite
            };
        }
    }
}