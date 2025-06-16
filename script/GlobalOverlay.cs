using Godot;
using System;

public partial class GlobalOverlay : Node
{
    public static GlobalOverlay Instance { get; private set; }
    private Node2D cursor;
    Color InColor = new(0, 0, 0, 1);
    Color OutColor = new(1, 1, 1, 1);
    public Tween tween;
    public bool TweenWorking = true;
    public bool TweenConnected = false;

    [Signal]
    public delegate void TweenFinishedEventHandler();

    public override void _Ready()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            QueueFree();
        }
        cursor = GetNode<Node2D>("cursor");
        var debugRect = new ColorRect();
    }
    public void FadeInOut(bool In, double duration)
    {
        if (TweenConnected == true)
        {
            tween.Finished -= OnTweenFinished;
            TweenConnected = false;
        }

        if (tween != null && tween.IsRunning())
        {
            tween.Kill();
        }
        tween = CreateTween();
        tween.SetParallel();
        tween.Finished += OnTweenFinished;
        TweenConnected = true;

        Color target = In ? InColor : OutColor;

        tween.TweenProperty(GetNode<CanvasModulate>("brightness"), "color", target, duration).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In);


        TweenWorking = true;
    }

    private void OnTweenFinished()
    {
        TweenWorking = false;
        EmitSignal("TweenFinished");
    }
    public override void _Process(double delta)
    {
        cursor.Position = GetViewport().GetMousePosition();
    }
}
