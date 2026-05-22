using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Candy
{
    public class MultiplierCandyActor : CandyActor
    {
        [SerializeField] private float multiplier = 1.2f;
        
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
        
        public override void ActivateAbility()
        {
            base.ActivateAbility();
            
            //GET SURROUNDING CANDY FUNC();
            List<CandyActor> surroundingCandy = new();

            foreach (CandyActor candy in surroundingCandy)
            {
                candy.ApplyPointMultiplier(this.multiplier);
            }
        }
    }
}