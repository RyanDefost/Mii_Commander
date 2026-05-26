using System;

/// <summary>
/// A simple timer that sends out events for timer based functions
/// </summary>
public class Timer
{
    private double currentTime;
    private float maxTime;
    private bool isRunning;
    private bool shouldLoop;
    
    public bool IsRunning { get => this.isRunning; private set => this.isRunning = value; }
    public float MaxTime { get => this.maxTime; private set => this.maxTime = value; }
    public float CurrentTime { get => (float)this.currentTime; private set => this.currentTime = value; }

    private Action onEnd;
    private Action<double> onPlaying; // Double passed is current time

    /// <param name="maxTime">time until the timer is finished</param>
    /// <param name="shouldLoop">Dictates if the timer should restart and replay after reaching the end</param>
    /// <param name="playOnConstruction">Dictates if the Timer should be running after creation</param>
    /// <param name="onEnd">Action called at the end of the timer</param>
    /// <param name="onPlaying">Action called during every timer update (if its playing) takes the parameter currentTime</param>
    public Timer(float maxTime, bool shouldLoop = false, bool playOnConstruction = true, Action onEnd = null, Action<double> onPlaying = null)
    {
        this.maxTime = maxTime;
        this.onEnd = onEnd;
        this.onPlaying = onPlaying;
        this.shouldLoop = shouldLoop;
        if (playOnConstruction)
            Play();
    }

    /// <summary>
    /// Brings the timer's current time forward until finished
    /// </summary>
    /// <param name="dt">Delta time, time in between frames</param>
    public void UpdateTime(double dt)
    {
        if (!this.isRunning)
            return;
        this.currentTime += dt;
        this.onPlaying?.Invoke(this.currentTime);
        CheckIfEndIsReached();
    }

    private void CheckIfEndIsReached()
    {
        if (!(this.currentTime >= this.maxTime)) return;
        this.onEnd?.Invoke();
        Stop();
        if (this.shouldLoop)
            ResetAndReplay();
    }
    
    public void Reset() => this.currentTime = 0;
    public void ResetAndReplay()
    {
        Reset();
        Play();
    }
    
    public void Stop() => this.isRunning = false;
    public void Play() => this.isRunning = true;

    public void AddToMaxTime(float time) => this.maxTime += time;

    public void RegisterOnEndListener(Action listener)
    {
        if (listener == null) return;
        this.onEnd += listener;
    }
    public void UnregisterOnEndListener(Action listener) => this.onEnd -= listener;
    
    public void RegisterOnPlayingListener(Action<double> listener)
    {
        if (listener == null) return;
        this.onPlaying += listener;
    }
    public void UnregisterOnPlayingListener(Action<double> listener) => this.onPlaying -= listener;

    public static string GetSecondsInTimeFormatted(double time)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        return $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    }
        
    public string GetCurrentTimeFormatted(bool countDown = false) => countDown ? GetSecondsInTimeFormatted(this.maxTime - this.currentTime) : GetSecondsInTimeFormatted(this.currentTime);

    public void ResetWaitTime(float waitTime) => this.maxTime = waitTime;
}