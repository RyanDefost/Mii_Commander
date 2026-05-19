using System;
using Grid;
using Managers;
using UnityEngine;

/// <summary>
/// Makes a board item move to a target position on the grid.
/// TODO: If the player is not allowed to move the piece it should snap back to previous target
/// </summary>
public class GridMoveable : BoardItemComponent
{
    private GridManager grid;
    private MoveManager moveManagerRef;
    protected GridManager.GridInstance Target { get; private set; }
    private GridManager.GridInstance oldTarget;
    [SerializeField]
    private float offset = 0.25f;
    [SerializeField]
    private float minimalDist = 1f;
    private float? previousDist;
    protected Action onUpdate;
    public bool moveImmunity;

    private void Start()
    {
        this.grid = ComponentRegistry.GetComponent<GridManager>();
        this.moveManagerRef = ComponentRegistry.GetComponent<GameManager>()?.MoveManager;
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
        TriggerUpdateTargetWithImmunityState();
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
                TriggerUpdateTargetWithoutMove();
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
        this.boardItem.OnAddToBoard?.Invoke();
    }
    
    protected bool UpdateTarget(bool usesMove)
    {
        if (this.Target != null)
            return true;
        this.grid ??= ComponentRegistry.GetComponent<GridManager>();
        if (this.grid)
            SetTarget(this.grid.GetNearestPosition(this.transform.position, this.gameObject), usesMove);
        else
            ComponentRegistry.TrySubscribeForComponent<GridManager>(TriggerUpdateTargetWithoutMove);
        return this.Target != null;
    }
    
    protected void TriggerUpdateTargetWithoutMove() => UpdateTarget(false);
    protected void TriggerUpdateTargetWithImmunityState() => UpdateTarget(!this.moveImmunity);
    
    public void SetTarget(GridManager.GridInstance newTarget, bool usesMove)
    {
        if (this.moveManagerRef.MoveAmount == 0 && usesMove)
        {
            this.grid.ReleaseInstance(newTarget);
            if (this.oldTarget == null)
                return;
            this.Target = this.oldTarget;
            this.grid.RegisterInGrid(this.Target);
        }
        else
            this.Target = newTarget;
        if (usesMove)
            this.moveManagerRef.SetMove();
    }

    public void ResetTarget()
    {
        if (this.Target == null)
            return;
        this.grid.ReleaseInstance(this.Target);
        this.oldTarget = this.Target;
        this.Target = null;
    }

    public bool HasTarget() => this.Target != null;
    
    protected Vector3 GetTargetPosition() => this.Target.position + Vector3.back * this.offset;
}