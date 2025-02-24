using Godot;
using System;

public partial class scenehandle : Node
{
    public static scenehandle Instance { get; private set; }
    private PackedScene newscene;
    CanvasModulate brightness;
    Tween fadeTween;
    Color InColor = new Color(0, 0, 0, 1);  // Fade to black
    Color OutColor = new Color(1, 1, 1, 1); // Fade to white

    public override void _Ready()
    {
        if (Instance == null) Instance = this;
        FadeInOut(scenename, false, 2);
    }

    public async void ChangeScene(string scenename, bool transition = true, float sec = 2)
    {
        if (transition)
        {
            GD.Print("working 1");
            FadeInOut(scenename, true, sec);  // Fade in
            await ToSignal(fadeTween, "finished"); // Wait for fade to finish
        }
        else GetTree().ChangeSceneToFile(scenename);

        if (transition)
        {
            GD.Print("working 2");
            FadeInOut(scenename, false, sec);  // Fade out
            await ToSignal(fadeTween, "finished"); // Wait for fade to finish
        }
    }

    public void FadeInOut(string scenename, bool In = true, float sec = 2)
    {
        var scene = GetTree().CurrentScene;
        if (In) brightness = scene.GetNode("brightness") as CanvasModulate;
        else brightness = scene.GetNode("brightness") as CanvasModulate;
        fadeTween = CreateTween();
        fadeTween.SetTrans(Tween.TransitionType.Sine);
        fadeTween.SetEase(Tween.EaseType.InOut);

        if (In)
        {
            brightness.Color = OutColor;
            fadeTween.TweenProperty(brightness, "color", InColor, sec);
        }
        else
        {
            brightness.Color = InColor;
            fadeTween.TweenProperty(brightness, "color", OutColor, sec);
        }

        fadeTween.Play();
    }
}
