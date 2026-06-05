using BoardItems;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class MoveableSearchToggler : MonoBehaviour
{
    [SerializeField] private bool searchStatusToggle = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out GridMoveable gridMoveable))
        {
            gridMoveable.isActiveSearching = this.searchStatusToggle;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out GridMoveable gridMoveable))
        {
            gridMoveable.isActiveSearching = !this.searchStatusToggle;
        }
    }
}