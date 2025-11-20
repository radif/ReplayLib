using System.Diagnostics;

public class ReplayTimer
{
    Stopwatch _stopWatch = new Stopwatch();
    bool _isTicking = false;
    bool _paused = false;
    public void StartTimer()
    {
        _stopWatch.Restart();
        _isTicking = true;
        _paused = false;
    }
    public void ResetTimer()
    {
        _stopWatch.Restart();
        _isTicking = true;
        _paused = false;
    }

    public void StopTimer()
    {
        _stopWatch.Stop();
        _isTicking = false;
        _paused = false;
    }

    public bool isTicking => _isTicking;
    
    public bool paused
    {
        get => _paused;
        set
        {
            if (_paused != value)
            {
                _paused = value;
                if (_paused)
                    _stopWatch.Stop();
                else if (_isTicking)
                    _stopWatch.Start();
            }
        }
    }
    public long elapsedMilliseconds => _stopWatch.ElapsedMilliseconds;
    public double elapsedSeconds => elapsedMilliseconds / 1000F;
    
    public double GetCountdownSeconds(double duration)
    {
        double retVal = duration - elapsedSeconds;

        if (retVal < 0F)
            retVal = 0F;

        return retVal;
    }

    public double GetElapsedTimeSeconds()
        => elapsedSeconds;
    
    public bool GetIsExpired(double duration) => GetCountdownSeconds(duration) <= 1F;
}
