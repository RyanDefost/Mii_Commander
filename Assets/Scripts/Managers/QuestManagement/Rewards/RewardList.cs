using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;

namespace Managers.QuestManagement.Rewards
{
    [CreateAssetMenu(fileName = "RewardList", menuName = "QuestRewards/RewardList")]
    public class RewardList : QuestReward
    {
        [SerializeField]
        private List<QuestReward> itemRewards;
        [SerializeField]
        private int waitInMilliseconds = 500;
        
        public override void Activate() => this.itemRewards.ForEach(ActivateAsync);

        private async void ActivateAsync(QuestReward reward)
        {
            if (!reward) return;
            Debug.Log(reward.name);
            await Task.Delay(this.waitInMilliseconds);
            reward.Activate();
        }
    }
}