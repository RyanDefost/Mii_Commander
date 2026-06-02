using System.Collections.Generic;
using Managers;
using PlayerHand;
using Scoring;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class EndScreen : MonoBehaviour
{
    private PlayerHandManager playerHandManager;
    private GameManager gameManager;
    private ScoreManager scoreManager;
    
    [SerializeField]private GameObject EndScreenPanel;
    [SerializeField]private GameObject FinishScreenPanel;
    
    [FormerlySerializedAs("scoreText")]
    [Header("UI Elements")]
    [SerializeField] private List<TextMeshPro> scoreTexts;
    [SerializeField] private TextMeshPro winStateText;

    private void Start()
    {
        this.playerHandManager ??= ComponentRegistry.GetComponent<PlayerHandManager>();
        this.gameManager ??= ComponentRegistry.GetComponent<GameManager>();
        this.scoreManager ??= this.gameManager.ScoreManager;
        
        EndScreenPanel.SetActive(false);
        FinishScreenPanel.SetActive(false);
        
        this.gameManager.ScoreManager.OnReachedGoal += GoalAchieved;
        this.gameManager.OnGameEnd += Activate;
    }

    private void Activate()
    {
        foreach (TextMeshPro text in scoreTexts)
        {
            SetScore(this.scoreManager.GetScore(), this.scoreManager.GetGoal(), text, winStateText);
        }
        EndScreenPanel.SetActive(true);
        
        this.gameManager.OnGameEnd -= Activate;
    }

    private void GoalAchieved()
    {
        foreach (TextMeshPro text in scoreTexts)
        {
            SetScore(this.scoreManager.GetScore(), this.scoreManager.GetGoal(), text, winStateText);
        }
        FinishScreenPanel.SetActive(true);
        playerHandManager.SetCanGrab(false);
        
        this.gameManager.ScoreManager.OnReachedGoal -= GoalAchieved;
    }
    
    private static void SetScore(int score, int minScore, TextMeshPro textElement, TextMeshPro winTextElement)
    {
        textElement.text = $"Score\n{score}";
        winTextElement.text = score > minScore ? "You win!" : "You lose!";
    }

    public void Continue()
    {
        this.FinishScreenPanel.SetActive(false);
        playerHandManager.SetCanGrab(true);
        
        this.gameManager.ScoreManager.OnReachedGoal -= GoalAchieved;
    }
    
    public static void Restart() => SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    public static void Quit() => Application.Quit();
}
