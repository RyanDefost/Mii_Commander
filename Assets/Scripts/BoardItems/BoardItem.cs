using System;
using System.Collections.Generic;
using System.Linq;
using Grid;
using UnityEngine;

/// <summary>
/// Holds all board item events,
/// used to let board item specific components hook into logic
/// and represent a single toy/item
/// </summary>
public class BoardItem : MonoBehaviour
{
    public GridManager.GridInstance gridInstanceRef;
    [SerializeField, HideInInspector]
    private BoardItemComponent[] components;
    [SerializeField]
    private Rigidbody rb;
    public Rigidbody Rb { get => this.rb; private set => this.rb = value; }

    public Action OnAddToHand;
    public Action OnRemovedFromHand; 
    public Action OnInitiate;
    public Action OnAddToBoard;
    public Action OnActivate;
    public Action<BoardItem> OnStarted;
    
    private void Awake()
    {
        this.rb = GetComponent<Rigidbody>();
        this.components = GetComponentsInChildren<BoardItemComponent>();
    }

    private void Start()
    {
        foreach (BoardItemComponent boardItemComponent in this.components)
            boardItemComponent.ConnectToBoardItem();
        
        OnStarted?.Invoke(this);
    }

    /// <summary>Called when released and on the playing field</summary>
    public void Initiate() => this.OnInitiate?.Invoke();
    
    /// <summary>Called when object should activate its ability, explode, push, pull, collect</summary>
    public void Activate() => this.OnActivate?.Invoke();

    public T GetBoardComponent<T>() where T : BoardItemComponent
    {
        BoardItemComponent found = this.components.FirstOrDefault(c => c is T);
        if (!found) return null;
        return (T)found;
    }
}