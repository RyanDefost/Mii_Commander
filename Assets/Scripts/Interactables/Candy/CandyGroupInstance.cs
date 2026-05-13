using System.Collections.Generic;
using PlayerHand;
using UnityEngine;

/// <summary>
/// A class for managing multiple candy instances,
/// this would make it more efficient to reuse dead candy when adding multiple to the hand
/// </summary>
public class CandyGroupInstance : IPoolable
{
    public bool Active { get; set; }

    private readonly List<CandyInstance> instances;
    private readonly PlayerHandManager playerHandRef;
    
    /// <summary>
    /// Manages the data of a candy instance,
    /// counterpart to the CandyComponent that handles its in game logic
    /// TODO move to its own file
    /// </summary>
    private class CandyInstance
    {
        public readonly Rigidbody rb;
        public readonly CandyComponent component;
        public readonly GameObject gameObject;

        public CandyInstance(Rigidbody rb, CandyComponent candyComponent, GameObject gameObject)
        {
            this.rb = rb;
            this.component = candyComponent;
            this.gameObject = gameObject;
        }
    }

    public CandyGroupInstance(PlayerHandManager playerHandManager, GameObject prefab, Transform parent, int amount)
    {
        const float clumpingDist = 0.3f;
        
        this.instances = new List<CandyInstance>();
        for (int i = 0; i < amount; i++)
        {
            GameObject gameObject = Object.Instantiate(prefab, parent);
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.position += new Vector3(Random.Range(-clumpingDist, clumpingDist), Random.Range(-clumpingDist, clumpingDist), 0);

            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezePosition;
            
            CandyComponent component = gameObject.GetComponent<CandyComponent>();
            this.instances.Add(new CandyInstance(rb, component, gameObject));
        }

        this.playerHandRef = playerHandManager;
        playerHandManager.OnGrabReleased += OnGrabReleased;
    }
    
    public void OnEnableObject()
    {
        foreach (CandyInstance instance in this.instances) instance.gameObject.SetActive(true);
    }

    public void OnDisableObject()
    {
        foreach (CandyInstance instance in this.instances)
        {
            instance.gameObject.SetActive(false);
            instance.rb.constraints = RigidbodyConstraints.FreezePosition;
        }
    }

    private void OnGrabReleased(Vector2 handMovementDir, Vector2 throwForce)
    {
        foreach (CandyInstance instance in this.instances) 
            CandyComponent.OnGrabReleased(instance.rb, instance.component, handMovementDir, throwForce);
        this.playerHandRef.OnGrabReleased -= OnGrabReleased;
    }

    public void SetParent(Transform parent)
    {
        foreach (CandyInstance instance in this.instances)
        {
            instance.gameObject.transform.parent = parent;
            instance.gameObject.transform.localPosition = Vector3.zero;
            instance.gameObject.transform.position += new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0);
        }
    }
}