using Godot;
using System;

public class ResetGameControl : HBoxContainer
{
    [Signal]
    public delegate void ResetConfirmed();

    public void OnResetButtonPressed()
    {
        GetNode<ConfirmationDialog>("ConfirmResetDialog").PopupCentered();
        GetNode<Button>("ResetButton").Disabled = true;
    }

    public void OnConfirmResetDialogConfirmed()
    {
        EmitSignal(nameof(ResetConfirmed));
    }

    public void OnConfirmResetDialogPopupHide()
    {
        GetNode<Button>("ResetButton").Disabled = false;
    }
}
