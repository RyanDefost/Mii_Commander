using UnityEngine;

namespace Managers.QuestManagement.Rewards
{
    public abstract class QuestReward : ScriptableObject
    {
        public abstract void Activate();
    }
}