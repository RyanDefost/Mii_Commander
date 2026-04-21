using System;
using UnityEngine;

public class AspectRatioTracker : MonoBehaviour
{
    public static Action<float> OnAspectRatioChanged;
    private static float lastAspectRatio;

    private void Start()
    {
        OnAspectRatioChanged ??= delegate { };
        CheckAspectRatio();
    }

    private void FixedUpdate()
    {
        CheckAspectRatio();
    }
    
    private void CheckAspectRatio()
    {
        float currentAspectRatio = (float)Screen.width / Screen.height;

        if (Mathf.Approximately(currentAspectRatio, lastAspectRatio)) return;
        OnAspectRatioChanged.Invoke(currentAspectRatio);
        lastAspectRatio = currentAspectRatio;
    }
}