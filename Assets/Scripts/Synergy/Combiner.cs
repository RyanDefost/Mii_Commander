using System;
using System.Linq;
using Grid;
using Managers;
using UnityEngine;

namespace Synergy
{
    public class Combiner : BoardItemComponent
    {
        [SerializeField] private bool shouldActivateOnItsOwn = true;
        
        private NeighborPatternRegistry neighborPatternRegistry;
        private SynergyLookUp synergyLookUp;
        private GridManager gridManager;
        private GameManager gameManager;

        private GameObject currentOutput;
        
        public override void ConnectToBoardItem()
        {
            if (this.shouldActivateOnItsOwn)
                EnableActivate();
        }

        public void Activate()
        {
            this.neighborPatternRegistry ??= ComponentRegistry.GetComponent<NeighborPatternRegistry>();
            if(!this.neighborPatternRegistry) Debug.LogWarning($"{nameof(this.neighborPatternRegistry)} is null");
            
            this.synergyLookUp ??= ComponentRegistry.GetComponent<SynergyLookUp>();
            this.gridManager ??= ComponentRegistry.GetComponent<GridManager>();
            this.gameManager ??= ComponentRegistry.GetComponent<GameManager>();
            
            NeighborPattern? sidePattern = NeighborPattern.Right;
            GridManager.GridInstance[] foundNeighbors = this.gridManager.GetNeighbors(this.boardItem.gridInstanceRef, sidePattern.Value);
            
            
            foreach (GridManager.GridInstance neighbor in foundNeighbors)
            {
                if (neighbor == null || !neighbor.gameObj) continue;

                Tuple<Synergy, GridManager.GridInstance[]> synergyInfo = this.synergyLookUp.GetSynergy(neighbor);
                if (synergyInfo != null)
                {
                    this.gameManager.TurnManager.AddToWaitTime(5f);
                    foreach (var instance in synergyInfo.Item2)
                    {
                        this.gridManager.ReleaseInstance(instance);
                        Destroy(instance.gameObj);
                    }
                    
                    currentOutput = Instantiate(synergyInfo.Item1.output, neighbor.position, Quaternion.identity);
                    
                    //Wait till object is initialized
                    if (currentOutput.TryGetComponent(out BoardItem boardItem))
                        boardItem.OnStarted += SetOutputToBoard;
                }
                return;
            }
        }

        private void SetOutputToBoard() => currentOutput.GetComponent<BoardItem>().Initiate();

        public void EnableActivate() => this.boardItem.OnActivate += Activate;
    }
}
