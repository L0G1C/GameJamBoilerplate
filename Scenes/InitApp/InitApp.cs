using Godot;
using System;

public class InitApp : Node
{
    [Export(PropertyHint.File, "*.tscn")]
    public string NextScene;

    public override void _Ready()
    {
        AppSettings.Instance.InitializeFromConfig();
        SceneLoader.Instance.LoadScene(NextScene);
    }

}
