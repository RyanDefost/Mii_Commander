using System;
using Grid;
using Managers;
using UnityEngine;

/// <summary>
/// Makes a board item move to a target position on the grid.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class GridMoveable : BoardItemComponent
{
    private GridManager grid;
    private MoveManager moveManagerRef;
    
    [Space]
    public bool canIgnoreLocks;
    public bool moveImmunity;
    
    protected GridManager.GridInstance Target { get; private set; }
    private GridManager.GridInstance oldTarget;
    
    [SerializeField]
    private float offset = 0.25f;
    [SerializeField]
    private float minimalDist = 1f;
    private float? previousDist;
    [SerializeField]
    private float minVelocityToRecalculate = 1f;
    
    protected Action onUpdate;

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
    
    private void FixedUpdate() => this.onUpdate?.Invoke();

    private void OnAddToHand()
    {
        SetMoving(false);
        this.onUpdate += LockLocalPosition;
    }

    private void OnInitiate()
    {
        SetMoving(true);
        this.onUpdate -= LockLocalPosition;
    }

    public void SetMoving(bool newState)
    {
        if (newState)
            TriggerMovementToTarget();
        else
            StopMovementToTarget();
    }

    private void LockLocalPosition() => this.transform.localPosition = Vector3.zero;

    protected virtual void StopMovementToTarget()
    {
        this.onUpdate -= SnapToTarget;
        this.onUpdate -= AwaitDistanceToTarget;
        ResetTarget();
        this.previousDist = null;
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
            // The item is moving away from its target
            if (dist > this.previousDist)
            {
                float currentSpeed = this.boardItem.Rb ? this.boardItem.Rb.linearVelocity.magnitude : 0f;

                // If its rolling past, get a new target
                if (currentSpeed > this.minVelocityToRecalculate)
                {
                    ResetTarget(); 
                    this.previousDist = null;
                    UpdateTarget(false);
                    return;
                }

                // edge case, off board or unmoving
                SetMoving(false);
                return;
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
        this.previousDist = null;
    }
    
    protected bool UpdateTarget(bool usesMove)
    {
        if (this.Target != null)
            return true;
        this.grid ??= ComponentRegistry.GetComponent<GridManager>();
        if (this.grid)
            SetTarget(this.grid.GetNearestPosition(this.transform.position, this.gameObject, this.boardItem, null, this.canIgnoreLocks), usesMove);
        else
            ComponentRegistry.TrySubscribeForComponent<GridManager>(TriggerUpdateTargetWithoutMove);
        return this.Target != null;
    }
    
    protected void TriggerUpdateTargetWithoutMove() => UpdateTarget(false);
    protected void TriggerUpdateTargetWithImmunityState() => UpdateTarget(!this.moveImmunity);
    
    public void SetTarget(GridManager.GridInstance newTarget, bool usesMove)
    {
        if (this.oldTarget != null && newTarget.index.isOffGrid == this.oldTarget.index.isOffGrid && newTarget.position == this.oldTarget.position)
        {
            this.Target = newTarget;
            return;
        }
        
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

    public void ApplyImpulse(Vector3 forceAway) => this.boardItem.Rb.AddForce(forceAway, ForceMode.Impulse);
}