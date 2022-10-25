using Godot;
using System;

public class KeyBindingControl : HBoxContainer
{
    [Signal]
    public delegate void EditButtonPressed();
    [Export]
    public string ActionName 
    {
        get { return _actionName; } 
        set { SetActionName(value); }
    }

    private string _actionName;

    [Export]
    public uint ScanCode
    {
        get { return _scanCode; } 
        set { SetScanCode(value); }
    }
    private uint _scanCode;

    public void SetActionName(string value)
    {
        _actionName = value;
        var node = GetNodeOrNull<Label>("ActionLabel");

        if (node == null)
            return;
        
        node.Text = $"{_actionName}: ";
    }

    public void SetScanCode(uint value)
    {
        _scanCode = value;
        var node = GetNodeOrNull<Label>("AssignedKeyLabel");

        if (node == null)
            return;
        
        node.Text = OS.GetScancodeString(_scanCode);
    }

    public void OnEditButtonPressed()
    {
        EmitSignal(nameof(EditButtonPressed));
    }
}
