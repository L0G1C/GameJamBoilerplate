using Godot;
using System;

public class AudioOptionsMenu : CenterContainer
{
    private HSlider _masterSlider;
    private HSlider _sfxSlider;
    private HSlider _voiceSlider;
    private HSlider _musicSlider;
    private CheckButton _muteButton;
    private bool playAudioStreams;

    public override void _Ready()
    {
        _masterSlider = GetNode<HSlider>("VBoxContainer/MasterControl/MasterHSlider");
        _sfxSlider = GetNode<HSlider>("VBoxContainer/SFXControl/SFXHSlider");
        _voiceSlider = GetNode<HSlider>("VBoxContainer/VoiceControl/VoiceHSlider");
        _musicSlider = GetNode<HSlider>("VBoxContainer/MusicControl/MusicHSlider");
        _muteButton = GetNode<CheckButton>("VBoxContainer/MuteControl/MuteButton");

        AppSettings.Instance.InitAudioConfig();
        UpdateUI();
    }

    public override void _UnhandledKeyInput(InputEventKey @event)
    {
        if (@event.IsActionReleased("ui_mute"))
            _muteButton.Pressed = !(_muteButton.Pressed);
    }

    // TODO - Not needed?
    private void PlayNextAudioStream(Node streamParent)
    {        
    }

    private void PlayNextVocalAudioStream()
    {
        PlayNextAudioStream(GetNode<Node>("VocalAudioStreamPlayers"));
    }
    private void PlayNextSFXAudioStream()
    {
        PlayNextAudioStream(GetNode<Node>("SFXAudioStreamPlayers"));
    }

    private void UpdateUI()
    {
        _masterSlider.Value = AppSettings.Instance.GetBusVolume("Master");        
        _sfxSlider.Value = AppSettings.Instance.GetBusVolume("SFX");
        _voiceSlider.Value = AppSettings.Instance.GetBusVolume("Voice");
        _musicSlider.Value = AppSettings.Instance.GetBusVolume("Music");
        _muteButton.Pressed = AppSettings.Instance.IsMuted();
    }

    public void OnMasterHSliderValueChanged(float value)
    {
        AppSettings.Instance.SetBusVolume("Master", value);
    }
    public void OnSFXHSliderValueChanged(float value)
    {
        AppSettings.Instance.SetBusVolume("SFX", value);
        PlayNextSFXAudioStream();
    }
    public void OnVoiceHSliderValueChanged(float value)
    {
        AppSettings.Instance.SetBusVolume("Voice", value);
        PlayNextVocalAudioStream();
    }
    public void OnMusicHSliderValueChanged(float value)
    {
        AppSettings.Instance.SetBusVolume("Music", value);
    }
    public void OnMuteButtonToggled(bool buttonPressed)
    {
        AppSettings.Instance.SetMute(buttonPressed);
    }
}
