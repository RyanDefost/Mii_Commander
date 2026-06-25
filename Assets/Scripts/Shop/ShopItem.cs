using UnityEngine;

namespace Shop
{
    [CreateAssetMenu(fileName = "Shop", menuName = "ScriptableObjects/ShopItem", order = 1)]
    public class ShopItem : ScriptableObject
    {
        [Header("Shop Item")]
        public GameObject shopObject;
        public int cost = 10;
        
        [Header("Visual")]
        public Sprite itemSprite;
        public string itemName;
        [TextArea]
        public string itemHoverText;

        public string GetHoverText()
        {
            return $"[COST:{this.cost}] \n{this.itemHoverText}";
        }
    }
}