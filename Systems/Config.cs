using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Config : Node
{
    public static Config Instance;

    const string CONFIG_FILE_LOCATION = "user://config.cfg";
    const string DEFAULT_CONFIG_FILE_LOCATION = "res://default_config.cfg";

    private ConfigFile _configFile;

    // Init
    public Config()
    {
        LoadConfigFile();
    }

    public override void _Ready()
    {
        Instance = this;    
    }

    private void LoadConfigFile()
    {
        if (_configFile != null)
            return;
        
        _configFile = new ConfigFile();
        var loadError = _configFile.Load(CONFIG_FILE_LOCATION);
        if (loadError != Error.Ok)        
        {
            var loadDefaultConfigError = _configFile.Load(DEFAULT_CONFIG_FILE_LOCATION);
            if(loadDefaultConfigError != Error.Ok)
                GD.PrintErr($"Loading default config file faile with error{loadDefaultConfigError}");
            
            var saveError = _configFile.Save(CONFIG_FILE_LOCATION);
            if (saveError != Error.Ok)
                GD.PrintErr($"Saving config file failed with error {saveError}");
        }
    }

    public void SetConfig(string section, string key, object value)
    {
        LoadConfigFile();
        _configFile.SetValue(section, key, value);

        var saveError = _configFile.Save(CONFIG_FILE_LOCATION);
        if (saveError != Error.Ok)
            GD.PrintErr($"Saving config file failed with error {saveError}");
    }

    public object GetConfig(string section, string key, object value)
    {
        LoadConfigFile();
        return _configFile.GetValue(section, key, value);
    }

    public bool HasSection(string section)
    {
        LoadConfigFile();
        return _configFile.HasSection(section);
    }

    public List<string> GetSectionKeys(string section)
    {
        LoadConfigFile();
        return _configFile.GetSectionKeys(section).ToList();
    }
}
