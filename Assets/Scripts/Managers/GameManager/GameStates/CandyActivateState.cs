using System.Collections;
using System.Collections.Generic;
using Grid;
using Unity.VisualScripting;
using UnityEngine;

namespace Managers.GameStates
{
    public class CandyActivateState : State<GameManager>
    {
        private GridManager gridManager;
        
        private List<BoardItem> boardItems;
        private int currentAbilityCount = 0;
        
        private Timer timer;
        private bool activatedAllAbilities = false;
        
        public override void Start()
        {
            this.boardItems =  GetBoardItems();
            this.activatedAllAbilities = false;
            this.currentAbilityCount = 0;
            
            TryActivateAbilities();
            
            Debug.Log("ENTER ABILITYSTATE");
        }

        public override void Update()
        {
            this.timer?.UpdateTime(Time.deltaTime);

            if(this.activatedAllAbilities)
                this.Owner.SetGameState(GameState.PLAY);
        }

        public override void Exit()
        {
            
        }
        
        private List<BoardItem> GetBoardItems()
        {
            List<BoardItem> boardItems = new();
            
            this.gridManager = ComponentRegistry.GetComponent<GridManager>();
            this.gridManager.ForAllGridItems(instance =>
            {
                if (instance == null || !instance.boardItem)
                    return;
                
                boardItems.Add(instance.boardItem);
            });
            
            return boardItems;
        }
        
        private void TryActivateAbilities()
        {
            if (this.boardItems.Count == 0 || this.currentAbilityCount >= this.boardItems.Count)
            {
                activatedAllAbilities = true;
                return;
            }

            if (this.boardItems[this.currentAbilityCount])
                this.boardItems[this.currentAbilityCount]?.ActivateAbility();
            
            this.currentAbilityCount++;
            
            this.timer = new Timer(1, false, true, TryActivateAbilities );
        }
    }
}