using PlayerHand;
using UnityEngine;

namespace Managers.GameStates
{
    public class PlayGameState : State<GameManager>
    {
        private PlayerHandManager playerHandRef;
        
        public override void Start()
        {
            //Let player interact
            this.playerHandRef = ComponentRegistry.GetComponent<PlayerHandManager>();
            this.playerHandRef.SetCanGrab(true);
            
        }

        public override void Update()
        {
            //throw new System.NotImplementedException();
        }

        public override void Exit()
        {
        }
    }
}