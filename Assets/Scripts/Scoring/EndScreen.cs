using System;
using Grid;
using Managers;
using Scoring;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    private GameManager gameManager;
    private ScoreManager scoreManager;
    
    [SerializeField]private GameObject childPanel;
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshPro scoreText;
    [SerializeField] private TextMeshPro winStateText;
    

    private void Start()
    {
        this.gameManager ??= ComponentRegistry.GetComponent<GameManager>();
        this.scoreManager ??= this.gameManager.ScoreManager;
        
        childPanel.SetActive(false);
        this.gameManager.OnGameEnd += Activate;
    }

    private void Activate()
    {
        SetScore(this.scoreManager.GetScore(), this.scoreManager.GetGoal(), scoreText, winStateText);
        childPanel.SetActive(true);   
    }
    
    private static void SetScore(int score, int minScore, TextMeshPro textElement, TextMeshPro winTextElement)
    {
        textElement.text = $"Score\n{score}";
        winTextElement.text = score > minScore ? "You win!" : "You lose!";
    }

    public static void Restart() => SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    public static void Quit() => Application.Quit();
}
