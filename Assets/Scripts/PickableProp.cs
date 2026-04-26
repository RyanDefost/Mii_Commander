using UnityEngine;

public class PickableProp : MonoBehaviour, IDetectable
{
    [SerializeField] private DetectableTypes type;
    public DetectableTypes DetectableType { get => type; set => type = value; }
    
    public GameObject GameObject => this.gameObject;
    public Rigidbody Rigidbody { get; private set; }
    
    private void Awake()
    {
        this.Rigidbody = GetComponent<Rigidbody>();
    }
    
    public void OnHold()
    {
        Debug.Log("OnHold");
        
        this.transform.position += Vector3.up;
        this.Rigidbody.useGravity = false;
    }

    public void OnReleaseHold()
    {
        Debug.Log("OnReleaseHold");
        this.Rigidbody.useGravity = true;
    }
}
