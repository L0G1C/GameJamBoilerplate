using Godot;
using System;

public class LoadingScreen : CanvasLayer
{
    const string LOADING_COMPLETE_TEXT = "Loading Complete!";

    public void SetNewScene(PackedScene sceneResource)
    {
        Node sceneInstance = sceneResource.Instance();
        Error err = sceneInstance.Connect("ready", this, "queue_free");

        if (err != Error.Ok )
            GD.PrintErr($"Error setting new scene: {err}");

        GetNode<Node>("/root").AddChild(sceneInstance);
        GetTree().CurrentScene = sceneInstance;
    }

    public override async void _Process(float delta)
    {
        Error err = SceneLoader.Instance.Loader.Poll();
        if (err == Error.Ok)
            GetNode<ProgressBar>("Control/VBoxContainer/ProgressBar").Value = SceneLoader.Instance.Loader.GetStage();
        else if (err == Error.FileEof)
        {
            GetNode<ProgressBar>("Control/VBoxContainer/ProgressBar").Value = 100;
            GetNode<Label>("Control/VBoxContainer/Title").Text = LOADING_COMPLETE_TEXT;
            SetProcess(false);
            await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
            Resource resource = SceneLoader.Instance.Loader.GetResource(); 
            SetNewScene((PackedScene)resource);
        }
        else
        {
            GetNode<AcceptDialog>("Control/ErrorMsg").DialogText = $"Loading Error: {err}";
            GetNode<AcceptDialog>("Control/ErrorMsg").PopupCentered();
            SetProcess(false);
        }
    }

    public void OnErrorMsgConfirmed()
    {
        Error err = GetTree().ChangeScene((string)ProjectSettings.GetSetting("application/run/main_scene"));
        if (err != Error.Ok)
        {
            GD.PrintErr($"Failed to load main scene: {err}");
            GetTree().Quit();
        }
    }
}
