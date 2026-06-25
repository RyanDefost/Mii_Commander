using UnityEngine;

namespace Managers.QuestManagement.Rewards
{
    [CreateAssetMenu(fileName = "NewQuestReward", menuName = "QuestRewards/NewQuestReward")]
    public class NewQuestReward : QuestReward
    {
        [SerializeField] private Quest questTemplate;

        public override void Activate()
        {
            if (!this.questTemplate)
            {
                Debug.LogError("NewQuestReward: questTemplate is not assigned!");
                return;
            }

            QuestManager questManager = ComponentRegistry.GetComponent<GameManager>()?.QuestManager;
            if (questManager)
            {
                Quest newQuest = this.questTemplate.CreateInstanceFromRef();
                questManager.AddQuest(newQuest);
            }
            else
                Debug.LogError("NewQuestReward: QuestManager not found in registry.");
        }
    }
}