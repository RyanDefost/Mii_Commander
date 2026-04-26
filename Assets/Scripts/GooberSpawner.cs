using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class GooberSpawner : MonoBehaviour
{
    [SerializeField] GameObject spawnObject;
    [SerializeField] bool canSpawn = true;
    
    [Header("Settings")]
    [SerializeField] private Vector2 rangeSpawnRate = Vector2.zero;
    [SerializeField] private Vector2 rangeSpawnDistance = Vector2.zero;
    [SerializeField] private Vector2Int rangeSpawnAmount = Vector2Int.zero;
    
    private float delay = 3;
    private float timer = 0;
    
    private bool running = true;

    private void Update()
    {
        if(!canSpawn) return;
        
        if (running)
        {
            timer += Time.deltaTime;
            if (!(timer > delay)) return;
            
            int randAmount = Random.Range(rangeSpawnAmount.x, rangeSpawnAmount.y);
            Vector2 randCircle = Random.insideUnitCircle.normalized * Random.Range(rangeSpawnDistance.x, rangeSpawnDistance.y);
            Vector3 randPos = new Vector3(randCircle.x, 0.0f, randCircle.y);
            
            if(randPos == Vector3.zero) return;
            
            SpawnTarget(spawnObject, transform.position + randPos, randAmount);
            running = false;
        }
        else
        {
            delay = Random.Range(rangeSpawnRate.x, rangeSpawnRate.y);
            timer = 0;
            
            running = true;
        }
    }
    
private void SpawnTarget(GameObject target, Vector3 spawnPosition, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            float randomXPoint = Random.Range(-2f, 2f);
            float randomZPoint = Random.Range(-2f, 2f);
            Vector3 randomOffset = new Vector3(randomXPoint, 0.0f, randomZPoint);
            
            Instantiate(target, spawnPosition + randomOffset, Quaternion.identity);
        }
    }
}

