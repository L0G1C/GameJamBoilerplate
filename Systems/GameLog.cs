using Godot;
using System;

public class GameLog : Node
{
    const string GAME_LOG_SECTION = "GameLog";
    const string TOTAL_GAMES_STARTED = "TotalGamesStarted";
    const string MAX_LEVEL_REACHED = "MaxLevelReached";
    
    public void GameStarted()
    {
        var TotalGamesStarted = (int)Config.Instance.GetConfig(GAME_LOG_SECTION, TOTAL_GAMES_STARTED, 0);
        TotalGamesStarted += 1;
        Config.Instance.SetConfig(GAME_LOG_SECTION, TOTAL_GAMES_STARTED, TotalGamesStarted);
    }

    public void LevelReached(int levelNumber) 
    {
        var MaxLevelReached = (int)Config.Instance.GetConfig(GAME_LOG_SECTION, MAX_LEVEL_REACHED, 0);
        MaxLevelReached = Math.Max(levelNumber, MaxLevelReached);   
        Config.Instance.SetConfig(GAME_LOG_SECTION, MAX_LEVEL_REACHED, MaxLevelReached);
    }

    public void ResetGameData() 
    {
        Config.Instance.SetConfig(GAME_LOG_SECTION, MAX_LEVEL_REACHED, 0);
    }
}
