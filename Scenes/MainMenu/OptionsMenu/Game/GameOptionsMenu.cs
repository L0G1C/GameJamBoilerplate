using Godot;
using System;

public class GameOptionsMenu : CenterContainer
{
    private GameLog _gameLog;

    public override void _Ready()
    {
        //Connect("ResetConfirmed", this, "OnResetGameControlResetConfirmed");
        _gameLog = new GameLog();
    }
    public void OnResetGameControlResetConfirmed()
    {
        _gameLog.ResetGameData();
    }
}
