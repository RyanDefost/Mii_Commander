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

        private NeighborPatternRegistry neighborPatternRegistry;
        private GridManager gridManager;

        private GridManager.GridInstance[] currentInstances;
        
        private readonly NeighborPattern[] directions =
        {
            NeighborPattern.Left,
            NeighborPattern.Right,
            NeighborPattern.Up,
            NeighborPattern.Down
        };

        private void Awake() => ComponentRegistry.AddToRegistry(this);

        public Tuple<Synergy, GridManager.GridInstance[]> GetSynergy(GridManager.GridInstance gridInstanceRef)
        {
            this.neighborPatternRegistry ??= ComponentRegistry.GetComponent<NeighborPatternRegistry>();
            if(!this.neighborPatternRegistry) Debug.LogWarning($"{nameof(this.neighborPatternRegistry)} is null");
            
            this.gridManager ??= ComponentRegistry.GetComponent<GridManager>();
            if(!this.gridManager) Debug.LogWarning($"{nameof(SynergyLookUp)} requires a GridManager");

            
            foreach (Synergy currentSynergy in synergyList)
            {
                bool hasSynergy = CheckSynergy(currentSynergy, gridInstanceRef);
                if(hasSynergy) return  Tuple.Create(currentSynergy, currentInstances);
                
                /*NeighborPattern? newPattern = NeighborPattern.Right;
                GridManager.GridInstance[] synergyItems = this.gridManager.GetNeighbors(gridInstanceRef, newPattern.Value);
                
                //Check Synergy.
                bool hasSynergy = currentSynergy.hasOrder ? 
                   CheckOrderedSynergy(currentSynergy, synergyItems) : CheckUnOrderedSynergy(currentSynergy, synergyItems);
                
                if(hasSynergy) return currentSynergy;*/
            }
            
            return null;
        }

        private bool CheckSynergy(Synergy synergy, GridManager.GridInstance centerInstance)
        {
            List<BoardItem> synergyItems = synergy.synergyItems;
            
            foreach (NeighborPattern direction in directions)
            {
                bool directionHasSynergy = true;
                GridManager.GridInstance currentInstance = centerInstance;
                this.currentInstances = new GridManager.GridInstance[synergyItems.Count];

                for (int i = 0; i < synergyItems.Count; i++)
                {
                    if (currentInstance == null) //TODO FIX UGLY CODE.
                    {
                        directionHasSynergy = false;
                        break;
                    }
                    
                    string currentName = (currentInstance.gameObj.name).Replace("(Clone)", "");
                    if (currentName != synergyItems[i].gameObject.name)
                    {
                        directionHasSynergy = false;
                        break;
                    }
                    
                    this.currentInstances[i] = currentInstance;
                    
                    currentInstance = GetDirectNeighbor(currentInstance, direction);
                }
                
                if(directionHasSynergy) return true;
            }
            
            return false;
        }

        private GridManager.GridInstance GetDirectNeighbor(GridManager.GridInstance currentInstance, NeighborPattern direction)
        {
            GridManager.GridInstance[] nextInstances = this.gridManager.GetNeighbors(currentInstance, direction);
            foreach (GridManager.GridInstance neighbor in nextInstances)
            {
                if (neighbor == null || !neighbor.gameObj) continue;
                return neighbor;
            }

            return null;
        }
        

        /*private static bool CheckOrderedSynergy(Synergy synergy, GridManager.GridInstance[] synergyItems)
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
        }*/

        /*private static bool CheckUnOrderedSynergy(Synergy synergy, GridManager.GridInstance[] synergyItems)
        {
            print("Checking unordered synergy");
            List<string> lookupNames = synergy.synergyItems.Select(synergyItem => synergyItem.name).ToList();
            
            foreach (GridManager.GridInstance item in synergyItems)
            {
                if (lookupNames.Contains(item.gameObj.name))
                    lookupNames.Remove(item.gameObj.name);
                else
                    return false;
            }
            
            return true;
        }*/
    }
}