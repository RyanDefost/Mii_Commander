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

        private void Start()
        {
            this.neighborPatternRegistry = ComponentRegistry.GetComponent<NeighborPatternRegistry>();
            this.synergyLookUp = ComponentRegistry.GetComponent<SynergyLookUp>();
            this.gridManager = ComponentRegistry.GetComponent<GridManager>();
        }

        public override void ConnectToBoardItem()
        {
            if (this.shouldActivateOnItsOwn)
                EnableActivate();
        }

        public void Activate()
        {
            var nextNeighbor = gridManager.GetNeighbors(this.boardItem.gridInstanceRef, NeighborPattern.Right);
            this.synergyLookUp.GetSynergy(nextNeighbor.First());
        }

        public void EnableActivate() => this.boardItem.OnActivate += Activate;
    }
}
