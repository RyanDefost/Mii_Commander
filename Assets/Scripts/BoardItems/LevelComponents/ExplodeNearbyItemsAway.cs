using Grid;
using UnityEngine;

public class ExplodeNearbyItemsAway : BoardItemComponent
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private NeighborPatternRegistry patternRegistry;
    [SerializeField] private string explosionPatternName;

    public override void ConnectToBoardItem() => this.boardItem.OnActivate += Activate;
    private void OnDestroy() => this.boardItem.OnActivate -= Activate;

    private void Activate()
    {
        this.gridManager ??= ComponentRegistry.GetComponent<GridManager>();
        if (!this.gridManager) return;
        
        this.patternRegistry = ComponentRegistry.GetComponent<NeighborPatternRegistry>();
        if (!this.patternRegistry) return;
        
        NeighborPattern? explosionPattern = this.patternRegistry.GetNeighborPattern(this.explosionPatternName);
        if (explosionPattern == null) return;

        GridManager.GridInstance[] foundNeighbors = this.gridManager.GetNeighbors(this.boardItem.gridInstanceRef, explosionPattern.Value);
        foreach (GridManager.GridInstance neighbor in foundNeighbors)
        {
            if (neighbor == null)
                continue;

            Debug.Log($"neighbor {neighbor.index}, {neighbor.gameObj.name}");
            this.gridManager.ReleaseInstance(neighbor);
            Destroy(neighbor.gameObj);
        }
    }
}