using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Terget : MonoBehaviour, IDetectable
{
    [SerializeField] private DetectableTypes type;
    public DetectableTypes DetectableType { get => type; set => type = value; }
    
    public GameObject GameObject => this.gameObject;
    public Rigidbody Rigidbody { get; private set; }

    private CentralTower centralTower;
    private NavMeshAgent navMeshAgent;
    
    private void Awake()
    {
        this.centralTower = FindFirstObjectByType<CentralTower>();    
        this.navMeshAgent = this.GetComponent<NavMeshAgent>();
        this.Rigidbody = this.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        this.navMeshAgent.enabled = true;
        this.navMeshAgent.destination = this.centralTower.transform.position;
    }

    public void OnHold()
    {
        throw new System.NotImplementedException();
    }

    public void OnReleaseHold()
    {
        throw new System.NotImplementedException();
    }

}
