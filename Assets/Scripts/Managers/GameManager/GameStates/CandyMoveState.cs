using Grid;
using PlayerHand;
using UnityEngine;

namespace Managers.GameStates
{
    public class CandyMoveState : State<GameManager>
    {
        private PlayerHandManager playerHandRef;
        private GridManager gridManager;
        
        public override void Start()
        {
            this.Owner.TurnManager.OnreachedEnd += TrySetScore;
            
            //Stop player interact
            this.playerHandRef = ComponentRegistry.GetComponent<PlayerHandManager>();
            this.playerHandRef.SetCanGrab(false);
            
            this.Owner.MoveManager.SetMove();
            this.Owner.TurnManager.NextTurn();
            
            Debug.Log("ENTER MOVECANDYSTATE");
        }

        public override void Update()
        {
            this.Owner.SetGameState(GameState.CANDYACTIVATE);
        }

        public override void Exit()
        {
            this.Owner.TurnManager.OnreachedEnd -= TrySetScore;
            
            Debug.Log("EXIT WaitGameState");
        }

        private void TrySetScore(GridMoveable moveable)
        {
            if (moveable.gameObject.TryGetComponent(out CandyActor candyActor))
                this.Owner.ScoreManager.AddScore(candyActor.Points);
        }


    }
}