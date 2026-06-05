using Grid;
using HelperStructs;
using PlayerHand;
using Unity.VisualScripting;
using UnityEngine;

namespace Managers.GameStates
{
    public class CandyMoveState : State<GameManager>
    {
        private PlayerHandManager playerHandRef;
        private CandyCleaner candyCleaner;
        
        public CandyMoveState(GameManager owner) : base(owner)
        {
            this.Owner.TurnManager.OnEndReached += TrySetScore;
            this.candyCleaner = ComponentRegistry.GetComponent<CandyCleaner>();
        }

        public override void Start()
        {
            //Let player interact
            this.playerHandRef = ComponentRegistry.GetComponent<PlayerHandManager>();
            this.playerHandRef.SetCanGrab(false);
            
            
            if(this.candyCleaner) this.candyCleaner.CleanBoard();
            this.Owner.TurnManager.AddToWaitTime(1f); //TODO: DOES NOT WORK FOR ABILITY WAITING.
                                                            //SHOULD BE CHANGED IN FUTURE FOR BETTER PACING
            this.Owner.MoveManager.SetMove();
            this.Owner.TurnManager.NextTurn();
        }

        public override void Update()
        {
            this.Owner.SetGameState(GameState.PLAY);
        }

        public override void Exit() { }

        private void TrySetScore(GridMoveable moveable)
        {
            if (moveable.gameObject.TryGetComponent(out CandyActor candyActor))
                this.Owner.ScoreManager.AddScore(candyActor.Points);
        }


    }
}