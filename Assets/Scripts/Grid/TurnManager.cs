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
        
        [SerializeField] float waitTimeToDestroy = 4;
        private Timer TimerToDestroy;
        [SerializeField] float waitTimeAfterMoving = 2;
        private Timer TimeAfterMoving;
        
        private List<GridMoveable> waitingMoves =  new();
        private bool waitToDestroy = true;
        private bool isDestroying;
        
        public Action<GridMoveable> OnEndReached;
        
        public Action OnWaitingToMove;
        public Action OnDoneMoving;
        
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

        private void Update()
        {
            this.waitTimer.UpdateTime(Time.deltaTime);

            this.TimerToDestroy?.UpdateTime(Time.deltaTime);
            this.TimeAfterMoving?.UpdateTime(Time.deltaTime);
        } 

        private void MoveAllItemsDown()
        {
            this.waitingMoves.Clear();
            this.gridManager.ForAllGridItems(instance =>
            {
                if (instance == null || !instance.gameObj) return;

                if(this.gridManager.CheckPositionLocked(instance.position)) return;
                
                GridMoveable moveComponent = instance.moveable;
                if (!moveComponent) return;
                
                moveComponent.SetMoving(false);
                
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
                    this.isDestroying = true;
                    return;
                }
                
                waitingMoves.Add(moveComponent);
            });

            if (this.isDestroying && this.waitToDestroy)
            {
                this.isDestroying = false;
                this.OnWaitingToMove?.Invoke();
                
                this.TimerToDestroy = new Timer(waitTimeToDestroy, false, true, Moving);
                return;
            }
            Moving();
        }

        private void Moving()
        {
            foreach (GridMoveable moveable in this.waitingMoves)
                moveable.SetMoving(true);
            
            this.TimeAfterMoving = new Timer(waitTimeAfterMoving, false, true, OnStopMoving);
        }

        private void OnStopMoving() => this.OnDoneMoving?.Invoke();
        
        public void AddToWaitTime(float time) => this.waitTime += time;
        public void SetWaitOnDestroy(bool wait) => this.waitToDestroy = wait;
    }
}