using UnityEngine;

namespace Managers
{
    [CreateAssetMenu(fileName = "LevelRef", menuName = "ScriptableObjects/LevelRef", order = 1)]
    public class LevelRef : ScriptableObject
    {
        public int pointRequirement = 0;
        public int baseTurnAmount = 0;
        //public List<Quests> Quests = new();
    }
}