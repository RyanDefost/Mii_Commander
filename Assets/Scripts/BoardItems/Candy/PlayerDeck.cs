using PlayerHand;
using UnityEngine;

/// <summary>
/// Spawns candy within the player hand, using a pool of grouped candy.
/// </summary>
public class PlayerDeck : MonoBehaviour
{
    [SerializeField]
    private PlayerHandManager playerHandManager;
    [SerializeField]
    private GameObject candyPrefab;
    private ObjectPool<CandyGroupHandle> candyPool;
    
    private void OnValidate()
    {
        if (Application.isPlaying)
            return;
        this.enabled = this.candyPrefab && this.playerHandManager;
    }

    private void Awake() => this.candyPool = new ObjectPool<CandyGroupHandle>();

    /// <summary>Gets a randomized hand full of candy and adds it to the hand</summary>
    public void AddToHand()
    {
        CandyGroupHandle result = this.candyPool.RequestObject();

        if (result == null)
        {
            result = new CandyGroupHandle(this.playerHandManager,
                this.candyPrefab,
                this.playerHandManager.transform,
                5);
            this.candyPool.ActivateObject(result);
            return;
        }

        result.SetParent(this.playerHandManager.transform);
    }
}