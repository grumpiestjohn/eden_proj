using Godot;
using System;

public partial class modeselect : Node
{
    private static void OnExitPressed()
    {
        scenehandle.Instance.ChangeScene("res://tscn/menu.tscn");
    }
}
