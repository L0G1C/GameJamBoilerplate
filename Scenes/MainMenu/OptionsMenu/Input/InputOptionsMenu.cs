using Godot;
using Array = Godot.Collections.Array;
using System;
using System.Collections.Generic;

public class InputOptionsMenu : CenterContainer
{
    private string _placeHolderText;
    private PackedScene _keyBindingControlScene = (PackedScene)GD.Load("res://Scenes/UI/KeyBindingControl/KeyBindingControl.tscn");
    private Dictionary<string, KeyBindingControl> _inputNodeMap;
    private string _editingActionName;
    private InputEventWithModifiers _lastInputEvent;
    public override void _Ready()
    {
        _placeHolderText = GetNode<ConfirmationDialog>("KeyAssignmentDialog").DialogText;
        _inputNodeMap = new Dictionary<string, KeyBindingControl>();
        AppSettings.Instance.SetInputsFromConfig();
        UpdateUI();
    }

    public override void _Input(InputEvent @event)
    {
        if (!@event.IsPressed() || !IsEditingKeyBinding())
            return;
        
        if (@event is InputEventKey)
        {
            var dialog = GetNode<ConfirmationDialog>("KeyAssignmentDialog");
            var eventKey = (InputEventKey)@event;
            _lastInputEvent = (InputEventWithModifiers)eventKey;
            dialog.DialogText = OS.GetScancodeString(eventKey.GetScancodeWithModifiers());
            dialog.GetOk().Disabled = false;
        }
    }

    public bool IsEditingKeyBinding()
    {
        return !string.IsNullOrEmpty(_editingActionName);
    }

    public void EditKey (string actionName)
    {
        _editingActionName = actionName;
        var dialog = GetNode<ConfirmationDialog>("KeyAssignmentDialog");
        dialog.WindowTitle = AppSettings.Instance.INPUT_MAP[actionName];
        dialog.DialogText = _placeHolderText;
        dialog.GetOk().Disabled = true;
        dialog.GetLabel().Align = Label.AlignEnum.Center;
        dialog.PopupCentered();
    }

    public void UpdateUI()
    {
        foreach (string actionName in AppSettings.Instance.INPUT_MAP.Keys)
        {
            var inputEvents = InputMap.GetActionList(actionName);
            GD.Print(InputMap.GetActionList(actionName));
            if (inputEvents.Count < 1)
            {
                GD.Print($"{actionName} is empty");
                continue;
            }

            InputEventKey inputEvent = (InputEventKey)inputEvents[0];
             GD.Print($"Input Event Key Name: {inputEvent.ResourceName}");
             GD.Print($"Input Event Scan Code Value: {inputEvent.Scancode}");
             GD.Print($"Input Event Physical Scan Code Value: {inputEvent.PhysicalScancode}");
            var readableName = AppSettings.Instance.INPUT_MAP[actionName];
            KeyBindingControl controlInstance;

            if (!_inputNodeMap.ContainsKey(actionName))
            {
                controlInstance = (KeyBindingControl)_keyBindingControlScene.Instance();
                controlInstance.ActionName = readableName;
                controlInstance.Connect("EditButtonPressed", this, "EditKey", new Array{actionName});
                _inputNodeMap[actionName] = controlInstance;
                GetNode<VBoxContainer>("VBoxContainer").AddChild(controlInstance);
            } 
            else
                controlInstance = (KeyBindingControl)_inputNodeMap[actionName];

            controlInstance.ScanCode = AppSettings.Instance.GetInputEventScancode(inputEvent);
        }
    }

    public void AssignKeyToAction(InputEventKey actionEvent, string actionName)
    {
        foreach (InputEventKey oldEvent in InputMap.GetActionList(actionName))
            InputMap.ActionEraseEvent(actionName, oldEvent);
        
        InputMap.ActionAddEvent(actionName, actionEvent);
        AppSettings.Instance.SetActionScancode(actionName, AppSettings.Instance.GetInputEventScancode(actionEvent));
        UpdateUI();        
    }

    public void OnKeyAssignmentDialogConfirmed()
    {
        AssignKeyToAction((InputEventKey)_lastInputEvent, _editingActionName);
        _editingActionName = "";
    }
}
