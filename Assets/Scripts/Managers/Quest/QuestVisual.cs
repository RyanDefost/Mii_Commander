using System;
using TMPro;
using UnityEngine;

namespace Managers.Quest
{
    public class QuestVisual : MonoBehaviour
    {
        private static readonly int SlideOut = Animator.StringToHash("SlideOut");

        [SerializeField]
        private TextMeshProUGUI textObj;
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private AnimationClip slideOutClip;

        private bool isDestroying;

        private void OnValidate()
        {
            if (Application.isPlaying) return;
            this.textObj = GetComponentInChildren<TextMeshProUGUI>();
            this.animator = GetComponent<Animator>();
        }

        public TextMeshProUGUI GetTextComponent() => this.textObj;

        public void TriggerDestroy()
        {
            if (this.isDestroying)
                return;
            this.isDestroying = true;
            this.animator.SetTrigger(SlideOut);
            Invoke(nameof(RemoveFromScene), this.slideOutClip.length);
        }

        private void RemoveFromScene() => Destroy(this.gameObject);
    }
}