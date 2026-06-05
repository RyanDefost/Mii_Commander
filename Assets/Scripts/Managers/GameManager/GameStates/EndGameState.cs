using Grid;
using UnityEngine;

namespace Managers.GameStates
{
    public class EndGameState : State<GameManager> 
    {
        private GridManager gridManager;
        
        private bool isDone = false;
        
        public EndGameState(GameManager owner) : base(owner)
        {
            
        }

        public override void Start()
        {
            Debug.unityLogger.Log("Beginning game state");
            this.gridManager ??= ComponentRegistry.GetComponent<GridManager>();
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
        }

        public override void Exit()
        {
            
        }
    }
}