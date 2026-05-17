using PlayerHand;
using UnityEngine;

namespace Managers.GameStates
{
    public class WaitGameState : State<GameManager>
    {
        private PlayerHandManager playerHandRef;
        
        public override void Start()
        {
            //Let player interact
            this.playerHandRef = ComponentRegistry.GetComponent<PlayerHandManager>();
            this.playerHandRef.SetCanGrab(false);
            
            this.Owner.MoveManager.SetMove();
            this.Owner.TurnManager.NextTurn();
            
            Debug.Log("ENTER WaitGameState");
        }

        public override void Update()
        {
            this.Owner.SetGameState(GameState.PLAY);
        }

        public override void Exit()
        {
            Debug.Log("EXIT WaitGameState");
        }
    }
}