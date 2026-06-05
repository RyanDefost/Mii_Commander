using System;
using Grid;
using PlayerHand;
using UnityEngine;

namespace HelperStructs
{
    [RequireComponent(typeof(BoxCollider))]
    public class DiscardPile : MonoBehaviour
    {
        private GridManager gridManager;
        private PlayerHandManager handManager;

        private void Start()
        {
            this.gridManager ??= ComponentRegistry.GetComponent<GridManager>();
            this.handManager ??= ComponentRegistry.GetComponent<PlayerHandManager>();
        }

        private void OnTriggerEnter(Collider other)
        {
            GameObject obj = other.gameObject;
            
            if (!obj.TryGetComponent(out BoardItem boardItem) ||
                obj.GetComponentInParent<PlayerHandManager>()) return;
            
            if(boardItem.gridInstanceRef != null)
                this.gridManager.ReleaseInstance(boardItem.gridInstanceRef);
            Destroy(obj);
        }
    }
}