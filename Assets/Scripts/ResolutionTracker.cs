using System;
using UnityEngine;

public class ResolutionTracker : MonoBehaviour
{
    public static Action<int, int> OnResolutionChanged;
    private static Vector2Int lastResolution;

    private void Start()
    {
        OnResolutionChanged ??= delegate { };
        CheckResolution();
    }

    private void Update() => CheckResolution();

    private void CheckResolution()
    {
        int currentWidth = Screen.width;
        int currentHeight = Screen.height;

        if (currentWidth == lastResolution.x && currentHeight == lastResolution.y) 
            return;

        OnResolutionChanged.Invoke(currentWidth, currentHeight);
        lastResolution = new Vector2Int(currentWidth, currentHeight);
    }
}