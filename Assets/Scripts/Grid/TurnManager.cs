using System;
using System.Collections;
using System.Collections.Generic;
using BoardItems;
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
        private float waitTime;
        private Timer waitTimer;

        private List<GridMoveable> waitingMoves =  new();
        private bool isDestroying = false;
        
        public Action<GridMoveable> OnEndReached;
        
        private void OnValidate()
        {
            if (Application.isPlaying)
                return;
            this.gridManager = GetComponent<GridManager>();
            this.enabled = this.gridManager;
        }

        private void Start() => this.waitTimer = new Timer(1f, false, false, MoveAllItemsDown);

        /// <summary>Moves all relevant items downwards</summary>
        public void NextTurn()
        {
            this.waitTime = 0;
            this.gridManager.ForAllGridItems(instance =>
            {
                if (instance == null || !instance.gameObj) return;
                instance.boardItem?.Activate();
            });

            this.waitTimer.ResetWaitTime(this.waitTime);
            this.waitTimer.ResetAndReplay();
        }

        private void Update() => this.waitTimer.UpdateTime(Time.deltaTime);

        private void MoveAllItemsDown()
        {
            this.gridManager.ForAllGridItems(instance =>
            {
                if (instance == null || !instance.gameObj) return;

                if(this.gridManager.CheckPositionLocked(instance.position)) return;
                
                GridMoveable moveComponent = instance.moveable;
                if (!moveComponent) return;
                
                moveComponent.ResetTarget();
                GridManager.GridInstance newTarget = this.gridManager.GetNearestPosition(
                    instance.position + this.gridManager.CellSize.y * Vector3.down, instance.gameObj,
                    moveComponent.GetBoardItem(), instance); // TODO maybe replace this with a neighborpattern?
                
                
                if (newTarget != null)
                    moveComponent.SetTarget(newTarget, false);

                if (!moveComponent.HasTarget())
                {
                    this.gridManager.ReleaseInstance(instance);
                    this.OnEndReached?.Invoke(moveComponent);
                    
                    Destroy(moveComponent.gameObject, 5f);
                    moveComponent.SetMoving(false);
                    this.isDestroying = true;
                    return;
                }
                
                waitingMoves.Add(moveComponent);
            });

            StartCoroutine(SetMoving());
        }

        private IEnumerator SetMoving()
        {
            if (this.isDestroying)
            {
                print("Is destroying"); // THERE IS MORE LOGIC FOR MOVING THAT IS DONE BEFORE THIS
                yield return new WaitForSeconds(10f); // SO THE STILL MOVE DOWN.
            }
            
            print("Is moving");
            foreach (var moveable in waitingMoves)
            {
                moveable.SetMoving(true);
            }
            
            this.waitingMoves.Clear();
            this.isDestroying = false;
        }
        

        public void AddToWaitTime(float time) => this.waitTime += time;
    }
}