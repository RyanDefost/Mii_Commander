using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers.QuestManagement
{
    /// <summary>
    /// Keeps track of all quests within the level
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        [SerializeField] private Transform uiParent;
        [SerializeField]
        private GameObject questPrefab;
        private List<Quest> quests;
    
        public void SetQuests(IList<Quest> quests)
        {
            this.quests = new List<Quest>(quests);
            StartCoroutine(LoadQuests());
        }

        public void AddQuest(Quest quest) => this.quests.Add(quest);

        private IEnumerator LoadQuests(float delay = 1)
        {
            yield return new WaitForSeconds(delay);
            foreach (Quest quest in this.quests) yield return LoadQuest(quest);
        }

        private YieldInstruction LoadQuest(Quest quest)
        {
            GameObject questObj = Instantiate(this.questPrefab, this.uiParent);
            
            QuestVisual visual = questObj.GetComponent<QuestVisual>();
            quest.Init(visual);
            visual.GetTextComponent().text = quest.GetText();
            quest.RegisterOnTextChange(newText => visual.GetTextComponent().text = newText);
            
            return new WaitForSeconds(quest.GetDelay());
        }

        public void UpdateQuests()
        {
            List<Quest> questCopy = new(this.quests);
            questCopy.ForEach(quest =>
            {
                if (!quest) return;
                quest.Update();
            });
        }
    }
}
