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
        private string amountText;

        public override string GetText() => $"Get {this.gameManagerRef?.ScoreManager?.GetScore() - this.pointsOnInit}/{this.pointRequirement} {this.amountText}";

        public override void Init(QuestVisual visual)
        {
            base.Init(visual);
            this.amountText = this.pointRequirement == 1 ? "point" : "points";
            this.gameManagerRef ??= ComponentRegistry.GetComponent<GameManager>();
            if (!this.gameManagerRef.ScoreManager) return;
            this.gameManagerRef.ScoreManager.OnChangeScore += CallOnTextChange;
            this.pointsOnInit = this.gameManagerRef.ScoreManager.GetScore();
        }

        private void CallOnTextChange(int _ = -1) => this.OnTextChange?.Invoke(GetText());
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