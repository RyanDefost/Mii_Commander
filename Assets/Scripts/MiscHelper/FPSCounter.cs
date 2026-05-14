using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// An FPS counter that Lucas stole from stack overflow, then connected to a Text component
/// </summary>
public class FPSCounter : MonoBehaviour
{
    [SerializeField] private Text counter;
    [SerializeField] private float updateInterval = 0.5f;
    
    private float accumulatedTime = 0f;
    private int frameCount = 0;

    private void OnValidate()
    {
        if (Application.isPlaying)
            return;
        this.counter = GetComponent<Text>();
    }

    private void Start() => this.counter.text = "";

    private void Update()
    {
        this.accumulatedTime += Time.unscaledDeltaTime;
        this.frameCount++;

        if (!(this.accumulatedTime >= this.updateInterval)) return;
        this.counter.text = $"FPS: {this.frameCount / this.accumulatedTime:F0}";
        this.accumulatedTime = 0f;
        this.frameCount = 0;
    }
}