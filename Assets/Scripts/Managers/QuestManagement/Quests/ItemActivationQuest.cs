using BoardItems;
using UnityEngine;

namespace Managers.QuestManagement
{
    [CreateAssetMenu(fileName = "ItemActivationQuest", menuName = "Quests/ItemActivationQuest")]
    public class ItemActivationQuest : Quest
    {
        [SerializeField]
        private string itemName;
        [SerializeField]
        private string abilityName;
        [SerializeField] private int requiredAmount;
        private int counter;
        private string amountText;
        
        public override string GetText() => $"Trigger {this.itemName}'s ability '{this.abilityName}' {this.requiredAmount} {this.amountText}";

        public override void Init(QuestVisual visual)
        {
            base.Init(visual);
            Helper.OnItemActivated += CustomUpdate;
            this.amountText = this.requiredAmount == 1 ? "time" : "times";
        }

        private void CustomUpdate(string itemName, string abilityName)
        {
            if (itemName != this.itemName || abilityName != this.abilityName)
                return;
            this.counter += 1;
            this.OnTextChange?.Invoke(GetText());
        }

        protected override bool CheckCondition() => this.counter >= this.requiredAmount;

        protected override void Disable() => Helper.OnItemActivated -= CustomUpdate;

        public override Quest CreateInstanceFromRef()
        {
            ItemActivationQuest newInstance = CreateInstance<ItemActivationQuest>();
            newInstance.SetItemName(this.itemName);
            newInstance.SetAbilityName(this.abilityName);
            newInstance.SetRequiredAmount(this.requiredAmount);
            newInstance.SetReward(this.reward);
            return newInstance;
        }

        private void SetItemName(string newItemName) => this.itemName = newItemName;
        private void SetAbilityName(string newAbilityName) => this.abilityName = newAbilityName;
        private void SetRequiredAmount(int newPointRequirement) => this.requiredAmount = newPointRequirement;
    }
}