using Grid;
using Managers;
using UnityEngine;

public class TempNeighborTester : BoardItemComponent
{
    private GridManager gridManager;
    private GameManager gameManagerRef;

    public override void ConnectToBoardItem() => this.boardItem.OnActivate += Activate;
    private void OnDestroy() => this.boardItem.OnActivate -= Activate;

    private void Activate()
    {
        this.gridManager ??= ComponentRegistry.GetComponent<GridManager>();
        if (!this.gridManager) return;

        GridManager.GridInstance[] foundNeighbors = this.gridManager.GetNeighbors(this.boardItem.gridInstanceRef, NeighborPattern.Down);
        Debug.Log(foundNeighbors.Length);
        foreach (GridManager.GridInstance neighbor in foundNeighbors)
        {
            if (neighbor == null || !neighbor.gameObj) continue;

            this.gridManager.ReleaseInstance(neighbor);
            Destroy(neighbor.gameObj);
        }
        
        this.gameManagerRef ??= ComponentRegistry.GetComponent<GameManager>();
        if (!this.gameManagerRef) return;
        this.gameManagerRef.TurnManager.AddToWaitTime(0.2f);
    }
}