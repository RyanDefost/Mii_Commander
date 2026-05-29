using System;
using System.Linq;
using Grid;
using Managers;
using UnityEngine;

namespace Synergy
{
    public class Combiner : BoardItemComponent
    {
        private NeighborPatternRegistry neighborPatternRegistry;
        private SynergyLookUp synergyLookUp;
        private GridManager gridManager;
        private GameManager gameManager;

        private GameObject currentOutput;
        
        public override void ConnectToBoardItem() => this.boardItem.OnActivate += Activate;

        /// <summary>
        /// When activated, needed pattern direction is chosen and checks synergyLookup for possible synergies.
        /// </summary>
        private void Activate()
        {
            //Init
            this.neighborPatternRegistry ??= ComponentRegistry.GetComponent<NeighborPatternRegistry>();
            this.synergyLookUp ??= ComponentRegistry.GetComponent<SynergyLookUp>();
            this.gridManager ??= ComponentRegistry.GetComponent<GridManager>();
            this.gameManager ??= ComponentRegistry.GetComponent<GameManager>();
            
            //Get Neighbors
            NeighborPattern? sidePattern = GetForwardNeighborPattern(this.transform.rotation.eulerAngles.z);
            GridManager.GridInstance[] foundNeighbors = this.gridManager.GetNeighbors(this.boardItem.gridInstanceRef, sidePattern.Value);
            
            //Check neighbors for any synergy.
            foreach (GridManager.GridInstance neighbor in foundNeighbors)
            {
                if (neighbor == null || !neighbor.gameObj) continue;

                Tuple<Synergy, GridManager.GridInstance[]> synergyInfo = this.synergyLookUp.TryGetSynergy(neighbor);
                if (synergyInfo == null) continue;
                
                ApplySynergy(synergyInfo, neighbor);
            }
        }

        /// <summary>
        /// Applies and Sets changes required for the synergy.
        /// </summary>
        /// <param name="synergyInfo">Synergy Type and relevant boardItems.</param>
        /// <param name="neighbor">BoardItem that interacted with the combiner.</param>
        private void ApplySynergy(Tuple<Synergy, GridManager.GridInstance[]> synergyInfo, GridManager.GridInstance neighbor)
        {
            this.gameManager.TurnManager.AddToWaitTime(2f);
            
            foreach (GridManager.GridInstance instance in synergyInfo.Item2)
            {
                this.gridManager.ReleaseInstance(instance);
                Destroy(instance.gameObj);
            }
                    
            //Initialize GameObject
            currentOutput = Instantiate(synergyInfo.Item1.output, neighbor.position, Quaternion.identity);
            if (currentOutput.TryGetComponent(out BoardItem boardItem))
                boardItem.OnStarted += SetOutputToBoard;
        }
        
        /// <summary>
        /// Helper function to convert eulerAngles.Z into pattern direction.
        /// </summary>
        /// <param name="rotation">Object eulerAngles.Z</param>
        /// <returns>Returns corresponding NeighborPattern</returns>
        private static NeighborPattern GetForwardNeighborPattern(float rotation)
        {
            int[] directions = { 0, 90, 180, 270 };
            int nearest = directions.OrderBy(x => Math.Abs((long) x - rotation)).First();
            return nearest switch
            {
                0 => NeighborPattern.Up,
                90 => NeighborPattern.Left,
                180 => NeighborPattern.Down,
                270 => NeighborPattern.Right,
                var _ => NeighborPattern.Up
            };
        }

        /// <summary>
        /// Initializes the currentOutput when object has started.
        /// </summary>
        private void SetOutputToBoard() => currentOutput.GetComponent<BoardItem>().Initiate();
    }
}
