using UnityEngine;

namespace Managers.Quest
{
    public abstract class QuestReward : ScriptableObject
    {
        public abstract void Activate();
    }
}