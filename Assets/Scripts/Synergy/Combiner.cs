using System;
using System.Linq;
using Grid;
using UnityEngine;

namespace Synergy
{
    public class Combiner : BoardItemComponent
    {
        [SerializeField] private bool shouldActivateOnItsOwn = true;
        
        private NeighborPatternRegistry neighborPatternRegistry;
        private SynergyLookUp synergyLookUp;
        private GridManager gridManager;
        
        public override void ConnectToBoardItem()
        {
            if (this.shouldActivateOnItsOwn)
                EnableActivate();
        }

        public void Activate()
        {
            this.neighborPatternRegistry ??= ComponentRegistry.GetComponent<NeighborPatternRegistry>();
            this.synergyLookUp ??= ComponentRegistry.GetComponent<SynergyLookUp>();
            this.gridManager ??= ComponentRegistry.GetComponent<GridManager>();
            
            NeighborPattern? sidePattern = NeighborPattern.Right;
            GridManager.GridInstance[] foundNeighbors = this.gridManager.GetNeighbors(this.boardItem.gridInstanceRef, sidePattern.Value);
            
            foreach (GridManager.GridInstance neighbor in foundNeighbors)
            {
                if (neighbor == null || !neighbor.gameObj) continue;
                
                this.synergyLookUp.GetSynergy(neighbor);
                return;
            }
        }

        public void EnableActivate() => this.boardItem.OnActivate += Activate;
    }
}
