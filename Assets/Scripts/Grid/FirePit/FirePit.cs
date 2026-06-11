using System;
using Managers;
using UnityEngine;

namespace Grid.FirePit
{
    [RequireComponent(typeof(Animator))]
    public class FirePit : MonoBehaviour
    {
        [SerializeField] private AnimationClip openCloseAnimation;
        
        [SerializeField] private AnimationClip StayOpenAnimation;
        [SerializeField] private AnimationClip StayClosedAnimation;
        private TurnManager turnManager;
        private Animator animator;

        private bool staysOpen;
        
        private void Start()
        {
            this.animator = this.GetComponent<Animator>();
            
            this.turnManager = ComponentRegistry.GetComponent<GameManager>().TurnManager;
            this.turnManager.OnWaitingToMove += Activate;
        }
        private void OnDestroy() => this.turnManager.OnWaitingToMove -= Activate;

        private void Activate()
        {
            if(staysOpen) return;
            this.animator.Play(openCloseAnimation.name);   
        }

        public void SetFixedOpenState(bool isOpen)
        {
            switch (isOpen)
            {
                case true when !this.staysOpen:
                    this.animator.Play(StayOpenAnimation.name);
                    this.staysOpen = true;
                    break;
                case false when this.staysOpen:
                    this.animator.Play(StayClosedAnimation.name);
                    this.staysOpen = false;
                    break;
            }
        } 
    }
}
