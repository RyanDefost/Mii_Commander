using System.Collections.Generic;
using PlayerHand;
using UnityEngine;

public class CandyInstance : IPoolable
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
            GameObject instance = Object.Instantiate(prefab, parent);
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

    private void OnGrabReleased(Vector2 handMovementDir, Vector2 throwForce)
    {
        foreach (Rigidbody instance in this.instances)
        {
            instance.transform.parent = instance.transform.parent.parent;
            instance.constraints = RigidbodyConstraints.None;
            Vector3 throwVel = throwForce;
            throwVel += Quaternion.Euler(0, 0, Random.Range(-90, 90)) * handMovementDir;
            instance.AddForce(throwVel, ForceMode.Impulse);
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