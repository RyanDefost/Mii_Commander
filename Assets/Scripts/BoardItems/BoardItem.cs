using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Holds all board item events,
/// used to let board item specific components hook into logic
/// and represent a single toy/item
/// </summary>
public class BoardItem : MonoBehaviour
{
    [SerializeField]
    private Interactable interactable;
    [SerializeField, HideInInspector]
    private BoardItemComponent[] components;
    [SerializeField]
    private Rigidbody rb;
    public Rigidbody Rb { get => this.rb; private set => this.rb = value; }

    public Action OnAddToHand;
    public Action OnRemovedFromHand; 
    public Action OnInitiate;
    public Action OnAddToBoard;
    
    private void OnValidate()
    {
        if (Application.isPlaying)
            return;
        this.rb = GetComponent<Rigidbody>();
        this.interactable = GetComponentInChildren<Interactable>();
        
        this.components = GetComponentsInChildren<BoardItemComponent>();
    }

    private void Start()
    {
        foreach (BoardItemComponent boardItemComponent in this.components) 
            boardItemComponent.ConnectToBoardItem();
    }

    protected virtual void CustomOnValidate() {}
    
    /// <summary>Called when released and on the playing field</summary>
    public void Initiate() => this.OnInitiate?.Invoke();

    public T GetBoardComponent<T>() where T : BoardItemComponent
    {
        BoardItemComponent found = this.components.FirstOrDefault(c => c is T);
        if (!found) return null;
        return (T)found;
    }
}