using System;
using System.Collections.Generic;
using System.Linq;
using Grid;
using Managers;
using Unity.Mathematics;
using UnityEngine;

namespace Synergy
{
    public class Splitter : BoardItemComponent
    {
        private NeighborPatternRegistry neighborPatternRegistry;
        private GridManager gridManager;
        private GameManager gameManager;

        private int splitPoints = 0;
        
        public override void ConnectToBoardItem() => this.boardItem.OnActivate += Activate;

        /// <summary>
        /// When activated, needed pattern direction is chosen and checks synergyLookup for possible synergies.
        /// </summary>
        private void Activate()
        {
            //Init.
            this.neighborPatternRegistry ??= ComponentRegistry.GetComponent<NeighborPatternRegistry>();
            this.gridManager ??= ComponentRegistry.GetComponent<GridManager>();
            this.gameManager ??= ComponentRegistry.GetComponent<GameManager>();
            
            //Get Neighbors.
            NeighborPattern? sidePattern = GetForwardNeighborPattern(this.transform.rotation.eulerAngles.z);
            GridManager.GridInstance[] foundNeighbors = this.gridManager.GetNeighbors(this.boardItem.gridInstanceRef, sidePattern.Value);
            
            //Check neighbors for any synergy.
            foreach (GridManager.GridInstance neighbor in foundNeighbors)
            {
                if (neighbor == null || !neighbor.gameObj) continue;
                
                if(neighbor.gameObj.TryGetComponent(out IngredientComponent ingredientComponent))
                    ApplySplit(ingredientComponent, neighbor);
            }
        }

        private void ApplySplit(IngredientComponent ingredientComponent, GridManager.GridInstance splitInstance)
        {
            List<GameObject> ingredients = ingredientComponent.GetIngredients();
            if(ingredients.Count == 0) return;
            
            if (splitInstance.boardItem is CandyActor actor) 
                this.splitPoints = actor.Points;
            
            //Release splitInstance
            this.gridManager.ReleaseInstance(splitInstance);
            Destroy(splitInstance.gameObj);
                    
            //Initialize GameObject.
            foreach (GameObject ingredient in ingredients)
            {
                this.gameManager.TurnManager.AddToWaitTime(0.2f);
                
                GameObject currentOutput = Instantiate(ingredient, splitInstance.position, Quaternion.identity);    
                
                if (currentOutput.TryGetComponent(out BoardItem boardItem))
                    boardItem.OnStarted += SetOutputToBoard;
            }
        }
        
        /// <summary>
        /// Helper function to convert eulerAngles.Z into pattern direction.
        /// </summary>
        /// <param name="rotation">Object eulerAngles.Z</param>
        /// <returns>Returns corresponding NeighborPattern</returns>
        private static NeighborPattern GetForwardNeighborPattern(float rotation)
        {
            int[] directions = { 0, 90, 180, 270 };
            int nearest = directions.OrderBy(x => math.abs((long) x - rotation)).First();
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
        private void SetOutputToBoard(BoardItem item)
        {
            item.Initiate();
            
            if (item is CandyActor actor)
                actor.AddPoints(this.splitPoints);
            
            item.OnStarted -= SetOutputToBoard;
        }
    }
}