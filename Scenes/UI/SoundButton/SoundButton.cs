using Godot;
using System;

public class SoundButton : Button
{
    public override void _Pressed()
    {
        GetNode<AudioStreamPlayer>("AudioStreamPlayers/ClickSound").Play();
    }

    public void OnSoundButtonMouseEntered()
    {
        if (Disabled)
            return;
        
        GetNode<AudioStreamPlayer>("AudioStreamPlayers/HoverSound").Play();
    }
}
