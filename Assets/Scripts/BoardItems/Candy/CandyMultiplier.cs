using System.Collections.Generic;
using Grid;
using UnityEngine;

namespace Candy
{
    public class CandyMultiplier : BoardItemComponent
    {
        [SerializeField] private float multiplier = 1.2f;
        [SerializeField] private bool canRepeat = true;
        [Space]
        [SerializeField] private string effectPatternName = "Square3x3";

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
            
            //GET SURROUNDING CANDY FUNC();

            GridManager.GridInstance[] surroundingCandy =
                gridManager.GetNeighbors(this.boardItem.gridInstanceRef, multiplierPattern.Value);

            print(surroundingCandy.Length+ "|" + surroundingCandy);
            
            foreach (GridManager.GridInstance gridInstance in surroundingCandy)
            {
                if(gridInstance == null || gridInstance == this.boardItem.gridInstanceRef)  continue;

                if (gridInstance.boardItem.gameObject.TryGetComponent(out CandyActor actor))
                {
                    actor.ApplyPointMultiplier(this.multiplier);
                    
                }
            }
        }
    }
}