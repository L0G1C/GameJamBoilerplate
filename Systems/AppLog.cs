using Godot;
using System;

public class AppLog : Node
{
    const string APP_LOG_SECTION = "AppLog";
    const string APP_OPENED = "AppOpened";
    const string FIRST_VERSION_OPENED = "FirstVersionOpened";
    const string LAST_VERSION_OPENED = "LastVersionOpened";
    const string UNKNOWN_VERSION = "unknown";

    public static AppLog Instance;

    public override void _Ready()
    {
        Instance = this;
    }

    public void AppOpened()
    {
        var totalAppOpened = (int)Config.Instance.GetConfig(APP_LOG_SECTION, APP_OPENED, 0);
        totalAppOpened += 1;
        Config.Instance.SetConfig(APP_LOG_SECTION, APP_OPENED, totalAppOpened);
    }

    public void SetFirstVersionOpened(string value)
    {
        var firstVersionOpened = (string)Config.Instance.GetConfig(APP_LOG_SECTION, FIRST_VERSION_OPENED, UNKNOWN_VERSION);
        if (firstVersionOpened != UNKNOWN_VERSION)
            return;
        Config.Instance.SetConfig(APP_LOG_SECTION, FIRST_VERSION_OPENED, value);
    }

    public void SetLastVersionOpened(string value)
    {
        Config.Instance.SetConfig(APP_LOG_SECTION, LAST_VERSION_OPENED, value);
    }

    public void VersionOpened(string value)
    {
        SetFirstVersionOpened(value);
        SetLastVersionOpened(value);
    }
}
