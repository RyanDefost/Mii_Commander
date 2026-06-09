using System;
using System.Collections;
using UnityEngine;

namespace Managers.QuestManagement.Rewards
{
    public class RewardSpawnPoint : MonoBehaviour
    {
        private void Awake() => ComponentRegistry.AddToRegistry(this);
        private void OnDestroy() => ComponentRegistry.RemoveFromRegistry(this);

        public void Spawn(GameObject rewardObject, int rewardAmount, float spawnDelayInBetween) => 
            StartCoroutine(SpawnMultipleOverTime(rewardObject, rewardAmount, spawnDelayInBetween));
        public void Spawn(GameObject rewardObject) => Instantiate(rewardObject, this.transform.position, this.transform.rotation);

        private IEnumerator SpawnMultipleOverTime(GameObject rewardObject, int rewardAmount, float spawnDelayInBetween)
        {
            for (int i = 0; i < rewardAmount; i++)
            {
                Spawn(rewardObject);
                yield return new WaitForSeconds(spawnDelayInBetween);
            }
        }

    }
}