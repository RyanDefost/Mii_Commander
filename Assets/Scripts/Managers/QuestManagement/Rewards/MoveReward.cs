using UnityEngine;

namespace Managers.QuestManagement.Rewards
{
    [CreateAssetMenu(fileName = "MoveReward", menuName = "QuestRewards/MoveReward")]
    public class MoveReward : QuestReward
    {
        [SerializeField] private int rewardValue;
        
        public override void Activate()
        {
            GameManager gameManagerRef = ComponentRegistry.GetComponent<GameManager>();
            if (gameManagerRef || gameManagerRef.MoveManager) 
                gameManagerRef.MoveManager.AddMoveAmount(this.rewardValue);
        }
    }
}