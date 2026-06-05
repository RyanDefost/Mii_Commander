using System.Collections.Generic;
using System.Linq;
using Grid;
using UnityEngine;

namespace Candy
{
    public class CandyMultiplier : BoardItemComponent
    {
        [Header("Multiply Effect")]
        [SerializeField] private float multiplier = 1.2f;
        [SerializeField] private string effectPatternName = "Square3x3";
        [SerializeField] private bool canRepeat = true;
        [Header("Effected Candy")]
        [SerializeField] private bool effectAllCandy = false;
        [SerializeField] private List<int> effectedCandyTypeIndexes = new();

        private NeighborPatternRegistry patternRegistry;
        private GridManager gridManager;
        
        public override void ConnectToBoardItem() => this.boardItem.OnActivate += Activate;

        private void OnDestroy() => this.boardItem.OnActivate -= Activate;

        private void Activate()
        {
            this.patternRegistry ??= ComponentRegistry.GetComponent<NeighborPatternRegistry>();
            if(!this.patternRegistry) return;
            
            this.gridManager ??= ComponentRegistry.GetComponent<GridManager>();
            if(!this.gridManager) return;
            
            NeighborPattern? multiplierPattern = this.patternRegistry.GetNeighborPattern(this.effectPatternName);
            if (multiplierPattern == null) return;
            
            GridManager.GridInstance[] surroundingCandy =
                this.gridManager.GetNeighbors(this.boardItem.gridInstanceRef, multiplierPattern.Value);
            
            
            foreach (GridManager.GridInstance gridInstance in surroundingCandy)
            {
                if(gridInstance == null || gridInstance == this.boardItem.gridInstanceRef)  continue;

                if (gridInstance.boardItem.GetType() == typeof(CandyActor))
                {
                    CandyActor actor = gridInstance.boardItem as CandyActor;
                    
                    if(this.effectedCandyTypeIndexes.All(index => actor.candyType != index) && !this.effectAllCandy) continue;
                    actor.ApplyPointMultiplier(this.multiplier);
                }
            }
            
            if(!this.canRepeat) Deactivate();
        }

        private void Deactivate() => this.boardItem.OnActivate -= Activate;
    }
}