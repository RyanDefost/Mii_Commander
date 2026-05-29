using System;
using System.Collections.Generic;
using HelperStructs.Shop;
using Managers;
using PlayerHand;
using Scoring;
using TMPro;
using UnityEngine;

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
    
    private GameManager gameManager;
    private ScoreManager scoreManager;

    private bool isBuyable = false;

    private GameObject currentGrabbable;
    
    private void Start()
    {
        this.gameManager = ComponentRegistry.GetComponent<GameManager>();
        this.scoreManager = gameManager.ScoreManager;
        
        this.scoreManager.OnChangeScore += UpdateBuyableState;
        
        this.currentItemAmount = this.itemAmount;
        
        UpdateVisuals();
        UpdateBuyableState();
    }
    
    private void OnDestroy() => this.scoreManager.OnChangeScore -= UpdateBuyableState;

    public void Interact()
    {
        if(!this.isBuyable) return;
        
        currentGrabbable = Instantiate(this.shopItem.shopObject, this.transform.position, this.transform.rotation);

        if (currentGrabbable.TryGetComponent(out BoardItem boardItem))
        {
            boardItem.OnStarted += GrabBoardItem;
        }
    }

    private void GrabBoardItem(BoardItem startedBoardItem)
    {
        if (currentGrabbable.TryGetComponent(out HandHandler handHandler))
        {
            handHandler.AddToHand();
            
            this.scoreManager.RemoveScore(this.shopItem.cost);
            this.currentItemAmount--;
            
            UpdateBuyableState();
        }
        
    }
    
    public void UpdateVisuals()
    {
        itemSpriteRenderer.sprite = shopItem.itemSprite;
        itemNameRenderer.text = shopItem.itemName;
    }
    
    private void UpdateBuyableState()
    {
        this.isBuyable = this.scoreManager.GetScore() >= this.shopItem.cost;
        this.grayOutSpriteRenderer.enabled = !this.isBuyable;
    }
}
