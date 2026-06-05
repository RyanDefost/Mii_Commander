using UnityEngine;

namespace Managers.Quest
{
    [CreateAssetMenu(fileName = "TypedPointQuest", menuName = "Quests/TypedPointQuest")]
    public class TypedPointQuest : Quest
    {
        [SerializeField]
        private int candyType;
        private GameManager gameManagerRef;
        [SerializeField] private int pointRequirement;
        private int lastPoints;
        private int collectedPoints;
        
        private string amountText;

        public override string GetText()
        {
            CandyTypeLookup.CandyTypeOption candy = ComponentRegistry.GetComponent<CandyTypeLookup>()?.Get(this.candyType);
            return candy != null ? $"Get {this.collectedPoints}/{this.pointRequirement} {candy.name} {this.amountText}" : $"Get {this.collectedPoints}/{this.pointRequirement} {this.amountText}";
        }

        public override void Init(QuestVisual visual)
        {
            base.Init(visual);
            this.amountText = this.pointRequirement == 1 ? "point" : "points";
            this.gameManagerRef ??= ComponentRegistry.GetComponent<GameManager>();
            if (!this.gameManagerRef.ScoreManager) return;
            this.gameManagerRef.ScoreManager.OnChangeScore += CustomUpdate;
            this.lastPoints = this.gameManagerRef.ScoreManager.GetScore();
        }

        private void CustomUpdate(int candyId)
        {
            this.gameManagerRef ??= ComponentRegistry.GetComponent<GameManager>();
            if (!this.gameManagerRef.ScoreManager) return;
            
            int diff = this.gameManagerRef.ScoreManager.GetScore() - this.lastPoints;
            if (candyId == this.candyType && diff > 0) this.collectedPoints += diff;
            this.OnTextChange?.Invoke(GetText());

            this.lastPoints = this.gameManagerRef.ScoreManager.GetScore();
        }

        protected override bool CheckCondition() => this.collectedPoints >= this.pointRequirement;

        protected override void Disable() => this.gameManagerRef.ScoreManager.OnChangeScore -= CustomUpdate;

        public override Quest CreateInstanceFromRef()
        {
            TypedPointQuest newInstance = CreateInstance<TypedPointQuest>();
            newInstance.SetPointRequirement(this.pointRequirement);
            newInstance.SetReward(this.reward);
            return newInstance;
        }

        private void SetPointRequirement(int newPointRequirement) => this.pointRequirement = newPointRequirement;
    }
}