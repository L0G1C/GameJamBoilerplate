using Godot;
using System;

public class AppLogger : Node
{
    const string TOTAL_RUN_TIME = "TotalRunTime";
    const float UPDATE_COUNTER_RESET = 3.0f;

    private float _totalRunTime = 0.0f;
    private float _updateCounter = 0.0f;

    // public AppLogger()
    // {
        
    // }

    public override void _Ready()
    {
        SetLocalFromConfig();
        AppLog.Instance.AppOpened();
    }

    public override void _Process(float delta)
    {
        _totalRunTime += delta;
        _updateCounter += delta;

        if (_updateCounter > UPDATE_COUNTER_RESET)
        {
            _updateCounter = 0.0f;
            Config.Instance.SetConfig("AppLog", TOTAL_RUN_TIME, _totalRunTime);
        }
    }

    public void SetLocalFromConfig()
    {
        _totalRunTime = (float)Config.Instance.GetConfig("AppLog", TOTAL_RUN_TIME, _totalRunTime);
    }

}
