using Godot;
using System;

public partial class menu : Node
{
    //BUTTONS
    private static void OnStartPressed()
    {
        scenehandle.Instance.ChangeScene("res://tscn/modeselect.tscn");
    }

    private static void OnOptionsPressed()
    {
        scenehandle.Instance.ChangeScene("res://tscn/options.tscn");
    }

    private void OnExitPressed()
    {
        GetTree().Quit();
    }
}