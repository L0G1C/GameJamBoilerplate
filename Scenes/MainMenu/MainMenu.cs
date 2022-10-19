using Godot;
using System;

public class MainMenu : Control
{
    [Export(PropertyHint.File, "*.tscn")]
    public string GameScene;

    [Export]
    public string VersionName = "0.0.0";

    private AnimationNodeStateMachinePlayback _animationStateMachine;
    private Control _subMenu;

    public override void _Ready()
    {
        SetupForWeb();
        SetupVersionName();
        _animationStateMachine = (AnimationNodeStateMachinePlayback)GetNode<AnimationTree>("MenuAnimationTree").Get("parameters/playback");
    }

    public void LoadScene(string scenePath)
    {
        SceneLoader.Instance.LoadScene(scenePath);
    }

    public override void _Input(InputEvent @event)
    {
        if (_animationStateMachine.GetCurrentNode() == "Intro" &&            
            (@event is InputEventMouseButton || @event is InputEventKey))      
            IntroDone();
    }

    public void PlayGame()
    {
        //GameLog.GameStarted();
        SceneLoader.Instance.LoadScene(GameScene);
    }

    public void OpenSubMenu(Control menu)
    {
        menu.Visible = true;
        menu.SetProcess(true);
        _subMenu = menu;
        _animationStateMachine.Travel("OpenSubMenu");
    }

    public void CloseSubMenu()
    {
        if (_subMenu == null)
            return;
        
        _animationStateMachine.Travel("MainMenuOpen");
        _subMenu.Visible = false;
        _subMenu.SetProcess(false);
        _subMenu = null;
        _animationStateMachine.Travel("MainMenuOpen");
    }

    public void OnPlayButtonPressed()
    {
        PlayGame();
    }
	

    public void OnTutorialButtonPressed()
    {
        return;
    }        

    public void OnOptionsButtonPressed()
    {
        OpenSubMenu(GetNode<Control>("MasterOptionsMenu"));
    }        

    public void OnCreditsButtonPressed()
    {
        var credits = GetNode<Control>("CreditsContainer/Credits");
        OpenSubMenu(credits);
        //credits.reset()
    }

    public void OnExitButtonPressed()
    {
        GetTree().Quit();
    }        

    public void OnBackButtonPressed()
    {
        CloseSubMenu();
    } 

    public void IntroDone()
    {
        GetNode<AnimationTree>("MenuAnimationTree").Set("parameters/conditions/IntroDone", true);
    }

    public void SetupForWeb()
    {
        if (OS.HasFeature("web"))
            GetNode<Button>("MarginContainer/Main/buttonContainer/Exit").Disabled = true;
    }

    public void SetupVersionName()
    {
        //AppLogger.VersionOpened(VersionName);
        GetNode<Label>("%VersionNameLabel").Text = $"v{VersionName}";
    }

    public void OnCreditsEndReached()
    {
        CloseSubMenu();
    }

}
