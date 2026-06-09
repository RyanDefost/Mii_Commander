using System;
using UnityEngine;

namespace Scoring
{
    /// <summary>
    /// Keeps track of the current score and needed amount.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private int startScore = 0;
    
        private int score = 1;
        private int scoreGoal = 1;

        public Action<int> OnChangeScore;
        public Action OnReachedGoal;

        private void Awake() => this.score = this.startScore;

        /// <summary>
        /// Adds given value to score.
        /// </summary>
        /// <param name="score">added int value</param>
        /// <param name="id">candy id</param>
        public void AddScore(int score, int id) => SetScore(this.score + score, id);
        
        /// <summary>
        /// Removes given value from score.
        /// </summary>
        /// <param name="score">added int value</param>
        public void RemoveScore(int score)  => SetScore(this.score - score);
        
        private void SetScore(int score, int id = -1)
        {
            this.score = score;
            this.OnChangeScore?.Invoke(id);

            if (this.score >= this.scoreGoal)
            {
                this.OnReachedGoal?.Invoke();
            }
        }

        private void ResetScore() => SetScore(0);
    
        public int GetScore() => this.score;
        
        public int GetGoal() => this.scoreGoal;

        public void SetGoal(int goal)
        {
            this.scoreGoal = goal;
            SetScore(this.score);
        }
    }
}
