using Godot;
using System;
using System.Collections.Generic;

public class AppSettings : Node
{
    public static AppSettings Instance;

    const string INPUT_SECTION = "InputSettings";
    const string AUDIO_SECTION = "AudioSettings";
    const string VIDEO_SECTION = "VideoSettings";

    const string FULLSCREEN_ENABLED = "FullscreenEnabled";
    const string MASTER_AUDIO_BUS = "Master";
    const string VOICE_AUDIO_BUS = "Voice";
    const string SFX_AUDIO_BUS = "SFX";
    const string MUSIC_AUDIO_BUS = "Music";
    const string MUTE_SETTING = "Mute";
    public Dictionary<string, string> INPUT_MAP = new Dictionary<string, string>(){
        {"move_forward", "Forward"},
        {"move_backward", "Backward"},
        {"move_left", "Left"},
        {"move_right", "Right"},
        {"run", "Run"},
        {"jump", "Jump"},
        {"interact", "Interact"}
    };

    public override void _Ready()
    {
        Instance = this;
    }

    # region Input Methods
    public uint GetActionScancode(string actionName, object defaultValue = null)
    {
        return Convert.ToUInt32(Config.Instance.GetConfig(INPUT_SECTION, actionName, defaultValue));
    }

    public void SetActionScancode(string actionName, uint scanCode)
    {
        Config.Instance.SetConfig(INPUT_SECTION, actionName, scanCode);
    }

    public List<string> GetInputActions()
    {
        return Config.Instance.GetSectionKeys(INPUT_SECTION);
    }

    public uint GetInputEventScancode(InputEventKey actionEvent)
    {
        if (actionEvent.Scancode != 0)
            return actionEvent.GetScancodeWithModifiers();
        else
            return OS.KeyboardGetScancodeFromPhysical(actionEvent.GetScancodeWithModifiers());
    }

    public void ResetInputConfig()
    {
        foreach(string actionName in INPUT_MAP.Keys)
        {
            var actionEventsArray = InputMap.GetActionList(actionName);
            
            if (actionEventsArray.Count == 0)
                return;

            InputEventWithModifiers actionEvent = (InputEventWithModifiers)actionEventsArray[0];

            if (actionEvent is InputEventKey)
            {
                SetActionScancode(actionName, GetInputEventScancode((InputEventKey)actionEvent));
            }
        }
    }

    public void SetInputsFromConfig()
    {
        foreach (string actionName in GetInputActions())
        {
            uint scanCode = GetActionScancode(actionName);
            var eventKey = new InputEventKey();

            foreach (var oldEvent in InputMap.GetActionList(actionName))
            {
                if (oldEvent is InputEventKey)
                    InputMap.ActionEraseEvent(actionName, (InputEvent)oldEvent);
                
                InputMap.ActionAddEvent(actionName, (InputEvent)oldEvent);
            }

        }
    }

    public void InitInputConfig()
    {
        if (!Config.Instance.HasSection(INPUT_SECTION))
        {
            ResetInputConfig();            
        }
        SetInputsFromConfig();
    }

    # endregion

    # region Audio Methods

        public float GetBusVolume(string busName)
        {
            int busIndex = AudioServer.GetBusIndex(busName);
            if (busIndex < 0)
                return 0.0f;
            
            float volumeDb = AudioServer.GetBusVolumeDb(busIndex);

            return GD.Db2Linear(volumeDb);
        }
        
        public void SetBusVolume(string busName, float linear)
        {
            int busIndex = AudioServer.GetBusIndex(busName);
            if (busIndex < 0)
                return;
            
            var volumeDb = GD.Linear2Db(linear);
            AudioServer.SetBusVolumeDb(busIndex, volumeDb);
            Config.Instance.SetConfig(AUDIO_SECTION, busName, linear);
        }

        public bool IsMuted()
        {
            int busIndex = AudioServer.GetBusIndex(MASTER_AUDIO_BUS);
            return AudioServer.IsBusMute(busIndex);
        }

        public void SetMute(bool muteFlag)
        {
            int busIndex = AudioServer.GetBusIndex(MASTER_AUDIO_BUS);
            AudioServer.SetBusMute(busIndex, muteFlag);
            Config.Instance.SetConfig(AUDIO_SECTION, MUTE_SETTING, muteFlag);
        }

        public void ResetAudioConfig()
        {
            Config.Instance.SetConfig(AUDIO_SECTION, MASTER_AUDIO_BUS, GetBusVolume(MASTER_AUDIO_BUS));
            Config.Instance.SetConfig(AUDIO_SECTION, SFX_AUDIO_BUS, GetBusVolume(SFX_AUDIO_BUS));
            Config.Instance.SetConfig(AUDIO_SECTION, VOICE_AUDIO_BUS, GetBusVolume(VOICE_AUDIO_BUS));
            Config.Instance.SetConfig(AUDIO_SECTION, MUSIC_AUDIO_BUS, GetBusVolume(MUSIC_AUDIO_BUS));
            Config.Instance.SetConfig(AUDIO_SECTION, MUTE_SETTING, IsMuted());
        }

        public void SetAudioFromConfig()
        {
            float masterAudioValue = GetBusVolume(MASTER_AUDIO_BUS);
            float sfxAudioValue = GetBusVolume(MASTER_AUDIO_BUS);
            float voiceAudioValue  = GetBusVolume(MASTER_AUDIO_BUS);
            float musicAudioValue  = GetBusVolume(MASTER_AUDIO_BUS);
            bool muteAudioFlag = IsMuted();

            masterAudioValue = (float)Config.Instance.GetConfig(AUDIO_SECTION, MASTER_AUDIO_BUS, masterAudioValue);
            sfxAudioValue = (float)Config.Instance.GetConfig(AUDIO_SECTION, SFX_AUDIO_BUS, sfxAudioValue);
            voiceAudioValue = (float)Config.Instance.GetConfig(AUDIO_SECTION, VOICE_AUDIO_BUS, voiceAudioValue);
            musicAudioValue = (float)Config.Instance.GetConfig(AUDIO_SECTION, MUSIC_AUDIO_BUS, musicAudioValue);
            muteAudioFlag = (bool)Config.Instance.GetConfig(AUDIO_SECTION, MUTE_SETTING, muteAudioFlag);

            SetBusVolume(MASTER_AUDIO_BUS, masterAudioValue);
            SetBusVolume(SFX_AUDIO_BUS, sfxAudioValue);
            SetBusVolume(VOICE_AUDIO_BUS, voiceAudioValue);
            SetBusVolume(MUSIC_AUDIO_BUS, musicAudioValue);
            SetMute(muteAudioFlag);
        }

        public void InitAudioConfig()
        {
            if (!Config.Instance.HasSection(AUDIO_SECTION))
            {
                ResetAudioConfig();
            }

            SetAudioFromConfig();
        }

    # endregion

    # region Video Config

    # endregion

    public void InitializeFromConfig()
    {
        InitInputConfig();
        InitAudioConfig();
        // InitVideoConfig();
    }
}
