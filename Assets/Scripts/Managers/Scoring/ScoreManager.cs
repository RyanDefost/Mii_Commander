using System;
using UnityEngine;

namespace Managers.Scoring
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private int startScore;
        [SerializeField] private int scoreGoal;
    
        private int score;

        public Action OnChangeScore;
        public Action OnReachedGoal;

        private void Awake() => this.score = startScore;

        public void AddScore(int score) => SetScore(this.score + score);
        public void RemoveScore(int score)  => SetScore(this.score - score);
        
        private void SetScore(int score)
        {
            this.score = score;
            OnChangeScore?.Invoke();

            if (this.score >= this.scoreGoal)
            {
                this.OnReachedGoal?.Invoke();
            }
        }

        private void ResetScore() => SetScore(0);
    
        public int GetScore() => this.score;
        public int GetGoal() => this.scoreGoal;
    }
}
