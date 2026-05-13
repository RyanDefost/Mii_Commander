using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A simple class for hooking into interactions, player pickup etc
/// </summary>
public class Interactable : MonoBehaviour
{
    [SerializeField]
    private UnityEvent interaction;

    public void Trigger() => this.interaction.Invoke();
}
