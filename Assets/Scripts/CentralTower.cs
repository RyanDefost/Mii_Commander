using System;
using UnityEngine;

public class CentralTower : MonoBehaviour
{
    [SerializeField] private int health = 10;
    [SerializeField] private GameObject ground;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.TryGetComponent<IDetectable>(out IDetectable detectable))
        {
            if (detectable.DetectableType == DetectableTypes.ENEMY)
            {
                Destroy(detectable.GameObject);
                health--;
            }
        }
    }

    private void Update()
    {
        if (health <= 0)
        {
            Destroy(ground);
        }
    }
}
