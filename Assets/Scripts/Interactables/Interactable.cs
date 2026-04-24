using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField]
    private UnityEvent interaction;

    public void Trigger() => this.interaction.Invoke();
}
