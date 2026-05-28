using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Grid;

namespace Synergy
{
    public class SynergyLookUp : MonoBehaviour
    {
        [SerializeField] private List<Synergy> synergyList = new();

        private NeighborPatternRegistry neighborPatternRegistry = new();
        private GridManager gridManager;

        private readonly Vector2Int[] directions =
        {
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.up,
            Vector2Int.down
        };

        private void Awake() => ComponentRegistry.AddToRegistry(this);

        public Synergy GetSynergy(GridManager.GridInstance gridInstanceRef)
        {
            this.neighborPatternRegistry ??= ComponentRegistry.GetComponent<NeighborPatternRegistry>();
            if(!this.neighborPatternRegistry) Debug.LogWarning($"{nameof(this.neighborPatternRegistry)} is null");
            
            this.gridManager ??= ComponentRegistry.GetComponent<GridManager>();
            if(!this.gridManager) Debug.LogWarning($"{nameof(SynergyLookUp)} requires a GridManager");

            
            foreach (Synergy currentSynergy in synergyList)
            {
                print(gridInstanceRef);
                
                NeighborPattern? newPattern = NeighborPattern.Right;
                GridManager.GridInstance[] synergyItems = this.gridManager.GetNeighbors(gridInstanceRef, newPattern.Value);
                
                //Check Synergy.
                bool hasSynergy = currentSynergy.hasOrder ? 
                   CheckOrderedSynergy(currentSynergy, synergyItems) : CheckUnOrderedSynergy(currentSynergy, synergyItems);
                
                if(hasSynergy) return currentSynergy;
            }
            
            return null;
        }

        private static bool CheckOrderedSynergy(Synergy synergy, GridManager.GridInstance[] synergyItems)
        {
            print("Checking ordered synergy");
            List<BoardItem> lookupItems = synergy.synergyItems;
            
            //Loop trough both lists and check if both of them are the same candyType.
            for (int i = 0; lookupItems.Count <= 0; i++)
            {
                if (lookupItems[i].gameObject.name != synergyItems[i].gameObj.name)
                    return false;
            }
            
            return true;
        }

        private static bool CheckUnOrderedSynergy(Synergy synergy, GridManager.GridInstance[] synergyItems)
        {
            print("Checking unordered synergy");
            List<string> lookupNames = synergy.synergyItems.Select(synergyItem => synergyItem.name).ToList();
            foreach (var VARIABLE in lookupNames)
            {
                print(VARIABLE);
            }
            
            
            foreach (GridManager.GridInstance item in synergyItems)
            {
                print(item);
                
                /*if (lookupNames.Contains(item.gameObj.name))
                    lookupNames.Remove(item.gameObj.name);
                else
                    return false;*/
            }
            
            return true;
        }
    }
}