using System.Linq;
using Managers;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Handles board item rotation, uses a player move
/// </summary>
public class GridRotatable : BoardItemComponent
{
    private MoveManager moveManagerRef;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float stepAngle = 9f;
    private float progression;
    private bool lockedRotation;
    private Vector3 lastRotation;
    private Vector3 rotation;
    
    public Vector3 StaticForwardDirection { get; private set; }

    private void Start() => this.moveManagerRef = ComponentRegistry.GetComponent<GameManager>()?.MoveManager;

    public override void ConnectToBoardItem()
    {
        this.boardItem.OnAddToHand += OnAddToHand;
        this.boardItem.OnAddToBoard += OnAddToBoard;
    }

    private void OnDestroy()
    {
        this.boardItem.OnAddToHand -= OnAddToHand;
        this.boardItem.OnAddToBoard -= OnAddToBoard;
    }

    /// <summary>
    /// Rotates the object clockwise along its Z-axis by a step.
    /// </summary>
    /// <param name="clockWiseProgression">pre deltaTime value that counts up, until a step is reached</param>
    public void RotateClockWise(float clockWiseProgression)
    {
        this.progression += clockWiseProgression * Time.deltaTime * this.rotationSpeed;
        if (!(Mathf.Abs(this.progression) >= this.stepAngle))
            return;
        
        float angle = -this.stepAngle * Mathf.Sign(this.progression);
        this.transform.Rotate(0, 0, angle, Space.World);
        this.rotation = this.transform.localEulerAngles;
            
        this.progression = 0;
    }

    /// <summary>
    /// Snaps the Z rotation to the nearest 90-degree cardinal direction
    /// </summary>
    private void SnapToNearestCardinalZ()
    {
        if (this.progression == 0)
            return;
        if (this.moveManagerRef.MoveAmount == 0)
            this.transform.localRotation = Quaternion.Euler(this.lastRotation);
        else
        {
            Vector3 currentAngles = this.transform.localEulerAngles;
            float snappedZ = Mathf.Round(currentAngles.z / 90f) * 90f;
            this.transform.localRotation = Quaternion.Euler(currentAngles.x, currentAngles.y, snappedZ);
        }

        this.progression = 0;
        this.moveManagerRef.SetMove();
        
        this.rotation = this.transform.localEulerAngles;
        this.StaticForwardDirection = GetForwardDirection(this.transform.localEulerAngles.z); 
        this.lockedRotation = true;
    }
    
    private void OnAddToHand()
    {
        this.boardItem.OnAddToBoard += OnAddToBoard;
        this.boardItem.OnRemovedFromHand += OnRemovedFromHand;
        // lock rotation
        this.lockedRotation = true;
        this.lastRotation = this.transform.localEulerAngles;
        this.rotation = this.lastRotation;
    }

    private void OnRemovedFromHand()
    {
        this.lockedRotation = false;
        this.boardItem.OnRemovedFromHand -= OnRemovedFromHand;
    }

    private void OnAddToBoard()
    {
        SnapToNearestCardinalZ();
        this.boardItem.OnAddToBoard -= OnAddToBoard;
    }

    private void Update()
    {
        if (this.lockedRotation)
            this.transform.rotation = Quaternion.Euler(this.rotation);
    }

    private static Vector3 GetForwardDirection(float rotation)
    {
        int[] directions = { 0, 90, 180, 270 };
        int nearest = directions.OrderBy(x => Mathf.Abs((long) x - rotation)).First();
        return nearest switch
        {
            0 => Vector3.up,
            90 => Vector3.left,
            180 => Vector3.down,
            270 => Vector3.right,
            var _ => Vector3.up
        };
    } 
}