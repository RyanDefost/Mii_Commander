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
        
        private readonly NeighborPattern[] directions = {
            NeighborPattern.Left,
            NeighborPattern.Right,
            NeighborPattern.Up,
            NeighborPattern.Down
        };

        private void Awake() => ComponentRegistry.AddToRegistry(this);
        
        /// <summary>
        /// Tries to return the found synergy and affected boardItems.
        /// </summary>
        /// <param name="gridInstanceRef">CenterPoint to check for possible synergy</param>
        /// <returns></returns>
        public Tuple<Synergy, GridManager.GridInstance[]> TryGetSynergy(GridManager.GridInstance gridInstanceRef)
        {
            this.neighborPatternRegistry ??= ComponentRegistry.GetComponent<NeighborPatternRegistry>();
            this.gridManager ??= ComponentRegistry.GetComponent<GridManager>();

            //Loop over all synergies to find first applicable synergy.
            return (from currentSynergy in this.synergyList let hasSynergy = CheckSynergy(currentSynergy, gridInstanceRef) 
                where hasSynergy select Tuple.Create(currentSynergy, this.currentInstances)).FirstOrDefault();
        }

        /// <summary>
        /// Loops over all possible directions and orders where the given synergy could be applied.
        /// </summary>
        /// <param name="synergy">The current Synergy that is being checked for existing</param>
        /// <param name="centerInstance">StartingPoint from the instances to check</param>
        /// <returns>Indication if the synergy is found with the centerInstance</returns>
        private bool CheckSynergy(Synergy synergy, GridManager.GridInstance centerInstance)
        {
            List<BoardItem> synergyItems = synergy.synergyItems;
            foreach (NeighborPattern direction in this.directions)
            {
                //Check for synergy from center.
                bool hasCenterSynergy = CheckDirection(synergyItems, centerInstance, direction, synergy.hasOrder);
                if(hasCenterSynergy) return true;
                
                //Check for synergy towards center.
                GridManager.GridInstance[] centerConnectedCandy = this.currentInstances;
                foreach (GridManager.GridInstance instance in centerConnectedCandy)
                {
                    NeighborPattern inverseDirection = GetInverse(direction);
                    bool hasInverseSynergy = CheckDirection(synergyItems, instance, inverseDirection, synergy.hasOrder);
                    if(hasInverseSynergy) return true;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Checks from the given center and direction if boardItems align with the expected SynergyItems. 
        /// </summary>
        /// <param name="synergyItems">List of BoardItems that are expected for a Synergy</param>
        /// <param name="centerInstance">StartingPoint from the instances to check</param>
        /// <param name="direction">direction to check towards from the centerPoint</param>
        /// <param name="isOrdered">Checks differ depending on need for synergy order</param>
        /// <returns>Indication if a synergy is found in the current direction</returns>
        private bool CheckDirection(List<BoardItem> synergyItems, GridManager.GridInstance centerInstance, NeighborPattern direction, bool isOrdered = true)
        {
            GridManager.GridInstance currentInstance = centerInstance;
            this.currentInstances = new GridManager.GridInstance[synergyItems.Count];
            List<string> lookupNames = synergyItems.Select(synergyItem => synergyItem.name).ToList();
            
            bool hasSynergy = true;
            for (int i = 0; i < synergyItems.Count; i++)
            {
                if (currentInstance == null) return false;
                string correctedName = (currentInstance.gameObj.name).Replace("(Clone)", "");
                
                //Check if Synergy is Ordered.
                if (isOrdered) 
                {
                    if (correctedName != synergyItems[i].gameObject.name) 
                        hasSynergy = false;
                }
                //Check if Synergy is Unordered.
                else
                {
                    if (lookupNames.Contains(correctedName))
                        lookupNames.Remove(correctedName);
                    else
                        hasSynergy = false;
                }
                
                this.currentInstances[i] = currentInstance;
                currentInstance = GetDirectNeighbor(currentInstance, direction);
            }

            return hasSynergy;
        }

        /// <summary>
        /// HelperFunction to inverse neighborPattern direction.
        /// </summary>
        /// <param name="neighborPattern">Pattern that needs to be inversed</param>
        /// <returns>inversed Pattern</returns>
        private static NeighborPattern GetInverse(NeighborPattern neighborPattern)
        {
            if(neighborPattern.Equals(NeighborPattern.Right)) return NeighborPattern.Left;
            else if(neighborPattern.Equals(NeighborPattern.Left)) return NeighborPattern.Right;
            else if(neighborPattern.Equals(NeighborPattern.Down)) return NeighborPattern.Up;
            else if(neighborPattern.Equals(NeighborPattern.Up)) return NeighborPattern.Down;
            
            return default;
        }

        /// <summary>
        /// Helper function that Gets the only relevant gridInstance that is not Null.
        /// </summary>
        /// <param name="currentInstance">Instance that checks for neighbors</param>
        /// <param name="direction">NeighborPattern in needed direction</param>
        /// <returns>Instance that has been found and != Null</returns>
        private GridManager.GridInstance GetDirectNeighbor(GridManager.GridInstance currentInstance, NeighborPattern direction)
        {
            GridManager.GridInstance[] nextInstances = this.gridManager.GetNeighbors(currentInstance, direction);
            return nextInstances.FirstOrDefault(neighbor => neighbor != null && neighbor.gameObj);
        }
    }
}