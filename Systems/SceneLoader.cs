using Godot;
using System;

public class SceneLoader : Node
{
    private PackedScene _loadingScreen = (PackedScene)GD.Load("res://Scenes/LoadingScreen/LoadingScreen.tscn");
    public ResourceInteractiveLoader Loader;
    public string SceneToLoad;

    public static SceneLoader Instance;

    public override void _Ready()
    {
        Instance = this;
    }

    public void ReloadCurrentScene()
    {
        LoadScene(SceneToLoad);
    }

    public void LoadScene(string path)
    {
        if (string.IsNullOrEmpty(path))
            return;
        
        if (SceneToLoad != path || Loader == null)
        {
            SceneToLoad = path;
            Loader = ResourceLoader.LoadInteractive(SceneToLoad);
        }

        GetTree().Paused = false;

        Error err = GetTree().ChangeSceneTo(_loadingScreen);

        if (err != Error.Ok)
        {
            GD.PrintErr($"Failed to load loading screen: {err}");
            GetTree().Quit();
        }
    }
}
