using System.Collections.Generic;
using System.Linq;
using Grid;
using Managers;
using UnityEngine;

namespace BoardItems
{
    public class AttractNearbyItems : BoardItemComponent, IBoardItemDisablable
    {
        private GridManager gridManager;
        private NeighborPatternRegistry patternRegistry;
        private GameManager gameManagerRef;
        [SerializeField] private string attractionPatternName;
        [SerializeField] private float attractionStrengthVisual = 1f;
        [SerializeField] private float attractionTime = 1f;
        [SerializeField] private bool singleUse;
        private bool shouldActivateOnItsOwn = true;
    
        public override void ConnectToBoardItem()
        {
            if (this.shouldActivateOnItsOwn)
                EnableActivate();
        }

        private void OnDestroy() => DisableActivate();

        public void Activate()
        {
            this.gridManager ??= ComponentRegistry.GetComponent<GridManager>();
            if (!this.gridManager) return;
        
            this.patternRegistry ??= ComponentRegistry.GetComponent<NeighborPatternRegistry>();
            if (!this.patternRegistry) return;
        
            NeighborPattern? attractionPattern = this.patternRegistry.GetNeighborPattern(this.attractionPatternName);
            if (attractionPattern == null) return;

            GridManager.GridInstance[] foundNeighbors = this.gridManager.GetNeighbors(this.boardItem.gridInstanceRef, attractionPattern.Value);
            int neighborsEffected = 0;
        
            // make a list of empty neighbors, these will be used as spots to move to
            List<Vector3> emptySpots = new();
            for (int i = 0; i < foundNeighbors.Length; i++)
            {
                GridManager.GridInstance neighbor = foundNeighbors[i];
                if (neighbor != null && neighbor.gameObj) continue;
                Vector3? cellPos = this.gridManager.TryGetTileFromPattern(i, this.boardItem.gridInstanceRef, attractionPattern.Value);
                if (cellPos != null)
                    emptySpots.Add(cellPos.Value);
            }

            foreach (GridManager.GridInstance neighbor in foundNeighbors)
            {
                if (neighbor == null || !neighbor.gameObj) continue;

                GridMoveable moveComponent = neighbor.moveable;
                if (!moveComponent) continue;
                neighborsEffected++;
            
                // if there are no empty spots stop the activation
                // get the closest position
                // if the closest position is further then I am, don't move
                // if it is closer, move towards that position
                if (emptySpots.Count == 0) return;
                Vector3 closest = emptySpots.OrderBy(v =>
                {
                    float distToAttractor = Vector2.Distance(v.XY(), this.boardItem.transform.position.XY());
                    float distToOrigin = Vector2.Distance(v.XY(), neighbor.position.XY());
                    return (distToAttractor + distToOrigin) / 2;
                }).FirstOrDefault();
                if (Vector2.Distance(closest.XY(), this.boardItem.transform.position.XY()) > Vector2.Distance(neighbor.position.XY(), this.boardItem.transform.position.XY())) continue;
                moveComponent.ResetTarget();
            
                GridManager.GridInstance newTarget = this.gridManager.GetNearestPosition(closest, neighbor.gameObj,
                    moveComponent.GetBoardItem(), neighbor);
                moveComponent.ApplyImpulse(neighbor.position.XY().DirectionTo(this.boardItem.transform.position.XY()) * this.attractionStrengthVisual);
                
                if (newTarget != null)
                    moveComponent.SetTarget(newTarget, false);
                moveComponent.SetMoving(true);
            }

            if (neighborsEffected > 1)
            {
                this.gameManagerRef ??= ComponentRegistry.GetComponent<GameManager>();
                if (!this.gameManagerRef) return;
                this.gameManagerRef.TurnManager.AddToWaitTime(this.attractionTime);
            }
        
            if (this.singleUse)
                DisableActivate();
        }

        public void DisableActivate()
        {
            this.shouldActivateOnItsOwn = false;
            this.boardItem.OnActivate -= Activate;
        }

        public void EnableActivate() => this.boardItem.OnActivate += Activate;
    }
}