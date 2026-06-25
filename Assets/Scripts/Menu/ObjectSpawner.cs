using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Menu
{
    [RequireComponent(typeof(BoxCollider))]
    public class ObjectSpawner : MonoBehaviour
    {
        [SerializeField] private List<GameObject> spawnables = new();
        [Space]
        [SerializeField] private int maxSpawnAmount = 5;
        private int currentSpawned;

        [SerializeField] private Vector2Int minMaxSpawnTime;
        [SerializeField] private float despawnTime;
    
        private BoxCollider colliderRef;

        private void Start()
        {
            this.colliderRef = GetComponent<BoxCollider>();
            this.colliderRef.isTrigger = true;
        }

        private void Update()
        {
            if (this.currentSpawned >= this.maxSpawnAmount) return;
        
            this.currentSpawned++;
            StartCoroutine(SpawnObjects());
        }

        private IEnumerator SpawnObjects()
        {
            yield return new WaitForSeconds(Random.Range(this.minMaxSpawnTime.x, this.minMaxSpawnTime.y));
        
            GameObject spawnable = this.spawnables[Random.Range(0, this.spawnables.Count)]; 
            Vector3 spawnPoint = GetRandomPointInBounds(this.colliderRef.bounds);
        
            GameObject spawnedObject = Instantiate(spawnable, spawnPoint, Quaternion.identity);
        
            yield return new WaitForSeconds(this.despawnTime);
            DestroyImmediate(spawnedObject);
            this.currentSpawned--;
        }
    
        private Vector3 GetRandomPointInBounds(Bounds bounds) {
            float minX = bounds.size.x * -0.5f;
            float minY = bounds.size.y * -0.5f;
            float minZ = bounds.size.z * -0.5f;

            return (Vector3)this.gameObject.transform.TransformPoint(
                new Vector3(Random.Range(minX, -minX),
                    Random.Range (minY, -minY),
                    Random.Range (minZ, -minZ))
            );
        }

    }   
}
