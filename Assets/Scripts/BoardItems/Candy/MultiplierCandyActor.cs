using System;
using Managers;

namespace Candy
{
    public class MultiplierCandyActor : CandyActor
    {
        private GameManager gameManager;

        private void Start()
        {
            this.gameManager = ComponentRegistry.GetComponent<GameManager>();
            this.gameManager.OnChangeState += ActivateOnState;
        }

        private void OnDestroy() => this.gameManager.OnChangeState -= ActivateOnState;  

        private void ActivateOnState(GameState newState)
        {
            if (newState == GameState.WAIT)
                ActivateAbility();
        }
        
        protected override void ActivateAbility()
        {
            print("Activate Ability");
            base.ActivateAbility();
        }
    }
}