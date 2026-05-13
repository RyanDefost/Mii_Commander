public class GridMoveable : BoardItemComponent
{
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

    protected virtual void DoDisable()
    {
        // throw new System.NotImplementedException();
    }

    protected virtual void DoEnable()
    {
        // throw new System.NotImplementedException();
    }
}