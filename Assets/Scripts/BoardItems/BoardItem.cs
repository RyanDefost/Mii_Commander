using System;
using UnityEngine;
using Random = UnityEngine.Random;

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
    public Action OnInitiate;
    
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
}