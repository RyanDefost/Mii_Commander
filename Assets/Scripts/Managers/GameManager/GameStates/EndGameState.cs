using Grid;
using Grid.FirePit;
using StateMachine;
using UnityEngine;

namespace Managers.GameStates
{
    public class EndGameState : State<GameManager> 
    {
        private GridManager gridManager;
        private FirePit firePit;
        
        private bool isDone = false;
        
        public EndGameState(GameManager owner) : base(owner)
        {
            
        }

        public override void Start()
        {
            Debug.unityLogger.Log("Beginning END state");
            this.gridManager ??= ComponentRegistry.GetComponent<GridManager>();
            
            this.Owner.TurnManager.SetWaitOnDestroy(false);
            
            this.firePit ??= Object.FindFirstObjectByType<FirePit>();
            this.firePit.SetFixedOpenState(true);
            
        }

        public override void Update()
        {
            if(this.isDone) return;
            
            if (this.gridManager.HasActiveInstances())
            {
                this.Owner.SetGameState(GameState.CANDYMOVE);
                return;
            }

            this.isDone = true;
            this.Owner.OnGameEnd?.Invoke();
            
            this.firePit.SetFixedOpenState(false);
        }

        public override void Exit()
        {
            
        }
    }
}