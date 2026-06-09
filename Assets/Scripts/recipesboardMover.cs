using System;
using System.Collections;
using BoardItems;
using PlayerHand;
using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(Animator))]
    public class recipesboardMover : MonoBehaviour
    {
        [SerializeField] AnimationClip moveUpClip;
        [SerializeField] AnimationClip moveDownClip;
        
        private Animator animator;
        
        private bool isDown = false;
        private bool isMoving = false;

        private void Start()
        {
            this.animator = this.GetComponent<Animator>();
        }

        public void TogglePosition()
        {
            this.animator.Play(isDown ? moveUpClip.name : moveDownClip.name);
            isDown = !isDown;
        }

        private IEnumerator Move(Vector2 position, GameObject target)
        {
            
            target.transform.position = position;
            yield return new WaitForSeconds(0.1f);
        }
    }
}