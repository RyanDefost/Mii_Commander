using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

namespace Menu
{
    [Serializable]
    public struct TutorialInfo
    {
        public VideoClip videoClip;
        public string infoText;
    }
    
    [RequireComponent(typeof(Animator), typeof(VideoPlayer))]
    public class TutorialScreen : MonoBehaviour
    {
        [Header("Transitions")]
        [SerializeField] private AnimationClip moveOutClip;
        [SerializeField] private AnimationClip moveInClip;
        private Animator animator;
        
        [Header("Tutorial Clips")]
        [SerializeField] private List<TutorialInfo> tutorials = new();
        private VideoPlayer videoPlayer;
        
        [Header("References")] 
        [SerializeField] private List<TextMeshProUGUI> textElements;

        private int currentIndex = -1;
        private bool isMoving;
        
        private void Start()
        {
            this.animator = GetComponent<Animator>();
            this.videoPlayer = GetComponent<VideoPlayer>();

            this.currentIndex++;
            this.animator.Play(this.moveInClip.name);
        }

        public void ToggleNextTutorial()
        {
            if(this.isMoving) return;
            this.isMoving = true;

            this.currentIndex++;
            this.animator.Play(this.moveOutClip.name);   
        }
        public void TogglePreviousTutorial()
        {
            if(this.isMoving) return;
            this.isMoving = true;

            this.currentIndex--;
            this.animator.Play(this.moveOutClip.name);
        }

        /// <summary>
        /// Bound to Tutorial_MoveOut Animation Event On 'Screen' animator.
        /// </summary>
        public void OnMovedOut()
        {
            this.animator.Play(this.moveInClip.name);
        }
        
        /// <summary>
        /// Bound to Tutorial_MoveIn Animation Event On 'Screen' animator.
        /// </summary>
        public void OnMoveIn()
        {
            this.isMoving = false;
            this.currentIndex = Mathf.Clamp(this.currentIndex, 0, this.tutorials.Count - 1);
            
            SetTutorialInfo(this.tutorials[this.currentIndex]);
        }

        private void SetTutorialInfo(TutorialInfo info)
        {
            if(info.videoClip == null) return;
            
            this.videoPlayer.clip = info.videoClip;
            foreach (TextMeshProUGUI textElement in this.textElements) 
                textElement.text = info.infoText;
        }
    }
}