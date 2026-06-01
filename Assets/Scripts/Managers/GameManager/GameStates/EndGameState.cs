using Grid;
using UnityEngine;

namespace Managers.GameStates
{
    public class EndGameState : State<GameManager> 
    {
        private  GridManager gridManager;
        
        public EndGameState(GameManager owner) : base(owner)
        {
            
        }

        public override void Start()
        {
            Debug.unityLogger.Log("Beginning game state");
            this.gridManager ??= ComponentRegistry.GetComponent<GridManager>();
            if (gridManager.HasActiveInstances())
            {
                Debug.Log("True");
            }
        }

        public override void Update()
        {
            if (this.gridManager.HasActiveInstances())
                this.Owner.SetGameState(GameState.CANDYMOVE);
        }

        public override void Exit()
        {
            
        }
    }
}