using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers.Scoring
{
    public class ScoreVisualiser: MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI text;
        
        private GameManager gameManager;
        private ScoreManager scoreManager;
        
        [Header("Slider Settings")]
        [Range(0.1f,5f)][SerializeField] private float applySpeed = 2.5f;
        
        private Coroutine runningRoutine;
        
        private void Start()
        {
            this.gameManager = ComponentRegistry.GetComponent<GameManager>();
            this.scoreManager = this.gameManager.ScoreManager;
            this.scoreManager.OnChangeScore += SetVisuals;
            
            SetVisuals();
        }
        
        private void SetVisuals()
        {
            int score = scoreManager.GetScore();
            int goal = scoreManager.GetGoal();
            float fractionValue = CalculateFractionValue(score, goal);
            
            this.text.text = score.ToString();
            
            if(this.runningRoutine != null) StopCoroutine(this.runningRoutine);
            this.runningRoutine = StartCoroutine(GradualApply(fractionValue));
        }

        private IEnumerator GradualApply(float finalValue)
        {
            float startValue = this.slider.value;
            for (float i = 0; i < 1; i += (applySpeed * Time.deltaTime))
            {
                this.slider.value = Mathf.Lerp(startValue, finalValue, i);  
                yield return new WaitForSeconds(Time.deltaTime);
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