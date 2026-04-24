using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public int FramesPerSec { get; protected set; }

    [SerializeField] private float frequency = 0.5f;
    [SerializeField] private Text counter;

    private void OnValidate()
    {
        if (Application.isPlaying)
            return;
        this.counter = GetComponent<Text>();
    }

    private void Start()
    {
        this.counter.text = "";
        StartCoroutine(FPS());
    }

    private IEnumerator FPS()
    {
        for (; ; )
        {
            int lastFrameCount = Time.frameCount;
            float lastTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(this.frequency);

            float timeSpan = Time.realtimeSinceStartup - lastTime;
            int frameCount = Time.frameCount - lastFrameCount;

            this.FramesPerSec = Mathf.RoundToInt(frameCount / timeSpan);
            this.counter.text = "FPS: " + this.FramesPerSec.ToString();
        }
    }
}