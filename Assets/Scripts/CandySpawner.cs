using System;
using System.Collections.Generic;
using PlayerHand;
using UnityEngine;
using Random = UnityEngine.Random;

public class CandySpawner : MonoBehaviour
{
    [SerializeField]
    private PlayerHandManager playerHandManager;
    [SerializeField]
    private GameObject candyPrefab;
    private ObjectPool<CandyInstance> candyPool;

    private class CandyInstance : IPoolable
    {
        public bool Active { get; set; }

        private readonly List<Rigidbody> instances;
        private readonly PlayerHandManager playerHandRef;

        public CandyInstance(PlayerHandManager playerHandManager, GameObject prefab, Transform parent, int amount)
        {
            const float clumpingDist = 0.3f;
            
            this.instances = new List<Rigidbody>();
            for (int i = 0; i < amount; i++)
            {
                GameObject instance = Instantiate(prefab, parent);
                instance.transform.localPosition = Vector3.zero;
                instance.transform.position += new Vector3(Random.Range(-clumpingDist, clumpingDist), Random.Range(-clumpingDist, clumpingDist), 0);

                Rigidbody rb = instance.GetComponent<Rigidbody>();
                rb.constraints = RigidbodyConstraints.FreezePosition;
                this.instances.Add(rb);
            }

            this.playerHandRef = playerHandManager;
            playerHandManager.OnGrabReleased += OnGrabReleased;
        }
        
        public void OnEnableObject()
        {
            foreach (Rigidbody instance in this.instances) instance.gameObject.SetActive(true);
        }

        public void OnDisableObject()
        {
            foreach (Rigidbody instance in this.instances)
            {
                instance.gameObject.SetActive(false);
                instance.constraints = RigidbodyConstraints.FreezePosition;
            }
        }

        private void OnGrabReleased()
        {
            foreach (Rigidbody instance in this.instances)
            {
                instance.transform.parent = instance.transform.parent.parent;
                instance.constraints = RigidbodyConstraints.None;
            }
            this.playerHandRef.OnGrabReleased -= OnGrabReleased;
        }

        public void SetParent(Transform parent)
        {
            foreach (Rigidbody instance in this.instances)
            {
                instance.transform.parent = parent;
                instance.transform.localPosition = Vector3.zero;
                instance.transform.position += new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0);
            }
        }
    }

    private void OnValidate()
    {
        this.enabled = this.candyPrefab && this.playerHandManager;
    }

    private void Awake() => this.candyPool = new ObjectPool<CandyInstance>();

    public void AddToHand()
    {
        CandyInstance result = this.candyPool.RequestObject();

        if (result == null)
        {
            result = new CandyInstance(this.playerHandManager, this.candyPrefab, this.playerHandManager.transform, 5);
            this.candyPool.ActivateObject(result);
            return;
        }

        result.SetParent(this.playerHandManager.transform);
    }
}