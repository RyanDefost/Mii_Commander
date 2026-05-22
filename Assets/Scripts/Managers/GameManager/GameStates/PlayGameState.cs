using PlayerHand;

namespace Managers.GameStates
{
    public class PlayGameState : State<GameManager>
    {
        private PlayerHandManager playerHandRef;

        public PlayGameState(GameManager owner) : base(owner)
        {
            
        }

        public override void Start()
        {
            //Let player interact
            this.playerHandRef = ComponentRegistry.GetComponent<PlayerHandManager>();
            this.playerHandRef.SetCanGrab(true);
            
            // Debug.Log("ENTER PlayGameState");
        }

        public override void Update()
        {
            //throw new System.NotImplementedException();
        }

        public override void Exit()
        {
            // Debug.Log("EXIT PlayGameState");
        }
    }
}