using System;
using Managers.QuestManagement.Rewards;
using UnityEngine;

namespace Managers.QuestManagement
{
    public abstract class Quest : ScriptableObject
    {
        [SerializeField]
        protected QuestReward reward;
        private bool rewardUsed;
        private QuestVisual visual;
        protected Action<string> OnTextChange;
        
        public virtual float GetDelay() => 0.2f;
        public abstract string GetText();

        public virtual void Init(QuestVisual visual) => this.visual = visual;

        public void Update()
        {
            if (CheckCondition())
                TriggerReward();
        }

        protected abstract bool CheckCondition();

        private void TriggerReward()
        {
            if (this.rewardUsed)
                return;
            this.rewardUsed = true;
            this.reward.Activate();
            Disable();
            this.visual.TriggerDestroy();
        }
        
        protected abstract void Disable();

        public void RegisterOnTextChange(Action<string> func) => this.OnTextChange += func;
        public void UnRegisterOnTextChange(Action<string> func) => this.OnTextChange -= func;

        public abstract Quest CreateInstanceFromRef();
        
        protected void SetReward(QuestReward questReward) => this.reward = questReward;
    }
}