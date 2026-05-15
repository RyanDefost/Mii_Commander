using System;
using Grid;
using UnityEngine;

/// <summary>
/// Makes a board item move to a target position on the grid.
/// TODO: If the player is not allowed to move the piece it should snap back to previous target
/// </summary>
public class GridMoveable : BoardItemComponent
{
    private GridManager grid;
    protected GridManager.GridInstance Target { get; private set; }
    [SerializeField]
    private float offset = 0.25f;
    [SerializeField]
    private float minimalDist = 1f;
    private float? previousDist = null;
    protected Action onUpdate;

    private void Start()
    {
        this.grid = ComponentRegistry.GetComponent<GridManager>();
        SetMoving(true);
    }
    
    public override void ConnectToBoardItem()
    { 
        this.boardItem.OnAddToHand += OnAddToHand; 
        this.boardItem.OnInitiate += OnInitiate;
    }

    private void OnDestroy()
    {
        this.boardItem.OnAddToHand -= OnAddToHand; 
        this.boardItem.OnInitiate -= OnInitiate;
    }
    
    private void Update() => this.onUpdate?.Invoke();

    private void OnAddToHand() => SetMoving(false);
    private void OnInitiate() => SetMoving(true);

    public void SetMoving(bool newState)
    {
        if (newState)
            TriggerMovementToTarget();
        else
            StopMovementToTarget();
    }

    protected virtual void StopMovementToTarget()
    {
        this.onUpdate -= SnapToTarget;
        this.onUpdate -= AwaitDistanceToTarget;
        ResetTarget(); // TODO: logic for limited player moves here
    }

    protected virtual void TriggerMovementToTarget()
    {
        TriggerUpdateTarget();
        this.onUpdate += AwaitDistanceToTarget;
    }

    private void AwaitDistanceToTarget()
    {
        if (this.Target == null)
            return;
        float dist = Vector3.Distance(this.transform.position, this.Target.position);
        this.previousDist ??= dist;
        
        if (dist > this.minimalDist)
        {
            if (dist > this.previousDist)
            {
                ResetTarget();
                TriggerUpdateTarget();
                this.previousDist = null;
            }
            this.previousDist = dist;
            return;
        }
        this.onUpdate += SnapToTarget;
        this.onUpdate -= AwaitDistanceToTarget;
    }
    
    protected virtual void SnapToTarget()
    {
        if (this.Target == null)
            return;
        this.transform.position = GetTargetPosition();
    }
    
    protected bool UpdateTarget()
    {
        if (this.Target != null)
            return true;
        this.grid ??= ComponentRegistry.GetComponent<GridManager>();
        if (this.grid)
            SetTarget(this.grid.GetNearestPosition(this.transform.position, this.gameObject));
        else
            ComponentRegistry.TrySubscribeForComponent<GridManager>(TriggerUpdateTarget);
        return this.Target != null;
    }
    
    private void TriggerUpdateTarget() => UpdateTarget();
    
    public void SetTarget(GridManager.GridInstance newTarget) => this.Target = newTarget;
    
    public void ResetTarget()
    {
        if (this.Target == null)
            return;
        this.grid.ReleaseInstance(this.Target);
        this.Target = null;
    }

    public bool HasTarget() => this.Target != null;
    
    protected Vector3 GetTargetPosition() => this.Target.position + Vector3.back * this.offset;
}