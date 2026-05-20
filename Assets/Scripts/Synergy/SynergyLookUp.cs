using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Grid;
using Synergy;
using Unity.VisualScripting;
using UnityEngine;

namespace Synergy
{
    public class SynergyLookUp : MonoBehaviour
    {
        //[SerializeField] private List<Synergy> synergyList = new();
        [SerializeField] private Synergy synergy;
    
        private Vector3[] directions = new []{Vector3.up, Vector3.down, Vector3.left, Vector3.right};
        
        private List<GridManager.GridInstance> everyItems = new(); 
        private List<GridManager.GridInstance> synergyItems = new();
        
        private GridManager myGridManager;
        

        private void OnDrawGizmos()
        {
            foreach (var item in everyItems)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(item.position, 1);
            }
            
            foreach (var item in synergyItems)
            {
                Gizmos.color = Color.lawnGreen;
                Gizmos.DrawSphere(item.position, 1.2f);
            }
        }


        private void Start()
        {
            this.myGridManager = ComponentRegistry.GetComponent<GridManager>();
            StartCoroutine(GetSynergyVisuals());
        }

        private IEnumerator GetSynergyVisuals()
        {
            yield return new WaitForSeconds(10);

            var tempInstance = this.myGridManager.GetNearestGridPosition( Vector3.zero, null);
            CheckForSynergy(myGridManager, tempInstance);
            
            Debug.Log(everyItems.Count);
        }


        public bool CheckForSynergy(GridManager gridManager, GridManager.GridInstance centerInstance)
        {
            foreach (Vector3 direction in directions)
            {
                if (DirectionContainsSynergy(direction, gridManager, centerInstance))
                {
                    // Activate
                    return true;
                }
            }
            
            return false;
        }

        /// <summary>
        /// IN THE GRIDMANAGER --> ADD FINCTION TO GET A LIST OF ALL POSITIONS AND A FUNTION TO GET GRIDINSTANCE BASED ON THE POSITION!!!!!
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="gridManager"></param>
        /// <param name="centerInstance"></param>
        /// <returns></returns>
        
        // Returns if direction contains synergy
        private bool DirectionContainsSynergy(Vector3 direction, GridManager gridManager ,GridManager.GridInstance centerInstance)
        {
            List<GridManager.GridInstance> currentItems = new();
            for (int i = 0; i < synergy.synergyItems.Count; )
            {
                //Get next gridInstance.
                Vector3 offset = direction * i;
                GridManager.GridInstance currentInstance = gridManager.GetNearestPosition(centerInstance.position + gridManager.CellSize.y * offset,
                    centerInstance.boardItem, centerInstance);
                    
                print(currentInstance);
                
                //Check if expected position.
                if(!(currentInstance.position == centerInstance.position + gridManager.CellSize.y * offset)) return false;
                
                print("Correct pos");
                currentItems.Add(currentInstance);
                everyItems.Add(currentInstance);
                
                //Check for same type.
                if (!currentInstance.boardItem == synergy.synergyItems[i].BoardItem) return false;
                
                print("Correct type");
                
                i++;
            }
            synergyItems.AddRange(currentItems);
            return true;
        }
        
        public void AddSynergy(Synergy synergy)
        {
            //synergyList.Add(synergy);
        }
    }
}