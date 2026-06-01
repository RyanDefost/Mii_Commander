using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A simple class for hooking into interactions, player pickup etc
/// </summary>
public class Interactable : MonoBehaviour
{
    public BoardItem boardItem;
    public bool interactionOverride;
    [SerializeField]
    private UnityEvent interaction;

    public void Trigger(bool canInteract)
    {
        if(!canInteract && !interactionOverride) return;
        this.interaction?.Invoke();  
    } 

    public bool HasBoardItemParent() => this.boardItem;
}
