using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(MouseFollower), typeof(AreaSelector))]
public class ObjectHolder : MonoBehaviour
{
    public float GrabSize = 3;
    [SerializeField] float throwSpeed = 5f;
    [SerializeField] float trowMultiplier = 2f;
    [SerializeField] private GameObject holdCollection;
    
    [SerializeField, Header("Input Actions")]
    private InputActionAsset inputActionAsset;
    private InputActionMap inputActionMap;
    private InputAction interactAction;
    private InputAction pushAction;
    private InputAction scrollAction;
    
    private MouseFollower mouseFollower;
    private AreaSelector areaSelector;
    
    private List<IDetectable> HeldObjects = new();

    private Vector3 lastPosition;
    private Vector3 forceDirection;

    private Vector3 startHoldPosition;
    
    private void Awake() => InitControls();

    private void OnValidate()
    {
        this.mouseFollower = GetComponent<MouseFollower>();
        this.areaSelector = GetComponent<AreaSelector>();
    }

    private void InitControls()
    {
        this.inputActionMap = this.inputActionAsset.FindActionMap("UI");
        this.inputActionMap.Enable();
        
        this.scrollAction = this.inputActionMap.FindAction("ScrollWheel");
        
        this.pushAction = this.inputActionMap.FindAction("RightClick"); 
        this.pushAction.performed += PerformPush;
        
        this.interactAction = this.inputActionMap.FindAction("Click");
        this.interactAction.started += StartHold;
        this.interactAction.canceled += CancelHold;
    }

    private void Update()
    {
        if (this.scrollAction.ReadValue<Vector2>().y > 0f && GrabSize <= 3f)
            GrabSize += 0.2f;
        if (this.scrollAction.ReadValue<Vector2>().y < 0f && GrabSize > 0.2f)
            GrabSize -= 0.2f;
        
        if(!this.interactAction.inProgress) return;
        
        this.forceDirection = GetCalculateDirectionForce(this.startHoldPosition ,this.mouseFollower.HitInfo.point, throwSpeed);
        this.lastPosition = this.mouseFollower.MousePosition; 
        
        //Holding();
    }

    private void StartHold(InputAction.CallbackContext context)
    {
        if (!mouseFollower.TryGetHitInfo().HasValue) return;
        Vector3 selectionPoint = mouseFollower.HitInfo.point;
        
        holdCollection.transform.position = mouseFollower.HitInfo.point;
        startHoldPosition = mouseFollower.HitInfo.point; //

        HeldObjects = areaSelector.GetSelection(selectionPoint, this.GrabSize,DetectableTypes.GOOBER);
        
        foreach (IDetectable heldObject in HeldObjects)
        {
            heldObject.GameObject.transform.SetParent(holdCollection.transform);
            heldObject.OnHold();
        }
    }

    private void Holding()
    {
        holdCollection.transform.position =
            Vector3.Lerp(
                holdCollection.transform.position,
                this.mouseFollower.HitInfo.point,
                Time.deltaTime * 2
            );
    }
    
    private void CancelHold(InputAction.CallbackContext context)
    {
        foreach (IDetectable heldObject in HeldObjects)
        {
            heldObject.GameObject.transform.SetParent(null);
            
            heldObject.Rigidbody.linearVelocity = Vector3.zero;
            heldObject.Rigidbody.linearVelocity += Vector3.up * 2;
            heldObject.Rigidbody.linearVelocity += forceDirection * trowMultiplier;
            
            heldObject.OnReleaseHold();
        }
    }

    private void PerformPush(InputAction.CallbackContext context)
    {
        if (!mouseFollower.TryGetHitInfo().HasValue) return;
        Vector3 selectionPoint = mouseFollower.HitInfo.point;
        
        HeldObjects = areaSelector.GetSelection(selectionPoint, this.GrabSize, new[] { DetectableTypes.GOOBER, DetectableTypes.ENEMY });
        foreach (IDetectable heldObject in HeldObjects)
        {
            Vector3 pushForce = 
                GetCalculateDirectionForce(heldObject.GameObject.transform.position ,selectionPoint, throwSpeed * 2);
            
            heldObject.Rigidbody.linearVelocity += Vector3.up * 2;
            heldObject.Rigidbody.linearVelocity -= pushForce * trowMultiplier;
        }
    }
    
    private static Vector3 GetCalculateDirectionForce(Vector3 startVector, Vector3 endVector, float speed = 1)
    {
        float xDir = endVector.x - startVector.x;
        float zDir = endVector.z - startVector.z;
        Vector3 direction = new Vector3(xDir, 0, zDir);
        
        return direction *= (speed * Time.deltaTime);
    }
}
