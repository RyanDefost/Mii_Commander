using System;
using Scoring;
using UnityEngine;

namespace Managers.Quest
{
    [CreateAssetMenu(fileName = "PointQuest", menuName = "Quests/PointQuest")]
    public class PointQuest : Quest
    {
        private GameManager gameManagerRef;
        [SerializeField] private int pointRequirement;
        private int pointsOnInit;

        public override string GetText() => $"Get {this.gameManagerRef?.ScoreManager?.GetScore() - this.pointsOnInit}/{this.pointRequirement} points";

        public override void Init(QuestVisual visual)
        {
            base.Init(visual);
            this.gameManagerRef ??= ComponentRegistry.GetComponent<GameManager>();
            if (!this.gameManagerRef.ScoreManager) return;
            this.gameManagerRef.ScoreManager.OnChangeScore += CallOnTextChange;
            this.pointsOnInit = this.gameManagerRef.ScoreManager.GetScore();
        }

        private void CallOnTextChange() => this.OnTextChange?.Invoke(GetText());
        protected override bool CheckCondition() => this.gameManagerRef?.ScoreManager?.GetScore() - this.pointsOnInit >= this.pointRequirement;

        protected override void Disable() => this.gameManagerRef.ScoreManager.OnChangeScore -= CallOnTextChange;

        public override Quest CreateInstanceFromRef()
        {
            PointQuest newInstance = CreateInstance<PointQuest>();
            newInstance.SetPointRequirement(this.pointRequirement);
            newInstance.SetReward(this.reward);
            return newInstance;
        }

        private void SetPointRequirement(int newPointRequirement) => this.pointRequirement = newPointRequirement;
    }
}