using System;
using UnityEngine;

/// <summary>
/// Handles board item rotation, uses a player move
/// </summary>
public class GridRotatable : BoardItemComponent
{
    [SerializeField] private HandHandler handHandler;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float stepAngle = 9f;
    private float progression;
    private bool lockedRotation;
    private Vector3 lastRotation;

    protected override void CustomOnValidate()
    {
        base.CustomOnValidate();
        this.handHandler = GetComponent<HandHandler>();
    }

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
        this.lastRotation = this.transform.localEulerAngles;
            
        this.progression = 0;
    }

    /// <summary>
    /// Snaps the Z rotation to the nearest 90-degree cardinal direction
    /// </summary>
    private void SnapToNearestCardinalZ()
    {
        Vector3 currentAngles = this.transform.localEulerAngles;
        float snappedZ = Mathf.Round(currentAngles.z / 90f) * 90f;
        this.transform.localRotation = Quaternion.Euler(currentAngles.x, currentAngles.y, snappedZ);

        this.progression = 0;
    }
    
    private void OnAddToHand()
    {
        this.handHandler.OnHandGrabRelease += OnGrabReleased;
        // lock rotation
        this.lockedRotation = true;
        this.lastRotation = this.transform.localEulerAngles;
    }

    private void OnGrabReleased()
    {
        this.lockedRotation = false;
        this.handHandler.OnHandGrabRelease -= OnGrabReleased;
    }

    private void OnAddToBoard() => SnapToNearestCardinalZ();

    private void Update()
    {
        if (this.lockedRotation)
            this.transform.rotation = Quaternion.Euler(this.lastRotation);
    }
}