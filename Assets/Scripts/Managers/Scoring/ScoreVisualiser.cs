using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Managers.Scoring
{
    public class ScoreVisualiser: MonoBehaviour
    {
        [SerializeField] private Slider slider;
        
        private GameManager gameManager;
        private ScoreManager scoreManager;
        
        [Header("Slider Settings")]
        [SerializeField] private float applySpeed = 0.5f;
        [SerializeField] private float lerpTime = 0.5f;
        private Coroutine runningRoutine;
        
        int interpolationFramesCount = 300; 
        int elapsedFrames = 0;
        
        private void Start()
        {
            this.gameManager = ComponentRegistry.GetComponent<GameManager>();
            this.scoreManager = this.gameManager.ScoreManager;
            this.scoreManager.OnChangeScore += SetVisuals;
        }

        private void InitVisual(Slider slider)
        {
            slider.minValue = 0;
            slider.maxValue = scoreManager.GetGoal();
        }
        
        private void SetVisuals()
        {
            int score = scoreManager.GetScore();
            int goal = scoreManager.GetGoal();
            float fractionValue = CalculateFractionValue(score, goal);
            
            if(this.runningRoutine != null) StopCoroutine(this.runningRoutine);
            this.runningRoutine = StartCoroutine(GradualApply(fractionValue));
        }

        private IEnumerator GradualApply(float finalValue)
        {
            while (!Mathf.Approximately(this.slider.value, finalValue))
            {
                Debug.Log("Gradually applied " + finalValue);
                float interpolationRatio = (float)elapsedFrames / interpolationFramesCount;
                this.slider.value = Mathf.Lerp(this.slider.value, finalValue, interpolationRatio);
                
                yield return new WaitForSeconds(applySpeed);
            }
            
            this.slider.value = finalValue;
        }

        private float CalculateFractionValue(float value, float maxValue)
        {
            float percentage = (value / maxValue) * 100;
            float fractionValue = percentage / 100;
            
            return fractionValue;
        }
    }
}