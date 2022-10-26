using Godot;
using System;

public class VideoOptionsMenu : CenterContainer
{
    const string FULLSCREEN_ENABLED = "FullscreenEnabled";
    const string VIDEO_SECTION = "VideoSettings";

    private CheckButton _fullscreenButton;
    public override void _Ready()
    {
        _fullscreenButton = GetNode<CheckButton>("VBoxContainer/FullscreenControl/FullscreenButton");
        AppSettings.Instance.InitVideoConfig();
        UpdateUI();
    }

    private void UpdateUI()
    {
        _fullscreenButton.Pressed = OS.WindowFullscreen;
    }

    public void OnFullscreenButtonToggled(bool buttonPressed)
    {
        AppSettings.Instance.SetFullscreenEnabled(buttonPressed);
    }
}
