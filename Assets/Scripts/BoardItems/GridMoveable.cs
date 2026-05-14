using Grid;

/// <summary>
/// Makes it so that a board item gets to move to another place on the board
/// TODO: If the player is not allowed to move the piece it should snap back to previous target
/// </summary>
public class GridMoveable : BoardItemComponent
{
    private GridManager grid;
    protected GridManager.GridInstance Target { get; private set; }

    private void Start()
    {
        this.grid = ComponentRegistry.GetComponent<GridManager>();
        SetEnabled(true);
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

    private void OnAddToHand() => SetEnabled(false);
    private void OnInitiate() => SetEnabled(true);

    public void SetEnabled(bool newState)
    {
        if (newState)
            DoEnable();
        else
            DoDisable();
    }

    protected virtual void DoDisable() {}
    protected virtual void DoEnable() {}
    
    protected bool GetTarget()
    {
        if (this.Target != null)
            return true;
        this.grid ??= ComponentRegistry.GetComponent<GridManager>();
        SetTarget(this.grid.GetNearestPosition(this.transform.position, this.gameObject));
        return this.Target != null;
    }
    
    public void SetTarget(GridManager.GridInstance newTarget) => this.Target = newTarget;
    
    public void ResetTarget()
    {
        if (this.Target == null)
            return;
        this.grid.ReleaseInstance(this.Target);
        this.Target = null;
    }

    public bool HasTarget() => this.Target != null;
}