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

    public bool isActiveSearching = true;
    public bool moveImmunity;
    [Space]
    public string startCellName = "start";
    protected GridManager.GridInstance Target { get; private set; }
    private GridManager.GridInstance oldTarget;
    
    [Space, SerializeField]
    private float offset = 0.25f;
    [SerializeField]
    private float minimalDist = 1f;
    private float? previousDist;
    [SerializeField]
    private float minVelocityToRecalculate = 1f;
    
    protected Action onUpdate;
    protected Action onFixedUpdate;

    public Action OnMoved;
    public Action OnStartMoving;
    private bool hasMoved;
    
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
    private void FixedUpdate() => this.onFixedUpdate?.Invoke();

    private void OnAddToHand()
    {
        SetMoving(false);
        this.onFixedUpdate += LockLocalPosition;
    }

    private void OnInitiate()
    {
        SetMoving(true);
        this.onUpdate += CheckForSearch;
        this.onFixedUpdate -= LockLocalPosition;
    }

    private void CheckForSearch()
    {
        if(!this.isActiveSearching) return;
        
        SetMoving(true);
        this.onUpdate -= CheckForSearch;
    }

    public void SetMoving(bool newState)
    {
        this.OnStartMoving?.Invoke();
        this.hasMoved = false;
        
        if (newState)
            TriggerMovementToTarget();
        else
            StopMovementToTarget();
    }

    private void LockLocalPosition() => this.transform.localPosition = Vector3.zero;

    protected virtual void StopMovementToTarget()
    {
        this.onUpdate -= SnapToTarget;
        this.onFixedUpdate -= AwaitDistanceToTarget;
        ResetTarget();
        this.previousDist = null;
    }

    protected virtual void TriggerMovementToTarget()
    {
        TriggerUpdateTargetWithImmunityState();
        this.onFixedUpdate += AwaitDistanceToTarget;
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
        this.onFixedUpdate -= AwaitDistanceToTarget;
    }
    
    protected virtual void SnapToTarget()
    {
        if (this.Target == null)
            return;

        
        this.transform.position = GetTargetPosition();
        this.boardItem.OnAddToBoard?.Invoke();
        this.previousDist = null;
        
        if (this.hasMoved) 
            return;
        
        this.hasMoved = true;
        this.OnMoved?.Invoke();
    }
    
    protected bool UpdateTarget(bool usesMove)
    {
        if (this.Target != null)
            return true;
        this.grid ??= ComponentRegistry.GetComponent<GridManager>();
        if (this.grid)
            SetTarget(this.grid.GetNearestCellPassPosition(this.startCellName , this.transform.position, this.gameObject, this.boardItem), usesMove);
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