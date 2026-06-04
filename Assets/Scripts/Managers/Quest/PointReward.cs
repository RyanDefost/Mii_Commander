using UnityEngine;

namespace Managers.Quest
{
    [CreateAssetMenu(fileName = "PointReward", menuName = "QuestRewards/PointReward")]
    public class PointReward : QuestReward
    {
        [SerializeField] private int rewardValue;
        
        public override void Activate()
        {
            GameManager gameManagerRef = ComponentRegistry.GetComponent<GameManager>();
            if (gameManagerRef || gameManagerRef.ScoreManager) 
                gameManagerRef.ScoreManager.AddScore(this.rewardValue);
        }
    }
}