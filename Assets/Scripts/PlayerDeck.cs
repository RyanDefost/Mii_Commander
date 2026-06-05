using System;
using System.Collections.Generic;
using System.Linq;
using BoardItems;
using PlayerHand;
using UnityEngine;

/// <summary>
/// Spawns candy within the player hand, using a pool of grouped candy.
/// </summary>
public class PlayerDeck : MonoBehaviour
{
    [SerializeField]
    private PlayerHandManager playerHandManager;
    private ObjectPool<CandyGroupHandle> candyPool;
    [SerializeField] 
    private List<DeckOption> options = new();
    private int currentOption = 0;

    /// <summary>
    /// Contains a list of possible candies, handles the distribution of random candy from deck
    /// </summary>
    [Serializable]
    public class DeckOption
    {
        [SerializeField]
        private string name;
        [SerializeField]
        private CandyOption[] possibleCandies;

        /// <summary>
        /// Weighted candy lookup reference
        /// </summary>
        [Serializable]
        private class CandyOption
        {
            [Range(0, 100)]
            public float chance;
            public int typeIndex;
        }

        /// <summary>
        /// Gets a candy from the weighted candylist
        /// </summary>
        /// <param name="prefab">prefab to instantiate</param>
        /// <param name="typeIndex">reference to the candy lookup table</param>
        public void GetCandy(out GameObject prefab, out int typeIndex)
        {
            typeIndex = -1;
            prefab = null;

            CandyTypeLookup lookupRef = ComponentRegistry.GetComponent<CandyTypeLookup>();
            if (!lookupRef)
                return;
            
            float totalWeight = this.possibleCandies.Sum(option => option.chance);
            
            float randomValue = UnityEngine.Random.Range(0, totalWeight);
            float processedWeight = 0;

            foreach (CandyOption option in this.possibleCandies)
            {
                processedWeight += option.chance;
                if (!(randomValue <= processedWeight)) continue;
                
                typeIndex = option.typeIndex;
                prefab = lookupRef.Get(typeIndex).candyPrefab; 
                return;
            }
        }
    }
    
    private void OnValidate()
    {
        if (Application.isPlaying)
            return;
        this.enabled = this.playerHandManager;
    }

    private void Awake() => this.candyPool = new ObjectPool<CandyGroupHandle>();

    /// <summary>Gets a randomized hand full of candy and adds it to the hand</summary>
    public void AddToHand()
    {
        CandyGroupHandle result = this.candyPool.RequestObject();

        if (result == null)
        {
            result = new CandyGroupHandle(this.playerHandManager,
                this.options[this.currentOption],
                this.playerHandManager.transform,
                5);
            this.candyPool.ActivateObject(result);
            return;
        }

        result.SetParent(this.playerHandManager.transform);
    }
}