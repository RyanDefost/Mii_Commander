using System;
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

        private GridManager gridManager;
        private bool hasActivated = false;

        private void Update()
        {
            if(hasActivated) return;
            hasActivated = true;
            
            Debug.Log("HERE");
            this.gridManager = ComponentRegistry.GetComponent<GridManager>();

            this.gridManager.PrintInstances();
        }

        public bool CheckForSynergy(GridManager gridManager, GridManager.GridInstance targetInstance)
        {
            
            for (int i = 0; i < synergy.synergyItems.Count; i++)
            {
                var synergyBoardItem = synergy.synergyItems[i].BoardItem;
                //var instanceBoardItem = gridManager.
            }
            
            return false;
        }

        private void CompareSynergyList(int index, List<SynergyItem> synergyItems, GridManager.GridInstance targetInstance)
        {
            if (synergyItems[index].BoardItem == targetInstance.boardItem)
            {
                
            }
        }
        
        public void AddSynergy(Synergy synergy)
        {
            //synergyList.Add(synergy);
        }
    }
}