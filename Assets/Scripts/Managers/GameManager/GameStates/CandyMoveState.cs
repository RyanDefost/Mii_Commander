using Grid;
using PlayerHand;
using Unity.VisualScripting;
using UnityEngine;

namespace Managers.GameStates
{
    public class CandyMoveState : State<GameManager>
    {
        private PlayerHandManager playerHandRef;
        
        private Timer waitTimer;

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
            
            //Timer
            this.waitTimer = new Timer(1f, false, false, ExitState);
            this.waitTimer.ResetAndReplay();
        }

        public override void Update()
        {
            this.waitTimer.UpdateTime(Time.deltaTime);
        }

        public override void Exit() { }

        private void TrySetScore(GridMoveable moveable)
        {
            if (moveable.gameObject.TryGetComponent(out CandyActor candyActor))
                this.Owner.ScoreManager.AddScore(candyActor.Points);
        }

        private void ExitState() => this.Owner.SetGameState(this.Owner.HasEndedGame ? GameState.END : GameState.PLAY);
    }
}