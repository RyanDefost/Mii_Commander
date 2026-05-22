using PlayerHand;
using Unity.VisualScripting;
using UnityEngine;

namespace Managers.GameStates
{
    public class WaitGameState : State<GameManager>
    {
        private PlayerHandManager playerHandRef;

        public WaitGameState(GameManager owner) : base(owner)
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
            if (moveable.gameObject.TryGetComponent<CandyActor>(out CandyActor candyActor))
            {
                this.Owner.ScoreManager.AddScore(candyActor.Points);
            }
        }
    }
}