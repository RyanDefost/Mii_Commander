using System;
using TMPro;
using UnityEngine;

namespace Managers
{
    public class MoveManager : MonoBehaviour
    {
        [SerializeField] private Bounds<int, int> clampAmount =  new(0, 30);
        private int initialMoveAmount = 0;
        
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI moveAmountText;
        
        public int MoveAmount { get; private set; }
        
        public Action OnSetMove;
        public Action OnUndoMove;
        public Action OnChanged;
        public Action OnLastMove;

        private void Start() => this.OnChanged += SetUI;

        public void SetMove()
        {
            this.OnSetMove?.Invoke();
            SetMoveAmount(this.MoveAmount -1);
            
            if(this.MoveAmount == 0) this.OnLastMove?.Invoke();
        }

        public void UndoMove()
        {
            this.OnUndoMove?.Invoke();
            SetMoveAmount(this.MoveAmount +1);
        }
        
        public void ResetMoveAmount() => SetMoveAmount(this.initialMoveAmount);

        public void SetInitialMoveAmount(int amount)
        {
            this.initialMoveAmount = amount;
            SetMoveAmount(this.initialMoveAmount);
        }
        
        private void SetMoveAmount(int moveAmount)
        {
            this.MoveAmount = Mathf.Clamp(moveAmount, this.clampAmount.min, this.clampAmount.max);
            this.OnChanged?.Invoke();
        }

        public void AddMoveAmount(int amount)
        {
            this.MoveAmount = Mathf.Clamp(this.MoveAmount + amount, this.clampAmount.min, this.clampAmount.max);
            this.OnChanged?.Invoke();
        }

        private void SetUI() => this.moveAmountText.text = $"{this.MoveAmount} : Moves";
    }
}