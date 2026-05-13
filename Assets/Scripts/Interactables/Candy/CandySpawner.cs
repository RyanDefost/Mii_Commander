using System;
using System.Collections.Generic;
using PlayerHand;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Spawns candy within the player hand, using a pool of grouped candy.
/// </summary>
public class CandySpawner : MonoBehaviour
{
    [SerializeField]
    private PlayerHandManager playerHandManager;
    [SerializeField]
    private GameObject candyPrefab;
    private ObjectPool<CandyGroupInstance> candyPool;
    
    private void OnValidate()
    {
        if (Application.isPlaying)
            return;
        this.enabled = this.candyPrefab && this.playerHandManager;
    }

    private void Awake() => this.candyPool = new ObjectPool<CandyGroupInstance>();

    /// <summary>Gets a randomized hand full of candy and adds it to the hand</summary>
    public void AddToHand()
    {
        CandyGroupInstance result = this.candyPool.RequestObject();

        if (result == null)
        {
            result = new CandyGroupInstance(this.playerHandManager,
                this.candyPrefab,
                this.playerHandManager.transform,
                5);
            this.candyPool.ActivateObject(result);
            return;
        }

        result.SetParent(this.playerHandManager.transform);
    }
}