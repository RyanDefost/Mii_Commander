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
            if(this.staysOpen) return;
            this.animator.Play(this.openCloseAnimation.name);   
        }

        public void SetFixedOpenState(bool isOpen)
        {
            switch (isOpen)
            {
                case true when !this.staysOpen:
                    this.animator.Play(this.StayOpenAnimation.name);
                    this.staysOpen = true;
                    break;
                case false when this.staysOpen:
                    this.animator.Play(this.StayClosedAnimation.name);
                    this.staysOpen = false;
                    break;
            }
        } 
    }
}
