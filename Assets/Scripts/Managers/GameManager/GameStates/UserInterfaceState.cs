using PlayerHand;
using StateMachine;
using UnityEngine;

namespace Managers.GameStates
{
    public class UserInterfaceState : State<GameManager>
    {
        private PlayerHandManager playerHandRef;
        
        public UserInterfaceState(GameManager owner) : base(owner) { }

        public override void Start()
        {
            Debug.Log("UserInterfaceState.Start");
            
            //Let player interact
            this.playerHandRef ??= ComponentRegistry.GetComponent<PlayerHandManager>();
            this.playerHandRef.SetCanGrab(false);
        }

        public override void Update() { }

        public override void Exit()
        {
            this.playerHandRef.SetCanGrab(true);
        }
    }
}