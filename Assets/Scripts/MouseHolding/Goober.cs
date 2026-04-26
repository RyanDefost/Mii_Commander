using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody), typeof(AreaSelector))]
public class Goober : MonoBehaviour, IDetectable
{
    [SerializeField] private int health = 5;
    [SerializeField] private float viewRadius = 5f;
    
    [SerializeField] private DetectableTypes type;
    public DetectableTypes DetectableType { get => type; set => type = value; }
    
    public GameObject GameObject => this.gameObject;
    public Rigidbody Rigidbody { get; private set; }
    
    private AreaSelector areaSelector;
    private List<IDetectable> visableEnemies = new();
    
    private IDetectable target = null;
    private Vector3 wanderTarget = Vector3.zero;
    
    private bool isPickedUp = false;
    private bool isGrounded = false;
    
    private void Awake()
    {
        this.Rigidbody = GetComponent<Rigidbody>();
        this.areaSelector = GetComponent<AreaSelector>();
    }

    private void Update()
    {
        if(this.isPickedUp) return;
        
        CenterRotation();
        TryFindDestination();
    }


    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            //this.Rigidbody.linearVelocity = Vector3.zero;
        }
    }

    public void OnHold()
    {
        Debug.Log("OnHold");
        this.isPickedUp = true;
        this.isGrounded = false;
        
        this.transform.position += Vector3.up;
        this.Rigidbody.useGravity = false;
    }

    public void OnReleaseHold()
    {
        Debug.Log("OnReleaseHold");
        this.Rigidbody.useGravity = true;
        
        this.isPickedUp = false;
    }
    
    private void CenterRotation()
    {
        if(transform.eulerAngles is { x: 0, y: 0 }) return;
        
        this.Rigidbody.angularVelocity = Vector3.zero;
        float xRot = Mathf.LerpAngle(transform.eulerAngles.x, 0,  Time.deltaTime);
        float yRot = Mathf.LerpAngle(transform.eulerAngles.y, 0,  Time.deltaTime);
        float zRot = Mathf.LerpAngle(transform.eulerAngles.z, 0,  Time.deltaTime);
        
        transform.eulerAngles = new Vector3(xRot, yRot, zRot);
    }

    private void TryFindDestination()
    {
        if (this.isPickedUp || !this.isGrounded)
        {
            wanderTarget = Vector3.zero;
            return;
        }
        
        visableEnemies = this.areaSelector.GetSelection(this.transform.position, viewRadius, DetectableTypes.ENEMY);
        if (visableEnemies.Count > 0)
        {
            target = visableEnemies.First();
        
            this.transform.position =
                Vector3.Lerp(
                    this.transform.position,
                    target.GameObject.transform.position,
                    Time.deltaTime * 1
                );

            if (Vector3.Distance(this.transform.position, target.GameObject.transform.position) <= 0.8f)
            {
                Destroy(target.GameObject);
                health--;

                if (health <= 0)
                    Destroy(gameObject);
            }
        }
        else // WANDER DESTINATION
        {
            if (wanderTarget == Vector3.zero)
            {
                 var circlePos= (Random.insideUnitCircle * 2);
                 wanderTarget = transform.position +  new Vector3(circlePos.x, 0, circlePos.y);
            }
        
            this.transform.position =
                Vector3.Lerp(
                    this.transform.position,
                    wanderTarget,
                    Time.deltaTime * 1
                );
            
            if(Vector3.Distance(this.transform.position, wanderTarget) <= 0.1f) 
                wanderTarget = Vector3.zero;
        }
    }
}
