using Grid;
using Managers;
using UnityEngine;

public class ExplodeNearbyItemsAway : BoardItemComponent
{
    private GridManager gridManager;
    private NeighborPatternRegistry patternRegistry;
    private GameManager gameManagerRef;
    [SerializeField] private string explosionPatternName;
    [SerializeField] private float explosionStrengthTile = 1f;
    [SerializeField] private float explosionStrengthVisual = 1f;
    [SerializeField] private float explosionTime = 1f;

    public override void ConnectToBoardItem() => this.boardItem.OnActivate += Activate;
    private void OnDestroy() => this.boardItem.OnActivate -= Activate;

    private void Activate()
    {
        this.gridManager ??= ComponentRegistry.GetComponent<GridManager>();
        if (!this.gridManager) return;
        
        this.patternRegistry ??= ComponentRegistry.GetComponent<NeighborPatternRegistry>();
        if (!this.patternRegistry) return;
        
        NeighborPattern? explosionPattern = this.patternRegistry.GetNeighborPattern(this.explosionPatternName);
        if (explosionPattern == null) return;

        GridManager.GridInstance[] foundNeighbors = this.gridManager.GetNeighbors(this.boardItem.gridInstanceRef, explosionPattern.Value);
        foreach (GridManager.GridInstance neighbor in foundNeighbors)
        {
            if (neighbor == null || !neighbor.gameObj) continue;

            GridMoveable moveComponent = neighbor.moveable;
            if (!moveComponent) continue;
                
            moveComponent.ResetTarget();
            Vector3 dirAway = this.boardItem.transform.position.XY().DirectionTo(neighbor.position.XY());
            GridManager.GridInstance newTarget = this.gridManager.GetNearestPosition(neighbor.position + dirAway * this.explosionStrengthTile, neighbor.gameObj,
                moveComponent.GetBoardItem(), neighbor);
            moveComponent.ApplyImpulse(dirAway * this.explosionStrengthVisual);
                
            if (newTarget != null)
                moveComponent.SetTarget(newTarget, false);
            moveComponent.SetMoving(true);
        }
        
        this.gameManagerRef ??= ComponentRegistry.GetComponent<GameManager>();
        if (!this.gameManagerRef) return;
        this.gameManagerRef.TurnManager.AddToWaitTime(this.explosionTime);
    }
}