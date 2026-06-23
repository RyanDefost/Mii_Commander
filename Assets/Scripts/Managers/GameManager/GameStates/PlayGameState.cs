using Grid;
using PlayerHand;
using StateMachine;

namespace Managers.GameStates
{
    public class PlayGameState : State<GameManager>
    {
        private PlayerHandManager playerHandRef;
        
        private bool gameEnded;
        
        public PlayGameState(GameManager owner) : base(owner)
        {
            this.Owner.MoveManager.OnLastMove += SetGameEndState;
        }

        public override void Start()
        {
            //Let player interact
            this.playerHandRef ??= ComponentRegistry.GetComponent<PlayerHandManager>();
            this.playerHandRef.SetCanGrab(true);
        }

        public override void Update()
        {
            this.Owner.QuestManager.UpdateQuests();
            
            /*if(this.Owner.HasEndedGame)
                this.Owner.SetGameState(GameState.END);*/
        }

        public override void Exit()
        {
            // Debug.Log("EXIT PlayGameState");
        }

        private void SetGameEndState()
        {
            this.gameEnded = true;
            this.Owner.MoveManager.OnLastMove -= SetGameEndState;
        } 
    }
}