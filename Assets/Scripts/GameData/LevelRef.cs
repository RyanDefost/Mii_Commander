using System.Collections.Generic;
using Managers.QuestManagement;
using UnityEngine;

namespace GameData
{
    [CreateAssetMenu(fileName = "LevelRef", menuName = "ScriptableObjects/LevelRef", order = 1)]
    public class LevelRef : ScriptableObject
    {
        public int pointRequirement = 0;
        public int baseTurnAmount = 0;
        [SerializeField]
        private List<ScriptableObject> quests = new();

        private void OnValidate()
        {
            if (Application.isPlaying) return;
            List<ScriptableObject> newList = new();
            foreach (ScriptableObject obj in this.quests)
                if (obj is Quest || !obj)
                    newList.Add(obj);
        }

        public IList<Quest> GetQuests()
        {
            return this.quests.ConvertAll(obj =>
            {
                Quest questRef = obj as Quest;
                return !questRef ? null : questRef.CreateInstanceFromRef();
            });
        }
    }
}