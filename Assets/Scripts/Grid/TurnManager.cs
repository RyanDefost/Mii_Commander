using System;
using UnityEngine;

namespace Grid
{
    
    /// <summary>
    /// Handles turns on the grid, moving anything on the grid down
    /// </summary>
    [RequireComponent(typeof(GridManager))]
    public class TurnManager : MonoBehaviour
    {
        [SerializeField]
        private GridManager gridManager;

        public Action<GridMoveable> OnreachedEnd;
        
        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
            this.gridManager = GetComponent<GridManager>();
            this.enabled = this.gridManager;
        }

        /// <summary>Moves all relevant items downwards</summary>
        public void NextTurn()
        {
            this.gridManager.ForAllGridItems(instance =>
            {
                if (instance == null || !instance.boardItem)
                    return;

                GridMoveable moveComponent = instance.boardItem.gameObject.GetComponent<GridMoveable>();
                if (!moveComponent)
                    return;
                moveComponent.ResetTarget();
                moveComponent.SetTarget(this.gridManager.GetNearestPosition(
                    instance.position + this.gridManager.CellSize.y * Vector3.down, instance.boardItem, instance));

                if (!moveComponent.HasTarget())
                {
                    this.gridManager.ReleaseInstance(instance);
                    OnreachedEnd?.Invoke(moveComponent);
                    
                    Destroy(moveComponent.gameObject);
                }
                moveComponent.SetMoving(true);
            });
        }
    }
}