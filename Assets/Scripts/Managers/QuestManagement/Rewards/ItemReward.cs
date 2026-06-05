using System;
using BoardItems;
using UnityEngine;

namespace Managers.QuestManagement.Rewards
{
    [CreateAssetMenu(fileName = "ItemReward", menuName = "QuestRewards/ItemReward")]
    public class ItemReward : QuestReward
    {
        [SerializeField] private GameObject rewardObject;
        [SerializeField] private int rewardAmount;
        [SerializeField] private float spawnDelayInBetween;

        private void OnValidate()
        {
            if (Application.isPlaying ||
                !this.rewardObject ||
                !this.rewardObject.TryGetComponent(out BoardItem boardItem)||
                boardItem)
                return;
            this.rewardObject = null;
            Debug.LogError("ItemReward requires a BoardItem component");
        }

        public override void Activate()
        {
            if (this.rewardAmount < 1 || !this.rewardObject) return;
            
            RewardSpawnPoint spawnPoint = ComponentRegistry.GetComponent<RewardSpawnPoint>();
            if (!spawnPoint) return;
            
            if (this.rewardAmount > 1)
                spawnPoint.Spawn(this.rewardObject, this.rewardAmount, this.spawnDelayInBetween);
            else if (this.rewardAmount == 1)
                spawnPoint.Spawn(this.rewardObject);
        }
    }
}