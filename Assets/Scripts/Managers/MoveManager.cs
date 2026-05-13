using System;
using HelperStructs;
using UnityEngine;

namespace Managers
{
    public class MoveManager : MonoBehaviour
    {
        public int InitialMoveAmount = 0;
        [SerializeField] private Bounds<int, int> clampAmount =  new(0, 30);

        public int MoveAmount { get; private set; }
        
        public Action OnSetMove;
        public Action OnUndoMove;
        public Action OnChanged;
        public Action OnLastMove;

        private void Start()
        {
            SetMoveAmount(InitialMoveAmount);
        }

        public void SetMove()
        {
            OnSetMove?.Invoke();
            SetMoveAmount(this.MoveAmount -1);
            
            if(MoveAmount == 0) OnLastMove?.Invoke();
        }

        public void UndoMove()
        {
            OnUndoMove?.Invoke();
            SetMoveAmount(this.MoveAmount +1);
        }

        private void SetMoveAmount(int moveAmount)
        {
            Mathf.Clamp(moveAmount, clampAmount.min, clampAmount.max);
            this.MoveAmount = moveAmount;
            
            OnChanged?.Invoke();
        }

        public void ResetMoveAmount() => SetMoveAmount(InitialMoveAmount);
    }
}