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
                if (instance == null || !instance.gameObj) return;
                instance.boardItem?.Activate();
            });

            this.gridManager.ForAllGridItems(instance =>
            {
                if (instance == null || !instance.gameObj) return;

                GridMoveable moveComponent = instance.moveable;
                if (!moveComponent) return;
                
                moveComponent.ResetTarget();
                GridManager.GridInstance newTarget = this.gridManager.GetNearestPosition(
                    instance.position + this.gridManager.CellSize.y * Vector3.down, instance.gameObj,
                    moveComponent.GetBoardItem(), instance);
                
                if (newTarget != null)
                    moveComponent.SetTarget(newTarget, false);

                if (!moveComponent.HasTarget())
                {
                    this.gridManager.ReleaseInstance(instance);
                    this.OnreachedEnd?.Invoke(moveComponent);
                    
                    Destroy(moveComponent.gameObject);
                    return;
                }
                moveComponent.SetMoving(true);
            });
        }
    }
}