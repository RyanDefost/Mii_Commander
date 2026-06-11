using System;
using BoardItems;
using Managers;
using Scoring;
using TMPro;
using UnityEngine;

namespace Shop
{
    public class ShopComponent : MonoBehaviour
    {
        [SerializeField] private ShopItem shopItem;
        [SerializeField] private int itemAmount;
        private int currentItemAmount;
    
        [Space]
        [SerializeField] private SpriteRenderer itemSpriteRenderer;
        [SerializeField] private TextMeshPro itemNameRenderer;
        [SerializeField] private TextMeshPro amountRenderer;
        [SerializeField] private SpriteRenderer grayOutSpriteRenderer;
        [Space]
        [SerializeField] private GameObject hoveringTextBox;
        private Timer visabilityTimer;
        
        private GameManager gameManager;
        private ScoreManager scoreManager;

        private bool isBuyable = false;

        private GameObject currentGrabbable;
    
        private void Start()
        {
            this.gameManager = ComponentRegistry.GetComponent<GameManager>();
            this.scoreManager = this.gameManager.ScoreManager;
        
            this.scoreManager.OnChangeScore += UpdateBuyableState;
        
            this.currentItemAmount = this.itemAmount;
        
            UpdateVisuals();
            UpdateBuyableState();
        }
    
        private void OnDestroy() => this.scoreManager.OnChangeScore -= UpdateBuyableState;

        public void Interact()
        {
            if(!this.isBuyable) return;

            this.currentGrabbable = Instantiate(this.shopItem.shopObject, this.transform.position, this.transform.rotation);

            if (this.currentGrabbable.TryGetComponent(out BoardItem boardItem))
            {
                boardItem.OnStarted += GrabBoardItem;
            }
        }

        private void Update()
        {
            this.visabilityTimer?.UpdateTime(Time.deltaTime);
        }

        public void Hovering()
        {
            if (this.visabilityTimer == null)
            {
                this.visabilityTimer = new Timer(0.5f, false, true, removeVisual);
                this.hoveringTextBox?.SetActive(true);
            }
            
            this.visabilityTimer.ResetAndReplay();
        }

        private void removeVisual()
        {
            this.visabilityTimer = null;
            this.hoveringTextBox?.SetActive(false);
        }

        private void GrabBoardItem(BoardItem startedBoardItem)
        {
            if (this.currentGrabbable.TryGetComponent(out HandHandler handHandler))
            {
                handHandler.AddToHand();
            
                this.scoreManager.RemoveScore(this.shopItem.cost);
                this.currentItemAmount--;
            
                UpdateBuyableState();
            }
        
        }
    
        public void UpdateVisuals()
        {
            this.itemSpriteRenderer.sprite = this.shopItem.itemSprite;
            this.itemNameRenderer.text = this.shopItem.itemName;
        }
    
        private void UpdateBuyableState(int _ = -1)
        {
            this.isBuyable = this.scoreManager.GetScore() >= this.shopItem.cost;
            this.grayOutSpriteRenderer.enabled = !this.isBuyable;
        }
    }
}
