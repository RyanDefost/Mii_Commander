using System;
using HelperStructs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        private void Start()
        {
            OnChanged += SetUI;
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
        
        public void ResetMoveAmount() => SetMoveAmount(initialMoveAmount);

        public void SetInitialMoveAmount(int amount)
        {
            initialMoveAmount = amount;
            SetMoveAmount(initialMoveAmount);
        }
        
        private void SetMoveAmount(int moveAmount)
        {
            this.MoveAmount =Mathf.Clamp(moveAmount, clampAmount.min, clampAmount.max);
            OnChanged?.Invoke();
        }

        private void SetUI() => moveAmountText.text = $"{MoveAmount} : Moves";
        
    }
}