using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseFollower : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask mask;
    
    [Header("Visuals")]
    [SerializeField] private Sprite renderSprite;

    public RaycastHit HitInfo { get; private set; }
    
    //Events
    public Event OnMouseEnter;
    public Event OnMouseOver;
    public Event OnMouseLeave;
    
    //Mouse Object
    private GameObject mouseObject;
    private SpriteRenderer renderComponent;
    private LineRenderer lineRenderer;
    private ObjectHolder objectHolder;
    private Camera mainCamera;
    
    public Vector2 MousePosition { get; private set; }

    private void OnEnable() => ToggleMouse();
    private void OnDisable() => ToggleMouse(false);

    private void OnValidate()
    {
        this.lineRenderer = GetComponent<LineRenderer>();
        this.objectHolder = GetComponent<ObjectHolder>();
        this.mainCamera = Camera.main;
    }

    private void Awake()
    {
        Color c1 = new Color(0.5f, 0.5f, 0.5f, 1);
        lineRenderer.SetColors(c1, c1);
        lineRenderer.SetWidth(0.25f, 0.25f);
        lineRenderer.SetVertexCount(16 + 1);
        lineRenderer.useWorldSpace = true;
    }

    private void Update()
    {
        MousePosition = Mouse.current.position.ReadValue();
        SetRay();
        
        //UpdateRay();
        UpdateVisuals();   
    }

    private void SetRay()
    {
        Ray rayPosition = mainCamera.ScreenPointToRay(MousePosition);
        Physics.Raycast(rayPosition, out RaycastHit hit, Mathf.Infinity, mask);
        this.HitInfo = hit;
    }
    
    public RaycastHit? TryGetHitInfo()
    {
        Ray rayPosition = mainCamera.ScreenPointToRay(MousePosition); 
        if (Physics.Raycast(rayPosition, out RaycastHit hit, Mathf.Infinity, mask))
        {
            Debug.Log("HITTING");
            
            HitInfo = hit;
            return hit;
        }
        
        return null;
    }

    private void UpdateVisuals()
    {
        this.mouseObject.transform.position = MousePosition;
        DoRenderer(16, this.objectHolder.GrabSize);
    }
    
    private void ToggleMouse(bool activeState = true)
    {
        if (mouseObject == null)
        {
            mouseObject = new GameObject("Mouse Object");
            
            this.renderComponent =  mouseObject.AddComponent<SpriteRenderer>();
            renderComponent.sprite = renderSprite;
        }
        
        mouseObject.SetActive(activeState);
    }

    public void DoRenderer(int numSegments, float radius)
    {
        float deltaTheta = (float)(2.0 * Mathf.PI) / numSegments;
        float theta = 0f;

        for (int i = 0; i < numSegments + 1; i++)
        {
            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);
            Vector3 pos = new Vector3(x, 0.1f, z);
            lineRenderer.SetPosition(i, pos + new Vector3(HitInfo.point.x, 0, HitInfo.point.z));
            theta += deltaTheta;
        }
    }
}
