using System;

public class Timer
{
    private double currentTime = 0;
    private float maxTime = 0;
    public Action onEnd;
    public Action<double> onPlaying;
    public bool running;

    public Timer(float maxTime, Action onEnd)
    {
        this.maxTime = maxTime;
        this.onEnd = onEnd;
        this.running = true;
    }
        
    public Timer(float maxTime)
    {
        this.maxTime = maxTime;
        this.running = true;
    }
        
    public void Reset()
    {
        this.currentTime = 0;
        this.running = true;
    }

    public void Update(double dt)
    {
        if (!this.running)
            return;
        this.currentTime += dt;
        this.onPlaying?.Invoke(this.currentTime);
        CheckIfEndIsReached();
    }

    public void CheckIfEndIsReached()
    {
        if (!(this.currentTime >= this.maxTime)) return;
        this.onEnd?.Invoke();
        this.running = false;
    }
    
    private string GetFormattedTime(double time)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        return $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    }
        
    public string GetFormattedTime(bool countDown = false) => countDown ? GetFormattedTime(this.maxTime - this.currentTime) : GetFormattedTime(this.currentTime);

    public double GetCurrentTime() => this.currentTime;
    public void Add(float time) => this.maxTime += time;
}