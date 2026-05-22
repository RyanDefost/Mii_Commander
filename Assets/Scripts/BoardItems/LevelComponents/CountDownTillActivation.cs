using System;
using Grid;
using Managers;
using UnityEngine;

public class CountDownTillActivation : BoardItemComponent, IUserInterfaceValueGetter
{
    [SerializeField] private BoardItemComponent toActivate;
    [SerializeField] private int count = 1;
    private IBoardItemDisablable currentToActivate;
    private int currentCount;
    private Action<object, Type> countChanged;

    protected override void CustomOnValidate()
    {
        base.CustomOnValidate();
        if (!this.toActivate || this.toActivate is IBoardItemDisablable) return;
        this.toActivate = null;
        Debug.LogError($"{GetType()}-{GetInstanceID()}: toActivate needs to be {nameof(IBoardItemDisablable)}");
        
        this.enabled = this.toActivate;
    }

    private void Awake()
    {
        this.currentToActivate = (IBoardItemDisablable)this.toActivate;
        this.currentToActivate.DisableActivate();
    }

    public override void ConnectToBoardItem()
    {
        this.boardItem.OnActivate += Activate;
        this.currentCount = this.count;
        this.countChanged.Invoke(this.currentCount, typeof(int));
    }

    private void OnDestroy() => this.boardItem.OnActivate -= Activate;

    private void Activate()
    {
        if (!this.enabled)
            return;
        this.currentCount--;
        this.countChanged.Invoke(this.currentCount, typeof(int));

        if (this.currentCount < 1)
        {
            this.enabled = false;
            return;        
        }
        if (this.currentCount > 1) return;
        
        this.currentToActivate.EnableActivate();
    }

    public void Reset()
    {
        this.enabled = true;
        this.currentCount = this.count;
        this.countChanged.Invoke(this.currentCount, typeof(int));
        this.currentToActivate.DisableActivate();
    }

    public object GetValue(string valueName, out Type returnType)
    {
        if (valueName == "count")
        {
            returnType = typeof(int);
            return this.currentCount;
        }

        returnType = null;
        return null;
    }

    public void SubscribeOnChangeValue(string valueName, Action<object, Type> callback)
    {
        if (valueName == "count") this.countChanged += callback;
    }

    public void UnSubscribeOnChangeValue(string valueName, Action<object, Type> callback)
    {
        if (valueName == "count") this.countChanged -= callback;
    }
}