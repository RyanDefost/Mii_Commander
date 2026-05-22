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
                if (instance == null || !instance.gameObj)
                    return;

                GridMoveable moveComponent = instance.gameObj.GetComponent<GridMoveable>();
                if (!moveComponent)
                    return;
                moveComponent.ResetTarget();
                moveComponent.SetTarget(this.gridManager.GetNearestPosition(
                    instance.position + this.gridManager.CellSize.y * Vector3.down, instance.gameObj, instance));

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