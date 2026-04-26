using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float currentTime = 0;
    public bool timeIsRunning = false;
    public TextMeshProUGUI timerText;

    void Update()
    {
        if(!timeIsRunning) return;
        
        currentTime += Time.deltaTime;
        displayTime();
    }

    void displayTime()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("[Time Survived] {0:00}:{1:00}", minutes, seconds);
    }
}