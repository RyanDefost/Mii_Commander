using BoardItems;
using Grid;
using PlayerHand;
using Unity.VisualScripting;
using UnityEngine;

namespace Managers.GameStates
{
    public class CandyMoveState : State<GameManager>
    {
        private PlayerHandManager playerHandRef;

        public CandyMoveState(GameManager owner) : base(owner)
        {
            this.Owner.TurnManager.OnEndReached += TrySetScore;
        }

        public override void Start()
        {
            //Let player interact
            this.playerHandRef = ComponentRegistry.GetComponent<PlayerHandManager>();
            this.playerHandRef.SetCanGrab(false);
            
            this.Owner.MoveManager.SetMove();
            this.Owner.TurnManager.NextTurn();
            
            // Debug.Log("ENTER WaitGameState");
        }

        public override void Update()
        {
            this.Owner.SetGameState(GameState.PLAY);
        }

        public override void Exit() { }

        private void TrySetScore(GridMoveable moveable)
        {
            if (moveable.gameObject.TryGetComponent(out CandyActor candyActor))
                this.Owner.ScoreManager.AddScore(candyActor.Points, candyActor.candyType);
        }


    }
}