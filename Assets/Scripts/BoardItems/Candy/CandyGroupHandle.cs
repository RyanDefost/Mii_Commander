using System.Collections.Generic;
using PlayerHand;
using UnityEngine;

/// <summary>
/// A class for managing multiple candy instances,
/// this would make it more efficient to reuse dead candy when adding multiple to the hand
/// </summary>
public class CandyGroupHandle : IPoolable
{
    public bool Active { get; set; }

    private readonly List<CandyHandle> instances;
    private readonly PlayerHandManager playerHandRef;

    public CandyGroupHandle(PlayerHandManager playerHandManager, GameObject prefab, Transform parent, int amount)
    {
        const float clumpingDist = 0.3f;
        
        this.instances = new List<CandyHandle>();
        for (int i = 0; i < amount; i++)
        {
            GameObject gameObject = Object.Instantiate(prefab, parent);
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.position += new Vector3(Random.Range(-clumpingDist, clumpingDist), Random.Range(-clumpingDist, clumpingDist), 0);

            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezePosition;
            
            CandyActor actor = gameObject.GetComponent<CandyActor>();
            this.instances.Add(new CandyHandle(rb, actor, gameObject));
        }

        this.playerHandRef = playerHandManager;
        playerHandManager.OnGrabReleased += OnGrabReleased;
    }
    
    public void OnEnableObject()
    {
        foreach (CandyHandle instance in this.instances) instance.gameObject.SetActive(true);
    }

    public void OnDisableObject()
    {
        foreach (CandyHandle instance in this.instances)
        {
            instance.gameObject.SetActive(false);
            instance.rb.constraints = RigidbodyConstraints.FreezePosition;
        }
    }

    private void OnGrabReleased(Vector2 handMovementDir, Vector2 throwForce)
    {
        foreach (CandyHandle instance in this.instances) 
            CandyActor.OnGrabReleased(instance.rb, instance.actor, handMovementDir, throwForce);
        this.playerHandRef.OnGrabReleased -= OnGrabReleased;
    }

    public void SetParent(Transform parent)
    {
        foreach (CandyHandle instance in this.instances)
        {
            instance.gameObject.transform.parent = parent;
            instance.gameObject.transform.localPosition = Vector3.zero;
            instance.gameObject.transform.position += new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0);
        }
    }
}