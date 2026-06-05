using BoardItems;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class MoveDeactivator : MonoBehaviour
{
    [SerializeField] private Vector3 pushDirection = Vector3.forward;
    private void OnTriggerStay(Collider other)
    {
        GameObject obj = other.gameObject;
            
        if (!obj.TryGetComponent(out GridMoveable movable)) return;
        movable.ApplyImpulse(this.pushDirection);
    }
}